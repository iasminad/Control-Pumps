using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return Content("Welcome to the Home Page!");
    }

    public IActionResult Error()
    {
        return Content("An error occurred.");
    }
}