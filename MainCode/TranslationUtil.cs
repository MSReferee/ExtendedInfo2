using System;
using System.Linq;
using ColossalFramework.Plugins;
using ICities;

namespace IINS.ExtendedInfo.TranslationFramework
{
    public static class Util
    {
        public static string AssemblyPath
        {
            get { return PluginInfo.modPath; }
        }

        private static PluginManager.PluginInfo PluginInfo
        {
            get
            {
                var pluginManager = PluginManager.instance;
                var plugins = pluginManager.GetPluginsInfo();

                foreach (var item in plugins)
                {
                    try
                    {
                        var instances = item.GetInstances<IUserMod>();
                        if (!(instances.FirstOrDefault() is ExtendedInfoMod))
                        {
                            continue;
                        }
                        return item;
                    }
                    catch
                    {

                    }
                }
                throw new Exception("Failed to find ExtendedInfo Panel 2 assembly!");

            }
        }
    }
}