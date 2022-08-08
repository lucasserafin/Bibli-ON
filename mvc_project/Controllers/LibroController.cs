using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc_project.Models;
using mvc_project.Models.Common;
using mvc_project.Models.Login;
using mvc_project.Models.Libro;

namespace mvc_project.Controllers
{
    public class LibroController : Controller
    {
        private readonly ILogger<LibroController> _logger;

        public LibroController(ILogger<LibroController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>(
                                "LibroLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            return View();
        }

        public IActionResult Nuevo()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("LibroLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            LibroViewModel libroViewModel = new LibroViewModel
            {
                autorLibro = "",
                id = 0,
                tituloLibro = "",
                nombreLibro = "",
               
                accion = CodigosAccion.Nuevo
            };

            return View("~/Views/Libro/Libro.cshtml", libroViewModel);
        }

        public IActionResult Editar(long idLibro)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("LibroLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<LibroModel> list = HttpContext.Session.Get<List<LibroModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<LibroModel>();
            }

            LibroModel libroModel = list.Find(x => x.id == idLibro);

            LibroViewModel libroViewModel = new LibroViewModel 
            {
                accion = CodigosAccion.Editar,
                autorLibro = libroModel.autorLibro,
                id = libroModel.id,
                tituloLibro = libroModel.tituloLibro,
                nombreLibro = libroModel.nombreLibro,
                
            };

            return View("~/Views/Libro/Libro.cshtml", libroViewModel);
        }

        public IActionResult Ver(long idLibro)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("LibroLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<LibroModel> list = HttpContext.Session.Get<List<LibroModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<LibroModel>();
            }

            LibroModel libroModel = list.Find(x => x.id == idLibro);

            LibroViewModel libroViewModel = new LibroViewModel 
            {
                accion = CodigosAccion.Ver,
                autorLibro = libroModel.autorLibro,
                id = libroModel.id,
                tituloLibro = libroModel.tituloLibro,
                nombreLibro = libroModel.nombreLibro,
               
            };

            return View("~/Views/Libro/Libro.cshtml", libroViewModel);
        }

        [HttpPost]
        public JsonResult Listar(QueryGridModel queryGridModel)
        {
            List<LibroModel> list = HttpContext.Session.Get<List<LibroModel>>("ListaLibros");
            
            if(list == null)
            {
                list = new List<LibroModel>();
            }

            IEnumerable<LibroModel> listaLibros = list;
            if(queryGridModel.order != null && queryGridModel.order.Count > 0)
            {
                if(queryGridModel.columns[queryGridModel.order[0].column].name == "tituloLibro")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaLibros = list.OrderBy(x => x.tituloLibro);
                    }
                    else
                    {
                        listaLibros = list.OrderByDescending(x => x.tituloLibro);
                    }
                }
                else if(queryGridModel.columns[queryGridModel.order[0].column].name == "autorLibro")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaLibros = list.OrderBy(x => x.autorLibro);
                    }
                    else
                    {
                        listaLibros = list.OrderByDescending(x => x.autorLibro);
                    }
                }
                else if(queryGridModel.columns[queryGridModel.order[0].column].name == "nombreLibro")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaLibros = list.OrderBy(x => x.nombreLibro);
                    }
                    else
                    {
                        listaLibros = list.OrderByDescending(x => x.nombreLibro);
                    }
                }
            }

            if(queryGridModel.search != null && queryGridModel.search.value != null)
            {
                listaLibros = listaLibros.Where(x => x.nombreLibro.Contains(queryGridModel.search.value));
            }

            return Json(JsonReturn.SuccessWithInnerObject(new
            {
                draw = queryGridModel.draw,
                recordsFiltered = listaLibros.Count(),
                recordsTotal = list.Count,
                data = listaLibros
            }));
        }

        [HttpPost]
        public JsonResult Guardar(LibroModel libroModel)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("LibroLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<LibroModel> list = HttpContext.Session.Get<List<LibroModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<LibroModel>();
            }

            if(libroModel.id == 0)
            {
                libroModel.id = list.Count + 1;
                list.Add(libroModel);
            }
            else
            {
                LibroModel libro = list.Find(x => x.id == libroModel.id);
                libro.autorLibro = libroModel.autorLibro;
                libro.tituloLibro = libroModel.tituloLibro;
                if(!string.IsNullOrEmpty(libroModel.password))
                {
                    libro.password = libroModel.password;
                }
                libro.nombreLibro = libroModel.nombreLibro;
            }

            HttpContext.Session.Set<List<LibroModel>>("ListaLibros", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }

        [HttpPost]
        public JsonResult Eliminar(long id)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("LibroLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<LibroModel> list = HttpContext.Session.Get<List<LibroModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<LibroModel>();
            }

            LibroModel libro = list.Find(x => x.id == id);
            
            if(libro == null)
            {
                return Json(Models.Common.JsonReturn.ErrorWithSimpleMessage("El libro que desea eliminar no existe más"));
            }
            
            list.Remove(libro);
            
            HttpContext.Session.Set<List<LibroModel>>("ListaLibros", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }
    }
}
