using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Template.Framework
{
	public class HttpVerbMiddleWare
	{
		private readonly RequestDelegate _next;

		public HttpVerbMiddleWare(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			if (context.Request.Method.ToLower() == "post" && context.Request.HasFormContentType)
			{
				if (context.Request.Form != null)
				{
					var hiddenMethod = context.Request.Form["_method"].FirstOrDefault();
					if (hiddenMethod != null)
						context.Request.Method = hiddenMethod.ToUpper();
				}
			}

			await _next.Invoke(context);
		}
	}
}
