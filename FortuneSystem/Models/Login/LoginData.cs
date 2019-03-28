using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Login
{
    public class LoginData
    {
       

        //instancia a la capa de datos de empleado

        enum Roles
        {
            Admin = 1,
            Supervisor = 2,
            Encargado = 3,
            Recibos = 4,
            PrintShop = 5,
            Shipping = 6,
            Stagingi = 7,
            PNL = 8,
            Packing = 9,
            Trims = 10,
            Inventario = 11
        }

        public void IniciarSesion(CatUsuario usuario)
        {
             Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SpLogin";
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@Usuario", usuario.NoEmpleado);
                comando.Parameters.AddWithValue("@Password", usuario.Contrasena);

                leer = comando.ExecuteReader();
                comando.ExecuteNonQuery();
            }
            finally
            {
                conn.CerrarConexion();
                conn.Dispose();
            }  

        }

        public bool IsValid (string _username, string _password, CatUsuario usuario)
        {
            Conexion conn = new Conexion();
            SqlCommand comando = new SqlCommand();
            SqlDataReader leer;
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "SpLogin";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Usuario", _username);
            comando.Parameters.AddWithValue("@Password",_password);
           

            leer = comando.ExecuteReader();
          
            
            while (leer.Read())
            {

                usuario.Cargo = Convert.ToInt32(leer["Cargo"]);
                PrivilegioUsuario(usuario);
            }
            if (leer.HasRows)
            {
                leer.Dispose();
                comando.Dispose();
                return true;

            }
            else
            {
                leer.Dispose();
                comando.Dispose();
                return false;
            }
               
        }

        public int PrivilegioUsuario(CatUsuario usuario)
        {

            int cargo = 0;
            if (usuario.Cargo == (int)Roles.Admin)
            {
                cargo = 1;
                Debug.WriteLine("Administrador");
            }
            else if(usuario.Cargo == (int)Roles.PrintShop)
            {
                cargo = 5;
                Debug.WriteLine("PrintShop");
            }

            return cargo;
            
        }




    }
}