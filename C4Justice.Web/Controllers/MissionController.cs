using Microsoft.AspNetCore.Mvc;

namespace C4Justice.Web.Controllers;

public class MissionController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
