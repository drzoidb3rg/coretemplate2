using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template.Framework.Hypermedia;

namespace Template.Features.Home.Hypermedia
{
	public class HomeHypermedia : HydraClass
	{

		public HomeHypermedia()
		{
			
		}

		public HomeHypermedia(IContext ctx)
		{
			Id = ctx.Link<HomeHypermedia>().Id;
		}
    }
}
