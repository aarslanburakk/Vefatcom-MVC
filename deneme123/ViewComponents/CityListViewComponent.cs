using deneme123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace deneme123.ViewComponents
{
    public class CityListViewComponent : ViewComponent
    {
        private IDeadRepository repository;
        public CityListViewComponent(IDeadRepository _repository)
        {

            repository = _repository;
        }

   

        public ViewViewComponentResult Invoke()
        {
            var city = repository.GetCities();
       

            return View(city);  
        }


    }
}
