using System.Collections.Generic;
using System.Collections;
using System;

namespace CentralAgentesMvc.Models
{
    [Serializable]
    public class SearchViewModel
    {
        public string CtrlModelID { get; set; }
        public string CtrlDescripcionModel { get; set; }
        public string CtrlSpecificModel { get; set; }

        public int MarcaID { get; set; }
        public int SubMarcaID { get; set; }
        public string Modelo { get; set; }
        public string SearchText { get; set; }
        public string ModelosDisponibles { get; set; }

        public Dictionary<string, string> YearsSource { get; set; }
        public IEnumerable MarcasSource { get; set; }
        public IEnumerable SubMarcasSource { get; set; }
    }
}