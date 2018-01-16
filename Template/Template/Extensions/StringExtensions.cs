using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Template.Extensions
{
	public static class StringExtensions
	{
		private static readonly Lazy<Regex> alphaNumericRegex = new Lazy<Regex>(() => new Regex("[^a-zA-Z0-9]"));

		public static bool IsEmpty(this string s)
		{
			return string.IsNullOrEmpty(s);
		}

		public static bool NotEmpty(this string s)
		{
			return !s.IsEmpty();
		}

		public static bool HasText(this string s)
		{
			if (s == null)
				return false;

			return s.Replace(" ", "").NotEmpty();
		}

		public static string ToNotNull(this string s)
		{
			if (s == null)
				return "";

			return s;
		}

		public static string CleanFreetext(this string s)
		{
			if (string.IsNullOrWhiteSpace(s)) return "";
			s = s.Replace("\t", " ");
			while (s.Contains("  "))
				s = s.Replace("  ", " ");

			return s.Trim();
		}

		public static IEnumerable<string> ToWordList(this string s)
		{
			return Regex.Replace(s.ToLowerInvariant(), "[^a-z0-9 ]", " ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Distinct();
		}

		public static bool IsDateTime(this string s)
		{
			DateTime date;
			return DateTime.TryParse(s, out date);
		}

		public static string HtmlEncode(this string s)
		{
			return WebUtility.HtmlEncode(s);
		}

		public static string UrlEncode(this string s)
		{
			return WebUtility.UrlEncode(s);
		}

		public static string UrlDecode(this string s)
		{
			return WebUtility.UrlDecode(s);
		}

		public static string ToHTMLId(this string s)
		{
			return Regex.Replace(s.TrimStart('/'), "[/|?|=]", "-");
		}


		public static DateTime ToDateTime(this string s)
		{
			if (s.IsEmpty() || !s.IsDateTime())
				return DateTime.MinValue;

			return DateTime.Parse(s);
		}

		public static bool IsInt(this string s)
		{
			int ret;
			return int.TryParse(s, out ret);
		}

		public static int ToInt(this string s)
		{
			if (string.IsNullOrEmpty(s))
				return 0;

			int ret;

			bool result = int.TryParse(s, out ret);

			if (result)
				return ret;

			return 0;
		}

		public static bool IsBool(this string s)
		{
			bool ret;
			return bool.TryParse(s, out ret);
		}

		public static bool ToBool(this string s)
		{
			if (string.IsNullOrWhiteSpace(s))
				return false;
			bool ret;
			if (bool.TryParse(s, out ret))
				return ret;
			return false;
		}

		public static string Truncate(this string s, int length)
		{
			return s.Substring(0, Math.Min(length, s.Length)) + "...";
		}

		public static string ToSimpleText(this string s)
		{
			return alphaNumericRegex.Value.Replace(s, "").ToLower();
		}

		public static string RemoveQueryString(this string str)
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;

			return str.Contains("?") ? str.Substring(0, str.LastIndexOf("?", StringComparison.Ordinal)) : str;
		}

		public static string AppendToQueryString(this string str, string key, string value)
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;

			return str.Contains("?") ? $"{str}&{key}={value}" : $"{str}?{key}={value}";
		}

		public static int ToIntId(this string s, string replaceText)
		{
			var ret = s.Replace(replaceText, "");
			return ret.ToInt();
		}

		public static string ToLuceneEncoded(this string s)
		{
			return s.Replace("/", "\\/");
		}

		public static string WithPlaceHolderIfEmpty(this string s, string placeholder = "-")
		{
			if (s.IsEmpty())
				return placeholder;
			return s;
		}
	}
}
