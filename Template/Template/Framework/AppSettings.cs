using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Template.Framework
{
	public class AppSettings
	{
		public string AuthKey { get; set; }
		public string RavenHost { get; set; }
		public string AuthUrl { get; set; }
		public string SystemAdmin { get; set; }
		public string ApplicationMode { get; set; }

		public AppSettings()
		{
		}

		public bool IsSystemAdmin(string email)
		{
			return !string.IsNullOrEmpty(email) && (SystemAdmin.ToLower().Contains(email.ToLower()));
		}
	}
}
