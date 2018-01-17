using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using NetEscapades.AspNetCore.SecurityHeaders;
using Raven.Client;
using Raven.Client.Document;
using Template.CustomErrorHandling;
using Template.Framework;

namespace Template
{
	public class ApplicationHostConfig
	{
		public IHostingEnvironment HostingEnvironment;
		public IConfigurationRoot ConfigurationRoot;
		public string ApplicationMode;

		public Action<IServiceCollection> AddRavenAction;

		public Action<IServiceCollection> AddWSClientAction;
		public Action<IServiceCollection> AddMachineLearningClientAction;
		public Action<IServiceCollection> AddUserPermissionsServiceAction;

		public Action<IServiceCollection> ConfigureServicesAction;

		public Func<IHostingEnvironment> HostingEnvironmentFunc;

		public Func<IHostingEnvironment, IConfigurationRoot> ConfigFunc;

		public List<Action<IApplicationBuilder, IConfigurationRoot>> ConfigureFirst;

		public Action<IApplicationBuilder, IConfigurationRoot> ConfigureLiveAction;

		public Action<IApplicationBuilder, IConfigurationRoot> ConfigureDevAction;

		public Action<IApplicationBuilder, IConfigurationRoot> ConfigureTestAction;

		public Action<IApplicationBuilder, IConfigurationRoot> ConfigureForModeAction;

		public Action<IApplicationBuilder, IConfigurationRoot> ConfigureCommonAction;

		public Action<IApplicationBuilder> ConfigureAction;

		public Action<ILoggerFactory> ConfigureLoggingAction;

		public Func<string> ContentRootPath;

		public ApplicationHostConfig()
		{
			ConfigureFirst = new List<Action<IApplicationBuilder, IConfigurationRoot>>();
			AddRavenAction = (sc) => DoRaven(sc, ConfigurationRoot);
			HostingEnvironmentFunc = () => GetHostingEnvironment(ContentRootPath);
			ConfigFunc = GetConfig;
			ConfigureCommonAction = ConfigureCommon;
			ConfigureServicesAction = (sc) => ConfigureServices(sc, ConfigurationRoot);
			ConfigureLiveAction = ConfigureLive;
			ConfigureTestAction = ConfigureTest;
			ConfigureDevAction = ConfigureDev;
			ContentRootPath = Directory.GetCurrentDirectory;
			ConfigureLoggingAction = ConfigureLogging;
		}


		public ApplicationHostConfig Initialise()
		{
			HostingEnvironment = HostingEnvironmentFunc();
			ConfigurationRoot = ConfigFunc(HostingEnvironment);

			ApplicationMode = ConfigurationRoot["AppSettings:ApplicationMode"];
			ConfigureAction = (sc) => ConfigureAll(sc, ConfigurationRoot);

			return this;
		}

