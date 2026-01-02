namespace Skald.Core
{
    public static class SkaldContext
    {
        public static bool IsAIInfluenceEnabled { get; internal set; }
        public static string ActiveAIName { get; internal set; }

        public static void Reset()
        {
            IsAIInfluenceEnabled = false;
            ActiveAIName = "Native";
        }
    }
}