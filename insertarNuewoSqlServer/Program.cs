
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace insertarNuewoSqlServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlConnectionStringBuilder rutaConexion = new SqlConnectionStringBuilder();
            SqlConnection conexion = null;
            try
            {
                rutaConexion.DataSource = "localhost";
                rutaConexion.UserID = "sa";
                rutaConexion.Password = "135678";
                rutaConexion.IntegratedSecurity = true;
                rutaConexion.InitialCatalog = "DB_PRACTICAS";
                conexion = new SqlConnection(rutaConexion.ToString());
                conexion.Open();
                int pos = 0, randomTipo = 0;
                string primerValorTipoRazon = "", tipoTrx = ""; ;
                SqlBulkCopy bulkCopy = new SqlBulkCopy(conexion);
                Random random = new Random();
                Dictionary<int, string> keyValueTipoTrx = obtenerTipoTrx(conexion);
                Dictionary<string, List<string>> tipoRazonPorId = null;
                List<string> valores = null;
                DataRow fila = null;
                DataTable dt = new DataTable();
                dt.Columns.Add("pr_id", typeof(int));
                dt.Columns.Add("pr_fecha", typeof(DateTime));
                dt.Columns.Add("pr_comercio", typeof(int));
                dt.Columns.Add("pr_tarjeta", typeof(string));
                dt.Columns.Add("pr_valor", typeof(decimal));
                dt.Columns.Add("pr_tipoTrx", typeof(string));
                dt.Columns.Add("pr_razon", typeof(string));
                dt.Columns.Add("pr_autoriza", typeof(string));
                for (int i = 1; i <= 100000; i++)
                {
                    pos = random.Next(1, 55);
                    tipoTrx = keyValueTipoTrx[pos];
                    tipoRazonPorId = obtenerRazonesPorTipo(keyValueTipoTrx, pos, conexion);
                    fila = dt.NewRow();
                    fila["pr_id"] = i;
                    fila["pr_fecha"] = DateTime.Now;
                    fila["pr_comercio"] = i;
                    fila["pr_tarjeta"] = "1233423213";
                    fila["pr_valor"] = "0";
                    fila["pr_tipoTrx"] = tipoTrx;
                    if (tipoRazonPorId.ContainsKey(keyValueTipoTrx[pos]))
                    {
                        valores = tipoRazonPorId[keyValueTipoTrx[pos]];
                        randomTipo = random.Next(0, valores.Count);
                    }
                    primerValorTipoRazon = tipoRazonPorId.ContainsKey(keyValueTipoTrx[pos]) ? tipoRazonPorId[keyValueTipoTrx[pos]][randomTipo] : "0";
                    fila["pr_razon"] = primerValorTipoRazon;
                    fila["pr_autoriza"] = "ney";
                    dt.Rows.Add(fila);
                }
                bulkCopy.DestinationTableName = "pr_transacciones";
                bulkCopy.WriteToServer(dt);
                bulkCopy.Close();

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {

                conexion.Close();
            }
        }

        static Dictionary<int, string> obtenerTipoTrx(SqlConnection conexion)
        {
            string query = "";
            int indexTipo = 1;
            SqlCommand command = null;
            SqlDataReader reader = null;
            Dictionary<int, string> keyValueTipoTrx = new Dictionary<int, string>();
            query = "select id from tipotrx;";
            command = new SqlCommand(query, conexion);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                keyValueTipoTrx.Add(indexTipo, reader[0].ToString());
                indexTipo++;
            }
            reader.Close();

            return keyValueTipoTrx;
        }


        static Dictionary<string, List<string>> obtenerRazonesPorTipo(Dictionary<int, string> dicionatioTipo, int pos, SqlConnection conexion)
        {
            Dictionary<string, List<string>> tipoRazonPorId = new Dictionary<string, List<string>>();
            string query = $"select id_razon from TRtrx where id_tipo = {dicionatioTipo[pos]};";
            SqlCommand command = new SqlCommand(query, conexion);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                //tipoRazonPorId.Add(keyValueTipoTrx[pos].ToString(), reader[0].ToString());
                // Console.WriteLine(reader[0].ToString());
                AddElement(tipoRazonPorId, dicionatioTipo[pos], reader[0].ToString());
            }
            reader.Close();
            return tipoRazonPorId;
        }
        private static void OnSqlRowsCopied(
                object sender, SqlRowsCopiedEventArgs e)
        {
            Console.WriteLine("Copied {0} so far...", e.RowsCopied);
        }

        static void AddElement(Dictionary<string, List<string>> dict, string clave, string valor)
        {
            if (dict.ContainsKey(clave))
            {
                dict[clave].Add(valor);
            }
            else
            {
                dict[clave] = new List<string> { valor };
            }
        }
        //Console.WriteLine(primerValorTipoRazon);
        //if (tipoRazonPorId.ContainsKey(keyValueTipoTrx[pos]))
        //{
        //    List<string> valores = tipoRazonPorId[keyValueTipoTrx[pos]];
        //    foreach (string valor in valores)
        //    {
        //        Console.WriteLine($"Clave:{keyValueTipoTrx[pos]}, Valor: {valor}");
        //    }
        //}
        //else
        //{

        //foreach (KeyValuePair<int, string> kvp in keyValueTipoTrx)
        //{
        //    Console.WriteLine("key: " + kvp.Key + ", value: " + kvp.Value);
        //}

        //   Console.WriteLine(tipoRazonPorId[keyValueTipoTrx[pos][0].ToString()]);




        //    Console.WriteLine("No se encontro el codigo");
        //}
    }
}
