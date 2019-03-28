using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Catalogos
{
    public class CatClienteFinalData
    {


        //Muestra la lista de clientes
        public IEnumerable<CatClienteFinal> ListaClientesFinal()
        {
            Conexion conn = new Conexion();
            List<CatClienteFinal> listClientesFinal = new List<CatClienteFinal>();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_ClientesFinal";
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                while (leer.Read())
                {
                    CatClienteFinal clientesFinal = new CatClienteFinal()
                    {
                        CustomerFinal = Convert.ToInt32(leer["CUSTOMER_FINAL"]),
                        NombreCliente = leer["NAME_FINAL"].ToString()
                    };

                    listClientesFinal.Add(clientesFinal);
                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

            return listClientesFinal;
        }

        //Permite crear un nuevo cliente
        public void AgregarClientesFinal(CatClienteFinal clientesFinal)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "AgregarClienteFinal";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Nombre", clientesFinal.NombreCliente);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }

        }

        //Permite consultar los detalles de un cliente
        public CatClienteFinal ConsultarListaClientesFinal(int? id)
        {
            Conexion conn = new Conexion();
            CatClienteFinal clientesFinal = new CatClienteFinal();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;                
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Listar_ClienteFinal_Por_Id";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", id);
                leer = comando.ExecuteReader();
                while (leer.Read())
                {

                    clientesFinal.CustomerFinal = Convert.ToInt32(leer["CUSTOMER_FINAL"]);
                    clientesFinal.NombreCliente = leer["NAME_FINAL"].ToString();

                }
                leer.Close();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
            return clientesFinal;

        }

        //Permite actualiza la informacion de un cliente
        public void ActualizarClienteFinal(CatClienteFinal clientesFinal)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "Actualizar_ClienteFinal";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Id", clientesFinal.CustomerFinal);
                comando.Parameters.AddWithValue("@Nombre", clientesFinal.NombreCliente);
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }
        }

        //Permite eliminar la informacion de un cliente
        public void EliminarClienteFinal(int? id)
        {
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "EliminarClientesFinal";
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


    }
}


