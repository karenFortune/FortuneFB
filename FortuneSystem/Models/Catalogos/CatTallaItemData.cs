using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTallaItemData
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;

        //Muestra la lista de tallas
        public IEnumerable<CatTallaItem> ListaTallas()
        {
            List<CatTallaItem> listTallas = new List<CatTallaItem>();
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Tallas";
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatTallaItem tallas = new CatTallaItem()
                    {
                        Id = Convert.ToInt32(leer["ID"]),
                        Talla = leer["TALLA"].ToString(),
                       
                };

                    if (!Convert.IsDBNull(leer["ORDEN"]))
                    {
                        tallas.Orden = Convert.ToInt32(leer["ORDEN"]);
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
        

        public List<String> Lista_tallas()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                //string listaStag="";                
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TALLA from CAT_ITEM_SIZE ORDER by cast(ORDEN AS int) ASC ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {

                    Lista.Add(leer["TALLA"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion();
                con.Dispose();
            }
            //return listaStag;
            return Lista;


        }



        //Permite crear una nueva talla
        public void AgregarTallas(CatTallaItem tallas)
        {
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "AgregarTalla";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Talla", tallas.Talla);
                comando.Parameters.AddWithValue("@Orden", tallas.Orden);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

        }

        public IEnumerable<CatTallaItem> Lista_tallas_Estilo_Arte(int? idEstilo)
        {
            Conexion con = new Conexion();
            List<CatTallaItem> Lista = new List<CatTallaItem>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                //string listaStag="";               
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT S.TALLA FROM ITEM_SIZE IZ " +
                    "INNER JOIN CAT_ITEM_SIZE S ON IZ.TALLA_ITEM=S.ID " +
                    "WHERE ID_SUMMARY='" + idEstilo + "'  ORDER by cast(ORDEN AS int) ASC ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {

                    CatTallaItem catTalla = new CatTallaItem()
                    {

                        Talla = leer["TALLA"].ToString()
                    };

                    Lista.Add(catTalla);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion();
                con.Dispose();
            }
            return Lista;


        }

        public IEnumerable<UPC> Lista_tallas_upc(int? idSummary)
        {
            Conexion con = new Conexion();
            List<UPC> Lista = new List<UPC>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                //string listaStag="";               
                com.Connection = con.AbrirConexion();
                com.CommandText = "select S.TALLA, UPC from UPC U  " +
                    "INNER JOIN CAT_ITEM_SIZE S ON U.IdTalla=S.ID " +
                    "where IdSummary='" + idSummary + "'  ORDER by cast(ORDEN AS int) ASC ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    UPC catTalla = new UPC()
                    {

                        Talla = leer["TALLA"].ToString(),
                        UPC1 = Convert.ToInt64(leer["UPC"])
                    };
                    Lista.Add(catTalla);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion();
                con.Dispose();
            }
            return Lista;


        }

        //Permite consultar los detalles de una talla
        public CatTallaItem ConsultarListaTallas(int? id)
        {
            CatTallaItem tallas = new CatTallaItem();
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Talla_Por_Id";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    tallas.Id = Convert.ToInt32(leer["ID"]);
                    tallas.Talla = leer["TALLA"].ToString();             

                    if (!Convert.IsDBNull(leer["ORDEN"]))
                    {
                        tallas.Orden = Convert.ToInt32(leer["ORDEN"]);
                    }
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
                return tallas;

        }

        //Permite actualiza la informacion de una talla
        public void ActualizarTallas(CatTallaItem tallas)
        {
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Actualizar_Talla";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", tallas.Id);
                comando.Parameters.AddWithValue("@Talla", tallas.Talla);
                comando.Parameters.AddWithValue("@Orden", tallas.Orden);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }

        //Permite eliminar la informacion de una talla
        public void EliminarTallas(int? id)
        {
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "EliminarTalla";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }
        //Permite obtener el id de un talla
        public int ObtenerIdTalla(string talla)
        {
            int idTalla = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select ID from CAT_ITEM_SIZE WHERE TALLA='" + talla + "'";
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

        //Permite eliminar la informacion de una talla por estilo
        public void EliminarTallasIdEstilo(int idEstilo, int idTalla)
        {

            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "DELETE FROM ITEM_SIZE WHERE ID_SUMMARY='" + idEstilo + "' AND TALLA_ITEM='" + idTalla + "' ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                conex.CerrarConexion();
            }finally {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }


    }
}

