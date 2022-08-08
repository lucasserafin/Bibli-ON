using mvc_project.Models.Common;

namespace mvc_project.Models.Comic
{
    public class ComicViewModel
    {
        public long id { get; set; }

        public string nombreComic { get; set; }

        public string tituloLibro { get; set; }

        public string autorLibro { get; set; }

        public CodigosAccion accion { get; set; }
    }
}
