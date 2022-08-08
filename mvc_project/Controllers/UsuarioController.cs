using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc_project.Models;
using mvc_project.Models.Common;
using mvc_project.Models.Login;
using mvc_project.Models.Usuario;

namespace mvc_project.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>(
                                "UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            return View();
        }

        public IActionResult Nuevo()
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            UsuarioViewModel usuarioViewModel = new UsuarioViewModel
            {
                autorLibro = "",
                id = 0,
                tituloLibro = "",
                nombreUsuario = "",
                accion = CodigosAccion.Nuevo
            };

            return View("~/Views/Usuario/Usuario.cshtml", usuarioViewModel);
        }

        public IActionResult Editar(long idUsuario)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            UsuarioModel usuarioModel = list.Find(x => x.id == idUsuario);

            UsuarioViewModel usuarioViewModel = new UsuarioViewModel 
            {
                accion = CodigosAccion.Editar,
                autorLibro = usuarioModel.autorLibro,
                id = usuarioModel.id,
                tituloLibro = usuarioModel.tituloLibro,
                nombreUsuario = usuarioModel.nombreUsuario
            };

            return View("~/Views/Usuario/Usuario.cshtml", usuarioViewModel);
        }

        public IActionResult Ver(long idUsuario)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Redirect("~/Home/Index");
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            UsuarioModel usuarioModel = list.Find(x => x.id == idUsuario);

            UsuarioViewModel usuarioViewModel = new UsuarioViewModel 
            {
                accion = CodigosAccion.Ver,
                autorLibro = usuarioModel.autorLibro,
                id = usuarioModel.id,
                tituloLibro = usuarioModel.tituloLibro,
                nombreUsuario = usuarioModel.nombreUsuario
            };

            return View("~/Views/Usuario/Usuario.cshtml", usuarioViewModel);
        }

        [HttpPost]
        public JsonResult Listar(QueryGridModel queryGridModel)
        {
            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaLibros");
            
            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            IEnumerable<UsuarioModel> listaLibros = list;
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
                else if(queryGridModel.columns[queryGridModel.order[0].column].name == "nombreUsuario")
                {
                    if(queryGridModel.order[0].dir == DirectionModel.asc)
                    {
                        listaLibros = list.OrderBy(x => x.nombreUsuario);
                    }
                    else
                    {
                        listaLibros = list.OrderByDescending(x => x.nombreUsuario);
                    }
                }
            }

            if(queryGridModel.search != null && queryGridModel.search.value != null)
            {
                listaLibros = listaLibros.Where(x => x.nombreUsuario.Contains(queryGridModel.search.value));
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
        public JsonResult Guardar(UsuarioModel usuarioModel)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            if(usuarioModel.id == 0)
            {
                usuarioModel.id = list.Count + 1;
                list.Add(usuarioModel);
            }
            else
            {
                UsuarioModel usuario = list.Find(x => x.id == usuarioModel.id);
                usuario.autorLibro = usuarioModel.autorLibro;
                usuario.tituloLibro = usuarioModel.tituloLibro;
                if(!string.IsNullOrEmpty(usuarioModel.password))
                {
                    usuario.password = usuarioModel.password;
                }
                usuario.nombreUsuario = usuarioModel.nombreUsuario;
            }

            HttpContext.Session.Set<List<UsuarioModel>>("ListaLibros", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }

        [HttpPost]
        public JsonResult Eliminar(long id)
        {
            LoginModel loginModel = HttpContext.Session.Get<LoginModel>("UsuarioLogueado");

            if(loginModel == null)
            {
                return Json(Models.Common.JsonReturn.Redirect("Home/Index"));
            }

            List<UsuarioModel> list = HttpContext.Session.Get<List<UsuarioModel>>("ListaLibros");

            if(list == null)
            {
                list = new List<UsuarioModel>();
            }

            UsuarioModel usuario = list.Find(x => x.id == id);
            
            if(usuario == null)
            {
                return Json(Models.Common.JsonReturn.ErrorWithSimpleMessage("El usuario que desea eliminar no existe más"));
            }
            
            list.Remove(usuario);
            
            HttpContext.Session.Set<List<UsuarioModel>>("ListaLibros", list);

            return Json(JsonReturn.SuccessWithoutInnerObject());
        }
    }
}
