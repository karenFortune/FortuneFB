using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatTypeBrandData
    {

        //Obtener el tipo de brand de estilo 
        public string Obtener_Tipo_Brand_Por_Estilo(string codigo)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            Conexion conex = new Conexion();
            try
            {
                cmd.Connection = conex.AbrirConexion();
                cmd.CommandText = "SELECT TYPE_BRAND FROM CAT_TYPE_BRAND WHERE CODE_BRAND = '" + codigo + "' ";
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return reader["TYPE_BRAND"].ToString();
                }

            }
            finally
            {
                conex.CerrarConexion();
                conex.Dispose();
            }
            return "";
        }
    }
}