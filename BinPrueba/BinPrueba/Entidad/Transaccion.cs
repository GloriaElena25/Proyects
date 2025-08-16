using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BinPrueba.Entidad
{
    public class Transaccion
    {
        public string Destinatario { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }

    }
}