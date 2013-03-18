using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MvcApplication1.Controllers;
using RestfulRouting;

namespace MvcApplication1
{
    public class RouteConfig : RouteSet
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoutes<RouteConfig>();
        }

        public override void Map(IMapper map)
        {
            map.DebugRoute("routedebug");
            map.Root<ShopsController>(s => s.Index());

            map.Resources<ShopsController>(s =>
            {
                s.As("boutiques");
                s.Only("index", "show");
            });

            map.Redirect("shops")
               .WithName("old_boutiques_index")
               .To(new { controller = "shops", action = "index" })
               .GetOnly();

            map.Redirect("shops/{id}")
               .WithName("old_boutiques_show")
               .To(new { controller = "shops", action = "show" })
               .AllowMethods("POST", "GET");
        }
    }

    public class RedirectFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            var redirect = filterContext.RouteData.Route as RedirectRoute;
            if (redirect != null)
            {
                var helper = new UrlHelper(filterContext.RequestContext);
                var values = new RouteValueDictionary(filterContext.RequestContext.RouteData.Values);
                var merged = new RouteValueDictionary(redirect.DataTokens);

                // keep the values we specified, and add the other routeValues
                // that we we didn't have overrides for.
                foreach (var key in values.Keys.Where(key => !merged.ContainsKey(key)))
                    merged.Add(key, filterContext.RouteData.Values[key]);

                var url = helper.RouteUrl(merged);
                filterContext.Result = new RedirectResult(url, redirect.IsPermanent);
            }
        }
    }

    public static class MapperExtensions
    {
        public static IRedirectMapper Redirect(this IMapper mapper, string oldPath)
        {
            var route = new RedirectRoute(oldPath);
            mapper.Route(route);
            return route;
        }
    }

    public class RedirectRoute : NamedRoute, IRedirectMapper
    {
        public RedirectRoute(string url)
            : base(Guid.NewGuid().ToString(), url, new RouteValueDictionary(), new RouteValueDictionary(), new MvcRouteHandler())
        {
            Values = new RouteValueDictionary();
            Name = Guid.NewGuid().ToString();
            IsPermanent = false;
        }

        public bool IsPermanent { get; set; }
        public RouteValueDictionary Values { get; set; }

        public IRedirectMapper WithName(string name)
        {
            Name = name;
            return this;
        }

        public IRedirectMapper From(object values)
        {
            Defaults = new RouteValueDictionary(values);
            return this;
        }

        public IRedirectMapper To(object values)
        {
            if (Defaults == null || !Defaults.Any())
                Defaults = new RouteValueDictionary(values);

            DataTokens = new RouteValueDictionary(values);
            return this;
        }

        public IRedirectMapper GetOnly()
        {
            if (Constraints.ContainsKey("httpMethod"))
                Constraints.Remove("httpMethod");

            Constraints.Add("httpMethod", new HttpMethodConstraint("GET"));
            return this;
        }

        public IRedirectMapper AllowMethods(params string[] httpVerbs)
        {
            if (Constraints.ContainsKey("httpMethod"))
                Constraints.Remove("httpMethod");

            Constraints.Add("httpMethod", new HttpMethodConstraint(httpVerbs));
            return this;
        }

        public IRedirectMapper Permanent()
        {
            IsPermanent = true;
            return this;
        }

        public IRedirectMapper NotPermanent()
        {
            IsPermanent = false;
            return this;
        }
    }

    public interface IRedirectMapper
    {
        IRedirectMapper WithName(string name);
        IRedirectMapper From(object values);
        IRedirectMapper To(object values);
        IRedirectMapper GetOnly();
        IRedirectMapper AllowMethods(params string[] httpVerbs);
        IRedirectMapper Permanent();
        IRedirectMapper NotPermanent();
    }
}