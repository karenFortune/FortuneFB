using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Shipping;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FortuneSystem.Models.Trims{
    public class DatosTrim{
        DatosInventario di = new DatosInventario();
        DatosReportes dr = new DatosReportes();
        DatosShipping ds = new DatosShipping();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        public string query = "";

        public List<salidas> obtener_lista_recibos_trim(string busqueda) {
            if (busqueda != ""){
                string[] fecha = busqueda.Split('*');
                query = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_recibo,driver,pallet from salidas "+
                    " where fecha_recibo between '"+fecha[0]+" 00:00:00' and '"+fecha[1]+" 23:59:59' ";
            }else {
                query = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_recibo,driver,pallet from salidas ";
            }
            List<salidas> lista = new List<salidas>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = query;
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    int trim=buscar_trims_salida(Convert.ToInt32(leer_ltf["id_salida"]));
                    if (trim == 1) {
                        salidas l = new salidas();
                        l.id_salida = Convert.ToInt32(leer_ltf["id_salida"]);
                        l.fecha = (Convert.ToDateTime(leer_ltf["fecha"])).ToString("MMM dd yyyy");
                        l.id_usuario = Convert.ToInt32(leer_ltf["id_usuario"]);
                        l.id_origen = Convert.ToInt32(leer_ltf["id_origen"]);
                        l.id_destino = Convert.ToInt32(leer_ltf["id_destino"]);
                        l.estado_aprobacion = Convert.ToInt32(leer_ltf["estado_aprobacion"]);
                        l.estado_entrega = Convert.ToInt32(leer_ltf["estado_entrega"]);
                        l.sello = Convert.ToInt32(leer_ltf["sello"]);
                        l.responsable = Convert.ToString(leer_ltf["responsable"]);
                        l.id_envio = leer_ltf["id_envio"].ToString();
                        l.fecha_solicitud = (Convert.ToDateTime(leer_ltf["fecha_solicitud"])).ToString("MM-dd-yyyy");
                        l.driver = Convert.ToString(leer_ltf["driver"]);
                        l.pallet = Convert.ToString(leer_ltf["pallet"]);
                        l.total = Convert.ToInt32(leer_ltf["total"]);
                        l.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer_ltf["id_usuario"]));
                        l.origen = consultas.buscar_nombres_lugares(Convert.ToString(leer_ltf["id_origen"]));
                        l.destino = consultas.buscar_nombres_lugares(Convert.ToString(leer_ltf["id_destino"]));
                        l.lista_salidas_item = obtener_lista_items(l.id_salida);
                        lista.Add(l);
                    }
                }leer_ltf.Close();
            }finally{con_ltf.CerrarConexion(); con_ltf.Dispose();}
            return lista;
        }
        public int buscar_trims_salida(int salida) {
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT i.id_categoria_inventario from inventario i,salidas_items s where s.id_salida='" + salida + "' and s.id_inventario=i.id_inventario ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    if (Convert.ToInt32(leer_ltd["id_categoria_inventario"]) == 2) { trim++; }
                }leer_ltd.Close();
            }finally{con_ltd.CerrarConexion(); con_ltd.Dispose();}
            return trim;
        }
        public List<salidas_item> obtener_lista_items(int salida){
            List<salidas_item> lista = new List<salidas_item>();
            Conexion connn = new Conexion();
            try{
                SqlCommand commm = new SqlCommand();
                SqlDataReader leerrr = null;
                commm.Connection = connn.AbrirConexion();
                commm.CommandText = "SELECT s.id_inventario,s.cantidad,i.mill_po,i.descripcion,s.id_inventario,s.id_pedido,s.id_estilo,s.codigo,s.id_salida_item from salidas_items s,inventario i where s.id_salida='" + salida + "' and s.id_inventario=i.id_inventario and i.id_categoria_inventario=2";
                leerrr = commm.ExecuteReader();
                while (leerrr.Read()){
                    salidas_item l = new salidas_item();
                    l.id_salida_item= Convert.ToInt32(leerrr["id_salida_item"]);
                    l.id_inventario = Convert.ToInt32(leerrr["id_inventario"]);
                    l.cantidad = Convert.ToInt32(leerrr["cantidad"]);
                    l.descripcion = consultas.buscar_descripcion_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.po = consultas.buscar_po_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.estilo = consultas.obtener_estilo(Convert.ToInt32(leerrr["id_inventario"]));
                    l.summary = consultas.obtener_po_summary(Convert.ToInt32(leerrr["id_pedido"]), Convert.ToInt32(leerrr["id_estilo"]));
                    l.po_number = buscar_po_number_summary(l.summary);
                    l.codigo = Convert.ToString(leerrr["codigo"]);
                    l.mp_number = buscar_mp_number_inventario(Convert.ToInt32(leerrr["id_inventario"]));
                    lista.Add(l);
                }leerrr.Close();
            }finally { connn.CerrarConexion(); connn.Dispose(); }
            return lista;
        }
        public string buscar_mp_number_inventario(int inventario){
            string trim = "0";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT r.mp_number from recibos r,recibos_items ri where r.id_recibo=ri.id_recibo and ri.id_inventario='" + inventario + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["mp_number"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public string buscar_po_number_summary(int summary){
            string trim = "0";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT NUMBER_PO FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + summary + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["NUMBER_PO"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public List<Pedido_t> lista_ordenes(){
            List<Pedido_t> listar = new List<Pedido_t>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT ID_PEDIDO,PO from PEDIDO where ID_STATUS!=7 AND ID_STATUS!=6 ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Pedido_t l = new Pedido_t();
                    l.id_pedido = Convert.ToInt32(leerFilas["ID_PEDIDO"]);
                    l.pedido = leerFilas["PO"].ToString();
                    listar.Add(l);
                }
                leerFilas.Close();
            }
            finally { conn.CerrarConexion(); conn.Dispose(); }
            return listar;
        }
        public List<Estilos_t> lista_estilos_dropdown(string po){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT ID_PO_SUMMARY,ITEM_ID FROM PO_SUMMARY WHERE ID_PEDIDOS='"+po+"' ";                
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Estilos_t i = new Estilos_t();
                    i.id_po_summary= Convert.ToInt32(leer_led["ID_PO_SUMMARY"]);
                    i.id_estilo = Convert.ToInt32(leer_led["ITEM_ID"]);
                    i.estilo = consultas.obtener_estilo(i.id_estilo);
                    i.descripcion = consultas.buscar_descripcion_estilo(i.id_estilo);
                    lista.Add(i);
                }leer_led.Close();
            }finally{con_led.CerrarConexion(); con_led.Dispose();}
            return lista;
        }
        public List<Talla_t> lista_tallas_dropdown(string estilo){
            List<Talla_t> lista = new List<Talla_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT TALLA_ITEM FROM ITEM_SIZE WHERE ID_SUMMARY='" + estilo + "' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Talla_t i = new Talla_t();
                    i.id_talla = Convert.ToInt32(leer_led["TALLA_ITEM"]);
                    i.talla = consultas.obtener_size_id(Convert.ToString(leer_led["TALLA_ITEM"]));
                    lista.Add(i);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public List<Item_t> lista_trim_items(){
            List<Item_t> lista = new List<Item_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,descripcion FROM items_catalogue where tipo=2 ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Item_t i = new Item_t();
                    i.id_item = Convert.ToInt32(leer_led["item_id"]);
                    i.descripcion = Convert.ToString(leer_led["descripcion"]);
                    lista.Add(i);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public int obtener_total_estilo(string summary,string talla){
            int total = 0;
            if (talla == "0"){
                query = "SELECT CANTIDAD FROM ITEM_SIZE WHERE ID_SUMMARY='" + summary + "'";
            }else {
                query = "SELECT CANTIDAD FROM ITEM_SIZE WHERE ID_SUMMARY='" + summary + "' and TALLA_ITEM='"+talla+"'";
            }
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = query;
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    total += Convert.ToInt32(leer_ltd["CANTIDAD"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return total;
        }
        public string obtener_family_trim_item(string item){
            string trim = "";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT fabric_type from items_catalogue where item_id='"+item+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["fabric_type"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public void guardar_request(string total, string estilo, string item, string talla,int usuario,string cantidad,string blank){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_requests(id_po_summary,id_size,id_item,total,restante,revision,usuario,fecha,cantidad,blanks,id_status) VALUES "+
                    "('" + estilo + "','" + talla + "','" + item + "','" + total + "','" + total + "','0','" + usuario + "','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','"+cantidad+"','"+blank+"','1')";
                com_c.ExecuteNonQuery();
            }finally{con_c.CerrarConexion(); con_c.Dispose();}
        }
        public void eliminar_trim_request(string request){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText ="DELETE FROM trim_requests WHERE id_request='"+request+"'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public List<Trim> lista_trims_por_pedir() {
            List<Trim> lista = new List<Trim>();
            Conexion con= new Conexion();
            try{
                SqlCommand com= new SqlCommand();
                SqlDataReader leer= null;
                com.Connection = con.AbrirConexion();
                com.CommandText = " SELECT id_request,id_po_summary,id_size,id_item,restante,usuario,fecha from trim_requests where revision=0 ";
                leer= com.ExecuteReader();
                while (leer.Read()){
                    Trim t = new Trim();
                    t.estilo=buscar_informacion_estilo( Convert.ToInt32(leer["id_po_summary"]));
                    t.po= buscar_informacion_pedido(Convert.ToInt32(leer["id_po_summary"]));                    
                    t.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer["usuario"]));
                    t.item = buscar_informacion_item(Convert.ToInt32(leer["id_request"]));
                    lista.Add(t);
                    marcar_revision_item(Convert.ToInt32(leer["id_request"]));
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public string obtener_componente_item(int item){
            string lista = "";
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item FROM items_catalogue where item_id='"+item+"' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    lista= Convert.ToString(leer_led["item"]);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public string obtener_descripcion_item(int item){
            string lista = "";
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT descripcion FROM items_catalogue where item_id='" + item + "' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    lista = Convert.ToString(leer_led["descripcion"]);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public void marcar_revision_item(int item){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET revision=1 WHERE id_request='" + item + "' ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion(); con_s.Dispose();}
        }
        public Estilos_t buscar_informacion_estilo(int summary) {
            Estilos_t e = new Estilos_t();
            e.id_po_summary = summary;
            e.id_estilo = consultas.obtener_estilo_summary(e.id_po_summary);
            e.estilo = consultas.obtener_estilo(e.id_estilo);
            e.descripcion = consultas.buscar_descripcion_estilo(e.id_estilo);
            return e;
        }
        public Pedido_t buscar_informacion_pedido(int summary){
            Pedido_t p = new Pedido_t();
            p.id_pedido = consultas.obtener_id_pedido_summary(summary);
            p.pedido = consultas.obtener_po_id(Convert.ToString(p.id_pedido));
            return p;
        }
        public Item_t buscar_informacion_item(int request){
            Item_t i = new Item_t();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT id_request,id_po_summary,id_size,id_item,restante,usuario,fecha from trim_requests where id_request='"+request+"' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    i.id_item = Convert.ToInt32(leer_led["id_item"]);
                    i.total = Convert.ToInt32(leer_led["restante"]);
                    i.componente = obtener_componente_item(i.id_item);
                    i.descripcion = obtener_descripcion_item(i.id_item);
                    i.fecha = (Convert.ToDateTime(leer_led["fecha"])).ToString("MMM dd yyyy");
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }            
            return i;
        }
        //
        public List<Trim_inventario> lista_trim_recibidos(){
            List<Trim_inventario> lista = new List<Trim_inventario>();
            Conexion con_ltf = new Conexion();
            try{
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                //com_ltf.CommandText = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_recibo,driver,pallet from salidas where estado_entrega=1 and id_destino=1 "; 
                com_ltf.CommandText = "SELECT  i.id_inventario,i.id_pedido,i.id_estilo,i.descripcion,i.id_family_trim,i.id_unit,i.id_trim,i.total,i.id_item" +
                    " FROM inventario i,PEDIDO P where P.ID_PEDIDO=i.id_pedido AND P.ID_STATUS!=6 AND P.ID_STATUS!=7 and i.id_categoria_inventario=2 and i.auditoria=0"; 
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read()){
                    Trim_inventario i = new Trim_inventario();
                    i.id_inventario= Convert.ToInt32(leer_ltf["id_inventario"]);
                    i.id_pedido= Convert.ToInt32(leer_ltf["id_pedido"]);
                    i.pedido = consultas.obtener_po_id(Convert.ToString(i.id_pedido));
                    i.id_estilo= Convert.ToInt32(leer_ltf["id_estilo"]);
                    i.estilo = consultas.obtener_estilo(i.id_estilo) + " " + consultas.buscar_descripcion_estilo(i.id_estilo);
                    i.id_family_trim= Convert.ToInt32(leer_ltf["id_family_trim"]);
                    i.family_trim = consultas.obtener_family_id((i.id_family_trim).ToString());
                    i.id_unit= Convert.ToInt32(leer_ltf["id_unit"]);
                    i.unit = consultas.obtener_unit_id(Convert.ToString(i.id_unit));
                    i.id_item= Convert.ToInt32(leer_ltf["id_item"]);
                    i.descripcion = Convert.ToString(leer_ltf["descripcion"]);
                    i.total = Convert.ToInt32(leer_ltf["total"]);
                    lista.Add(i);                   

                }
                leer_ltf.Close();
            }finally { con_ltf.CerrarConexion(); con_ltf.Dispose(); }
            return lista;
        }
        //
        public salidas_item obtener_id_inventario_salida(int salida){
            salidas_item s = new salidas_item();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_inventario,id_salida,cantidad,id_salida_item,id_pedido,id_estilo FROM salidas_items WHERE id_salida_item='"+salida+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    s.id_inventario = Convert.ToInt32(leer_ltd["id_inventario"]);
                    s.id_salida= Convert.ToInt32(leer_ltd["id_salida"]);
                    s.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    s.id_salida_item = Convert.ToInt32(leer_ltd["id_salida_item"]);
                    s.summary = consultas.obtener_po_summary(Convert.ToInt32(leer_ltd["id_pedido"]), Convert.ToInt32(leer_ltd["id_estilo"]));
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return s;
        }
        

        public void cambiar_cantidad_salida_item(int salida, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas_items SET cantidad='" + cantidad + "' WHERE id_salida_item='" + salida + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void actualizar_cantidad_salida(int salida){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas SET total=(SELECT SUM(cantidad) FROM salidas_items WHERE id_salida='"+salida+"') WHERE id_salida='" + salida + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void sumar_cantidad_item_request(int summary, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET restante=restante+'" + cantidad + "' WHERE id_po_summary='" + summary + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void restar_cantidad_item_request(int summary, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET restante=restante-'" + cantidad + "' WHERE id_po_summary='" + summary + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public List<Estilos_t> obtener_lista_summary(int pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT ID_PO_SUMMARY,ITEM_ID,ID_GENDER FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Estilos_t i = new Estilos_t();
                    i.id_po_summary = Convert.ToInt32(leer_led["ID_PO_SUMMARY"]);
                    i.id_estilo = Convert.ToInt32(leer_led["ITEM_ID"]);
                    i.estilo = consultas.obtener_estilo(i.id_estilo);
                    i.descripcion = consultas.buscar_descripcion_estilo(i.id_estilo);
                    lista.Add(i);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public List<Estilos_t> obtener_lista_generos(int pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT distinct ID_GENDER FROM PO_SUMMARY WHERE ID_PEDIDOS='" + pedido + "' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Estilos_t i = new Estilos_t();
                    i.genero = consultas.obtener_sigla_genero(Convert.ToString(leer_led["ID_GENDER"]));
                    lista.Add(i);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public List<Trim> obtener_datos_trim_card(string pedido,string usuario) {
            List<Trim> lista = new List<Trim>();
            int id_pedido = consultas.buscar_pedido(pedido);
            Trim t = new Trim();
            t.lista_estilos= obtener_lista_summary(id_pedido);
            t.lista_generos= obtener_lista_generos(id_pedido);
            t.id_pedido = id_pedido;
            t.pedido = Regex.Replace(pedido, @"\s+", " "); 
            t.usuario = consultas.buscar_nombre_usuario(usuario);
            t.customer= Regex.Replace(consultas.obtener_customer_id(Convert.ToString(consultas.buscar_cliente_final_po(pedido))), @"\s+", " ");
            lista.Add(t);                
            return lista;
        }

        public void actualizar_cantidad_inventario(int inventario, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total='" + cantidad + "' WHERE id_inventario='" + inventario + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }


        public List<Trim_item> lista_descripciones_trims(){
            List<Trim_item> lista = new List<Trim_item>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,descripcion from items_catalogue where tipo=2 ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Trim_item ti = new Trim_item();
                    ti.id_item = Convert.ToInt32(leer_led["item_id"]);
                    ti.descripcion = Convert.ToString(leer_led["descripcion"]);
                    lista.Add(ti);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }

        public List<Trim_item> informacion_editar_item_trim(string id){
            List<Trim_item> lista = new List<Trim_item>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,item,body_type,descripcion,fabric_type,unit,minimo from items_catalogue where item_id='"+id+"' ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Trim_item ti = new Trim_item();
                    ti.id_item = Convert.ToInt32(leer_led["item_id"]);
                    ti.item = Convert.ToString(leer_led["item"]);
                    ti.descripcion = Convert.ToString(leer_led["descripcion"]);
                    ti.family = Convert.ToString(leer_led["fabric_type"]);
                    ti.unit = Convert.ToString(leer_led["unit"]);
                    ti.minimo = Convert.ToInt32(leer_led["minimo"]);
                    lista.Add(ti);
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }

        public void editar_informacion_trim(string id,string item,string minimo,string descripcion,string family,string unidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE items_catalogue SET item='" + item + "', descripcion='"+descripcion+ "',body_type='" + unidad + "', " +
                    " unit='" + unidad + "', fabric_type='" + family + "', minimo='"+minimo+"' " +
                    " WHERE item_id='" + id + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public List<Trim_item> lista_trims_tabla_inicio(){
            List<Trim_item> lista = new List<Trim_item>();
            Conexion con_led = new Conexion();
            try{
                SqlCommand com_led = new SqlCommand();
                SqlDataReader leer_led = null;
                com_led.Connection = con_led.AbrirConexion();
                com_led.CommandText = " SELECT item_id,item,body_type,descripcion,fabric_type,unit,minimo from items_catalogue where minimo!=0 and tipo=2 ";
                leer_led = com_led.ExecuteReader();
                while (leer_led.Read()){
                    Trim_item ti = new Trim_item();
                    ti.id_item = Convert.ToInt32(leer_led["item_id"]);
                    ti.item = Convert.ToString(leer_led["item"]);
                    ti.descripcion = Convert.ToString(leer_led["descripcion"]);
                    ti.family = Convert.ToString(leer_led["fabric_type"]);
                    ti.unit = Convert.ToString(leer_led["unit"]);
                    ti.minimo = Convert.ToInt32(leer_led["minimo"]);
                    ti.total = buscar_totales_item(ti.id_item);
                    if (ti.total < ti.minimo) {
                        lista.Add(ti);
                    }                    
                }leer_led.Close();
            }finally { con_led.CerrarConexion(); con_led.Dispose(); }
            return lista;
        }
        public int buscar_totales_item(int id){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from inventario where id_item='" + id + "'"; //AQUI QUEDA PENDIENTE QUE INVENTARIO VOY A REVISAR
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally{ con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public int obtener_cajas_estilo(string summary,string talla) {
            int cajas = 0,contador_ppk=0;
            List<ratio_tallas> lista = obtener_ratio_estilo(summary, talla);
            foreach (ratio_tallas r in lista) {
                if (r.tipo_empaque == 1) {
                    cajas += r.total / r.piezas;
                } else {
                    if (contador_ppk == 0) {
                        cajas += r.total / r.ratio;
                    }
                    contador_ppk++;
                }
            }
            return cajas;
        }
        public List<ratio_tallas> obtener_ratio_estilo(string summary, string talla) {
            string query;
            if (talla == "0"){
                query = "SELECT PIECES,RATIO,ID_TALLA,TYPE_PACKING FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + summary + "'";
            }else{
                query = "SELECT PIECES,RATIO,ID_TALLA,TYPE_PACKING FROM PACKING_TYPE_SIZE WHERE ID_SUMMARY='" + summary + "' and ID_TALLA='" + talla + "'";
            }
            List<ratio_tallas> lista = new List<ratio_tallas>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = query;
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    ratio_tallas r = new ratio_tallas();
                    r.ratio= Convert.ToInt32(leer_ltd["RATIO"]);
                    r.id_talla= Convert.ToInt32(leer_ltd["ID_TALLA"]);
                    r.piezas = Convert.ToInt32(leer_ltd["PIECES"]);
                    r.tipo_empaque= Convert.ToInt32(leer_ltd["TYPE_PACKING"]);
                    r.total = obtener_total_estilo(summary, talla);
                    lista.Add(r);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }

        public List<Trim_requests> obtener_trims_anteriores(string summary) {
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT entregado,id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario,fecha,id_recibo,cantidad,blanks,comentarios from trim_requests where id_po_summary='"+summary+"'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item))+" "+consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    tr.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    tr.blanks = Convert.ToInt32(leer_ltd["blanks"]);
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.fecha=Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.id_usuario= Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.comentarios = Convert.ToString(leer_ltd["comentarios"]);
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo =consultas.obtener_estilo((tr.id_estilo))+" " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.entregado = Convert.ToInt32(leer_ltd["entregado"]);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }


        public List<Estilos_t> obtener_trims_anteriores_pedido(string pedido){
            List<Estilos_t> lista = new List<Estilos_t>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT ID_PO_SUMMARY FROM PO_SUMMARY WHERE ID_PEDIDOS='"+pedido+"' ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Estilos_t tr = new Estilos_t();
                    tr.id_po_summary = Convert.ToInt32(leer_ltd["ID_PO_SUMMARY"]);
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_po_summary);
                    tr.estilo = consultas.obtener_estilo(tr.id_estilo) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.lista_requests = obtener_trims_anteriores(Convert.ToString(tr.id_po_summary));
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }

        public List<Trim_requests> comparacion_inventario_trim(string salida){
            List<Trim_requests> lista = new List<Trim_requests>();
            string[] salidas = salida.Split('*');
            for (int i = 1; i < salidas.Length; i++) {
                int summary=0, item=0, total=0,pedido=0;
                Conexion con_ltd = new Conexion();
                try{
                    SqlCommand com_ltd = new SqlCommand();
                    SqlDataReader leer_ltd = null;
                    com_ltd.Connection = con_ltd.AbrirConexion();
                    com_ltd.CommandText = "SELECT id_inventario,total,id_estilo,id_pedido,id_item from inventario where id_inventario='" + salidas[i] + "'";
                    leer_ltd = com_ltd.ExecuteReader();
                    while (leer_ltd.Read()){
                        summary = consultas.obtener_po_summary(Convert.ToInt32(leer_ltd["id_pedido"]), Convert.ToInt32(leer_ltd["id_estilo"]));
                        item= Convert.ToInt32(leer_ltd["id_item"]);
                        pedido= Convert.ToInt32(leer_ltd["id_pedido"]);
                        total+= Convert.ToInt32(leer_ltd["total"]);
                    }leer_ltd.Close();
                }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
                Trim_requests tr = obtener_trim_comparacion(summary, item, total,pedido);
                if (tr != null) {
                    lista.Add(tr);
                }
            }   
            return lista;
        }

        public Trim_requests obtener_trim_comparacion(int summary,int item, int total,int pedido){
            //Trim_requests lista = new Trim_requests();
            Trim_requests tr = new Trim_requests();            
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario," +
                    " fecha,id_recibo,cantidad,blanks,comentarios from trim_requests " +
                    " where id_po_summary='" + summary + "' and id_item='" + item + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    //tr.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    tr.cantidad =total; //LO QUE LLEGÓ
                    tr.blanks = Convert.ToInt32(leer_ltd["blanks"]);
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);                    
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    //tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    tr.fecha = buscar_fecha_recibo_item(tr.recibo);
                    tr.id_usuario = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.comentarios = Convert.ToString(leer_ltd["comentarios"]);
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    tr.pedido = consultas.obtener_po_id(Convert.ToString(pedido));                    
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return tr;
        }

        public string buscar_fecha_recibo_item(int recibo){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT fecha from recibos where id_recibo='" + recibo + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public void cambiar_estado_auditoria_inventario(string inventario){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET auditoria=1 WHERE id_inventario='" + inventario + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
                    
        public List<Pedido_t> obtener_lista_ordenes_estados(string busqueda) {
            string query = "";
            if (busqueda == "0"){
                query = "SELECT TOP 30 ID_PEDIDO,PO FROM PEDIDO WHERE ID_STATUS!=6 AND ID_STATUS!=7";
            }else {
                query = "SELECT TOP 30 ID_PEDIDO,PO FROM PEDIDO WHERE ID_STATUS!=6 AND ID_STATUS!=7 AND PO LIKE'%"+busqueda+"%'";
            }
            List<Pedido_t> lista = new List<Pedido_t>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                //com.CommandText = "SELECT TOP 25 ID_PEDIDO,PO FROM PEDIDO WHERE ID_STATUS!=6 AND ID_STATUS!=7";
                com.CommandText =query;
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Pedido_t p = new Pedido_t();
                    p.id_pedido= Convert.ToInt32(leer["ID_PEDIDO"]);
                    p.pedido= Convert.ToString(leer["PO"]);
                    p.lista_estilos= obtener_trims_anteriores_pedido(Convert.ToString(leer["ID_PEDIDO"]));
                    lista.Add(p);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public List<Inventario> obtener_lista_trims_inicio(string busqueda,string fecha) {
            List<Inventario> lista = new List<Inventario>();
            if (busqueda == "0" && fecha=="0"){
                lista.AddRange(lista_trims_inicio_default());                
            }else{
                if (busqueda == "0" && fecha != "0") {
                    lista.AddRange(lista_trims_inicio_fecha(busqueda, fecha));
                } else {
                    lista.AddRange(lista_trims_inicio_trim(busqueda, fecha));
                    lista.AddRange(lista_trims_inicio_mp(busqueda, fecha));
                    lista.AddRange(lista_trims_inicio_po(busqueda, fecha));
                }                
            }            
            return lista;
        }
        public List<Inventario> lista_trims_inicio_default() {
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo " +
                    " AND r.fecha between '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00' AND '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +//AND r.fecha<'2019-03-01 12:00:00'
                    "AND ic.tipo=2 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_trim(string busqueda, string fecha){
            string query = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo AND ic.tipo=2 ";                    
            if (busqueda != "0") { query+= " AND i.descripcion like '%" + busqueda + "%'  "; }
            if (fecha != "0") { query+= " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  "; }
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = query;                  
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_mp(string busqueda, string fecha){
            string query = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo AND ic.tipo=2  ";
            if (busqueda != "0") { query += " AND r.mp_number like '%" + busqueda + "%'  "; }
            if (fecha != "0") { query += " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  "; }
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = query;
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_po(string busqueda, string fecha){
            string query = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic,PEDIDO p" +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo AND i.id_pedido=p.ID_PEDIDO AND ic.tipo=2 ";
            if (busqueda != "0") { query += "  AND p.PO like '%" + busqueda + "%' "; }
            if (fecha != "0") { query += " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  "; }
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = query;
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        public List<Inventario> lista_trims_inicio_fecha(string busqueda, string fecha){        
            List<Inventario> lista = new List<Inventario>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT DISTINCT r.fecha,i.id_inventario,i.id_pedido,i.id_estilo,i.total,i.id_item,i.auditoria,ic.descripcion,ic.fabric_type " +
                    "  FROM recibos r,inventario i,recibos_items ri,items_catalogue ic " +
                    " WHERE i.id_inventario=ri.id_inventario AND ic.item_id=i.id_item AND ri.id_recibo=r.id_recibo " +
                    " AND r.fecha between '" + fecha + " 00:00:00' and  '" + fecha + " 23:59:59'  " +
                    "AND ic.tipo=2 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    i.estilo = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(leer["id_estilo"]));
                    i.total = Convert.ToInt32(leer["total"]);
                    i.auditoria = Convert.ToInt32(leer["auditoria"]);
                    i.descripcion = Convert.ToString(leer["descripcion"]) + " " + Convert.ToString(leer["fabric_type"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("dd-MM-yyyy");
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }
        /*******************************************************************************************************************************************************/
        public int obtener_id_recibo_inventario(int inventario,int total){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT TOP 1 id_recibo from recibos_items where id_inventario='" + inventario + "' and total='"+total+"' order by id_recibo_item desc ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_recibo"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }

        public List<recibo> buscar_mp_recibos_hoy() {
            List<recibo> lista = new List<recibo>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT distinct r.mp_number from recibos r,recibos_items ri,inventario i,items_catalogue ic where" +
                    " r.id_recibo=ri.id_recibo AND ri.id_inventario=i.id_inventario AND i.id_item=ic.item_id " +
                    " AND r.fecha between '" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00' AND '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' " +//AND r.fecha<'2019-03-01 12:00:00'
                    "AND ic.tipo=2 ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    recibo i = new recibo();
                    i.mp_number = Convert.ToString(leer["mp_number"]);                  
                    lista.Add(i);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public Trim_requests buscar_trim( int summary,int item) {
            Trim_requests tr = new Trim_requests();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_request,id_po_summary,id_size,id_item,total,restante,revision,usuario," +
                    " fecha,id_recibo,cantidad,blanks,comentarios from trim_requests " +
                    " where id_po_summary='" + summary + "' and id_item='" + item + "'";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);
                    tr.cantidad = Convert.ToInt32(leer_ltd["cantidad"]);
                    tr.blanks = Convert.ToInt32(leer_ltd["blanks"]);
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.recibo = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM-yyyy");
                    //tr.fecha = buscar_fecha_recibo_item(tr.recibo);
                    tr.id_usuario = Convert.ToInt32(leer_ltd["id_recibo"]);
                    tr.usuario = consultas.buscar_nombre_usuario(Convert.ToString(tr.id_usuario));
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    //tr.comentarios = Convert.ToString(leer_ltd["comentarios"]);
                    //tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    //tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return tr;
        }
        public void update_trim_request(int request, int cantidad, int inventario){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET restante=restante-'" + cantidad + "',id_inventario='" + inventario + 
                    "' WHERE id_request='" + request + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public void cambiar_estado_trim_request(int request, int estado){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET id_status='" + estado + "' WHERE id_request='" + request + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }


        public void revisar_estados_cantidades_trim(int inventario,int cantidad) {
            Inventario inv = dt.consultar_item(inventario);
            int summary = consultas.obtener_po_summary(inv.id_pedido,inv.id_estilo);
            Trim_requests trim = buscar_trim(summary,inv.id_item);
            update_trim_request(trim.id_request,cantidad,inv.id_inventario);
            trim.restante = trim.restante - cantidad;
            if (trim.restante >= trim.total) {
                cambiar_estado_trim_request(trim.id_request,3);
            } else {
                cambiar_estado_trim_request(trim.id_request, 2);
            }           
        }
        
        public void guardar_entrega(string pedido, string entrega, string recibe){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_entregas(entrega,recibe,fecha,id_pedido) VALUES " +
                    "('" + entrega + "','" + recibe + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + pedido + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int obtener_ultima_entrega(){
            int temp = 0;
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i.Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT TOP 1 id_entrega FROM trim_entregas order by id_entrega desc ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    temp = Convert.ToInt32(leer_u_i["id_entrega"]);
                }leer_u_i.Close();
            }finally { con_u_i.CerrarConexion(); con_u_i.Dispose(); }
            return temp;
        }
        public void guardar_entrega_item(int entrega, string request, string cantidad){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO trim_entrega_items(id_entrega,id_request,total) VALUES " +
                    "('" + entrega + "','" + request + "','" + cantidad + "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void actualizar_estado_entrega_request(string request, string cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE trim_requests SET entregado=entregado+'" + cantidad + "' " +
                    " WHERE id_request='" + request + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public int obtener_id_inventario_request(string request){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario from trim_requests where id_request='" + request + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_inventario"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public void actualizar_inventario(int inventario, string cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total=total-'" + cantidad + "' " +
                    "' WHERE id_inventario='" + inventario + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public List<Trim_entregas> obtener_lista_entregas_fechas(string inicio,string final) {
            List<Trim_entregas> lista = new List<Trim_entregas>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_entrega,entrega,recibe,fecha,id_pedido from trim_entregas  where" +
                    "  fecha between '" + inicio + " 00:00:00' AND '" + final + " 23:59:59'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Trim_entregas i = new Trim_entregas();
                    i.id_entrega = Convert.ToInt32(leer["id_entrega"]);
                    i.fecha = Convert.ToDateTime(leer["fecha"]).ToString("MM/dd/yyyy hh:mm:ss");//dd-MM-yyyy
                    i.entrega = Convert.ToString(leer["entrega"]);
                    i.recibe = Convert.ToString(leer["recibe"]);
                    i.lista_request = obtener_lista_entregas_fechas(i.id_entrega);
                    i.pedido = consultas.obtener_po_id(Convert.ToString(leer["id_pedido"]));
                    lista.Add(i);
                }leer.Close();
            }
            finally { con.CerrarConexion(); con.Dispose(); }
            return lista;
        }

        public List<Trim_requests> obtener_lista_entregas_fechas(int entrega){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT tr.id_request,tr.id_po_summary,tr.id_size,tr.id_item,revision,tr.usuario," +
                    " tr.fecha,tr.id_recibo,tr.cantidad,tei.total from trim_requests tr,trim_entrega_items tei " +
                    " where tei.id_entrega='" + entrega + "' and tei.id_request=tr.id_request";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_amt_item(Convert.ToString(tr.id_item)) + " " + consultas.buscar_descripcion_item(Convert.ToString(tr.id_item));
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);                                        
                    tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.obtener_estilo((tr.id_estilo)) + " " + consultas.buscar_descripcion_estilo(tr.id_estilo);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }

        public Pedidos_trim buscar_estado_total_pedido(int pedido) {
            Pedidos_trim p = new Pedidos_trim();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT P.PO,P.CUSTOMER,P.DATE_ORDER,CC.NAME from " +
                    " PEDIDO P, CAT_CUSTOMER CC where P.ID_PEDIDO='" + pedido + "' and CC.CUSTOMER=P.CUSTOMER ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    p.id_pedido = pedido;
                    p.pedido = Convert.ToString(leer["PO"]);
                    p.id_customer = Convert.ToInt32(leer["CUSTOMER"]);                    
                    p.customer = Convert.ToString(leer["NAME"]);
                    p.ship_date= Convert.ToDateTime(leer["DATE_ORDER"]).ToString("dd-MMM");
                    List<int> temporal = consultas.Lista_generos_po(pedido);
                    p.gender = "";
                    foreach (int x in temporal) {
                        p.gender += consultas.obtener_genero_id(Convert.ToString(x));
                    }
                    p.gender= Regex.Replace(p.gender, @"\s+", " ");
                    p.lista_families = obtener_lista_familias(pedido);
                    p.lista_empaque = lista_tipos_empaque(Convert.ToString(pedido));
                    p.lista_assort = ds.lista_assortments_pedido(pedido);
                }
                leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return p;
        }

        public List<Family_trim> obtener_lista_familias(int pedido) {
            List<Family_trim> lista = new List<Family_trim>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_family_trim,family_trim FROM family_trims";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Family_trim ft = new Family_trim();
                    ft.id_family_trim = Convert.ToInt32(leer_ltd["id_family_trim"]);
                    ft.family_trim = Convert.ToString(leer_ltd["family_trim"]);
                    ft.lista_requests = buscar_trims_pedido_familia(pedido,ft.family_trim);
                    lista.Add(ft);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }

        public List<Trim_requests> buscar_trims_pedido_familia(int pedido,string familia){
            List<Trim_requests> lista = new List<Trim_requests>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT tr.entregado,tr.id_request,tr.id_po_summary,tr.id_size,tr.id_item,tr.total,tr.restante, " +
                    " tr.revision,tr.usuario,tr.fecha,tr.id_recibo,tr.id_inventario,tr.id_status,tr.id_inventario,ts.status " +
                    " FROM trim_requests tr,items_catalogue ic,PO_SUMMARY ps, trim_status ts " +
                    " WHERE ps.ID_PEDIDOS='"+pedido+"' AND ps.ID_PO_SUMMARY=tr.id_po_summary AND ic.item_id=tr.id_item " +
                    " AND ic.fabric_type='"+familia+"' AND ts.id_status=tr.id_status ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Trim_requests tr = new Trim_requests();
                    tr.id_request = Convert.ToInt32(leer_ltd["id_request"]);
                    tr.id_summary = Convert.ToInt32(leer_ltd["id_po_summary"]);
                    tr.id_talla = Convert.ToInt32(leer_ltd["id_size"]);
                    tr.id_item = Convert.ToInt32(leer_ltd["id_item"]);
                    tr.item = consultas.buscar_descripcion_item(Convert.ToString(tr.id_item)); 
                    tr.tipo_item = consultas.buscar_tipo_trim_item(Convert.ToString(tr.id_item));
                    tr.total = Convert.ToInt32(leer_ltd["total"]);                                     
                    tr.restante = Convert.ToInt32(leer_ltd["restante"]);
                    tr.cantidad = tr.total-tr.restante;//cuanto ha llegado
                    tr.fecha = Convert.ToDateTime(leer_ltd["fecha"]).ToString("dd-MM");
                    if (tr.id_talla != 0){
                        tr.talla = consultas.obtener_size_id(Convert.ToString(tr.id_talla));
                    }else {
                        tr.talla = "";
                    }
                    tr.id_estilo = consultas.obtener_estilo_summary(tr.id_summary);
                    tr.estilo = consultas.buscar_descripcion_estilo(tr.id_estilo); 
                    tr.entregado = Convert.ToInt32(leer_ltd["entregado"]);
                    tr.id_inventario = Convert.ToInt32(leer_ltd["id_inventario"]);
                    tr.recibo = buscar_id_recibo_inventario(tr.id_inventario);
                    if (tr.recibo != 0) { tr.recibo_item = di.lista_recibo_detalles(Convert.ToString(tr.recibo)); }
                    tr.id_estado = Convert.ToInt32(leer_ltd["id_status"]);
                    tr.estado = Convert.ToString(leer_ltd["status"]);
                    lista.Add(tr);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }
        public string buscar_status_trims(int estado){
            string trim = "";
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT status from trim_status where id_status='" + estado + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToString(leer_ltd["status"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public int buscar_id_recibo_inventario(int inventario){
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT r.id_recibo FROM recibos r,recibos_items ri WHERE" +
                    " r.id_recibo=ri.id_recibo AND ri.id_inventario='" + inventario + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToInt32(leer_ltd["id_recibo"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }
        public int buscar_estado_auditoria(int inventario){
            int trim = 0;
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT auditoria FROM inventario WHERE id_inventario='" + inventario + "'   ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    trim = Convert.ToInt32(leer_ltd["auditoria"]);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return trim;
        }

        public List<Pedidos_trim> obtener_pedidos_fechas(string inicio,string final) {
            List<Pedidos_trim> lista = new List<Pedidos_trim>();            
                Conexion con = new Conexion();
                try{
                    SqlCommand com = new SqlCommand();
                    SqlDataReader leer = null;
                    com.Connection = con.AbrirConexion();
                    com.CommandText = "SELECT P.ID_PEDIDO,P.PO,P.CUSTOMER,P.DATE_ORDER,CC.NAME from " +
                        " PEDIDO P, CAT_CUSTOMER CC where CC.CUSTOMER=P.CUSTOMER " +
                        " and P.DATE_ORDER BETWEEN '" + inicio + " 00:00:00' AND '" + final + " 23:59:59' ";                    
                    leer = com.ExecuteReader();
                    while (leer.Read()){
                        Pedidos_trim p = new Pedidos_trim();
                        p.id_pedido = Convert.ToInt32(leer["ID_PEDIDO"]);
                        p.pedido = Convert.ToString(leer["PO"]);
                        p.id_customer = Convert.ToInt32(leer["CUSTOMER"]);
                        p.customer = Convert.ToString(leer["NAME"]);
                        p.ship_date = Convert.ToDateTime(leer["DATE_ORDER"]).ToString("dd-MMM");
                        List<int> temporal = consultas.Lista_generos_po(p.id_pedido);
                        p.gender = "";
                        foreach (int x in temporal){
                            p.gender += consultas.obtener_genero_id(Convert.ToString(x));
                        }
                        p.gender = Regex.Replace(p.gender, @"\s+", " ");
                        p.lista_families = obtener_lista_familias(p.id_pedido);
                        p.lista_empaque = lista_tipos_empaque(Convert.ToString(p.id_pedido));
                        p.lista_assort = ds.lista_assortments_pedido(p.id_pedido);
                    lista.Add(p);
                    }leer.Close();
                }finally { con.CerrarConexion(); con.Dispose(); }            
            return lista;
        }

        public List<Family_trim> obtener_familias(){
            List<Family_trim> lista = new List<Family_trim>();
            Conexion con_ltd = new Conexion();
            try{
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_family_trim,family_trim FROM family_trims";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read()){
                    Family_trim ft = new Family_trim();
                    ft.id_family_trim = Convert.ToInt32(leer_ltd["id_family_trim"]);
                    ft.family_trim = Convert.ToString(leer_ltd["family_trim"]);                    
                    lista.Add(ft);
                }leer_ltd.Close();
            }finally { con_ltd.CerrarConexion(); con_ltd.Dispose(); }
            return lista;
        }

        public List<Empaque> lista_tipos_empaque(string id_pedido){
            List<Empaque> lista = new List<Empaque>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "select distinct ITEM_ID from PO_SUMMARY where ID_PEDIDOS='" + id_pedido + "' ";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    estilo_shipping l = new estilo_shipping();
                    l.id_estilo = Convert.ToInt32(leerFilas["ITEM_ID"]);
                    l.id_summary = consultas.obtener_po_summary(Convert.ToInt32(id_pedido), l.id_estilo);                   ;
                    List<Empaque> lista_e = new List<Empaque>();
                    List<string> tipo_empaque_temporal = consultas.buscar_tipo_empaque(l.id_summary);
                    foreach (string s in tipo_empaque_temporal){
                        Empaque e = new Empaque();
                        e.tipo_empaque = Convert.ToInt32(s);
                        if (s == "1") { e.lista_ratio = ds.obtener_lista_tallas_estilo(l.id_summary, l.id_estilo); }
                        if (s == "2") { e.lista_ratio = ds.obtener_lista_ratio(l.id_summary, l.id_estilo, 2); }
                        lista.Add(e);
                    }
                }leerFilas.Close();
            }finally { conn.CerrarConexion(); conn.Dispose(); }
            return lista;
        }



























    }
}