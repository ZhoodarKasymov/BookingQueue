using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;

namespace BookingQueue.Controllers;

public class HomeController : Controller
{
    
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateReCaptcha]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string name, string phone)
    {
        // Validate the captcha response
        if (!ModelState.IsValid)
            return View(new {name, phone});
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}