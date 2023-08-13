using Microsoft.AspNetCore.Mvc;

namespace BookingQueue.Controllers;

public class BranchController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}