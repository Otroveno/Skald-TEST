using System;
using System.Reflection;
using TaleWorlds.MountAndBlade;

namespace Skald.Core.Utils
{
    public static class ModDetector
    {
        public static bool IsLoaded(string modId)
        {
            try
            {
                // Méthode par réflexion - plus robuste
                var moduleInfoType = Type.GetType("TaleWorlds.Core.ModuleInfo, TaleWorlds.Core");
                if (moduleInfoType != null)
                {
                    var getModulesMethod = moduleInfoType.GetMethod("GetModules", BindingFlags.Public | BindingFlags.Static);
                    if (getModulesMethod != null)
                    {
                        var modules = getModulesMethod.Invoke(null, null) as System.Collections.IEnumerable;
                        if (modules != null)
                        {
                            foreach (var module in modules)
                            {
                                var idProperty = module.GetType().GetProperty("Id");
                                if (idProperty != null && idProperty.GetValue(module) as string == modId)
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}