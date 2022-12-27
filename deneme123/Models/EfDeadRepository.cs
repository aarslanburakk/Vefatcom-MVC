using System.Net;
using System.Net.Mail;

namespace deneme123.Models
{
    public class EfDeadRepository : IDeadRepository
    {

        private string code = null;
        private MyDbContext _context;
        public EfDeadRepository(MyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            web = webHostEnvironment;
        }
        private readonly IWebHostEnvironment web;
        public IQueryable<Dead> Deads => _context.Deads;

        public IEnumerable<Admin> Admins => _context.Admins;
        IEnumerable<PasswordCode> IDeadRepository.PasswordCode => _context.PasswordCode;

        public IQueryable<City> Cities => _context.Cities;

        public IQueryable<User> Users => _context.Users;

        public IQueryable<Image> Images => _context.Images;

        public IQueryable<Comment> Comments => _context.Comments;




        private string getCode()
        {
            if (code ==     null)
            {
                Random rand = new Random();
                code = "";
                for (int i = 0; i < 6; i++)
                {
                    char tmp = Convert.ToChar(rand.Next(48, 58));
                    code += tmp;
                }
            }
            return code;
        }
        public void Create(Dead dead)
        {


            _context.Deads.Add(dead);
            _context.SaveChanges();


        }

        public void Delete(int id)
         {
            List<Comment> getcomment = new List<Comment>();

            getcomment = _context.Comments.Where(i => i.DeadID == id).ToList();
            var dead = _context.Deads.Find(id);
            if (getcomment != null)
            {
                foreach (var s in getcomment)
                {
                    _context.Comments.Remove(s);
                }
            }
            _context.Remove(dead);
            _context.SaveChanges();


        }



        public Dead GetById(int id)
        {

            return _context.Deads.Find(id);
        }

        public IEnumerable<Dead> GetDeads()
        {

            return _context.Deads.OrderByDescending(x => x.Id).Include(x => x.City).ToList();
        }


        public void Update(Dead updatedDead)
        {
            _context.Deads.Update(updatedDead);
            _context.SaveChanges();
        }



        IEnumerable<Dead> IDeadRepository.GetDeadsByActive(bool isActive, string name = null, decimal? city = null, decimal? age = null, string date = null, decimal? item = null)
        {
            IQueryable<Dead> query = _context.Deads;


            if (name != null)
            {
                query = query.Where(i => i.DeadNameUsername.ToLower().Contains(name.ToLower()));

            }
            if (city != null)
            {

                query = query.Where(i => i.CityId == city);

            }
            if (age != null)
            {
                query = query.Where(i => i.DeadAge == age);
            }
            if (date != null)
            {
                query = query.Where(i => i.DeadDate == date);

            }
            if (item != null)
            {
                query = query.Where(i => i.CityId == item);
            }
   
            

            return query.Where(i => i.isActive == true).OrderByDescending(x => x.Id).ToList();
        }

        void IDeadRepository.Register(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }


        public void SendCode(string email)
        {
            var user = _context.Users.FirstOrDefault(w => w.Email.Equals(email));
            if (user != null)
            {
                _context.Add(new PasswordCode { UserId = user.Id, Code = getCode() });
                _context.SaveChanges();
                string text = "<h3>Şifre Sıfrlama Kodunuz : </h3>" + getCode() + " ";
                string subject = "Şifre Sıfrlama";
                MailMessage msg = new MailMessage("destek@ve-fat.com", email, subject, text);
                msg.IsBodyHtml = true;
                SmtpClient sc = new SmtpClient("smtp.yandex.com", 587);
                sc.UseDefaultCredentials = false;
                NetworkCredential cre = new NetworkCredential("destek@ve-fat.com", "Melodica21..");
                sc.Credentials = cre;
                sc.EnableSsl = true;
                sc.Send(msg);
            }

        }

        public void ResetPass(string code, string pass)
        {
            var passwordCode = _context.PasswordCode.FirstOrDefault(w => w.Code.Equals(code));
            if (passwordCode != null)
            {
                var user = _context.Users.Find(passwordCode.UserId);
                user.Passs = pass;
                _context.Update(user);
                _context.Remove(passwordCode);
                _context.SaveChanges();
            }

        }

        public  IEnumerable<Dead> GetDeadBySearch(string name = null, string city = null, decimal? age = null, string date = null, decimal? item = null)
        {
            IQueryable<Dead> query = _context.Deads;

            if (name != null)
            {
                query = query.Where(i => i.DeadNameUsername.ToLower().Contains(name.ToLower()));

            }
            //if (city != null)
            //{

            //    query = query.Where(i => i.CityId== city);

            //}
            if (age != null)
            {
                query = query.Where(i => i.DeadAge == age);
            }
            if (date != null)
            {
                query = query.Where(i => i.DeadDate == date);

            }
            if (item != null)
            {
                query = query.Where(i => i.CityId == item);
            }
            return query.Where(i=>i.isActive==true).OrderByDescending(x => x.Id).Include(x => x.City).ToList();
        }



        public IEnumerable<City> GetCities()
        {

            return _context.Cities.OrderBy(i => i.CityName).ToList();
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.OrderByDescending(x => x.Id).ToList();
        }

        public IEnumerable<Comment> GetAdminComments()
        {

            IQueryable<Comment> query = _context.Comments;


            return query.OrderByDescending(x => x.CommentID).Include(x=>x.Dead).ToList();
        }




        public IEnumerable<Image> GetImages()
        {

            return _context.Images.OrderByDescending(i => i.Id).ToList();
        }


        public async void DeleteImg(int id)
        {

            var img = _context.Images.Find(id);

            _context.Remove(img);
            _context.SaveChanges();

        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            _context.Remove(user);
            _context.SaveChanges();
        }

        public User UserGetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void UserUpdate(User updatedUser)
        {

            _context.Users.Update(updatedUser);
            _context.SaveChanges();

        }

        public IEnumerable<Dead> GetUserDeads(int id)
        {
            IQueryable<Dead> query = _context.Deads;

            query = query.Where(i => i.UserId == id);


            return query.OrderByDescending(x => x.Id).Include(x => x.City).ToList();
        }

        public void AddImage(Image image)
        {
            _context.Images.Add(image);
            _context.SaveChanges();
            _context.SaveChanges();

        }

        public IEnumerable<Dead> GetDeadsComment(int id)
        {
            IQueryable<Dead> query = _context.Deads;

            query = query.Where(z => z.Id == id);
            GetComments(id);



            return query.Include(x => x.City).ToList();
        }
        public IEnumerable<Comment> GetComments(int id)
        {
            IQueryable<Comment> deneme1 = _context.Comments;

            deneme1 = deneme1.Where(z => z.DeadID == id);

            return deneme1.ToList();

        }

        public void AddComment(Comment comment)
        {
            comment.CommentDate = DateTime.Now;
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public void CommentDelete(int id)
        {
            var comment = _context.Comments.Find(id);
            _context.Remove(comment);
            _context.SaveChanges();
        }
        public void CommentEdit(Comment updatedComment)
        {
            _context.Comments.Update(updatedComment);
            _context.SaveChanges();
        }

        public Comment CommentGetById(int id)
        {
            return _context.Comments.Find(id);
        }

       
    }
}
