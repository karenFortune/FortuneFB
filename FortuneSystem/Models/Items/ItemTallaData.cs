using FortuneSystem.Models.Packing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using FortuneSystem.Models.Staging;
using FortuneSystem.Models.Items;

namespace FortuneSystem.Models.Item
{
    public class ItemTallaData
    {
        PackingData packing = new PackingData();

        public void RegistroTallas(ItemTalla tallas)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "INSERT INTO  ITEM_SIZE (TALLA_ITEM,CANTIDAD,EXTRAS,EJEMPLOS,ID_SUMMARY) " +
                    " VALUES((SELECT ID FROM CAT_ITEM_SIZE WHERE TALLA ='" + tallas.Talla + "'),'" + tallas.Cantidad + "','" + tallas.Extras + "','" + tallas.Ejemplos + "','" + tallas.IdSummary + "')";
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }

        public void RegistroCatTypePack(CatTypePackItem TypePack)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "INSERT INTO  CAT_TYPE_PACK_STYLE (ID_SUMMARY,DESC_PACK) " +
                    " VALUES('" + TypePack.IdSummary + "','" + TypePack.DescripcionPack + "')";
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }

        public void RegistroTallasUPC(UPC tallas)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "INSERT INTO  UPC (IdTalla,IdSummary,UPC) " +
                    " VALUES((SELECT ID FROM CAT_ITEM_SIZE WHERE TALLA ='" + tallas.Talla + "'),'" + tallas.IdSummary + "','" + tallas.UPC1 + "')";
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }

        //Muestra la lista de tallas por estilo
        public IEnumerable<ItemTalla> ListaTallasPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Tallas_Por_Estilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int calidad = 0;
                while (leer.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["TALLA_ITEM"]),
                        Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                        Extras = Convert.ToInt32(leer["EXTRAS"]),
                        Ejemplos = Convert.ToInt32(leer["EJEMPLOS"]),
                        Estilo = leer["ITEM_STYLE"].ToString(),
                        DescripcionEstilo = leer["DESCRIPTION"].ToString()

                    };
                    calidad = Convert.ToInt32(leer["CANTIDAD"]) + Convert.ToInt32(leer["EXTRAS"]);

                    tallas.Cantidad = calidad;
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

        //Muestra la lista de tallas por estilo
        public IEnumerable<ItemTalla> ListadoTallasPorEstilos(int? id)
        {
            Conexion conn = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Tallas_Por_Estilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int calidad = 0;
                while (leer.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["TALLA_ITEM"]),
                        Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                        Extras = Convert.ToInt32(leer["EXTRAS"]),
                        Ejemplos = Convert.ToInt32(leer["EJEMPLOS"]),
                        Estilo = leer["ITEM_STYLE"].ToString(),
                        DescripcionEstilo = leer["DESCRIPTION"].ToString()

                    };
                    calidad = Convert.ToInt32(leer["CANTIDAD"]);

                    tallas.Cantidad = calidad;
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

        //Muestra la lista de tallas por estilo
        public IEnumerable<ItemTalla> ListaTallasPorEstiloPrint(int? id)
        {
            Conexion conn = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Tallas_Por_Estilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int calidad = 0;
                while (leer.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["TALLA_ITEM"]),
                        Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                        Extras = Convert.ToInt32(leer["EXTRAS"]),
                        Ejemplos = Convert.ToInt32(leer["EJEMPLOS"]),
                        Estilo = leer["ITEM_STYLE"].ToString(),
                        DescripcionEstilo = leer["DESCRIPTION"].ToString()

                    };
                    calidad = Convert.ToInt32(leer["CANTIDAD"]) + Convert.ToInt32(leer["EXTRAS"]) + Convert.ToInt32(leer["EJEMPLOS"]);

                    tallas.Cantidad = calidad;
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

        //Muestra la lista de tallas por estilo
        public IEnumerable<ItemTalla> ListaTallasPorEstilopnl(int? id)
        {
            Conexion conn = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Tallas_Por_Estilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int calidad = 0;
                while (leer.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["TALLA_ITEM"]),
                        Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                        Extras = Convert.ToInt32(leer["EXTRAS"]),
                        Ejemplos = Convert.ToInt32(leer["EJEMPLOS"]),
                        Estilo = leer["ITEM_STYLE"].ToString(),
                        DescripcionEstilo = leer["DESCRIPTION"].ToString()

                    };
                    calidad = Convert.ToInt32(leer["CANTIDAD"]) + Convert.ToInt32(leer["EXTRAS"]) + Convert.ToInt32(leer["EJEMPLOS"]);

                    tallas.Cantidad = calidad;
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

        //Muestra la lista de tallas por estilo para reporte
        public IEnumerable<ItemTalla> ListadoTallasPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Tallas_Por_Estilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int calidad = 0;
                while (leer.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["TALLA_ITEM"]),
                        Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                        Extras = Convert.ToInt32(leer["EXTRAS"]),
                        Ejemplos = Convert.ToInt32(leer["EJEMPLOS"]),
                        Estilo = leer["ITEM_STYLE"].ToString()

                    };
                    calidad = Convert.ToInt32(leer["CANTIDAD"]) + Convert.ToInt32(leer["EXTRAS"]) + Convert.ToInt32(leer["EJEMPLOS"]);

                    tallas.Cantidad = calidad;
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

        //Muestra la lista de tallas por estilo
        public IEnumerable<ItemTalla> ListaTallasPorEstiloRev(int? id)
        {
            Conexion conn = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Lista_Tallas_Por_Estilo";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                int calidad = 0;
                while (leer.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["TALLA_ITEM"]),
                        Cantidad = Convert.ToInt32(leer["CANTIDAD"]),
                        Extras = Convert.ToInt32(leer["EXTRAS"]),
                        Ejemplos = Convert.ToInt32(leer["EJEMPLOS"]),
                        Estilo = leer["ITEM_STYLE"].ToString()

                    };
                    calidad = Convert.ToInt32(leer["CANTIDAD"]);

                    tallas.Cantidad = calidad;
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

        //Muestra la lista de tallas por estilo
        public IEnumerable<ItemTalla> ListaTallasAssortPorEstilo(int? id, string namePack)
        {
            Conexion conn = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            List<PackingTypeSize> listadoEstilos = packing.ObtenerListadoEstilosPackAssort(id, namePack);
            string valores = "";
            for (int v = 0; v < listadoEstilos.Count; v++)
            {
                if (v > 0)
                {
                    valores += "," + listadoEstilos[v].IdSummary;
                }
                else
                {
                    valores += listadoEstilos[v].IdSummary;
                }

            }
            string query = valores;
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select I.TALLA_ITEM,S.TALLA, S.ORDEN, I.CANTIDAD, I.EXTRAS, I.EJEMPLOS from  ITEM_SIZE I  " +
                       "INNER JOIN CAT_ITEM_SIZE S ON S.ID=I.TALLA_ITEM " +
                       "WHERE I.ID_SUMMARY in(" + query + ") ORDER by cast(S.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();
                int calidad = 0;
                int cant = 0;
                int ext = 0;
                int ejm = 0;
                while (leer.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leer["TALLA"].ToString(),
                        IdTalla = Convert.ToInt32(leer["TALLA_ITEM"])

                    };
                    tallas.Cantidad += Convert.ToInt32(leer["CANTIDAD"]);
                    tallas.Extras += Convert.ToInt32(leer["EXTRAS"]);
                    tallas.Ejemplos += Convert.ToInt32(leer["EJEMPLOS"]);


                    ItemTalla result = listTallas.Find(x => x.IdTalla == tallas.IdTalla);
                    if (result == null)
                    {
                        listTallas.Add(tallas);
                    }
                    else
                    {
                        if (result.IdTalla == tallas.IdTalla)
                        {
                            cant = result.Cantidad + tallas.Cantidad;
                            /*ext += result.Extras;*/
                            ejm += result.Ejemplos + tallas.Ejemplos;

                            calidad = cant + ejm;

                            result.Cantidad = calidad;
                        }
                    }
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

        //Muestra la lista de tallas de Staging por estilo
        public IEnumerable<Staging.StagingD> ListaTallasStagingPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<Staging.StagingD> listTallas = new List<Staging.StagingD>();
            int cant = 0;
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select sc.id_talla, cat.talla, sc.total from staging s " +
                       "inner join staging_count sc on s.id_staging=sc.id_staging " +
                       "INNER JOIN CAT_ITEM_SIZE cat ON cat.ID=SC.ID_TALLA " +
                       "WHERE s.id_summary='" + id + "' ORDER by cast(cat.ORDEN AS int) ASC";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    Staging.StagingD tallas = new Staging.StagingD()
                    {
                        talla = leer["TALLA"].ToString(),
                        id_talla = Convert.ToInt32(leer["id_talla"])

                    };
                    tallas.total += Convert.ToInt32(leer["TOTAL"]);

                    Staging.StagingD result = listTallas.Find(x => x.talla == tallas.talla);
                    if (result == null)
                    {
                        listTallas.Add(tallas);
                    }
                    else
                    {
                        if (result.talla == tallas.talla)
                        {
                            cant = result.total + tallas.total;

                            result.total = cant;
                        }
                    }

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


        //Muestra la lista de tallas de Staging por estilo
        public IEnumerable<Staging.StagingDatos> ListaTallasStagingDatosPorEstilo(int? id)
        {
            Conexion conn = new Conexion();
            List<Staging.StagingDatos> listaDatos = new List<Staging.StagingDatos>();
            int idPais = 0;
            int idColor = 0;
            int idPorcentaje = 0;
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select sc.id_pais, sc.id_color, sc.id_porcentaje from staging s " +
                       "inner join staging_count sc on s.id_staging=sc.id_staging " +
                       "WHERE s.id_summary='" + id + "'";
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    idPais = Convert.ToInt32(leer["id_pais"]);
                    idColor = Convert.ToInt32(leer["id_color"]);
                    idPorcentaje = Convert.ToInt32(leer["id_porcentaje"]);

                    Staging.StagingDatos Datos = new Staging.StagingDatos() {
                        Pais = ObtenerPais(idPais),
                        Porcentaje = ObtenerPorcentaje(idPorcentaje),
                        NombreColor = ObtenerColor(idColor)

                };

                  

                    listaDatos.Add(Datos);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listaDatos;
        }


        //Muestra la lista de tallas por summary
        public IEnumerable<ItemTalla> ListaTallasPorSummary(int? id)
        {
            Conexion conexion = new Conexion();
            List<ItemTalla> listTallas = new List<ItemTalla>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leerF = null;
                com.Connection = conexion.AbrirConexion();
                com.CommandText = "Lista_Tallas_Por_Summary";
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", id);
                leerF = com.ExecuteReader();

                while (leerF.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        Talla = leerF["TALLA"].ToString(),
                        Cantidad = Convert.ToInt32(leerF["CANTIDAD"]),
                        Extras = Convert.ToInt32(leerF["EXTRAS"]),
                        Ejemplos = Convert.ToInt32(leerF["EJEMPLOS"]),
                        IdSummary = Convert.ToInt32(leerF["ID_SUMMARY"])

                    };

                    listTallas.Add(tallas);
                }
                leerF.Close();
            }
            finally
            {
                conexion.CerrarConexion();
                conexion.Dispose();
            }

            return listTallas;
        }

        //Muestra la lista de cantidades por tallas de estilo
        public IEnumerable<ItemTalla> ListaCantidadesTallasPorEstilo(int? idEstilo)
        {
            Conexion conex = new Conexion();
            List<ItemTalla> listCantidades = new List<ItemTalla>();
            try
            {
                SqlCommand c = new SqlCommand();
                SqlDataReader leerF = null;
                c.Connection = conex.AbrirConexion();
                c.CommandText = "select its.talla_item, s.orden, s.talla, its.cantidad, its.id_summary from item_size its " +
                                "INNER JOIN CAT_ITEM_SIZE S ON S.ID= its.talla_item " +
                                "where its.id_summary='" + idEstilo + "' GROUP by S.ORDEN, its.talla_item, S.TALLA, its.cantidad, its.id_summary  " +
                                "ORDER BY cast(S.ORDEN AS int) ASC  ";
                leerF = c.ExecuteReader();

                while (leerF.Read())
                {
                    ItemTalla tallas = new ItemTalla()
                    {
                        IdSummary = Convert.ToInt32(leerF["id_summary"]),
                        IdTalla = Convert.ToInt32(leerF["talla_item"]),
                        Talla = leerF["TALLA"].ToString(),
                        Cantidad = Convert.ToInt32(leerF["cantidad"])
                    };

                    listCantidades.Add(tallas);
                }
                leerF.Close();
            }
            catch (Exception)
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

            return listCantidades;
        }

        //Permite actualiza la informacion de un usuario
        public void Actualizar_Tallas_Estilo(ItemTalla tallas)
        {
            Conexion conexion = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conexion.AbrirConexion();
                com.CommandText = "Actualizar_Tallas_Estilo";
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.AddWithValue("@Id", tallas.Id);
                com.Parameters.AddWithValue("@IdTalla", tallas.IdTalla);
                com.Parameters.AddWithValue("@Cantidad", tallas.Cantidad);
                com.Parameters.AddWithValue("@Extras", tallas.Extras);
                com.Parameters.AddWithValue("@Ejemplos", tallas.Ejemplos);
                com.Parameters.AddWithValue("@IdSummary", tallas.IdSummary);

                com.ExecuteNonQuery();
            }
            finally
            {
                conexion.CerrarConexion();
                conexion.Dispose();
            }


        }

        public int ObtenerIdTalla(string talla, int idEstilo)
        {
            int idTalla = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT I.TALLA_ITEM FROM ITEM_SIZE I " +
                        "INNER JOIN CAT_ITEM_SIZE TALLA ON I.TALLA_ITEM=TALLA.ID " +
                        "INNER JOIN PO_SUMMARY ESTILOS ON I.ID_SUMMARY=ESTILOS.ID_PO_SUMMARY " +
                        "where I.ID_SUMMARY='" + idEstilo + "'and TALLA.TALLA='" + talla + "'";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idTalla += Convert.ToInt32(leerF["TALLA_ITEM"]);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
            }
            return idTalla;
        }

        public string ObtenerPais(int idPais)
        {
            string pais ="";
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT pais FROM paises  " +
                        "where id_pais='" + idPais + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    pais += leerF["pais"].ToString();
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
            }
            return pais;
        }

        public string ObtenerPorcentaje(int idPorcentaje)
        {
            string porcentaje = "";
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT fabric_percent FROM fabric_percents  " +
                        "where id_fabric_percent='" + idPorcentaje + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    porcentaje += leerF["fabric_percent"].ToString();
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
            }
            return porcentaje;
        }


        public string ObtenerColor(int idColor)
        {
            string colorDesc = "";
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT DESCRIPCION FROM CAT_COLORES " +
                        "where ID_COLOR='" + idColor + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    colorDesc += leerF["DESCRIPCION"].ToString();
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
            }
            return colorDesc;
        }


        public int ObtenerIdTallaEstilo(string talla, int idEstilo)
        {
            int idTalla = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT I.ID_TALLA FROM ITEM_SIZE I " +
                        "INNER JOIN CAT_ITEM_SIZE TALLA ON I.TALLA_ITEM=TALLA.ID " +
                        "INNER JOIN PO_SUMMARY ESTILOS ON I.ID_SUMMARY=ESTILOS.ID_PO_SUMMARY " +
                        "where I.ID_SUMMARY='" + idEstilo + "'and TALLA.TALLA='" + talla + "'";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idTalla += Convert.ToInt32(leerF["ID_TALLA"]);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idTalla;
        }

        public int ObtenerIdTallas(string talla)
        {
            int idTalla = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT I.ID FROM CAT_ITEM_SIZE I " +          
                        "where I.TALLA='" + talla + "'";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idTalla += Convert.ToInt32(leerF["ID"]);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idTalla;
        }
    }
}