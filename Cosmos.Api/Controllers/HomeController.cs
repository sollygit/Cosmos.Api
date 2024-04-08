using Microsoft.AspNetCore.Mvc;

namespace Cosmos.Api.Controllers
{
    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectPermanent("/index.html");
        }
    }
}