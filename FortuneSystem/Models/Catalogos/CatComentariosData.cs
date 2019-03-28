using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatComentariosData
    {
        CatUsuarioData objUsr = new CatUsuarioData();

        public IEnumerable<CatComentarios> ListaComentarios(int? IdSummary)
        {
            List<CatComentarios> listComentario = new List<CatComentarios>();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT * FROM CAT_COMENTARIOS WHERE ID_SUMMARY='"+ IdSummary + "' ";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatComentarios coment = new CatComentarios()
                    {
                        IdComentario = Convert.ToInt32(leer["ID_COMENTARIOS"]),
                        Comentario = leer["COMENTARIO"].ToString(),
                        FechaComentario = Convert.ToDateTime(leer["FECHA_COMENT"]),
                        IdUsuario = Convert.ToInt32(leer["ID_USUARIO"])
                    };

                    coment.FechaComents = String.Format("{0:dd/MMM/yyyy}", coment.FechaComentario);

                    listComentario.Add(coment);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listComentario;
        }

        public IEnumerable<CatComentarios> ListadoAllComentarios()
        {
            List<CatComentarios> listComentario = new List<CatComentarios>();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT * FROM CAT_COMENTARIOS ";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatComentarios coment = new CatComentarios()
                    {
                        IdComentario = Convert.ToInt32(leer["ID_COMENTARIOS"]),
                        Comentario = leer["COMENTARIO"].ToString(),
                        FechaComentario = Convert.ToDateTime(leer["FECHA_COMENT"]),
                        IdUsuario = Convert.ToInt32(leer["ID_USUARIO"]),
                        IdSummary = Convert.ToInt32(leer["ID_SUMMARY"])
                    };

                    if (!Convert.IsDBNull(leer["ID_USUARIO"]))
                    {
                        coment.IdUsuario = Convert.ToInt32(leer["ID_USUARIO"]);
                    }
                    if (coment.IdUsuario != 0)
                    {
                        coment.NombreUsuario = objUsr.Obtener_Nombre_Usuario_PorID(coment.IdUsuario);
                    }
                    else
                    {
                        coment.NombreUsuario = "-";
                    }
                    coment.FechaComents = String.Format("{0:dd/MMM/yyyy}", coment.FechaComentario);

                    listComentario.Add(coment);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listComentario;
        }

        public CatComentarios Obtener_Utlimo_Comentario_Por_IdSummnary(int? IdSummary)
        {
            Conexion conex = new Conexion();
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            CatComentarios ComentariosG = new CatComentarios();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT TOP 1 ID_COMENTARIOS, COMENTARIO, FECHA_COMENT, ID_USUARIO FROM CAT_COMENTARIOS  WHERE ID_SUMMARY='" + IdSummary + "' ORDER BY ID_COMENTARIOS DESC ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    ComentariosG.IdComentario = Convert.ToInt32(reader["ID_COMENTARIOS"]);
                    ComentariosG.Comentario = reader["COMENTARIO"].ToString();
                    ComentariosG.FechaComentario = Convert.ToDateTime(reader["FECHA_COMENT"]);
                    ComentariosG.IdUsuario = Convert.ToInt32(reader["ID_USUARIO"]);                  
 
                }
                conex.CerrarConexion();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return ComentariosG;
        }


        //Permite registrar un comentario nuevo
        public void AgregarComentario(CatComentarios comentarios)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "AgregarComentario";
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@fecha", comentarios.FechaComentario);
                comando.Parameters.AddWithValue("@comentario", comentarios.Comentario);
                comando.Parameters.AddWithValue("@idUser", comentarios.IdUsuario);
                comando.Parameters.AddWithValue("@idSummary", comentarios.IdSummary);

                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

        }
    }
}