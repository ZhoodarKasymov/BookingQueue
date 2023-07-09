using AspNetCore.ReCaptcha;
using BookingQueue.Common.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BookingQueue.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;

    public HomeController(IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateReCaptcha(ErrorMessage = "Вы не прошли проверку Recaptcha.")]
    [ValidateAntiForgeryToken]
    public IActionResult Index(BookViewModel bookViewModel)
    {
        // Validate the captcha response
        if (!ModelState.IsValid)
            return View(bookViewModel);
        
        return View(bookViewModel);
    }

    public IActionResult Privacy()
    {
        var webRootPath = _hostEnvironment.WebRootPath;
        var filePath = Path.Combine(webRootPath, "privacy\\privacy.pdf");
        var contentType = "application/pdf";

        if (System.IO.File.Exists(filePath))
        {
            // Return the file to the client
            return File(System.IO.File.OpenRead(filePath), contentType, "Документ условия и соглашения.pdf");
        }

        // Handle file not found scenario
        return NotFound();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}