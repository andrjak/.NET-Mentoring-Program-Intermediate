using System.IO;
using System.Linq;
using System.Web.Mvc;
using ProfileSample.DAL;
using ProfileSample.Models;

namespace ProfileSample.Controllers
{
    public class HomeController : Controller
    {
        private const int ImageCount = 20;

        public ActionResult Index()
        {
            var context = new ProfileSampleEntities();

            var sources = context.ImgSources
                .Take(ImageCount)
                .ToList();

            return View(
                sources.Select(dbImage => new ImageModel()
                {
                    Data = $"data:image/jpg;base64,{System.Convert.ToBase64String(dbImage.Data)}",
                }
            ).ToList());
        }

        // Direct call only
        public ActionResult Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

            using (var context = new ProfileSampleEntities())
            {
                var imagesToAdd = files.Select(file =>
                {
                    using (var stream = new FileStream(file, FileMode.Open))
                    {
                        var buff = new byte[stream.Length];
                        stream.Read(buff, 0, (int)stream.Length);

                        return new ImgSource()
                        {
                            Name = Path.GetFileName(file),
                            Data = buff,
                        };
                    }
                });

                context.ImgSources.AddRange(imagesToAdd);
                context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}