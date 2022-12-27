using deneme123.Models;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Net;
using System.Net.Mail;
using X.PagedList;

namespace deneme123.Controllers
{

    public class HomeController : Controller
    {
        private IDeadRepository repository;
        public HomeController(IDeadRepository _repository)
        {

            repository = _repository;
        }
        public IActionResult About()
        {



            return View();
        }

     
        public IActionResult Forgot()
        {



            return View();
        }
      
        public IActionResult AddDead()
        {



            return View();
        }


        public IActionResult Reset()
        {



            return View();
        }
        public  IActionResult Index(string name = null, decimal? city = null, decimal? age = null, string date=null ,decimal? item = null,int page =1)
        {


            var dead = repository.GetDeadsByActive(true,name,city,age,date,item) ;
            return View(dead.ToPagedList(page,81));

        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user ,string Email)
        {
            var errors = new List<string>();    
            var email = repository.Users.FirstOrDefault(w => w.Email.Equals(Email));
            if (email != null)
            {
                ViewBag.Result = new ViewModelResult(false, "E-Mail Zaten Kayıtlı.", errors);
                return View();

            }
            if(user.UserName==null)
            {
                ViewBag.Result = new ViewModelResult(false, "Tüm Boşluklları Doldurun.", errors);
                return View();
            }
            if (user.UserSurname == null)
            {
                ViewBag.Result = new ViewModelResult(false, "Tüm Boşluklları Doldurun.", errors);
                return View();
            }
            if (user.Passs == null)
            {
                ViewBag.Result = new ViewModelResult(false, "Tüm Boşluklları Doldurun.", errors);
                return View();
            }
            repository.Register(user);
            ViewBag.Result = new ViewModelResult(true, "Kayıt Oldunuz Giriş Yapabilirsiniz.");
            return View();
        }
       
        [HttpGet]
        public IActionResult login()
        {

            return View();
        }


        [HttpPost]
        public IActionResult Login(string UserEmail, string UserPass)
        {
            var errors = new List<string>();

            var model = repository.Users.FirstOrDefault(x => x.Email.Equals(UserEmail) && x.Passs.Equals(UserPass));
            if (model != null)
            {
                HttpContext.Session.SetInt32("userid", model.Id);
                HttpContext.Session.SetString("fullname", model.UserName + " " + model.UserSurname);
                HttpContext.Session.SetString("email", model.Email);
                return Redirect("/User/Index");

            }
            
            ViewBag.Result = new ViewModelResult(false, "E-Posta Veya Şifre Hatalı.", errors);

            return View();
        }

        public IActionResult SendCode(string email)
        {
            var errors = new List<string>();
            var user = repository.Users.FirstOrDefault(w => w.Email.Equals(email));
            if (user != null)
            {
                repository.SendCode(email);
                return Redirect("Reset");
                ViewBag.Result = new ViewModelResult(true, "Mailinize Kod Gönderildi.");
            }
            ViewBag.Result = new ViewModelResult(false, "E-Mail Hatalı Veya Kayıtlı Değil.", errors);
            return View("Forgot");

        }

        public IActionResult ResetPass(string code,string pass)
        {
            var errors = new List<string>();
            var passwordCode = repository.PasswordCode.FirstOrDefault(w => w.Code.Equals(code));
            if(passwordCode != null)
            {
                repository.ResetPass(code,pass);
                ViewBag.Result = new ViewModelResult(true, "Şifre Değişim İşlemi Başarılı. Giriş Yapabilirsiniz.");
                return View("Reset");
            }
            ViewBag.Result = new ViewModelResult(false, "Şifre Değiştiriken Bir Hata Oluştu. Tekrar Deneyiniz.", errors);
            return View("Reset");

        }

    
        public IActionResult Contact()
        {



            return View();
        }

        [HttpPost]
        public IActionResult Contact(Contact contact)
        {

            var errors = new List<string>();
            if(contact.Subject == null)
            {
                ViewBag.Result = new ViewModelResult(false, "Tüm Boşlukları Doldurunuz.");
                return View("Contact");

            }
            if (contact.İsim == null)
            {
                ViewBag.Result = new ViewModelResult(false, "Tüm Boşlukları Doldurunuz.");
                return View("Contact");

            }
            if (contact.Email == null)
            {
                ViewBag.Result = new ViewModelResult(false, "Tüm Boşlukları Doldurunuz.");
                return View("Contact");

            }
            if (contact.Message == null)
            {
                ViewBag.Result = new ViewModelResult(false, "Tüm Boşlukları Doldurunuz.");
                return View("Contact");

            }
            else
            {

                try
                {
                    var from = "destek@ve-fat.com";
                    var to = "destek@ve-fat.com";
                    const string Pass = "Melodica21..";
                    string mail_subject = contact.Subject;
                    string mail_message = "İsim soyisim: " + contact.İsim + "\n";
                    mail_message += "E-Mail: " + contact.Email + "\n";
                    mail_message += "Konu: " + contact.Subject + "\n";
                    mail_message += "Mesaj: " + contact.Message + "\n";

                    var smtp = new SmtpClient();
                    {
                        smtp.Host = "smtp.yandex.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.Credentials = new NetworkCredential(from, Pass);
                        smtp.Timeout = 20000;
                    }
                    smtp.Send(from, to, mail_subject, mail_message);
                    contact.Email = "";
                    contact.Subject = "";
                    contact.İsim = "";
                    contact.Message = "";
                }
                catch
                {

                }
                ViewBag.Result = new ViewModelResult(true, "Görüşünüzü Bildirdiğiniz İçin Teşekkürler.");
                return View();


            }

            
        }
        [HttpGet]
        public IActionResult DeadComment(int id)
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Teachers = GetDeadsComment(id);
            mymodel.Students = GetComments(id);




            return View(mymodel);
        }

        private dynamic GetComments(int id)
        {
            IQueryable<Comment> query1 = repository.Comments;

            query1 = query1.Where(z => z.DeadID == id ).Where(z=>z.CommentİsActive == true);




            return query1.Include(x => x.Dead).OrderBy(i => i.CommentID).ToList();
        }

        private dynamic GetDeadsComment(int id)
        {
            IQueryable<Dead> query = repository.Deads;

            query = query.Where(z => z.Id == id);
            




            return query.Include(x => x.City).ToList();
        }

      









    }
}

