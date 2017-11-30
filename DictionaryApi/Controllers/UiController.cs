using System.Web.Mvc;

namespace DictionaryApi.Controllers
{
    /// <summary>
    /// Class Ui
    /// </summary>
    /// <seealso cref="System.Web.Mvc.Controller" />
    public class UiController : Controller
    {
        // GET: Ui
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}