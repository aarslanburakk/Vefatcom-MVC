using deneme123.Models;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System.Configuration;

namespace deneme123
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.




            var connetionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<MyDbContext>(options =>
            {
                options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
                options.EnableSensitiveDataLogging(true);
            });
            builder.Services.AddSession();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication();
          
            builder.Services.AddTransient<IDeadRepository, EfDeadRepository>();
            
            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
          
               

            app.Run();
        }
    }
}