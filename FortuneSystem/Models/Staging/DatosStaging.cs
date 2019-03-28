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

namespace FortuneSystem.Models.Staging
{
    public class DatosStaging
    {
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        StagingGeneral stag = new StagingGeneral();
        /*******************************************************************************************************************/

       

        public List<pedido_staging> lista_papeleta(int estilo,int pedido,int turno){
            List<pedido_staging> lista = new List<pedido_staging>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                /* if (tipo == "r") {
                     com.CommandText = "SELECT r.id_inventario,r.total, i.id_pedido,i.id_estilo,i.descripcion,rg.fecha from recibos_items r,inventario i,recibos rg where " +
                    "  r.id_inventario=i.id_inventario and i.id_inventario='" + inventario + "' and r.id_recibo=rg.id_recibo order by rg.fecha desc";
                 }
                 if (tipo == "s") {*/
                //com.CommandText = "SELECT i.id_inventario, i.id_pedido,i.id_estilo,i.descripcion from inventario i where " +
                //"   i.id_inventario='" + inventario + "' ";
                //}
                com.CommandText = "select ID_PEDIDOS,ITEM_ID from PO_SUMMARY WHERE ID_PEDIDOS='"+pedido+"' AND ITEM_ID='"+estilo+"'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    pedido_staging ps = new pedido_staging();
                    ps.id_pedido = Convert.ToInt32(leer["ID_PEDIDOS"]);
                    ps.id_estilo = Convert.ToInt32(leer["ITEM_ID"]);
                    ps.descripcion = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.po = consultas.obtener_po_id((ps.id_pedido).ToString());
                    //ps.total = Convert.ToInt32(leer["total"]);
                    ps.estilo = consultas.obtener_estilo(ps.id_estilo);
                    //ps.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    //ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    if (turno == 1) { ps.turno = "PRIMER TURNO"; } else { ps.turno = "SEGUNDO TURNO"; }                    
                    lista.Add(ps);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public int buscar_id_summary(string po,string estilo){
            int temp = 0;
            Conexion conx = new Conexion();
            try{
                SqlCommand comx = new SqlCommand();
                SqlDataReader leerx = null;
                comx.Connection = conx.AbrirConexion();
                comx.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PEDIDOS='" + po + "' AND ITEM_ID='"+estilo+"'  ";
                leerx = comx.ExecuteReader();
                while (leerx.Read()){
                    temp = Convert.ToInt32(leerx["ID_PO_SUMMARY"]);
                }
                leerx.Close();
            }finally{ conx.CerrarConexion(); conx.Dispose(); }
            return temp;
        }

        public void guardar_stag_bd(string pedido,string estilo,int total,int usuario, int summary,string comentarios){
            Conexion con_r = new Conexion();
            try{
                SqlCommand com_r = new SqlCommand();
                com_r.Connection = con_r.AbrirConexion();
                com_r.CommandText = "INSERT INTO staging(id_pedido,id_estilo,total,id_usuario_captura,id_summary,comentarios,fecha) values('" + 
                    pedido + "','" + estilo + "','" + total + "','" + usuario + "','" + summary + "','" + comentarios + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                com_r.ExecuteNonQuery();
            }finally{
                con_r.CerrarConexion(); con_r.Dispose();
            }
        }

        public int obtener_ultimo_stag(){
            int temporal = 0;
            Conexion con_u_r_i = new Conexion();
            try{
                SqlCommand com_u_r_i = new SqlCommand();
                SqlDataReader leer_u_r_i = null;
                com_u_r_i.Connection = con_u_r_i.AbrirConexion();
                com_u_r_i.CommandText = "SELECT TOP 1 id_staging FROM staging order by id_staging desc ";
                leer_u_r_i = com_u_r_i.ExecuteReader();
                while (leer_u_r_i.Read()){
                    temporal = Convert.ToInt32(leer_u_r_i["id_staging"]);
                }
                leer_u_r_i.Close();
            }finally{
                con_u_r_i.CerrarConexion();
                con_u_r_i.Dispose();
            }
            return temporal;
        }

        public void guardar_stag_conteos(int staging,int talla,int pais,int color, int porcentaje, int total,string usuario)
        {
            Conexion con_r = new Conexion();
            try
            {
                SqlCommand com_r = new SqlCommand();
                com_r.Connection = con_r.AbrirConexion();
                com_r.CommandText = "INSERT INTO staging_count(id_staging,id_talla,id_pais,id_color,id_porcentaje,total,id_empleado) values('" +
                    staging + "','" + talla + "','" + pais + "','" + color + "','" +porcentaje + "','" + total + "','"+usuario+"') ";
                com_r.ExecuteNonQuery();
            }
            finally
            {
                con_r.CerrarConexion(); con_r.Dispose();
            }
        }
        //lista_papeleta_staging
        public List<stag_conteo> lista_papeleta_staging(int stag,int turno)
        {
            List<stag_conteo> lista = new List<stag_conteo>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT s.id_pedido,s.id_estilo,s.comentarios,s.fecha,s.id_usuario_captura,sc.id_talla,sc.id_pais,sc.id_color,sc.id_porcentaje,sc.total,sc.id_empleado  " +
                    " from staging_count sc,staging s where sc.id_staging=s.id_staging and sc.id_staging='"+stag+"' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    stag_conteo ps = new stag_conteo();
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.po = consultas.obtener_po_id(leer["id_pedido"].ToString());
                    ps.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    ps.color= consultas.obtener_color_id(Convert.ToString(leer["id_color"]))+"-"+consultas.obtener_descripcion_color_id(Convert.ToString(leer["id_color"]));
                    ps.talla= consultas.obtener_size_id(Convert.ToString(leer["id_talla"]));
                    ps.porcentaje = consultas.obtener_fabric_percent_id(Convert.ToString(leer["id_porcentaje"]));
                    ps.pais= consultas.obtener_pais_id(Convert.ToString(leer["id_pais"]));
                    ps.cantidad = Convert.ToString(leer["total"]);
                    ps.usuario_conteo = Convert.ToString(leer["id_empleado"]);
                    ps.observaciones = leer["comentarios"].ToString();
                    ps.usuario = (consultas.buscar_nombre_usuario(leer["id_usuario_captura"].ToString())).ToUpper();
                    if (turno == 1) { ps.turno = "PRIMER TURNO"; } else { ps.turno = "SEGUNDO TURNO"; }
                    lista.Add(ps);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public string obtener_nombre_empleado(int cadena)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT nombre_empleado from empleados where id_empleado ='" + cadena + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["nombre_empleado"]).ToUpper();
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }

        public List<stag_conteo> obtener_staging_inicio(){
            /*string query = "";
            if (busqueda == "0"){
                query = "SELECT top 20 s.id_staging,s.id_pedido,s.id_estilo,s.comentarios,s.fecha,s.id_usuario_captura,s.total  " +
                    " from staging s order by s.id_staging desc";
            }else {
                query = "SELECT top 20 s.id_staging,s.id_pedido,s.id_estilo,s.comentarios,s.fecha,s.id_usuario_captura,s.total  " +
                    " from staging s " +
                    "" +
                    "order by s.id_staging desc";
            }*/
            List<stag_conteo> lista = new List<stag_conteo>();
            Conexion con = new Conexion();
            int i = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT  s.id_staging,s.id_pedido,s.id_estilo,s.comentarios,s.fecha,s.id_usuario_captura,s.total  " +
                  " from staging s order by s.id_staging desc";
                //com.CommandText = query;
                leer = com.ExecuteReader();
                while (leer.Read()){
                    stag_conteo ps = new stag_conteo();
                    ps.id_staging = Convert.ToInt32(leer["id_staging"]);
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.po = consultas.obtener_po_id(leer["id_pedido"].ToString());
                    ps.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"]));
                    ps.fecha = ((Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy")).ToUpper();
                    ps.cantidad = Convert.ToString(leer["total"]);
                    ps.usuario = (consultas.buscar_nombre_usuario(leer["id_usuario_captura"].ToString())).ToUpper();
                    lista.Add(ps);
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        

        public List<pedido_staging> buscar_pedidos_recibo(int sucursal, string busqueda)
        {
            //string query = "";

            List<pedido_staging> lista = new List<pedido_staging>();
            if (busqueda == "0")
            {
                List<pedido_staging> lista_inicial= obtener_stag_inicial ();
                foreach (pedido_staging ps in lista_inicial) {
                    lista.Add(ps);
                }

                //query = "SELECT top 25 'p' as 'tipo',PS.ID_PEDIDOS,PS.ITEM_ID,PS.ID_COLOR,PS.ID_PO_SUMMARY FROM PO_SUMMARY PS,PEDIDO P WHERE P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 ";
            }else{
                List<pedido_staging> lista_stag_pedido = obtener_stag_pedido(busqueda);
                foreach (pedido_staging ps in lista_stag_pedido){
                    lista.Add(ps);
                }
                List<pedido_staging> lista_stag_estilos= obtener_stag_estilos(busqueda);
                foreach (pedido_staging ps in lista_stag_estilos){
                    lista.Add(ps);
                }
               /* query = "(SELECT top 25 'p' as 'tipo',PS.ID_PEDIDOS,PS.ITEM_ID,PS.ID_COLOR,PS.ID_PO_SUMMARY FROM PO_SUMMARY PS,PEDIDO P,ITEM_DESCRIPTION ITD WHERE P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 " +
                    " AND ITD.ITEM_ID=PS.ITEM_ID AND ITD.DESCRIPTION LIKE '%" + busqueda + "%' ) UNION" +
                    " (SELECT top 25 'p' as 'tipo',PS.ID_PEDIDOS,PS.ITEM_ID,PS.ID_COLOR,PS.ID_PO_SUMMARY FROM PO_SUMMARY PS,PEDIDO P,ITEM_DESCRIPTION ITD WHERE P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7  " +
                    " AND P.PO LIKE '%" + busqueda + "%')";*/
            }



           /* Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                /*com.CommandText = "(SELECT 'r' as 'tipo',r.id_inventario,r.total, i.id_pedido,i.id_estilo,i.descripcion,rg.fecha from recibos_items r,inventario i,recibos rg where " +
                    "  r.id_inventario=i.id_inventario and i.id_sucursal='1' and r.id_recibo=rg.id_recibo and i.id_estilo!=0 and i.id_pedido!=0 and i.id_categoria_inventario=1 )" +
                    " UNION (SELECT 's' as 'tipo',si.id_inventario_nuevo as 'id_inventario',si.cantidad as 'total', i.id_pedido,i.id_estilo,i.descripcion,s.fecha_recibo as 'fecha' from salidas_items si,inventario i,salidas s where " +
                "  si.id_inventario=i.id_inventario and s.id_destino='1' and si.id_salida=s.id_salida and  " +
                "s.estado_entrega=1 and i.id_categoria_inventario=1 ) order by fecha desc";*/
                //com.CommandText = "SELECT top 25 'p' as 'tipo',PS.ID_PEDIDOS,PS.ITEM_ID,PS.ID_COLOR,PS.ID_PO_SUMMARY FROM PO_SUMMARY PS,PEDIDO P WHERE P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 ";
               /* com.CommandText = query;
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    pedido_staging ps = new pedido_staging();
                    ps.id_summary = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    ps.id_pedido = Convert.ToInt32(leer["ID_PEDIDOS"]);
                    ps.id_estilo = Convert.ToInt32(leer["ITEM_ID"]);
                    ps.descripcion = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    //ps.id_customer= Convert.ToInt32(leer["id_customer"]);
                    //ps.id_customer_final= Convert.ToInt32(leer["id_customer_final"]);
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.po = consultas.obtener_po_id((ps.id_pedido).ToString());
                    //ps.total= Convert.ToInt32(leer["total"]);
                    ps.estilo = consultas.obtener_estilo(ps.id_estilo);
                    //ps.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ps.id_inventario = 0;
                    //ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    ps.tipo_stag = Convert.ToString(leer["tipo"]);
                    lista.Add(ps);
                }
                leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }*/
            return lista;
        }

        public List<pedido_staging> obtener_stag_inicial()
        {
            List<pedido_staging> lista = new List<pedido_staging>();
           
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT top 25 'p' as 'tipo',PS.ID_PEDIDOS,PS.ITEM_ID,PS.ID_COLOR,PS.ID_PO_SUMMARY FROM PO_SUMMARY PS,PEDIDO P WHERE P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read())
                {
                    pedido_staging ps = new pedido_staging();
                    ps.id_summary = Convert.ToInt32(leerFilas["ID_PO_SUMMARY"]);
                    ps.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDOS"]);
                    ps.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    ps.descripcion = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    /*ps.id_customer= Convert.ToInt32(leer["id_customer"]);
                    ps.id_customer_final= Convert.ToInt32(leer["id_customer_final"]);*/
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.po = consultas.obtener_po_id((ps.id_pedido).ToString());
                    //ps.total= Convert.ToInt32(leer["total"]);
                    ps.estilo = consultas.obtener_estilo(ps.id_estilo);
                    //ps.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ps.id_inventario = 0;
                    //ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    ps.tipo_stag = Convert.ToString(leerFilas["tipo"]);
                    lista.Add(ps);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<pedido_staging> obtener_stag_pedido(string busqueda)
        {
            List<pedido_staging> lista = new List<pedido_staging>();

            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT DISTINCT 'p' as 'tipo',PS.ID_PEDIDOS,PS.ITEM_ID,PS.ID_COLOR,PS.ID_PO_SUMMARY FROM PO_SUMMARY PS,PEDIDO P,ITEM_DESCRIPTION ITD WHERE P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 " +
                    " AND ITD.ITEM_ID=PS.ITEM_ID AND ITD.DESCRIPTION LIKE '%" + busqueda + "%'  ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read())
                {
                    pedido_staging ps = new pedido_staging();
                    ps.id_summary = Convert.ToInt32(leerFilas["ID_PO_SUMMARY"]);
                    ps.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDOS"]);
                    ps.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    ps.descripcion = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    /*ps.id_customer= Convert.ToInt32(leer["id_customer"]);
                    ps.id_customer_final= Convert.ToInt32(leer["id_customer_final"]);*/
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.po = consultas.obtener_po_id((ps.id_pedido).ToString());
                    //ps.total= Convert.ToInt32(leer["total"]);
                    ps.estilo = consultas.obtener_estilo(ps.id_estilo);
                    //ps.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ps.id_inventario = 0;
                    //ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    ps.tipo_stag = Convert.ToString(leerFilas["tipo"]);
                    lista.Add(ps);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<pedido_staging> obtener_stag_estilos(string busqueda)
        {
            List<pedido_staging> lista = new List<pedido_staging>();

            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT DISTINCT 'p' as 'tipo',PS.ID_PEDIDOS,PS.ITEM_ID,PS.ID_COLOR,PS.ID_PO_SUMMARY FROM PO_SUMMARY PS,PEDIDO P,ITEM_DESCRIPTION ITD WHERE P.ID_PEDIDO=PS.ID_PEDIDOS AND P.ID_STATUS!=6 AND P.ID_STATUS!=7  " +
                    " AND P.PO LIKE '%" + busqueda + "%'";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read())
                {
                    pedido_staging ps = new pedido_staging();
                    ps.id_summary = Convert.ToInt32(leerFilas["ID_PO_SUMMARY"]);
                    ps.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDOS"]);
                    ps.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    ps.descripcion = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    /*ps.id_customer= Convert.ToInt32(leer["id_customer"]);
                    ps.id_customer_final= Convert.ToInt32(leer["id_customer_final"]);*/
                    ps.estilo_nombre = consultas.buscar_descripcion_estilo(ps.id_estilo);
                    ps.po = consultas.obtener_po_id((ps.id_pedido).ToString());
                    //ps.total= Convert.ToInt32(leer["total"]);
                    ps.estilo = consultas.obtener_estilo(ps.id_estilo);
                    //ps.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ps.id_inventario = 0;
                    //ps.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MMM dd yyyy");
                    ps.tipo_stag = Convert.ToString(leerFilas["tipo"]);
                    lista.Add(ps);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }



        public List<Talla_staging> obtener_cantidades_tallas_estilo(int posummary)
        {
            List<Talla_staging> lista = new List<Talla_staging>();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT TALLA_ITEM,CANTIDAD,EXTRAS,EJEMPLOS FROM ITEM_SIZE WHERE ID_SUMMARY='" + posummary + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read())
                {
                    Talla_staging ts = new Talla_staging();
                    ts.id_talla = Convert.ToInt32(leerFilas["TALLA_ITEM"]);
                    ts.talla = consultas.obtener_size_id(Convert.ToString(leerFilas["TALLA_ITEM"]));
                    ts.total = Convert.ToInt32(leerFilas["CANTIDAD"]) + Convert.ToInt32(leerFilas["EXTRAS"]) + Convert.ToInt32(leerFilas["EJEMPLOS"]);
                    lista.Add(ts);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }


        public List<Talla_staging> obtener_cantidades_tallas_estilo_staging(List<Talla_staging> lista_tallas, int summary)
        {
            //List<Talla_staging> lista = new List<Talla_staging>();
            foreach (Talla_staging t in lista_tallas) { t.total = 0; }
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT sc.id_staging,sc.id_talla,sc.total from staging_count sc,staging s where" +
                    " sc.id_staging=s.id_staging and s.id_summary='" + summary + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read())
                {
                    foreach (Talla_staging t in lista_tallas)
                    {
                        if (t.id_talla == Convert.ToInt32(leerFilas["ID_TALLA"]))
                        {
                            t.total += Convert.ToInt32(leerFilas["total"]);
                        }
                    }
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista_tallas;
        }















































    }//No
}//No