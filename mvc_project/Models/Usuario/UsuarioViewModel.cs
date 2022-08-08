using mvc_project.Models.Common;

namespace mvc_project.Models.Usuario
{
    public class UsuarioViewModel
    {
        public long id { get; set; }

        public string nombreUsuario { get; set; }

        public string tituloLibro { get; set; }

        public string autorLibro { get; set; }

        public CodigosAccion accion { get; set; }
    }
}
