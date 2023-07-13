using System.Globalization;
using AspNetCore.ReCaptcha;
using BookingQueue.BLL.Services;
using BookingQueue.BLL.Services.Interfaces;
using BookingQueue.Common.Models.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookingQueue.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IServicesService _servicesService;
    private readonly IConfiguration _configuration;
    private readonly IAdvanceService _advanceService;

    public HomeController(
        IWebHostEnvironment hostEnvironment, 
        IServicesService servicesService, 
        IConfiguration configuration,
        IAdvanceService advanceService)
    {
        _hostEnvironment = hostEnvironment;
        _servicesService = servicesService;
        _configuration = configuration;
        _advanceService = advanceService;
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
        if (!ModelState.IsValid)
            return View(bookViewModel);

        TempData["bookViewModel"] = bookViewModel;
        
        return RedirectToAction("SelectServices");
    }

    public async Task<IActionResult> SelectServices()
    {
        var ci = CultureInfo.CurrentCulture.Name;
        var activeServices = await _servicesService.GetAllActiveAsync();
        ViewData["Services"] = activeServices.Select(s => new SelectListItem(ci == "uk" ? s.TranslatedName : s.Name, s.Id.ToString()));
        return View();
    }

    public async Task<string> BookingTime(DateTime? bookingTime, long? serviceId)
    {
        var bookViewModel = TempData["bookViewModel"] as BookViewModel;
        var maxUserCountOnService = _configuration.GetValue<int>("MaxClientsInService:MaxCount");
        
        bookViewModel.BookingDate = bookingTime;
        bookViewModel.ServiceId = serviceId;
        
        var result = await _advanceService.BookTimeAsync(bookViewModel, maxUserCountOnService);
        return result;
    }

    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
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