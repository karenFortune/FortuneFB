using FortuneSystem.Models.Item;
using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.PrintShop
{
    public class PrintShopData
    {
        CatUsuarioData objCatUser = new CatUsuarioData();
        ItemTallaData objTalla = new ItemTallaData();
        //Muestra la lista de tallas de PrintShop por estilo
        public IEnumerable<PrintShopC> ListaTallasPrintShop(int? id)
        {
            Conexion conn = new Conexion();
            List<PrintShopC> listTallas = new List<PrintShopC>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_PrintShop";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int i = 0;
                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {
                        Talla = leer["TALLA"].ToString(),
                        Printed = Convert.ToInt32(leer["PRINTED"]),
                        MisPrint = Convert.ToInt32(leer["MISPRINT"]),
                        Defect = Convert.ToInt32(leer["DEFECT"]),
                        Repair = Convert.ToInt32(leer["REPAIR"])

                    };

                    listTallas.Add(tallas);
                    i++;
                }
                if (i == 0)
                {
                    listTallas = ObtenerTallas(id);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas de Batch por id
        public IEnumerable<PrintShopC> ListaTallasBatchId(int? id)
        {
            Conexion conn = new Conexion();
            List<PrintShopC> listTallas = new List<PrintShopC>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Batch";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leer["ID_BATCH"]),
                        Printed = Convert.ToInt32(leer["PRINTED"]),
                        MisPrint = Convert.ToInt32(leer["MISPRINT"]),
                        Defect = Convert.ToInt32(leer["DEFECT"]),
                        Repair = Convert.ToInt32(leer["REPAIR"])

                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL de PrintShop por estilo
        public IEnumerable<PrintShopC> ListaTallasTotalPrintShop(int? id)
        {
            Conexion conn = new Conexion();
            List<PrintShopC> listTallas = new List<PrintShopC>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Total_PrintShop";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {

                        Printed = Convert.ToInt32(leer["TOTAL"])
                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL de PrintShop por estilo
        public List<PrintShopC> ObtenerTallas(int? id)
        {
            Conexion conn = new Conexion();
            List<PrintShopC> listTallas = new List<PrintShopC>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select S.TALLA from ITEM_SIZE I " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=I.TALLA_ITEM " +
                    "WHERE I.ID_SUMMARY= '" + id + "' ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {

                        Talla = leer["TALLA"].ToString()

                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }


        //Muestra la lista de tallas TOTAL de PrintShop por estilo
        public IEnumerable<int> ListaTotalTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct cast(S.ORDEN AS int) as codigo, S.TALLA FROM PRINTSHOP T  " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ";
                leer = comando.ExecuteReader();
                int total = 0;
                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {
                     
                        Talla = leer["TALLA"].ToString()

                    };
                    tallas.IdTalla = objTalla.ObtenerIdTallas(tallas.Talla);
                    total = SumaTotalBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(total);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL MisPrint de PrintShop por estilo
        public IEnumerable<int> ListaTotalMPTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct cast(S.ORDEN AS int) as codigo, S.TALLA FROM PRINTSHOP T  " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ";
                leer = comando.ExecuteReader();
                int totalMisPrint = 0;
                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {
                       
                        Talla = leer["TALLA"].ToString()

                    };
                    tallas.IdTalla = objTalla.ObtenerIdTallas(tallas.Talla);
                    totalMisPrint = SumaTotalMisprintBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalMisPrint);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
            }

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL Defect de PrintShop por estilo
        public IEnumerable<int> ListaTotalDefTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct cast(S.ORDEN AS int) as codigo, S.TALLA FROM PRINTSHOP T  " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ";
                leer = comando.ExecuteReader();
                int totalDefect = 0;
                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {
                        
                        Talla = leer["TALLA"].ToString()

                    };
                    tallas.IdTalla = objTalla.ObtenerIdTallas(tallas.Talla);
                    totalDefect = SumaTotalDefectBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalDefect);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas TOTAL en Reparacion de PrintShop por estilo
        public IEnumerable<int> ListaTotalRepTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct cast(S.ORDEN AS int) as codigo, S.TALLA FROM PRINTSHOP T  " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ";
                leer = comando.ExecuteReader();
                int totalRepair = 0;
                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {
                        Talla = leer["TALLA"].ToString()
                    };
                    tallas.IdTalla = objTalla.ObtenerIdTallas(tallas.Talla);
                    totalRepair = SumaTotalRepairBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalRepair);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return listTallas;
        }

        //Muestra la lista de tallas TOTAL Printed de PrintShop por estilo
        public IEnumerable<int> ListaTotalPrintedTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct cast(S.ORDEN AS int) as codigo, S.TALLA FROM PRINTSHOP T  " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ";
                leer = comando.ExecuteReader();
                int totalPrinted = 0;
                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {

                        Talla = leer["TALLA"].ToString()
                        
                    };
                    tallas.IdTalla = objTalla.ObtenerIdTallas(tallas.Talla);
                    totalPrinted = SumaTotalPrintedBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalPrinted);

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de suma de  tallas por Batch
        public int SumaTotalBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT PRINTED, MISPRINT, DEFECT, REPAIR  FROM PRINTSHOP WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["PRINTED"]) + Convert.ToInt32(leerF["MISPRINT"]) + Convert.ToInt32(leerF["DEFECT"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }

        //Muestra la lista de suma de Printed tallas por Batch
        public int SumaTotalPrintedBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT PRINTED  FROM PRINTSHOP WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["PRINTED"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }

        //Muestra la lista de suma de MisPrint tallas por Batch
        public int SumaTotalMisprintBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT MISPRINT  FROM PRINTSHOP WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["MISPRINT"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }


            return suma;
        }


        //Muestra la lista de suma de DEFECT tallas por Batch
        public int SumaTotalDefectBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT DEFECT  FROM PRINTSHOP WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {
                    suma += Convert.ToInt32(leerF["DEFECT"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }

        //Muestra la lista de suma de Reparaciones tallas por Batch
        public int SumaTotalRepairBacheTalla(int? idEstilo, int idTalla)
        {
            Conexion conex = new Conexion();
            int suma = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conex.AbrirConexion();
                com.CommandText = "SELECT REPAIR  FROM PRINTSHOP WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = com.ExecuteReader();
                while (leerF.Read())
                {

                    if (!Convert.IsDBNull(leerF["REPAIR"]))
                    {
                        suma += Convert.ToInt32(leerF["REPAIR"]);
                    }
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return suma;
        }

        //Muestra la lista de tallas de Batch por estilo
        public IEnumerable<PrintShopC> ListaBatch(int? id)
        {
            Conexion conn = new Conexion();
            List<PrintShopC> listTallas = new List<PrintShopC>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct ID_BATCH FROM PRINTSHOP WHERE ID_SUMMARY='" + id + "'";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {

                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])


                    };
                    tallas.Batch = ListaTallasBatch(tallas.IdBatch, id);

                    foreach (var item in tallas.Batch)
                    {
                        tallas.TipoTurno = item.TipoTurno;
                        tallas.NombreUsr = item.NombreUsr;
                        tallas.IdPrintShop = item.IdPrintShop;
                        tallas.Maquina = item.Maquina;
                        ObtenerNombreMaquina(tallas);
                        tallas.NombreUsrModif = item.NombreUsrModif;
                        tallas.Status = item.Status;
                        tallas.Cargo = item.Cargo;
                        tallas.Comentarios = item.Comentarios;
                    }




                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        public void ObtenerNombreMaquina(PrintShopC printShop)
        {
            switch (printShop.Maquina)
            {
                case 1:
                    printShop.NombreMaquina = "Machine 1";
                    break;
                case 2:
                    printShop.NombreMaquina = "Machine 2";
                    break;
                case 3:
                    printShop.NombreMaquina = "Machine 3";
                    break;
                case 4:
                    printShop.NombreMaquina = "Machine 4";
                    break;
                case 5:
                    printShop.NombreMaquina = "Machine 5";
                    break;
                case 6:
                    printShop.NombreMaquina = "Machine 6";
                    break;
                case 7:
                    printShop.NombreMaquina = "Machine 7";
                    break;
                case 8:
                    printShop.NombreMaquina = "Machine 8";
                    break;
                case 9:
                    printShop.NombreMaquina = "Machine 9";
                    break;
                case 10:
                    printShop.NombreMaquina = "Machine 10";
                    break;
                case 11:
                    printShop.NombreMaquina = "Machine 11";
                    break;
                case 12:
                    printShop.NombreMaquina = "Machine 12";
                    break;
                default:
                    printShop.NombreMaquina = "-";
                    break;
            }
        }
        //Muestra la lista de tallas de UN Batch por estilo y id Batch seleccionado
        public IEnumerable<PrintShopC> ListaCantidadesTallaPorIdBatchEstilo(int? idEstilo, int idBatch)
        {
            Conexion conn = new Conexion();
            List<PrintShopC> listTallas = new List<PrintShopC>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PRINTSHOP, ID_TALLA, S.TALLA, PRINTED, MISPRINT, DEFECT, REPAIR FROM PRINTSHOP " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=PRINTSHOP.ID_TALLA " +
                    "WHERE ID_SUMMARY='" + idEstilo + "' AND ID_BATCH='" + idBatch + " 'ORDER by cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {

                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Printed = Convert.ToInt32(leer["PRINTED"]),
                        MisPrint = Convert.ToInt32(leer["MISPRINT"]),
                        Defect = Convert.ToInt32(leer["DEFECT"]),
                        Repair = Convert.ToInt32(leer["REPAIR"])


                    };

                    listTallas.Add(tallas);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de tallas por Batch
        public List<PrintShopC> ListaTallasBatch(int? batch, int? id)
        {
            Conexion conex = new Conexion();
            List<PrintShopC> listTallas = new List<PrintShopC>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "SELECT P.ID_PRINTSHOP, P.ID_SUMMARY, P.ID_BATCH, CONCAT(U.Nombres,' ',U.Apellidos)AS NOMBRE,P.TURNO, P.MAQUINA, P.ID_USUARIO_MODIF, P.STATUS_PALLET, " +
                    " P.ID_TALLA, S.TALLA, P.PRINTED, P.MISPRINT, P.DEFECT, P.REPAIR, sum(PRINTED+MISPRINT+DEFECT+REPAIR)AS TOTAL, P.COMENTARIOS FROM PRINTSHOP P " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=P.ID_TALLA " +
                    "INNER JOIN USUARIOS U ON U.Id=P.ID_USUARIO " +
                    "WHERE P.ID_BATCH='" + batch + "' AND P.ID_SUMMARY='" + id + "'  GROUP BY P.ID_PRINTSHOP,P.ID_SUMMARY, P.ID_BATCH, P.ID_TALLA, S.TALLA, " +
                    "P.PRINTED, P.MISPRINT, P.DEFECT, P.REPAIR, U.Nombres, U.Apellidos, P.TURNO, P.MAQUINA, P.ID_USUARIO_MODIF,P.STATUS_PALLET,P.COMENTARIOS,S.ORDEN ORDER by cast(S.ORDEN AS int) ASC ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    PrintShopC tallas = new PrintShopC()
                    {
                        Talla = leerF["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leerF["ID_BATCH"]),
                        IdPrintShop = Convert.ToInt32(leerF["ID_PRINTSHOP"]),
                        IdSummary = Convert.ToInt32(leerF["ID_SUMMARY"]),
                        TipoTurno = Convert.ToInt32(leerF["TURNO"]),
                        NombreUsr = leerF["NOMBRE"].ToString(),
                        Printed = Convert.ToInt32(leerF["PRINTED"]),
                        MisPrint = Convert.ToInt32(leerF["MISPRINT"]),
                        Defect = Convert.ToInt32(leerF["DEFECT"]),
                        Total = Convert.ToInt32(leerF["TOTAL"]),
                        Comentarios = leerF["COMENTARIOS"].ToString()



                    };
                    if (!Convert.IsDBNull(leerF["REPAIR"]))
                    {
                        tallas.Repair = Convert.ToInt32(leerF["REPAIR"]);
                    }

                    if (!Convert.IsDBNull(leerF["STATUS_PALLET"]))
                    {
                        tallas.EstadoPallet = Convert.ToBoolean(leerF["STATUS_PALLET"]);
                    }

                    if (!Convert.IsDBNull(leerF["MAQUINA"]))
                    {
                        tallas.Maquina = Convert.ToInt32(leerF["MAQUINA"]);
                    }

                    if (!Convert.IsDBNull(leerF["ID_USUARIO_MODIF"]))
                    {
                        tallas.UsuarioModif = Convert.ToInt32(leerF["ID_USUARIO_MODIF"]);
                    }

                    if (tallas.UsuarioModif != 0)
                    {
                        tallas.NombreUsrModif = objCatUser.Obtener_Nombre_Usuario_PorID(tallas.UsuarioModif);
                    }
                    else
                    {
                        tallas.NombreUsrModif = "-";
                    }

                    if (tallas.EstadoPallet != false)
                    {
                        tallas.Status = "C";
                    }
                    else
                    {
                        tallas.Status = "I";
                    }


                    listTallas.Add(tallas);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return listTallas;
        }

        //Agregar las tallas de un batch
        public void AgregarTallasPrintShop(PrintShopC printShop)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarPrintShop";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@idSummary", printShop.IdSummary);
                com.Parameters.AddWithValue("@idBatch", printShop.IdBatch);
                com.Parameters.AddWithValue("@idTalla", printShop.IdTalla);
                com.Parameters.AddWithValue("@printed", printShop.Printed);
                com.Parameters.AddWithValue("@mp", printShop.MisPrint);
                com.Parameters.AddWithValue("@def", printShop.Defect);
                com.Parameters.AddWithValue("@rep", printShop.Repair);
                com.Parameters.AddWithValue("@maq", printShop.Maquina);
                com.Parameters.AddWithValue("@turno", printShop.TipoTurno);
                com.Parameters.AddWithValue("@idStatus", printShop.EstadoPallet);
                com.Parameters.AddWithValue("@idUsr", printShop.Usuario);
                com.Parameters.AddWithValue("@idUsrAct", printShop.UsuarioModif);
                com.Parameters.AddWithValue("@coment", printShop.Comentarios);
                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }


        }

        //Permite actualizar la información de un batch
        public void ActualizarTallasPrintShop(PrintShopC printShop)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "ActualizarBatchPrintShop";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@id", printShop.IdPrintShop);
                com.Parameters.AddWithValue("@idSummary", printShop.IdSummary);
                com.Parameters.AddWithValue("@idBatch", printShop.IdBatch);
                com.Parameters.AddWithValue("@idTalla", printShop.IdTalla);
                com.Parameters.AddWithValue("@printed", printShop.Printed);
                com.Parameters.AddWithValue("@mp", printShop.MisPrint);
                com.Parameters.AddWithValue("@def", printShop.Defect);
                com.Parameters.AddWithValue("@rep", printShop.Repair);
                com.Parameters.AddWithValue("@maq", printShop.Maquina);
                com.Parameters.AddWithValue("@turno", printShop.TipoTurno);
                com.Parameters.AddWithValue("@idStatus", printShop.EstadoPallet);
                com.Parameters.AddWithValue("@idUsr", printShop.Usuario);
                com.Parameters.AddWithValue("@idUsrAct", printShop.UsuarioModif);
                com.Parameters.AddWithValue("@coment", printShop.Comentarios);


                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

        //Permite obtener el id del batch de los registro 
        public int ObtenerIdBatch(int id)
        {
            int idBatch = 0;
            int idTotal = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT distinct ID_BATCH FROM PRINTSHOP WHERE ID_SUMMARY='" + id + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idBatch += Convert.ToInt32(leerF["ID_BATCH"]);
                    idTotal++;
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idTotal;
        }

        //Permite obtener el idPrintshop del batch de los registro por idestilo
        public int ObtenerIdPrintShopPorBatchEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idPrintShop = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_PRINTSHOP FROM PRINTSHOP WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idPrintShop += Convert.ToInt32(leerF["ID_PRINTSHOP"]);


                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idPrintShop;
        }

        //Permite obtener el idUsuario del batch registrado
        public int ObtenerIdUsuarioPorBatchEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idUsuario = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_USUARIO FROM PRINTSHOP WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idUsuario += Convert.ToInt32(leerF["ID_USUARIO"]);


                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idUsuario;
        }


    }

}