using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.IO;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Shipping;
//using FortuneSystem.Models.Catalogos;
using System.Text.RegularExpressions;

namespace FortuneSystem.Models.Almacen
{
    public class FuncionesInventarioGeneral
    {
        /*private Conexion conn = new Conexion();
        private SqlCommand comando = new SqlCommand();
        private SqlDataReader leerFilas = null;*/
        public int id_estilo, id_inventario, id_tipo, id_recibo, id_amt, id_unit, id_company, cantidad, id_familia, minimo, id_pedido, id_sucursal, id_usuario;
        public int id_pais, id_fabricante, id_color, id_body_type, id_size, id_gender, id_fabric_type, id_percent, id_quantity, purchased, id_ubicacion, id_customer, id_customer_final, id_recibo_item, id_trim;
        public string descripcion;

        public List<String> Lista_po()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PO from PEDIDO order by ID_PEDIDO ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Lista.Add(leer["PO"].ToString());
                }
                leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public List<String> Lista_po_abiertos(){
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PO from PEDIDO WHERE ID_STATUS!=6 AND ID_STATUS!=7 order by ID_PEDIDO ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Lista.Add(leer["PO"].ToString());
                }leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public List<String> Lista_styles()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ITEM_STYLE from ITEM_DESCRIPTION order by ITEM_ID ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["ITEM_STYLE"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<Inventario> buscar_categorias_inventario()
        {
            List<Inventario> Lista = new List<Inventario>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_categoria,categoria from categorias_inventarios order by id_categoria ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Inventario i = new Inventario();
                    i.id_categoria_inventario = Convert.ToInt32(leer["id_categoria"]);
                    i.categoria_inventario = leer["categoria"].ToString();
                    Lista.Add(i);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<estilos> Lista_estilos_dropdown(string po)
        {
            List<estilos> listestilos = new List<estilos>();
            Conexion con_led = new Conexion();
            try
            {
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT e.ITEM_ID,e.ITEM_STYLE,e.DESCRIPTION from PEDIDO p,PO_SUMMARY ps,ITEM_DESCRIPTION e"+
                    " where p.PO='" + po + "' and p.ID_PEDIDO=ps.ID_PEDIDOS AND p.ID_STATUS!=7 AND ID_STATUS!=6 AND "
                + " ps.ITEM_ID=e.ITEM_ID";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read())
                {
                    estilos i = new estilos();
                    i.id_estilo = Convert.ToInt32(leer_led["ITEM_ID"]);
                    i.description = leer_led["ITEM_STYLE"].ToString() + " " + leer_led["DESCRIPTION"].ToString();
                    listestilos.Add(i);
                }
                leer_led.Close();
            }
            finally
            {
                con_led.CerrarConexion(); con_led.Dispose();
            }
            return listestilos;
        }
        public List<String> Lista_colores()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DESCRIPCION from CAT_COLORES order by DESCRIPCION ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["DESCRIPCION"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_colores_codigos()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT CODIGO_COLOR from CAT_COLORES order by ID_COLOR ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["CODIGO_COLOR"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_fabricantes()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fabricante from fabricantes order by fabricante ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["fabricante"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_paises()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;

                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT pais from paises ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["pais"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_body_types()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;

                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT body_type from body_types order by body_type";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["body_type"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_tallas()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TALLA from CAT_ITEM_SIZE order by TALLA";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["TALLA"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_dcs(){
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DISTINCT dc from shipping_ids order by dc";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista.Add(leer["dc"].ToString());
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return Lista;
        }
        public List<String> Lista_generos()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT GENERO FROM CAT_GENDER ORDER BY GENERO";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["GENERO"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_telas()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT FABRIC FROM CAT_FABRIC_CODES ORDER BY FABRIC";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["FABRIC"].ToString());
                }
                leer.Close();
            }
            finally
            { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public List<String> Lista_porcentajes()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;

                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fabric_percent from fabric_percents order by fabric_percent";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["fabric_percent"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_empleados(int turno,int departamento){
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;

                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT nombre_empleado from empleados where id_departamento='"+departamento+"' and turno='"+turno+ "' order by nombre_empleado";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista.Add(leer["nombre_empleado"].ToString());
                }
                leer.Close();
            }finally{ con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }



        public List<String> Lista_customers()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;

                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT NAME FROM CAT_CUSTOMER ORDER BY NAME";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["NAME"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_units(){
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;                
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT unit from units order by unit";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista.Add(leer["unit"].ToString());
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return Lista;
        }
        public List<String> Lista_location()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;                
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT lugar from lugares order by lugar";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["lugar"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_family()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;

                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT family_trim from family_trims";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["family_trim"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_ubicaciones()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ubicacion from ubicaciones";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["ubicacion"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_clientes_finales()
        {
            Conexion con = new Conexion();
            List<String> Lista = new List<String>();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
               
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT NAME_FINAL from CAT_CUSTOMER_PO";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["NAME_FINAL"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public List<String> Lista_trims()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;

                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT trim from trims";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["trim"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }

        public int buscar_tipo_inventario(string tipo)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_categoria from categorias_inventarios where categoria='" + tipo + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_categoria"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_unit(string unit)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_unit from units where unit='" + unit + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_unit"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_lugares(string company)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_lugar from lugares where lugar='" + company + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_lugar"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_familia_trim(string familia)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_family_trim from family_trims where family_trim='" + familia + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_family_trim"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_pedido(string po)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_PEDIDO from PEDIDO where PO='" + po + "' and ID_STATUS!=7 AND ID_STATUS!=6";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["ID_PEDIDO"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_id_pais(string pais)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_pais from paises where pais='" + pais + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_pais"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_fabricante(string fabricante)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_fabricante from fabricantes where fabricante='" + fabricante + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_fabricante"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_color(string color)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_COLOR from CAT_COLORES where DESCRIPCION='" + color + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["ID_COLOR"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_body_type(string body_type)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_body_type from body_types where body_type='" + body_type + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_body_type"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_talla(string size)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID from CAT_ITEM_SIZE where TALLA='" + size + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["ID"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_genero(string genero)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_GENDER from CAT_GENDER where GENERO like'%" + genero + "%'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["ID_GENDER"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_fabric_type(string fabric_type)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID from CAT_FABRIC_CODES where FABRIC like'%" + fabric_type + "%'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["ID"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_percent(string percent)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_fabric_percent from fabric_percents where fabric_percent='" + percent + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_fabric_percent"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_ubicacion(string ubicacion)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_ubicacion from ubicaciones where ubicacion='" + ubicacion + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_ubicacion"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_customer(string customer)
        {
            int temp = 0;
            Conexion con = new Conexion();
                try
                {
                    SqlCommand com = new SqlCommand();
            SqlDataReader leer = null;
            com.Connection = con.AbrirConexion();
            com.CommandText = "SELECT CUSTOMER from CAT_CUSTOMER where NAME='" + customer + "'";
            leer = com.ExecuteReader();
            while (leer.Read())
            {
                temp = Convert.ToInt32(leer["CUSTOMER"]);
            }
            leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_customer_id(string customer)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT NAME from CAT_CUSTOMER where CUSTOMER='" + customer + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["NAME"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_customer_final(string customer_final)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT CUSTOMER_FINAL from CAT_CUSTOMER_PO where NAME_FINAL='" + customer_final + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["CUSTOMER_FINAL"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_customer_final_id(string customer_final)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT NAME_FINAL from CAT_CUSTOMER_PO where CUSTOMER_FINAL='" + customer_final + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["NAME_FINAL"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion();
                con.Dispose();
   
            }
            return temp;
        }
        public int buscar_trim(string trim)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_trim from trims where trim='" + trim + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_trim"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string buscar_ultimo_recibo(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT r.fecha from recibos r,recibos_items ri,inventario i where i.id_inventario='" + cadena + "' and i.id_inventario=ri.id_inventario and ri.id_recibo=r.id_recibo";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_po_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PO from PEDIDO where ID_PEDIDO='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["PO"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion();
                con.Dispose();
            }
            return temp;
        }
        public string obtener_family_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT family_trim from family_trims where id_family_trim='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["family_trim"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_unit_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT unit from units where id_unit='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["unit"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_trim_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT trim from trims where id_trim='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["trim"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_size_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TALLA from CAT_ITEM_SIZE where ID='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["TALLA"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_fabric_percent_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fabric_percent from fabric_percents where id_fabric_percent='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["fabric_percent"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_fabric_type_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT FABRIC from CAT_FABRIC_CODES where ID='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["FABRIC"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_body_type_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT body_type from body_types where id_body_type='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["body_type"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_ubicacion_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ubicacion from ubicaciones where id_ubicacion='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["ubicacion"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_genero_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT GENERO from CAT_GENDER where ID_GENDER='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["GENERO"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_color_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT CODIGO_COLOR from CAT_COLORES where ID_COLOR='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["CODIGO_COLOR"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int obtener_color_id_item(int pedido,int estilo){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_COLOR from PO_SUMMARY where ITEM_ID='" + estilo + "' and ID_PEDIDOS='"+pedido+"'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["ID_COLOR"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }

        public int obtener_color_id_item_cat(int summary)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_COLOR from PO_SUMMARY where ID_PO_SUMMARY='" + summary + "' ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["ID_COLOR"]);
                }
                leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public string obtener_descripcion_color_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DESCRIPCION from CAT_COLORES where ID_COLOR='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["DESCRIPCION"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_fabricante_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fabricante from fabricantes where id_fabricante='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["fabricante"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_pais_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT pais from paises where id_pais='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["pais"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string obtener_sucursal_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT sucursal from sucursales where id_sucursal='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["sucursal"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }

        public string obtener_sucursal_id_usuario(int cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_sucursal from Usuarios where Id='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["id_sucursal"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }

        public string obtener_turno_usuario(int cadena){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT turno from Usuarios where Id='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["turno"]);
                }
                leer.Close();
            }
            finally{ con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public string obtener_categoria_inventario_id(string cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT categoria from categorias_inventarios where id_categoria='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["categoria"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_id_usuario(string usuario)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT Id from Usuarios where Email='" + usuario + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["Id"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public int buscar_id_sucursal_usuario(int id_usuario)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_sucursal from Usuarios where Id='" + id_usuario + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_sucursal"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public void crear_unidad(string cadena)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO units(unit) VALUES ('" + cadena + "')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public void crear_familia_trim(string cadena)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO family_trims(family_trim) VALUES ('" + cadena + "')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public void crear_trim(string cadena)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trims(trim) VALUES ('" + cadena + "')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public void crear_pais(string cadena)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO paises(pais) VALUES ('" + cadena + "')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public void crear_fabricante(string cadena)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO fabricantes(fabricante) VALUES ('" + cadena + "')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public void crear_body_type(string cadena)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO body_types(body_type) VALUES ('" + cadena + "')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public void crear_ubicacion(string cadena)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO ubicaciones(ubicacion) VALUES ('" + cadena + "')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public salidas buscar_sello_nuevo(int sucursal){
            salidas s = new salidas();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_sello,usado from sellos where id_sucursal='" + sucursal + "' and usado<final and estado=0";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    s.id_sello = (Convert.ToInt32(leer["id_sello"]) + 1);
                    s.sello = (Convert.ToInt32(leer["usado"]) + 1);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return s;
        }
        public string buscar_nombre_usuario(string usuario){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT Nombres,Apellidos from Usuarios where Id='" + usuario + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = leer["Nombres"].ToString() + " " + leer["Apellidos"].ToString();
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public string buscar_nombres_lugares(string id)
        {
            string temp ="";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT lugar from lugares where id_lugar='" + id + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["lugar"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string buscar_descripcion_item(int id_inventario){
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT descripcion from inventario where id_inventario='" + id_inventario + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["descripcion"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        public string buscar_po_item(int id_inventario){
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_pedido from inventario where id_inventario='" + id_inventario + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = obtener_po_id(Convert.ToString(leer["id_pedido"]));
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }

        public int buscar_yarn(string unit)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_yarn from yarns where yarn='" + unit + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_yarn"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }


        public int buscar_color_codigo(string color)
        {
            int temp = 0;
            string x = "";
            //color= Regex.Replace(color, @"\s+", " ");
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_COLOR  from CAT_COLORES where CODIGO_COLOR='" + color + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    /*x = leer["CODIGO_COLOR"].ToString();
                    x = Regex.Replace(x, @"\s+", " ");
                    if (x == color)
                    {
                        temp = Convert.ToInt32(leer["ID_COLOR"]);
                    }*/
                    temp = Convert.ToInt32(leer["ID_COLOR"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }

        public List<String> Lista_yarn()
        {
            List<String> Lista = new List<String>();
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
               
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT yarn from yarns order by yarn ";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista.Add(leer["yarn"].ToString());
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return Lista;
        }
        public int buscar_tipo_inventario_item(string tipo){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT tipo from items_catalogue where item_id='" + tipo + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["tipo"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }

        public string  body_type, fabric_type,unit,family;
        public string buscar_informacion_trim_item(string tipo){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT descripcion,body_type,fabric_type from items_catalogue where item_id='" + tipo + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    descripcion = Convert.ToString(leer["descripcion"]);
                    unit = Convert.ToString(leer["body_type"]);
                    family = Convert.ToString(leer["fabric_type"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public string item, color, fabricante, size,   gender,  fabric_percent, yarn, division;
        public string buscar_informacion_blank_item(string tipo){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT item,color,fabricante,size,descripcion,body_type,gender,fabric_type,fabric_percent,yarn,division from items_catalogue where item_id='" + tipo + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    item = Convert.ToString(leer["item"]);
                    color = Convert.ToString(leer["color"]);
                    fabricante = Convert.ToString(leer["fabricante"]);
                    size = Convert.ToString(leer["size"]);
                    descripcion = Convert.ToString(leer["descripcion"]);
                    body_type = Convert.ToString(leer["body_type"]);
                    gender = Convert.ToString(leer["gender"]);
                    fabric_type = Convert.ToString(leer["fabric_type"]);
                    fabric_percent = Convert.ToString(leer["fabric_percent"]);
                    yarn = Convert.ToString(leer["yarn"]);
                    division = Convert.ToString(leer["division"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }


        public int obtener_estilo_id(string tipo){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ITEM_ID from ITEM_DESCRIPTION where ITEM_STYLE='" + tipo + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["ITEM_ID"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public string obtener_estilo(int tipo){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ITEM_STYLE from ITEM_DESCRIPTION where ITEM_ID='" + tipo + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["ITEM_STYLE"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }

        public int buscar_cliente_final_po(string po){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT CUSTOMER_FINAL from PEDIDO where PO='" + po + "' AND ID_STATUS!=7 AND ID_STATUS!=6";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["CUSTOMER_FINAL"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public string buscar_amt_item(string id){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT item from items_catalogue where item_id='" + id + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["item"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public string buscar_descripcion_item(string id){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT descripcion from items_catalogue where item_id='" + id + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["descripcion"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public string buscar_tipo_trim_item(string id){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fabric_type from items_catalogue where item_id='" + id + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["fabric_type"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public double item_age;
        public string mill_po;
        public string obtener_fecha_recibo(int id){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 1 r.fecha,r.mill_po from recibos r,recibos_items ri where ri.id_recibo=r.id_recibo and ri.id_recibo='" + id + "' order by ri.id_recibo_item desc";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    mill_po = leer["mill_po"].ToString();
                    DateTime d1 = DateTime.Now;
                    DateTime d2 = Convert.ToDateTime(leer["fecha"]);
                    TimeSpan t = d1 - d2;
                    item_age = Math.Round(t.TotalDays, 2, MidpointRounding.AwayFromZero);

                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        //
        public string obtener_color_item(string id){
            string temp = "";
            Conexion conx = new Conexion();
            try{
                SqlCommand comx = new SqlCommand();
                SqlDataReader leerx = null;
                comx.Connection = conx.AbrirConexion();
                comx.CommandText = "SELECT cc.CODIGO_COLOR from CAT_COLORES cc,inventario i where i.id_inventario='" + id + "' and cc.ID_COLOR=i.id_color  ";
                leerx = comx.ExecuteReader();
                while (leerx.Read()){
                    temp = Convert.ToString(leerx["CODIGO_COLOR"]);
                }leerx.Close();
            }finally{conx.CerrarConexion(); conx.Dispose();}
            return temp;
        }

        public int obtener_po_summary(int po, int estilo){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PS.ID_PO_SUMMARY from PO_SUMMARY PS,PEDIDO P where PS.ITEM_ID='" + estilo+ "' and PS.ID_PEDIDOS='"+po+"' " +
                    " AND P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 ";
                leer = com.ExecuteReader();
                while(leer.Read()){
                    temp = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public int obtener_estilo_summary(int summary){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ITEM_ID from PO_SUMMARY where ID_PO_SUMMARY='" + summary + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["ITEM_ID"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public string buscar_descripcion_estilo(int estilo){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DESCRIPTION from ITEM_DESCRIPTION where ITEM_ID='" + estilo + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["DESCRIPTION"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        //buscar_categoria_item
        public int buscar_categoria_item(int item){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_categoria_inventario from inventario where id_inventario='" + item + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_categoria_inventario"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }

        public string buscar_direccion_lugar(int cadena){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT direccion from lugares where id_lugar='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["direccion"]);
                }
                leer.Close();
            }finally{ con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        
        public int obtener_customer_po(int item){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT CUSTOMER from PEDIDO where ID_PEDIDO='" + item + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["CUSTOMER"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public int obtener_customer_final_po(int item){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT CUSTOMER_FINAL from PEDIDO where ID_PEDIDO='" + item + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["CUSTOMER_FINAL"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public string obtener_sigla_genero(string cadena){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT GENERO from CAT_GENDER where ID_GENDER='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["GENERO"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public string obtener_sigla_product_type(string cadena){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DESCRIPTION from CAT_PRODUCT_TYPE_CODES where ID_TYPE_CODE='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["DESCRIPTION"]);
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public string obtener_sigla_fabric(string cadena){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT FABRIC from CAT_FABRIC_CODES where ID='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["FABRIC"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        /******************************************************************/
        public List<string> buscar_tipo_empaque(int summary){
            List<string> temp = new List<string>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TYPE_PACKING from PACKING_TYPE_SIZE where ID_SUMMARY='" + summary + "' and TYPE_PACKING!=3";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    bool isEmpty = !temp.Any();
                    if (isEmpty){
                        temp.Add(Convert.ToString(leer["TYPE_PACKING"]));
                    }else{
                        int same = 0;
                        foreach (string s in temp) {
                            if (s == Convert.ToString(leer["TYPE_PACKING"])) {
                                same++;
                            }
                        }
                        if (same == 0) {
                            temp.Add(Convert.ToString(leer["TYPE_PACKING"]));
                        }
                    }
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public int obtener_ratio_summary_talla(int summary, int talla){
            int temp = 0,tipo,ratio,piezas;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT TYPE_PACKING,RATIO,PIECES FROM PACKING_TYPE_SIZE where ID_SUMMARY='" + summary + "' and ID_TALLA='" + talla + "'  ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    tipo= Convert.ToInt32(leer_u_r["TYPE_PACKING"]);
                    ratio = Convert.ToInt32(leer_u_r["RATIO"]);
                    piezas= Convert.ToInt32(leer_u_r["PIECES"]);
                    if (tipo == 1) { temp = piezas; }
                    else { temp = ratio; }                    
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return temp;
        }
        public int obtener_id_pedido_summary(int summary){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_PEDIDOS from PO_SUMMARY where ID_PO_SUMMARY='" + summary + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["ID_PEDIDOS"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public void insertar_registro(int inventario,int usuario,string registro,string tipo){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO registros(id_usuario,tipo,fecha,id_inventario,registro) VALUES "
                    +"('" + usuario+ "','"+tipo+"','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','"+inventario+"','"+registro+"')";
                com_c.ExecuteNonQuery();
            }finally{con_c.CerrarConexion(); con_c.Dispose();}
        }

        public int buscar_total_inventario(int inventario){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from inventario where id_inventario='" + inventario + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }

        public string obtener_siglas_cliente(string customer_final){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT siglas from siglas_clientes where id_customer_po='" + customer_final + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["siglas"]);
                }leer.Close();
            }finally{con.CerrarConexion();con.Dispose();}
            return temp;
        }
        public List<int> Lista_generos_po(int pedido){
            Conexion con = new Conexion();
            List<int> Lista = new List<int>();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_GENDER FROM PO_SUMMARY WHERE ID_PEDIDOS='"+pedido+"'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista.Add(Convert.ToInt32(leer["ID_GENDER"]));
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return Lista.Distinct().ToList(); 
        }




































    }//no
}//no