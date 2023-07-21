﻿using System.Globalization;
using AspNetCore.ReCaptcha;
using BookingQueue.BLL.Resources;
using BookingQueue.BLL.Services.Interfaces;
using BookingQueue.Common.Models.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BookingQueue.Controllers;

public class HomeController : Controller
{
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IServicesService _servicesService;
    private readonly IAdvanceService _advanceService;
    private readonly LocService _localization;

    public HomeController(
        IWebHostEnvironment hostEnvironment, 
        IServicesService servicesService,
        IAdvanceService advanceService,
        LocService localization)
    {
        _hostEnvironment = hostEnvironment;
        _servicesService = servicesService;
        _advanceService = advanceService;
        _localization = localization;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateReCaptcha]
    [ValidateAntiForgeryToken]
    public IActionResult Index(BookViewModel bookViewModel)
    {
        if (!ModelState.IsValid)
        {
            if (ModelState.ContainsKey("Recaptcha"))
            {
                ModelState.Remove("Recaptcha");
                ModelState.AddModelError("Recaptcha", _localization.GetLocalizedString("RecaptchaErrorMessage"));
            }

            return View(bookViewModel);
        }

        TempData["bookViewModel"] = JsonConvert.SerializeObject(bookViewModel);
        
        return RedirectToAction("SelectServices");
    }

    public async Task<IActionResult> SelectServices()
    {
        var ci = CultureInfo.CurrentCulture.Name;
        var activeServices = await _servicesService.GetAllActiveAsync();
        var services = activeServices
            .Select(s => new SelectListItem(ci == "uk" ? s.TranslatedName : s.Name, s.Id.ToString()));
        
        return View(services);
    }

    public async Task<List<SelectListItem>> GetTimeWithPeriodByDate(DateTime? bookingTime, long? serviceId)
    {
        ValidateParams(bookingTime, serviceId);
        var timesWithPeriods = await _servicesService.GetTimeWithPeriodByDate(bookingTime.Value.ToLocalTime(), serviceId);
        
        return timesWithPeriods.Select(t => new SelectListItem(t, t)).ToList();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<string> BookingTime(DateTime? bookingTime, long? serviceId)
    {
        ValidateParams(bookingTime, serviceId);
        
        var bookViewModel = JsonConvert.DeserializeObject<BookViewModel>((string)TempData["bookViewModel"]!);
        
        bookViewModel.BookingDate = bookingTime.Value.ToLocalTime();
        bookViewModel.ServiceId = serviceId;
        
        var result = await _advanceService.BookTimeAsync(bookViewModel);
        return result;
    }

    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(1) }
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

    #region Private methods

    private void ValidateParams(DateTime? bookingDate, long? serviceId)
    {
        if (bookingDate is null) throw new Exception(_localization.GetLocalizedString("ChooseDateAndTimePlease"));
        
        if (serviceId is null) throw new Exception(_localization.GetLocalizedString("ChooseServicesPlease"));
    }

    #endregion
}