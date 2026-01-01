using System.Linq;
using TaleWorlds.MountAndBlade;

namespace Skald.Core.Utils
{
    public static class ModDetector
    {
        public static bool IsLoaded(string id)
        {
            return ModuleHelper.GetModules().Any(m => m.Id == id);
        }
    }
}