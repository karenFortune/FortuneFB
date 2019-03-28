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
using FortuneSystem.Models.Shipping;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Almacen;
using System.Text.RegularExpressions;


namespace FortuneSystem.Models.Shipping
{
    public class DatosShipping
    {
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();

        public List<recibo_fantasy> lista_ordenes(string busqueda) {
            List<recibo_fantasy> listar = new List<recibo_fantasy>();
            Conexion conn = new Conexion();
            string query;
            if (busqueda != "") {
                query = "SELECT TOP 50 ID_PEDIDO,PO from PEDIDO where PO like'%" + busqueda + "%' where ID_STATUS!=7 AND ID_STATUS!=6";
            } else {
                query = "SELECT TOP 50 ID_PEDIDO,PO from PEDIDO where ID_STATUS!=7 AND ID_STATUS!=6 ";
            }
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = query;
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    recibo_fantasy l = new recibo_fantasy();
                    l.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDO"]);
                    l.po = leerFilas["PO"].ToString();
                    listar.Add(l);
                }
                leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return listar;
        }
        public List<estilo_shipping> lista_estilos(string id_pedido) {         
            List<estilo_shipping> listar = new List<estilo_shipping>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct ITEM_ID from PO_SUMMARY where ID_PEDIDOS='" + id_pedido + "' "; 
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    estilo_shipping l = new estilo_shipping();
                    l.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    l.id_summary = consultas.obtener_po_summary(Convert.ToInt32(id_pedido), l.id_estilo);                   
                    l.id_color = consultas.obtener_color_id_item(Convert.ToInt32(id_pedido), l.id_estilo);
                    l.color = consultas.obtener_color_id(Convert.ToString(l.id_color));
                    l.estilo = consultas.obtener_estilo(l.id_estilo);
                    l.descripcion = consultas.buscar_descripcion_estilo(l.id_estilo);
                    List<Empaque> lista_e = new List<Empaque>();
                    List<string> tipo_empaque_temporal= consultas.buscar_tipo_empaque(l.id_summary);
                    foreach(string s in tipo_empaque_temporal) {
                        Empaque e = new Empaque();
                        e.tipo_empaque = Convert.ToInt32(s);
                        if (s=="1"){ e.lista_ratio = obtener_lista_tallas_estilo(l.id_summary, l.id_estilo); }
                        if (s=="2"){ e.lista_ratio = obtener_lista_ratio(l.id_summary, l.id_estilo,2); }                       
                        lista_e.Add(e);   
                    }
                    l.lista_empaque = lista_e;
                    listar.Add(l);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return listar;
        }


