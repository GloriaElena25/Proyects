using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BinPrueba.Services
{
    public class BilleteraService
    {
        private readonly string _connectionString;

        public BilleteraService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public string Transferir(string telefonoOrigen, string telefonoDestino, decimal monto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_Transferir", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TelefonoOrigen", telefonoOrigen);
                    cmd.Parameters.AddWithValue("@TelefonoDestino", telefonoDestino);
                    cmd.Parameters.AddWithValue("@Monto", monto);

                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return "Transferencia realizada con éxito.";
                    }
                    catch (SqlException ex)
                    {
                        return "Error: " + ex.Message;
                    }
                }
            }
        }

        public DataTable ObtenerUltimasTransacciones(string telefono)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT TOP 5 T.Monto, T.Fecha, U.Nombre AS Destinatario
                    FROM Transacciones T
                    INNER JOIN Usuarios U ON T.IdDestino = U.IdUsuario
                    INNER JOIN Usuarios O ON T.IdOrigen = O.IdUsuario
                    WHERE O.Telefono = @Telefono
                    ORDER BY T.Fecha DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            return dt;
        }
    }



}
