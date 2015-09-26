using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Crawler.Model.Enum
{
    public static class EnumHelper
    {
        public static TEnum GetEnumVal<TEnum>(string strVal) where TEnum : struct, IConvertible
        {
            var enumList = ToEnumList<TEnum>();

            foreach (var enumVal in enumList)
            {
                var enumStr = GetStrVal(enumVal);
                if (enumStr == strVal) return enumVal;
            }

            throw new ArgumentException(String.Format("Invalid argument \"{0}\" not found in TEnum", strVal));
        }

        public static string GetStrVal<TEnum>(TEnum enumVal)
        {
            FieldInfo fi = enumVal.GetType().GetField(enumVal.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return enumVal.ToString();
        }

        public static IEnumerable<TEnum> ToEnumList<TEnum>() where TEnum : struct, IConvertible
        {
            Type enumType = typeof(TEnum);

            if (enumType.BaseType != typeof(System.Enum))
                throw new ArgumentException("TEnum must be of type System.Enum");

            Array enumValArray = System.Enum.GetValues(enumType);
            List<TEnum> enumValList = new List<TEnum>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((TEnum)System.Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }

        public static IEnumerable<string> ToStrList<TEnum>() where TEnum : struct, IConvertible
        {
            return ToEnumList<TEnum>().Select(enumVal => GetStrVal(enumVal)).ToList();
        }
    }
}