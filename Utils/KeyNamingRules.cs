using System.Collections.Generic;

namespace _Project.System.DS.Utils
{
    public static class KeyNamingRules
    {
        //get key string for Type -> sometype_param1_param2
        public static string KeyFor<T>(params object[] args)
        {
            var typeName = typeof(T).Name;
            var parts = new List<string>(){typeName};

            foreach (var arg in args)
            {
                if (arg != null)
                    parts.Add(arg.ToString());
            }
            return string.Join("_", parts).ToLowerInvariant();
        }
    }
}