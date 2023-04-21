using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMDBUtils.Modules
{
    public static class ModulePasser
    {
        public static Dictionary<string, object> PrimaryObjectAllocation { get; private set; } = new Dictionary<string, object>();

        public static void AddObject(string name, object value) {
            PrimaryObjectAllocation[name] = value;
        }

        public static void RemoveObject(string name) { 
            PrimaryObjectAllocation[name] = null; 
        }

        public static object GetObject(string name)
        {
            if (!PrimaryObjectAllocation.ContainsKey(name)) return null;
            return PrimaryObjectAllocation[name];
        }
    }
}
