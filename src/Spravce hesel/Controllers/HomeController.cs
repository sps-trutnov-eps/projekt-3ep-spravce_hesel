using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Spravce_hesel.Classes;
using Spravce_hesel.Data;
using Spravce_hesel.Models;
using System.Net;

namespace Spravce_hesel.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        [Route("/Home/Index")]
        public IActionResult Index() => View();

        [Route("/Error/{code:int}")]
        public IActionResult Error(int code) => View(code);

        [Route("/MakeMeACoffee")]
        public IActionResult MakeMeACoffee() => StatusCode(418);
    }
}