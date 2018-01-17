using Template.Framework.Hypermedia;

namespace Template
{
	public interface IContext
	{
		HydraLink Link<T>() where T : HydraClass;

		HydraLink Link<T>(int id) where T : HydraClass;

		HydraLink Link<T>(object values) where T : HydraClass;

		HydraLink Link(string routeName, object values);

		HydraLink Link(HydraClass media, object values);

		string CurrentUrl();

		string Host();

		string Scheme();
	}

	public static class ContextExtensions
	{
		public static string GetId<T>(this IContext ctx, int id) where T : HydraClass
		{
			return ctx.Link<T>(id).Id;
		}
	}
}
