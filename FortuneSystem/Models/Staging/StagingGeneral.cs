using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace FortuneSystem.Models.Staging
{
    public class StagingGeneral
    {
        public string obtener_color_item(int id)
        {
            string temp = "";
            Conexion conx = new Conexion();
            try
            {
                SqlCommand comx = new SqlCommand();
                SqlDataReader leerx = null;
                comx.Connection = conx.AbrirConexion();
                comx.CommandText = "SELECT cc.CODIGO_COLOR,cc.DESCRIPCION from CAT_COLORES cc,inventario i where i.id_inventario='" + id + "' and cc.ID_COLOR=i.id_color  ";
                leerx = comx.ExecuteReader();
                while (leerx.Read())
                {
                    temp = Convert.ToString(leerx["CODIGO_COLOR"]) +" - "+ Convert.ToString(leerx["DESCRIPCION"]);
                }
                leerx.Close();
            }
            finally
            {
                conx.CerrarConexion(); conx.Dispose();
            }
            return temp;
        }

        public string obtener_pais_item(int id)
        {
            string temp = "";
            Conexion conx = new Conexion();
            try
            {
                SqlCommand comx = new SqlCommand();
                SqlDataReader leerx = null;
                comx.Connection = conx.AbrirConexion();
                comx.CommandText = "SELECT p.pais from paises p,inventario i where i.id_inventario='" + id + "' and p.id_pais=i.id_pais  ";
                leerx = comx.ExecuteReader();
                while (leerx.Read())
                {
                    temp = Convert.ToString(leerx["pais"]) ;
                }
                leerx.Close();
            }
            finally
            {
                conx.CerrarConexion(); conx.Dispose();
            }
            return temp;
        }


        public int obtener_id_empleado(string cadena)
        {
            int temp = 0;
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_empleado from empleados where nombre_empleado='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToInt32(leer["id_empleado"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }
        //obtener_lista_staging_grafica
        public String obtener_lista_staging_grafica()
        {
            Conexion con = new Conexion();
            //List<String> Lista = new List<String>();
            /*Lista.Add(Convert.ToString(grafica_staging_hoy()));
            Lista.Add(Convert.ToString(grafica_staging_ayer()));
            Lista.Add(Convert.ToString(grafica_staging_semana()));
            Lista.Add(Convert.ToString(grafica_staging_mes()));
            Lista.Add(Convert.ToString(grafica_staging_year()));*/
            string Lista = Convert.ToString(grafica_staging_hoy()) + "*"+ Convert.ToString(grafica_staging_ayer())+"*"+ Convert.ToString(grafica_staging_semana())+"*"+ Convert.ToString(grafica_staging_mes())+"*"+ Convert.ToString(grafica_staging_year());
            return Lista;
        }

        public int grafica_staging_hoy(){
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista=0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from staging where fecha between '"+fecha[0]+" 00:00:00' and '"+fecha[0]+" 23:59:59'  ";               
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista+=Convert.ToInt32(leer["total"]);
                }
                leer.Close();
            }finally{ con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
       
        public int grafica_staging_ayer(){
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from staging where fecha>= dateadd(day,datediff(day,1,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public int grafica_staging_semana()
        {
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from staging where fecha>= dateadd(day,datediff(day,7,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    Lista += Convert.ToInt32(leer["total"]);
                }
                leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public int grafica_staging_mes(){
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                //com.CommandText = "SELECT total from staging where fecha>= dateadd(day,datediff(day,30,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
                com.CommandText = "SELECT total from staging where YEAR(fecha)=YEAR('" + DateTime.Now + "') and MONTH(fecha)=MONTH('" + DateTime.Now + "')";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public int grafica_staging_year(){
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from staging where YEAR(fecha)=YEAR('"+ DateTime.Now + "')";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }



















    }
}