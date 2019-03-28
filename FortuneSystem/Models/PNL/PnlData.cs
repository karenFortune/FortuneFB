using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.PNL
{
    public class PnlData
    {
        CatUsuarioData objCatUser = new CatUsuarioData();
        //Muestra la lista de tallas de PNL por estilo
        public IEnumerable<Pnl> ListaTallasPnl(int? id)
        {
            Conexion conn = new Conexion();
            List<Pnl> listTallas = new List<Pnl>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;              
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Pnl";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int i = 0;
                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
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


        //Muestra la lista de tallas TOTAL de Pnl por estilo
        public List<Pnl> ObtenerTallas(int? id)
        {
            Conexion conn = new Conexion();
            List<Pnl> listTallas = new List<Pnl>();
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
                    Pnl tallas = new Pnl()
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



        //Muestra la lista de tallas de Batch por id
        public IEnumerable<Pnl> ListaTallasBatchId(int? id)
        {
            Conexion conn = new Conexion();
            List<Pnl> listTallas = new List<Pnl>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Batch_Pnl";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
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

        //Muestra la lista de tallas TOTAL de PNL por estilo
        public IEnumerable<Pnl> ListaTallasTotalPnl(int? id)
        {
            Conexion conn = new Conexion();
            List<Pnl> listTallas = new List<Pnl>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Total_Pnl";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
                    {

                        Printed = Convert.ToInt32(leer["TOTAL"]),


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

        //Muestra la lista de tallas TOTAL de PNL por estilo
        public IEnumerable<int> ListaTotalTallasPNLBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT T.ID_TALLA,S.TALLA, S.ORDEN FROM PNL T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                int total = 0;
                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
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

        //Muestra la lista de tallas TOTAL MisPrint de Pnl por estilo
        public IEnumerable<int> ListaTotalMPTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT T.ID_TALLA,S.TALLA, S.ORDEN FROM PNL T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                int totalMisPrint = 0;
                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
                    totalMisPrint = SumaTotalMisprintBacheTalla(id, tallas.IdTalla);
                    listTallas.Add(totalMisPrint);
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

        //Muestra la lista de tallas TOTAL Defect de PNL por estilo
        public IEnumerable<int> ListaTotalDefTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT T.ID_TALLA,S.TALLA, S.ORDEN FROM PNL T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                int totalDefect = 0;
                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
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

        //Muestra la lista de tallas TOTAL en Reparacion de pnl por estilo
        public IEnumerable<int> ListaTotalRepTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT T.ID_TALLA,S.TALLA, S.ORDEN FROM PNL T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                int totalRepair = 0;
                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
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
                com.CommandText = "SELECT REPAIR  FROM PNL WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
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

        //Muestra la lista de tallas TOTAL Printed de Pnl por estilo
        public IEnumerable<int> ListaTotalPrintedTallasBatchEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<int> listTallas = new List<int>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT T.ID_TALLA,S.TALLA, S.ORDEN FROM PNL T " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=T.ID_TALLA " +
                    "WHERE T.ID_SUMMARY= '" + id + "' ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                int totalPrinted = 0;
                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
                    {
                        IdTalla = Convert.ToInt32(leer["ID_TALLA"]),
                        Talla = leer["TALLA"].ToString()

                    };
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
                com.CommandText = "SELECT PRINTED, MISPRINT, DEFECT, REPAIR  FROM PNL WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
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
                com.CommandText = "SELECT PRINTED  FROM PNL WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
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
                com.CommandText = "SELECT MISPRINT  FROM PNL WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
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
                com.CommandText = "SELECT DEFECT FROM PNL WHERE ID_SUMMARY='" + idEstilo + "' AND ID_TALLA='" + idTalla + "' ";
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

        //Muestra la lista de tallas de Batch por estilo
        public IEnumerable<Pnl> ListaBatch(int? id)
        {
            Conexion conn = new Conexion();
            List<Pnl> listTallas = new List<Pnl>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT distinct ID_BATCH FROM PNL WHERE ID_SUMMARY='" + id + "'";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
                    {
                        IdBatch = Convert.ToInt32(leer["ID_BATCH"])

                    };
                    tallas.Batch = ListaTallasBatch(tallas.IdBatch, id);
                    foreach (var item in tallas.Batch)
                    {
                        tallas.TipoTurno = item.TipoTurno;
                        tallas.NombreUsr = item.NombreUsr;
                        tallas.IdPnl = item.IdPnl;
                        tallas.Maquina = item.Maquina;
                        ObtenerNombreMaquina(tallas);
                        tallas.NombreUsrModif = item.NombreUsrModif;
                        tallas.Status = item.Status;
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

        public void ObtenerNombreMaquina(Pnl pnl)
        {
            switch (pnl.Maquina)
            {
                case 1:
                    pnl.NombreMaquina = "Machine 1";
                    break;
                case 2:
                    pnl.NombreMaquina = "Machine 2";
                    break;
                case 3:
                    pnl.NombreMaquina = "Machine 3";
                    break;
                case 4:
                    pnl.NombreMaquina = "Machine 4";
                    break;
                case 5:
                    pnl.NombreMaquina = "Machine 5";
                    break;
                case 6:
                    pnl.NombreMaquina = "Machine 6";
                    break;
                case 7:
                    pnl.NombreMaquina = "Machine 7";
                    break;
                case 8:
                    pnl.NombreMaquina = "Machine 8";
                    break;
                case 9:
                    pnl.NombreMaquina = "Machine 9";
                    break;
                case 10:
                    pnl.NombreMaquina = "Machine 10";
                    break;
                case 11:
                    pnl.NombreMaquina = "Machine 11";
                    break;
                case 12:
                    pnl.NombreMaquina = "Machine 12";
                    break;
                default:
                    pnl.NombreMaquina = "-";
                    break;
            }
        }
        //Permite obtener el idUsuario del batch registrado
        public int ObtenerIdUsuarioPorBatchPNLEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idUsuario = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_USUARIO FROM PNL WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
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
        //Muestra la lista de tallas de UN Batch por estilo y id Batch seleccionado
        public IEnumerable<Pnl> ListaCantidadesTallaPorIdBatchEstilo(int? idEstilo, int idBatch)
        {
            Conexion conn = new Conexion();
            List<Pnl> listTallas = new List<Pnl>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;               
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PNL, ID_TALLA, S.TALLA, PRINTED, MISPRINT, DEFECT, REPAIR FROM PNL " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=PNL.ID_TALLA " +
                    "WHERE ID_SUMMARY='" + idEstilo + "' AND ID_BATCH='" + idBatch + " 'ORDER by cast(S.ORDEN AS int) ASC ";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    Pnl tallas = new Pnl()
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
        public List<Pnl> ListaTallasBatch(int? batch, int? id)
        {
            Conexion conex = new Conexion();
            List<Pnl> listTallas = new List<Pnl>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;  
                c.Connection = conex.AbrirConexion();
                c.CommandText = "SELECT P.ID_PNL, P.ID_SUMMARY, P.ID_BATCH, CONCAT(U.Nombres,' ',U.Apellidos)AS NOMBRE, P.TURNO,P.MAQUINA, P.ID_USUARIO_MODIF, P.STATUS_PALLET, " +
                    " P.ID_TALLA, S.TALLA, P.PRINTED, P.MISPRINT, P.DEFECT,P.REPAIR, sum(PRINTED+MISPRINT+DEFECT+REPAIR)AS TOTAL,P.COMENTARIOS FROM PNL P " +
                    "INNER JOIN CAT_ITEM_SIZE S ON S.ID=P.ID_TALLA " +
                    "INNER JOIN USUARIOS U ON U.Id=P.ID_USUARIO " +
                    "WHERE P.ID_BATCH='" + batch + "' AND P.ID_SUMMARY='" + id + "'  GROUP BY P.ID_PNL,P.ID_SUMMARY, P.ID_BATCH, P.ID_TALLA, S.TALLA, " +
                    "P.PRINTED, P.MISPRINT, P.DEFECT,P.REPAIR, U.Nombres, U.Apellidos, P.TURNO,P.MAQUINA, P.ID_USUARIO_MODIF,P.STATUS_PALLET,P.COMENTARIOS,S.ORDEN ORDER by cast(S.ORDEN AS int) ASC ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    Pnl tallas = new Pnl()
                    {
                        Talla = leerF["TALLA"].ToString(),
                        IdBatch = Convert.ToInt32(leerF["ID_BATCH"]),
                        IdPnl = Convert.ToInt32(leerF["ID_PNL"]),
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
        public void AgregarTallasPnl(Pnl pnl)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarPnl";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@idSummary", pnl.IdSummary);
                com.Parameters.AddWithValue("@idBatch", pnl.IdBatch);
                com.Parameters.AddWithValue("@idTalla", pnl.IdTalla);
                com.Parameters.AddWithValue("@printed", pnl.Printed);
                com.Parameters.AddWithValue("@mp", pnl.MisPrint);
                com.Parameters.AddWithValue("@def", pnl.Defect);
                com.Parameters.AddWithValue("@rep", pnl.Repair);
                com.Parameters.AddWithValue("@maq", pnl.Maquina);
                com.Parameters.AddWithValue("@turno", pnl.TipoTurno);
                com.Parameters.AddWithValue("@idStatus", pnl.EstadoPallet);
                com.Parameters.AddWithValue("@idUsr", pnl.Usuario);
                com.Parameters.AddWithValue("@idUsrAct", pnl.UsuarioModif);
                com.Parameters.AddWithValue("@coment", pnl.Comentarios);

                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }           

        }


        //Permite actualizar la información de un batch
        public void ActualizarTallasPnl(Pnl pnl)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "ActualizarBatchPnl";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@id", pnl.IdPnl);
                com.Parameters.AddWithValue("@idSummary", pnl.IdSummary);
                com.Parameters.AddWithValue("@idBatch", pnl.IdBatch);
                com.Parameters.AddWithValue("@idTalla", pnl.IdTalla);
                com.Parameters.AddWithValue("@printed", pnl.Printed);
                com.Parameters.AddWithValue("@mp", pnl.MisPrint);
                com.Parameters.AddWithValue("@def", pnl.Defect);
                com.Parameters.AddWithValue("@rep", pnl.Repair);
                com.Parameters.AddWithValue("@maq", pnl.Maquina);
                com.Parameters.AddWithValue("@turno", pnl.TipoTurno);
                com.Parameters.AddWithValue("@idStatus", pnl.EstadoPallet);
                com.Parameters.AddWithValue("@idUsr", pnl.Usuario);
                com.Parameters.AddWithValue("@idUsrAct", pnl.UsuarioModif);
                com.Parameters.AddWithValue("@coment", pnl.Comentarios);

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
                coman.CommandText = "SELECT distinct ID_BATCH FROM PNL WHERE ID_SUMMARY='" + id + "' ";
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

        //Permite obtener el idPNL del batch de los registro por idestilo
        public int ObtenerIdPnlPorBatchEstilo(int idBatch, int idSummary, int idTalla)
        {

            int idPnl = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_PNL FROM PNL WHERE ID_BATCH='" + idBatch + "' AND ID_SUMMARY='" + idSummary + "' AND ID_TALLA='" + idTalla + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idPnl += Convert.ToInt32(leerF["ID_PNL"]);

                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            
            return idPnl;
        }



    }



}