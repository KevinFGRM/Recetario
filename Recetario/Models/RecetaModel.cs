using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recetario.Models
{
    public class RecetaModel
    {
        public string nombre { get; set; }
        public List<string> ingredientes { get; set; }
        public string instrucciones { get; set; }
        public string url { get; set; }
    }
}
