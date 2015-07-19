using System.Web.Mvc;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class ApplicationErrorController : Controller
    {
        //
        // GET: /ApplicationError/

        public ActionResult Index()
        {
            try
            {
                ApplicationError error = (ApplicationError)Session["ApplicationError"];
                ViewData["Controller"] = error.Controller;
                ViewData["Action"] = error.Action;
                ViewData["ErrorMessage"] = error.ErrorMessage;

            }
            catch { }

            return View();
        }

    }
}
