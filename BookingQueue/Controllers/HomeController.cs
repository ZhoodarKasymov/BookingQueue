using AspNetCore.ReCaptcha;
using BookingQueue.BLL.Services.Interfaces;
using BookingQueue.Common.Models.ViewModels;
using BookingQueue.Resources;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingQueue.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IServicesService _servicesService;

    public HomeController(IWebHostEnvironment hostEnvironment, 
        IServicesService servicesService)
    {
        _hostEnvironment = hostEnvironment;
        _servicesService = servicesService;
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
        
        return RedirectToAction("SelectServices", bookViewModel);
    }

    public async Task<IActionResult> SelectServices(BookViewModel bookViewModel)
    {
        var activeServices = await _servicesService.GetAllActiveAsync();
        ViewData["Services"] = activeServices.Select(s => new SelectListItem(s.Name, s.Id.ToString()));
        return View(bookViewModel);
    }

    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        var cookieName = CookieRequestCultureProvider.DefaultCookieName;
        
        Response.Cookies.Append(
            cookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(returnUrl);
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