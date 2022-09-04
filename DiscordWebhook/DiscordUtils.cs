using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Discord
{
    public class DiscordUtils
    {
        private DiscordUtils()
        {

        }

        /// <summary>
        /// Convert Color object into hex integer
        /// </summary>
        /// <param name="color">Color to be converted</param>
        /// <returns>Converted hex integer</returns>
        public static int ColorToHex(Color color)
        {
            string HS =
                color.R.ToString("X2") +
                color.G.ToString("X2") +
                color.B.ToString("X2");

            return int.Parse(HS, System.Globalization.NumberStyles.HexNumber);
        }

        internal static JObject StructToJson(object @struct)
        {
            Type type = @struct.GetType();
            JObject json = new();

            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
            {
                string name = FieldNameToJsonName(field.Name);
                object value = field.GetValue(@struct);
                if (value == null)
                    continue;

                if (value is bool)
                    json.Add(name, (bool)value);
                else if (value is int)
                    json.Add(name, (int)value);
                else if (value is Color)
                    json.Add(name, ColorToHex((Color)value));
                else if (value is string)
                    json.Add(name, value as string);
                else if (value is DateTime)
                    json.Add(name, ((DateTime)value).ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
                else if (value is Array)
                {
                    JArray array = new();
                    foreach (object obj in value as Array)
                        array.Add(StructToJson(obj));
                    json.Add(name, array);
                }
                else json.Add(name, StructToJson(value));
            }
            return json;
        }

        static readonly string[] ignore = { "InLine" };
        internal static string FieldNameToJsonName(string name)
        {
            if (ignore.ToList().Contains(name))
                return name.ToLower();

            List<char> result = new();

            if (IsFullUpper(name))
                result.AddRange(name.ToLower().ToCharArray());
            else
                for (int i = 0; i < name.Length; i++)
                {
                    if (i > 0 && char.IsUpper(name[i]))
                        result.AddRange(new[] { '_', char.ToLower(name[i]) });
                    else result.Add(char.ToLower(name[i]));
                }
            return string.Join("", result);
        }

        internal static bool IsFullUpper(string str)
        {
            bool upper = true;
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsUpper(str[i]))
                {
                    upper = false;
                    break;
                }
            }
            return upper;
        }
    }
}
