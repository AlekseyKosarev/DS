using System.Collections.Generic;

namespace DS.Utils
{
    public static class KeyNamingRules
    {
        //get key string for Type -> sometype_param1_param2
        public static string KeyFor<T>(params object[] args)
        {
            var typeName = typeof(T).Name;
            var parts = new List<string> { typeName };
        
            foreach (var arg in args)
            {
                if (arg != null)
                {
                    // Для чисел: добавляем все (включая отрицательные)
                    if (arg is int intArg)
                    {
                        parts.Add(intArg.ToString());
                    }
                    // Для строк: добавляем все непустые
                    else if (arg is string stringArg && !string.IsNullOrEmpty(stringArg))
                    {
                        parts.Add(stringArg);
                    }
                    // Для других типов: преобразуем в строку
                    else
                    {
                        parts.Add(arg.ToString());
                    }
                }
            }
            return string.Join("_", parts).ToLowerInvariant();
        }
    }
}