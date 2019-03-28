using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatStatusData
    {

        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;

        //Muestra la lista de estados
        public IEnumerable<CatStatus> ListarEstados()
        {
            List<CatStatus> listEstados = new List<CatStatus>();
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_Estados";
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatStatus estados = new CatStatus()
                    {
                        IdStatus = Convert.ToInt32(leer["ID_STATUS"]),
                        Estado = leer["ESTADO"].ToString()
                    };
                    listEstados.Add(estados);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return listEstados;
        }

    }
}