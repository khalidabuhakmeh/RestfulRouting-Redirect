using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcApplication1.Models
{
    public class PreviousActionValuesAttribute : ActionFilterAttribute
    {
        public const string PreviousActionsKey = "__previous_action";

        // after the action has executed
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;

            filterContext.Controller.TempData.Add(PreviousActionsKey, new PreviousAction(filterContext));
        }
    }

    public class PreviousAction
    {
        public PreviousAction()
        {
            RouteData = new RouteValueDictionary();
            QueryString = new NameValueCollection();
        }

        public PreviousAction(ActionExecutedContext context)
        {
            RouteData = context.RouteData.Values;
            QueryString = context.RequestContext.HttpContext.Request.QueryString;
        }

        public RouteValueDictionary RouteData { get; set; }
        public NameValueCollection QueryString { get; set; }

        public object Get(string key)
        {
            if (RouteData.ContainsKey(key))
                return RouteData[key];

            if (QueryString.AllKeys.Any(x => x == key))
                return QueryString[key];

            return null;
        }

        public T Get<T>(string key)
        {
            return (T) Get(key);
        }
    }

    public static class PreviousActionExtensionMethods
    {
        public static PreviousAction Previous(this TempDataDictionary dictionary)
        {
            if (!dictionary.ContainsKey(PreviousActionValuesAttribute.PreviousActionsKey))
                return new PreviousAction();

            return (PreviousAction) dictionary[PreviousActionValuesAttribute.PreviousActionsKey];
        }
    }
}