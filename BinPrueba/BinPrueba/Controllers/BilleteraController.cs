using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BinPrueba.Services;
using BinPrueba.Entidad;
using System.Configuration;

namespace BinPrueba.Controllers
{
    public class BilleteraController : Controller
    {
        private readonly BilleteraService _service = new BilleteraService();
        // Obtiene el connection string desde Web.config
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Billetera
        public ActionResult Index(string telefono = null)
        {
            if (!string.IsNullOrEmpty(telefono))
            {
                ViewBag.Historial = _service.ObtenerUltimasTransacciones(telefono);
                ViewBag.TelefonoUsuario = telefono;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Transferir(string telefonoOrigen, string telefonoDestino, decimal monto)
        {
            try
            {
                // Ejecutar el SP
                ViewBag.Mensaje = _service.Transferir(telefonoOrigen, telefonoDestino, monto);

                // Mostrar historial actualizado
                ViewBag.Historial = _service.ObtenerUltimasTransacciones(telefonoOrigen);
                ViewBag.TelefonoUsuario = telefonoOrigen;
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error: " + ex.Message;
            }

            return View("Index");
        }


        public ActionResult Historial(string telefonoUsuario)
        {
            var model = ConstruirModeloBilletera(telefonoUsuario);
            return View("Index", model);
        }

        /// <summary>
        /// Método auxiliar que construye el ViewModel de la billetera
        /// </summary>
        private BilleteraViewModel ConstruirModeloBilletera(string telefonoUsuario)
        {
            var model = new BilleteraViewModel
            {
                Historial = new List<Transaccion>()
            };

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Obtener datos del usuario
                string queryUsuario = "SELECT IdUsuario, Nombre, Saldo FROM Usuarios WHERE Telefono = @Telefono";
                int idUsuario = 0;

                using (var cmd = new SqlCommand(queryUsuario, conn))
                {
                    cmd.Parameters.AddWithValue("@Telefono", telefonoUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idUsuario = Convert.ToInt32(reader["IdUsuario"]);
                            model.NombreUsuario = reader["Nombre"].ToString();
                            model.SaldoActual = Convert.ToDecimal(reader["Saldo"]);
                        }
                    }
                }

                // Obtener historial de transacciones con JOIN para mostrar el nombre del destinatario
                string queryHistorial = @"
                    SELECT u.Nombre AS Destinatario, t.Monto, t.Fecha
                    FROM Transacciones t
                    INNER JOIN Usuarios u ON u.IdUsuario = t.IdDestino
                    WHERE t.IdOrigen = @IdUsuario
                    ORDER BY t.Fecha DESC";

                using (var cmd = new SqlCommand(queryHistorial, conn))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Historial.Add(new Transaccion
                            {
                                Destinatario = reader["Destinatario"].ToString(),
                                Monto = Convert.ToDecimal(reader["Monto"]),
                                Fecha = Convert.ToDateTime(reader["Fecha"])
                            });
                        }
                    }
                }
            }

            return model;
        }
    }
}
