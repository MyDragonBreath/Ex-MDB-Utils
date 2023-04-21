using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Linq;
using System.Reflection;

namespace ExMDBUtils
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public class Module : System.Attribute
    {
        public string ModuleName { get; set; }
        public Module(string name)
        {
            ModuleName = name;
        }
    }

    public abstract class BaseModule
    {
        abstract public void Enable(Plugin _plugin);
    }

    public class ModuleManager
    {
        Plugin eUtils;
        public ModuleManager(Plugin plugin)
        {
            eUtils = plugin;
            Setup();
        }


        void Setup()
        {
            var Modules = from type in Assembly.GetAssembly(typeof(Module)).GetTypes() where type.GetCustomAttributes(typeof(Module), true).Length > 0 select type;
            if (eUtils.Config.Debug) Log.Debug($"{Modules.Count()} modules");
            foreach (var x in Modules)
            {
                Module mod = ((Module)x.GetCustomAttribute(typeof(Module)));
                string ModuleName = mod.ModuleName;
                if (eUtils.Config.Debug) Log.Debug($"Found {ModuleName}");

                
                object settings = typeof(Config).GetProperty(ModuleName, BindingFlags.Public | BindingFlags.Instance).GetValue(eUtils.Config, null);
                if (eUtils.Config.Debug) Log.Debug($"Config {ModuleName}");
                bool enabled = ((bool)settings.GetType().GetProperty("IsEnabled", BindingFlags.Public | BindingFlags.Instance).GetValue(settings, null));
                if (eUtils.Config.Debug) Log.Debug($"Enable {ModuleName}: {enabled}");
                if (enabled)
                {
                    var enabled_module = (BaseModule)Activator.CreateInstance(x);
                    enabled_module.Enable(eUtils);
                    Log.Info($"ExUtils - {ModuleName} ENABLED");
                }
            }
            
        }
    }

}
