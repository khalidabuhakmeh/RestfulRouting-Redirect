using System.Web.Mvc;

namespace MvcApplication1.Controllers
{
    public class ShopsController : ApplicationController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(string id)
        {
            ViewBag.Name = id;
            return View();
        }
    }
}
