using deneme123.Models;
using Microsoft.AspNetCore.Mvc;

namespace deneme123.Controllers
{
    public class SincapController : Controller
    {
        private MyDbContext _myDbContext;
        public SincapController(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        public IActionResult Admin()
        {
            return View();
        }
        [HttpGet]
        public IActionResult SincapPanel()
        {

            return View();
        }


        [HttpPost]
        public IActionResult SincapPanel(string username, string userpasword)
        {
            var errors = new List<string>();
            var model = _myDbContext.Admins.FirstOrDefault(x => x.User.Equals(username) && x.UserPass.Equals(userpasword));
            if (model != null)
            {
                HttpContext.Session.SetInt32("adminid", model.Id);
                HttpContext.Session.SetString("adminname", model.AdminName + " " + model.AdminSurname);
                return Redirect("/Admin/Index");

            }

            
            return Redirect("/Sincap/Admin");
        }
        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            return Redirect("/Sincap/Admin");
        }

    }
}
