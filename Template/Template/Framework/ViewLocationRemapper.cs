using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Template.Framework
{
	public class ViewLocationRemapper : IViewLocationExpander
	{
		private static readonly Lazy<IEnumerable<string>> Locations = new Lazy<IEnumerable<string>>(GetData);

		private static IEnumerable<string> GetData()
		{
			return new List<string>
			{
				"/Views/{1}/{0}.cshtml",
				"/Views/Shared/{0}.cshtml",
				"/Features/Home/Views/{0}.cshtml",
				"/Features/Signin/Views/{0}.cshtml"
			};
		}

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			// var temp = viewLocations.ToList();
			return Locations.Value;
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			// do nothing.. not entirely needed for this 
		}
	}
}
