using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // If routes are not found, return index.html in the wwwroot folder.
    public class FallbackController : Controller
    {
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), 
                "wwwroot", "index.html"), "text/HTML");
        }
    }
}