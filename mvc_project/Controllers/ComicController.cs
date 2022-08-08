using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc_project.Models;
using mvc_project.Models.Common;
using mvc_project.Models.Login;
using mvc_project.Models.Comic;

namespace mvc_project.Controllers
{
    public class ComicController : Controller
    {
        private readonly ILogger<ComicController> _logger;

        public ComicController(ILogger<ComicController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>(
                                "ComicLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            return View();
        }

        public IActionResult Nuevo()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("ComicLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            ComicViewModel comicViewModel = new ComicViewModel
            {
                autorLibro = "",
                id = 0,
                tituloLibro = "",
                nombreComic = "",
                accion = CodigosAccion.Nuevo
            };

            return View("~/Views/Comic/Comic.cshtml", comicViewModel);
        }

        public IActionResult Editar(long idComic)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("ComicLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<ComicModel> list = HttpContext.Session.Get<List<ComicModel>>("ListaComics");

            if(list == null)
            {
                list = new List<ComicModel>();
            }

            ComicModel comicModel = list.Find(x => x.id == idComic);

            ComicViewModel comicViewModel = new ComicViewModel 
            {
                accion = CodigosAccion.Editar,
                autorLibro = comicModel.autorLibro,
                id = comicModel.id,
                tituloLibro = comicModel.tituloLibro,
                nombreComic = comicModel.nombreComic
            };

            return View("~/Views/Comic/Comic.cshtml", comicViewModel);
        }

        public IActionResult Ver(long idComic)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("ComicLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<ComicModel> list = HttpContext.Session.Get<List<ComicModel>>("ListaComics");

            if(list == null)
            {
                list = new List<ComicModel>();
            }

            ComicModel comicModel = list.Find(x => x.id == idComic);

            ComicViewModel comicViewModel = new ComicViewModel 
            {
                accion = CodigosAccion.Ver,
                autorLibro = comicModel.autorLibro,
                id = comicModel.id,
                tituloLibro = comicModel.tituloLibro,
                nombreComic = comicModel.nombreComic
            };

            return View("~/Views/Comic/Comic.cshtml", comicViewModel);
        }

        [HttpPost]
        public JsonResult Listar(QueryGridModel queryGridModel)
        {
            List<ComicModel> list = HttpContext.Session.Get<List<ComicModel>>("ListaComics");
            
            if(list == null)
            {
                list = new List<ComicModel>();
            }

            IEnumerable<ComicModel> listaComics = list;
            if(queryGridModel.order != null && queryGridModel.order.Count > 0)
            {
                if(queryGridModel.columns[queryGridModel.order[0].column].name == "tituloLibro")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaComics = list.OrderBy(x => x.tituloLibro);
                    }
                    else
                    {
                        listaComics = list.OrderByDescending(x => x.tituloLibro);
                    }
                }
                else if(queryGridModel.columns[queryGridModel.order[0].column].name == "autorLibro")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaComics = list.OrderBy(x => x.autorLibro);
                    }
                    else
                    {
                        listaComics = list.OrderByDescending(x => x.autorLibro);
                    }
                }
                else if(queryGridModel.columns[queryGridModel.order[0].column].name == "nombreComic")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaComics = list.OrderBy(x => x.nombreComic);
                    }
                    else
                    {
                        listaComics = list.OrderByDescending(x => x.nombreComic);
                    }
                }
            }

            if(queryGridModel.search != null && queryGridModel.search.value != null)
            {
                listaComics = listaComics.Where(x => x.nombreComic.Contains(queryGridModel.search.value));
            }

            return Json(JsonReturn.SuccessWithInnerObject(new
            {
                draw = queryGridModel.draw,
                recordsFiltered = listaComics.Count(),
                recordsTotal = list.Count,
                data = listaComics
            }));
        }

        [HttpPost]
        public JsonResult Guardar(ComicModel comicModel)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("ComicLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<ComicModel> list = HttpContext.Session.Get<List<ComicModel>>("ListaComics");

            if(list == null)
            {
                list = new List<ComicModel>();
            }

            if(comicModel.id == 0)
            {
                comicModel.id = list.Count + 1;
                list.Add(comicModel);
            }
            else
            {
                ComicModel comic = list.Find(x => x.id == comicModel.id);
                comic.autorLibro = comicModel.autorLibro;
                comic.tituloLibro = comicModel.tituloLibro;
                if(!string.IsNullOrEmpty(comicModel.password))
                {
                    comic.password = comicModel.password;
                }
                comic.nombreComic = comicModel.nombreComic;
            }

            HttpContext.Session.Set<List<ComicModel>>("ListaComics", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }

        [HttpPost]
        public JsonResult Eliminar(long id)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("ComicLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<ComicModel> list = HttpContext.Session.Get<List<ComicModel>>("ListaComics");

            if(list == null)
            {
                list = new List<ComicModel>();
            }

            ComicModel comic = list.Find(x => x.id == id);
            
            if(comic == null)
            {
                return Json(Models.Common.JsonReturn.ErrorWithSimpleMessage("El comic que desea eliminar no existe más"));
            }
            
            list.Remove(comic);
            
            HttpContext.Session.Set<List<ComicModel>>("ListaComics", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }
    }
}
