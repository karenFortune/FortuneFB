using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTipoOrdenData
    {
        public IEnumerable<CatTipoOrden> ListaTipoOrden()
        {
            List<CatTipoOrden> listTipoOrden = new List<CatTipoOrden>();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT * FROM CAT_TYPE_ORDER ";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatTipoOrden tipoOrden = new CatTipoOrden()
                    {
                        IdTipoOrden = Convert.ToInt32(leer["ID_TYPE_ORDER"]),
                        TipoOrden = leer["TYPE_ORDER"].ToString()

                    };

                    listTipoOrden.Add(tipoOrden);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listTipoOrden;
        }

        //Obtener el nombre del tipo de orden por id 
        public string Obtener_Tipo_Orden_Por_id(int? id)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT TYPE_ORDER FROM CAT_TYPE_ORDER WHERE ID_TYPE_ORDER = '" + id + "' ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return reader["TYPE_ORDER"].ToString();
                }

            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return "";
        }

        //Permite consultar los detalles de un tipo de orden
        public CatTipoOrden ConsultarListaTipoOrden(int? id)
        {
            Conexion conn = new Conexion();
            CatTipoOrden tipoOrden = new CatTipoOrden();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT * FROM CAT_TYPE_ORDER ";
                comando.CommandType = CommandType.Text;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    tipoOrden.IdTipoOrden = Convert.ToInt32(leer["ID_TYPE_ORDER"]);
                    tipoOrden.TipoOrden = leer["TYPE_ORDER"].ToString();

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return tipoOrden;

        }
    }
}