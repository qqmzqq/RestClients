using Microsoft.AspNetCore.Mvc;

namespace RestClients.Controllers
{
    public class CxSastController: Controller
    {
        public IActionResult CxSastHome()
        {
            return View();
        }
    }
}