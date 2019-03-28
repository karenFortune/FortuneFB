using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.Roles
{
    public class CatRolesData
    {
        private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leer = null;

        //Muestra la lista de Roles
        public IEnumerable<CatRoles> ListaRoles()
        {
            List<CatRoles> listRoles = new List<CatRoles>();
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Roles";
            comando.CommandType = CommandType.StoredProcedure;
            leer = comando.ExecuteReader();

            while (leer.Read())
            {
                CatRoles roles = new CatRoles();
                roles.Id= Convert.ToInt32(leer["id_Rol"]);
                roles.Rol = leer["rol"].ToString();



                listRoles.Add(roles);
            }
            leer.Close();
            conn.CerrarConexion();

            return listRoles;
        }

        //Permite crear un nuevo rol
        public void AgregarRoles(CatRoles roles)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "AgregarRoles";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Rol", roles.Rol);
 
            comando.ExecuteNonQuery();
            conn.CerrarConexion();

        }

        //Permite consultar los detalles de un rol
        public CatRoles ConsultarListaRoles(int? id)
        {
            CatRoles roles = new CatRoles();

            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Listar_Roles_Por_Id";
            comando.CommandType = CommandType.StoredProcedure;
            comando.Parameters.AddWithValue("@Id", id);

            leer = comando.ExecuteReader();
            while (leer.Read())
            {
                roles.Id = Convert.ToInt32(leer["id_Rol"]);
                roles.Rol= leer["rol"].ToString();

            }
           
            conn.CerrarConexion();
            return roles;

        }

        //Permite actualiza la informacion de un rol
        public void ActualizarRoles(CatRoles roles)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "Actualizar_Roles";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", roles.Id);
            comando.Parameters.AddWithValue("@Rol", roles.Rol);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }

        //Permite eliminar la informacion de un rol
        public void EliminarRol(int? id)
        {
            comando.Connection = conn.AbrirConexion();
            comando.CommandText = "EliminarRoles";
            comando.CommandType = CommandType.StoredProcedure;

            comando.Parameters.AddWithValue("@Id", id);

            comando.ExecuteNonQuery();
            conn.CerrarConexion();
        }


    }
}