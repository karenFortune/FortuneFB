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
    public class DatosTransferencias
    {

        public int id_salida, id_inventario, cantidad;
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();

        public List<lugares> lista_lugares_transfer(){
            List<lugares> listalugares = new List<lugares>();
            Conexion con_lt = new Conexion();
            try{
                SqlCommand com_lt = new SqlCommand();
                SqlDataReader leer_lt = null;
                com_lt.Connection = con_lt.AbrirConexion();
                com_lt.CommandText = "SELECT id_lugar,lugar from lugares where tipo_lugar=0 order by lugar ";
                leer_lt = com_lt.ExecuteReader();
                while (leer_lt.Read()){
                    lugares l = new lugares();
                    l.id_lugar = Convert.ToInt32(leer_lt["id_lugar"]);
                    l.lugar = Convert.ToString(leer_lt["lugar"]);
                    listalugares.Add(l);
                }leer_lt.Close();
            }finally{con_lt.CerrarConexion(); con_lt.Dispose();}
            return listalugares;
        }
        public List<lugares> lista_lugares_transfer_destino(string origen)
        {
            List<lugares> listalugares = new List<lugares>();
            Conexion con_ltd = new Conexion();
            try
            {
                SqlCommand com_ltd = new SqlCommand();
                SqlDataReader leer_ltd = null;
                com_ltd.Connection = con_ltd.AbrirConexion();
                com_ltd.CommandText = "SELECT id_lugar,lugar from lugares where id_lugar!='" + origen + "' order by lugar ";
                leer_ltd = com_ltd.ExecuteReader();
                while (leer_ltd.Read())
                {
                    lugares l = new lugares();
                    l.id_lugar = Convert.ToInt32(leer_ltd["id_lugar"]);
                    l.lugar = Convert.ToString(leer_ltd["lugar"]);
                    listalugares.Add(l);
                }
                leer_ltd.Close();
            }
            finally
            {
                con_ltd.CerrarConexion(); con_ltd.Dispose();
            }
            return listalugares;
        }
        public List<Inventario> obtener_inventario_sucursal(string origen, string busqueda){
            List<Inventario> listInventario = new List<Inventario>();
            Conexion con_ois = new Conexion();
            try{
                SqlCommand com_ois = new SqlCommand();
                SqlDataReader leer_ois = null;
                com_ois.Connection = con_ois.AbrirConexion();
                com_ois.CommandText = "SELECT top 50 i.id_inventario,i.id_pedido,i.total,i.descripcion" +
                    " from inventario i,sucursales s where i.id_sucursal=s.id_sucursal and i.total!=0 and"+
                    " i.id_sucursal='" + origen + "' and i.descripcion like '%" + busqueda + "%'";
                leer_ois = com_ois.ExecuteReader();
                while (leer_ois.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer_ois["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer_ois["id_pedido"]));
                    i.total = Convert.ToInt32(leer_ois["total"]);
                    i.descripcion = Convert.ToString(leer_ois["descripcion"]);
                    listInventario.Add(i);
                }leer_ois.Close();
            }finally{con_ois.CerrarConexion(); con_ois.Dispose();}
            return listInventario;
        }
        public void guardar_transferencia_inventario(string fecha, string persona, string sello, string origen, string destino, string driver, string pallet, string envio, int total, int id_usuario, string id_sello,string carro,string placas)
        {
            Conexion con_gti = new Conexion();
            try{
                SqlCommand com_gti = new SqlCommand();
                string id_sucursal = consultas.obtener_sucursal_id_usuario(id_usuario);
                Conexion con_c = new Conexion();
                SqlCommand com_c = new SqlCommand();
                com_gti.Connection = con_gti.AbrirConexion();
                com_gti.CommandText = "INSERT INTO salidas(fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_solicitud,driver,pallet,id_sello,id_sucursal,auto,placas) VALUES " +
                    " ('" + fecha + " 00:00:00','" + total + "','" + id_usuario + "','" + origen + "','" + destino + "','0','0','" + sello + "','" + persona + "','" + envio + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + driver + "','" + pallet + "','" + id_sello + "','" + id_sucursal + "','"+carro+"','"+placas+"')";
                com_gti.ExecuteNonQuery();
            }finally{con_gti.CerrarConexion(); con_gti.Dispose();}
        }
        public void guardar_edicion_transferencia_inventario(int salida,string fecha, string persona, string sello, string origen, string destino, string driver, string pallet, string envio, int total, int id_usuario, string id_sello, string carro, string placas)
        {
            Conexion con_gti = new Conexion();
            try{
                SqlCommand com_gti = new SqlCommand();
                string id_sucursal = consultas.obtener_sucursal_id_usuario(id_usuario);
                Conexion con_c = new Conexion();
                SqlCommand com_c = new SqlCommand();
                com_gti.Connection = con_gti.AbrirConexion();
                com_gti.CommandText = "UPDATE salidas SET fecha='" + fecha + " 00:00:00',total='" + total + "',id_usuario='" + id_usuario + "',id_origen='" + origen + "',id_destino='" + destino +
                    "', sello='" + sello + "',responsable='" + persona + "',id_envio='" + envio + "',fecha_solicitud='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "', driver='" + driver + "',pallet='" + pallet + "',id_sello='" + id_sello + "',id_sucursal='" + id_sucursal + "',auto='" + carro + "',placas='" + placas + "' where id_salida='"+salida+"'";
                com_gti.ExecuteNonQuery();
            }finally { con_gti.CerrarConexion(); con_gti.Dispose(); }
        }

        public int obtener_ultima_transferencia(){
            int id_salida = 0;
            Conexion con_u_r_i = new Conexion();
            try{
                SqlCommand com_u_r_i = new SqlCommand();
                SqlDataReader leer_u_r_i = null;
                com_u_r_i.Connection = con_u_r_i.AbrirConexion();
                com_u_r_i.CommandText = "SELECT TOP 1 id_salida FROM salidas order by id_salida desc ";
                leer_u_r_i = com_u_r_i.ExecuteReader();
                while (leer_u_r_i.Read()){
                    id_salida = Convert.ToInt32(leer_u_r_i["id_salida"]);
                }leer_u_r_i.Close();
            }finally{con_u_r_i.CerrarConexion(); con_u_r_i.Dispose();}
            return id_salida;
        }
        public void aumentar_sellos(string sello, int sucursal)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE sellos SET usado='" + sello + "' WHERE  id_sucursal='" + sucursal + "' and usado<final";
                com_s.ExecuteNonQuery();
            }
            finally
            {
                con_s.CerrarConexion(); con_s.Dispose();
            }
        }
        public void revisar_sellos(string id_sello, string sello)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE sellos SET estado=1 WHERE id_sello='" + id_sello + "' and usado=final ";
                com_s.ExecuteNonQuery();
            }
            finally
            {
                con_s.CerrarConexion(); con_s.Dispose();
            }
        }
        public void guardar_items_inventario(int salida, string id, string cantidad, int po, int estilo, string cajas,string codigo)
        {
            Conexion con_c = new Conexion();
            try
            {
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO salidas_items(id_salida,id_inventario,cantidad,id_pedido,id_estilo,cajas,codigo) VALUES ('" + salida + "','" + id + "','" + cantidad + "','" + po + "','" + estilo + "','"+cajas+"','"+codigo+"')";
                com_c.ExecuteNonQuery();
            }
            finally
            {
                con_c.CerrarConexion(); con_c.Dispose();
            }
        }
        public IEnumerable<salidas> ListaTransferencias()
        {
            List<salidas> listalugares = new List<salidas>();
            Conexion con_ltf = new Conexion();
            try
            {
                SqlCommand com_ltf = new SqlCommand();
                SqlDataReader leer_ltf = null;
                com_ltf.Connection = con_ltf.AbrirConexion();
                com_ltf.CommandText = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_solicitud,driver,pallet from salidas ";
                leer_ltf = com_ltf.ExecuteReader();
                while (leer_ltf.Read())
                {
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
                    listalugares.Add(l);
                }
                leer_ltf.Close();
            }
            finally
            {
                con_ltf.CerrarConexion(); con_ltf.Dispose();
            }
            return listalugares;
        }
        public void aprobar_transferencia_inventario(string salida){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas SET estado_aprobacion=1 WHERE id_salida='" + salida + "'  ";
                com_s.ExecuteNonQuery();
            }finally{ con_s.CerrarConexion(); con_s.Dispose();}
        }

        int id_caja, cantidad_restante;
        public void aprobar_transferencia_items(string id_salida){//BUSCAR SALIDAS ITEM DE ESTA SALIDA
            Conexion con = new Conexion();
            string codigo;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario,cantidad,codigo from salidas_items where id_salida='" + id_salida + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    codigo = Convert.ToString(leer["codigo"]);
                    id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    cantidad = Convert.ToInt32(leer["cantidad"]);
                    restar_inventario(id_inventario, cantidad);
                    if (codigo.Contains("caja")){
                        actualizar_caja_cantidad(codigo,cantidad); 
                    }else {
                        buscar_datos_cajas(id_inventario, cantidad);
                    }
                }leer.Close();
            }finally{ con.CerrarConexion(); con.Dispose(); }
        }
        public void actualizar_caja_cantidad(string codigo, int cantidad){
            string[] caja = codigo.Split('_');
            Conexion con_sri = new Conexion();
            try{
                SqlCommand com_sri = new SqlCommand();
                com_sri.Connection = con_sri.AbrirConexion();
                com_sri.CommandText = "UPDATE cajas_inventario SET cantidad_restante=cantidad_restante-" + cantidad + "  WHERE id_caja='" + caja[1] + "'  ";
                com_sri.ExecuteNonQuery();
            }finally{ con_sri.CerrarConexion(); con_sri.Dispose(); }
        }

        public void restar_inventario_cajas(int caja, int qty){
            Conexion con_sri = new Conexion();
            try{
                SqlCommand com_sri = new SqlCommand();
                com_sri.Connection = con_sri.AbrirConexion();
                com_sri.CommandText = "UPDATE cajas_inventario SET cantidad_restante=" + qty + "  WHERE id_caja='" + caja + "'  ";
                com_sri.ExecuteNonQuery();
            }finally{ con_sri.CerrarConexion(); con_sri.Dispose(); }
        }

        public void buscar_datos_cajas(int inventario, int cantidad){
            //BUSCAR SALIDAS ITEM DE ESTA SALIDA
            int cantidad_total = cantidad;
            Conexion con_bdc = new Conexion();
            try{
                SqlCommand com_bdc = new SqlCommand();
                SqlDataReader leer_bdc = null;
                com_bdc.Connection = con_bdc.AbrirConexion();
                com_bdc.CommandText = "SELECT id_caja,cantidad_restante from cajas_inventario where id_inventario='" + inventario + "' and cantidad_restante>0 order by id_caja ";
                leer_bdc = com_bdc.ExecuteReader();
                while (leer_bdc.Read()){
                    id_caja = Convert.ToInt32(leer_bdc["id_caja"]);
                    cantidad_restante = Convert.ToInt32(leer_bdc["cantidad_restante"]);
                    if (cantidad_restante <= cantidad_total){
                        cantidad_total -= cantidad_restante;
                        restar_inventario_cajas(id_caja, 0);
                    }else{
                        if (cantidad_total > 0){
                            cantidad_restante -= cantidad_total;
                            restar_inventario_cajas(id_caja, cantidad_restante);
                            cantidad_total = 0;
                        }
                    }
                }leer_bdc.Close();
            }finally{con_bdc.CerrarConexion(); con_bdc.Dispose();}
        }



        public void restar_inventario(int inventario, int qty)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total=total-" + qty + "  WHERE id_inventario='" + inventario + "'  ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion(); con_s.Dispose();}
        }
        public void negar_transferencia_inventario(string salida){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas SET estado_aprobacion=2 WHERE id_salida='" + salida + "'  ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion(); con_s.Dispose();}
        }
        public List<salidas> obtener_informacion_transferencia(string id_salida){
            List<salidas> listasalidas = new List<salidas>();
            Conexion con_oit = new Conexion();
            try{
                SqlCommand com_oit = new SqlCommand();
                SqlDataReader leer_oit = null;
                com_oit.Connection = con_oit.AbrirConexion();
                com_oit.CommandText = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_solicitud,driver,pallet,auto,placas from salidas where id_salida='" + id_salida + "' ";
                leer_oit = com_oit.ExecuteReader();
                while (leer_oit.Read()){
                    salidas l = new salidas();
                    List<salidas_item> items = new List<salidas_item>();
                    l.id_salida = Convert.ToInt32(leer_oit["id_salida"]);
                    l.fecha = (Convert.ToDateTime(leer_oit["fecha"])).ToString("MMM dd yyyy");
                    l.id_usuario = Convert.ToInt32(leer_oit["id_usuario"]);
                    l.id_origen = Convert.ToInt32(leer_oit["id_origen"]);
                    l.id_destino = Convert.ToInt32(leer_oit["id_destino"]);
                    l.estado_aprobacion = Convert.ToInt32(leer_oit["estado_aprobacion"]);
                    l.estado_entrega = Convert.ToInt32(leer_oit["estado_entrega"]);
                    l.sello = Convert.ToInt32(leer_oit["sello"]);
                    l.responsable = Convert.ToString(leer_oit["responsable"]);
                    l.id_envio = leer_oit["id_envio"].ToString();
                    l.fecha_solicitud = (Convert.ToDateTime(leer_oit["fecha_solicitud"])).ToString("MM-dd-yyyy");
                    l.driver = Convert.ToString(leer_oit["driver"]);
                    l.pallet = Convert.ToString(leer_oit["pallet"]);
                    l.total = Convert.ToInt32(leer_oit["total"]);
                    l.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer_oit["id_usuario"]));
                    l.origen = consultas.buscar_nombres_lugares(Convert.ToString(leer_oit["id_origen"]));
                    l.destino = consultas.buscar_nombres_lugares(Convert.ToString(leer_oit["id_destino"]));
                    l.auto= Convert.ToString(leer_oit["auto"]);
                    l.placas= Convert.ToString(leer_oit["placas"]);
                    items = buscar_lista_items_transferencia(id_salida);
                    l.lista_salidas_item = items;
                    listasalidas.Add(l);
                }leer_oit.Close();
            }finally{ con_oit.CerrarConexion(); con_oit.Dispose(); }
            return listasalidas;
        }
        /**************************************PDF TRANSFERENCIA***************************************/
        public IEnumerable<salidas> lista_transfer_ticket(int id_salida){
            List<salidas> listasalidas = new List<salidas>();
            Conexion con_oit = new Conexion();
            try{
                SqlCommand com_oit = new SqlCommand();
                SqlDataReader leer_oit = null;
                com_oit.Connection = con_oit.AbrirConexion();
                com_oit.CommandText = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_solicitud,driver,pallet,id_usuario_recibio,auto,placas from salidas where id_salida='" + id_salida + "' ";
                leer_oit = com_oit.ExecuteReader();
                while (leer_oit.Read()){
                    salidas l = new salidas();
                    List<salidas_item> items = new List<salidas_item>();
                    l.id_salida = Convert.ToInt32(leer_oit["id_salida"]);
                    l.fecha = (Convert.ToDateTime(leer_oit["fecha"])).ToString("MMM dd yyyy");
                    l.id_usuario = Convert.ToInt32(leer_oit["id_usuario"]);
                    l.id_origen = Convert.ToInt32(leer_oit["id_origen"]);
                    l.id_destino = Convert.ToInt32(leer_oit["id_destino"]);
                    l.estado_aprobacion = Convert.ToInt32(leer_oit["estado_aprobacion"]);
                    l.estado_entrega = Convert.ToInt32(leer_oit["estado_entrega"]);
                    l.sello = Convert.ToInt32(leer_oit["sello"]);
                    l.responsable = Convert.ToString(leer_oit["responsable"]);
                    l.id_envio = (leer_oit["id_envio"].ToString()).ToUpper();
                    l.fecha_solicitud = (Convert.ToDateTime(leer_oit["fecha_solicitud"])).ToString("MM-dd-yyyy");
                    l.driver = Convert.ToString(leer_oit["driver"]);
                    l.pallet = Convert.ToString(leer_oit["pallet"]);
                    l.total = Convert.ToInt32(leer_oit["total"]);
                    l.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer_oit["id_usuario"]));
                    l.recibio = consultas.buscar_nombre_usuario(Convert.ToString(leer_oit["id_usuario_recibio"]));
                    l.origen = consultas.buscar_nombres_lugares(Convert.ToString(leer_oit["id_origen"]));
                    l.destino = consultas.buscar_nombres_lugares(Convert.ToString(leer_oit["id_destino"]));
                    items = buscar_lista_items_transfer_ticket(id_salida.ToString());
                    l.lista_salidas_item = items;
                    l.auto = leer_oit["auto"].ToString();
                    l.placas= leer_oit["placas"].ToString();
                    l.direccion_destino = consultas.buscar_direccion_lugar(Convert.ToInt32(leer_oit["id_destino"]));
                    l.direccion_origen= consultas.buscar_direccion_lugar(Convert.ToInt32(leer_oit["id_origen"]));
                    listasalidas.Add(l);
                }leer_oit.Close();
            }finally{con_oit.CerrarConexion(); con_oit.Dispose();}
            return listasalidas;
        }
        //consultar_item
        public List<salidas_item> buscar_lista_items_transfer_ticket(string id_salida)
        {
            List<salidas_item> lista_items = new List<salidas_item>();
            Conexion connn = new Conexion();
            try{
                SqlCommand commm = new SqlCommand();
                SqlDataReader leerrr = null;
                commm.Connection = connn.AbrirConexion();
                commm.CommandText = "SELECT s.id_inventario,s.cantidad,i.mill_po,i.descripcion,s.id_inventario,s.id_pedido,s.id_estilo,s.cajas from salidas_items s,inventario i where s.id_salida='" + id_salida + "' and s.id_inventario=i.id_inventario";
                leerrr = commm.ExecuteReader();
                while (leerrr.Read()){
                    salidas_item l = new salidas_item();
                    l.id_inventario= Convert.ToInt32(leerrr["id_inventario"]);
                    l.cantidad = Convert.ToInt32(leerrr["cantidad"]);                   
                    l.po = consultas.buscar_po_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.estilo = consultas.obtener_estilo(Convert.ToInt32(leerrr["id_inventario"]));
                    l.summary = consultas.obtener_po_summary(Convert.ToInt32(leerrr["id_pedido"]), Convert.ToInt32(leerrr["id_estilo"]));
                    //List<Inventario> lista = new List<Inventario>();
                    l.lista_inventario = consultar_item_transfer(Convert.ToInt32(leerrr["id_inventario"]));
                    int categoria = consultas.buscar_categoria_item(Convert.ToInt32(leerrr["id_inventario"]));
                    if (categoria == 1){
                        l.descripcion = consultas.buscar_descripcion_estilo(Convert.ToInt32(leerrr["id_estilo"]));
                        if (l.descripcion == ""){
                            l.descripcion = consultas.buscar_descripcion_item(Convert.ToInt32(leerrr["id_inventario"]));
                        }
                    }else {
                        l.descripcion = consultas.buscar_descripcion_item(Convert.ToInt32(leerrr["id_inventario"]));
                    }
                    l.cajas = Convert.ToInt32(leerrr["cajas"]);

                    lista_items.Add(l);
                }
                leerrr.Close();
            }finally{ connn.CerrarConexion(); connn.Dispose(); }
            return lista_items;
        }

        public List<Inventario> consultar_item_transfer(int id){
            List<Inventario> lista = new List<Inventario>();
            Conexion con_ci = new Conexion();
            try{
                SqlCommand com_ci = new SqlCommand();
                SqlDataReader leer_ci = null;
                com_ci.Connection = con_ci.AbrirConexion();
                com_ci.CommandText = "SELECT id_estilo,id_inventario,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type," +
                    "id_location,total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,date_comment,comment,id_family_trim,id_unit,id_trim,stock,descripcion,id_item  " +
                    " from inventario  where id_inventario='" + id + "' ";
                leer_ci = com_ci.ExecuteReader();
                while (leer_ci.Read()){
                    Inventario i = new Inventario();
                    i.amt_item = consultas.buscar_amt_item(Convert.ToString(leer_ci["id_item"]));
                    i.id_inventario = Convert.ToInt32(leer_ci["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer_ci["id_pedido"]));
                    i.color = consultas.obtener_descripcion_color_id(Convert.ToString(leer_ci["id_color"]));
                    i.body_type = consultas.obtener_body_type_id(Convert.ToString(leer_ci["id_body_type"]));
                    i.genero = consultas.obtener_genero_id(Convert.ToString(leer_ci["id_genero"]));
                    i.fabric_type = consultas.obtener_fabric_type_id(Convert.ToString(leer_ci["id_fabric_type"]));
                    i.size = consultas.obtener_size_id(Convert.ToString(leer_ci["id_size"]));
                    lista.Add(i);
                }leer_ci.Close();
            }finally{con_ci.CerrarConexion(); con_ci.Dispose();}
            return lista;
        }

        public List<salidas_item> buscar_lista_items_transferencia(string id_salida){
            List<salidas_item> lista_items = new List<salidas_item>();
            Conexion connn = new Conexion();
            try{
                SqlCommand commm = new SqlCommand();
                SqlDataReader leerrr = null;
                commm.Connection = connn.AbrirConexion();
                commm.CommandText = "SELECT s.id_salida_item,s.id_inventario,s.cantidad,i.mill_po,i.descripcion,s.id_inventario,s.id_pedido,s.id_estilo,s.codigo,s.cajas from salidas_items s,inventario i where s.id_salida='" + id_salida + "' and s.id_inventario=i.id_inventario";
                leerrr = commm.ExecuteReader();
                while (leerrr.Read()){
                    salidas_item l = new salidas_item();
                    l.id_salida_item = Convert.ToInt32(leerrr["id_salida_item"]);
                    l.cantidad = Convert.ToInt32(leerrr["cantidad"]);
                    l.id_inventario = Convert.ToInt32(leerrr["id_inventario"]);
                    l.descripcion = consultas.buscar_descripcion_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.po = consultas.buscar_po_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.estilo = consultas.obtener_estilo(Convert.ToInt32(leerrr["id_inventario"]));
                    l.summary = consultas.obtener_po_summary(Convert.ToInt32(leerrr["id_pedido"]), Convert.ToInt32(leerrr["id_estilo"]));
                    l.codigo = Convert.ToString(leerrr["codigo"]);
                    l.total_inventario = consultas.buscar_total_inventario(l.id_inventario);
                    l.cajas= Convert.ToInt32(leerrr["cajas"]);
                    lista_items.Add(l);
                }leerrr.Close();
            }finally{ connn.CerrarConexion(); connn.Dispose(); }
            return lista_items;
        }

        public IEnumerable<Inventario> obtener_informacion_inventario(int id){
            List<Inventario> listInventario = new List<Inventario>();
            Conexion con_oii = new Conexion();
            try{
                SqlCommand com_oii = new SqlCommand();
                SqlDataReader leer_oii = null;
                com_oii.Connection = con_oii.AbrirConexion();
                com_oii.CommandText = "SELECT i.id_inventario,i.id_sucursal,i.id_pedido,i.mill_po,i.id_categoria_inventario,i.total,i.minimo,i.notas,i.descripcion,c.categoria,s.sucursal" +
                    " from inventario i,categorias_inventarios c,sucursales s where i.id_categoria_inventario=c.id_categoria and i.id_sucursal=s.id_sucursal and i.total!=0 and i.id_inventario='" + id + "' ";
                leer_oii = com_oii.ExecuteReader();
                while (leer_oii.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer_oii["id_inventario"]);
                    i.sucursal = consultas.obtener_po_id(Convert.ToString(leer_oii["id_sucursal"]));
                    listInventario.Add(i);
                }leer_oii.Close();
            }finally{con_oii.CerrarConexion(); con_oii.Dispose();}
            return listInventario;
        }

        public List<Inventario> obtener_item_editar(int id){
            List<Inventario> listInventario = new List<Inventario>();
            Conexion con_oie = new Conexion();
            try{
                SqlCommand com_oie = new SqlCommand();
                SqlDataReader leer_oie = null;
                com_oie.Connection = con_oie.AbrirConexion();
                com_oie.CommandText = "SELECT id_estilo,id_inventario,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type," +
                    "id_location,total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,date_comment,comment,id_family_trim,id_unit,id_trim " +
                    " from inventario  where id_inventario='" + id + "' ";
                leer_oie = com_oie.ExecuteReader();
                while (leer_oie.Read()){
                    Inventario i = new Inventario();
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
                    listInventario.Add(i);
                }leer_oie.Close();
            }finally{con_oie.CerrarConexion(); con_oie.Dispose();}
            return listInventario;
        }

        public salidas obtener_datos_cambio_sello(string id_salida){
            salidas s = new salidas();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_sucursal,id_sello,sello from salidas where id_salida='" + id_salida + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    s.id_sucursal = Convert.ToInt32(leer["id_sucursal"]);
                    s.id_sello = Convert.ToInt32(leer["id_sello"]);
                    s.sello = Convert.ToInt32(leer["sello"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return s;
        }
        public void cambiar_sello(int salida, int sello, int id_sello){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas SET sello='" + sello + "',id_sello='" + id_sello + "' WHERE id_salida='" + salida + "' ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion(); con_s.Dispose();}
        }
        public IEnumerable<salidas> lista_transferencias_por_recibir(string sucursal){
            List<salidas> listalugares = new List<salidas>();
            Conexion con_ltpr = new Conexion();
            try{
                SqlCommand com_ltpr = new SqlCommand();
                SqlDataReader leer_ltpr = null;
                com_ltpr.Connection = con_ltpr.AbrirConexion();
                com_ltpr.CommandText = "SELECT id_salida,fecha,total,id_usuario,id_origen,id_destino,estado_aprobacion,estado_entrega,sello,responsable,id_envio,fecha_solicitud,driver,pallet from salidas where estado_entrega=0 and estado_aprobacion=1 and id_destino='" + sucursal + "' ";
                leer_ltpr = com_ltpr.ExecuteReader();
                while (leer_ltpr.Read()){
                    salidas l = new salidas();
                    l.id_salida = Convert.ToInt32(leer_ltpr["id_salida"]);
                    l.fecha = (Convert.ToDateTime(leer_ltpr["fecha"])).ToString("MMM dd yyyy");
                    l.id_usuario = Convert.ToInt32(leer_ltpr["id_usuario"]);
                    l.id_origen = Convert.ToInt32(leer_ltpr["id_origen"]);
                    l.id_destino = Convert.ToInt32(leer_ltpr["id_destino"]);
                    l.estado_aprobacion = Convert.ToInt32(leer_ltpr["estado_aprobacion"]);
                    l.estado_entrega = Convert.ToInt32(leer_ltpr["estado_entrega"]);
                    l.sello = Convert.ToInt32(leer_ltpr["sello"]);
                    l.responsable = Convert.ToString(leer_ltpr["responsable"]);
                    l.id_envio = leer_ltpr["id_envio"].ToString();
                    l.fecha_solicitud = (Convert.ToDateTime(leer_ltpr["fecha_solicitud"])).ToString("MM-dd-yyyy");
                    l.driver = Convert.ToString(leer_ltpr["driver"]);
                    l.pallet = Convert.ToString(leer_ltpr["pallet"]);
                    l.total = Convert.ToInt32(leer_ltpr["total"]);
                    l.usuario = consultas.buscar_nombre_usuario(Convert.ToString(leer_ltpr["id_usuario"]));
                    l.origen = consultas.buscar_nombres_lugares(Convert.ToString(leer_ltpr["id_origen"]));
                    l.destino = consultas.buscar_nombres_lugares(Convert.ToString(leer_ltpr["id_destino"]));
                    listalugares.Add(l);
                }leer_ltpr.Close();
            }finally{con_ltpr.CerrarConexion(); con_ltpr.Dispose();}
            return listalugares;
        }
        public Inventario consultar_item(int id){
            Inventario i = new Inventario();
            Conexion con_ci = new Conexion();
            try{
                SqlCommand com_ci = new SqlCommand();
                SqlDataReader leer_ci = null;
                com_ci.Connection = con_ci.AbrirConexion();
                com_ci.CommandText = "SELECT id_item,id_estilo,id_inventario,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type," +
                    "id_location,total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,date_comment,comment,id_family_trim,id_unit,id_trim,stock,descripcion " +
                    " from inventario  where id_inventario='" + id + "' ";
                leer_ci = com_ci.ExecuteReader();
                while (leer_ci.Read()){
                    i.id_inventario = Convert.ToInt32(leer_ci["id_inventario"]);
                    i.id_pedido = Convert.ToInt32(leer_ci["id_pedido"]);
                    i.id_pedido = Convert.ToInt32(leer_ci["id_pedido"]);
                    i.id_categoria_inventario = Convert.ToInt32(leer_ci["id_categoria_inventario"]);
                    i.minimo = Convert.ToInt32(leer_ci["minimo"]);
                    i.notas = Convert.ToString(leer_ci["notas"]);
                    i.id_pais = Convert.ToInt32(leer_ci["id_pais"]);
                    i.id_fabricante = Convert.ToInt32(leer_ci["id_fabricante"]);
                    i.id_color = Convert.ToInt32(leer_ci["id_color"]);
                    i.id_body_type = Convert.ToInt32(leer_ci["id_body_type"]);
                    i.id_genero = Convert.ToInt32(leer_ci["id_genero"]);
                    i.id_fabric_type = Convert.ToInt32(leer_ci["id_fabric_type"]);
                    i.id_location = Convert.ToInt32(leer_ci["id_location"]);
                    i.total = Convert.ToInt32(leer_ci["total"]);
                    i.id_size = Convert.ToInt32(leer_ci["id_size"]);
                    i.id_customer = Convert.ToInt32(leer_ci["id_customer"]);
                    i.id_final_customer = Convert.ToInt32(leer_ci["id_customer_final"]);
                    i.id_fabric_percent = Convert.ToInt32(leer_ci["id_fabric_percent"]);
                    string[] fecha = (Convert.ToString(leer_ci["date_comment"])).Split(' ');
                    i.date_comment = fecha[0];
                    i.comment = Convert.ToString(leer_ci["comment"]);
                    i.id_family_trim = Convert.ToString(leer_ci["id_family_trim"]);
                    i.id_unit = Convert.ToString(leer_ci["id_unit"]);
                    i.id_trim = Convert.ToInt32(leer_ci["id_trim"]);
                    i.id_estilo = Convert.ToInt32(leer_ci["id_estilo"]);
                    i.stock = Convert.ToString(leer_ci["stock"]);
                    i.descripcion = Convert.ToString(leer_ci["descripcion"]);
                    i.id_estilo = Convert.ToInt32(leer_ci["id_estilo"]);
                    i.id_item= Convert.ToInt32(leer_ci["id_item"]);
                    //listInventario.Add(i);
                }
                leer_ci.Close();
            }finally{con_ci.CerrarConexion(); con_ci.Dispose();}
            return i;
        }
        public int comparar_inventario(Inventario i, int destino){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario FROM inventario WHERE id_sucursal='" + destino + "' and id_pedido='" + i.id_pedido + "' and id_pais='" + i.id_pais + "' and id_fabricante='" + i.id_fabricante + "' " +
                    " and id_categoria_inventario='" + i.id_categoria_inventario + "' and id_color='" + i.id_color + "' and id_body_type='" + i.id_body_type + "' and id_genero='" + i.id_genero + "' and " +
                    "  id_fabric_type='" + i.id_fabric_type + "' and id_location='" + i.id_location + "' and id_size='" + i.id_size + "' and id_customer='" + i.id_customer + "' and id_customer_final='" + i.id_final_customer + "' " +
                    " and id_fabric_percent='" + i.id_fabric_percent + "' and id_family_trim='" + i.id_family_trim + "' and id_unit='" + i.id_unit + "'  and id_trim='" + i.id_trim + "' and id_inventario!='"+i.id_inventario+"'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_inventario"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public void agregar_inventario_desde_transferencia(Inventario i, int sucursal, int total){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO inventario(id_sucursal,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type,id_location," +
                    "total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,stock,date_comment,comment,id_family_trim,id_unit,id_trim,descripcion,id_estilo)  " +
                    " VALUES('" + sucursal + "','" + i.id_pedido + "','" + i.id_pais + "','" + i.id_fabricante + "','" + i.id_categoria_inventario + "','" + i.id_color + "','" + i.id_body_type + "'," +
                    " '" + i.id_genero + "','" + i.id_fabric_type + "','" + i.id_location + "','" + total + "','" + i.id_size + "','" + i.id_customer + "', '" + i.id_final_customer + "','" + i.minimo + "','" + i.notas + "'" +
                    " ,'" + i.id_fabric_percent + "','" + i.stock + "','" + i.date_comment + "','" + i.comment + "','" + i.id_family_trim + "','" + i.id_unit + "','" + i.id_trim + "','" + i.descripcion + "','" + i.id_estilo + "'  )";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion(); con_s.Dispose();}
        }
        public void actualizar_transferencia(int usuario, int salida)
        {
            Conexion con_s = new Conexion();
            try
            {
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas SET id_usuario_recibio='" + usuario + "',fecha_recibo='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',estado_entrega=1 WHERE id_salida='" + salida + "' ";
                com_s.ExecuteNonQuery();
            }
            finally
            {
                con_s.CerrarConexion(); con_s.Dispose();
            }
        }

        public int revisar_existencia_lugar(string nombre){
            int tempo = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_lugar from lugares where lugar='" +nombre+ "'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    tempo = Convert.ToInt32(leer["id_lugar"]);
                }
                leer.Close();
            }finally{ con.CerrarConexion(); con.Dispose();}
            return tempo;
        }

        public void guardar_nuevo_lugar(string nombre, string direccion,string tipo)
        {
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                com.Connection = con.AbrirConexion();
                com.CommandText = "INSERT INTO lugares(lugar,tipo_lugar,id_sucursal,direccion) VALUES('"+nombre+"','"+tipo+"','0','"+direccion+"')";
                com.ExecuteNonQuery();
            }
            finally { con.CerrarConexion(); con.Dispose();}
        }

        public int buscar_tipo_salida(int salida){
            int temporal = 0;
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                SqlDataReader leer = null;
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "select l.tipo_lugar from lugares l,salidas s where s.id_destino=l.id_lugar and s.id_salida='"+salida+"' ";
                leer = com_s.ExecuteReader();
                while (leer.Read()){
                    temporal = Convert.ToInt32(leer["tipo_lugar"]);
                }
                leer.Close();
            }
            finally{ con_s.CerrarConexion(); con_s.Dispose(); }
            return temporal;
        }


        public String obtener_lista_grafica()
        {
            Conexion con = new Conexion();
            string Lista = Convert.ToString(grafica_hoy()) + "*" + Convert.ToString(grafica_ayer()) + "*" + Convert.ToString(grafica_semana()) + "*" + Convert.ToString(grafica_mes()) + "*" + Convert.ToString(grafica_year());
            return Lista;
        }

        public int grafica_hoy()
        {
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from recibos where fecha between '" + fecha[0] + " 00:00:00' and '" + fecha[0] + " 23:59:59'  ";
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

        public int grafica_ayer()
        {
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from recibos where fecha>= dateadd(day,datediff(day,1,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
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
        public int grafica_semana()
        {
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from recibos where fecha>= dateadd(day,datediff(day,7,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
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
        public int grafica_mes()
        {
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                //com.CommandText = "SELECT total from staging where fecha>= dateadd(day,datediff(day,30,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
                com.CommandText = "SELECT total from recibos where YEAR(fecha)=YEAR('" + DateTime.Now + "') and MONTH(fecha)=MONTH('" + DateTime.Now + "')";
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
        public int grafica_year()
        {
            Conexion con = new Conexion();
            int Lista = 0;
            try
            {
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from recibos where YEAR(fecha)=YEAR('" + DateTime.Now + "')";
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
        public String obtener_lista_grafica_transferencias(){
            Conexion con = new Conexion();
            string Lista = Convert.ToString(grafica_hoy_trans()) + "*" + Convert.ToString(grafica_ayer_trans()) + "*" + Convert.ToString(grafica_semana_trans()) + "*" + Convert.ToString(grafica_mes_trans()) + "*" + Convert.ToString(grafica_year_trans());
            return Lista;
        }
        public int grafica_hoy_trans(){
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from salidas where fecha between '" + fecha[0] + " 00:00:00' and '" + fecha[0] + " 23:59:59'  ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public int grafica_ayer_trans(){
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from salidas where fecha>= dateadd(day,datediff(day,1,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public int grafica_semana_trans(){
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from salidas where fecha>= dateadd(day,datediff(day,7,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public int grafica_mes_trans(){
            string[] fecha = (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Split(' ');
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                //com.CommandText = "SELECT total from staging where fecha>= dateadd(day,datediff(day,30,GETDATE()),0) and fecha< dateadd(day,datediff(day,0,GETDATE()),0)";
                com.CommandText = "SELECT total from salidas where YEAR(fecha)=YEAR('" + DateTime.Now + "') and MONTH(fecha)=MONTH('" + DateTime.Now + "')";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public int grafica_year_trans(){
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT total from salidas where YEAR(fecha)=YEAR('" + DateTime.Now + "')";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista += Convert.ToInt32(leer["total"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        /*QR PRUEBA*//*QR PRUEBA*/
        /*QR PRUEBA*//*QR PRUEBA*/
        public List<int> qrs(){
            List<int> listalugares = new List<int>();
            Conexion con_lt = new Conexion();
            try{
                SqlCommand com_lt = new SqlCommand();
                SqlDataReader leer_lt = null;
                com_lt.Connection = con_lt.AbrirConexion();
                com_lt.CommandText = "SELECT id_ubicacion,ubicacion from ubicaciones ";
                leer_lt = com_lt.ExecuteReader();
                while (leer_lt.Read()){
                    listalugares.Add(Convert.ToInt32(leer_lt["id_ubicacion"]));
                }leer_lt.Close();
            }finally{con_lt.CerrarConexion(); con_lt.Dispose();}
            return listalugares;
        }
        /*QR PRUEBA*//*QR PRUEBA*/
        /*QR PRUEBA*//*QR PRUEBA*//*QR PRUEBA*/
        public List<Inventario> buscar_lista_productos_ubicacion(string ubicacion){
            List<Inventario> listInventario = new List<Inventario>();
            Conexion con_ois = new Conexion();
            try{
                SqlCommand com_ois = new SqlCommand();
                SqlDataReader leer_ois = null;
                com_ois.Connection = con_ois.AbrirConexion();
                com_ois.CommandText = "SELECT id_inventario,id_pedido,total,descripcion,id_sucursal " +
                    " from inventario where id_location='"+ubicacion+"' ";
                leer_ois = com_ois.ExecuteReader();
                while (leer_ois.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer_ois["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer_ois["id_pedido"]));
                    i.total = Convert.ToInt32(leer_ois["total"]);
                    i.descripcion = Convert.ToString(leer_ois["descripcion"]);
                    i.id_sucursal= Convert.ToInt32(leer_ois["id_sucursal"]);
                    listInventario.Add(i);
                }leer_ois.Close();
            }finally{ con_ois.CerrarConexion(); con_ois.Dispose(); }
            return listInventario;
        }

        public List<Inventario> buscar_lista_productos_cajas(string caja){
            List<Inventario> listInventario = new List<Inventario>();
            Conexion con_ois = new Conexion();
            try{
                SqlCommand com_ois = new SqlCommand();
                SqlDataReader leer_ois = null;
                com_ois.Connection = con_ois.AbrirConexion();
                com_ois.CommandText = "SELECT i.id_inventario,i.id_pedido,i.descripcion,i.id_sucursal,ci.cantidad_restante " +
                    " from inventario i,cajas_inventario ci where ci.id_caja='" + caja + "' and i.id_inventario=ci.id_inventario ";
                leer_ois = com_ois.ExecuteReader();
                while (leer_ois.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leer_ois["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leer_ois["id_pedido"]));
                    i.total = Convert.ToInt32(leer_ois["cantidad_restante"]);
                    i.descripcion = Convert.ToString(leer_ois["descripcion"]);
                    i.id_sucursal = Convert.ToInt32(leer_ois["id_sucursal"]);
                    listInventario.Add(i);
                }leer_ois.Close();
            }finally { con_ois.CerrarConexion(); con_ois.Dispose(); }
            return listInventario;
        }

        public void cambiar_id_inventario_caja(int inventario, string caja){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE cajas_inventario SET id_inventario='" + inventario + "' WHERE id_caja='" + caja + "' ";
                com_s.ExecuteNonQuery();
            }finally{ con_s.CerrarConexion(); con_s.Dispose(); }
        }

        public int obtener_contenido_caja(int caja){
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT cantidad_restante from cajas_inventario where id_caja='"+caja+"'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista = Convert.ToInt32(leer["cantidad_restante"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        //agregar_id_inventario_nuevo_transferencia
        public void agregar_id_inventario_nuevo_transferencia(int salida, int inventario){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE salidas_items SET id_inventario_nuevo='" + inventario + "' WHERE id_salida_item='" + salida + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        public string buscar_estilo_inventario(int inventario){
            Conexion con = new Conexion();
            string Lista = "";
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_estilo from inventario where id_inventario='" + inventario + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista = consultas.obtener_estilo(Convert.ToInt32(leer["id_estilo"]));
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Lista;
        }
        public string buscar_cajas_inventario(int inventario){
            Conexion con = new Conexion();
            int Lista = 0;
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_caja from cajas_inventario where id_inventario='" + inventario + "' and cantidad_restante>=1";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    Lista++;
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return Convert.ToString(Lista);
        }
        public salidas_item obtener_datos_salida_item(int salida){
            salidas_item l = new salidas_item();
            Conexion connn = new Conexion();
            try{
                SqlCommand commm = new SqlCommand();
                SqlDataReader leerrr = null;
                commm.Connection = connn.AbrirConexion();
                commm.CommandText = "SELECT s.id_salida_item,s.id_inventario,s.cantidad,i.mill_po,i.descripcion,s.id_inventario,s.id_pedido,s.id_estilo,s.codigo,s.cajas from salidas_items s,inventario i where s.id_salida_item='" + salida + "' and s.id_inventario=i.id_inventario";
                leerrr = commm.ExecuteReader();
                while (leerrr.Read()){
                    l.id_salida_item = Convert.ToInt32(leerrr["id_salida_item"]);
                    l.cantidad = Convert.ToInt32(leerrr["cantidad"]);
                    l.id_inventario = Convert.ToInt32(leerrr["id_inventario"]);
                    /*l.descripcion = consultas.buscar_descripcion_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.po = consultas.buscar_po_item(Convert.ToInt32(leerrr["id_inventario"]));
                    l.estilo = consultas.obtener_estilo(Convert.ToInt32(leerrr["id_inventario"]));
                    l.summary = consultas.obtener_po_summary(Convert.ToInt32(leerrr["id_pedido"]), Convert.ToInt32(leerrr["id_estilo"]));
                    l.codigo = Convert.ToString(leerrr["codigo"]);
                    l.total_inventario = consultas.buscar_total_inventario(l.id_inventario);
                    l.cajas = Convert.ToInt32(leerrr["cajas"]);*/
                }leerrr.Close();
            }finally { connn.CerrarConexion(); connn.Dispose(); }
            return l;
        }

        public void regresar_datos_cajas(int inventario, int cantidad)        {
            //BUSCAR SALIDAS ITEM DE ESTA SALIDA
            int cantidad_total = cantidad;
            int cantidad_inicial;
            Conexion con_bdc = new Conexion();
            try{
                SqlCommand com_bdc = new SqlCommand();
                SqlDataReader leer_bdc = null;
                com_bdc.Connection = con_bdc.AbrirConexion();
                com_bdc.CommandText = "SELECT id_caja,cantidad_restante,cantidad_inicial from cajas_inventario where id_inventario='" + inventario + "' and cantidad_restante!=cantidad_inicial order by id_caja DESC";
                leer_bdc = com_bdc.ExecuteReader();
                while (leer_bdc.Read()){
                    id_caja = Convert.ToInt32(leer_bdc["id_caja"]);
                    cantidad_restante = Convert.ToInt32(leer_bdc["cantidad_restante"]);
                    cantidad_inicial= Convert.ToInt32(leer_bdc["cantidad_inicial"]);
                    if (cantidad_total >cantidad_inicial){
                        restar_inventario_cajas(id_caja, cantidad_inicial);
                        cantidad_total = cantidad_total - (cantidad_inicial-cantidad_restante);
                    }else{
                        if (cantidad_total > 0){
                            restar_inventario_cajas(id_caja, (cantidad_total+cantidad_restante));
                            cantidad_total = 0;
                        }
                    }
                }leer_bdc.Close();
            }finally { con_bdc.CerrarConexion(); con_bdc.Dispose(); }
        }
        public void eliminar_salida_item(int salida_item){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "DELETE FROM salidas_items WHERE id_salida_item='" + salida_item + "' ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }


















































    }
}