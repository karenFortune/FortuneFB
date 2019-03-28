using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTipoCamisetaData
    {
       
        public IEnumerable<CatTipoCamiseta> ListaTipoCamiseta()
        {
            List<CatTipoCamiseta> listTipoCamiseta = new List<CatTipoCamiseta>();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Tipo_Camiseta";
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();

                while (leer.Read())
                {
                    CatTipoCamiseta tipoC = new CatTipoCamiseta()
                    {
                        IdTipo = Convert.ToInt32(leer["ID_TYPE_CODE"]),
                        TipoProducto = leer["PRODUCT_TYPE_CODE"].ToString(),
                        DescripcionTipo = leer["DESCRIPTION"].ToString(),
                        TipoGrupo = leer["GROUP_TYPE"].ToString()
                    };

                    listTipoCamiseta.Add(tipoC);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTipoCamiseta;
        }

        //Permite crear un nuevo tipo de camiseta
        public void AgregarCamiseta(CatTipoCamiseta camiseta)
        {
            Conexion conne = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                com.Connection = conne.AbrirConexion();
                com.CommandText = "AgregarTipoCamiseta";
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Codigo", camiseta.TipoProducto);
                com.Parameters.AddWithValue("@Descripcion", camiseta.DescripcionTipo);
                com.Parameters.AddWithValue("@Grupo", camiseta.TipoGrupo);
                com.ExecuteNonQuery();
            }
            finally
            {
                conne.CerrarConexion();
                conne.Dispose();
            }

        }

        //Permite consultar los detalles de un tipo de camiseta
        public CatTipoCamiseta ConsultarListaCamisetas(int? id)
        {
            CatTipoCamiseta camiseta = new CatTipoCamiseta();
            Conexion connex = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerC = null;
                comando.Connection = connex.AbrirConexion();
                comando.CommandText = "Listar_Camiseta_Por_Id";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leerC = comando.ExecuteReader();
                while (leerC.Read())
                {
                    camiseta.IdTipo = Convert.ToInt32(leerC["ID_TYPE_CODE"]);
                    camiseta.TipoProducto = leerC["PRODUCT_TYPE_CODE"].ToString();
                    camiseta.DescripcionTipo = leerC["DESCRIPTION"].ToString();
                    camiseta.TipoGrupo = leerC["GROUP_TYPE"].ToString();
                }
                leerC.Close();
            }
            finally
            {
                connex.CerrarConexion();
                connex.Dispose();
            }
                return camiseta;

        }

        //Permite actualiza la informacion de un tipo de camiseta
        public void ActualizarCamisetas(CatTipoCamiseta camiseta)
        {
            Conexion connexi = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = connexi.AbrirConexion();
                comando.CommandText = "Actualizar_Camiseta";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", camiseta.IdTipo);
                comando.Parameters.AddWithValue("@Codigo", camiseta.TipoProducto);
                comando.Parameters.AddWithValue("@Descripcion", camiseta.DescripcionTipo);
                comando.Parameters.AddWithValue("@Grupo", camiseta.TipoGrupo);
                comando.ExecuteNonQuery();
            }
            finally
            {
                connexi.CerrarConexion();
                connexi.Dispose();
            }
        }

        //Permite eliminar la informacion de un tipo de camiseta
        public void EliminarCamisetas(int? id)
        {
            Conexion con = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = con.AbrirConexion();
                comando.CommandText = "EliminarCamiseta";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                comando.ExecuteNonQuery();
            }
            finally
            {
                con.CerrarConexion();
                con.Dispose();
            }
        }

        public int ObtenerIdTipoCamiseta(string tipoCamiseta)
        {
            int idTipoCamiseta = 0;
            Conexion conex = new Conexion();
            try
            {
                SqlCommand coman = new SqlCommand();
                SqlDataReader leerF = null;
                coman.Connection = conex.AbrirConexion();
                coman.CommandText = "SELECT ID_TYPE_CODE FROM CAT_PRODUCT_TYPE_CODES " +
                                     "WHERE PRODUCT_TYPE_CODE='" + tipoCamiseta + "' ";
                leerF = coman.ExecuteReader();
                while (leerF.Read()){
                    idTipoCamiseta += Convert.ToInt32(leerF["ID_TYPE_CODE"]);
                }
                leerF.Close();
            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return idTipoCamiseta;
        }
    }
}