using System.Globalization;
using AspNetCore.ReCaptcha;
using BookingQueue.BLL.Resources;
using BookingQueue.BLL.Services.Interfaces;
using BookingQueue.Common.Enums;
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
        var activeServices = await _servicesService.GetAllActiveAsync();
        return View(activeServices);
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
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult DownloadDocs(DocTypeEnum docType)
    {
        var file = GetFileFromDocType(docType);

        if (file is null) return NotFound();
        
        return file;
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

    private FileStreamResult? GetFileFromDocType(DocTypeEnum docType)
    {
        var isRus = string.Equals(CultureInfo.CurrentCulture.Name, "ru", StringComparison.OrdinalIgnoreCase);
        var webRootPath = _hostEnvironment.WebRootPath;
        string downloadFileText;
        string filePath;

        switch (docType)
        {
            case DocTypeEnum.Privacy:
                filePath = Path.Combine(webRootPath, "documents\\privacy.pdf");
                downloadFileText = "Документ условия и соглашения.pdf";
                break;
            case DocTypeEnum.InstructionForQueue:
                filePath = Path.Combine(webRootPath, isRus ? "documents\\InstructionsRus.pdf" : "documents\\InstructionKG.pdf");
                downloadFileText = $"{_localization.GetLocalizedString("Email_request")}.pdf";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(docType), docType, @"Документ для скачивания не найден");
        }

        return System.IO.File.Exists(filePath) 
            ? File(System.IO.File.OpenRead(filePath), "application/pdf", downloadFileText) 
            : null;
    }

    #endregion
}