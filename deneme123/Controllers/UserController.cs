using deneme123.Filter;
using deneme123.Models;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Net;
using System.Net.Mail;
using X.PagedList;

namespace deneme123.Controllers
{
    [UserFilter]
    public class UserController : Controller
    {
        private IDeadRepository repository;
        public UserController(IDeadRepository _repository)
        {

            repository = _repository;
        }

        private void PopulateCreateLookups()
        {
            var Citylist = repository.Cities.OrderBy(x=>x.CityName).ToList();
            Citylist.Insert(0, new City { CityName = "Şehir Seçiniz" });
            ViewBag.listofcity = Citylist;


            
        }
     
        public IActionResult About()
        {



            return View();
        }
        public IActionResult ImagePay()
        {



            return View();
        }
        public IActionResult Contact()
        {
           

            return View();
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

            query1 = query1.Where(z => z.DeadID == id).Where(z => z.CommentİsActive == true);
            




            return query1.Include(x => x.Dead).OrderBy(i=>i.CommentID).ToList();
        }

        private dynamic GetDeadsComment(int id)
        {
            IQueryable<Dead> query = repository.Deads;

            query = query.Where(z => z.Id == id);
            



            return query.Include(x => x.City).ToList();
        }

        [HttpPost]
        public IActionResult DeadComment(Comment comment, int deadid, string commenttext)
        {
            
            
            var commetnuser= HttpContext.Session.GetString("fullname");
            comment.CommentContent = commenttext;
            comment.CommentUser = commetnuser;
            comment.DeadID = deadid;
            ViewBag.Result = new ViewModelResult(true, "Yorum Yapma İşlemi Başarılı. En Kısa Zamanda Yorumunuz İncelendikten Sonra Aktif Edilecektir. Teşekkürler...");
          
            repository.AddComment(comment);
           
            dynamic mymodel = new ExpandoObject();
            mymodel.Teachers = GetDeadsComments(deadid);
            mymodel.Students = GetCommentss(deadid);

            return View(mymodel);
        }

        private dynamic GetCommentss(int deadid)
        {
            IQueryable<Comment> query1 = repository.Comments;

            query1 = query1.Where(z => z.DeadID == deadid).Where(z => z.CommentİsActive == true);

            return query1.Include(x => x.Dead).OrderBy(i => i.CommentID).ToList();
        }

        private dynamic GetDeadsComments(int deadid)
        {
            IQueryable<Dead> query = repository.Deads;

            query = query.Where(z => z.Id == deadid);




            return query.Include(x => x.City).ToList();
        }

        public IActionResult Hesap(int id)
        {
            id = (int)HttpContext.Session.GetInt32("userid");
            dynamic mymodel = new ExpandoObject();
            mymodel.Deads = GetUserDeads(id);
            mymodel.Users = GetUsers(id);

            return View(mymodel);
        }

        private dynamic GetUsers(int id)
        {
            IQueryable<User> query = repository.Users;

            query = query.Where(z => z.Id == id);


            return query.ToList();
        }

        private dynamic GetUserDeads(int id)
        {
            IQueryable<Dead> query = repository.Deads;

            query = query.Where(z => z.UserId == id);

            return query.Include(x => x.City).OrderByDescending(i=>i.Id).ToList();
        }

        public IActionResult EditHesap(int id)
        {   
            var Hesap = repository.Users.Where(x=>x.Id==id).FirstOrDefault();       

            return PartialView("EditHesap",Hesap);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateCreateLookups();

            return View();
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<IActionResult> Create(Dead dead , List<IFormFile> DeadImage)
        {
            string Bilinmiyor = "Girilmemiş.";
            int  userid= (int)HttpContext.Session.GetInt32("userid");
            var errors = new List<string>();
       
            if (!ModelState.IsValid)
            {
                if (dead.DeadAge > 110)

                {
                    PopulateCreateLookups();
                    ViewBag.Result = new ViewModelResult(false, "Vefat Yaşı 110'nun Altı Olmalıdır", errors);
                    return View();
                }
                if (dead.DeadNameUsername == null)
                {
                    PopulateCreateLookups();
                    ViewBag.Result = new ViewModelResult(false, "Vefat Adı Soyadı Boş Olamaz.", errors);
                    return View();
                }
                if (dead.DeadDate == null)
                {
                    PopulateCreateLookups();
                    ViewBag.Result = new ViewModelResult(false, "Vefat Tarihi Boş Olamaz.", errors);
                    return View();
                }
                if (dead.CityId == 0)
                {
                    PopulateCreateLookups();
                    ViewBag.Result = new ViewModelResult(false, "Vefat Şehri Seçiniz.", errors);
                    return View();
                }

                foreach (var item in DeadImage)

                {
                    if (item.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await item.CopyToAsync(stream);
                            dead.DeadImage = stream.ToArray();
                        }
                    }

                }

                PopulateCreateLookups();         
                dead.UserId = userid;
                repository.Create(dead);
                ViewBag.Result = new ViewModelResult(true, "Vefat İlanı Verildi. En Kısa Zamanda İlanınız Aktif Edilecektir. Teşekkürler...");
                return View();
            }

            repository.Create(dead);
            ViewBag.Result = new ViewModelResult(true, "Vefat İlanı Verildi. En Kısa Zamanda İlanınız Aktif Edilecektir. Teşekkürler...");


            return View();
        }
        public IActionResult Index(string name = null, decimal? city = null, decimal? age = null, string date = null, decimal? item = null ,int page = 1)
        {


           
            var dead = repository.GetDeadsByActive(true, name, city, age, date, item);

            return View(dead.ToPagedList(page, 81));

        }

        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            return Redirect("/Home/Index");
        }
   

        [HttpPost]
        public IActionResult Contact(Contact contact)
        {

            var errors = new List<string>();
            if (contact.Subject == null)
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


    }
}
