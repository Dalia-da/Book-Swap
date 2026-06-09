using Microsoft.AspNetCore.Mvc;

public class messages : Controller
{
    public IActionResult MyMessages()
    {
        return View();
    }
}
