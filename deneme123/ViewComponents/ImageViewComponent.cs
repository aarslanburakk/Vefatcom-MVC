using deneme123.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace deneme123.ViewComponents
{
    public class ImageViewComponent: ViewComponent
    {

        private IDeadRepository repository;
        public ImageViewComponent(IDeadRepository _repository)
        {

            repository = _repository;
        }
        public ViewViewComponentResult Invoke()
        {
            var image = repository.GetImages();

            return View(image);
        }


    }
}

