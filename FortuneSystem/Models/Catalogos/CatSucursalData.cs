using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatSucursalData
    {
        //Muestra la lista de sucursales
        public IEnumerable<CatSucursal> ListaSucursales()
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();
            SqlDataReader leer = null;
            List<CatSucursal> listSucursal = new List<CatSucursal>();
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select * from sucursales";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatSucursal sucursal = new CatSucursal()
                    {
                        IdSucursal = Convert.ToInt32(leer["ID_SUCURSAL"]),
                        Sucursal = leer["SUCURSAL"].ToString(),
                        Direccion = leer["DIRECCION"].ToString()
                    };
                    listSucursal.Add(sucursal);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listSucursal;
        }

        //Permite consultar los detalles de una sucursal
        public CatSucursal ConsultarListaSucursal(int? id)
        {


            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();
            SqlDataReader leer = null;
            CatSucursal sucursal = new CatSucursal();
            try
            {
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select * from sucursales where id_sucursal='" + id + "'";
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    sucursal.IdSucursal = Convert.ToInt32(leer["ID_SUCURSAL"]);
                    sucursal.Sucursal = leer["SUCURSAL"].ToString();


                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return sucursal;
        }
    }
}