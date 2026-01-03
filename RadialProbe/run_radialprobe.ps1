param(
  [Parameter(Mandatory=$true)]
  [string]$BinPath,

  [string]$Keywords = "Quest,Journal,Map,Encyclopedia,Screen,GameState,Campaign,Clan,Kingdom",

  [string]$DllFilter = "TaleWorlds",

  [int]$Max = 300
)

Write-Host "Running RadialProbe on: $BinPath"
dotnet restore
dotnet run -- $BinPath --keywords $Keywords --dllFilter $DllFilter --max $Max

Write-Host "Done. Outputs:"
Write-Host " - radialprobe_report.json"
Write-Host " - radialprobe_report.txt"
