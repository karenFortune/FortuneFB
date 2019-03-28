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
using FortuneSystem.Models.Catalogos;

namespace FortuneSystem.Models.Almacen
{
    public class DatosReportes
    {
        public int id_estilo, id_inventario, id_tipo, id_recibo, id_amt, id_unit, id_company, cantidad, id_familia, minimo, id_pedido, id_sucursal, id_usuario;
        public int id_pais, total, minimo_trim, id_fabric_percent, id_fabricante, id_color, id_yarn, id_body_type, id_size, id_gender, id_fabric_type, id_percent, id_quantity, purchased, id_ubicacion, id_customer, id_customer_final, id_recibo_item, id_trim;
        public string descripcion, po_referencia, yarn, item, division;
        public string mill_po, amt, po, date_comment, comments, notas, cajas, cantidades, amt_item;
        public int quantity = 0, id_caja, id_item;
        public int id_salida;
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();

        public List<Inventario> obtener_item_diario(int id)
        {
            List<Inventario> listInventario = new List<Inventario>();
            Conexion con_oie = new Conexion();
            try
            {
                SqlCommand com_oie = new SqlCommand();
                SqlDataReader leer_oie = null;                
                com_oie.Connection = con_oie.AbrirConexion();
                com_oie.CommandText = "SELECT id_item,id_estilo,id_inventario,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type," +
                    " id_location,total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,date_comment,comment,id_family_trim,id_unit,id_trim " +
                    " from inventario  where  id_categoria_inventario=1  ";
                leer_oie = com_oie.ExecuteReader();
                while (leer_oie.Read())
                {
                    Inventario i = new Inventario();
                    i.amt_item = consultas.buscar_amt_item(leer_oie["id_item"].ToString());
                    i.id_inventario = Convert.ToInt32(leer_oie["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer_oie["id_pedido"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer_oie["id_pedido"]));
                    i.id_categoria_inventario = Convert.ToInt32(leer_oie["id_categoria_inventario"]);
                    i.categoria_inventario = consultas.obtener_categoria_inventario_id(Convert.ToString(leer_oie["id_categoria_inventario"]));
                    i.minimo = Convert.ToInt32(leer_oie["minimo"]);
                    i.notas = Convert.ToString(leer_oie["notas"]);
                    i.pais = consultas.obtener_pais_id(Convert.ToString(leer_oie["id_pais"]));
                    i.fabricante = consultas.obtener_fabricante_id(Convert.ToString(leer_oie["id_fabricante"]));
                    i.color = consultas.obtener_descripcion_color_id(Convert.ToString(leer_oie["id_color"]));
                    i.body_type = consultas.obtener_body_type_id(Convert.ToString(leer_oie["id_body_type"]));
                    i.genero = consultas.obtener_genero_id(Convert.ToString(leer_oie["id_genero"]));
                    i.fabric_type = consultas.obtener_fabric_type_id(Convert.ToString(leer_oie["id_fabric_type"]));
                    i.location = consultas.obtener_ubicacion_id(Convert.ToString(leer_oie["id_location"]));
                    i.total = Convert.ToInt32(leer_oie["total"]);
                    i.size = consultas.obtener_size_id(Convert.ToString(leer_oie["id_size"]));
                    i.customer = consultas.obtener_customer_id(Convert.ToString(leer_oie["id_customer"]));
                    i.final_customer = consultas.obtener_customer_final_id(Convert.ToString(leer_oie["id_customer_final"]));
                    i.fabric_percent = consultas.obtener_fabric_percent_id(Convert.ToString(leer_oie["id_fabric_percent"]));
                    string[] fecha = (Convert.ToString(leer_oie["date_comment"])).Split(' ');
                    i.date_comment = fecha[0];
                    i.comment = Convert.ToString(leer_oie["comment"]);
                    i.family_trim = consultas.obtener_family_id(Convert.ToString(leer_oie["id_family_trim"]));
                    i.unit = consultas.obtener_unit_id(Convert.ToString(leer_oie["id_unit"]));
                    i.trim = consultas.obtener_trim_id(Convert.ToString(leer_oie["id_trim"]));
                    i.id_estilo = Convert.ToInt32(leer_oie["id_estilo"]);
                    i.codigo_color = consultas.obtener_color_id(Convert.ToString(leer_oie["id_color"]));
                    listInventario.Add(i);
                }
                leer_oie.Close();
            }
            finally
            {
                con_oie.CerrarConexion(); con_oie.Dispose();
            }
            return listInventario;
        }

        public string fecha_salidas,ids_salidas;
        public List<Inventario> obtener_inventario_transferencia_mes(int suc)
        {
            List<Inventario> listInventario = new List<Inventario>();
            Conexion con_oie = new Conexion();
            try
            {
                SqlCommand com_oie = new SqlCommand();
                SqlDataReader leer_oie = null;
                com_oie.Connection = con_oie.AbrirConexion();
                com_oie.CommandText = "SELECT i.id_item,i.id_estilo,i.id_inventario,i.id_pedido,i.id_pais,i.id_fabricante,i.id_categoria_inventario,i.id_color,i.id_body_type,i.id_genero,i.id_fabric_type," +
                    " i.id_location,i.total,i.id_size,i.id_customer,i.id_customer_final,i.minimo,i.notas,i.id_fabric_percent,i.date_comment,i.comment,i.id_family_trim,i.id_unit,i.id_trim, " +
                    " s.sucursal,sa.fecha,sa.id_salida  " +
                    " from inventario i,sucursales s,salidas sa,salidas_items si  where i.id_sucursal=s.id_sucursal and i.id_sucursal='" + suc + "' " +
                    " and i.id_inventario=si.id_inventario and si.id_salida=sa.id_salida and YEAR(sa.fecha)='" + DateTime.Now.Year.ToString() + "' and MONTH(sa.fecha)='" + DateTime.Now.Month.ToString() + "'" +
                    "  ";
                leer_oie = com_oie.ExecuteReader();
                while (leer_oie.Read())
                {
                    ids_salidas += "*" + leer_oie["id_salida"].ToString();
                    fecha_salidas += "*" + leer_oie["fecha"].ToString();
                    Inventario i = new Inventario();
                    i.amt_item = consultas.buscar_amt_item(leer_oie["id_item"].ToString());
                    i.id_inventario = Convert.ToInt32(leer_oie["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer_oie["id_pedido"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer_oie["id_pedido"]));
                    i.id_categoria_inventario = Convert.ToInt32(leer_oie["id_categoria_inventario"]);
                    i.categoria_inventario = consultas.obtener_categoria_inventario_id(Convert.ToString(leer_oie["id_categoria_inventario"]));
                    i.minimo = Convert.ToInt32(leer_oie["minimo"]);
                    i.notas = Convert.ToString(leer_oie["notas"]);
                    i.pais = consultas.obtener_pais_id(Convert.ToString(leer_oie["id_pais"]));
                    i.fabricante = consultas.obtener_fabricante_id(Convert.ToString(leer_oie["id_fabricante"]));
                    i.color = consultas.obtener_descripcion_color_id(Convert.ToString(leer_oie["id_color"]));
                    i.body_type = consultas.obtener_body_type_id(Convert.ToString(leer_oie["id_body_type"]));
                    i.genero = consultas.obtener_genero_id(Convert.ToString(leer_oie["id_genero"]));
                    i.fabric_type = consultas.obtener_fabric_type_id(Convert.ToString(leer_oie["id_fabric_type"]));
                    i.location = consultas.obtener_ubicacion_id(Convert.ToString(leer_oie["id_location"]));
                    i.total = Convert.ToInt32(leer_oie["total"]);
                    i.size = consultas.obtener_size_id(Convert.ToString(leer_oie["id_size"]));
                    i.customer = consultas.obtener_customer_id(Convert.ToString(leer_oie["id_customer"]));
                    i.final_customer = consultas.obtener_customer_final_id(Convert.ToString(leer_oie["id_customer_final"]));
                    i.fabric_percent = consultas.obtener_fabric_percent_id(Convert.ToString(leer_oie["id_fabric_percent"]));
                    string[] fecha = (Convert.ToString(leer_oie["date_comment"])).Split(' ');
                    i.date_comment = fecha[0];
                    i.comment = Convert.ToString(leer_oie["comment"]);
                    i.family_trim = consultas.obtener_family_id(Convert.ToString(leer_oie["id_family_trim"]));
                    i.unit = consultas.obtener_unit_id(Convert.ToString(leer_oie["id_unit"]));
                    i.trim = consultas.obtener_trim_id(Convert.ToString(leer_oie["id_trim"]));
                    i.id_estilo = Convert.ToInt32(leer_oie["id_estilo"]);
                    i.codigo_color = consultas.obtener_color_id(Convert.ToString(leer_oie["id_color"]));
                    listInventario.Add(i);
                }
                leer_oie.Close();
            }
            finally
            {
                con_oie.CerrarConexion(); con_oie.Dispose();
            }
            return listInventario;
        }
        public string buscar_datos_transferencia(string salida)
        {
            string temp = "";
            Conexion con = new Conexion();
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT sa.id_envio,l.lugar from salidas sa,lugares l where l.id_lugar=sa.id_destino and sa.id_salida='" + salida + "'";
                leer = com.ExecuteReader();
                while (leer.Read())
                {
                    temp = Convert.ToString(leer["id_envio"]) + " " + Convert.ToString(leer["lugar"]);
                }
                leer.Close();
            }
            finally
            {
                con.CerrarConexion(); con.Dispose();
            }
            return temp;
        }

        public List<salidas> obtener_envios_anual()
        {
            List<salidas> lista = new List<salidas>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT s.id_salida,s.fecha,s.responsable,s.id_envio,s.total,l.lugar from salidas s,lugares l where s.id_destino=l.id_lugar and YEAR(s.fecha)='" + DateTime.Now.Year.ToString() + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    salidas s = new salidas();
                    s.id_salida = Convert.ToInt32(leer["id_salida"]);
                    s.fecha_requested=Convert.ToDateTime(leer["fecha"]).ToString("MMM dd yyyy");
                    s.responsable = leer["responsable"].ToString();
                    s.id_envio= leer["id_envio"].ToString();
                    s.total = Convert.ToInt32(leer["total"]);
                    s.destino = leer["lugar"].ToString();
                    s.lista_salidas_item = obtener_datos_salidas_item_anual(s.id_salida);
                    lista.Add(s);
                }
                leer.Close();
            }finally{
                con.CerrarConexion();
                con.Dispose();
            }
            return lista;
        }


        public List<salidas_item> obtener_datos_salidas_item_anual( int salida)
        {
            List<salidas_item> lista = new List<salidas_item>();
            Conexion con_si = new Conexion();
            try
            {
                SqlCommand com_si = new SqlCommand();
                SqlDataReader leer_si = null;
                com_si.Connection = con_si.AbrirConexion();
                com_si.CommandText = "SELECT id_inventario,cantidad,id_pedido,cantidad from salidas_items where id_salida='"+salida+"'  ";
                leer_si = com_si.ExecuteReader();
                while (leer_si.Read())
                {
                    salidas_item s = new salidas_item();                    
                    s.cantidad = Convert.ToInt32(leer_si["cantidad"]);
                    s.po = consultas.obtener_po_id(leer_si["id_pedido"].ToString());
                    s.color = consultas.obtener_color_item(leer_si["id_inventario"].ToString());
                }
                leer_si.Close();
            }
            finally
            {
                con_si.CerrarConexion();
                con_si.Dispose();
            }
            return lista;
        }

        
















    }
}