using Microsoft.AspNetCore.Mvc;

namespace deneme123.Models
{
    public interface IDeadRepository
    {
        IQueryable<Dead> Deads { get; }

        IEnumerable<Admin> Admins { get; }
        IQueryable<City> Cities { get; }
        IQueryable<User> Users { get; }
        IQueryable<Image> Images { get; }
        IQueryable<Comment> Comments { get; }
        IEnumerable<PasswordCode> PasswordCode { get; }





        Dead GetById(int id);
        Comment CommentGetById(int id);
       
        User UserGetById(int id);

        IEnumerable<Dead> GetDeads();
        IEnumerable<Dead> GetUserDeads(int id);
        IEnumerable<Dead> GetDeadsComment(int id);
        IEnumerable<User> GetUsers();       
        IEnumerable<Comment> GetAdminComments();       
        IEnumerable<Image> GetImages(); 
        IEnumerable<City> GetCities();
        IEnumerable<Dead> GetDeadsByActive(bool isActive , string name = null, decimal? city = null, decimal? age = null, string date = null, decimal? item = null);
        IEnumerable<Dead> GetDeadBySearch(string name = null, string city = null, decimal? age = null, string date = null, decimal? item = null);
        void Create(Dead dead);
        void Update(Dead updatedDead);
        void UserUpdate(User updatedUser);
        void DeleteImg(int id);
        void Delete(int id);
        void CommentDelete(int id);
        void DeleteUser(int id);
        void Register(User user);
        void SendCode(string email);
        void ResetPass(string code, string pass);
        void AddImage(Image image);
        void AddComment(Comment comment);
        void CommentEdit(Comment updatedComment);


    }

}
