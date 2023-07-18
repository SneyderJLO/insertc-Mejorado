
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
            Dictionary<string, List<string>> tipoRazonPorId = new Dictionary<string, List<string>>();
            try
            {
                rutaConexion.DataSource = "192.168.1.55";
                rutaConexion.UserID = "sa";
                rutaConexion.Password = "123*abc*456";
                // rutaConexion.IntegratedSecurity = false;
                rutaConexion.InitialCatalog = "DB_PRACTICAS";
                conexion = new SqlConnection(rutaConexion.ToString());
                conexion.Open();
                string primerValorTipoRazon = "", tipoTrx = "";
                Random random = new Random();
                Dictionary<int, string> keyValueTipoTrx = obtenerTipoTrx(conexion);
                Dictionary<string, int> dicionarioFilasTrx = obtenerFilasTipoTrx(keyValueTipoTrx, conexion);
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
                    int pos = random.Next(1, 55);
                    tipoTrx = keyValueTipoTrx[pos];
                    primerValorTipoRazon = obtenerRazonesPorTipo(tipoRazonPorId, dicionarioFilasTrx, keyValueTipoTrx, pos, conexion);
                    fila = dt.NewRow();
                    fila["pr_id"] = i;
                    fila["pr_fecha"] = DateTime.Now.AddMilliseconds(1);
                    int comercio = random.Next(1, 10);
                    fila["pr_comercio"] = comercio;
                    fila["pr_tarjeta"] = GenerarNumeroAleatorio();
                    double numAleatorio = random.NextDouble() * 1000;
                    decimal valor = Convert.ToDecimal(numAleatorio);
                    fila["pr_valor"] = valor;
                    fila["pr_tipoTrx"] = tipoTrx;
                    fila["pr_razon"] = primerValorTipoRazon;
                    fila["pr_autoriza"] = "ney";
                    dt.Rows.Add(fila);
                    if (i % 50000 == 0)
                    {
                        SqlBulkCopy bulkCopy = new SqlBulkCopy(conexion);
                        bulkCopy.DestinationTableName = "pr_transacciones";
                        bulkCopy.WriteToServer(dt);
                        dt.Clear();
                        //bulkCopy.Close();
                        Console.WriteLine(i);
                    }
                }
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
            try
            {
                command = new SqlCommand(query, conexion);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    keyValueTipoTrx.Add(indexTipo, reader[0].ToString());
                    indexTipo++;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return keyValueTipoTrx;
        }


        static Dictionary<string, int> obtenerFilasTipoTrx(Dictionary<int, string> keyValueTipoTrx, SqlConnection conexion)
        {
            string query = "";
            SqlCommand command = null;
            Dictionary<string, int> dictionaryFilas = new Dictionary<string, int>();
            try
            {
                foreach (KeyValuePair<int, string> item in keyValueTipoTrx)
                {
                    query = $"select count(*) from TRtrx where id_tipo = {keyValueTipoTrx[item.Key]};";
                    command = new SqlCommand(query, conexion);
                    dictionaryFilas.Add(keyValueTipoTrx[item.Key], Convert.ToInt32(command.ExecuteScalar()));
                }
                //foreach (KeyValuePair<string, int> item in dictionaryFilas)
                //{
                // Console.WriteLine($"{item.Key} -- {item.Value}");
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dictionaryFilas;
        }


        public static string obtenerRazonesPorTipo(Dictionary<string, List<string>> tipoRazonPorId, Dictionary<string, int> dicionarioFilasTrx, Dictionary<int, string> dicionatioTipo, int pos, SqlConnection conexion)
        {
            string razonValue;
            Random random = new Random();
            razonValue = "0";
            string query = $"select id_razon from TRtrx where id_tipo = {dicionatioTipo[pos]};";

            int filas = dicionarioFilasTrx[dicionatioTipo[pos]];
            try
            {

                SqlCommand command = new SqlCommand(query, conexion);

                if (tipoRazonPorId.ContainsKey(dicionatioTipo[pos].ToString()))
                {
                    int randomTipo = random.Next(0, filas);
                    razonValue = tipoRazonPorId[dicionatioTipo[pos].ToString()][randomTipo];

                }
                else
                {
                    query = $"select id_razon from TRtrx where id_tipo = {dicionatioTipo[pos]};";
                    command = new SqlCommand(query, conexion);

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string idRazon = reader[0].ToString();

                        if (!tipoRazonPorId.ContainsKey(dicionatioTipo[pos].ToString()))
                        {
                            tipoRazonPorId[dicionatioTipo[pos].ToString()] = new List<string>();
                        }

                        tipoRazonPorId[dicionatioTipo[pos].ToString()].Add(idRazon);
                    }
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return razonValue;
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

        public static long GenerarNumeroAleatorio()
        {
            Guid guid = Guid.NewGuid();
            byte[] bytes = guid.ToByteArray();
            long numeroAleatorio = BitConverter.ToInt64(bytes, 0);
            numeroAleatorio = Math.Abs(numeroAleatorio);
            numeroAleatorio %= 9000000000;
            numeroAleatorio += 1000000000000000;
            return numeroAleatorio;
        }
        //Console.WriteLine(primerValorTipoRazon);
        //if (tipoRazonPorId.ContainsKey(keyValueTipoTrx[pos]))
        //{
        // List<string> valores = tipoRazonPorId[keyValueTipoTrx[pos]];
        // foreach (string valor in valores)
        // {
        // Console.WriteLine($"Clave:{keyValueTipoTrx[pos]}, Valor: {valor}");
        // }
        //}
        //else
        //{

        //foreach (KeyValuePair<int, string> kvp in keyValueTipoTrx)
        //{
        // Console.WriteLine("key: " + kvp.Key + ", value: " + kvp.Value);
        //}

        // Console.WriteLine(tipoRazonPorId[keyValueTipoTrx[pos][0].ToString()]);




        // Console.WriteLine("No se encontro el codigo");
        //}
    }
}
