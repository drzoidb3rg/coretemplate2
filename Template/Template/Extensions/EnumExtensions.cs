using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Template.Extensions
{
	public static class EnumExtensions
	{
		public static List<KeyValuePair<int, string>> AskeyValuePairList<T>()
		{
			var ret = new List<KeyValuePair<int, string>>();
			var values = Enum.GetValues(typeof(T));

			for (int i = 0; i < values.Length; i++)
			{
				ret.Add(new KeyValuePair<int, string>(i, values.GetValue(i).ToString()));
			}

			return ret;
		}


		public static T ToEnum<T>(this string s)
		{
			return (T)Enum.Parse(typeof(T), s);
		}

		public static bool IsEnum<T>(this string s)
		{
			try
			{
				var e = s.ToEnum<T>();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public static T ParseEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value, true);
		}

		public static IEnumerable<(int Value, string Description)> AsValueDescriptionList<T>() where T : struct, IConvertible
		{
			int i = -1;
			foreach (var v in Enum.GetValues(typeof(T)))
			{
				i++;
				yield return (i, v.DisplayName());
			}

		}

		public static string DisplayName(this object item)
		{
			var type = item.GetType();
			var member = type.GetMember(item.ToString());
			DisplayAttribute displayName = (DisplayAttribute)member[0]
				.GetCustomAttributes(typeof(DisplayAttribute), false)
				.FirstOrDefault();

			if (displayName != null)
			{
				return displayName.Name;
			}

			return "";
		}

		public static string DisplayName(this Enum item)
		{
			var type = item.GetType();
			var member = type.GetMember(item.ToString());
			DisplayAttribute displayName = (DisplayAttribute)member[0]
				.GetCustomAttributes(typeof(DisplayAttribute), false)
				.FirstOrDefault();

			if (displayName != null)
			{
				return displayName.Name;
			}

			return item.ToString();
		}

	}
}
