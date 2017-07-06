using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Tools.EnumHelper
{
    public class EnumHelper
    {
        public static string GetCustomEnumDesc(Type t, Enum e)
        {
            DescriptionAttribute da = null;
            FieldInfo fi;
            foreach (Enum type in Enum.GetValues(t))
            {
                fi = t.GetField((type.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (da != null && e.ToString() == type.ToString())
                    return da.Description;
            }
            return string.Empty;
        }

        public static Enum GetCustomEnum(Type t, string value)
        {
            Enum e = null;
            foreach (Enum type in Enum.GetValues(t))
            {
                if (type.ToString() == value)
                    e = type;
            }
            return e;
        }

        public static Dictionary<K, string> GetCustomEnums<K>(Type enumType) where K : struct
        {
            Dictionary<K, string> enums = new Dictionary<K, string>();

            DescriptionAttribute da = null;
            FieldInfo fi;
            foreach (Enum type in Enum.GetValues(enumType))
            {
                fi = enumType.GetField((type.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                var description = string.Empty;
                if (da != null)
                {
                    description = da.Description;
                }
                enums.Add((K)Convert.ChangeType(type, typeof(K)), description);
            }
            return enums;
        }


        public static Dictionary<string, string> GetCustomKeyEnums(Type enumType)
        {
            Dictionary<string, string> enums = new Dictionary<string, string>();

            DescriptionAttribute da = null;
            FieldInfo fi;
            foreach (Enum type in Enum.GetValues(enumType))
            {
                fi = enumType.GetField((type.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                var description = string.Empty;
                if (da != null)
                {
                    description = da.Description;
                }
                enums.Add(type.ToString(), description);
            }
            return enums;
        }


        public static Enum GetCustomEnumByDesc(Type t, string description)
        {
            Enum e = null;
            DescriptionAttribute da = null;
            FieldInfo fi;
            foreach (Enum type in Enum.GetValues(t))
            {
                fi = t.GetField((type.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if (da != null && description == da.Description)
                    e = type;
            }
            return e;
        }

        public static Dictionary<K, KeyValuePair<string, string>> GetEnumKeyValuesWithDescription<K>(Type enumType) where K : struct
        {
            Dictionary<K, KeyValuePair<string, string>> enums = new Dictionary<K, KeyValuePair<string, string>>();

            FieldInfo fi;
            DescriptionAttribute da = null;
            foreach (Enum type in Enum.GetValues(enumType))
            {
                fi = enumType.GetField((type.ToString()));
                da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                var description = string.Empty;
                if (da != null)
                {
                    description = da.Description;
                }
                enums.Add((K)Convert.ChangeType(type, typeof(K)), new KeyValuePair<string, string>(type.ToString(), description));
            }
            return enums;
        }
    }
}
