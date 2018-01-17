using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Template.CustomErrorHandling
{
	public class TestErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;

		public TestErrorHandlingMiddleware(ILoggerFactory loggerFactory, RequestDelegate next)
		{
			_next = next;
			_logger = loggerFactory.CreateLogger<TestErrorHandlingMiddleware>();
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				var type = ex.GetType();
				_logger.LogError(ex.Message);

				if (context.Response.HasStarted)
				{
					_logger.LogWarning("The response has already started, the error page middleware will not be executed.");
				}
				throw;
			}
			finally
			{
				var statusCode = context.Response.StatusCode;

				if (statusCode == 400 && context.Request.ContentType != null && context.Request.ContentType.Contains("text/html"))
				{
					_logger.LogError(string.Format("Bad request {0}", context.Request.Path));
					context.Request.Path = "/badrequest";
					await _next(context);
				}

				if (statusCode == 404)
				{
					_logger.LogError(string.Format("Page not found {0}", context.Request.Path));
					context.Request.Path = "/notfound";
					await _next(context);
				}
			}
		}
	}
}
