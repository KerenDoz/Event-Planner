using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View("Error404");
        }

        [Route("Error/400")]
        public IActionResult Error400()
        {
            Response.StatusCode = 400;
            return View("Error400");
        }

        [Route("Error/500")]
        public IActionResult Error500()
        {
            Response.StatusCode = 500;
            return View("Error500");
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => RedirectToAction(nameof(Error400)),
                404 => RedirectToAction(nameof(Error404)),
                _ => RedirectToAction(nameof(Error500))
            };
        }
    }
}