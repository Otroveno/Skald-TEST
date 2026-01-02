using System;

namespace RadialCore.Core.Versioning
{
    /// <summary>
    /// Version sémantique (Major.Minor.Patch).
    /// Utilisé pour versioning des plugins et du Core.
    /// </summary>
    public struct Version : IComparable<Version>, IEquatable<Version>
    {
        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        public Version(int major, int minor, int patch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public static Version Parse(string versionString)
        {
            var parts = versionString.TrimStart('v', 'V').Split('.');
            if (parts.Length != 3)
                throw new ArgumentException($"Invalid version format: {versionString}");

            return new Version(
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                int.Parse(parts[2])
            );
        }

        public bool IsCompatibleWith(Version requiredVersion)
        {
            // Breaking change si Major différent
            if (Major != requiredVersion.Major)
                return false;

            // Minor/Patch backward compatible
            if (Minor < requiredVersion.Minor)
                return false;

            if (Minor == requiredVersion.Minor && Patch < requiredVersion.Patch)
                return false;

            return true;
        }

        public int CompareTo(Version other)
        {
            if (Major != other.Major) return Major.CompareTo(other.Major);
            if (Minor != other.Minor) return Minor.CompareTo(other.Minor);
            return Patch.CompareTo(other.Patch);
        }

        public bool Equals(Version other)
        {
            return Major == other.Major && Minor == other.Minor && Patch == other.Patch;
        }

        public override bool Equals(object? obj)
        {
            return obj is Version version && Equals(version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Major.GetHashCode();
                hash = hash * 23 + Minor.GetHashCode();
                hash = hash * 23 + Patch.GetHashCode();
                return hash;
            }
        }

        public override string ToString() => $"{Major}.{Minor}.{Patch}";

        public static bool operator ==(Version left, Version right) => left.Equals(right);
        public static bool operator !=(Version left, Version right) => !left.Equals(right);
        public static bool operator >(Version left, Version right) => left.CompareTo(right) > 0;
        public static bool operator <(Version left, Version right) => left.CompareTo(right) < 0;
        public static bool operator >=(Version left, Version right) => left.CompareTo(right) >= 0;
        public static bool operator <=(Version left, Version right) => left.CompareTo(right) <= 0;
    }
}
