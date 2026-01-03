using System.Text;
using System.Text.Json;
using Mono.Cecil;
using Mono.Cecil.Cil;

static class Program
{
    // Usage:
    // dotnet run -- <pathToBin> --keywords Quest,Journal,Map,Encyclopedia,Screen,GameState --dllFilter TaleWorlds
    // Outputs:
    //  - radialprobe_report.json
    //  - radialprobe_report.txt
    public static int Main(string[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: RadialProbe <pathToBin> [--keywords a,b,c] [--dllFilter TaleWorlds] [--max 200]");
                return 2;
            }

            string binPath = args[0];
            var keywords = new List<string> { "Quest", "Journal", "Map", "Encyclopedia", "Screen", "GameState", "Campaign", "Clan", "Kingdom" };
            string dllFilter = "TaleWorlds";
            int maxResults = 250;

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] == "--keywords" && i + 1 < args.Length)
                {
                    keywords = args[i + 1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    i++;
                }
                else if (args[i] == "--dllFilter" && i + 1 < args.Length)
                {
                    dllFilter = args[i + 1];
                    i++;
                }
                else if (args[i] == "--max" && i + 1 < args.Length && int.TryParse(args[i + 1], out var m))
                {
                    maxResults = Math.Clamp(m, 50, 5000);
                    i++;
                }
            }

            if (!Directory.Exists(binPath))
            {
                Console.WriteLine($"ERROR: Directory not found: {binPath}");
                return 2;
            }

            var dlls = Directory.GetFiles(binPath, "*.dll", SearchOption.TopDirectoryOnly)
                .Where(p => Path.GetFileName(p).Contains(dllFilter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (dlls.Count == 0)
            {
                Console.WriteLine($"No DLLs found in {binPath} matching filter '{dllFilter}'.");
                return 1;
            }

            var report = new ProbeReport
            {
                BinPath = binPath,
                Keywords = keywords,
                DllFilter = dllFilter,
                GeneratedAtUtc = DateTime.UtcNow.ToString("o"),
                Assemblies = new List<AssemblyHit>()
            };

            int totalHits = 0;

            foreach (var dll in dlls)
            {
                var asmHit = ProbeAssembly(dll, keywords, maxResults - totalHits);
                if (asmHit is null) continue;

                if (asmHit.Hits.Count > 0)
                {
                    report.Assemblies.Add(asmHit);
                    totalHits += asmHit.Hits.Count;
                    if (totalHits >= maxResults) break;
                }
            }

            // Write JSON
            var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("radialprobe_report.json", json, Encoding.UTF8);

            // Write TXT
            File.WriteAllText("radialprobe_report.txt", RenderText(report), Encoding.UTF8);

            Console.WriteLine($"OK. Hits: {totalHits}. Output: radialprobe_report.json + radialprobe_report.txt");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("FATAL: " + ex);
            return 3;
        }
    }

    private static AssemblyHit? ProbeAssembly(string dllPath, List<string> keywords, int remainingBudget)
    {
        try
        {
            var rp = new ReaderParameters
            {
                ReadSymbols = false,
                InMemory = true
            };

            using var asm = AssemblyDefinition.ReadAssembly(dllPath, rp);

            var hits = new List<MethodHit>(capacity: 64);

            foreach (var module in asm.Modules)
            {
                foreach (var type in module.Types)
                {
                    ProbeTypeRecursive(type, keywords, hits, remainingBudget);
                    if (hits.Count >= remainingBudget) break;
                }
                if (hits.Count >= remainingBudget) break;
            }

            return new AssemblyHit
            {
                AssemblyName = asm.Name.Name,
                Path = dllPath,
                Hits = hits
            };
        }
        catch
        {
            // Fail-soft: ignore unreadable DLLs
            return null;
        }
    }

    private static void ProbeTypeRecursive(TypeDefinition type, List<string> keywords, List<MethodHit> hits, int remainingBudget)
    {
        if (hits.Count >= remainingBudget) return;

        // Heuristic: if type name matches keywords, we scan all public methods first
        bool typeNameMatch = keywords.Any(k =>
            type.FullName.Contains(k, StringComparison.OrdinalIgnoreCase) ||
            type.Name.Contains(k, StringComparison.OrdinalIgnoreCase));

        foreach (var method in type.Methods)
        {
            if (hits.Count >= remainingBudget) return;
            if (!method.HasBody) continue;

            // Score method
            var mh = ScoreMethod(type, method, keywords, typeNameMatch);
            if (mh is not null) hits.Add(mh);
        }

        // Nested types
        if (type.HasNestedTypes)
        {
            foreach (var nt in type.NestedTypes)
            {
                ProbeTypeRecursive(nt, keywords, hits, remainingBudget);
                if (hits.Count >= remainingBudget) return;
            }
        }
    }

    private static MethodHit? ScoreMethod(TypeDefinition type, MethodDefinition method, List<string> keywords, bool typeNameMatch)
    {
        int score = 0;
        var reasons = new List<string>(capacity: 6);

        // Public methods get a small boost (likely stable entrypoints)
        if (method.IsPublic)
        {
            score += 2;
            reasons.Add("public");
        }
        if (method.IsStatic)
        {
            score += 1;
            reasons.Add("static");
        }
        if (typeNameMatch)
        {
            score += 2;
            reasons.Add("typeNameMatch");
        }

        // Name matches
        foreach (var k in keywords)
        {
            if (method.Name.Contains(k, StringComparison.OrdinalIgnoreCase))
            {
                score += 2;
                reasons.Add($"methodName:{k}");
            }
        }

        // IL heuristics:
        // - string operands containing keywords (often UI labels / screen names)
        // - calls to types containing "ScreenManager" or "GameState" etc.
        bool hasInterestingIL = false;

        foreach (var ins in method.Body.Instructions)
        {
            if (ins.OpCode == OpCodes.Ldstr && ins.Operand is string s)
            {
                foreach (var k in keywords)
                {
                    if (s.Contains(k, StringComparison.OrdinalIgnoreCase))
                    {
                        score += 3;
                        reasons.Add($"ldstr:{k}");
                        hasInterestingIL = true;
                        break;
                    }
                }
            }
            else if ((ins.OpCode == OpCodes.Call || ins.OpCode == OpCodes.Callvirt) && ins.Operand is MethodReference mr)
            {
                var dt = mr.DeclaringType?.FullName ?? "";
                // Very conservative heuristics: we don't assume exact API names.
                if (dt.Contains("ScreenManager", StringComparison.OrdinalIgnoreCase) ||
                    dt.Contains("GameState", StringComparison.OrdinalIgnoreCase) ||
                    dt.Contains("Campaign", StringComparison.OrdinalIgnoreCase))
                {
                    score += 3;
                    reasons.Add($"call:{Trim(dt)}.{mr.Name}");
                    hasInterestingIL = true;
                }
            }
        }

        // If nothing matched at all, ignore
        if (score <= 2 && !hasInterestingIL && !typeNameMatch) return null;

        // Build signature
        string signature = BuildSignature(type, method);

        return new MethodHit
        {
            TypeFullName = type.FullName,
            MethodName = method.Name,
            Signature = signature,
            Score = score,
            Reasons = reasons.Distinct().Take(10).ToList()
        };
    }

    private static string BuildSignature(TypeDefinition type, MethodDefinition method)
    {
        var sb = new StringBuilder();
        sb.Append(type.FullName);
        sb.Append("::");
        sb.Append(method.Name);
        sb.Append("(");
        for (int i = 0; i < method.Parameters.Count; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(method.Parameters[i].ParameterType.FullName);
        }
        sb.Append(") : ");
        sb.Append(method.ReturnType.FullName);
        return sb.ToString();
    }

    private static string Trim(string s)
    {
        // keep last 80 chars for readability
        if (s.Length <= 80) return s;
        return "…" + s[^79..];
    }

    private static string RenderText(ProbeReport r)
    {
        var sb = new StringBuilder();
        sb.AppendLine("RADIALPROBE REPORT");
        sb.AppendLine($"BinPath: {r.BinPath}");
        sb.AppendLine($"DllFilter: {r.DllFilter}");
        sb.AppendLine($"Keywords: {string.Join(", ", r.Keywords)}");
        sb.AppendLine($"GeneratedAtUtc: {r.GeneratedAtUtc}");
        sb.AppendLine();

        foreach (var a in r.Assemblies)
        {
            sb.AppendLine($"=== {a.AssemblyName} ===");
            sb.AppendLine(a.Path);
            sb.AppendLine();

            foreach (var h in a.Hits
                         .OrderByDescending(x => x.Score)
                         .ThenBy(x => x.TypeFullName, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(x => x.MethodName, StringComparer.OrdinalIgnoreCase)
                         .Take(2000))
            {
                sb.AppendLine($"[{h.Score}] {h.Signature}");
                if (h.Reasons.Count > 0)
                    sb.AppendLine($"  reasons: {string.Join("; ", h.Reasons)}");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }

    private sealed class ProbeReport
    {
        public string BinPath { get; set; } = "";
        public string DllFilter { get; set; } = "";
        public List<string> Keywords { get; set; } = new();
        public string GeneratedAtUtc { get; set; } = "";
        public List<AssemblyHit> Assemblies { get; set; } = new();
    }

    private sealed class AssemblyHit
    {
        public string AssemblyName { get; set; } = "";
        public string Path { get; set; } = "";
        public List<MethodHit> Hits { get; set; } = new();
    }

    private sealed class MethodHit
    {
        public string TypeFullName { get; set; } = "";
        public string MethodName { get; set; } = "";
        public string Signature { get; set; } = "";
        public int Score { get; set; }
        public List<string> Reasons { get; set; } = new();
    }
}
