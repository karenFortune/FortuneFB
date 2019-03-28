using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatGeneroData
    {  

        //Muestra la lista de genero
        public IEnumerable<CatGenero> ListaGeneros()
        {
            List<CatGenero> listGenero = new List<CatGenero>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leer = null;
                coman.Connection = con.AbrirConexion();
                coman.CommandText = "Listar_Genero";
                coman.CommandType = CommandType.StoredProcedure;
                leer = coman.ExecuteReader();
                while (leer.Read())
                {
                    CatGenero generos = new CatGenero()
                    {
                        IdGender = Convert.ToInt32(leer["ID_GENDER"]),
                        Genero = leer["GENERO"].ToString(),
                        GeneroCode = leer["GENERO_CODE"].ToString()
                    };
                    listGenero.Add(generos);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion();
                con.Dispose();
            }

            return listGenero;
        }

        //Permite crear nuevo genero
        public void AgregarGenero(CatGenero generos)
        {
            Conexion conex = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conex.AbrirConexion();
                com.CommandText = "AgregarGeneros";
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Genero", generos.Genero);
                com.Parameters.AddWithValue("@Codigo", generos.GeneroCode);
                com.ExecuteNonQuery();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }

        }

        //Permite consultar los detalles de un genero
        public CatGenero ConsultarListaGenero(int? id)
        {
            
            CatGenero generos = new CatGenero();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerG = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Genero_Por_Id";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leerG = comando.ExecuteReader();
                while (leerG.Read())
                {
                    generos.IdGender = Convert.ToInt32(leerG["ID_GENDER"]);
                    generos.Genero = leerG["GENERO"].ToString();
                    generos.GeneroCode = leerG["GENERO_CODE"].ToString();
                }
                //leerF.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return generos;

        }

        public IEnumerable<CatGenero> ListarTallasPorGenero(string genero)
        {
            List<CatGenero> listGenero = new List<CatGenero>();
            Conexion conne = new Conexion();
            try
            {
                SqlCommand comand = new SqlCommand();
                SqlDataReader leerTG = null;
                comand.Connection = conne.AbrirConexion();
                comand.CommandText = "Listar_Tallas_Por_Genero";
                comand.CommandType = CommandType.StoredProcedure;
                comand.Parameters.AddWithValue("@Genero", genero);
                leerTG = comand.ExecuteReader();
                while (leerTG.Read())
                {
                    CatGenero generos = new CatGenero()
                    {
                        IdGender = Convert.ToInt32(leerTG["ID_GENDER"]),
                        Genero = leerTG["GENERO"].ToString()
                    };

                    CatTallaItem catTalla = new CatTallaItem()
                    {
                        Id = Convert.ToInt32(leerTG["ID"]),
                        Talla = leerTG["TALLA"].ToString()
                    };
                    generos.CatTallaItem = catTalla;
                    listGenero.Add(generos);
                }
                leerTG.Close();
            }
            finally
            {
                conne.CerrarConexion();
                conne.Dispose();
            }
            return listGenero;
        }

        //Permite actualiza la informacion de un genero
        public void ActualizarGenero(CatGenero generos)
        {
            Conexion connex = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = connex.AbrirConexion();
                comando.CommandText = "Actualizar_Genero";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", generos.IdGender);
                comando.Parameters.AddWithValue("@Genero", generos.Genero);
                comando.Parameters.AddWithValue("@Codigo", generos.GeneroCode);
                comando.ExecuteNonQuery();
            }
            finally
            {
                connex.CerrarConexion();
                connex.Dispose();
            }
        }

        //Permite eliminar la informacion de un genero
        public void EliminarGenero(int? id)
        {
            Conexion connexi = new Conexion();            
            try
            {
                SqlCommand coman = new SqlCommand();
                coman.Connection = connexi.AbrirConexion();
                coman.CommandText = "EliminarGenero";
                coman.CommandType = CommandType.StoredProcedure;
                coman.Parameters.AddWithValue("@Id", id);
                coman.ExecuteNonQuery();
            }
            finally
            {
                connexi.CerrarConexion();
                connexi.Dispose();
            }
        }

        public int ObtenerIdGenero(string genero)
        {
            int idGenero = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "select ID_GENDER from CAT_GENDER " +
                                     "WHERE GENERO_CODE='" + genero + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read())
                {
                    idGenero += Convert.ToInt32(leerF["ID_GENDER"]);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idGenero;
        }


    }
}