        public List<ratio_tallas> obtener_lista_tallas_estilo(int posummary, int estilo) {
            List<ratio_tallas> lista = new List<ratio_tallas>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select ID_TALLA from ITEM_SIZE where ID_SUMMARY='" + posummary + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    ratio_tallas e = new ratio_tallas();
                    e.id_estilo = estilo;
                    e.id_talla = Convert.ToInt32(leerFilas["ID_TALLA"]);
                    e.talla = consultas.obtener_size_id(Convert.ToString(leerFilas["ID_TALLA"]));
                    e.ratio = buscar_piezas_empaque_bull(posummary, e.id_talla);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public int buscar_piezas_empaque_bull(int summary,int talla){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PIECES FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + summary + "' AND ID_TALLA='"+talla+"' and TYPE_PACKING=1 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["PIECES"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public string obtener_nombre_packing(int summary){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PACKING_NAME FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + summary + "' AND TYPE_PACKING=3 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["PACKING_NAME"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public int obtener_id_assort(string name){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_PACKING_ASSORT FROM PACKING_ASSORT WHERE PACKING_NAME='" + name + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["ID_PACKING_ASSORT"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public int buscar_existencia_inventario(int id_color, int id_talla, string id_estilo) {
            int temp = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario from inventario_fantasy where id_color='" + id_color + "' and id_talla='" + id_talla + "' and id_estilo='" + id_estilo + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    temp = Convert.ToInt32(leer["id_inventario"]);
                }
                leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public void guardar_recibo_fantasy(int pedido, int usuario, int piezas, int cajas) {
            Conexion con_c = new Conexion();
            try {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO recibos_fantasy(id_pedido,fecha,id_usuario,total,total_cajas) VALUES " +
                    " ('" + pedido + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + usuario + "','" + piezas + "','" + cajas + "')";
                com_c.ExecuteNonQuery();
            } finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public int obtener_ultimo_recibo() {
            int id_recibo = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT TOP 1 id_recibo FROM recibos_fantasy order by id_recibo desc ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id_recibo = Convert.ToInt32(leer_u_r["id_recibo"]);
                }
                leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id_recibo;
        }

        public void guardar_item_inventario(int color, int talla, string estilo, string descripcion, int total)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO inventario_fantasy(id_color,id_talla,id_estilo,descripcion,total) VALUES " +
                    " ('" + color + "','" + talla + "','" + estilo + "','" + descripcion + "','" + total + "')";
                com_c.ExecuteNonQuery();
            }
            finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public int obtener_ultimo_item() {
            int id = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT TOP 1 id_inventario FROM inventario_fantasy order by id_inventario desc ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id = Convert.ToInt32(leer_u_r["id_inventario"]);
                }
                leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }

        public void aumentar_inventario(int inventario, int cajas, int piezas) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario_fantasy SET total=(total+'" + cajas * piezas + "') WHERE id_inventario='" + inventario + "' ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        //ds.guardar_recibo_fantasy_item(id_recibo,id_inventario,Cajas[i],Piezas[i]);
        public void guardar_recibo_fantasy_item(int recibo, int inventario, string cajas, string piezas) {
            Conexion con_c = new Conexion();
            try {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO recibos_fantasy_items(id_recibo,id_inventario,cajas,piezas) VALUES " +
                    " ('" + recibo + "','" + inventario + "','" + cajas + "','" + piezas + "')";
                com_c.ExecuteNonQuery();
            } finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        //buscar_estilos_po
        public List<estilos> buscar_estilos_po(string po) {
            List<estilos> lista = new List<estilos>();
            int id_pedido = consultas.buscar_pedido(po);
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct ITEM_ID from PO_SUMMARY where ID_PEDIDOS='" + id_pedido + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    estilos e = new estilos();
                    e.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    e.estilo = consultas.buscar_descripcion_estilo(e.id_estilo);
                    e.estilo = Regex.Replace(e.estilo, @"\s+", " ");
                    lista.Add(e);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        /*public void guardar_estilos_dcs(string po, string estilos, string dcs, string number_po, int pk) {
            string[] estilo = estilos.Split('*'), dc = dcs.Split('*');
            //int id_pedido = consultas.buscar_pedido(po);
            for (int i = 1; i < estilo.Length; i++) {
                int summary = consultas.obtener_po_summary(Convert.ToInt32(po), Convert.ToInt32(estilo[i]));
                if (Convert.ToInt32(dc[i]) != 0) {
                    insertar_estilos_dcs(Convert.ToInt32(po), estilo[i], dc[0], dc[i], number_po, pk, summary);
                }
            }
        }*/
        //GUARDAR DCS
        /* public void insertar_estilos_dcs(int pedido, string estilo, string dc, string cajas, string number_po, int pk, int summary) {
             Conexion con_c = new Conexion();
             try {
                 SqlCommand com_c = new SqlCommand();
                 com_c.Connection = con_c.AbrirConexion();
                 com_c.CommandText = "INSERT INTO shipping_ids(id_pedido,id_estilo,dc,cantidad,id_tarima,number_po,used,id_po_summary) VALUES " +
                     " ('" + pedido + "','" + estilo + "','" + dc + "','" + cajas + "','0','" + number_po + "','" + pk + "','" + summary + "')";
                 com_c.ExecuteNonQuery();
             }
             finally { con_c.CerrarConexion(); con_c.Dispose(); }
         }*/
        //GUARDAR ESTILOS SIN DC
        /* public void guardar_estilos_paking(int pk, string number_po, string pedido, string estilo, string cantidad, string tienda, string tipo, string talla, int summary) {
             Conexion con_c = new Conexion();
             try {
                 SqlCommand com_c = new SqlCommand();
                 com_c.Connection = con_c.AbrirConexion();
                 com_c.CommandText = "INSERT INTO shipping_ids(cantidad,id_tarima,used,id_pedido,id_estilo,number_po,dc,id_po_summary,id_talla,store,tipo) VALUES " +
                     " ('" + cantidad + "','0','" + pk + "','" + pedido + "','" + estilo + "','" + number_po + "','0','" + summary + "','" + talla + "','" + tienda + "','" + tipo + "')";
                 com_c.ExecuteNonQuery();
             } finally { con_c.CerrarConexion(); con_c.Dispose(); }
         }*/
        //GUARDAR ESTILOS CON DC
        /* public void guardar_estilos_paking_dcs(int pk, string number_po, string pedido, string estilo, string cantidad, string tienda, string tipo, string talla, int summary, string dc) {
             Conexion con_c = new Conexion();
             try {
                 SqlCommand com_c = new SqlCommand();
                 com_c.Connection = con_c.AbrirConexion();
                 com_c.CommandText = "UPDATE shipping_ids SET store='" + tienda + "',tipo='" + tipo + "',id_talla=0 WHERE " +
                     "used='" + pk + "' and id_pedido='" + pedido + "' and id_estilo='" + estilo + "' and dc='" + dc + "' and id_po_summary='" + summary + "' ";
                 com_c.ExecuteNonQuery();
             } finally { con_c.CerrarConexion(); con_c.Dispose(); }
         }*/
        //GUARDAR ESTILOS ASSORTMENT
        /* public void guardar_estilos_paking_assort(int pk, string number_po, string pedido, string estilo, string cantidad, string tienda, string tipo) {
             Conexion con_c = new Conexion();
             try {
                 SqlCommand com_c = new SqlCommand();
                 com_c.Connection = con_c.AbrirConexion();
                 com_c.CommandText = "INSERT INTO shipping_ids(cantidad,id_tarima,used,id_pedido,id_estilo,number_po,dc,id_po_summary,id_talla,store,tipo) VALUES " +
                     " ('" + cantidad + "','0','" + pk + "','" + pedido + "','" + estilo + "','" + number_po + "','0','0','0','" + tienda + "','" + tipo + "')";
                 com_c.ExecuteNonQuery();
             }
             finally { con_c.CerrarConexion(); con_c.Dispose(); }
         }*/

        public void guardar_estilos_paking(string cantidad, string id_tarima, string used, string id_pedido, string id_estilo, string number_po, string dc, string id_po_summary, string id_talla, string store, string tipo, string ext, string tipo_empaque, string index_dc)
        {
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO shipping_ids(cantidad,id_tarima,used,id_pedido,id_estilo,number_po,dc,id_po_summary,id_talla,store,tipo,ext,tipo_empaque,index_dc)" +
                    " VALUES ('"+cantidad+ "','0','"+used+ "','"+id_pedido+ "','"+id_estilo+ "','"+number_po+ "','"+dc+ "','"+id_po_summary+ "','"+id_talla+"'," +
                    " '"+store+ "','"+tipo+ "','"+ext+ "','"+tipo_empaque+ "','"+index_dc+"')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public void agregar_cantidades_enviadas(string summary, string talla, string cantidad, string shipping_id, string tipo, string assort) {
            Conexion con_c = new Conexion();
            try {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO totales_envios(id_summary,id_talla,total,id_shipping_id,tipo,assort) VALUES " +
                    " ('" + summary + "','" + talla + "','" + cantidad + "','" + shipping_id + "','" + tipo + "','" + assort + "')";
                com_c.ExecuteNonQuery();
            } finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        /*public int buscar_tipo_pk(int summary) {
            int temp = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TYPE_PACKING from PACKING_TYPE_SIZE where ID_SUMMARY='" + summary + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    temp = Convert.ToInt32(leer["TYPE_PACKING"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }*/

        /*public void actualizar_tipo_empaque_pk(int pk, int tipo) {
            Conexion con_c = new Conexion();
            try {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE packing_list SET id_packing_type='" + tipo + "' WHERE  id_packing_list='" + pk + "'  ";
                com_c.ExecuteNonQuery();
            } finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }*/
        //obtener_lista_dc_estilos_id
        public List<Breakdown> obtener_lista_dc_id(string po_number) {
            List<Breakdown> lista = new List<Breakdown>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct dc from shipping_ids where number_po='" + po_number + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Breakdown e = new Breakdown();
                    e.dc = Convert.ToInt32(leerFilas["dc"]);
                    lista.Add(e);
                }
                leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<Breakdown> obtener_lista_estilos_id(string po_number) {
            List<Breakdown> lista = new List<Breakdown>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct id_estilo from shipping_ids where number_po='" + po_number + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Breakdown e = new Breakdown();
                    e.id_estilo = Convert.ToInt32(leerFilas["id_estilo"]);
                    e.estilo = consultas.obtener_estilo(e.id_estilo);
                    e.descripcion = consultas.buscar_descripcion_estilo(e.id_estilo);
                    e.descripcion = Regex.Replace(e.descripcion, @"\s+", " ");
                    e.id_pedido = buscar_pedido_dc(e.id_estilo, po_number);
                    e.id_color = consultas.obtener_color_id_item(e.id_pedido, e.id_estilo);
                    e.codigo_color = consultas.obtener_color_id(Convert.ToString(e.id_color));
                    e.codigo_color = Regex.Replace(e.codigo_color, @"\s+", " ");
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public int buscar_pedido_dc(int estilo, string po_number) {
            int id = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT id_pedido  FROM shipping_ids where id_estilo='" + estilo + "' and number_po='" + po_number + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id = Convert.ToInt32(leer_u_r["id_pedido"]);
                } leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }

        public void agregar_id_tarima_estilo_dc(string number_po, string dc, string id, string estilo) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE shipping_ids set id_tarima='" + id + "' where number_po='" + number_po + "' and dc='" + dc + "' and id_estilo='" + estilo + "' ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public void agregar_id_tarima_estilo_ppk(string cajas, string tarima, int pedido, string estilo, int summary) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO shipping_ids(cantidad,id_tarima,id_pedido,id_estilo,id_po_summary,id_talla)values('" + cajas + "','" + tarima + "','" + pedido + "','" + estilo + "','" + summary + "','0') ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void agregar_id_tarima_estilo_bulpack(string tarima, int pedido, string estilo, int summary, int talla, string total) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO shipping_ids(cantidad,id_tarima,id_pedido,id_estilo,id_po_summary,id_talla)values('" + total + "','" + tarima + "','" + pedido + "','" + estilo + "','" + summary + "','" + talla + "') ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public List<Drivers> obtener_drivers() {
            List<Drivers> lista = new List<Drivers>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_driver,carrier,driver_name,plates,scac,caat,tractor from drivers";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Drivers e = new Drivers();
                    e.id_driver = Convert.ToInt32(leerFilas["id_driver"]);
                    e.carrier = Convert.ToString(leerFilas["carrier"]);
                    e.driver_name = Convert.ToString(leerFilas["driver_name"]);
                    e.plates = Convert.ToString(leerFilas["plates"]);
                    e.scac = Convert.ToString(leerFilas["scac"]);
                    e.caat = Convert.ToString(leerFilas["caat"]);
                    e.tractor = Convert.ToString(leerFilas["tractor"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<Drivers> obtener_carriers()
        {
            List<Drivers> lista = new List<Drivers>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct carrier from drivers";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Drivers e = new Drivers();
                    e.carrier = Convert.ToString(leerFilas["carrier"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public void guardar_nuevo_conductor(string carrier, string nombre, string plates, string scac, string caat, string tractor) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO drivers(carrier,driver_name,plates,scac,caat,tractor)values" +
                    "('" + carrier + "','" + nombre + "','" + plates + "','" + scac + "','" + caat + "','" + tractor + "') ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public List<Drivers> obtener_conductor_edicion(string id)
        {
            List<Drivers> lista = new List<Drivers>();
            Conexion conn = new Conexion();
            try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_driver,carrier,driver_name,plates,scac,caat,tractor from drivers where id_driver='" + id + "'";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read())
                {
                    Drivers e = new Drivers();
                    e.id_driver = Convert.ToInt32(leerFilas["id_driver"]);
                    e.carrier = Convert.ToString(leerFilas["carrier"]);
                    e.driver_name = Convert.ToString(leerFilas["driver_name"]);
                    e.plates = Convert.ToString(leerFilas["plates"]);
                    e.scac = Convert.ToString(leerFilas["scac"]);
                    e.caat = Convert.ToString(leerFilas["caat"]);
                    e.tractor = Convert.ToString(leerFilas["tractor"]);
                    lista.Add(e);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public void guardar_conductor_edicion(string id, string carrier, string nombre, string plates, string scac, string caat, string tractor)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE drivers SET carrier='" + carrier + "', driver_name='" + nombre + "',plates='" + plates + "',scac='" + scac + "',caat='" + caat + "'," +
                    " tractor='" + tractor + "' where id_driver='" + id + "' ";
                com_s.ExecuteNonQuery();
            }
            finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public List<Direccion> obtener_direcciones() {
            List<Direccion> lista = new List<Direccion>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_direccion,nombre,direccion,ciudad,codigo_postal from direcciones_envio ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Direccion e = new Direccion();
                    e.id_direccion = Convert.ToInt32(leerFilas["id_direccion"]);
                    e.nombre = Convert.ToString(leerFilas["nombre"]);
                    e.direccion = Convert.ToString(leerFilas["direccion"]);
                    e.ciudad = Convert.ToString(leerFilas["ciudad"]);
                    e.zip = Convert.ToString(leerFilas["codigo_postal"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public void guardar_nueva_direccion(string nombre, string direccion, string ciudad, string zip) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO direcciones_envio(nombre,direccion,ciudad,codigo_postal)values" +
                    "('" + nombre + "','" + direccion + "','" + ciudad + "','" + zip + "') ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public List<Direccion> obtener_direccion_edicion(string id) {
            List<Direccion> lista = new List<Direccion>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_direccion,nombre,direccion,ciudad,codigo_postal from direcciones_envio where id_direccion='" + id + "'";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Direccion e = new Direccion();
                    e.id_direccion = Convert.ToInt32(leerFilas["id_direccion"]);
                    e.nombre = Convert.ToString(leerFilas["nombre"]);
                    e.direccion = Convert.ToString(leerFilas["direccion"]);
                    e.ciudad = Convert.ToString(leerFilas["ciudad"]);
                    e.zip = Convert.ToString(leerFilas["codigo_postal"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public void guardar_direccion_edicion(string id, string nombre, string direccion, string ciudad, string zip) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE direcciones_envio SET nombre='" + nombre + "', direccion='" + direccion + "',ciudad='" + ciudad + "',codigo_postal='" + zip + "' " +
                    "  where id_direccion='" + id + "' ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public void borrar_conductor(string id)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "DELETE FROM drivers  where id_driver='" + id + "' ";
                com_s.ExecuteNonQuery();
            }
            finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public void borrar_direccion(string id)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "DELETE FROM direcciones_envio  where id_direccion='" + id + "' ";
                com_s.ExecuteNonQuery();
            }
            finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public List<Container> obtener_contenedores_select() {
            List<Container> lista = new List<Container>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_container,eco,plates from containers ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Container e = new Container();
                    e.id_container = Convert.ToInt32(leerFilas["id_container"]);
                    e.eco = Convert.ToString(leerFilas["eco"]) + " " + Convert.ToString(leerFilas["plates"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        
        public List<Drivers> obtener_conductores_select() {
            List<Drivers> lista = new List<Drivers>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_driver,driver_name,plates from drivers";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Drivers e = new Drivers();
                    e.id_driver = Convert.ToInt32(leerFilas["id_driver"]);
                    e.driver_name = Convert.ToString(leerFilas["driver_name"]) + " " + Convert.ToString(leerFilas["plates"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<Direccion> obtener_direcciones_select() {
            List<Direccion> lista = new List<Direccion>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_direccion,nombre from direcciones_envio";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Direccion e = new Direccion();
                    e.id_direccion = Convert.ToInt32(leerFilas["id_direccion"]);
                    e.nombre = Convert.ToString(leerFilas["nombre"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<Tarima> obtener_lista_tarimas_estilos(int pedido) {
            List<Tarima> lista = new List<Tarima>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct id_tarima from shipping_ids where id_pedido='" + pedido + "' and id_tarima!=0 and used=0";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Tarima e = new Tarima();
                    e.id_tarima = Convert.ToInt32(leerFilas["id_tarima"]);
                    e.lista_estilos = buscar_lista_estilos_tarima(e.id_tarima);
                    //e.id_direccion = Convert.ToInt32(leerFilas["id_direccion"]);
                    //e.nombre = Convert.ToString(leerFilas["nombre"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<estilos> buscar_lista_estilos_tarima(int tarima) {
            List<estilos> lista = new List<estilos>();
            Conexion conn = new Conexion(); try
            {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_estilo,number_po,cantidad,dc from shipping_ids where id_tarima='" + tarima + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    estilos e = new estilos();
                    e.id_estilo = Convert.ToInt32(leerFilas["id_estilo"]);
                    e.estilo = consultas.obtener_estilo(e.id_estilo);
                    e.descripcion = consultas.buscar_descripcion_estilo(e.id_estilo);
                    e.number_po = Convert.ToInt32(leerFilas["number_po"]);
                    e.boxes = Convert.ToInt32(leerFilas["cantidad"]);
                    e.dc = Convert.ToString(leerFilas["dc"]);
                    lista.Add(e);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public string obtener_ultimo_pk() {
            string lista = "";
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select top 1 pk from packing_list order by id_packing_list desc ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    lista = Convert.ToString(leerFilas["pk"]);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public void guardar_pk_nuevo(int pedido, int customer, int customer_final, string pk, string address, string driver, string container, string seal, string replacement, string manager, string tipo, int usuario, string num_envio)
        {
            Conexion con_c = new Conexion();
            try {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO packing_list(pk,id_customer,id_customer_po,id_direccion_envio,id_pedido,id_driver,id_container,shipping_manager,seal,replacement,fecha,tipo,id_usuario,envio) VALUES " +
                    " ('" + pk + "','" + customer + "','" + customer_final + "','" + address + "','" + pedido + "','" + driver + "','" + container + "','" + manager + "','" + seal + "','" + replacement + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + tipo + "','" + usuario + "','" + num_envio + "')";
                com_c.ExecuteNonQuery();
            } finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public int obtener_ultimo_pk_registrado() {
            int lista = 0;
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select top 1 id_packing_list from packing_list order by id_packing_list desc ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    lista = Convert.ToInt32(leerFilas["id_packing_list"]);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public int obtener_ultimo_shipping_registrado() {
            int lista = 0;
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select top 1 id_shipping_id from shipping_ids order by id_shipping_id desc ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    lista = Convert.ToInt32(leerFilas["id_shipping_id"]);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public void guardar_pk_tarima(string tarima, int pk) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE shipping_ids SET used='" + pk + "' where id_tarima='" + tarima + "' ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void guardar_pk_labels(string label, int pk, string type_labels) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO ucc_labels(label,id_packing,tipo) VALUES('" + label + "','" + pk + "','" + type_labels + "') ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        //*******************************************************************************************************************************************************************************
        //*******************************************************************************************************************************************************************************
        public int summary, id_pedido;
        public List<Pk> obtener_packing_list(int pk) {
            List<Pk> lista = new List<Pk>();
            Conexion connx = new Conexion();
            try {
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select id_packing_list,pk,id_customer,id_customer_po,id_direccion_envio,id_pedido,id_driver,id_container,shipping_manager,seal,replacement,fecha,tipo,id_packing_type from packing_list where id_packing_list='" + pk + "' ";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()) {
                    Pk p = new Pk(); //Regex.Replace(color, @"\s+", " ");
                    p.id_packing_list = Convert.ToInt32(leerFilasx["id_packing_list"]);
                    p.packing = Convert.ToString(leerFilasx["pk"]);
                    p.customer = Regex.Replace(consultas.obtener_customer_id(Convert.ToString(leerFilasx["id_customer"])), @"\s+", " ");
                    p.customer_po = Regex.Replace(consultas.obtener_customer_final_id(Convert.ToString(leerFilasx["id_customer_po"])), @"\s+", " ");
                    p.destino = obtener_direccion(Convert.ToInt32(leerFilasx["id_direccion_envio"]));
                    p.pedido = consultas.obtener_po_id(Convert.ToString(leerFilasx["id_pedido"]));
                    id_pedido = Convert.ToInt32(leerFilasx["id_pedido"]);
                    p.conductor = obtener_driver(Convert.ToInt32(leerFilasx["id_driver"]));
                    p.contenedor = obtener_contenedor(Convert.ToInt32(leerFilasx["id_container"]));
                    p.shipping_manager = Convert.ToString(leerFilasx["shipping_manager"]);
                    p.seal = Convert.ToString(leerFilasx["seal"]);
                    p.replacement = Convert.ToString(leerFilasx["replacement"]);
                    p.fecha = (Convert.ToDateTime(leerFilasx["fecha"])).ToString("MM/dd/yyyy");
                    p.parte = buscar_pks_pedido(id_pedido, p.id_packing_list);
                    /*if (p.tipo_empaque != 3) {
                        p.lista_tarimas = obtener_tarimas(p.id_packing_list, p.tipo_empaque);
                    } else {
                        p.lista_tarimas = obtener_tarimas_assort(p.id_packing_list, p.tipo_empaque);
                    }*/
                    p.number_po = buscar_number_po(id_pedido);
                    p.tipo = leerFilasx["tipo"].ToString();//TIPO DE PACKING LIST
                    // p.tipo_empaque = Convert.ToInt32(leerFilasx["id_packing_type"]);
                    p.lista_tarimas = obtener_tarimas(p.id_packing_list);
                    p.siglas_cliente = consultas.obtener_siglas_cliente(Convert.ToString(leerFilasx["id_customer_po"]));


                    lista.Add(p);
                } leerFilasx.Close();
            } finally { connx.CerrarConexion(); connx.Dispose(); }
            return lista;
        }
        public Direccion obtener_direccion(int direccion) {
            Direccion d = new Direccion();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select nombre,direccion,ciudad,codigo_postal from direcciones_envio where id_direccion='" + direccion + "'";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    d.nombre = Convert.ToString(leerFilas["nombre"]);
                    d.direccion = Convert.ToString(leerFilas["direccion"]);
                    d.ciudad = Convert.ToString(leerFilas["ciudad"]);
                    d.zip = Convert.ToString(leerFilas["codigo_postal"]);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return d;
        }
        public Drivers obtener_driver(int driver) {
            Drivers d = new Drivers();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select carrier,driver_name,plates,scac,caat,tractor from drivers where id_driver='" + driver + "'";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    d.carrier = Convert.ToString(leerFilas["carrier"]);
                    d.driver_name = Convert.ToString(leerFilas["driver_name"]);
                    d.plates = Convert.ToString(leerFilas["plates"]);
                    d.scac = Convert.ToString(leerFilas["scac"]);
                    d.caat = Convert.ToString(leerFilas["caat"]);
                    d.tractor = Convert.ToString(leerFilas["tractor"]);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return d;
        }
        public Container obtener_contenedor(int container) {
            Container d = new Container();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select eco,plates from containers where id_container='" + container + "'";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    d.eco = Convert.ToString(leerFilas["eco"]);
                    d.plates = Convert.ToString(leerFilas["plates"]);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return d;
        }
        public string buscar_dc_pk(int pk) {////TAL VEZ AQUI TENGA QUE ACOMODAR PARA LOS EXCEL
            string tempo = "";
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT dc from shipping_ids where used='" + pk + "' and dc!='0' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToString(leer["dc"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }

        public int contar_labels(int pk) {
            int tempo = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT tipo from ucc_labels where id_packing='" + pk + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToInt32(leer["tipo"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }

        public List<Labels> obtener_etiquetas(int pk) {
            List<Labels> lista = new List<Labels>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_label,label,tipo from ucc_labels where id_packing='" + pk + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Labels e = new Labels();
                    e.id_label = Convert.ToInt32(leerFilas["id_label"]);
                    e.label = Convert.ToString(leerFilas["label"]);
                    e.tipo = Convert.ToString(leerFilas["tipo"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<Tarima> obtener_tarimas(int pk) {
            List<Tarima> lista = new List<Tarima>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct id_tarima from shipping_ids where used='" + pk + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    if (Convert.ToInt32(leerFilas["id_tarima"]) != 0) {
                        Tarima e = new Tarima();
                        e.id_tarima = Convert.ToInt32(leerFilas["id_tarima"]);
                        e.lista_estilos = obtener_lista_estilos_tarima(e.id_tarima);
                        lista.Add(e);
                    }
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<Tarima> obtener_tarimas_assort(int pk, int tipo_empaque) {
            List<Tarima> lista = new List<Tarima>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct id_tarima from shipping_ids where used='" + pk + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    if (Convert.ToInt32(leerFilas["id_tarima"]) != 0) {
                        Tarima e = new Tarima();
                        e.id_tarima = Convert.ToInt32(leerFilas["id_tarima"]);
                        e.lista_assortments = obtener_lista_assort(e.id_tarima);
                        //e.lista_estilos = obtener_lista_estilos_tarima(e.id_tarima, tipo_empaque);
                        lista.Add(e);
                    }
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<Assortment> obtener_lista_assort(int tarima) {
            List<Assortment> lista = new List<Assortment>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_estilo,id_shipping_id,number_po,store,tipo,id_talla,cantidad,id_po_summary from shipping_ids where id_tarima='" + tarima + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Assortment a = new Assortment();
                    a = obtener_assortment(Convert.ToInt32(leerFilas["id_estilo"]), tarima, Convert.ToInt32(leerFilas["id_shipping_id"]), Convert.ToInt32(leerFilas["number_po"]), Convert.ToString(leerFilas["store"]), Convert.ToString(leerFilas["tipo"]), Convert.ToInt32(leerFilas["id_talla"]), Convert.ToInt32(leerFilas["cantidad"]), Convert.ToInt32(leerFilas["id_po_summary"]));

                    lista.Add(a);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public Assortment obtener_assortment(int id, int tarima, int id_shipping, int number_po, string store, string tipo, int id_talla, int cantidad, int po_summary) {//OBTENER EL ASSORTMENT CON ESTILOS
            Assortment a = new Assortment();
            List<estilos> lista = new List<estilos>();
            string packing_name = buscar_packing_name(id);
            if (tipo != "NONE" && tipo != "INITIAL") {
                estilos e = new estilos();
                a.id_summary = po_summary;
                a.nombre = " ";
                a.cartones = 1;
                e.lista_ratio = obtener_lista_ratio_otros(id_shipping);
                e.id_estilo = consultas.obtener_estilo_summary(a.id_summary);
                e.id_po_summary = a.id_summary;
                e.id_color = consultas.obtener_color_id_item_cat(a.id_summary);
                e.color = Regex.Replace(consultas.obtener_color_id((e.id_color).ToString()), @"\s+", " ");
                e.estilo = Regex.Replace(consultas.obtener_estilo(e.id_estilo), @"\s+", " ");
                e.descripcion = Regex.Replace(consultas.buscar_descripcion_estilo(e.id_estilo), @"\s+", " ");
                e.number_po = number_po;
                e.boxes = cantidad;//cantidad de cartones
                e.dc = "0";
                e.descripcion_final = Regex.Replace(buscar_descripcion_final_estilo(a.id_summary), @"\s+", " ");
                e.tipo = tipo;
                e.store = store;
                e.id_talla = id_talla;
                if (e.id_talla != 0) { e.talla = consultas.obtener_size_id(Convert.ToString(e.id_talla)); }
                lista.Add(e);
            } else {
                Conexion conna1 = new Conexion();
                try {
                    SqlCommand comandoa1 = new SqlCommand();
                    SqlDataReader leerFilasa1 = null;
                    comandoa1.Connection = conna1.AbrirConexion();
                    comandoa1.CommandText = "select distinct PTZ.ID_SUMMARY from PACKING_ASSORT PA,PACKING_TYPE_SIZE PTZ where " +
                        " PA.PACKING_NAME=PTZ.PACKING_NAME and PA.ID_PACKING_ASSORT='" + id + "'   ";
                    leerFilasa1 = comandoa1.ExecuteReader();
                    while (leerFilasa1.Read()) {
                        estilos e = new estilos();
                        a.id_summary = Convert.ToInt32(leerFilasa1["ID_SUMMARY"]);
                        a.nombre = buscar_nombre_assort(id);
                        a.cartones = cantidad;
                        e.lista_ratio = obtener_lista_ratio_assort(a.id_summary, e.id_estilo, packing_name);
                        e.id_estilo = consultas.obtener_estilo_summary(a.id_summary);
                        e.id_po_summary = a.id_summary;
                        e.id_color = consultas.obtener_color_id_item_cat(a.id_summary);
                        e.color = Regex.Replace(consultas.obtener_color_id((e.id_color).ToString()), @"\s+", " ");
                        e.estilo = Regex.Replace(consultas.obtener_estilo(e.id_estilo), @"\s+", " ");
                        e.descripcion = Regex.Replace(consultas.buscar_descripcion_estilo(e.id_estilo), @"\s+", " ");
                        e.number_po = number_po;
                        e.boxes = cantidad;//cantidad de cartones
                        e.dc = "0";
                        e.descripcion_final = Regex.Replace(buscar_descripcion_final_estilo(a.id_summary), @"\s+", " ");
                        e.tipo = tipo;
                        e.store = store;
                        e.id_talla = id_talla;
                        if (e.id_talla != 0) { e.talla = consultas.obtener_size_id(Convert.ToString(e.id_talla)); }
                        lista.Add(e);
                    } leerFilasa1.Close();
                } finally { conna1.CerrarConexion(); conna1.Dispose(); }
            }
            a.lista_estilos = lista;
            return a;
        }
        public List<ratio_tallas> obtener_lista_ratio_assort(int posummary, int estilo, string packing_name) {
            List<ratio_tallas> lista = new List<ratio_tallas>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select ID_TALLA,RATIO from PACKING_TYPE_SIZE where ID_SUMMARY='" + posummary + "' and PACKING_NAME='" + packing_name + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    ratio_tallas e = new ratio_tallas();
                    e.id_estilo = estilo;
                    e.id_talla = Convert.ToInt32(leerFilas["ID_TALLA"]);
                    e.talla = Regex.Replace(consultas.obtener_size_id(Convert.ToString(leerFilas["ID_TALLA"])), @"\s+", " ");
                    e.ratio = Convert.ToInt32(leerFilas["RATIO"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<estilos> obtener_lista_estilos_tarima(int tarima) {
            List<estilos> lista = new List<estilos>();
            Conexion conn = new Conexion();
            try { //Regex.Replace(color, @"\s+", " ");
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_shipping_id,id_estilo,number_po,cantidad,dc,id_po_summary,id_talla,store,tipo,ext,tipo_empaque,index_dc,dc,id_pedido from shipping_ids where id_tarima='" + tarima + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    estilos e = new estilos();
                    e.id_estilo = Convert.ToInt32(leerFilas["id_estilo"]);
                    summary = consultas.obtener_po_summary(id_pedido, e.id_estilo);
                    e.id_po_summary = Convert.ToInt32(leerFilas["id_po_summary"]);
                    e.id_color = consultas.obtener_color_id_item_cat(e.id_po_summary);
                    e.color = Regex.Replace(consultas.obtener_color_id((e.id_color).ToString()), @"\s+", " ");
                    e.estilo = Regex.Replace(consultas.obtener_estilo(e.id_estilo), @"\s+", " ");
                    e.descripcion = Regex.Replace(consultas.buscar_descripcion_estilo(e.id_estilo), @"\s+", " ");
                    e.number_po = Convert.ToInt32(leerFilas["number_po"]);
                    e.boxes = Convert.ToInt32(leerFilas["cantidad"]);
                    e.dc = Convert.ToString(leerFilas["dc"]);
                    e.descripcion_final = Regex.Replace(buscar_descripcion_final_estilo(summary), @"\s+", " ");
                    e.tipo = Convert.ToString(leerFilas["tipo"]);                    
                    e.id_talla = Convert.ToInt32(leerFilas["id_talla"]);
                     if (e.id_talla != 0) {
                         e.talla = consultas.obtener_size_id(Convert.ToString(e.id_talla));
                         e.piezas = buscar_cajas_talla_estilo(summary, e.id_talla);
                    }
                    e.store = Convert.ToString(leerFilas["store"]);
                    e.dc = Convert.ToString(leerFilas["dc"]);
                    e.ext = Convert.ToString(leerFilas["ext"]);
                    e.tipo_empaque = Convert.ToInt32(leerFilas["tipo_empaque"]);
                    e.index_dc = Convert.ToInt32(leerFilas["index_dc"]);
                    if (e.tipo == "DMG" || e.tipo == "EXT" || e.tipo == "EXAMPLES") {
                        e.lista_ratio = obtener_lista_ratio_otros(Convert.ToInt32(leerFilas["id_shipping_id"]));
                    }else{
                        if (e.tipo_empaque ==1){
                            e.lista_ratio = obtener_lista_ratio(summary, e.id_estilo,1);
                        }
                        if (e.tipo_empaque ==2){
                            e.lista_ratio = obtener_lista_ratio(summary, e.id_estilo,2);
                        }
                        if (e.tipo_empaque == 3){
                            e.assort = assortment_id(e.id_po_summary, Convert.ToInt32(leerFilas["id_pedido"]));
                            e.assort_nombre = obtener_nombre_assort(e.id_po_summary);
                        }
                    }
                   
                    e.usado = 0;
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public string obtener_nombre_assort(int po_summary){
            string cadena = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PTS.ASSORT_NAME from PACKING_ASSORT PA,PACKING_TYPE_SIZE PTS where PA.ID_PACKING_ASSORT='" + po_summary + "'" +
                    " and PA.PACKING_NAME=PTS.PACKING_NAME ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    cadena = Convert.ToString(leer["ASSORT_NAME"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return cadena;
        }
        public string buscar_descripcion_final_estilo(int estilo)
        {
            string cadena = "";
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_GENDER,ID_TELA,ID_PRODUCT_TYPE from PO_SUMMARY where ID_PO_SUMMARY='" + estilo + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    cadena = consultas.obtener_sigla_genero(Convert.ToString(leer["ID_GENDER"])) + " " + consultas.obtener_sigla_product_type(Convert.ToString(leer["ID_PRODUCT_TYPE"])) + " " + consultas.obtener_sigla_fabric(Convert.ToString(leer["ID_TELA"]));
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return cadena;
        }
        public List<ratio_tallas> obtener_lista_ratio(int posummary, int estilo,int tipo_empaque) {
            List<ratio_tallas> lista = new List<ratio_tallas>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select ID_TALLA,RATIO,PIECES from PACKING_TYPE_SIZE where ID_SUMMARY='" + posummary + "' and TYPE_PACKING='"+tipo_empaque+"' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    ratio_tallas e = new ratio_tallas();
                    e.id_estilo = estilo; //Regex.Replace(color, @"\s+", " ");
                    e.id_talla = Convert.ToInt32(leerFilas["ID_TALLA"]);
                    e.talla = Regex.Replace(consultas.obtener_size_id(Convert.ToString(leerFilas["ID_TALLA"])), @"\s+", " ");
                    e.ratio = Convert.ToInt32(leerFilas["RATIO"]);
                    e.piezas = Convert.ToInt32(leerFilas["PIECES"]);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
       
        public List<ratio_tallas> obtener_lista_ratio_assort_r(int posummary, int estilo,string packing_name){///***********************************************************        
            List<ratio_tallas> lista = new List<ratio_tallas>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();//obtener el packing name 
                comando.CommandText = "SELECT PTS.ID_TALLA,PTS.RATIO,PTS.PACKING_NAME,PTS.ASSORT_NAME,PA.ID_PACKING_ASSORT " +
                    "FROM PACKING_TYPE_SIZE PTS,PACKING_ASSORT PA where PA.PACKING_NAME='" + packing_name + "'" +
                    " AND PA.ID_BLOCK=PTS.ID_BLOCK_PACK AND PA.PACKING_NAME=PTS.PACKING_NAME ";                  
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    ratio_tallas e = new ratio_tallas();
                    e.id_estilo = estilo;
                    e.id_talla = Convert.ToInt32(leerFilas["ID_TALLA"]);
                    e.talla = consultas.obtener_size_id(Convert.ToString(leerFilas["ID_TALLA"]));
                    e.id_packing_assort= Convert.ToInt32(leerFilas["ID_PACKING_ASSORT"]);
                    e.packing_name= Convert.ToString(leerFilas["PACKING_NAME"]);
                    e.assort_name= Convert.ToString(leerFilas["ASSORT_NAME"]);
                    e.ratio= Convert.ToInt32(leerFilas["RATIO"]);
                    lista.Add(e);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<ratio_tallas> obtener_lista_ratio_otros(int shipping) {
            List<ratio_tallas> lista = new List<ratio_tallas>();
            Conexion conn = new Conexion();
            try { //Regex.Replace(color, @"\s+", " ");
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_talla,total,id_shipping_id from shipping_ratio where id_shipping_id='" + shipping + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    ratio_tallas e = new ratio_tallas();
                    //e.id_estilo = estilo;
                    e.id_talla = Convert.ToInt32(leerFilas["ID_TALLA"]);
                    e.talla = Regex.Replace(consultas.obtener_size_id(Convert.ToString(leerFilas["ID_TALLA"])), @"\s+", " ");
                    e.ratio = Convert.ToInt32(leerFilas["total"]);
                    lista.Add(e);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public string buscar_pks_pedido(int pedido, int pk) {
            string cadena = "";
            int parte = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_pedido from packing_list where id_pedido='" + pedido + "' and id_packing_list<='" + pk + "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    parte++;
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            switch (parte) {
                case 1: cadena = "1st Part"; break;
                case 2: cadena = "2nd Part"; break;
                case 3: cadena = "3rd Part"; break;
                default: cadena = Convert.ToString(parte) + "st Part"; break;
            }
            return cadena;
        }
        public string buscar_number_po(int pedido) {
            string cadena = "";
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT number_po from shipping_ids where id_pedido='" + pedido + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    cadena = Convert.ToString(leer["number_po"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return cadena;
        }
        //***************************************************************
        public List<Pk> lista_buscar_pk_inicio(string busqueda) {
            List<Pk> lista = new List<Pk>();
            Conexion connx = new Conexion();
            try {
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "(select distinct used from shipping_ids where used like'%" + busqueda + "%') union (select distinct used from shipping_ids where id_tarima like'%" + busqueda + "%' ) union (select distinct used from shipping_ids where  number_po like'%" + busqueda + "%' ) union (select distinct used from shipping_ids where dc like'%" + busqueda + "%' ) union ( select id_packing_list from packing_list where pk like'%" + busqueda + "%' )";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()) {
                    Pk p = new Pk();
                    p = obtener_packing(Convert.ToInt32(leerFilasx["used"]));
                    lista.Add(p);
                } leerFilasx.Close();
            } finally { connx.CerrarConexion(); connx.Dispose(); }
            return lista;
        }
        public Pk obtener_packing(int pk) {
            Pk p = new Pk();
            Conexion connx = new Conexion();
            try {
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select id_packing_list,pk,id_customer_po,fecha,id_packing_type from packing_list where id_packing_list='" + pk + "' ";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()) {
                    p.id_packing_list = Convert.ToInt32(leerFilasx["id_packing_list"]);
                    p.packing = Convert.ToString(leerFilasx["pk"]);
                    p.customer_po = consultas.obtener_customer_final_id(Convert.ToString(leerFilasx["id_customer_po"]));
                    p.fecha = (Convert.ToDateTime(leerFilasx["fecha"])).ToString("MM/dd/yyyy");
                    // p.lista_tarimas = obtener_tarimas(p.id_packing_list, Convert.ToInt32(leerFilasx["id_packing_type"]));
                } leerFilasx.Close();
            } finally { connx.CerrarConexion(); connx.Dispose(); }
            return p;
        }
        public int obtener_id_staging(int pedido, int estilo, int talla) {
            int tempo = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT s.id_staging from staging s,staging_count sc " +
                    "where s.id_staging=sc.id_staging and  s.id_pedido='" + pedido + "' and s.id_estilo='" + estilo + "' and sc.id_talla='" + talla + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToInt32(leer["id_staging"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        public int obtener_id_pais_staging(int staging, int talla) {
            int tempo = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_pais from staging_count where id_staging='" + staging + "' and id_talla='" + talla + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToInt32(leer["id_pais"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        public int buscar_cajas_talla_estilo(int summary, int talla) {
            int tempo = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT PIECES from PACKING_TYPE_SIZE where ID_SUMMARY='" + summary + "' and ID_TALLA='" + talla + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToInt32(leer["PIECES"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        public List<Fabricantes> buscar_paises_estilos(List<estilos> lista_estilo) {
            List<Fabricantes> lista = new List<Fabricantes>();
            foreach (estilos e in lista_estilo) {
                Conexion con = new Conexion();
                try {
                    SqlCommand com = new SqlCommand();
                    SqlDataReader leer = null;
                    com.Connection = con.AbrirConexion();
                    com.CommandText = "SELECT sc.id_pais,sc.total,sc.id_porcentaje from staging_count sc,staging s where sc.id_staging=s.id_staging and s.id_summary='" + e.id_po_summary + "'  ";
                    leer = com.ExecuteReader();
                    while (leer.Read()) {
                        Fabricantes f = new Fabricantes();
                        f.id_pais = Convert.ToInt32(leer["id_pais"]);
                        f.pais = consultas.obtener_pais_id(Convert.ToString(f.id_pais));
                        f.cantidad = Convert.ToInt32(leer["total"]);
                        f.percent = consultas.obtener_fabric_percent_id(Convert.ToString(leer["id_porcentaje"]));
                        lista.Add(f);
                    } leer.Close();
                } finally { con.CerrarConexion(); con.Dispose(); }
            }
            return lista;
        }
        public int obtener_id_porcentaje_staging(int staging, int talla) {
            int tempo = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_porcentaje from staging_count where id_staging='" + staging + "' and id_talla='" + talla + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToInt32(leer["id_porcentaje"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        //obtener_lista_po_summarys
        public List<Breakdown> obtener_lista_po_shipping() {
            List<Breakdown> lista = new List<Breakdown>();
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DISTINCT ID_PEDIDO,PO FROM PEDIDO WHERE CUSTOMER=1 AND ID_STATUS!=6 AND ID_STATUS!=7";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Breakdown b = new Breakdown();
                    b.id_pedido = Convert.ToInt32(leer["ID_PEDIDO"]);
                    b.po = Convert.ToString(leer["PO"]);
                    lista.Add(b);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<estilos> lista_estilos_packing(int pk) {
            List<estilos> lista = new List<estilos>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_shipping_id,id_estilo,number_po,cantidad,dc,id_po_summary,tipo_empaque,index_dc,tipo,id_pedido,id_talla,store from shipping_ids where used='" + pk + "' and id_tarima=0  "; 
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    estilos e = new estilos();
                    e.id_shipping_id = Convert.ToInt32(leerFilas["id_shipping_id"]);
                    e.id_estilo = Convert.ToInt32(leerFilas["id_estilo"]);
                    e.id_po_summary=Convert.ToInt32(leerFilas["id_po_summary"]);
                    e.id_color = consultas.obtener_color_id_item_cat(e.id_po_summary);
                    e.color = consultas.obtener_color_id((e.id_color).ToString());
                    e.estilo = consultas.obtener_estilo(e.id_estilo);
                    e.descripcion = consultas.buscar_descripcion_estilo(e.id_estilo);
                    e.number_po = Convert.ToInt32(leerFilas["number_po"]);
                    e.boxes = Convert.ToInt32(leerFilas["cantidad"]);
                    e.index_dc = Convert.ToInt32(leerFilas["index_dc"]);
                    e.tipo_empaque= Convert.ToInt32(leerFilas["tipo_empaque"]);
                    e.id_talla= Convert.ToInt32(leerFilas["id_talla"]);
                    e.tipo= Convert.ToString(leerFilas["tipo"]);
                    e.store= Convert.ToString(leerFilas["store"]);
                    if (e.tipo == "DMG" || e.tipo == "EXT" || e.tipo == "ECOM"){
                            e.lista_ratio = obtener_lista_ratio_otros(Convert.ToInt32(leerFilas["id_shipping_id"]));
                    }else{
                        if (e.tipo_empaque ==1){
                            e.lista_ratio = obtener_lista_ratio(e.id_po_summary, e.id_estilo,1);
                            e.piezas= buscar_piezas_empaque_bull(e.id_po_summary, e.id_talla);
                        }
                        if (e.tipo_empaque == 2){
                            e.lista_ratio = obtener_lista_ratio(e.id_po_summary, e.id_estilo, 2);
                        }
                        if (e.tipo_empaque == 3){
                            e.assort = assortment_id(e.id_po_summary, Convert.ToInt32(leerFilas["id_pedido"]));
                            e.assort_nombre = obtener_nombre_assort(e.id_po_summary);
                        }
                    }                    
                    e.dc = Convert.ToString(leerFilas["dc"]);
                    e.pk = pk;
                    
                    lista.Add(e);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<estilos> lista_estilos_packing_edicion(int pk){
            List<estilos> lista = new List<estilos>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_shipping_id,id_estilo,number_po,cantidad,dc,id_po_summary,tipo_empaque,index_dc,tipo,id_pedido,id_talla,store,ext from shipping_ids where used='" + pk + "' and tipo!='DMG' and tipo!='EXT'  ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    estilos e = new estilos();
                    e.id_shipping_id = Convert.ToInt32(leerFilas["id_shipping_id"]);
                    e.id_estilo = Convert.ToInt32(leerFilas["id_estilo"]);
                    e.id_po_summary = Convert.ToInt32(leerFilas["id_po_summary"]);
                    e.id_color = consultas.obtener_color_id_item_cat(e.id_po_summary);
                    e.color = consultas.obtener_color_id((e.id_color).ToString());
                    e.estilo = consultas.obtener_estilo(e.id_estilo);
                    e.descripcion = consultas.buscar_descripcion_estilo(e.id_estilo);
                    e.number_po = Convert.ToInt32(leerFilas["number_po"]);
                    e.boxes = Convert.ToInt32(leerFilas["cantidad"]);
                    e.index_dc = Convert.ToInt32(leerFilas["index_dc"]);
                    e.tipo_empaque = Convert.ToInt32(leerFilas["tipo_empaque"]);
                    e.id_talla = Convert.ToInt32(leerFilas["id_talla"]);
                    e.talla = consultas.obtener_size_id(Convert.ToString(e.id_talla));
                    e.tipo = Convert.ToString(leerFilas["tipo"]);
                    e.store = Convert.ToString(leerFilas["store"]);
                    e.ext = Convert.ToString(leerFilas["ext"]);
                    if (e.tipo == "DMG" || e.tipo == "EXT" || e.tipo == "ECOM"){
                        e.lista_ratio = obtener_lista_ratio_otros(Convert.ToInt32(leerFilas["id_shipping_id"]));
                    }else{
                        if (e.tipo_empaque == 1){
                            e.lista_ratio = obtener_lista_ratio(e.id_po_summary, e.id_estilo, 1);
                            e.piezas = buscar_piezas_empaque_bull(e.id_po_summary, e.id_talla);
                        }
                        if (e.tipo_empaque == 2){
                            e.lista_ratio = obtener_lista_ratio(e.id_po_summary, e.id_estilo, 2);
                        }
                        if (e.tipo_empaque == 3){
                            e.assort = assortment_id(e.id_po_summary, Convert.ToInt32(leerFilas["id_pedido"]));
                            e.assort_nombre = obtener_nombre_assort(e.id_po_summary);
                        }
                    }
                    e.dc = Convert.ToString(leerFilas["dc"]);
                    e.pk = pk;

                    lista.Add(e);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }


        public void guardar_ids_tarimas(string tarima, string shipping) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE shipping_ids SET id_tarima='" + tarima + "' where id_shipping_id='" + shipping + "' ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void guardar_ids_tarimas_bpdc(string tarima,string packing,string index){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE shipping_ids SET id_tarima='" + tarima + "' where used='" + packing + "'" +
                    " AND index_dc='"+index+"' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }




        public List<Assortment> lista_assortments_pedido(int pedido) {
            List<Assortment> lista = new List<Assortment>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct PACKING_NAME from PACKING_ASSORT where ID_PEDIDO='" + pedido + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Assortment a = new Assortment();
                    a = obtener_assortment_pedido(Convert.ToString(leerFilas["PACKING_NAME"]), pedido);
                    lista.Add(a);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public Assortment assortment_id(int id,int pedido){
            Assortment a = new Assortment();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct PACKING_NAME from PACKING_ASSORT where ID_PACKING_ASSORT='" + id + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    a = obtener_assortment_pedido(Convert.ToString(leerFilas["PACKING_NAME"]), pedido);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return a;
        }
        public Assortment obtener_assortment_pedido(string nombre_packing, int pedido) {
            Assortment aa = new Assortment();
            aa.cartones = 0;
            Conexion conna1 = new Conexion();
            try {
                SqlCommand comandoa1 = new SqlCommand();
                SqlDataReader leerFilasa1 = null;
                comandoa1.Connection = conna1.AbrirConexion();
                comandoa1.CommandText = "select ID_PACKING_ASSORT,ID_BLOCK,CANT_CARTONS,PACKING_NAME from PACKING_ASSORT where ID_PEDIDO='" + pedido + "' " +
                    "and PACKING_NAME='" + nombre_packing + "'";
                leerFilasa1 = comandoa1.ExecuteReader();
                while (leerFilasa1.Read()) {
                    aa.id_assortment = Convert.ToInt32(leerFilasa1["ID_PACKING_ASSORT"]);
                    aa.block = Convert.ToInt32(leerFilasa1["ID_BLOCK"]);
                    aa.cartones += Convert.ToInt32(leerFilasa1["CANT_CARTONS"]);
                    aa.nombre = Convert.ToString(leerFilasa1["PACKING_NAME"]);                    
                    aa.lista_estilos = obtener_lista_estilos_assort(nombre_packing, pedido);
                }leerFilasa1.Close();
            } finally { conna1.CerrarConexion(); conna1.Dispose(); }
            return aa;
        }
        public List<estilos> obtener_lista_estilos_assort(string nombre_packing, int pedido) {
            List<estilos> lista = new List<estilos>();
            Conexion conna2 = new Conexion();
            try {
                SqlCommand comandoa2 = new SqlCommand();
                SqlDataReader leerFilasa2 = null;
                comandoa2.Connection = conna2.AbrirConexion();
                comandoa2.CommandText = "select DISTINCT ID_SUMMARY FROM PACKING_TYPE_SIZE WHERE PACKING_NAME='"+nombre_packing+"' ";
                leerFilasa2 = comandoa2.ExecuteReader();
                while (leerFilasa2.Read()) {
                    estilos e = new estilos();
                    e.id_po_summary= Convert.ToInt32(leerFilasa2["ID_SUMMARY"]);
                    e.id_estilo = consultas.obtener_estilo_summary(e.id_po_summary);
                    e.estilo = consultas.obtener_estilo(e.id_estilo);
                    e.descripcion = consultas.buscar_descripcion_estilo(e.id_estilo);
                    e.id_color = consultas.obtener_color_id_item(pedido, e.id_estilo);
                    e.color = consultas.obtener_color_id((e.id_color).ToString());
                    //e.lista_ratio = obtener_lista_ratio(e.id_po_summary, e.id_estilo);
                    e.lista_ratio = obtener_lista_ratio_assort_r(e.id_po_summary, e.id_estilo, nombre_packing);
                    lista.Add(e);
                }leerFilasa2.Close();
            } finally { conna2.CerrarConexion(); conna2.Dispose(); }
            return lista;
        }
        public int obtener_tipo_empaque_pk(int pk) {
            int tempo = 0;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select id_packing_type from packing_list where id_packing_list='" + pk + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToInt32(leer["id_packing_type"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        public List<Assortment> lista_assortings_packing(int pk) {
            List<Assortment> lista = new List<Assortment>();
            Conexion conn = new Conexion();
            try {
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_shipping_id,id_estilo,cantidad from shipping_ids where used='" + pk + "' and id_tarima=0";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    Assortment a = new Assortment();
                    a.id_assortment = Convert.ToInt32(leerFilas["id_shipping_id"]);
                    a.cartones = Convert.ToInt32(leerFilas["cantidad"]);
                    a.nombre = buscar_nombre_assort(Convert.ToInt32(leerFilas["id_estilo"]));
                    a.pk = pk;
                    lista.Add(a);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public string buscar_nombre_assort(int assort) {
            string tempo = "";
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select PTZ.ASSORT_NAME FROM PACKING_ASSORT PA,PACKING_TYPE_SIZE PTZ WHERE PA.PACKING_NAME=PTZ.PACKING_NAME and PA.ID_PACKING_ASSORT='" + assort + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToString(leer["ASSORT_NAME"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        
        public Pk obtener_informacion_editar_pk(int pk) {
            Pk p = new Pk();
            Conexion connx = new Conexion();
            try {
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select id_packing_list,pk,id_driver,id_container,seal,replacement,id_direccion_envio from packing_list where id_packing_list='" + pk + "' ";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()) {
                    p.id_packing_list = Convert.ToInt32(leerFilasx["id_packing_list"]);
                    p.packing = Convert.ToString(leerFilasx["pk"]);
                    p.id_driver = Convert.ToInt32(leerFilasx["id_driver"]);
                    p.id_container = Convert.ToInt32(leerFilasx["id_container"]);
                    p.seal = Convert.ToString(leerFilasx["seal"]);
                    p.replacement = Convert.ToString(leerFilasx["replacement"]);
                    p.id_direccion_envio = Convert.ToInt32(leerFilasx["id_direccion_envio"]);
                } leerFilasx.Close();
            } finally { connx.CerrarConexion(); connx.Dispose(); }
            return p;
        }
        public void actualizar_datos_pk(string id, string sello, string replacement, string conductor, string contenedor,string direccion) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE packing_list SET id_driver='" + conductor + "',id_container='" + contenedor + "'," +
                    "seal='" + sello + "',replacement='" + replacement + "',id_direccion_envio='"+direccion+"' where id_packing_list='" + id + "'  ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
       
        public int obtener_ultimo_shipping_id() {
            int id = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT TOP 1 id_shipping_id FROM shipping_ids order by id_shipping_id desc ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id = Convert.ToInt32(leer_u_r["id_shipping_id"]);
                } leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
       
        public string buscar_po_number_pk(string pk) {
            string id = "";
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT number_po FROM shipping_ids where used='" + pk + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id = Convert.ToString(leer_u_r["number_po"]);
                } leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
        public int buscar_pedido_pk(string pk) {
            int id = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT id_pedido FROM shipping_ids where used='" + pk + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id = Convert.ToInt32(leer_u_r["id_pedido"]);
                } leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
        public List<Assortment> obtener_assortment_by_id(int id) {
            List<Assortment> aa = new List<Assortment>();
            Conexion conna1 = new Conexion();
            try {
                SqlCommand comandoa1 = new SqlCommand();
                SqlDataReader leerFilasa1 = null;
                comandoa1.Connection = conna1.AbrirConexion();
                comandoa1.CommandText = "select PTZ.ID_SUMMARY,PTZ.ID_TALLA,PTZ.RATIO from PACKING_ASSORT PA,PACKING_TYPE_SIZE PTZ where " +
                    " PA.PACKING_NAME=PTZ.PACKING_NAME and PA.ID_PACKING_ASSORT='" + id + "'   ";
                leerFilasa1 = comandoa1.ExecuteReader();
                while (leerFilasa1.Read()) {
                    Assortment a = new Assortment();
                    a.id_summary = Convert.ToInt32(leerFilasa1["ID_SUMMARY"]);
                    a.id_talla = Convert.ToInt32(leerFilasa1["ID_TALLA"]);
                    a.ratio = Convert.ToInt32(leerFilasa1["RATIO"]);
                    aa.Add(a);
                } leerFilasa1.Close();
            } finally { conna1.CerrarConexion(); conna1.Dispose(); }
            return aa;
        }
        public string buscar_packing_name(int assort) {
            string tempo = "";
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select PACKING_NAME FROM PACKING_ASSORT WHERE ID_PACKING_ASSORT='" + assort + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToString(leer["PACKING_NAME"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }

        public string buscar_packing_name_pedido(int pedido) {
            string tempo = "";
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select PA.PACKING_NAME FROM PACKING_ASSORT PA,PACKING_TYPE_SIZE PTZ WHERE PA.PACKING_NAME=PTZ.PACKING_NAME AND PA.ID_PEDIDO='" + pedido + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    tempo = Convert.ToString(leer["PACKING_NAME"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }

        public Cantidades_Estilos obtener_cantidades_estilos(int pedido) {
            //List<Cantidades_Estilos> lista = new List<Cantidades_Estilos>();
            Cantidades_Estilos lista = new Cantidades_Estilos();
            /*List<string> assorts = buscar_assorts_pedido(pedido);
            bool isEmpty = !assorts.Any();
            if (isEmpty) {
                lista = obtener_lista_cantidades_estilos(pedido);
            } else {
                lista = obtener_lista_cantidades_estilos_assort(pedido, assorts);
            }*/
            lista.total_enviado = obtener_total_enviado_pedido(pedido);
            lista.total_pedido = obtener_total_pedido(pedido);
            return lista;
        }
        public int obtener_total_enviado_pedido(int pedido){
            int tempo = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select te.total from totales_envios te,shipping_ids si where te.id_shipping_id=si.id_shipping_id and si.id_pedido='" + pedido + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    tempo += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            tempo += buscar_cantidades_ejemplos(pedido);
            return tempo;
        }
        public int buscar_cantidades_ejemplos(int pedido){
            int tempo = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select sr.total from shipping_ratio sr,shipping_ids si where sr.id_shipping_id=si.id_shipping_id and si.id_pedido='" + pedido + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    tempo += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }

        public int obtener_total_enviado_pedido_exclusivo(int pedido,int packing){
            int tempo = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select te.total from totales_envios te,shipping_ids si where te.id_shipping_id=si.id_shipping_id and si.id_pedido='" + pedido + "' and si.used!='"+packing+"'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    tempo += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            tempo += buscar_cantidades_ejemplos_exclusivo(pedido, packing);
            return tempo;
        }
        public int buscar_cantidades_ejemplos_exclusivo(int pedido,int packing){
            int tempo = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select sr.total from shipping_ratio sr,shipping_ids si where sr.id_shipping_id=si.id_shipping_id and si.id_pedido='" + pedido + "' and si.used!='" + packing + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    tempo += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        public int obtener_total_pedido(int pedido){
            int tempo = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select TOTAL_UNITS FROM PEDIDO where id_pedido='" + pedido + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    tempo = Convert.ToInt32(leer["TOTAL_UNITS"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }




        public List<Cantidades_Estilos> obtener_lista_cantidades_estilos(int pedido) {
            List<Cantidades_Estilos> lista = new List<Cantidades_Estilos>();
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select ID_PO_SUMMARY,ITEM_ID,QTY FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Cantidades_Estilos ce = new Cantidades_Estilos();
                    ce.id_pedido = pedido;
                    ce.id_summary = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    ce.id_estilo = Convert.ToInt32(leer["ITEM_ID"]);
                    ce.cantidad_pedido = Convert.ToInt32(leer["QTY"]);
                    ce.lista_tallas = obtener_tallas_cantidades_estilo(ce.id_summary);
                    //ce.id_assort=buscar_ass
                    lista.Add(ce);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Talla> obtener_tallas_cantidades_estilo(int summary) {
            List<Talla> lista = new List<Talla>();
            int total, extras, ejemplos;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select TALLA_ITEM,CANTIDAD,EXTRAS,EJEMPLOS FROM ITEM_SIZE WHERE ID_SUMMARY='" + summary + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Talla t = new Talla();
                    t.id_talla = Convert.ToInt32(leer["TALLA_ITEM"]);
                    total = obtener_total_enviadas(summary, t.id_talla, "NONE");
                    total += obtener_total_enviadas(summary, t.id_talla, "INITIAL");
                    extras = obtener_total_enviadas(summary, t.id_talla, "EXT");
                    ejemplos = obtener_total_enviadas(summary, t.id_talla, "EXAMPLES");
                    t.talla = consultas.obtener_size_id(Convert.ToString(t.id_talla));
                    t.total = Convert.ToInt32(leer["CANTIDAD"]) - total;
                    t.extras = Convert.ToInt32(leer["EXTRAS"]) - extras;
                    t.ejemplos = Convert.ToInt32(leer["EJEMPLOS"]) - ejemplos;
                    t.ratio = consultas.obtener_ratio_summary_talla(summary, t.id_talla);
                    lista.Add(t);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int obtener_total_enviadas(int summary, int talla, string tipo) {
            int id = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT total FROM totales_envios where id_summary='" + summary + "' and id_talla='" + talla + "' and tipo='" + tipo + "' and assort=0";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id += Convert.ToInt32(leer_u_r["total"]);
                } leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }

        public List<string> buscar_assorts_pedido(int pedido) {
            List<string> lista = new List<string>();
            Conexion con01 = new Conexion();
            try {
                SqlCommand com01 = new SqlCommand();
                SqlDataReader leer01 = null;
                com01.Connection = con01.AbrirConexion();
                com01.CommandText = " select ID_PACKING_ASSORT,PACKING_NAME FROM PACKING_ASSORT WHERE ID_PEDIDO='" + pedido + "' ";
                leer01 = com01.ExecuteReader();
                while (leer01.Read()) {
                    lista.Add(Convert.ToString(leer01["PACKING_NAME"]) + "*" + Convert.ToString(leer01["ID_PACKING_ASSORT"]));
                } leer01.Close();
            } finally { con01.CerrarConexion(); con01.Dispose(); }
            return lista;
        }
        public List<Cantidades_Estilos> obtener_lista_cantidades_estilos_assort(int pedido, List<string> assorts) {
            List<Cantidades_Estilos> lista = new List<Cantidades_Estilos>();
            foreach (string a in assorts) {
                string[] datos = a.Split('*');
                Conexion con02 = new Conexion();
                try {
                    SqlCommand com02 = new SqlCommand();
                    SqlDataReader leer02 = null;
                    com02.Connection = con02.AbrirConexion();
                    com02.CommandText = " select distinct ID_SUMMARY FROM PACKING_TYPE_SIZE WHERE PACKING_NAME='" + datos[0] + "' ";
                    leer02 = com02.ExecuteReader();
                    while (leer02.Read()) {
                        Cantidades_Estilos ce = new Cantidades_Estilos();
                        ce.id_pedido = pedido;
                        ce.id_summary = Convert.ToInt32(leer02["ID_SUMMARY"]);
                        ce.id_estilo = consultas.obtener_estilo_summary(ce.id_summary);
                        ce.lista_tallas = obtener_tallas_cantidades_estilo_assort(ce.id_summary, datos[0], datos[1]);
                        ce.id_assort = Convert.ToInt32(datos[1]);
                        lista.Add(ce);
                    } leer02.Close();
                } finally { con02.CerrarConexion(); con02.Dispose(); }
            }
            return lista;
        }
        public List<Talla> obtener_tallas_cantidades_estilo_assort(int summary, string assorts, string id_assort) {
            List<Talla> lista = new List<Talla>();
            int total, extras, ejemplos;
            Conexion con03 = new Conexion();
            try {
                SqlCommand com03 = new SqlCommand();
                SqlDataReader leer03 = null;
                com03.Connection = con03.AbrirConexion();
                com03.CommandText = " select TALLA_ITEM,CANTIDAD,EXTRAS,EJEMPLOS FROM ITEM_SIZE WHERE ID_SUMMARY='" + summary + "' ";
                leer03 = com03.ExecuteReader();
                while (leer03.Read()) {
                    Talla t = new Talla();
                    t.id_talla = Convert.ToInt32(leer03["TALLA_ITEM"]);
                    total = obtener_total_enviadas_assortment(summary, t.id_talla, "NONE", id_assort);
                    total += obtener_total_enviadas_assortment(summary, t.id_talla, "INITIAL", id_assort);
                    extras = obtener_total_enviadas_assortment(summary, t.id_talla, "EXT", id_assort);
                    ejemplos = obtener_total_enviadas_assortment(summary, t.id_talla, "EXAMPLES", id_assort);
                    t.talla = consultas.obtener_size_id(Convert.ToString(t.id_talla));
                    t.total = Convert.ToInt32(leer03["CANTIDAD"]) - total;
                    t.extras = Convert.ToInt32(leer03["EXTRAS"]) - extras;
                    t.ejemplos = Convert.ToInt32(leer03["EJEMPLOS"]) - ejemplos;
                    t.ratio = consultas.obtener_ratio_summary_talla(summary, t.id_talla);
                    lista.Add(t);
                } leer03.Close();
            } finally { con03.CerrarConexion(); con03.Dispose(); }
            return lista;
        }

        public int obtener_total_enviadas_assortment(int summary, int talla, string tipo, string assort) {
            int id = 0;
            Conexion con_u_r04 = new Conexion();
            try {
                SqlCommand com_u_r04 = new SqlCommand();
                SqlDataReader leer_u_r04 = null;
                com_u_r04.Connection = con_u_r04.AbrirConexion();
                com_u_r04.CommandText = "SELECT total FROM totales_envios where id_summary='" + summary + "' and id_talla='" + talla + "' and tipo='" + tipo + "' and assort='" + assort + "' ";
                leer_u_r04 = com_u_r04.ExecuteReader();
                while (leer_u_r04.Read()) {
                    id += Convert.ToInt32(leer_u_r04["total"]);
                } leer_u_r04.Close();
            } finally { con_u_r04.CerrarConexion(); con_u_r04.Dispose(); }
            return id;
        }

        public int obtener_number_po_pedido(int pedido) {
            int id = 0;
            Conexion con_u_r04 = new Conexion();
            try {
                SqlCommand com_u_r04 = new SqlCommand();
                SqlDataReader leer_u_r04 = null;
                com_u_r04.Connection = con_u_r04.AbrirConexion();
                com_u_r04.CommandText = "SELECT VPO FROM PEDIDO WHERE ID_PEDIDO='" + pedido + "' ";
                leer_u_r04 = com_u_r04.ExecuteReader();
                while (leer_u_r04.Read()) {
                    id = Convert.ToInt32(leer_u_r04["VPO"]);
                } leer_u_r04.Close();
            } finally { con_u_r04.CerrarConexion(); con_u_r04.Dispose(); }
            return id;
        }

        public void cerrar_pedido(int pedido) {
            Conexion con_s = new Conexion();
            try {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE PEDIDO SET ID_STATUS=7 WHERE ID_PEDIDO='" + pedido + "' ";
                com_s.ExecuteNonQuery();
            } finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        /***************************************************************************************************************************************************/
        /***************************************************************************************************************************************************/
        /***************************************************************************************************************************************************/
        public List<Pk> obtener_pedido_cantidades(string inicio, string final) {
            List<Pk> lista = new List<Pk>();
            Conexion connx = new Conexion();
            try {
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select ID_PEDIDO,PO,DATE_CANCEL,DATE_ORDER from PEDIDO where " +
                    " DATE_ORDER BETWEEN '" + inicio + "' and '" + final + " 23:59:59' AND ID_STATUS!=7 AND ID_STATUS!=6";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()) {
                    Pk p = new Pk(); //Regex.Replace(color, @"\s+", " ");   
                    p.id_pedido = Convert.ToInt32(leerFilasx["ID_PEDIDO"]);
                    p.pedido = Regex.Replace(Convert.ToString(leerFilasx["PO"]), @"\s+", " ");
                    p.cancel_date = (Convert.ToDateTime(leerFilasx["DATE_CANCEL"])).ToString("MM/dd/yyyy");
                    p.lista_estilos = obtener_lista_estilos_pedido(p.id_pedido);
                    lista.Add(p);
                } leerFilasx.Close();
            } finally { connx.CerrarConexion(); connx.Dispose(); }
            return lista;
        }
        public List<estilos> obtener_lista_estilos_pedido(int pedido) {
            List<estilos> lista = new List<estilos>();
            Conexion conn = new Conexion();
            try { //Regex.Replace(color, @"\s+", " ");
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select ID_PO_SUMMARY,ITEM_ID,ID_COLOR FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()) {
                    estilos e = new estilos();
                    e.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    e.id_po_summary = Convert.ToInt32(leerFilas["ID_PO_SUMMARY"]);
                    e.id_color = Convert.ToInt32(leerFilas["ID_COLOR"]);
                    e.color = Regex.Replace(consultas.obtener_color_id((e.id_color).ToString()), @"\s+", " ");
                    e.estilo = Regex.Replace(consultas.obtener_estilo(e.id_estilo), @"\s+", " ");
                    e.descripcion = Regex.Replace(consultas.buscar_descripcion_estilo(e.id_estilo), @"\s+", " ");
                    e.descripcion_final = Regex.Replace(buscar_descripcion_final_estilo(summary), @"\s+", " ");
                    e.lista_ratio = buscar_cantidades_faltantes_estilo(e.id_po_summary);
                    lista.Add(e);
                } leerFilas.Close();
            } finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public List<ratio_tallas> buscar_cantidades_faltantes_estilo(int summary) {
            List<ratio_tallas> lista = new List<ratio_tallas>();
            int enviadas;
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select TALLA_ITEM,CANTIDAD,EXTRAS,EJEMPLOS FROM ITEM_SIZE WHERE ID_SUMMARY='" + summary + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Talla t = new Talla();
                    t.id_talla = Convert.ToInt32(leer["TALLA_ITEM"]);
                    enviadas = obtener_total_enviadas_talla(summary, t.id_talla);
                    t.total = Convert.ToInt32(leer["CANTIDAD"]) + Convert.ToInt32(leer["EXTRAS"]) + Convert.ToInt32(leer["EJEMPLOS"]);

                    ratio_tallas rt = new ratio_tallas();
                    rt.id_talla = t.id_talla;
                    rt.talla = Regex.Replace(consultas.obtener_size_id((rt.id_talla).ToString()), @"\s+", " ");
                    rt.total_talla = t.total - enviadas;//total restante de enviar por talla

                    lista.Add(rt);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int obtener_total_enviadas_talla(int summary, int talla) {
            int id = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT total FROM totales_envios where id_summary='" + summary + "' and id_talla='" + talla + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id += Convert.ToInt32(leer_u_r["total"]);
                } leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
        /***************************************************************************************************************************************************/
        public List<Estilo_Pedido> obtener_estilos_pedido_status(int pedido) {
            List<Estilo_Pedido> lista = new List<Estilo_Pedido>();
            Conexion con = new Conexion();
            try {//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "select ID_PO_SUMMARY,ITEM_ID,ID_COLOR FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Estilo_Pedido e = new Estilo_Pedido();
                    e.id_estilo = Convert.ToInt32(leer["ITEM_ID"]);
                    e.id_summary = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    e.id_color = Convert.ToInt32(leer["ID_COLOR"]);
                    e.color = Regex.Replace(consultas.obtener_color_id((e.id_color).ToString()), @"\s+", " ");
                    e.estilo = Regex.Replace(consultas.obtener_estilo(e.id_estilo), @"\s+", " ");
                    e.descripcion = Regex.Replace(consultas.buscar_descripcion_estilo(e.id_estilo), @"\s+", " ");
                    e.totales_pedido = buscar_totales_summary(e.id_summary);
                    e.lista_pk = obtener_pk_estilos(e.id_summary);
                    lista.Add(e);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Talla> buscar_totales_summary(int summary) {
            List<Talla> lista = new List<Talla>();
            Conexion con = new Conexion();
            try {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select TALLA_ITEM,CANTIDAD,EXTRAS,EJEMPLOS FROM ITEM_SIZE WHERE ID_SUMMARY='" + summary + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Talla t = new Talla();
                    t.id_talla = Convert.ToInt32(leer["TALLA_ITEM"]);
                    t.talla = Regex.Replace(consultas.obtener_size_id((t.id_talla).ToString()), @"\s+", " ");
                    t.total = Convert.ToInt32(leer["CANTIDAD"]) + Convert.ToInt32(leer["EJEMPLOS"]) + Convert.ToInt32(leer["EXTRAS"]);
                    lista.Add(t);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Packing_Estilo> obtener_pk_estilos(int summary) {
            List<Packing_Estilo> lista = new List<Packing_Estilo>();
            Conexion con = new Conexion();
            try
            {//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_shipping_id,used,tipo from shipping_ids where  id_po_summary='" + summary + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Packing_Estilo pe = new Packing_Estilo();
                    pe.id_shipping = Convert.ToInt32(leer["id_shipping_id"]);
                    pe.id_packing = Convert.ToInt32(leer["used"]);
                    pe.tipo = Convert.ToString(leer["tipo"]);
                    pe.package = obtener_clave_packing(pe.id_packing);
                    pe.fecha = obtener_fecha_packing(pe.id_packing);
                    pe.lista_enviados = obtener_cantidades_enviado(pe.id_shipping, pe.tipo);
                    lista.Add(pe);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public string obtener_clave_packing(int packing) {
            string lista = "";
            Conexion con = new Conexion();
            try {//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT pk from packing_list where id_packing_list='" + packing + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    lista = Convert.ToString(leer["pk"]);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public string obtener_fecha_packing(int packing) {
            string lista = "";
            Conexion con = new Conexion();
            try {//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fecha from packing_list where id_packing_list='" + packing + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    lista = (Convert.ToDateTime(leer["fecha"])).ToString("MM/dd/yyyy");
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Talla> obtener_cantidades_enviado(int shipping, string tipo) {
            List<Talla> lista = new List<Talla>();
            Conexion con = new Conexion();
            try {//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_talla,total from totales_envios where id_shipping_id='" + shipping + "' and tipo='" + tipo + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Talla t = new Talla();
                    t.total = Convert.ToInt32(leer["total"]);
                    t.id_talla = Convert.ToInt32(leer["id_talla"]);
                    lista.Add(t);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        /***************************************************************************************************************************************************/
        public List<Estilo_PO> obtener_pedidos_po_estilo(int estilo) {
            List<Estilo_PO> lista = new List<Estilo_PO>();
            Conexion con = new Conexion();
            try {//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_PO_SUMMARY,ID_COLOR,QTY,ID_PEDIDOS FROM PO_SUMMARY WHERE ITEM_ID='" + estilo + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()) {
                    Estilo_PO epo = new Estilo_PO();
                    //e.id_shipping = Convert.ToInt32(leer["id_shipping_id"]);
                    epo.id_pedido = Convert.ToInt32(leer["ID_PEDIDOS"]);
                    epo.pedido = Regex.Replace(consultas.obtener_po_id((epo.id_pedido).ToString()), @"\s+", " ");
                    epo.id_summary = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                    epo.id_color = Convert.ToInt32(leer["ID_COLOR"]);
                    epo.total = Convert.ToInt32(leer["QTY"]);
                    epo.color = Regex.Replace(consultas.obtener_color_id((epo.id_color).ToString()), @"\s+", " ");
                    epo.estilo = Regex.Replace(consultas.obtener_estilo(estilo), @"\s+", " ");
                    epo.descripcion = Regex.Replace(consultas.buscar_descripcion_estilo(estilo), @"\s+", " ");
                    int enviadas = buscar_totales_enviadas_summary(epo.id_summary);
                    if (enviadas >= epo.total) {
                        epo.estado = "COMPLETE";
                    } else {
                        epo.estado = "INCOMPLETE";
                    }
                    epo.total = epo.total - enviadas;
                    lista.Add(epo);
                } leer.Close();
            } finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int buscar_totales_enviadas_summary(int summary) {
            int id = 0;
            Conexion con_u_r = new Conexion();
            try {
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT total FROM totales_envios where id_summary='" + summary + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()) {
                    id += Convert.ToInt32(leer_u_r["total"]);
                } leer_u_r.Close();
            } finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
        /***************************************************************************************************************************************************/
        public List<Shipping_pk> obtener_listado_packing_year(string year) {
            List<Shipping_pk> lista = new List<Shipping_pk>();
            Conexion con = new Conexion();
            try{//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_packing_list,pk,id_direccion_envio,id_pedido,fecha,id_packing_type,envio  FROM packing_list  WHERE year(fecha)='" + year + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Shipping_pk s = new Shipping_pk();
                    s.id_packing= Convert.ToInt32(leer["id_packing_list"]);
                    s.id_pedido= Convert.ToInt32(leer["id_pedido"]);
                    string[] t = (Convert.ToString(leer["pk"])).Split('-');
                    s.packing= t[0];
                    s.fecha= (Convert.ToDateTime(leer["fecha"])).ToString("MM/dd/yyyy");
                    s.num_envio = Convert.ToInt32(leer["envio"]);
                    s.pedido= Regex.Replace(consultas.obtener_po_id((s.id_pedido).ToString()), @"\s+", " ");
                    Direccion d = obtener_direccion(Convert.ToInt32(leer["id_direccion_envio"]));
                    s.destino = d.nombre;
                    int tipo_packing= Convert.ToInt32(leer["id_packing_type"]);
                    if (tipo_packing == 1) {
                        string[] t2 = (obtener_cantidades_piezas_packing_bullpack(s.id_packing)).Split('*');
                        s.piezas = Convert.ToInt32(t2[0]);
                        s.cajas = Convert.ToInt32(t2[1]);
                    }
                    if (tipo_packing == 2) {
                        string[] t2 = (obtener_cantidades_piezas_packing_ppk(s.id_packing)).Split('*');
                        s.piezas = Convert.ToInt32(t2[0]);
                        s.cajas = Convert.ToInt32(t2[1]);
                    }
                    if (tipo_packing == 2){
                        string[] t2 = (obtener_cantidades_piezas_packing_assort(s.id_packing)).Split('*');
                        s.piezas = Convert.ToInt32(t2[0]);
                        s.cajas = Convert.ToInt32(t2[1]);
                    }
                    s.pallets = obtener_numero_pallets(s.id_packing);
                    lista.Add(s);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int obtener_numero_pallets(int packing){
            int total = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT distinct id_tarima FROM shipping_ids where used='" + packing + "' and id_tarima!=0";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    total++;
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return total;
        }
        public string obtener_cantidades_piezas_packing_bullpack(int packing){
            int total = 0;
            int cajas = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT cantidad,id_po_summary,id_talla,tipo FROM shipping_ids where used='" + packing + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    if (Convert.ToString(leer_u_r["tipo"]) == "NONE" || Convert.ToString(leer_u_r["tipo"]) == "INITIAL"){
                        int cantidad = Convert.ToInt32(leer_u_r["cantidad"]);
                        total += cantidad;
                        int piezas_caja = buscar_cajas_talla_estilo(Convert.ToInt32(leer_u_r["id_po_summary"]), Convert.ToInt32(leer_u_r["id_talla"]));
                        cajas += (cantidad / piezas_caja);
                    }else {
                        total++;
                        cajas++;
                    }                    
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return (total.ToString()+"*"+cajas.ToString());
        }
        public string obtener_cantidades_piezas_packing_ppk(int packing){
            int total = 0;
            int cajas = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT cantidad,id_po_summary,tipo FROM shipping_ids where used='" + packing + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    if (Convert.ToString(leer_u_r["tipo"]) == "NONE" || Convert.ToString(leer_u_r["tipo"]) == "INITIAL"){
                        cajas += Convert.ToInt32(leer_u_r["cantidad"]);
                        total += (buscar_piezas_ratio(Convert.ToInt32(leer_u_r["id_po_summary"]))) * Convert.ToInt32(leer_u_r["cantidad"]);
                    }else {
                        cajas++;
                        total++;
                    }
                                       
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return (total.ToString() + "*" + cajas.ToString());
        }
        public int buscar_piezas_ratio(int summary){
            int id = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT RATIO FROM PACKING_TYPE_SIZE where ID_SUMMARY='" + summary + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    id += Convert.ToInt32(leer_u_r["RATIO"]);
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
        public string obtener_cantidades_piezas_packing_assort(int packing){
            int total = 0;
            int cajas = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT cantidad,id_po_summary,id_estilo,tipo FROM shipping_ids where used='" + packing + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    /*a.nombre = buscar_nombre_assort(id);
                    a.cartones = cantidad;
                    e.lista_ratio = obtener_lista_ratio_assort(a.id_summary, e.id_estilo, packing_name);*/
                    if (Convert.ToString(leer_u_r["tipo"]) == "NONE" || Convert.ToString(leer_u_r["tipo"]) == "INITIAL"){
                        string nombre = buscar_packing_name_assort(Convert.ToInt32(leer_u_r["id_estilo"]));
                        cajas += Convert.ToInt32(leer_u_r["cantidad"]);
                        total += (buscar_piezas_ratio_assort(Convert.ToInt32(leer_u_r["id_po_summary"]), nombre)) * Convert.ToInt32(leer_u_r["cantidad"]);
                    }else {
                        cajas++;
                        total++;
                    }
                        
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return (total.ToString() + "*" + cajas.ToString());
        }
        public int buscar_piezas_ratio_assort(int summary,string name){
            int id = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT RATIO FROM PACKING_TYPE_SIZE where ID_SUMMARY='" + summary + "' and  PACKING_NAME='"+name+"' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    id += Convert.ToInt32(leer_u_r["RATIO"]);
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
        public string buscar_packing_name_assort(int assort){
            string tempo = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " select PACKING_NAME FROM PACKING_ASSORT  WHERE ID_PACKING_ASSORT='" + assort + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    tempo = Convert.ToString(leer["PACKING_NAME"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return tempo;
        }
        /***************************************************************************************************************************************************/
        public List<Shipping_pk> obtener_listado_packing_diario(){
            List<Shipping_pk> lista = new List<Shipping_pk>();
            Conexion con = new Conexion();
            try{//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_packing_list,pk,id_direccion_envio,id_pedido,fecha,id_packing_type,envio FROM packing_list "+
                    " WHERE fecha between '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00' and '"+ DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Shipping_pk s = new Shipping_pk();
                    s.id_packing = Convert.ToInt32(leer["id_packing_list"]);
                    s.id_pedido = Convert.ToInt32(leer["id_pedido"]);
                    string[] t = (Convert.ToString(leer["pk"])).Split('-');
                    s.packing = t[0];
                    s.fecha = (Convert.ToDateTime(leer["fecha"])).ToString("MM/dd/yyyy");
                    s.num_envio = Convert.ToInt32(leer["envio"]);
                    s.pedido = Regex.Replace(consultas.obtener_po_id((s.id_pedido).ToString()), @"\s+", " ");
                    Direccion d = obtener_direccion(Convert.ToInt32(leer["id_direccion_envio"]));
                    s.destino = d.nombre;
                    int tipo_packing = Convert.ToInt32(leer["id_packing_type"]);
                    if (tipo_packing == 1){
                        string[] t2 = (obtener_cantidades_piezas_packing_bullpack(s.id_packing)).Split('*');
                        s.piezas = Convert.ToInt32(t2[0]);
                        s.cajas = Convert.ToInt32(t2[1]);
                    }
                    if (tipo_packing == 2){
                        string[] t2 = (obtener_cantidades_piezas_packing_ppk(s.id_packing)).Split('*');
                        s.piezas = Convert.ToInt32(t2[0]);
                        s.cajas = Convert.ToInt32(t2[1]);
                    }
                    if (tipo_packing == 3){
                        string[] t2 = (obtener_cantidades_piezas_packing_assort(s.id_packing)).Split('*');
                        s.piezas = Convert.ToInt32(t2[0]);
                        s.cajas = Convert.ToInt32(t2[1]);
                    }
                    s.pallets = obtener_numero_pallets(s.id_packing);
                    lista.Add(s);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        /***************************************************************************************************************************************************/
        public List<Pk> obtener_pedido_cantidades_orden(int pedido){
            List<Pk> lista = new List<Pk>();
            Conexion connx = new Conexion();
            try{
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select ID_PEDIDO,PO,DATE_CANCEL,DATE_ORDER from PEDIDO where ID_PEDIDO='" + pedido + "'   AND ID_STATUS!=7 AND ID_STATUS!=6";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()){
                    Pk p = new Pk(); //Regex.Replace(color, @"\s+", " ");   
                    p.id_pedido = Convert.ToInt32(leerFilasx["ID_PEDIDO"]);
                    p.pedido = Regex.Replace(Convert.ToString(leerFilasx["PO"]), @"\s+", " ");
                    p.cancel_date = (Convert.ToDateTime(leerFilasx["DATE_CANCEL"])).ToString("MM/dd/yyyy");
                    p.lista_estilos = obtener_lista_estilos_pedido(p.id_pedido);
                    p.total_po = buscar_totales(pedido);
                    p.total_enviado = buscar_totales_enviados(pedido);
                    lista.Add(p);
                }leerFilasx.Close();
            }finally { connx.CerrarConexion(); connx.Dispose(); }
            return lista;
        }
        public int buscar_totales(int pedido){
            int total = 0;            
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT ID_PO_SUMMARY,QTY FROM PO_SUMMARY where ID_PEDIDOS='" + pedido + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    total += Convert.ToInt32(leer_u_r["QTY"]);
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return total;
        }
        public int buscar_totales_enviados(int pedido){
            int enviado = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT ID_PO_SUMMARY,QTY FROM PO_SUMMARY where ID_PEDIDOS='" + pedido + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    enviado += buscar_totales_enviadas_summary(Convert.ToInt32(leer_u_r["ID_PO_SUMMARY"]));
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return enviado;
        }
        /***************************************************************************************************************************************************/
        public List<Po> obtener_lista_pedidos() {
            List<Po> lista = new List<Po>();
            Conexion con = new Conexion();
            try{//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ID_PEDIDO,PO,CUSTOMER,CUSTOMER_FINAL,DATE_CANCEL,TOTAL_UNITS,ID_STATUS FROM PEDIDO ORDER BY ID_PEDIDO ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Po p = new Po();
                    p.id_pedido = Convert.ToInt32(leer["ID_PEDIDO"]);
                    p.pedido= Regex.Replace(Convert.ToString(leer["PO"]), @"\s+", " ");
                    p.customer = Regex.Replace(consultas.obtener_customer_id(Convert.ToString(leer["CUSTOMER"])), @"\s+", " ");
                    p.customer_po = Regex.Replace(consultas.obtener_customer_final_id(Convert.ToString(leer["CUSTOMER_FINAL"])), @"\s+", " ");
                    p.fecha_cancelacion= (Convert.ToDateTime(leer["DATE_CANCEL"])).ToString("MM/dd/yyyy");
                    p.total= Convert.ToInt32(leer["TOTAL_UNITS"]);
                    p.estilos = buscar_total_estilos_pedido(p.id_pedido);
                    if (Convert.ToInt32(leer["ID_STATUS"]) == 6) { p.estado = "CANCELLED"; }
                    else {
                        if (Convert.ToInt32(leer["ID_STATUS"]) == 7) { p.estado = "COMPLETED"; }
                        else { p.estado = "INCOMPLETE"; }
                    }
                    lista.Add(p);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public int buscar_total_estilos_pedido(int pedido){
            int total = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY where ID_PEDIDOS='" + pedido + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    total++;
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return total;
        }
        /***************************************************************************************************************************************************/
        /***************************************************************************************************************************************************/
        /***************************************************************************************************************************************************/

        public void eliminar_inventario_pedido(int pedido) {
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario FROM inventario WHERE id_pedido='"+pedido+"' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    vaciar_cajas_inventario(Convert.ToInt32(leer["id_inventario"]));
                    vaciar_inventario(Convert.ToInt32(leer["id_inventario"]));
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
        }
        public void vaciar_cajas_inventario(int item){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE cajas_inventario SET cantidad_restante=0 where id_inventario='"+item+"' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void vaciar_inventario(int item){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total=0 where id_inventario='" + item + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public List<Talla> obtener_lista_tallas_pedido(List<estilo_shipping> estilos)
        {
            List<Talla> lista = new List<Talla>();
            foreach (estilo_shipping e in estilos) {
                Conexion con = new Conexion();
                try{
                    SqlCommand com = new SqlCommand();
                    SqlDataReader leer = null;
                    com.Connection = con.AbrirConexion();
                    com.CommandText = "SELECT TALLA_ITEM FROM ITEM_SIZE WHERE ID_SUMMARY='" + e.id_summary + "' ";
                    leer = com.ExecuteReader();
                    while (leer.Read()){
                        Talla t = new Talla();
                        t.id_talla = Convert.ToInt32(leer["TALLA_ITEM"]);
                        t.talla = consultas.obtener_size_id(Convert.ToString(leer["TALLA_ITEM"]));
                        t.total = 0; t.ratio = 0;
                        bool isEmpty = !lista.Any();
                        if (isEmpty){
                            lista.Add(t);
                        }else{
                            int existe = 0;
                            foreach (Talla size in lista) {
                                if (size.id_talla == t.id_talla) {
                                    existe++;
                                }
                            }
                            if (existe == 0) {
                                lista.Add(t);
                            }
                        }
                    }
                    leer.Close();
                }finally { con.CerrarConexion(); con.Dispose(); }
            }            
            return lista;
        }

        public List<Pk> obtener_packing_list_bol(int pk)
        {
            List<Pk> lista = new List<Pk>();
            Conexion connx = new Conexion();
            try{
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select id_packing_list,pk,id_customer,id_customer_po,id_direccion_envio,id_pedido,id_driver,id_container,shipping_manager,seal,replacement,fecha,tipo,id_packing_type from packing_list where id_packing_list='" + pk + "' ";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()){
                    Pk p = new Pk(); //Regex.Replace(color, @"\s+", " ");
                    p.id_packing_list = Convert.ToInt32(leerFilasx["id_packing_list"]);
                    p.packing = Convert.ToString(leerFilasx["pk"]);
                    p.customer = Regex.Replace(consultas.obtener_customer_id(Convert.ToString(leerFilasx["id_customer"])), @"\s+", " ");
                    p.customer_po = Regex.Replace(consultas.obtener_customer_final_id(Convert.ToString(leerFilasx["id_customer_po"])), @"\s+", " ");
                    p.destino = obtener_direccion(Convert.ToInt32(leerFilasx["id_direccion_envio"]));
                    p.pedido = consultas.obtener_po_id(Convert.ToString(leerFilasx["id_pedido"]));
                    id_pedido = Convert.ToInt32(leerFilasx["id_pedido"]);
                    p.conductor = obtener_driver(Convert.ToInt32(leerFilasx["id_driver"]));
                    p.contenedor = obtener_contenedor(Convert.ToInt32(leerFilasx["id_container"]));
                    p.shipping_manager = Convert.ToString(leerFilasx["shipping_manager"]);
                    p.seal = Convert.ToString(leerFilasx["seal"]);
                    p.replacement = Convert.ToString(leerFilasx["replacement"]);
                    p.fecha = (Convert.ToDateTime(leerFilasx["fecha"])).ToString("MM/dd/yyyy");
                    //p.dc = buscar_dc_pk(p.id_packing_list);
                    p.parte = buscar_pks_pedido(id_pedido, p.id_packing_list);
                    /*p.tipo_empaque = Convert.ToInt32(leerFilasx["id_packing_type"]);
                    if (p.tipo_empaque != 3)
                    {
                        p.lista_tarimas = obtener_tarimas(p.id_packing_list, p.tipo_empaque);
                    }
                    else
                    {
                        p.lista_tarimas = obtener_tarimas_assort(p.id_packing_list, p.tipo_empaque);
                    }
                    p.number_po = buscar_number_po(id_pedido);
                    p.tipo = leerFilasx["tipo"].ToString();
                    p.tipo_empaque = Convert.ToInt32(leerFilasx["id_packing_type"]);*/

                    p.total_tarimas = obtener_total_tarimas_pk(pk);
                    p.total_cajas = obtener_total_cajas_pk(pk);
                    p.total_piezas = obtener_total_piezas_pk(pk) + obtener_total_piezas_pk_extras(pk);
                    lista.Add(p);
                }
                leerFilasx.Close();
            }finally { connx.CerrarConexion(); connx.Dispose(); }
            return lista;
        }
        public int obtener_total_tarimas_pk(int packing){
            int total = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT DISTINCT id_tarima FROM shipping_ids WHERE used='" + packing + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    total++;
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return total;
        }
        public int obtener_total_piezas_pk(int packing){
            int total = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "(SELECT te.total from totales_envios te,shipping_ids si where si.used='" + packing + "' and si.id_shipping_id=te.id_shipping_id)";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    total+=Convert.ToInt32(leer_u_r["total"]);
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return total;
        }

        public int obtener_total_piezas_pk_extras(int packing){
            int total = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT sr.total from shipping_ratio sr,shipping_ids si where si.used='" + packing + "' and si.id_shipping_id=sr.id_shipping_id";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    total += Convert.ToInt32(leer_u_r["total"]);
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return total;
        }

        public int obtener_total_cajas_pk(int packing){
            int total = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT cantidad,tipo_empaque,id_po_summary,id_talla,id_pedido,tipo FROM shipping_ids WHERE used='" + packing + "' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    int tipo_empaque = Convert.ToInt32(leer_u_r["tipo_empaque"]);
                    int cantidad = Convert.ToInt32(leer_u_r["cantidad"]);
                    string tipo = Convert.ToString(leer_u_r["tipo"]);
                    if (tipo_empaque == 1){
                        if (tipo == "DMG" || tipo == "EXT" || tipo == "EXAMPLES"){
                            total++;
                        }else {
                            int ratio = buscar_piezas_empaque_bull(Convert.ToInt32(leer_u_r["id_po_summary"]), Convert.ToInt32(leer_u_r["id_talla"]));
                            total += (cantidad / ratio);
                        }
                    }
                    if(tipo_empaque==2){
                        total += cantidad;
                    }
                    if (tipo_empaque == 3) {
                        /* Assortment a = assortment_id(Convert.ToInt32(leer_u_r["id_po_summary"]), Convert.ToInt32(leer_u_r["id_pedido"]));
                         foreach (estilos e in a.lista_estilos) {
                             total += cantidad;
                         }*/
                        total += cantidad;
                    }
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return total;
        }
        public List<Container> obtener_contenedores(){
            List<Container> lista = new List<Container>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_container,eco,plates from containers ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Container e = new Container();
                    e.id_container = Convert.ToInt32(leerFilas["id_container"]);
                    e.eco = Convert.ToString(leerFilas["eco"]);
                    e.plates = Convert.ToString(leerFilas["plates"]);
                    lista.Add(e);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }

        public List<Container> obtener_contenedor_edicion(string id){
            List<Container> lista = new List<Container>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_container,eco,plates from containers where id_container='"+id+"' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Container e = new Container();
                    e.id_container = Convert.ToInt32(leerFilas["id_container"]);
                    e.eco = Convert.ToString(leerFilas["eco"]);
                    e.plates = Convert.ToString(leerFilas["plates"]);
                    lista.Add(e);
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public void borrar_contenedor(string id){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "DELETE FROM containers where id_container='" + id + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public void guardar_nuevo_contenedor(string eco, string plates){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO containers(eco,plates)values" +
                    "('" + eco + "','" + plates + "') ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void guardar_contenedor_edicion(string id, string eco, string plates){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE containers SET eco='" + eco + "', plates='" + plates + "' " +
                    "  where id_container='" + id + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public Pk obtener_informacion_editar_packing_completo_b(int pk){
            Pk p = new Pk();
            Conexion connx = new Conexion();
            try{
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select id_packing_list,id_direccion_envio,id_pedido,id_driver,id_container,seal,replacement,tipo,envio " +
                    "from packing_list where id_packing_list='" + pk + "' ";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()){
                    p.id_packing_list = Convert.ToInt32(leerFilasx["id_packing_list"]);
                    p.id_direccion_envio = Convert.ToInt32(leerFilasx["id_direccion_envio"]);
                    p.id_pedido= Convert.ToInt32(leerFilasx["id_pedido"]);                   
                    p.id_driver = Convert.ToInt32(leerFilasx["id_driver"]);
                    p.id_container = Convert.ToInt32(leerFilasx["id_container"]);
                    p.seal = Convert.ToString(leerFilasx["seal"]);
                    p.replacement = Convert.ToString(leerFilasx["replacement"]);
                    p.tipo = Convert.ToString(leerFilasx["id_driver"]);
                    p.num_envio = Convert.ToInt32(leerFilasx["envio"]);
                    p.lista_labels = obtener_lista_labels(pk);
                }
                leerFilasx.Close();
            }finally { connx.CerrarConexion(); connx.Dispose(); }
            return p;
        }
        public List<Pk> obtener_informacion_editar_packing_completo(int pk){
            List<Pk> lista = new List<Pk>();
            
            Conexion connx = new Conexion();
            try{
                SqlCommand comandox = new SqlCommand();
                SqlDataReader leerFilasx = null;
                comandox.Connection = connx.AbrirConexion();
                comandox.CommandText = "select id_packing_list,id_direccion_envio,id_pedido,id_driver,id_container,seal,replacement,tipo,envio " +
                    "from packing_list where id_packing_list='" + pk + "' ";
                leerFilasx = comandox.ExecuteReader();
                while (leerFilasx.Read()){
                    Pk p = new Pk();
                    p.id_packing_list = Convert.ToInt32(leerFilasx["id_packing_list"]);
                    p.id_direccion_envio = Convert.ToInt32(leerFilasx["id_direccion_envio"]);
                    p.id_pedido = Convert.ToInt32(leerFilasx["id_pedido"]);
                    p.id_driver = Convert.ToInt32(leerFilasx["id_driver"]);
                    p.id_container = Convert.ToInt32(leerFilasx["id_container"]);
                    p.seal = Convert.ToString(leerFilasx["seal"]);
                    p.replacement = Convert.ToString(leerFilasx["replacement"]);
                    p.tipo = Convert.ToString(leerFilasx["id_driver"]);
                    p.num_envio = Convert.ToInt32(leerFilasx["envio"]);
                    p.lista_labels = obtener_lista_labels(pk);
                    lista.Add(p);
                }leerFilasx.Close();
            }finally { connx.CerrarConexion(); connx.Dispose(); }
            return lista;
        }

        public List<Labels> obtener_lista_labels(int pk){
            List<Labels> lista = new List<Labels>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leer = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select id_label,label,tipo from ucc_labels where id_packing='" + pk + "' ";
                leer= comando.ExecuteReader();
                while (leer.Read()){
                    Labels l = new Labels();
                    l.id_label = Convert.ToInt32(leer["id_label"]);
                    l.label = Convert.ToString(leer["label"]);
                    l.tipo = Convert.ToString(leer["tipo"]);
                    lista.Add(l);
                }leer.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }
        public int obtener_pedido_packink(int packing){
            int lista = 0;
            Conexion con = new Conexion();
            try{//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_pedido from packing_list where id_packing_list='" + packing + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    lista = Convert.ToInt32(leer["id_pedido"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public void eliminar_estilos_packing_list(int packing){
            List<int> ids = lista_shipping_ids_packing(packing);
            foreach (int i in ids) {
                eliminar_totales_envios(i);
                eliminar_totales_ejemplos(i);
                eliminar_estilo_shipping_id(i);
            }
            
        }
        public List<int> lista_shipping_ids_packing(int packing) {
            List<int> lista = new List<int>();
            Conexion con = new Conexion();
            try{//Regex.Replace(, @"\s+", " ");
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_shipping_id from shipping_ids where used='" + packing + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    int i = 0;
                    i = Convert.ToInt32(leer["id_shipping_id"]);
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public void eliminar_totales_envios(int id_shipping){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "delete from totales_envios  where id_shipping_id='" + id_shipping + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void eliminar_totales_ejemplos(int id_shipping){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "delete from shipping_ratio where id_shipping_id='" + id_shipping + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void eliminar_estilo_shipping_id(int id_shipping){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "delete from shipping_ids where id_shipping_id='" + id_shipping + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public List<estilo_shipping> lista_estilos_extras(string id_pedido, string busqueda)
        {
            string cadena = "";
            if (busqueda == "0")
            {
                cadena = "select top 10 ID_PO_SUMMARY,ID_PEDIDOS,ID_COLOR,ITEM_ID from PO_SUMMARY where ID_PEDIDOS='" + id_pedido + "' ";
            }else{
                cadena = "select top 10 PS.ID_PO_SUMMARY,PS.ID_PEDIDOS,PS.ID_COLOR,PS.ITEM_ID FROM PO_SUMMARY PS,ITEM_DESCRIPTION ITD,PEDIDO P WHERE" +
                    " ITD.ITEM_STYLE LIKE '%" + busqueda + "%' AND ITD.ITEM_ID=PS.ITEM_ID AND P.ID_STATUS!=7 AND P.ID_STATUS!=6" +
                    " AND P.ID_PEDIDO=PS.ID_PEDIDOS  ";
            }
            List<estilo_shipping> listar = new List<estilo_shipping>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = cadena;
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    estilo_shipping l = new estilo_shipping();
                    /*l.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    l.id_summary = consultas.obtener_po_summary(Convert.ToInt32(id_pedido), l.id_estilo);
                    l.id_color = consultas.obtener_color_id_item(Convert.ToInt32(id_pedido), l.id_estilo);*/
                    l.id_summary = Convert.ToInt32(leerFilas["ID_PO_SUMMARY"]);
                    l.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    l.id_color = Convert.ToInt32(leerFilas["ID_COLOR"]);
                    l.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDOS"]);
                    l.po = consultas.obtener_po_id(Convert.ToString(l.id_pedido));
                    l.color = consultas.obtener_color_id(Convert.ToString(l.id_color));
                    l.estilo = consultas.obtener_estilo(l.id_estilo);
                    l.descripcion = consultas.buscar_descripcion_estilo(l.id_estilo);
                    List<Empaque> lista_e = new List<Empaque>();
                    List<string> tipo_empaque_temporal = consultas.buscar_tipo_empaque(l.id_summary);
                    foreach (string s in tipo_empaque_temporal){
                        Empaque e = new Empaque();
                        e.tipo_empaque = Convert.ToInt32(s);
                        if (s == "1") { e.lista_ratio = obtener_lista_tallas_estilo(l.id_summary, l.id_estilo); }
                        if (s == "2") { e.lista_ratio = obtener_lista_ratio(l.id_summary, l.id_estilo, 2); }
                        lista_e.Add(e);
                    }
                    l.lista_empaque = lista_e;
                    listar.Add(l);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return listar;
        }
        


        public void guardar_ratio_otros(int id, string cantidad, string talla)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO shipping_ratio(id_talla,total,id_shipping_id) VALUES " +
                                     " ('" + talla + "','" + cantidad + "','" + id + "')";
                com_s.ExecuteNonQuery();
            }
            finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }




































































    }
}