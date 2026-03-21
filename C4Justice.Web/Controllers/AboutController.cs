using Microsoft.AspNetCore.Mvc;

namespace C4Justice.Web.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
