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
using System.Threading;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.QrCode;


namespace FortuneSystem.Models.Almacen
{
    public class DatosInventario
    {
        public int id_estilo, id_inventario, id_tipo, id_recibo, id_amt, id_unit, id_company, cantidad, id_familia, minimo, id_pedido, id_sucursal, id_usuario;
        public int id_pais, total, minimo_trim, id_fabric_percent, id_fabricante, id_color, id_yarn, id_body_type, id_size, id_gender, id_fabric_type, id_percent, id_quantity, purchased, id_ubicacion, id_customer, id_customer_final, id_recibo_item, id_trim;
        public string descripcion, po_referencia, yarn, item, division;
        public string mill_po, amt, po, date_comment, comments, notas, cajas, cantidades, amt_item, notas_item, comment_item, color_aux;
        public int quantity = 0, id_caja, id_item;
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        //**************************************************************************
        public List<Inventario> ListaInventario(string busqueda){
            string query;
            if (busqueda != "") {
                query = "(SELECT top 30 i.id_inventario,i.id_sucursal,i.id_pedido,i.id_categoria_inventario,i.total,i.minimo,i.notas,i.descripcion" +
                    " from inventario i,PEDIDO p where i.descripcion like '%" + busqueda + "%' and p.ID_STATUS!=6 and p.ID_STATUS!=7 and i.id_pedido=p.ID_PEDIDO ) " +
                    " UNION (SELECT top 30 i.id_inventario,i.id_sucursal,i.id_pedido,i.id_categoria_inventario,i.total,i.minimo,i.notas,i.descripcion" +
                    " from inventario i,PEDIDO p where p.PO like '%" + busqueda + "%' and i.id_pedido=p.ID_PEDIDO and p.ID_STATUS!=6 and p.ID_STATUS!=7 and i.id_pedido=p.ID_PEDIDO  )" +
                    " UNION (SELECT top 30 i.id_inventario,i.id_sucursal,i.id_pedido,i.id_categoria_inventario,i.total,i.minimo,i.notas,i.descripcion" +
                    " from inventario i,ITEM_DESCRIPTION ides,PEDIDO p where ides.ITEM_ID=i.id_estilo and ides.DESCRIPTION like '%" + busqueda + "%' and p.ID_STATUS!=6 and p.ID_STATUS!=7 and i.id_pedido=p.ID_PEDIDO ) " +
                    " UNION (SELECT top 30 i.id_inventario,i.id_sucursal,i.id_pedido,i.id_categoria_inventario,i.total,i.minimo,i.notas,i.descripcion" +
                    " from inventario i,ITEM_DESCRIPTION ides,PEDIDO p where ides.ITEM_ID=i.id_estilo and ides.ITEM_STYLE like '%" + busqueda + "%' and p.ID_STATUS!=6 and p.ID_STATUS!=7 and i.id_pedido=p.ID_PEDIDO ) ";                    
            } else {
                query = "SELECT top 100 i.id_inventario,i.id_sucursal,i.id_pedido,i.id_categoria_inventario,i.total,i.minimo,i.notas,i.descripcion,c.categoria,s.sucursal" +
                    " from inventario i,categorias_inventarios c,sucursales s where i.id_categoria_inventario=c.id_categoria and i.id_sucursal=s.id_sucursal ";
            }
            List<Inventario> listInventario = new List<Inventario>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = query;          
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Inventario i = new Inventario();
                    i.id_inventario = Convert.ToInt32(leerFilas["id_inventario"]);
                    i.po = consultas.obtener_po_id(Convert.ToString(leerFilas["id_pedido"]));
                    i.categoria_inventario = consultas.obtener_categoria_inventario_id(Convert.ToString(leerFilas["id_categoria_inventario"]));
                    i.total = Convert.ToInt32(leerFilas["total"]);
                    i.sucursal = consultas.obtener_sucursal_id(Convert.ToString(leerFilas["id_sucursal"]));
                    i.minimo = Convert.ToInt32(leerFilas["minimo"]);
                    i.descripcion = Convert.ToString(leerFilas["descripcion"]);
                    if ((Convert.ToString(leerFilas["id_categoria_inventario"])) == "1"){
                        if (i.total > 0) { i.estado = "STOCK"; } else { i.estado = "UNAVAILABLE"; }
                    }else{
                        if (i.total >= i.minimo){
                            i.estado = "STOCK";
                        }else{
                            i.estado = Convert.ToString(i.minimo - i.total);
                        }
                    }listInventario.Add(i);
                }leerFilas.Close();
            }finally{ conn.CerrarConexion(); conn.Dispose();}
            return listInventario;
        }
        public void obtener_datos_trim(int item, int usuario, string estilo, string tipo, string po, string unit, string company, string cantidad, string descripcion_trim, string familia, string minimo)
        {
            id_item = item;
            id_estilo = consultas.obtener_estilo_id(estilo);
            id_tipo = consultas.buscar_tipo_inventario(tipo);
            id_unit = consultas.buscar_unit(unit);
            if (id_unit == 0){
                consultas.crear_unidad(unit);
                id_unit = consultas.buscar_unit(unit);
            }
            id_customer = consultas.buscar_customer(company);
            id_familia = consultas.buscar_familia_trim(familia);
            if (id_familia == 0){
                consultas.crear_familia_trim(familia);
                id_familia = consultas.buscar_familia_trim(familia);
            }
            id_pedido = consultas.buscar_pedido(po);
            id_usuario = usuario;
            id_sucursal = consultas.buscar_id_sucursal_usuario(id_usuario);
            id_trim = consultas.buscar_trim(descripcion_trim);
            if (id_trim == 0){
                consultas.crear_trim(descripcion_trim);
                id_trim = consultas.buscar_trim(descripcion_trim);
            }
            descripcion = unit + " " + descripcion_trim + " " + familia;
            descripcion = Regex.Replace(descripcion, @"\s+", " ");
            total = Convert.ToInt32(cantidad);
            minimo_trim = Convert.ToInt32(minimo);
        }
        /***********TRIMS******************************************************************************************************************************************************************************/

        public void guardar_trim_po(){
            Conexion conTrim = new Conexion();
            try{
                SqlCommand comTrim = new SqlCommand();
                comTrim.Connection = conTrim.AbrirConexion();
                comTrim.CommandText = "INSERT INTO inventario(id_estilo,id_sucursal,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type,id_location,total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,stock,date_comment,comment,id_family_trim,id_unit,id_trim,descripcion,id_item) " +
                "values('"+id_estilo+"','" + id_sucursal + "','" + id_pedido + "','0','0','" + id_tipo + "','0','0','0','0','0','" + total + "','0','" + id_customer + "','0','" + minimo_trim + "','N/A','0','N/A','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','N/A','" + id_familia + "','" + id_unit + "','" + id_trim + "','" + descripcion + "','" + id_item + "' ) ";
                comTrim.ExecuteNonQuery();
            }finally{conTrim.CerrarConexion();conTrim.Dispose();}
        }
        public int obtener_ultimo_inventario(){
            Conexion con_u_i = new Conexion();
            try{
                SqlCommand com_u_i = new SqlCommand();
                SqlDataReader leer_u_i = null;
                com_u_i .Connection = con_u_i.AbrirConexion();
                com_u_i.CommandText = "SELECT TOP 1 id_inventario FROM inventario order by id_inventario desc ";
                leer_u_i = com_u_i.ExecuteReader();
                while (leer_u_i.Read()){
                    id_inventario = Convert.ToInt32(leer_u_i["id_inventario"]);
                }leer_u_i.Close();
            }finally{con_u_i.CerrarConexion(); con_u_i.Dispose();            }
            return id_inventario;
        }
        public void guardar_recibo(int cantidad, int id_customer, string mill, string referencia){
            Conexion con_r = new Conexion();
            try{
                SqlCommand com_r = new SqlCommand();
                com_r.Connection = con_r.AbrirConexion();
                com_r.CommandText = "INSERT INTO recibos(fecha,total,id_usuario,id_sucursal,id_origen,mp_number,mill_po,po_reference) values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + cantidad + "','" + id_usuario + "','" + id_sucursal + "','" + id_company + "','n/a','" + mill + "','" + referencia + "') ";
                com_r.ExecuteNonQuery();
            }finally{con_r.CerrarConexion(); con_r.Dispose();}
        }
        public int obtener_ultimo_recibo(){
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT TOP 1 id_recibo FROM recibos order by id_recibo desc ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    id_recibo = Convert.ToInt32(leer_u_r["id_recibo"]);
                }leer_u_r.Close();
            }finally{con_u_r.CerrarConexion(); con_u_r.Dispose();}
            return id_recibo;
        }
        public int obtener_ultimo_recibo_item(){
            Conexion con_u_r_i = new Conexion();
            try{
                SqlCommand com_u_r_i = new SqlCommand();
                SqlDataReader leer_u_r_i = null;
                com_u_r_i.Connection = con_u_r_i.AbrirConexion();
                com_u_r_i.CommandText = "SELECT TOP 1 id_recibo_item FROM recibos_items order by id_recibo_item desc ";
                leer_u_r_i = com_u_r_i.ExecuteReader();
                while (leer_u_r_i.Read()){
                    id_recibo_item = Convert.ToInt32(leer_u_r_i["id_recibo_item"]);
                }leer_u_r_i.Close();
            }finally{con_u_r_i.CerrarConexion();con_u_r_i.Dispose();}
            return id_recibo_item;
        }
        public int obtener_id_ultima_caja(){
            Conexion con_u_r_i = new Conexion();
            try{
                SqlCommand com_u_r_i = new SqlCommand();
                SqlDataReader leer_u_r_i = null;
                com_u_r_i.Connection = con_u_r_i.AbrirConexion();
                com_u_r_i.CommandText = "SELECT TOP 1 id_caja FROM cajas_inventario order by id_caja desc ";
                leer_u_r_i = com_u_r_i.ExecuteReader();
                while (leer_u_r_i.Read()){
                    id_caja = Convert.ToInt32(leer_u_r_i["id_caja"]);
                }leer_u_r_i.Close();
            }finally{con_u_r_i.CerrarConexion(); con_u_r_i.Dispose();}
            return id_caja;
        }
        public void guardar_recibo_item(int id_recibo, string id_inventario, string cantidad){
            Conexion con_r_i = new Conexion();
            try{
                SqlCommand com_r_i = new SqlCommand();
                com_r_i.Connection = con_r_i.AbrirConexion();
                com_r_i.CommandText = "INSERT INTO recibos_items(id_recibo,id_inventario,total) values('" + id_recibo + "','" + id_inventario + "','" + cantidad + "') ";
                com_r_i.ExecuteNonQuery();
            }finally{con_r_i.CerrarConexion();con_r_i.Dispose();}
        }
        public int buscar_existencia_trim_inventario(){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario from inventario where id_categoria_inventario='" + id_tipo + "' and id_trim='" + id_trim + "' and id_family_trim='" + id_familia + "' and id_unit='" + id_unit + "' and id_sucursal='" + id_sucursal + "' and id_pedido='"+id_pedido+"'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_inventario"]);
                }leer.Close();
            }finally{con.CerrarConexion();con.Dispose();}
            return temp;
        }
        public void sumar_existencia_trim(int inventario){
            id_inventario = inventario;
            update_stock(id_inventario, Convert.ToInt32(cantidad), id_sucursal);
        }
        public void update_stock(int inventario, int cantidad, int sucursal){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total=(total+'" + cantidad + "') WHERE id_inventario='" + inventario + "' and id_sucursal='" + sucursal + "' ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion();con_s.Dispose();}
        }
        public void update_inventario(int inventario, int cantidad){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE inventario SET total=(total+'" + cantidad + "') WHERE id_inventario='" + inventario + "'  ";
                com_s.ExecuteNonQuery();
            }finally { con_s.CerrarConexion(); con_s.Dispose(); }
        }
        /***********BLANKS******************************************************************************************************************************************************************************/
        public void obtener_datos_blank(int item, int usuario, string estilo, string tipo, string po, string pais, string fabricante, string color, string body_type, string size, string gender, string fabric_type, string percent, string customer, string location, int customer_final, string datecoment, string comments, string notas, string cajas, string cantidades)
        {
            quantity = 0;
            id_item = item;
            id_estilo = consultas.obtener_estilo_id(estilo);
            id_tipo = consultas.buscar_tipo_inventario(tipo);
            id_pedido = consultas.buscar_pedido(po);
            id_usuario = usuario;
            id_sucursal = consultas.buscar_id_sucursal_usuario(id_usuario);
            id_pais = consultas.buscar_id_pais(pais);
            if (id_pais == 0){
                consultas.crear_pais(pais);
                id_pais = consultas.buscar_id_pais(pais);
            }
            id_fabricante = consultas.buscar_fabricante(fabricante);
            if (id_fabricante == 0){
                consultas.crear_fabricante(fabricante);
                id_fabricante = consultas.buscar_fabricante(fabricante);
            }
            id_color = consultas.buscar_color_codigo(color);
            id_body_type = consultas.buscar_body_type(body_type);
            if (id_body_type == 0){
                consultas.crear_body_type(body_type);
                id_body_type = consultas.buscar_body_type(body_type);
            }
            id_size = consultas.buscar_talla(size);
            id_gender = consultas.buscar_genero(gender);
            id_fabric_type = consultas.buscar_fabric_type(fabric_type);

            id_percent = consultas.buscar_percent(percent);
            id_ubicacion = consultas.buscar_ubicacion(location);
            if (id_ubicacion == 0){
                consultas.crear_ubicacion(location);
                id_ubicacion = consultas.buscar_ubicacion(location);
            }
            id_customer = consultas.buscar_customer(customer);
            id_customer_final = customer_final;
            date_comment = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //if (date_comment == "N/A") { date_comment = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); } else { date_comment = date_comment + " 00:00:00"; }
            string[] caja = cajas.Split('&'), cantidad = cantidades.Split('&');
            for (int i = 1; i < (caja.Length); i++){
                quantity += (Convert.ToInt32(cantidad[i]) * Convert.ToInt32(caja[i]));
            }
            descripcion = pais + " " + fabricante + " " + color + " " + body_type + " " + size + " " + gender + " " + fabric_type + " " + percent;
            descripcion = Regex.Replace(descripcion, @"\s+", " ");
            if (notas != "N/A"){
                notas_item = notas;
            }
            comment_item = comments;
        }
        public void guardar_caja(int recibo_item, string inventario, string cantidad_inicial, string cantidad_restante){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO cajas_inventario(id_recibo_item,id_inventario,cantidad_inicial,cantidad_restante) VALUES('" + recibo_item + "','" + inventario + "','" + cantidad_inicial + "','" + cantidad_restante + "') ";
                com_c.ExecuteNonQuery();
            }finally{con_c.CerrarConexion();con_c.Dispose();}
        }
        public void guardar_blank(){
            Conexion con_b = new Conexion();
            try{
                SqlCommand com_b = new SqlCommand();
                //SqlDataReader leer_s = null;
                com_b.Connection = con_b.AbrirConexion();
                com_b.CommandText = "INSERT INTO inventario(id_estilo,id_sucursal,id_pedido,id_pais,id_fabricante,id_categoria_inventario,id_color,id_body_type,id_genero,id_fabric_type,id_location,total,id_size,id_customer,id_customer_final,minimo,notas,id_fabric_percent,stock,date_comment,comment,id_family_trim,id_unit,id_trim,descripcion,id_item) " +
                    "values('" + id_estilo + "','" + id_sucursal + "','" + id_pedido + "','" + id_pais + "','" + id_fabricante + "','" + id_tipo + "','" + id_color + "','" + id_body_type + "','" + id_gender + "','" + id_fabric_type + "','" + id_ubicacion + "','" + quantity + "','" + id_size + "','" + id_customer + "','" + id_customer_final + "','0','" + notas + "','" + id_percent + "','STOCK','" + date_comment + "','" + comments + "','0','0','0','" + descripcion + "','" + id_item + "' ) ";
                com_b.ExecuteNonQuery();
            }finally{con_b.CerrarConexion();con_b.Dispose();}
        }
        public int buscar_existencia_blank_inventario(){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT id_inventario from inventario where id_categoria_inventario='" + id_tipo + "' and id_pais='" + id_pais + "' and id_fabricante='" + id_fabricante + "' and id_color='" + id_color + "' and " +
                    " id_body_type='" + id_body_type + "' and id_size='" + id_size + "' and id_fabric_type='" + id_fabric_type + "' and id_fabric_percent='" + id_percent + "' and id_sucursal='" + id_sucursal + "' and id_pedido='"+id_pedido+"' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["id_inventario"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public void sumar_existencia_blank(int inventario){
            id_inventario = inventario;
            update_stock(id_inventario, Convert.ToInt32(quantity), id_sucursal);
        }
        public void guardar_sello(string inicio, string final, string sucursal){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO sellos(inicio,final,usado,fecha_registro,id_sucursal) VALUES('" + inicio + "','" + final + "','" + (Convert.ToInt32(inicio) - 1) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + sucursal + "') ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion();con_s.Dispose();}
        }
        //**************************************************EDITAR*******************************************************************************************************************************************
        public void guardar_edicion_trim_po(){
            Conexion conTrim = new Conexion();
            try{
                SqlCommand comTrim = new SqlCommand();
                comTrim.Connection = conTrim.AbrirConexion();
                comTrim.CommandText = "UPDATE inventario  SET id_pedido='" + id_pedido + "',id_estilo='" + id_estilo + "',id_unit='" + id_unit +
                    "',total='" + total + "',id_family_trim='" + id_familia + "',id_trim='" + id_trim + "',id_customer='" + id_customer + "',minimo='" + minimo_trim + "',descripcion='" + descripcion + "'  " +
                     " where id_inventario='" + id_inventario + "'";
                comTrim.ExecuteNonQuery();
            }finally{conTrim.CerrarConexion();conTrim.Dispose();}
        }
        public void guardar_edicion_blank(){
            id_color = consultas.buscar_color(color_aux);
            Conexion con_b = new Conexion();
            try{
                SqlCommand com_b = new SqlCommand();
                com_b.Connection = con_b.AbrirConexion();
                com_b.CommandText = "UPDATE inventario SET id_estilo='" + id_estilo + "',id_pedido='" + id_pedido + "',id_pais='" + id_pais + "',id_fabricante='" + id_fabricante +
                    "',id_color='" + id_color + "',id_body_type='" + id_body_type + "',id_genero='" + id_gender + "',id_fabric_type='" + id_fabric_type + "',id_location='" + id_ubicacion + "',id_size='" + id_size + "',id_customer='" + id_customer + "',id_customer_final='" + id_customer_final +
                    "',notas='" + notas_item + "',id_fabric_percent='" + id_percent + "',date_comment='" + date_comment + "',comment='" + comment_item + "',descripcion='" + descripcion + "'  " +
                     " WHERE id_inventario='" + id_inventario + "' ";
                com_b.ExecuteNonQuery();
            }finally{con_b.CerrarConexion();con_b.Dispose();}
        }

        //********************************ITEMS********************************************************************
        public int buscar_existencia_item(string informacion,string talla){
            string[] datos = informacion.Split('*');
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT item_id from items_catalogue where item='" + datos[0] + "' and fabricante='" + datos[1] + "' and color='" + datos[2] + "' and size='" + talla +
                    "' and body_type='" + datos[4] + "' and gender='" + datos[5] + "' and fabric_type='" + datos[6] + "' and yarn='" + datos[8] + "' and division='" + datos[9] +
                    "' and tipo='" + datos[10] + "' and fabric_percent='" + datos[7] + "'  and body_type='" + datos[11] + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["item_id"]);
                }leer.Close();
            }finally{con.CerrarConexion(); con.Dispose();}
            return temp;
        }
        public void guardar_item_nuevo(string informacion,string talla,string unit,string minimo){
            string[] datos = informacion.Split('*');
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "INSERT INTO items_catalogue(item,color,fabricante,size,descripcion,body_type,gender,fabric_type,fabric_percent,yarn,division,tipo,unit,minimo) " +
                    "VALUES('" + datos[0] + "','" + datos[2] + "','" + datos[1] + "','" + talla + "','" + datos[11] + "','" + datos[4] + "','" + datos[5] +
                    "' ,'" + datos[6] + "','" + datos[7] + "','" + datos[8] + "','" + datos[9] + "','" + datos[10] + "','"+unit+"','"+minimo+"') ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion(); con_s.Dispose();}
        }
        public List<Item> lista_items_para_recibir(string item, string descripcion, string color, string size, string gender){
            List<Item> listItem = new List<Item>();
            string query;
            query = "Select TOP 500 item_id,item,color,fabricante,size,descripcion,body_type,gender,fabric_type,fabric_percent,yarn,division,tipo from items_catalogue  ";
            query += "where item like'%"+item+ "%' and descripcion like'%" + descripcion + "%' and color like'%" + color + "%' and size like'%" + size + "%' and gender like'%" + gender + "%' ";            
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = query;
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    Item i = new Item();
                    i.id_item = Convert.ToInt32(leerFilas["item_id"]);
                    i.item_nombre = leerFilas["item"].ToString();
                    i.descripcion = leerFilas["descripcion"].ToString();
                    i.color = leerFilas["color"].ToString();
                    i.size = leerFilas["size"].ToString();
                    i.body_type = leerFilas["body_type"].ToString();
                    i.gender = leerFilas["gender"].ToString();
                    i.fabric_type = leerFilas["fabric_type"].ToString();
                    i.fabric_percent = leerFilas["fabric_percent"].ToString();
                    i.yarn = leerFilas["yarn"].ToString();
                    listItem.Add(i);
                }leerFilas.Close();
            }finally{conn.CerrarConexion(); conn.Dispose();}
            return listItem;
        }
        public List<recibo> Listarecibos(){
            List<recibo> listar = new List<recibo>();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, s.sucursal from recibos r,Usuarios u,sucursales s " +
                " where r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal order by r.id_recibo desc";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    recibo l = new recibo();
                    l.id_recibo = Convert.ToInt32(leerFilas["id_recibo"]);
                    l.fecha = (Convert.ToDateTime(leerFilas["fecha"])).ToString("MMM dd yyyy");
                    l.usuario = leerFilas["Nombres"].ToString() + " " + leerFilas["apellidos"].ToString();
                    l.total = Convert.ToInt32(leerFilas["total"]);
                    l.sucursal = leerFilas["sucursal"].ToString();
                    l.items = buscar_items_recibo(l.id_recibo);
                    l.mp_number = leerFilas["mp_number"].ToString();
                    l.mill_po = leerFilas["mill_po"].ToString();
                    l.po_referencia = leerFilas["po_reference"].ToString();
                    listar.Add(l);
                }leerFilas.Close();
            }finally{conn.CerrarConexion(); conn.Dispose();}
            return listar;
        }
        public string buscar_items_recibo(int id){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "Select ic.item from items_catalogue ic,inventario i,recibos_items ri where ri.id_recibo='" + id + "' and i.id_inventario=ri.id_inventario and i.id_item=ic.item_id";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp += Convert.ToString(leer["item"]) + " ";
                }leer.Close();
            }finally{con.CerrarConexion();con.Dispose();}
            return temp;
        }
        public void agregar_mp_recibo(string id_recibo, string mp){
            Conexion con_s = new Conexion();
            try{
                SqlCommand com_s = new SqlCommand();
                com_s.Connection = con_s.AbrirConexion();
                com_s.CommandText = "UPDATE recibos SET mp_number='" + mp + "' where id_recibo='" + id_recibo + "' ";
                com_s.ExecuteNonQuery();
            }finally{con_s.CerrarConexion();con_s.Dispose();}
        }
        public recibo lista_recibo_detalles(string id_recibo){
            //List<recibo> listar = new List<recibo>();
            recibo rec = new recibo();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, s.sucursal from recibos r,Usuarios u,sucursales s " +
                    " where r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal and r.id_recibo='" + id_recibo + "' order by r.id_recibo desc";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    rec.id_recibo = Convert.ToInt32(leerFilas["id_recibo"]);
                    rec.fecha = (Convert.ToDateTime(leerFilas["fecha"])).ToString("MMM dd yyyy");
                    rec.usuario = leerFilas["Nombres"].ToString() + " " + leerFilas["apellidos"].ToString();
                    rec.total = Convert.ToInt32(leerFilas["total"]);
                    rec.sucursal = leerFilas["sucursal"].ToString();
                    rec.items = buscar_items_recibo(rec.id_recibo);
                    rec.mp_number = leerFilas["mp_number"].ToString();
                    rec.mill_po = leerFilas["mill_po"].ToString();
                    rec.po_referencia = leerFilas["po_reference"].ToString();
                    List<recibos_item> ri = new List<recibos_item>();
                    ri = obtener_lista_items(id_recibo);
                    rec.lista_recibos_item = ri;
                }leerFilas.Close();
            }finally{conn.CerrarConexion(); conn.Dispose();}
            return rec;
        }
        public List<recibos_item> obtener_lista_items(string id_recibo){
            List<recibos_item> lista = new List<recibos_item>();
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "Select  ri.id_inventario,ri.total,ri.id_recibo_item from recibos_items ri where ri.id_recibo='" + id_recibo + "' ";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    recibos_item ri = new recibos_item();
                    ri.id_recibo_item = Convert.ToInt32(leer["id_recibo_item"]);
                    ri.id_inventario = Convert.ToInt32(leer["id_inventario"]);
                    ri.total = Convert.ToInt32(leer["total"]);
                    ri.item = obtener_inventario(ri.id_inventario);
                    ri.lista_cajas = obtener_cajas_recibo(ri.id_recibo_item);
                    lista.Add(ri);
                }leer.Close();
            }finally{con.CerrarConexion();con.Dispose();}
            return lista;
        }
        public Inventario obtener_inventario(int id){
            Inventario listInventario = new Inventario();
            Conexion con_in = new Conexion();
            try{
                SqlCommand com_in = new SqlCommand();
                SqlDataReader leer_in = null;                
                com_in.Connection = con_in.AbrirConexion();
                com_in.CommandText = "SELECT i.id_inventario,i.id_pedido,i.id_categoria_inventario,i.descripcion,c.categoria " +
                    " from inventario i,categorias_inventarios c where i.id_categoria_inventario=c.id_categoria and i.id_inventario='" + id + "' ";
                leer_in = com_in.ExecuteReader();
                while (leer_in.Read()){
                    listInventario.id_inventario = Convert.ToInt32(leer_in["id_inventario"]);
                    listInventario.po = consultas.obtener_po_id(Convert.ToString(leer_in["id_pedido"]));
                    listInventario.categoria_inventario = Convert.ToString(leer_in["categoria"]);
                    listInventario.descripcion = Convert.ToString(leer_in["descripcion"]);
                }leer_in.Close();
            }finally{con_in.CerrarConexion(); con_in.Dispose();}
            return listInventario;
        }
        public List<caja_inventario> obtener_cajas_recibo(int id){
            Inventario i = new Inventario();
            List<caja_inventario> lista = new List<caja_inventario>();
            Conexion conn_cr = new Conexion();
            try{
                SqlCommand com_cr = new SqlCommand();
                SqlDataReader leer_cr = null;
                com_cr.Connection = conn_cr.AbrirConexion();
                com_cr.CommandText = "SELECT id_caja,cantidad_inicial,cantidad_restante from cajas_inventario where  id_recibo_item='" + id + "' ";
                leer_cr = com_cr.ExecuteReader();
                while (leer_cr.Read()){
                    caja_inventario caja = new caja_inventario();
                    caja.cantidad_inicial = Convert.ToInt32(leer_cr["cantidad_inicial"]);
                    caja.cantidad_restante = Convert.ToInt32(leer_cr["cantidad_restante"]);
                    caja.img = "caja_"+leer_cr["id_caja"].ToString()+".jpg";
                    lista.Add(caja);
                }leer_cr.Close();
            }finally{conn_cr.CerrarConexion();conn_cr.Dispose();}
            return lista;
        }
        public IEnumerable<recibo> lista_recibo_etiqueta(string id_recibo){
            List<recibo> listar = new List<recibo>();
           // recibo rec = new recibo();
            Conexion conn = new Conexion();
            try{
                SqlCommand comando = new SqlCommand();
                SqlDataReader leerFilas = null;
                comando.Connection = conn.AbrirConexion();
                comando.CommandText = "SELECT r.id_recibo,r.fecha,r.total,r.mp_number,r.mill_po,r.po_reference, u.Nombres,u.Apellidos, s.sucursal from recibos r,Usuarios u,sucursales s " +
                    " where r.id_usuario=u.Id and r.id_sucursal=s.id_sucursal and r.id_recibo='" + id_recibo + "' order by r.id_recibo desc";
                leerFilas = comando.ExecuteReader();
                while (leerFilas.Read()){
                    recibo rec = new recibo();
                    rec.id_recibo = Convert.ToInt32(leerFilas["id_recibo"]);
                    rec.fecha = (Convert.ToDateTime(leerFilas["fecha"])).ToString("MMM dd yyyy");
                    rec.usuario = leerFilas["Nombres"].ToString() + " " + leerFilas["apellidos"].ToString();
                    rec.total = Convert.ToInt32(leerFilas["total"]);
                    rec.sucursal = leerFilas["sucursal"].ToString();
                    rec.items = buscar_items_recibo(rec.id_recibo);
                    rec.mp_number = leerFilas["mp_number"].ToString();
                    rec.mill_po = leerFilas["mill_po"].ToString();
                    rec.po_referencia = leerFilas["po_reference"].ToString();
                    List<recibos_item> ri = new List<recibos_item>();
                    ri = obtener_lista_items(id_recibo);
                    rec.lista_recibos_item = ri;
                    listar.Add(rec);
                }leerFilas.Close();
            }finally{conn.CerrarConexion(); conn.Dispose();}
            return listar;
        }
        public int buscar_po_summary_inventario(string inventario){
            int temp = 0;
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT ps.ID_PO_SUMMARY FROM PO_SUMMARY ps, inventario i WHERE i.id_inventario='" + inventario + "' "+
                    " AND ps.ID_PEDIDOS=i.id_pedido AND  ps.ITEM_ID=i.id_estilo";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToInt32(leer["ID_PO_SUMMARY"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        



























    }
}