using mvc_project.Models.Common;

namespace mvc_project.Models.Libro
{
    public class LibroViewModel
    {
        public long id { get; set; }

        public string nombreLibro { get; set; }

       

        public string tituloLibro { get; set; }

        public string autorLibro { get; set; }

        public CodigosAccion accion { get; set; }
    }
}
