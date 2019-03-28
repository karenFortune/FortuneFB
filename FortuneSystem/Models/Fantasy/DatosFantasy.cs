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
using FortuneSystem.Models.Fantasy;
using System.Text.RegularExpressions;

namespace FortuneSystem.Models.Fantasy
{
    public class DatosFantasy{
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        string[] tallas = { "SM", "MD", "LG", "XL", "2X" };

        public List<Cliente> obtener_lista_clientes(){
            List<Cliente> lista = new List<Cliente>();            
            Conexion conc = new Conexion();
            try{
                SqlCommand comc = new SqlCommand();
                SqlDataReader leerc = null;
                comc.Connection = conc.AbrirConexion();
                comc.CommandText = "select id_cliente_fantasy,cliente,estado from clientes_fantasy order by id_cliente_fantasy ";
                leerc = comc.ExecuteReader();
                while (leerc.Read()){
                    Cliente c = new Cliente();
                    c.id_cliente = Convert.ToInt32(leerc["id_cliente_fantasy"]);
                    c.nombre = Convert.ToString(leerc["cliente"]);
                    c.estado = Convert.ToInt32(leerc["estado"]);
                    lista.Add(c);
                }leerc.Close();
            }finally { conc.CerrarConexion(); conc.Dispose(); }
            return lista;
        }
        public void agregar_nuevo_cliente(string cliente) {
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO clientes_fantasy(cliente,estado) VALUES ('" + cliente + "','0')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void editar_cliente(string id,string cliente){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE clientes_fantasy set cliente='" + cliente + "' where id_cliente_fantasy='"+id+"'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void cambiar_estado_cliente(string id, string estado){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE clientes_fantasy set estado='" + estado + "' where id_cliente_fantasy='" + id + "'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public List<Estilo> obtener_lista_estilos(){
            List<Estilo> lista = new List<Estilo>();
            Conexion conc = new Conexion();
            try{
                SqlCommand comc = new SqlCommand();
                SqlDataReader leerc = null;
                comc.Connection = conc.AbrirConexion();
                comc.CommandText = "select id_estilo_fantasy,id_estilo,id_cliente_fantasy,estado,id_color from estilos_fantasy order by id_cliente_fantasy ";
                leerc = comc.ExecuteReader();
                while (leerc.Read()){
                    Estilo c = new Estilo();
                    c.id_estilo_fantasy = Convert.ToInt32(leerc["id_estilo_fantasy"]);
                    c.id_estilo = Convert.ToInt32(leerc["id_estilo"]);
                    c.estilo = consultas.obtener_estilo(c.id_estilo);
                    c.descripcion = consultas.buscar_descripcion_estilo(c.id_estilo);
                    c.estado = Convert.ToInt32(leerc["estado"]);
                    c.id_cliente = Convert.ToInt32(leerc["id_cliente_fantasy"]);
                    c.cliente = obtener_nombre_cliente_id(Convert.ToInt32(leerc["id_cliente_fantasy"]));
                    c.id_color= Convert.ToInt32(leerc["id_color"]);
                    c.color = consultas.obtener_color_id(Convert.ToString(c.id_color));
                    lista.Add(c);
                }leerc.Close();
            }finally { conc.CerrarConexion(); conc.Dispose(); }
            return lista;
        }
        public string obtener_nombre_cliente_id(int cliente){
            string temp = "";
            Conexion con = new Conexion();
            try{
                SqlCommand com = new SqlCommand();
                SqlDataReader leer = null;
                com.Connection = con.AbrirConexion();
                com.CommandText = "SELECT cliente from clientes_fantasy where id_cliente_fantasy='" + cliente + "'";
                leer = com.ExecuteReader();
                while (leer.Read()){
                    temp = Convert.ToString(leer["cliente"]);
                }leer.Close();
            }finally { con.CerrarConexion(); con.Dispose(); }
            return temp;
        }
        public void guardar_nuevo_estilo(string estilo,string cliente,string color){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO estilos_fantasy(id_estilo,id_cliente_fantasy,estado,id_color) VALUES "+
                    "('"+consultas.obtener_estilo_id(estilo)+ "','" + cliente + "','0','"+consultas.buscar_color_codigo(color)+ "')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void crear_estilo_inventario(string estilo){
            foreach (string s in tallas) {
                Conexion con_c = new Conexion();
                try{
                    SqlCommand com_c = new SqlCommand();
                    com_c.Connection = con_c.AbrirConexion();
                    com_c.CommandText = "INSERT INTO inventario_fantasy(id_estilo,total,talla) VALUES ('" + consultas.obtener_estilo_id(estilo) + "','0','"+s+"')";
                    com_c.ExecuteNonQuery();
                }finally { con_c.CerrarConexion(); con_c.Dispose(); }
            }
        }
        public void editar_estilo_cliente(string id,string estilo, string cliente,string color){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE estilos_fantasy set id_cliente_fantasy='" + cliente + "',id_estilo='"+ consultas.obtener_estilo_id(estilo) +
                    "',id_color='"+ consultas.buscar_color_codigo(color) + "'  where id_estilo_fantasy='" + id + "'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void cambiar_estado_estilo(string id, string estado){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE estilos_fantasy set estado='" + estado + "' where id_estilo_fantasy='" + id + "'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void agregar_registro_inventory(int estilo, int total,string instruccion,string estado,string ship_date,string pedido){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO registros_fantasy(id_estilo,total,fecha,tipo,estado,ship_date,id_packing_list,pedido) VALUES " +
                    "('" + estilo + "','"+total+"','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") +
                    "','"+instruccion+"','"+estado+"','"+ship_date+"','0','"+pedido+"')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public int obtener_ultimo_registro(){
            int id_recibo = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT TOP 1 id_registro FROM registros_fantasy order by id_registro desc ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    id_recibo = Convert.ToInt32(leer_u_r["id_registro"]);
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id_recibo;
        }
        public void agregar_cantidades_fantasy(int registro,int total,int restante,string talla,int id_estilo) { 
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "INSERT INTO cantidades_fantasy(id_registro,total,restante,talla,id_estilo) VALUES "+
                    " ('" + registro+"','"+total+"','"+restante+"','"+talla+ "','"+id_estilo+"')";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public List<InventarioFantasy> obtener_inventario_estilo(int estilo){
            List<InventarioFantasy> lista = new List<InventarioFantasy>();
             Conexion conc = new Conexion();
            try{
                SqlCommand comc = new SqlCommand();
                SqlDataReader leerc = null;
                comc.Connection = conc.AbrirConexion();
                comc.CommandText = "select id_inventario,id_estilo,total,talla from inventario_fantasy where id_estilo='"+estilo+"'";
                leerc = comc.ExecuteReader();
                while (leerc.Read()){
                    InventarioFantasy i = new InventarioFantasy();
                    i.id_inventario = Convert.ToInt32(leerc["id_inventario"]);
                    i.id_estilo = Convert.ToInt32(leerc["id_estilo"]);
                    i.total = Convert.ToInt32(leerc["total"]);
                    i.talla = Convert.ToString(leerc["talla"]);
                    lista.Add(i);
                }
                leerc.Close();
            }finally { conc.CerrarConexion(); conc.Dispose(); }
            return lista;
        }
        public void actualizar_inventario(int estilo,string talla,int total){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE inventario_fantasy set total='"+total+"'  where id_estilo='"+estilo+"' and talla='"+talla+"'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public List<Registro> obtener_registro_completo(string registro){
            string[] registros = registro.Split('*');
            List<Registro> lista = new List<Registro>();
            for (int i = 1; i < registros.Length; i++) {
                Conexion conc = new Conexion();
                try{
                    SqlCommand comc = new SqlCommand();
                    SqlDataReader leerc = null;
                    comc.Connection = conc.AbrirConexion();
                    comc.CommandText = "select id_estilo,total,fecha,tipo,estado,ship_date,id_packing_list,pedido from registros_fantasy where id_registro='" + registros[i] + "' ";
                    leerc = comc.ExecuteReader();
                    while (leerc.Read()){
                        Registro r = new Registro();
                        r.id_registro = registro[i];
                        r.id_estilo_fantasy= Convert.ToInt32(leerc["id_estilo"]);
                        r.id_estilo = buscar_id_estilo(r.id_estilo_fantasy);
                        r.estilo = obtener_estilo_registro(r.id_estilo);
                        r.cliente = obtener_cliente_registro(r.estilo.id_cliente);
                        r.total = Convert.ToInt32(leerc["total"]);
                        r.fecha = (Convert.ToDateTime(leerc["fecha"])).ToString("MM/dd/yyyy");
                        r.tipo = Convert.ToString(leerc["tipo"]);
                        r.ship_date = (Convert.ToDateTime(leerc["ship_date"])).ToString("MM/dd/yyyy");
                        r.lista_cantidades = obtener_lista_cantidades_registro(Convert.ToInt32(registros[i]));
                        r.pedido = Regex.Replace(Convert.ToString(leerc["pedido"]), @"\s+", " "); 
                        lista.Add(r);
                    }leerc.Close();
                }finally { conc.CerrarConexion(); conc.Dispose(); }
            }
            return lista;
        }
        public int buscar_id_estilo(int estilo_fantasy){
            int id = 0;
            Conexion con_u_r = new Conexion();
            try{
                SqlCommand com_u_r = new SqlCommand();
                SqlDataReader leer_u_r = null;
                com_u_r.Connection = con_u_r.AbrirConexion();
                com_u_r.CommandText = "SELECT id_estilo FROM estilos_fantasy where id_estilo_fantasy='"+estilo_fantasy+"' ";
                leer_u_r = com_u_r.ExecuteReader();
                while (leer_u_r.Read()){
                    id = Convert.ToInt32(leer_u_r["id_estilo"]);
                }leer_u_r.Close();
            }finally { con_u_r.CerrarConexion(); con_u_r.Dispose(); }
            return id;
        }
        public Estilo obtener_estilo_registro(int estilo) {
            Estilo lista = new Estilo();
            Conexion con_e = new Conexion();
            try{
                SqlCommand com_e = new SqlCommand();
                SqlDataReader leer_e = null;
                com_e.Connection = con_e.AbrirConexion();
                com_e.CommandText = "SELECT id_estilo_fantasy,id_color,id_cliente_fantasy,estado from estilos_fantasy where id_estilo='"+estilo+"'";
                leer_e = com_e.ExecuteReader();
                while (leer_e.Read()){
                    lista.id_estilo_fantasy = Convert.ToInt32(leer_e["id_estilo_fantasy"]);
                    lista.id_estilo = estilo;
                    lista.estilo = Regex.Replace(consultas.obtener_estilo(lista.id_estilo), @"\s+", " "); 
                    lista.descripcion = Regex.Replace(consultas.buscar_descripcion_estilo(lista.id_estilo), @"\s+", " "); 
                    lista.estado = Convert.ToInt32(leer_e["estado"]);
                    lista.id_cliente = Convert.ToInt32(leer_e["id_cliente_fantasy"]);
                    lista.cliente = obtener_nombre_cliente_id(Convert.ToInt32(leer_e["id_cliente_fantasy"]));
                    lista.id_color = Convert.ToInt32(leer_e["id_color"]);
                    lista.color = Regex.Replace(consultas.obtener_color_id(Convert.ToString(lista.id_color)), @"\s+", " "); 
                }
                leer_e.Close();
            }finally { con_e.CerrarConexion();con_e.Dispose(); }
            return lista;
        }
        public Cliente obtener_cliente_registro(int cliente){
            Cliente lista = new Cliente();
            Conexion con_e = new Conexion();
            try{
                SqlCommand com_e = new SqlCommand();
                SqlDataReader leer_e = null;
                com_e.Connection = con_e.AbrirConexion();
                com_e.CommandText = "SELECT cliente from clientes_fantasy where id_cliente_fantasy='" + cliente + "'";
                leer_e = com_e.ExecuteReader();
                while (leer_e.Read()){
                    lista.id_cliente = cliente;
                    lista.nombre = Convert.ToString(leer_e["cliente"]);
                }leer_e.Close();
            }finally { con_e.CerrarConexion(); con_e.Dispose(); }
            return lista;
        }
        public List<Cantidades> obtener_lista_cantidades_registro(int registro){
            List<Cantidades> lista = new List<Cantidades>();
            Conexion concr = new Conexion();
            try{
                SqlCommand comcr = new SqlCommand();
                SqlDataReader leercr = null;
                comcr.Connection = concr.AbrirConexion();
                comcr.CommandText = "select id_cantidad,total,talla,restante from cantidades_fantasy where id_registro='" + registro + "' order by id_cantidad asc ";
                leercr = comcr.ExecuteReader();
                while (leercr.Read()){
                    Cantidades r = new Cantidades();
                    r.id_cantidad = Convert.ToInt32(leercr["id_cantidad"]);
                    r.total = Convert.ToInt32(leercr["total"]);
                    r.restante = Convert.ToInt32(leercr["restante"]);
                    r.talla= Convert.ToString(leercr["talla"]);
                    lista.Add(r);
                }leercr.Close();
            }finally { concr.CerrarConexion(); concr.Dispose(); }
            return lista;
        }
        public void agregar_inventario(int estilo, string talla,int total){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE inventario_fantasy set total=total+'" + total + "' " +
                    " where id_estilo='" + estilo + "' and talla='" + talla + "'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public List<Cantidades> obtener_lista_batch_pendientes(int estilo){
            List<Cantidades> lista = new List<Cantidades>();
            Conexion concr = new Conexion();
            try{
                SqlCommand comcr = new SqlCommand();
                SqlDataReader leercr = null;
                comcr.Connection = concr.AbrirConexion();
                comcr.CommandText = "select id_cantidad,total,talla,restante,id_registro from cantidades_fantasy where id_estilo='" + estilo + "' and restante!=0 order by id_cantidad desc ";
                leercr = comcr.ExecuteReader();
                while (leercr.Read()){
                    Cantidades r = new Cantidades();
                    r.id_registro= Convert.ToInt32(leercr["id_registro"]);
                    r.id_cantidad = Convert.ToInt32(leercr["id_cantidad"]);
                    r.total = Convert.ToInt32(leercr["total"]);
                    r.restante = Convert.ToInt32(leercr["restante"]);
                    r.talla = Convert.ToString(leercr["talla"]);
                    lista.Add(r);
                }
                leercr.Close();
            }
            finally { concr.CerrarConexion(); concr.Dispose(); }
            return lista;
        }
        public void actualizar_batch(int registro, int restante){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE cantidades_fantasy set restante='" + restante + "' where id_cantidad='" + registro + "' ";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void actualizar_talla_inventario(int estilo, string talla,int total){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE inventario_fantasy set total='" + total + "' where id_estilo='" + estilo + "' and talla='" + talla + "'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }
        public void verificar_batch(int batch){
           Conexion concr = new Conexion();
            int tempo = 0;
            try{
                SqlCommand comcr = new SqlCommand();
                SqlDataReader leercr = null;
                comcr.Connection = concr.AbrirConexion();
                comcr.CommandText = "select restante from cantidades_fantasy where id_registro='" + batch + "' ";
                leercr = comcr.ExecuteReader();
                while (leercr.Read()){
                    if (Convert.ToInt32(leercr["restante"]) != 0) {
                        tempo++;
                    }
                }leercr.Close();
            }finally { concr.CerrarConexion(); concr.Dispose(); }
            if (tempo == 0) {
                cambiar_estado_registro(batch,"COMPLETED");
            }
        }
        public void cambiar_estado_registro(int registro, string estado){
            Conexion con_c = new Conexion();
            try{
                SqlCommand com_c = new SqlCommand();
                com_c.Connection = con_c.AbrirConexion();
                com_c.CommandText = "UPDATE registros_fantasy set estado='" + estado + "' where id_registro='" + registro + "'";
                com_c.ExecuteNonQuery();
            }finally { con_c.CerrarConexion(); con_c.Dispose(); }
        }

        public List<Registro> obtener_registros_inicio(int cliente){
           
            List<Registro> lista = new List<Registro>();
           
                Conexion conc = new Conexion();
                try{
                    SqlCommand comc = new SqlCommand();
                    SqlDataReader leerc = null;
                    comc.Connection = conc.AbrirConexion();
                    comc.CommandText = "select top 100 r.id_registro,r.id_estilo,r.total,r.fecha,r.tipo,r.estado,r.ship_date,r.id_packing_list,r.pedido "+
                        " from registros_fantasy r,estilos_fantasy e where e.id_estilo_fantasy=r.id_estilo and e.id_cliente_fantasy='"+cliente+"'  order by r.id_registro desc ";
                    leerc = comc.ExecuteReader();
                    while (leerc.Read()){
                        Registro r = new Registro();
                        r.id_registro = Convert.ToInt32(leerc["id_registro"]);
                        r.id_estilo_fantasy = Convert.ToInt32(leerc["id_estilo"]);
                        r.id_estilo = buscar_id_estilo(r.id_estilo_fantasy);
                        r.estilo = obtener_estilo_registro(r.id_estilo);
                        r.cliente = obtener_cliente_registro(r.estilo.id_cliente);
                        r.total = Convert.ToInt32(leerc["total"]);
                        r.fecha = (Convert.ToDateTime(leerc["fecha"])).ToString("MM/dd/yyyy");
                        r.tipo = Convert.ToString(leerc["tipo"]);
                        r.ship_date = (Convert.ToDateTime(leerc["ship_date"])).ToString("MM/dd/yyyy");
                        r.lista_cantidades = obtener_lista_cantidades_registro(r.id_registro);
                        r.pedido = Regex.Replace(Convert.ToString(leerc["pedido"]), @"\s+", " ");
                        lista.Add(r);
                    }leerc.Close();
                }finally { conc.CerrarConexion(); conc.Dispose(); }
            
            return lista;
        }

        //obtener_registros_estilo

        public List<Registro> obtener_registros_estilo(int estilo){
            List<Registro> lista = new List<Registro>();
            Conexion conc = new Conexion();
            try{
                SqlCommand comc = new SqlCommand();
                SqlDataReader leerc = null;
                comc.Connection = conc.AbrirConexion();
                comc.CommandText = "select id_registro,id_estilo,total,fecha,tipo,estado,ship_date,id_packing_list,pedido " +
                    " from registros_fantasy where id_estilo='" + estilo + "'  order by fecha ASC";
                leerc = comc.ExecuteReader();
                while (leerc.Read()){
                    Registro r = new Registro();
                    r.id_registro = Convert.ToInt32(leerc["id_registro"]);
                    r.id_estilo_fantasy = Convert.ToInt32(leerc["id_estilo"]);
                    r.id_estilo = buscar_id_estilo(r.id_estilo_fantasy);
                    r.estilo = obtener_estilo_registro(r.id_estilo);
                    r.cliente = obtener_cliente_registro(r.estilo.id_cliente);
                    r.total = Convert.ToInt32(leerc["total"]);
                    r.fecha = (Convert.ToDateTime(leerc["fecha"])).ToString("MM/dd/yyyy");
                    r.tipo = Convert.ToString(leerc["tipo"]);
                    r.ship_date = (Convert.ToDateTime(leerc["ship_date"])).ToString("MM/dd/yyyy");
                    r.lista_cantidades = obtener_lista_cantidades_registro(r.id_registro);
                    r.pedido = Regex.Replace(Convert.ToString(leerc["pedido"]), @"\s+", " ");
                    lista.Add(r);
                }leerc.Close();
            }finally { conc.CerrarConexion(); conc.Dispose(); }
            return lista;
        }




























    }
}