		private static void ConfigureCommon(IApplicationBuilder app, IConfigurationRoot config)
		{
			app.UseAuthentication();

			app.UseStaticFiles();

			app.UseMiddleware<HttpVerbMiddleWare>();

			//Add security headers 
			var policyCollection = new HeaderPolicyCollection()
				.AddFrameOptionsSameOrigin()
				.AddXssProtectionBlock()
				.AddContentTypeOptionsNoSniff()
				.AddStrictTransportSecurityMaxAge()
				.AddReferrerPolicyNoReferrerWhenDowngrade()
				.RemoveServerHeader();
			//.AddCustomHeader("Content-Security-Policy", "script-src 'self'");

			app.UseCustomHeadersMiddleware(policyCollection);

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller=signin}/{action=Index}/{id?}");
			});

		}
		private static void AddPropertiesMIME(IApplicationBuilder app, IConfigurationRoot config)
		{
			try
			{//this is needed because when running tests, the path thing fails (prob because we're running all "in memory".
				//however, "AppSettings.ApplicationMode = Test" also when deploying to Test env, hence we still want to add the path thing in this second case.
				var provider = new FileExtensionContentTypeProvider();
				provider.Mappings[".properties"] = "text/plain";
				StaticFileOptions sfo = new StaticFileOptions();
				PhysicalFileProvider pfp = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\lib\pdfjs\web\locale"));
				sfo.FileProvider = pfp;
				sfo.ContentTypeProvider = provider;
				sfo.RequestPath = new PathString(@"/lib/pdfjs/web/locale");
				app.UseStaticFiles(sfo);
			}
			catch { }//nothing to do here, when running tests, an exception will happen :-(

		}
		private static void ConfigureDev(IApplicationBuilder app, IConfigurationRoot config)
		{
			app.UseDeveloperExceptionPage();
			AddPropertiesMIME(app, config);
		}

		private void ConfigureAll(IApplicationBuilder app, IConfigurationRoot config)
		{
			
			ConfigureFirst.ForEach(a => a(app, config));

			if (ApplicationMode.Equals("Test", StringComparison.OrdinalIgnoreCase))
				ConfigureTestAction(app, config);

			if (ApplicationMode.Equals("Development", StringComparison.OrdinalIgnoreCase))
				ConfigureDevAction(app, config);

			if (ApplicationMode.Equals("Production", StringComparison.OrdinalIgnoreCase))
				ConfigureLiveAction(app, config);

			ConfigureCommonAction(app, config);
		}

		private static void ConfigureTest(IApplicationBuilder app, IConfigurationRoot config)
		{
			app.UseTestErrorHandling();
			app.UseDeveloperExceptionPage();
			AddPropertiesMIME(app, config);//this is needed when running on test deployment!

		}

		private static void ConfigureLive(IApplicationBuilder app, IConfigurationRoot config)
		{
			AddPropertiesMIME(app, config);
			app.UseLiveErrorHandling();
		}

		private static IConfigurationRoot GetConfig(IHostingEnvironment env)
		{
			string appdata = Environment.GetEnvironmentVariable(System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows) ? "APPDATA" : "Home");
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddJsonFile($"{appdata}\\microsoft\\UserSecrets\\ER4.appsettings.User.json", optional: true)
				.AddEnvironmentVariables();

			if (env.IsDevelopment())
			{
				// This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
				builder.AddApplicationInsightsSettings(developerMode: true);
			}

			var config = builder.Build();
			return config;
		}

		private static IHostingEnvironment GetHostingEnvironment(Func<string> contentRootPath)
		{
			var hostingEnvironment = new HostingEnvironment();

			var webHostOptions = new WebHostOptions();

			hostingEnvironment.Initialize("Template", contentRootPath(), webHostOptions);

			hostingEnvironment.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			return hostingEnvironment;
		}


		private static void ConfigureServices(IServiceCollection services, IConfigurationRoot config)
		{

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options => {
					options.LoginPath = new PathString("/Signin/");
					options.LogoutPath = "/Account/LogOff";
					options.AccessDeniedPath = new PathString("/Signin/Forbidden/");
				});

			services.AddApplicationInsightsTelemetry(config);

			services.AddOptions();

			services.Configure<AppSettings>(config.GetSection("AppSettings"));

			//services.AddCustomHeaders();

			services.Configure<FormOptions>(x => x.ValueCountLimit = 2048);

			services.AddMvc(options =>
			{
				options.InputFormatters.Add(new XmlSerializerInputFormatter());

				options.RespectBrowserAcceptHeader = true;

			}).AddJsonOptions(options =>
			{
				//options.SerializerSettings.Converters.Add(new HydraJsonConverter());
			});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("SignedIn", policy => policy.RequireClaim("Id"));
			});

			services.Configure<RazorViewEngineOptions>(o => o.ViewLocationExpanders.Add(new ViewLocationRemapper()));
		}

		private static string GetBuildNumber()
		{
			var path = "releaseBuild.txt";
			var buildNumber = "";
			if (!File.Exists(path))
				return "Build version file not found";

			using (StreamReader reader = File.OpenText(path))
			{
				buildNumber = reader.ReadLine();
			}
			return buildNumber;
		}

		private static void DoRaven(IServiceCollection services, IConfigurationRoot config)
		{
			var ret = GetDocumentStore(config);
			if (ret != null)
				services.AddSingleton(ret);
		}


		private static IDocumentStore GetDocumentStore(IConfigurationRoot config)
		{
			var ravenHost = config["AppSettings:RavenHost"];

			var databaseName = config["AppSettings:DatabaseName"];

			IDocumentStore store = new DocumentStore
			{
				Url = ravenHost,
				DefaultDatabase = databaseName
			};

			store.Initialize();


			try
			{
				//need to work out how to get reference to assembly, so we can scan and load all indexes
				//IndexCreation.CreateIndexes(typeof(Startup).Assembly, store);

				//explicitly add each index for now

			}
			catch (Exception ex)
			{
				// log here, raven is down?
				Console.WriteLine(ex.Message);
			}
			return store;
		}

		private void ConfigureLogging(ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(ConfigurationRoot.GetSection("Logging"));
			loggerFactory.AddDebug();
			var env = ConfigurationRoot.GetSection("AppSettings")["ApplicationMode"];
		}

	}
}
