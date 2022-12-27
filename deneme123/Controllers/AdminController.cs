using deneme123.Filter;
using deneme123.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;
using System.Security.Claims;
using X.PagedList;

namespace deneme123.Controllers
{
    [AdminFilter]
    public class AdminController : Controller
    {

        private IDeadRepository repository;
        private readonly IWebHostEnvironment webHostEnvironment1;

        public AdminController(IDeadRepository _repository, IWebHostEnvironment webHostEnvironment)
        {

            repository = _repository;
            webHostEnvironment1 = webHostEnvironment;
        }
        private void PopulateCreateLookups()
        {
            var Citylist = repository.Cities.ToList();
            Citylist.Insert(0, new City { CityName = "Şehir Seçiniz" });
            ViewBag.listofcity = Citylist;


        }
        public IActionResult DelateImage()
        {

            var image = repository.GetImages();
            ViewBag.DeadCount = image.Count();
            return View(image);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id, string resim)
        {
            repository.DeleteImg(id);
            
            return RedirectToAction(nameof(DelateImage));
        }



        public IActionResult About()
        {
            return View();
        }
        public IActionResult Comments()
        {
            var comment = repository.GetAdminComments();

            return View(comment);

        }
        public IActionResult Users()
        {
            var user = repository.GetUsers();
            ViewBag.DeadCount = user.Count();
            return View(user);
        }
        [HttpPost]
        public IActionResult DeleteUsers(int id)
        {
            repository.DeleteUser(id);
            return RedirectToAction(nameof(Users));
        }

        public IActionResult DeadDelate()
        {
            var dead = repository.GetDeads();
            ViewBag.DeadCount = dead.Count();
            return View(dead);
        }
        public IActionResult Edit(int id)
        {

            PopulateCreateLookups();


            return View(repository.GetById(id));
        }

        [HttpPost]
        public IActionResult Edit(Dead entity)
        {
            repository.Update(entity);
            return RedirectToAction(nameof(DeadDelate));
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            repository.Delete(id);
            return RedirectToAction(nameof(DeadDelate));
        }
        public IActionResult EditUser(int id)
        {

            return View(repository.UserGetById(id));
        }
        [HttpPost]
        public IActionResult EditUser(User entity)
        {
            repository.UserUpdate(entity);
            return RedirectToAction(nameof(Users));
        }
        public IActionResult CommentEdit(int id)
        {
            return View(repository.CommentGetById(id));
        }
        [HttpPost]
        public IActionResult CommentDelete(int id)
        {
            repository.CommentDelete(id);
            return RedirectToAction(nameof(Comments));
        }
        [HttpPost]
        public IActionResult CommentEdit(Comment updatedComment)
        {
            repository.CommentEdit(updatedComment);
            return RedirectToAction(nameof(Comments));

        }
        public IActionResult SincapPanel()
        {

            return View();
        }


        public IActionResult Logout()
        {

            HttpContext.Session.Clear();
            return Redirect("/Sincap/Admin");
        }




        [HttpGet]
        public IActionResult Create()
        {

            PopulateCreateLookups();

            return View();
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 104857600)]
        public async Task<IActionResult> Create(Dead dead, List<IFormFile> DeadImage)
        {
         
            var errors = new List<string>();
            if (!ModelState.IsValid)
            {
                if (dead.DeadAge > 110)

                {
                    PopulateCreateLookups();
                    ViewBag.Result = new ViewModelResult(false, "Vefat Yaşı 110 Altı Olmalıdır", errors);
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
                dead.UserId = 1;
                repository.Create(dead);
                ViewBag.Result = new ViewModelResult(true, "Vefat İlanı Verildi. En Kısa Zamanda İlanınız Aktif Edilecektir. Teşekkürler...");
                return View();
            }
            
            repository.Create(dead);
            ViewBag.Result = new ViewModelResult(true, "Vefat İlanı Verildi. En Kısa Zamanda İlanınız Aktif Edilecektir. Teşekkürler...");


            return View();
        }



        public IActionResult Index(string name = null, decimal? city = null, decimal? age = null, string date = null, decimal? item = null, int page = 1)
        {


           
            var dead = repository.GetDeadsByActive(true, name, city, age, date, item);

            return View(dead.ToPagedList(page, 81));

        }
        [HttpGet]
        public IActionResult DeadImage()
        {

            return View();
        }
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit =104857600)]
        public async Task<IActionResult> DeadImage(Image image, List<IFormFile> Picture)
        {


            foreach (var item in Picture)

            {
                if (item.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await item.CopyToAsync(stream);
                        image.Picture = stream.ToArray();
                    }
                }

            }
            repository.AddImage(image);

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

            query1 = query1.Where(z => z.DeadID == id);




            return query1.Include(x => x.Dead).OrderBy(i => i.CommentID).ToList();
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

            var commetnuser = HttpContext.Session.GetString("adminname");
            comment.CommentContent = commenttext;
            comment.CommentUser = commetnuser;
            comment.DeadID = deadid;

            repository.AddComment(comment);

            ViewBag.Result = new ViewModelResult(true, "Yorum Yapma İşlemi Başarılı. En Kısa Zamanda Yorumunuz İncelendikten Sonra Aktif Edilecektir. Teşekkürler...");
            dynamic mymodel = new ExpandoObject();
            mymodel.Teachers = GetDeadsComments(deadid);
            mymodel.Students = GetCommentss(deadid);

            return View(mymodel);

          
        }
        private dynamic GetCommentss(int deadid)
        {
            IQueryable<Comment> query1 = repository.Comments;

            query1 = query1.Where(z => z.DeadID == deadid);





            return query1.Include(x => x.Dead).OrderBy(i => i.CommentID).ToList();
        }

        private dynamic GetDeadsComments(int deadid)
        {
            IQueryable<Dead> query = repository.Deads;

            query = query.Where(z => z.Id == deadid);




            return query.Include(x => x.City).ToList();
        }

    }
}
