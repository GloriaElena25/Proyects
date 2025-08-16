using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BinPrueba.Entidad
{
    public class BilleteraViewModel
    {
        public string NombreUsuario { get; set; }
        public decimal SaldoActual { get; set; }
        public List<Transaccion> Historial { get; set; }
    }
}