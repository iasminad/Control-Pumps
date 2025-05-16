using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;

public class AuthController : Controller
{
    public IActionResult Index()
    {
        return (IActionResult)Content("Welcome to the Home Page!");
    }

    public IActionResult Error()
    {
        return (IActionResult)Content("An error occurred.");
    }
}