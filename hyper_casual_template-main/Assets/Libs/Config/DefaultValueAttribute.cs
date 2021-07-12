using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HyperCasualTemplate
{
    /// <summary>
    /// デフォルト値を属性として定義する
    /// 参考　C# > enumに文字列を割り当てる。 - Qiita https://qiita.com/sugasaki/items/ea5eec093ad7934abd5c
    /// </summary>
    public class DefaultValueAttribute : System.Attribute
    {
        public System.Object defaultValue;
        public int sortPriority;
        public string displayName;
        public DefaultValueAttribute(System.Object defaultValue, int sortPriority = 100, string displayName = "")
        {
            this.defaultValue = defaultValue;
            this.sortPriority = sortPriority;
            this.displayName = displayName;
        }
        public static T GetDefaultValue<T>(System.Enum value)
        {
            System.Type type = value.GetType();

            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            if (fieldInfo == null) return default(T);

            DefaultValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

            // Return the first if there was a match.
            return attribs?.Length > 0 
                ? (T)System.Convert.ChangeType(attribs[0].defaultValue, typeof(T)) 
                : default(T);
        }

        public static object GetDefaultValue(System.Enum value)
        {
            System.Type type = value.GetType();

            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            if (fieldInfo == null) return null;
            
            DefaultValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

            // Return the first if there was a match.
            return attribs?.Length > 0 
                ? attribs[0].defaultValue 
                : null;
        }

        public static int GetSortPriority(System.Enum value)
        {
            System.Type type = value.GetType();

            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            if (fieldInfo == null) return 0;
            
            DefaultValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

            // Return the first if there was a match.
            return attribs?.Length > 0 
                ? attribs[0].sortPriority 
                : 0;
        }
        
        public static string GetDisplayName(System.Enum value)
        {
            System.Type type = value.GetType();

            System.Reflection.FieldInfo fieldInfo = type.GetField(value.ToString());

            if (fieldInfo == null) return "";
            
            DefaultValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[];

            // Return the first if there was a match.
            return attribs?.Length > 0 
                ? attribs[0].displayName 
                : "";
        }
    }
}
