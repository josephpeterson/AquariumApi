using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Spa()
        {
            return File("~/index.html", "text/html");
        }
    }
}
