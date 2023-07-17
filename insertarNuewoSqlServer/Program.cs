
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

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
                rutaConexion.DataSource = "192.168.1.55";
                rutaConexion.UserID = "sa";
                rutaConexion.Password = "123*abc*456";

                //rutaConexion.IntegratedSecurity = true;
                rutaConexion.InitialCatalog = "DB_PRACTICAS";
                //Console.WriteLine(rutaConexion);
                //databaseDataContext dpPrueba = new databaseDataContext(rutaConexion.ToString());
                int pos = 0, indexTipo = 1, randomTipo = 0;
                string query = "", primerValorTipoRazon = "";
                Dictionary<int, string> keyValueTipoTrx = new Dictionary<int, string>();
                Dictionary<string, List<string>> tipoRazonPorId = new Dictionary<string, List<string>>();



                conexion = new SqlConnection(rutaConexion.ToString());
                conexion.Open();

                Random random = new Random();

                SqlCommand command = null;
                SqlDataReader reader = null;

                query = "select id from tipotrx;";
                command = new SqlCommand(query, conexion);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    keyValueTipoTrx.Add(indexTipo, reader[0].ToString());
                    indexTipo++;
                }
                reader.Close();


                DataTable dt = new DataTable();
                dt.Columns.Add("pr_id", typeof(int));
                dt.Columns.Add("pr_fecha", typeof(DateTime));
                dt.Columns.Add("pr_comercio", typeof(int));
                dt.Columns.Add("pr_tarjeta", typeof(string));
                dt.Columns.Add("pr_valor", typeof(decimal));
                dt.Columns.Add("pr_tipoTrx", typeof(string));
                dt.Columns.Add("pr_razon", typeof(string));
                dt.Columns.Add("pr_autoriza", typeof(string));
                SqlBulkCopy bulkCopy =
                           new SqlBulkCopy(conexion);
                for (int i = 1; i <= 100000; i++)
                {


                    pos = random.Next(1, 55);

                    query = $"select id_razon from TRtrx where id_tipo = {keyValueTipoTrx[pos]};";

                    command = new SqlCommand(query, conexion);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //tipoRazonPorId.Add(keyValueTipoTrx[pos].ToString(), reader[0].ToString());
                        // Console.WriteLine(reader[0].ToString());
                        AddElement(tipoRazonPorId, keyValueTipoTrx[pos], reader[0].ToString());

                    }

                    reader.Close();

                    DataRow fila = dt.NewRow();
                    fila["pr_id"] = i;
                    fila["pr_fecha"] = DateTime.Now;
                    fila["pr_comercio"] = i;
                    fila["pr_tarjeta"] = "1233423213";
                    fila["pr_valor"] = "0";
                    fila["pr_tipoTrx"] = keyValueTipoTrx[pos];
                    if (tipoRazonPorId.ContainsKey(keyValueTipoTrx[pos]))
                    {
                        List<string> valores = tipoRazonPorId[keyValueTipoTrx[pos]];
                        //Console.WriteLine(valores.Count);
                        randomTipo = random.Next(1, valores.Count);
                        randomTipo -= 1;
                        //  Console.WriteLine($"{tipoRazonPorId[keyValueTipoTrx[pos]][randomTipo - 1]}");
                    }






                    primerValorTipoRazon = tipoRazonPorId.ContainsKey(keyValueTipoTrx[pos]) ? tipoRazonPorId[keyValueTipoTrx[pos]][randomTipo] : "0";





                    fila["pr_razon"] = primerValorTipoRazon;
                    fila["pr_autoriza"] = "ney";


                    dt.Rows.Add(fila);


                }

                bulkCopy.DestinationTableName =
                            "pr_transacciones";


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
        private static void OnSqlRowsCopied(
                object sender, SqlRowsCopiedEventArgs e)
        {
            Console.WriteLine("Copied {0} so far...", e.RowsCopied);
        }

        private static void AddElement(Dictionary<string, List<string>> dict, string clave, string valor)
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
