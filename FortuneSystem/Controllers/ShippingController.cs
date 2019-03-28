using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Shipping;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using OfficeOpenXml;
using System.Data;
using ClosedXML.Excel;
using ZXing.Common;
using ZXing.QrCode;
using System.Text.RegularExpressions;

namespace FortuneSystem.Controllers
{
    public class ShippingController : Controller
    {
        DatosInventario di = new DatosInventario();
        DatosReportes dr = new DatosReportes();
        DatosShipping ds = new DatosShipping();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        QRCodeController qr = new QRCodeController();
        PDFController pdf = new PDFController();

        public ActionResult Index()
        {
            //Session["id_usuario"] = 2;
            //Session["id_usuario"] = consultas.buscar_id_usuario(Convert.ToString(Session["usuario"]));
            /*if (Session["usuario"] == null){
                return View();
            }else {
                return View();
            }*/
            int id_usuario = Convert.ToInt32(Session["id_Empleado"]);
            Session["id_usuario"] = id_usuario;
            //Session["id_usuario"] = 2;
            return View();
        }

        public ActionResult new_pk(){ return View(); }
        public ActionResult new_packing_list(){ return View(); }

        public JsonResult buscar_pedidos_inicio(string busqueda)
        {
            return Json(ds.lista_ordenes(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos_pedido(string id_pedido)
        {
            Session["id_pedido"] = id_pedido;
            return Json(ds.lista_estilos(id_pedido), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Autocomplete_tallas(string term){
            var items = consultas.Lista_tallas();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_dcs(string term){
            var items = consultas.Lista_dcs();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_recibo_inventario_shipping(string estilos, string colores, string tallas, string piezas, string cajas){
            string[] Estilos = estilos.Split('*'), Colores = colores.Split('*'), Tallas = tallas.Split('*'), Piezas = piezas.Split('*'), Cajas = cajas.Split('*');
            int total_piezas = 0, id_talla, id_color, existencia, total_cajas = 0, id_recibo, id_inventario;
            string descripcion, estilo;
            for (int i = 1; i < Estilos.Length; i++){
                total_piezas += Convert.ToInt32(Piezas[i]);
                total_cajas += Convert.ToInt32(Cajas[i]);
            }
            ds.guardar_recibo_fantasy(Convert.ToInt32(Session["id_pedido"]), Convert.ToInt32(Session["id_usuario"]), total_piezas, total_cajas);
            id_recibo = ds.obtener_ultimo_recibo();
            for (int i = 1; i < Estilos.Length; i++){
                id_talla = consultas.buscar_talla(Tallas[i]);
                id_color = consultas.buscar_color_codigo(Colores[i]);
                estilo = consultas.obtener_estilo(Convert.ToInt32(Estilos[i])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(Estilos[i]));
                descripcion = estilo + " " + Colores[i] + " " + Tallas[i];
                descripcion = Regex.Replace(descripcion, @"\s+", " ");
                existencia = ds.buscar_existencia_inventario(id_color, id_talla, Estilos[i]);
                if (existencia == 0){
                    ds.guardar_item_inventario(id_color, id_talla, Estilos[i], descripcion, Convert.ToInt32(Cajas[i]) * Convert.ToInt32(Piezas[i]));
                    id_inventario = ds.obtener_ultimo_item();
                }else{
                    id_inventario = existencia;
                    ds.aumentar_inventario(existencia, Convert.ToInt32(Cajas[i]), Convert.ToInt32(Piezas[i]));
                }
                ds.guardar_recibo_fantasy_item(id_recibo, id_inventario, Cajas[i], Piezas[i]);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_po(string term){
            var items = consultas.Lista_po_abiertos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_estilos(string term){
            var items = consultas.Lista_styles();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_po_abiertos(string term)
        {
            var items = consultas.Lista_po_abiertos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_estilos_dc(string po)
        {
            Session["po"] = po;
            return Json(ds.buscar_estilos_po(po), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_conductores()
        {
            return Json(ds.obtener_drivers(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_carriers()
        {
            return Json(ds.obtener_carriers(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult enviar_informacion_driver(string carrier, string nombre, string plates, string scac, string caat, string tractor)
        {
            ds.guardar_nuevo_conductor(carrier, nombre, plates, scac, caat, tractor);
            return Json(ds.obtener_carriers(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_informacion_driver(string id)
        {
            Session["id_driver_edit"] = id;
            return Json(ds.obtener_conductor_edicion(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_driver_edicion(string id, string carrier, string nombre, string plates, string scac, string caat, string tractor)
        {
            ds.guardar_conductor_edicion(id, carrier, nombre, plates, scac, caat, tractor);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_direcciones_envio()
        {
            return Json(ds.obtener_direcciones(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_direccion(string nombre, string direccion, string ciudad, string zip)
        {
            ds.guardar_nueva_direccion(nombre, direccion, ciudad, zip);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_modificar_edicion(string id)
        {
            return Json(ds.obtener_direccion_edicion(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_direccion_edicion(string id, string nombre, string direccion, string ciudad, string zip)
        {
            ds.guardar_direccion_edicion(id, nombre, direccion, ciudad, zip);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult eliminar_conductor(string id)
        {
            ds.borrar_conductor(id);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult eliminar_direccion(string id)
        {
            ds.borrar_direccion(id);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_contenedores_select()
        {
            return Json(ds.obtener_contenedores_select(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_conductores_select()
        {
            return Json(ds.obtener_conductores_select(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_direcciones_select()
        {
            return Json(ds.obtener_direcciones_select(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos_pk(string pedido){
            int id_pedido = consultas.buscar_pedido(pedido);
            return Json(ds.obtener_lista_tarimas_estilos(id_pedido), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_pedidos(){
            return Json(ds.obtener_lista_po_shipping(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult guardar_pk(string po, string address, string driver, string container, string seal, string replacement, string manager, string tipo, string labels, string type_labels, int dcs,string num_envio)
        {
            int usuario = Convert.ToInt32(Session["id_usuario"]);
            string[] pk_ant, year_pk;
            string pk_anterior = ds.obtener_ultimo_pk();
            int indice_pk = 0, ultimo_pk, id_customer, id_customer_po, id_pedido;
            if (pk_anterior != ""){
                pk_ant = pk_anterior.Split('-');
                year_pk = pk_ant[1].Split(' ');
                if (year_pk[0] == (DateTime.Now.Year.ToString())){
                    indice_pk = (Convert.ToInt32(pk_ant[0]) + 1);
                }else { indice_pk = 1; }
            }else{
                indice_pk = 1;
            }
            id_pedido = Convert.ToInt32(po);
            id_customer = consultas.obtener_customer_po(id_pedido);
            id_customer_po = consultas.obtener_customer_final_po(id_pedido);
            ds.guardar_pk_nuevo(id_pedido, id_customer, id_customer_po, ((Convert.ToString(indice_pk).PadLeft(4, '0')) + "-" + DateTime.Now.Year.ToString() + " FFB"), address, driver, container, seal, replacement, manager, tipo, usuario, num_envio);
            ultimo_pk = ds.obtener_ultimo_pk_registrado();
            if (labels != "N/A"){
                string[] label = labels.Split('*');
                for (int i = 1; i < label.Length; i++){
                    ds.guardar_pk_labels(label[i], ultimo_pk, type_labels);
                }
            }
            Session["pedido"] = id_pedido;
            Session["pedido_pk"] = id_pedido;
            Session["pk"] = ultimo_pk;
            List<estilo_shipping> e = ds.lista_estilos(Convert.ToString(id_pedido));
            var result = Json(new { dc = dcs,
                estilos =e,
                //cantidades_estilos = ds.obtener_cantidades_estilos(id_pedido),
                number_po = ds.obtener_number_po_pedido(id_pedido),
                assorts = ds.lista_assortments_pedido(id_pedido),
                tallas = ds.obtener_lista_tallas_pedido(e)
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /**/
        public JsonResult guardar_pk_estilos(string summary,string extension,string piece,string box,string size,string store,string type,string dc_summary,string empaque,string dcs_array,string indice)
        {
            string[] summarys = summary.Split('*'), extensiones = extension.Split('*'), piezas = piece.Split('*'), cajas = box.Split('*'), tallas = size.Split('*');
            string[] stores = store.Split('*'), tipos = type.Split('*'), dc_summarys = dc_summary.Split('*'), empaques = empaque.Split('*'), dc_bp = dcs_array.Split(',');
            string[] indices=indice.Split('*');
            int packing = Convert.ToInt32(Session["pk"]);
            int id_pedido = Convert.ToInt32(Session["pedido_pk"]);
            int total_enviado = ds.obtener_total_enviado_pedido(id_pedido),total_pedido= ds.obtener_total_pedido(id_pedido);
            int number_po = ds.obtener_number_po_pedido(id_pedido);
            int shipping_id = 0,total_piezas_pk=0;
            for (int i = 1; i < summarys.Length; i++){
                int id_estilo = consultas.obtener_estilo_summary(Convert.ToInt32(summarys[i]));
                switch (empaques[i]){
                    case "1"://TIPO DE EMPAQUE BLPACK
                        if (empaques[i] == "1" && indices[i] == "0"){//SIN DC
                            total_piezas_pk += Convert.ToInt32(piezas[i]);
                        }
                        if (empaques[i] == "1" && indices[i] != "0"){//CON DC
                            for (int j = 1; j < dc_bp.Length; j++){
                                string[] dcs_filas = dc_bp[j].Split('*'), cabecera_dc = dc_bp[0].Split('*');
                                int columnas = dcs_filas.Length;
                                if (dcs_filas[columnas - 1] == indices[i]){
                                    for (int k = 2; k < (columnas - 1); k++){
                                        int ratio = ds.buscar_piezas_empaque_bull(Convert.ToInt32(summarys[i]), Convert.ToInt32(cabecera_dc[k]));
                                        if (dcs_filas[k] != "0"){ total_piezas_pk += (ratio * Convert.ToInt32(dcs_filas[k])); }
                                    }
                                }
                            }
                        }
                        break;
                    case "2":
                        List<ratio_tallas> ratios = ds.obtener_lista_ratio(Convert.ToInt32(summarys[i]), id_estilo, 2);
                        foreach (ratio_tallas r in ratios){
                            total_piezas_pk += (Convert.ToInt32(cajas[i]) * r.ratio); }
                        break;
                    case "3":
                        Assortment a = ds.assortment_id(Convert.ToInt32(summarys[i]), id_pedido);
                        foreach (estilos e in a.lista_estilos){
                            List<ratio_tallas> ratios_a = ds.obtener_lista_ratio_assort_r(e.id_po_summary, e.id_estilo, a.nombre);
                            foreach (ratio_tallas r in ratios_a){
                                total_piezas_pk += (Convert.ToInt32(cajas[i]) * r.ratio);
                            }
                        }
                        break;
                }
            }
            if ((total_enviado +total_piezas_pk)> (total_pedido + 100)){//ERROR
                return Json("1", JsonRequestBehavior.AllowGet);
            }else{
                for (int i = 1; i < summarys.Length; i++){
                    int id_estilo = consultas.obtener_estilo_summary(Convert.ToInt32(summarys[i]));
                    switch (empaques[i]){
                        case "1"://TIPO DE EMPAQUE BLPACK
                            if (empaques[i] == "1" && indices[i] == "0"){//SIN DC
                                int id_talla = consultas.buscar_talla(tallas[i]);
                                ds.guardar_estilos_paking(piezas[i], "0", packing.ToString(), id_pedido.ToString(), id_estilo.ToString(), number_po.ToString(), "0", summarys[i], id_talla.ToString(), stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                                shipping_id = ds.obtener_ultimo_shipping_registrado();
                                ds.agregar_cantidades_enviadas(summarys[i], id_talla.ToString(), piezas[i], shipping_id.ToString(), tipos[i], "0");
                            }
                            if (empaques[i] == "1" && indices[i] != "0"){//CON DC
                                for (int j = 1; j < dc_bp.Length; j++){
                                    string[] dcs_filas = dc_bp[j].Split('*'), cabecera_dc = dc_bp[0].Split('*');
                                    int columnas = dcs_filas.Length;
                                    if (dcs_filas[columnas - 1] == indices[i]){
                                        for (int k = 2; k < (columnas - 1); k++){
                                            int ratio = ds.buscar_piezas_empaque_bull(Convert.ToInt32(summarys[i]), Convert.ToInt32(cabecera_dc[k]));
                                            if (dcs_filas[k] != "0"){
                                                ds.guardar_estilos_paking((ratio * Convert.ToInt32(dcs_filas[k])).ToString(), "0", packing.ToString(), id_pedido.ToString(), id_estilo.ToString(), number_po.ToString(), dcs_filas[0], summarys[i], cabecera_dc[k], stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                                                shipping_id = ds.obtener_ultimo_shipping_registrado();
                                                ds.agregar_cantidades_enviadas(summarys[i], cabecera_dc[k], (ratio * Convert.ToInt32(dcs_filas[k])).ToString(), shipping_id.ToString(), tipos[i], "0");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "2":
                            if (tallas[i] == "") { tallas[i] = "0"; }
                            ds.guardar_estilos_paking(cajas[i], "0", packing.ToString(), id_pedido.ToString(), id_estilo.ToString(), number_po.ToString(), dc_summarys[i], summarys[i], tallas[i], stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                            shipping_id = ds.obtener_ultimo_shipping_registrado();
                            List<ratio_tallas> ratios = ds.obtener_lista_ratio(Convert.ToInt32(summarys[i]), id_estilo, 2);
                            foreach (ratio_tallas r in ratios){
                                ds.agregar_cantidades_enviadas(summarys[i], (r.id_talla).ToString(), (Convert.ToInt32(cajas[i]) * r.ratio).ToString(), shipping_id.ToString(), tipos[i], "0");
                            }
                            break;
                        case "3":
                            if (tallas[i] == "") { tallas[i] = "0"; }
                            ds.guardar_estilos_paking(cajas[i], "0", packing.ToString(), id_pedido.ToString(), "0", number_po.ToString(), dc_summarys[i], summarys[i], tallas[i], stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                            shipping_id = ds.obtener_ultimo_shipping_registrado();
                            Assortment a = ds.assortment_id(Convert.ToInt32(summarys[i]), id_pedido);
                            foreach (estilos e in a.lista_estilos){
                                List<ratio_tallas> ratios_a = ds.obtener_lista_ratio_assort_r(e.id_po_summary, e.id_estilo, a.nombre);
                                foreach (ratio_tallas r in ratios_a){
                                    ds.agregar_cantidades_enviadas((e.id_po_summary).ToString(), (r.id_talla).ToString(), (Convert.ToInt32(cajas[i]) * r.ratio).ToString(), shipping_id.ToString(), tipos[i], "1");
                                }
                            }
                            break;
                    }
                }
                verificar_estado_pedido(Convert.ToInt32(Session["pedido"]));
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
        //VERIFICAR SI YA SE MANDO TODO O NO PARA CERRAR EL PEDIDO
        public void verificar_estado_pedido(int pedido){           
            int total_enviado = ds.obtener_total_enviado_pedido(pedido), total_pedido = ds.obtener_total_pedido(pedido);
            if (total_enviado >= total_pedido){
                ds.cerrar_pedido(pedido);
                ds.eliminar_inventario_pedido(pedido);
            }
        }
        public JsonResult cerrar_po(string po) {
            ds.cerrar_pedido(consultas.buscar_pedido(po));
            return Json("", JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult buscar_estilos_packing(int id){
            return Json(Json(new { estilos = ds.lista_estilos_packing(id) }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult guardar_ids_pk(string tarima, string shipping_id, string packing,string index, string empaque){
            string[] Shippings = shipping_id.Split('*'),Packings=packing.Split('*'),Indices=index.Split('*'),Empaques=empaque.Split('*');
            for (int i = 1; i < Shippings.Length; i++){
                if (Shippings[i] == "0"){
                    ds.guardar_ids_tarimas_bpdc(tarima, Packings[i], Indices[i]);
                }else {
                    ds.guardar_ids_tarimas(tarima,Shippings[i]);
                }
            }           
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_pk_tabla(string busqueda){
            return Json(ds.lista_buscar_pk_inicio(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult abrir_pk(string id){
            Session["pk"] = id;//BOL
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_pk(){
            int rerferw = 0;
            rerferw++;
            List<Pk> lista = ds.obtener_packing_list(Convert.ToInt32(Session["pk"]));
            crear_excel(lista);
        }
        public void crear_excel(List<Pk> lista){
            string clave_packing = "";
            using (XLWorkbook libro_trabajo = new XLWorkbook()){
                string archivo = "";
                int estilos_total = 0, rows = 0, tarima_contador = 0, total_ratio, contador, r, c, total_cajas = 0, filas = 0, columnas = 0, tallas_id;
                List<Talla> tallas = new List<Talla>();
                List<Talla> tallas_bpdc = new List<Talla>();
                var ws = libro_trabajo.Worksheets.Add("PK");
                foreach (Pk item in lista){
                    //item.tipo_empaque = 2;                    
                    clave_packing = item.packing;
                    archivo = "PO "+item.pedido + " PK"+item.packing;
                    /*****INICIO CON DIRECCIONES, LOGO, ETC******/
                    ws.Cell("A2").Value = "FORTUNE FASHIONS BAJA, S.R.L. DE C.V.";
                    ws.Cell("A3").Value = "CALLE TORTUGA No 313-A";
                    ws.Cell("A4").Value = "MANEADERO CP 22790";
                    ws.Cell("A5").Value = "BAJA CALIFORNIA";
                    ws.Style.Font.FontSize = 11;

                    ws.Range("A2:A10").Style.Font.Bold = true;
                    ws.Range("A7:A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //DIRECCIÓN DE ORIGEN
                    ws.Cell("A7").Value = "CUSTOMER: ";
                    ws.Cell("B7").Value = item.customer;

                    int ex_label = ds.contar_labels(item.id_packing_list);
                    if (ex_label != 0){
                        List<Labels> lista_etiquetas = new List<Labels>();
                        lista_etiquetas = ds.obtener_etiquetas(item.id_packing_list);
                        ws.Cell("A8").Value = "P.O.: ";
                        string label = Regex.Replace(item.pedido, @"\s+", " ") + "(PO# ";
                        foreach (Labels l in lista_etiquetas) { label += " " + l.label; }
                        if (ex_label == 1){
                            label += " )" + " (With UCC Labels) " + item.parte;
                        }else{
                            label += " )" + " (With TPM Labels) " + item.parte;
                        }
                        ws.Cell("B8").Value = label;
                    }else{
                        ws.Cell("A8").Value = "P.O.: ";
                        ws.Cell("B8").Value = Regex.Replace(item.pedido, @"\s+", " ") + "(Without UCC Labels) " + item.parte;
                    }
                    ws.Cell("A9").Value = "RETAILER: ";
                    ws.Cell("B9").Value = item.customer_po;
                    if (item.tipo != "1"){
                        ws.Cell("A10").Value = "EXAMPLES ";
                        ws.Cell("A10").Style.Font.FontSize = 14;
                    }

                    //IMAGEN AL CENTRO
                    ws.Range(1, 7, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    var imagePath = Server.MapPath("/") + "/Content/img/LOGOFORTUNEEXCEL.png";
                    //var imagePath = @"C:\Users\melissa\source\repos\FortuneSys----\FortuneSystem\Content\img\LOGO FORTUNE.png";
                    //var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1")).Scale(0.30);
                    var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1"));
                    //PK ABAJO DE LA IMAGEN
                    ws.Cell("D7").Value = "PK: ";
                    ws.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D7").Style.Font.Bold = true;
                    ws.Cell("E7").Value = item.packing;
                    ws.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range("E7:F7").Merge();
                    ws.Range("E7", "F7").Style.Font.Bold = true;
                    ws.Range("D7", "F7").Style.Font.FontSize = 15;
                    ws.Range("E7:F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:F7").Style.Border.BottomBorderColor = XLColor.Black;
                    //DIRECCION DE DESTINO
                    ws.Cell("L2").Value = "SHIP TO: ";
                    ws.Cell("L3").Value = item.destino.nombre;
                    ws.Cell("L4").Value = item.destino.direccion;
                    ws.Cell("L5").Value = item.destino.ciudad + " " + item.destino.zip;
                    ws.Cell("L8").Value = "DATE:" + item.fecha;
                    ws.Range("L2", "L10").Style.Font.Bold = true;
                    var columna_a = ws.Range("A2", "A10");
                    ws.Rows("6").Height = 30;
 /****************T*A*B*L*A********************************************************************************************************************************************************/
 /****************T*A*B*L*A********************************************************************************************************************************************************/
 /****************T*A*B*L*A********************************************************************************************************************************************************/
                    int contador_cabeceras = 0, contador_tallas = 0, pallets = 0, tiendas = 0, dc = 0,ppk=0,bp=0,ass=0;
                    int suma_estilo, suma_cajas ;
                    List<estilos> lista_descripciones_finales = new List<estilos>();
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();//=  "ID", "P.O. NUM", "TYPE", "COLOR", "DESCRIPTION";
                    titulos.Add("ID"); titulos.Add("P.O. NUM"); titulos.Add("TYPE");
                    foreach (Tarima t in item.lista_tarimas){ //REVISAN TIPOS DE EMPAQUE, DATOS, DC,ETC
                        foreach (estilos e in t.lista_estilos){
                            if (e.store != "N/A" && e.store != "NA") { tiendas++; }
                            if (e.index_dc != 0) { dc++; }
                            if (e.tipo_empaque == 1) { bp++; }
                            if (e.tipo_empaque == 2) { ppk++; }
                            if (e.tipo_empaque == 3) { ass++; }
                        }
                    }
                    if (ass != 0) { titulos.Add("ASSORTMENT"); }
                    titulos.Add("STYLE"); titulos.Add("COLOR"); titulos.Add("DESCRIPTION");                    
                    if (tiendas != 0) { titulos.Add("STORE"); }
                    if (dc != 0) { titulos.Add("DC"); }
                    if (ppk!=0){ titulos.Add("RATIO"); }                        
                    //OBTENER_TODAS LAS TALLAS DE TODOS LOS ESTILOS
                    //OBTENER_TODAS LAS TALLAS DE TODOS LOS ESTILOS
                    foreach (Tarima t in item.lista_tarimas){
                        foreach (estilos e in t.lista_estilos){
                            if (e.tipo_empaque != 3) {
                                foreach (ratio_tallas ra in e.lista_ratio){
                                    bool isEmpty = !tallas.Any();
                                    if (isEmpty){
                                        Talla ta = new Talla();
                                        ta.id_talla = ra.id_talla;
                                        ta.talla = ra.talla;
                                        tallas.Add(ta);
                                    }else{
                                        int existe = 0;
                                        foreach (Talla sizes in tallas){
                                            if (sizes.id_talla == ra.id_talla){ existe++; }
                                        }
                                        if (existe == 0){
                                            Talla ta = new Talla();
                                            ta.id_talla = ra.id_talla;
                                            ta.talla = ra.talla;
                                            tallas.Add(ta);
                                        }
                                    }
                                } 
                            } else {
                                foreach (estilos ea in e.assort.lista_estilos) {
                                    foreach (ratio_tallas ras in ea.lista_ratio) {
                                        bool isEmpty = !tallas.Any();
                                        if (isEmpty){
                                            Talla ta = new Talla();
                                            ta.id_talla = ras.id_talla;
                                            ta.talla = ras.talla;
                                            tallas.Add(ta);
                                        }else{
                                            int existe = 0;
                                            foreach (Talla sizes in tallas){
                                                if (sizes.id_talla == ras.id_talla) { existe++; }
                                            }
                                            if (existe == 0){
                                                Talla ta = new Talla();
                                                ta.id_talla = ras.id_talla;
                                                ta.talla = ras.talla;
                                                tallas.Add(ta);
                                            }
                                        }
                                    }
                                }
                            }
                                                       
                        }
                        //OBTENER_TODAS LAS TALLAS DE TODOS LOS ESTILOS
                    } //OBTENER_TODAS LAS TALLAS DE TODOS LOS ESTILOS<-----TALLAS
                    List<int> tallas_id_temporal = new List<int>();
                    contador_tallas = 0;
                    foreach (Talla sizes in tallas){
                        titulos.Add(sizes.talla); //AQUI AGREGO LAS TALLAS A LA CABECERA
                        tallas_id_temporal.Add(sizes.id_talla);
                        contador_tallas++;
                    }
                    titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");
                    foreach (string s in titulos) { contador_cabeceras++; }
                    headers.Add(titulos.ToArray());
                    int total_titulos = (titulos.ToArray()).Length;
                    ws.Cell(11, 1).Value = headers;
                    ws.Column(2).AdjustToContents();
                    ws.Column(5).AdjustToContents();
                    for (int i = 1; i <= total_titulos; i++){
                        ws.Cell(11, i).Style.Font.Bold = true;
                        ws.Cell(11, i).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(11, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(11, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.RightBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.TopBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    ws.Rows("6").Height = 30;
 /**********************************************************************************************************************************************************************************************/
 /***********************C*A*B*E*C*E*R*A*S**D*E**L*A**T*A*B*L*A*********************************************************************************************************************************/
                    //int[] tallas_comparacion = tallas_id_temporal.ToArray(); 
                    foreach (Tarima tarimas in item.lista_tarimas) { pallets++; }
                    int[] sumas_tallas = new int[contador_tallas + 2];
                    for (int i = 0; i < contador_tallas + 2; i++) { sumas_tallas[i] = 0; }
                    r = 12;

                    //*************************************************************************T*A*R*I*M*A*S********************************************************************************************************
                    //*************************************************************************T*A*R*I*M*A*S********************************************************************************************************
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        suma_estilo = 0; suma_cajas = 0; estilos_total = 0;
                        
                        var celdas_estilos_i = new List<String[]>();
                        var celdas_estilos = new List<String[]>();
                        List<Tarima> index_dcs = new List<Tarima>();//BUSCO LOS INDEX DE LOS DC
                        //BUSCO LOS INDEX DE LOS DC Y CUENTO ESTILOS DE LA TARIMA
                        foreach (estilos estilo in tarimas.lista_estilos){//BUSCO LOS INDEX DE LOS DC Y CUENTO ESTILOS DE LA TARIMA
                            if (estilo.index_dc != 0){
                                bool isEmpty = !index_dcs.Any();
                                if (isEmpty){
                                    Tarima ta = new Tarima();
                                    ta.id_tarima = estilo.index_dc;
                                    index_dcs.Add(ta);
                                    estilos_total++;
                                }else{
                                    int existe = 0;
                                    foreach (Tarima index in index_dcs){
                                        if (index.id_tarima == estilo.index_dc) { existe++; }
                                    }
                                    if (existe == 0){
                                        Tarima ta = new Tarima();
                                        ta.id_tarima = estilo.index_dc;
                                        index_dcs.Add(ta);
                                        estilos_total++;
                                    }
                                }
                            }else{
                                if (estilo.tipo_empaque != 3){
                                    estilos_total++;
                                }else{
                                    foreach (estilos e in estilo.assort.lista_estilos) { estilos_total++; }
                                    //estilos_total = estilos_total - 1; //RESTO UNO PARA QUE NO SE PASE 
                                }
                            }
                        }
                        ws.Cell(r, 1).Value = tarimas.id_tarima;
                        ws.Range(r, 1, (r + (estilos_total - 1)), 1).Merge();
                        foreach (Talla sizes in tallas) { sizes.total = 0; }
                        
                        tallas_bpdc = tallas;
                        foreach (Tarima ta in index_dcs)//INDICES
                        {
                            int contador_index = 0; suma_estilo = 0; suma_cajas = 0;
                            List<String> datos_dc_bp = new List<string>();
                            foreach (Talla sizes in tallas_bpdc) { sizes.total = 0; sizes.ratio = 0; }
                            foreach (estilos estilo in tarimas.lista_estilos)
                            {
                                if (estilo.index_dc == ta.id_tarima && estilo.tipo_empaque == 1)
                                {
                                    if (contador_index == 0)
                                    {
                                        datos_dc_bp.Add(Convert.ToString(estilo.number_po));
                                        if (estilo.tipo != "NONE") { datos_dc_bp.Add(estilo.tipo); }
                                        else { datos_dc_bp.Add(" "); }
                                        if (ass != 0) { datos_dc_bp.Add(" "); }
                                        datos_dc_bp.Add(Regex.Replace(estilo.estilo + estilo.ext + item.siglas_cliente, @"\s+", ""));
                                        datos_dc_bp.Add(Regex.Replace(estilo.color , @"\s+", " ")); 
                                        datos_dc_bp.Add(Regex.Replace( estilo.descripcion, @"\s+", " ")); 
                                        if (tiendas != 0) { datos_dc_bp.Add(estilo.store); }
                                        if (dc != 0) { datos_dc_bp.Add(estilo.dc); }
                                        if (ppk != 0) { datos_dc_bp.Add(" "); }
                                        //datos.Add(estilo.dc);
                                        contador_index++;
                                    }
                                    foreach (Talla sizes in tallas_bpdc){
                                        if (sizes.id_talla == estilo.id_talla){
                                            sizes.total = estilo.boxes;
                                            sizes.ratio = ds.buscar_piezas_empaque_bull(estilo.id_po_summary, estilo.id_talla);
                                        }
                                    }
                                    estilo.usado = 1;
                                }//*******IF                                
                            }//*****************************E*S*T*I*L*O*S*****I*N*D*I*C*E*S*****************************
                            int i = 0;
                            foreach (Talla sizes in tallas_bpdc)
                            {
                                if (sizes.total != 0)
                                {
                                    datos_dc_bp.Add(Convert.ToString(sizes.total));
                                    sumas_tallas[i] += sizes.total;
                                    suma_estilo += sizes.total;
                                    suma_cajas += (sizes.total / sizes.ratio);
                                }
                                else
                                {
                                    datos_dc_bp.Add(" ");
                                }
                                i++;
                            }
                            int ct = contador_tallas;
                            sumas_tallas[ct] += suma_estilo;
                            sumas_tallas[ct + 1] += suma_cajas;
                            //total_cajas += suma_cajas;
                            datos_dc_bp.Add(Convert.ToString(suma_estilo));
                            datos_dc_bp.Add(Convert.ToString(suma_cajas));
                            celdas_estilos.Add(datos_dc_bp.ToArray());
                            /*ws.Cell(r, 2).Value = celdas_estilos_i;//// <-------------THIS!!
                            r++;*/
                        }//*************I*N*D*I*C*E*S*******************************************************************

                        int estilos_capturados = 0;
                        //*************E*S*T*I*L*O*S**N*O*R*M*A*L*E*S**************************************************
                        foreach (estilos estilo in tarimas.lista_estilos)
                        {
                            
                            suma_estilo = 0; suma_cajas = 0;
                            List<String> datos = new List<string>(); //CREO LISTA PARA LOS DATOS
                            if (estilo.usado == 0)
                            {
                                if (estilo.tipo_empaque == 3)
                                {
                                    int estilos_assort = 0, estilos_tarima = 0;
                                    foreach (estilos e in estilo.assort.lista_estilos) { estilos_assort++; }
                                    estilos_tarima += estilos_assort;
                                    int contador_assort = 0;
                                    //ESTILOS DEL ASSORT//ESTILOS DEL ASSORT
                                    foreach (estilos e in estilo.assort.lista_estilos)
                                    {//ESTILOS DEL ASSORT
                                        List<String> datos_a = new List<string>();
                                        datos_a.Add(Convert.ToString(estilo.number_po)); //AGREGO EL PO NUMBER
                                        if (estilo.tipo != "NONE") { datos_a.Add(estilo.tipo); } //HASTA AQUÍ VA LA COLUMNA DE "TIPO"
                                        else { datos_a.Add(" "); }//TIPO SI NO LLEVA NADA
                                        datos_a.Add(Regex.Replace(estilo.assort_nombre, @"\s+", " ")); 
                                        datos_a.Add(Regex.Replace(e.estilo + estilo.ext + item.siglas_cliente, @"\s+", ""));
                                        datos_a.Add(Regex.Replace( e.color, @"\s+", " ")); 
                                        datos_a.Add(Regex.Replace(e.descripcion , @"\s+", " ")); 
                                        suma_estilo = 0; suma_cajas = 0;
                                        if (tiendas != 0) { datos_a.Add(e.store); }
                                        if (dc != 0) { datos_a.Add(" "); }//DATOS ANTES DE LAS CANTIDADES
                                        if (ppk != 0) { datos_a.Add(" "); }//RATIO 
                                        int aa = 0;
                                        foreach (Talla sizes in tallas)
                                        {//CANTIDADES
                                            int existio_a = 0;
                                            foreach (ratio_tallas ratio in e.lista_ratio)
                                            {
                                                if (sizes.id_talla == ratio.id_talla)
                                                {
                                                    existio_a++;
                                                    datos_a.Add(Convert.ToString(ratio.ratio * estilo.boxes));//SE MULTIPLICA POR EL TOTAL DE CARTONES
                                                    suma_estilo += (ratio.ratio * estilo.boxes);
                                                    //suma_cajas = estilo.boxes;
                                                    //sumas_tallas[aa] += (ratio.ratio * estilo.boxes);
                                                }
                                            }
                                            if (existio_a == 0) { datos_a.Add(" "); }
                                            aa++;
                                        }
                                        int ct = contador_tallas;
                                        if (contador_assort == 0)
                                        {
                                            suma_cajas = estilo.boxes;
                                        }
                                        sumas_tallas[ct] += suma_estilo;
                                        sumas_tallas[ct + 1] += suma_cajas;
                                        datos_a.Add(Convert.ToString(suma_estilo));
                                        datos_a.Add(Convert.ToString(suma_cajas));
                                        celdas_estilos.Add(datos_a.ToArray());
                                        estilos_capturados++;
                                        if (contador_assort == 0)
                                        {//PONER LAS CAJAS / CARTONES DE EL ASSORT
                                            ws.Cell(r+ estilos_capturados-1, (contador_cabeceras - 1)).Value = estilo.boxes;
                                            ws.Range(r+ estilos_capturados-1, (contador_cabeceras - 1), ((r+ estilos_capturados-1) + estilos_assort - 1), (contador_cabeceras - 1)).Merge();
                                            contador_assort++;
                                            //sumas_tallas[ct + 1] += estilo.boxes;
                                            ws.Cell(r + estilos_capturados - 1, 4).Value = Regex.Replace(estilo.assort_nombre, @"\s+", " ");
                                            ws.Range(r + estilos_capturados - 1, 4, ((r + estilos_capturados - 1) + (estilos_assort - 1)), 4).Merge();
                                        }
                                        
                                    }//ESTILOS DEL ASSORT
                                }else{//SI EL TIPO DE EMPAQUE ES 1 O 2
                                    datos.Add(Convert.ToString(estilo.number_po)); //AGREGO EL PO NUMBER
                                    if (estilo.tipo != "NONE") { datos.Add(estilo.tipo); } //HASTA AQUÍ VA LA COLUMNA DE "TIPO"
                                    else { datos.Add(" "); }//TIPO SI NO LLEVA NADA
                                    if (ass != 0) { datos.Add(" "); }
                                    datos.Add(Regex.Replace(estilo.estilo + estilo.ext + item.siglas_cliente, @"\s+", ""));
                                    datos.Add(Regex.Replace(estilo.color , @"\s+", " ")); 
                                    datos.Add(Regex.Replace(estilo.descripcion , @"\s+", " ")); 
                                    suma_estilo = 0; suma_cajas = 0;
                                    if (tiendas != 0) { datos.Add(estilo.store); }
                                    if (dc != 0) { datos.Add(estilo.dc); }
                                    switch (estilo.tipo_empaque)
                                    {
                                        case 1:
                                            if (ppk != 0) { datos.Add(" "); }
                                            int i;
                                            if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "ECOM"){
                                                i = 0;
                                                foreach (Talla sizes in tallas) {
                                                    int existe_talla = 0;
                                                    foreach (ratio_tallas ra in estilo.lista_ratio){
                                                        if(sizes.id_talla == ra.id_talla){
                                                            datos.Add(Convert.ToString(ra.ratio)); //SI COINCIDEN AGREGO LA CANTIDAD DE PIEZAS
                                                            //suma_estilo += estilo.boxes; //SUMO EL TOTAL DE PIEZAS -ROW-                                                    
                                                            //suma_cajas += (estilo.boxes / ds.buscar_piezas_empaque_bull(estilo.id_po_summary, estilo.id_talla));
                                                            sumas_tallas[i] += ra.ratio;//SUMO LAS PIEZAS POR TALLA
                                                            existe_talla++;
                                                        }
                                                    }
                                                    if (existe_talla == 0) {
                                                        datos.Add(" ");
                                                    }
                                                    i++;
                                                }
                                                suma_estilo += estilo.boxes;
                                                suma_cajas++;
                                            }else {
                                                i = 0;
                                                foreach (Talla sizes in tallas)
                                                { //SI EL TIPO DE EMPAQUE ES 1 BP
                                                    if (sizes.id_talla == estilo.id_talla)
                                                    { //BUSCO EN EL ORDEN DE LA LÍNEA DE TALLAS
                                                        datos.Add(Convert.ToString(estilo.boxes)); //SI COINCIDEN AGREGO LA CANTIDAD DE PIEZAS
                                                        suma_estilo += estilo.boxes; //SUMO EL TOTAL DE PIEZAS -ROW-                                                    
                                                        suma_cajas += (estilo.boxes / ds.buscar_piezas_empaque_bull(estilo.id_po_summary, estilo.id_talla));
                                                        sumas_tallas[i] += estilo.boxes;//SUMO LAS PIEZAS POR TALLA
                                                    }
                                                    else { datos.Add(" "); }
                                                    i++;
                                                }
                                            }                                            
                                            break;
                                        case 2:
                                            int ii = 0;
                                            if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "ECOM")
                                            {
                                                datos.Add(" ");
                                            }
                                            else
                                            {
                                                contador = 0; string ppk_ratio = ""; total_ratio = 0;
                                                foreach (ratio_tallas ratio in estilo.lista_ratio)
                                                {
                                                    contador++; ppk_ratio += ratio.ratio;
                                                    //if (contador < total_ratio) { ppk_ratio += "-"; }
                                                    if (contador < ((estilo.lista_ratio).Count)) { ppk_ratio += "-"; }
                                                }
                                                datos.Add(ppk_ratio);
                                            }//RATIO
                                            foreach (Talla sizes in tallas)
                                            {
                                                int existio = 0;
                                                foreach (ratio_tallas ratio in estilo.lista_ratio)
                                                {
                                                    if (sizes.id_talla == ratio.id_talla)
                                                    {
                                                        existio++;
                                                        datos.Add(Convert.ToString(ratio.ratio * estilo.boxes));
                                                        suma_estilo += (ratio.ratio * estilo.boxes);
                                                        if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "ECOM")
                                                        {
                                                            suma_cajas++;
                                                        }
                                                        else
                                                        {
                                                            suma_cajas = estilo.boxes;
                                                        }
                                                        sumas_tallas[ii] += (ratio.ratio * estilo.boxes);
                                                    }
                                                }
                                                if (existio == 0) { datos.Add(" "); }
                                                ii++;
                                            }
                                            break;
                                    }//SWITCH
                                    int ct = contador_tallas;
                                    sumas_tallas[ct] += suma_estilo;
                                    sumas_tallas[ct + 1] += suma_cajas;
                                    datos.Add(Convert.ToString(suma_estilo));
                                    datos.Add(Convert.ToString(suma_cajas));
                                    estilos_capturados++;
                                    celdas_estilos.Add(datos.ToArray());
                                }//SI EL TIPO DE EMPAQUE ES 1 O 2
                            }//IF estilo.usado == 0
                        }//*************E*S*T*I*L*O*S**N*O*R*M*A*L*E*S**************************************************
                        ws.Cell(r, 2).Value = celdas_estilos;//// <-------------THIS!!
                        ws.Cell(r, contador_cabeceras).Value = "1";//PALLET
                        ws.Range(r, contador_cabeceras, (r + estilos_total - 1), contador_cabeceras).Merge();
                        r = r + (estilos_total);
                    }//************T*A*R*I*M*A*S*******************************************************************
                    //*************************************************************************T*A*R*I*M*A*S************************************************************************************
                    //***************************************************************************T*A*R*I*M*A*S********************************************************************************************************


                    /****************************************************************************************************************************************************************************************/
                    /****************************************************************************************************************************************************************************************/
                    /****************************************************************************************************************************************************************************************/
                    contador = 0;
                    string descripcion_final = "";
                    /*estilos desc = new estilos();
                    desc.descripcion = estilo.des
                    lista_descripciones_finales.Add();*/
                    foreach (Tarima tarimas in item.lista_tarimas){
                        foreach (estilos estilo in tarimas.lista_estilos){
                            bool isEmpty = !lista_descripciones_finales.Any();
                            if (isEmpty){
                                estilos desc = new estilos();
                                desc.id_estilo = estilo.id_estilo;
                                desc.descripcion = estilo.descripcion_final;
                                lista_descripciones_finales.Add(desc);
                                descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                            }else{
                                int existencia = 0;
                                foreach (estilos e in lista_descripciones_finales) {
                                    if (e.id_estilo==estilo.id_estilo) {
                                        existencia++;
                                    }
                                }
                                if (existencia == 0) {
                                    estilos desc = new estilos();
                                    desc.id_estilo = estilo.id_estilo;
                                    desc.descripcion = estilo.descripcion_final;
                                    lista_descripciones_finales.Add(desc);
                                    descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                                }
                            }
                        }
                    }

                    ws.Cell(r, 1).Value = descripcion_final;
                    ws.Cell(r, 1).Style.Font.Bold = true;
                    ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(r, 1, r, 4).Merge();
                    ws.Range(r, 1, r, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);

                    ws.Cell(r, 5).Value = "TOTAL";
                    ws.Cell(r, 5).Style.Font.Bold = true;
                    ws.Cell(r, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c = 6;
                    if (dc != 0) { c++; }
                    if (ppk != 0) { c++; }//POR EL RATIO
                    if (ass != 0) { c++; }
                    if (tiendas != 0) { c++; }
                    for (int i = 12; i < r; i++){
                        for (int j = 1; j <= c; j++){
                            ws.Column(c).AdjustToContents(r, c);
                        }
                    }
                    ws.Range(r, 5, r, c).Merge();
                    ws.Range(r, 5, r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    c++;

                    for (int i = 0; i < contador_tallas + 2; i++){
                        ws.Cell(r, c).Value = sumas_tallas[i];
                        ws.Cell(r, c).Style.Font.Bold = true;
                        ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        c++;
                    }
                    ws.Cell(r, c).Value = pallets;
                    ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    ws.Cell(r, c).Style.Font.Bold = true;
                    ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    filas = r;
                    
                    ws.Column(2).AdjustToContents(12,2);
                    ws.Column(3).AdjustToContents(12,3);
                    ws.Column(4).AdjustToContents(12,4);
                    for (int i = 12; i <= r; i++){
                        for (int j = 1; j <= c; j++){
                            ws.Cell(i, j).Style.Font.FontSize = 9;
                            ws.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(i, j).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.LeftBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.RightBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.TopBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.BottomBorderColor = XLColor.Black;

                        }
                    }


                    filas += 2;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Font.Bold = true;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Font.Bold = true;
                    ws.Range(filas, 2, (filas + 2), 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(filas, 8, (filas + 2), 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filas, 1).Value = "DRIVER NAME:";
                    ws.Cell(filas, 2).Value = item.conductor.driver_name;
                    ws.Cell(filas, 7).Value = "SHIPPING MANAGER:";
                    ws.Cell(filas, 8).Value = item.shipping_manager;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "TRAILER/PLATES:";
                    ws.Cell(filas, 2).Value = item.conductor.tractor + "/" + item.conductor.plates;
                    ws.Cell(filas, 7).Value = "SEAL:";
                    ws.Cell(filas, 8).Value = item.seal;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "CONTAINER/PLATES:";
                    ws.Cell(filas, 2).Value = item.contenedor.eco + "/" + item.contenedor.plates;
                    ws.Cell(filas, 7).Value = "REPLACEMENT:";
                    ws.Cell(filas, 8).Value = item.replacement;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 3).Value = "DOCUMENTO CONTROLADO. ÚNICAMENTE SE PUEDE MODIFICAR POR EL SUPERVISOR DE SHIPPING";
                    ws.Cell(filas, 3).Style.Font.FontColor = XLColor.FromArgb(100, 100, 100);
                    filas += 2;
                    columnas = 2;

                    var porcentajes = new List<String[]>();
                    List<String> p = new List<string>();
                    p.Add("ORIGIN"); p.Add("QTY"); p.Add("%");
                    porcentajes.Add(p.ToArray());
                    List<Fabricantes> totales_paises_estilo = new List<Fabricantes>();
                    List<Fabricantes> totales_paises = new List<Fabricantes>();
                    int add = 0, total_paises = 0, iguales;
                    foreach (Tarima tarimas in item.lista_tarimas){
                        totales_paises_estilo = ds.buscar_paises_estilos(tarimas.lista_estilos);
                        foreach (Fabricantes fa in totales_paises_estilo){
                            iguales = 0;
                            if (add == 0){
                                Fabricantes nf = new Fabricantes();
                                nf.cantidad = fa.cantidad;
                                nf.pais = fa.pais;
                                nf.percent = fa.percent;
                                totales_paises.Add(nf);
                                add++;
                            }else{
                                foreach (Fabricantes f in totales_paises.ToList()){
                                    if (f.pais == fa.pais){
                                        f.cantidad = fa.cantidad;
                                        iguales++;
                                    }
                                }
                                if (iguales == 0){
                                    Fabricantes nf = new Fabricantes();
                                    nf.cantidad = fa.cantidad;
                                    nf.pais = fa.pais;
                                    nf.percent = fa.percent;
                                    totales_paises.Add(nf);
                                }
                                add++;
                            }
                        }
                    }
                    foreach (Fabricantes f in totales_paises) { total_paises += f.cantidad; }
                    foreach (Fabricantes f in totales_paises) { f.porcentaje = (f.cantidad * 100) / total_paises; }
                    List<Fabricantes> totales_paises_envio = new List<Fabricantes>();
                    foreach (Fabricantes f in totales_paises){
                        Fabricantes nf = new Fabricantes();
                        double x = (((sumas_tallas[sumas_tallas.Length - 2]) * f.cantidad) / total_paises);
                        nf.cantidad = Convert.ToInt32(Math.Round(x));
                        nf.pais = f.pais;
                        nf.percent = f.percent;
                        totales_paises_envio.Add(nf);
                    }
                    foreach (Fabricantes f in totales_paises_envio){
                        porcentajes.Add(new string[] { f.pais, (f.cantidad).ToString(), f.percent });
                    }
                    ws.Cell(filas, 2).Value = "%";
                    ws.Cell(filas, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    filas++;
                    ws.Cell(filas, 2).Value = porcentajes;
                }
                ws.Rows().AdjustToContents();
                //ws.Columns().AdjustToContents();

                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"" + archivo + ".xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();

            }
        }




        public void excel_pk_ppk(List<Pk> lista)
        {
            
            string clave_packing = "";
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            {
                int estilos_total = 0, rows = 0, tarima_contador = 0, total_ratio, contador, r, c, total_cajas = 0, filas = 0, columnas = 0, tallas_id;

                var ws = libro_trabajo.Worksheets.Add("PK");
                foreach (Pk item in lista)
                {
                    //item.tipo_empaque = 2;                    
                    clave_packing = item.packing;
                    /*****INICIO CON DIRECCIONES, LOGO, ETC******/
                    ws.Cell("A2").Value = "FORTUNE FASHIONS BAJA, S.R.L. DE C.V.";
                    ws.Cell("A3").Value = "CALLE TORTUGA No 313-A";
                    ws.Cell("A4").Value = "MANEADERO CP 22790";
                    ws.Cell("A5").Value = "BAJA CALIFORNIA";
                    ws.Style.Font.FontSize = 11;

                    ws.Range("A2:A10").Style.Font.Bold = true;
                    ws.Range("A7:A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //DIRECCIÓN DE ORIGEN
                    ws.Cell("A7").Value = "CUSTOMER: ";
                    ws.Cell("B7").Value = item.customer;

                    int ex_label = ds.contar_labels(item.id_packing_list);
                    if (ex_label != 0)
                    {
                        List<Labels> lista_etiquetas = new List<Labels>();
                        lista_etiquetas = ds.obtener_etiquetas(item.id_packing_list);
                        ws.Cell("A8").Value = "P.O.: ";
                        string label = Regex.Replace(item.pedido, @"\s+", " ") + "(PO# ";
                        foreach (Labels l in lista_etiquetas) { label += " " + l.label; }
                        if (ex_label == 1)
                        {
                            label += " )" + " With UCC Labels " + item.parte;
                        }
                        else
                        {
                            label += " )" + " With TPM Labels " + item.parte;
                        }
                        ws.Cell("B8").Value = label;
                    }
                    else
                    {
                        ws.Cell("A8").Value = "P.O.: ";
                        ws.Cell("B8").Value = Regex.Replace(item.pedido, @"\s+", " ") + " Without UCC Labels " + item.parte;
                    }

                    ws.Cell("A9").Value = "RETAILER: ";
                    ws.Cell("B9").Value = item.customer_po;
                    if (item.tipo != "1")
                    {
                        ws.Cell("A10").Value = "EXAMPLES ";
                        ws.Cell("A10").Style.Font.FontSize = 14;
                    }

                    //IMAGEN AL CENTRO
                    ws.Range(1, 7, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    var imagePath = Server.MapPath("/") + "/Content/img/LOGOFORTUNEEXCEL.png";
                    //var imagePath = @"C:\Users\melissa\source\repos\FortuneSys----\FortuneSystem\Content\img\LOGO FORTUNE.png";
                    var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1")).Scale(0.30);
                    //PK ABAJO DE LA IMAGEN
                    ws.Cell("D7").Value = "PK: ";
                    ws.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D7").Style.Font.Bold = true;
                    ws.Cell("E7").Value = item.packing;
                    ws.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range("E7:F7").Merge();
                    ws.Range("E7", "F7").Style.Font.Bold = true;
                    ws.Range("D7", "F7").Style.Font.FontSize = 15;
                    ws.Range("E7:F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:F7").Style.Border.BottomBorderColor = XLColor.Black;
                    //DIRECCION DE DESTINO
                    ws.Cell("L2").Value = "SHIP TO: ";
                    ws.Cell("L3").Value = item.destino.nombre;
                    ws.Cell("L4").Value = item.destino.direccion;
                    ws.Cell("L5").Value = item.destino.ciudad + " " + item.destino.zip;
                    ws.Cell("L8").Value = "DATE:" + item.fecha;
                    ws.Range("L2", "L10").Style.Font.Bold = true;
                    var columna_a = ws.Range("A2", "A10");
                    ws.Rows("6").Height = 30;
                    /****************T*A*B*L*A************************************************/
                    int contador_cabeceras = 0, contador_tallas = 0, pallets = 0, tiendas = 0;
                    //PPK
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();//=  "ID", "P.O. NUM", "TYPE", "COLOR", "DESCRIPTION";
                    titulos.Add("ID"); titulos.Add("P.O. NUM"); titulos.Add("TYPE"); titulos.Add("STYLE"); titulos.Add("COLOR"); titulos.Add("DESCRIPTION");
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (estilos e in t.lista_estilos)
                        {
                            if (e.store != "N/A" && e.store != "NA")
                            {
                                tiendas++;
                            }
                        }
                    }
                    if (tiendas != 0) { titulos.Add("STORE"); }
                   // if (item.dc != 0) { titulos.Add("DC"); }
                    titulos.Add("PPK");
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (estilos e in t.lista_estilos)
                        {
                            if (contador_cabeceras == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                            {
                                foreach (ratio_tallas ra in e.lista_ratio)
                                {
                                    titulos.Add(ra.talla);
                                }
                                contador_cabeceras++;
                            }
                        }
                    }
                    titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");
                    headers.Add(titulos.ToArray());
                    int total_titulos = (titulos.ToArray()).Length;
                    ws.Cell(11, 1).Value = headers;
                    ws.Column(2).AdjustToContents();
                    ws.Column(5).AdjustToContents();
                    for (int i = 1; i <= total_titulos; i++)
                    {
                        ws.Cell(11, i).Style.Font.Bold = true;
                        ws.Cell(11, i).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(11, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(11, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.RightBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.TopBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    ws.Rows("6").Height = 30;
                    /***********************************************************************************/
                    List<int> tallas_id_temporal = new List<int>();
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador_tallas == 0)
                        {
                            foreach (estilos e in tarimas.lista_estilos)
                            {
                                if (contador_tallas == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (var ratio in e.lista_ratio)
                                    {
                                        contador_tallas++;
                                    }
                                }
                            }
                        }
                    }
                    int temporal = 0;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (estilos e in tarimas.lista_estilos)
                        {
                            if (temporal == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                            {
                                foreach (var ratio in e.lista_ratio)
                                {
                                    tallas_id_temporal.Add(ratio.id_talla);
                                }
                                temporal++;
                            }
                        }
                    }

                    //tallas_id_temporal.Add(ratio.id_talla);
                    int[] tallas_comparacion = tallas_id_temporal.ToArray();

                    foreach (Tarima tarimas in item.lista_tarimas) { pallets++; }

                    int[] sumas_tallas = new int[contador_tallas + 2];
                    for (int i = 0; i < contador_tallas + 2; i++) { sumas_tallas[i] = 0; }

                    r = 12;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        estilos_total = 0; rows = 0; tarima_contador = 0; rows = 0;
                        foreach (estilos estilo in tarimas.lista_estilos) { estilos_total++; }
                        ws.Cell(r, 1).Value = tarimas.id_tarima;
                        ws.Range(r, 1, (r + estilos_total - 1), 1).Merge();
                        var celdas_estilos = new List<String[]>();
                        foreach (estilos estilo in tarimas.lista_estilos)
                        {
                            List<String> datos = new List<string>();
                            datos.Add(Convert.ToString(estilo.number_po));
                            if (estilo.tipo != "NONE") { datos.Add(estilo.tipo); }
                            else { datos.Add(" "); }
                            datos.Add(estilo.estilo);
                            datos.Add(estilo.color);
                            datos.Add(estilo.descripcion);
                            if (tiendas != 0) { datos.Add(estilo.store); }
                            //if (item.dc != 0) { datos.Add(Convert.ToString(estilo.dc)); }
                            total_ratio = 0;
                            foreach (ratio_tallas ratio in estilo.lista_ratio) { total_ratio++; }
                            contador = 0;
                            string ppk = "";
                            if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                datos.Add("N/A");
                            }
                            else
                            {
                                foreach (ratio_tallas ratio in estilo.lista_ratio)
                                {
                                    contador++;
                                    ppk += ratio.ratio;
                                    if (contador < total_ratio) { ppk += "-"; }
                                }
                                datos.Add(ppk);
                            }

                            int ii = 0, total_talla, total_estilo = 0;
                            if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {

                                foreach (int tall in tallas_comparacion)
                                {
                                    foreach (ratio_tallas ratio in estilo.lista_ratio)
                                    {
                                        if (tall == ratio.id_talla)
                                        {
                                            total_talla = ratio.ratio * estilo.boxes;
                                            sumas_tallas[ii] += total_talla;
                                            total_estilo += total_talla;
                                            datos.Add(Convert.ToString(total_talla));
                                        }
                                        else
                                        {
                                            datos.Add("");
                                        }
                                        ii++;
                                        //tallas_id++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (ratio_tallas ratio in estilo.lista_ratio)
                                {
                                    total_talla = ratio.ratio * estilo.boxes;
                                    sumas_tallas[ii] += total_talla;
                                    total_estilo += total_talla;
                                    ii++;
                                    datos.Add(Convert.ToString(total_talla));
                                }
                            }

                            datos.Add(Convert.ToString(total_estilo));
                            sumas_tallas[ii] += total_estilo; ii++;
                            if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                datos.Add("1");
                                sumas_tallas[ii] += 1;
                            }
                            else
                            {
                                datos.Add(Convert.ToString(estilo.boxes));
                                sumas_tallas[ii] += estilo.boxes;
                            }
                            celdas_estilos.Add(datos.ToArray());
                        }//ESTILOS                            
                        ws.Cell(r, 2).Value = celdas_estilos;
                        c = 8;
                        //if (item.dc != 0) { c++; }
                        if (tiendas != 0) { c++; }
                        c = c + (contador_tallas + 2);
                        ws.Cell(r, c).Value = "1";
                        ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                        r += estilos_total;
                        columnas = c;
                    }//TARIMAS
                    contador = 0;
                    string descripcion_final = "";
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador == 0)
                        {
                            foreach (estilos estilo in tarimas.lista_estilos)
                            {
                                descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                            }
                            contador++;
                        }
                    }

                    ws.Cell(r, 1).Value = descripcion_final;
                    ws.Cell(r, 1).Style.Font.Bold = true;
                    ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(r, 1, r, 4).Merge();
                    ws.Range(r, 1, r, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


                    ws.Cell(r, 5).Value = "TOTAL";
                    ws.Cell(r, 5).Style.Font.Bold = true;
                    ws.Cell(r, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c = 7;
                    //if (item.dc != 0) { c++; }
                    if (tiendas != 0) { c++; }
                    ws.Range(r, 5, r, c).Merge();
                    ws.Range(r, 5, r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    c++;

                    for (int i = 0; i < contador_tallas + 2; i++)
                    {
                        ws.Cell(r, c).Value = sumas_tallas[i];
                        ws.Cell(r, c).Style.Font.Bold = true;
                        ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        c++;
                    }
                    ws.Cell(r, c).Value = pallets;
                    ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    ws.Cell(r, c).Style.Font.Bold = true;
                    ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    filas = r;
                    for (int i = 11; i <= (filas); i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            ws.Cell(i, j).Style.Font.FontSize = 9;
                            ws.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(i, j).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.LeftBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.RightBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.TopBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.BottomBorderColor = XLColor.Black;
                        }
                    }

                    //tabla------PPK

                    filas += 2;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Font.Bold = true;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Font.Bold = true;
                    ws.Range(filas, 2, (filas + 2), 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(filas, 8, (filas + 2), 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filas, 1).Value = "DRIVER NAME:";
                    ws.Cell(filas, 2).Value = item.conductor.driver_name;
                    ws.Cell(filas, 7).Value = "SHIPPING MANAGER:";
                    ws.Cell(filas, 8).Value = item.shipping_manager;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "TRAILER/PLATES:";
                    ws.Cell(filas, 2).Value = item.conductor.tractor + "/" + item.conductor.plates;
                    ws.Cell(filas, 7).Value = "SEAL:";
                    ws.Cell(filas, 8).Value = item.seal;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "CONTAINER/PLATES:";
                    ws.Cell(filas, 2).Value = item.contenedor.eco + "/" + item.contenedor.plates;
                    ws.Cell(filas, 7).Value = "REPLACEMENT:";
                    ws.Cell(filas, 8).Value = item.replacement;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 3).Value = "DOCUMENTO CONTROLADO. ÚNICAMENTE SE PUEDE MODIFICAR POR EL SUPERVISOR DE SHIPPING";
                    ws.Cell(filas, 3).Style.Font.FontColor = XLColor.FromArgb(100, 100, 100);
                    filas += 2;
                    columnas = 2;

                    var porcentajes = new List<String[]>();
                    List<String> p = new List<string>();
                    p.Add("ORIGIN"); p.Add("QTY");p.Add("%");
                    porcentajes.Add(p.ToArray());
                    List<Fabricantes> totales_paises_estilo = new List<Fabricantes>();
                    List<Fabricantes> totales_paises = new List<Fabricantes>();
                    int add = 0, total_paises = 0, iguales;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        totales_paises_estilo = ds.buscar_paises_estilos(tarimas.lista_estilos);
                        foreach (Fabricantes fa in totales_paises_estilo)
                        {
                            iguales = 0;
                            if (add == 0)
                            {
                                Fabricantes nf = new Fabricantes();
                                nf.cantidad = fa.cantidad;
                                nf.pais = fa.pais;
                                nf.percent = fa.percent;
                                totales_paises.Add(nf);
                                add++;
                            }
                            else
                            {
                                foreach (Fabricantes f in totales_paises.ToList())
                                {
                                    if (f.pais == fa.pais)
                                    {
                                        f.cantidad = fa.cantidad;
                                        iguales++;
                                    }
                                }
                                if (iguales == 0)
                                {
                                    Fabricantes nf = new Fabricantes();
                                    nf.cantidad = fa.cantidad;
                                    nf.pais = fa.pais;
                                    nf.percent = fa.percent;
                                    totales_paises.Add(nf);
                                }
                                add++;
                            }

                        }
                    }
                    foreach (Fabricantes f in totales_paises) { total_paises += f.cantidad; }
                    foreach (Fabricantes f in totales_paises) { f.porcentaje = (f.cantidad * 100) / total_paises; }
                    List<Fabricantes> totales_paises_envio = new List<Fabricantes>();
                    foreach (Fabricantes f in totales_paises)
                    {
                        Fabricantes nf = new Fabricantes();
                        double x = (((sumas_tallas[sumas_tallas.Length - 2]) * f.cantidad) / total_paises);
                        nf.cantidad = Convert.ToInt32(Math.Round(x));
                        nf.pais = f.pais;
                        nf.percent = f.percent;
                        totales_paises_envio.Add(nf);
                    }
                    foreach (Fabricantes f in totales_paises_envio)
                    {
                        porcentajes.Add(new string[] { f.pais, (f.cantidad).ToString(),f.percent });
                    }
                    ws.Cell(filas, 2).Value = "%";
                    ws.Cell(filas, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    filas++;
                    ws.Cell(filas, 2).Value = porcentajes;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();

                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Packing List " + clave_packing + ".xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();

            }
        }

        public void excel_pk_bp(List<Pk> lista)
        {
            
            string clave_packing = "";
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            {
                int estilos_total = 0, rows = 0, tarima_contador = 0, total_ratio, contador, r, c, total_cajas = 0, filas = 0, columnas = 0, tallas_id, piezas_estilo = 0;

                var ws = libro_trabajo.Worksheets.Add("PK");
                foreach (Pk item in lista){
                    //item.tipo_empaque = 2;                    
                    clave_packing = item.packing;
                    /*****INICIO CON DIRECCIONES, LOGO, ETC******/
                    ws.Cell("A2").Value = "FORTUNE FASHIONS BAJA, S.R.L. DE C.V.";
                    ws.Cell("A3").Value = "CALLE TORTUGA No 313-A";
                    ws.Cell("A4").Value = "MANEADERO CP 22790";
                    ws.Cell("A5").Value = "BAJA CALIFORNIA";
                    ws.Style.Font.FontSize = 11;

                    ws.Range("A2:A10").Style.Font.Bold = true;
                    ws.Range("A7:A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //DIRECCIÓN DE ORIGEN
                    ws.Cell("A7").Value = "CUSTOMER: ";
                    ws.Cell("B7").Value = item.customer;

                    int ex_label = ds.contar_labels(item.id_packing_list);
                    if (ex_label != 0){
                        List<Labels> lista_etiquetas = new List<Labels>();
                        lista_etiquetas = ds.obtener_etiquetas(item.id_packing_list);
                        ws.Cell("A8").Value = "P.O.: ";
                        string label = Regex.Replace(item.pedido, @"\s+", " ") + "(PO# ";
                        foreach (Labels l in lista_etiquetas) { label += " " + l.label; }
                        if (ex_label == 1){
                            label += " )" + " With UCC Labels " + item.parte;
                        }else{
                            label += " )" + " With TPM Labels " + item.parte;
                        }
                        ws.Cell("B8").Value = label;
                    }else{
                        ws.Cell("A8").Value = "P.O.: ";
                        ws.Cell("B8").Value = Regex.Replace(item.pedido, @"\s+", " ") + " Without UCC Labels " + item.parte;
                    }
                    ws.Cell("A9").Value = "RETAILER: ";
                    ws.Cell("B9").Value = item.customer_po;
                    if (item.tipo != "1"){
                        ws.Cell("A10").Value = "EXAMPLES ";
                        ws.Cell("A10").Style.Font.FontSize = 14;
                    }
                    //IMAGEN AL CENTRO
                    ws.Range(1, 7, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    var imagePath = Server.MapPath("/") + "/Content/img/LOGOFORTUNEEXCEL.png";
                    //var imagePath = @"C:\Users\melissa\source\repos\FortuneSys----\FortuneSystem\Content\img\LOGO FORTUNE.png";
                    var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1")).Scale(0.30);
                    //PK ABAJO DE LA IMAGEN
                    ws.Cell("D7").Value = "PK: ";
                    ws.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D7").Style.Font.Bold = true;
                    ws.Cell("E7").Value = item.packing;
                    ws.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range("E7:F7").Merge();
                    ws.Range("E7", "F7").Style.Font.Bold = true;
                    ws.Range("D7", "F7").Style.Font.FontSize = 15;
                    ws.Range("E7:F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:F7").Style.Border.BottomBorderColor = XLColor.Black;
                    //DIRECCION DE DESTINO
                    ws.Cell("L2").Value = "SHIP TO: ";
                    ws.Cell("L3").Value = item.destino.nombre;
                    ws.Cell("L4").Value = item.destino.direccion;
                    ws.Cell("L5").Value = item.destino.ciudad + " " + item.destino.zip;
                    ws.Cell("L8").Value = "DATE:" + item.fecha;
                    ws.Range("L2", "L10").Style.Font.Bold = true;
                    var columna_a = ws.Range("A2", "A10");
                    ws.Rows("6").Height = 30;
                    /****************T*A*B*L*A************************************************/
                    int contador_cabeceras = 0, contador_tallas = 0, pallets = 0, tiendas = 0;
                    //PPK
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();//=  "ID", "P.O. NUM", "TYPE", "COLOR", "DESCRIPTION";
                    titulos.Add("ID"); titulos.Add("P.O. NUM"); titulos.Add("TYPE"); titulos.Add("STYLE"); titulos.Add("COLOR"); titulos.Add("DESCRIPTION");
                    foreach (Tarima t in item.lista_tarimas){
                        foreach (estilos e in t.lista_estilos){
                            if (e.store != "N/A" && e.store != "NA") { tiendas++; }
                        }
                    }
                    if (tiendas != 0) { titulos.Add("STORE"); }
                    foreach (Tarima t in item.lista_tarimas){
                        foreach (estilos e in t.lista_estilos){
                            if (contador_cabeceras == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM"){
                                foreach (ratio_tallas ra in e.lista_ratio){
                                    titulos.Add(ra.talla);
                                }
                                contador_cabeceras++;
                            }
                        }
                    }
                    titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");
                    headers.Add(titulos.ToArray());
                    int total_titulos = (titulos.ToArray()).Length;
                    ws.Cell(11, 1).Value = headers;
                    ws.Column(2).AdjustToContents();
                    ws.Column(5).AdjustToContents();
                    for (int i = 1; i <= total_titulos; i++){
                        ws.Cell(11, i).Style.Font.Bold = true;
                        ws.Cell(11, i).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(11, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(11, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.RightBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.TopBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    ws.Rows("6").Height = 30;
                    /***********************************************************************************/
                    List<int> tallas_id_temporal = new List<int>();
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador_tallas == 0)
                        {
                            foreach (estilos e in tarimas.lista_estilos)
                            {
                                if (contador_tallas == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (var ratio in e.lista_ratio)
                                    {
                                        contador_tallas++;
                                    }
                                }
                            }
                        }
                    }
                    int temporal = 0;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (estilos e in tarimas.lista_estilos)
                        {
                            if (temporal == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                            {
                                foreach (var ratio in e.lista_ratio)
                                {
                                    tallas_id_temporal.Add(ratio.id_talla);
                                }
                                temporal++;
                            }
                        }
                    }
                    //tallas_id_temporal.Add(ratio.id_talla);
                    int[] tallas_comparacion = tallas_id_temporal.ToArray();//ARREGLO DE TALLAS PARA COMPARAR
                    foreach (Tarima tarimas in item.lista_tarimas) { pallets++; } //TOTAL DE TARIMAS
                    int[] sumas_tallas = new int[contador_tallas + 2];
                    for (int i = 0; i < contador_tallas + 2; i++) { sumas_tallas[i] = 0; }//totales de tallas +total piezas +total cajas
                    r = 12;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        estilos_total = 0; rows = 0; tarima_contador = 0; rows = 0;
                        foreach (estilos estilo in tarimas.lista_estilos) { estilos_total++; }
                        ws.Cell(r, 1).Value = tarimas.id_tarima;
                        ws.Range(r, 1, (r + estilos_total - 1), 1).Merge();
                        var celdas_estilos = new List<String[]>();
                        foreach (estilos estilo in tarimas.lista_estilos)
                        {
                            List<String> datos = new List<string>();
                            datos.Add(Convert.ToString(estilo.number_po));
                            if (estilo.tipo != "NONE") { datos.Add(estilo.tipo); }
                            else { datos.Add(" "); }
                            datos.Add(estilo.estilo);
                            datos.Add(estilo.color);
                            datos.Add(estilo.descripcion);
                            if (tiendas != 0) { datos.Add(estilo.store); }
                            total_ratio = 0;
                            foreach (ratio_tallas ratio in estilo.lista_ratio) { total_ratio++; }//esperar con esto a ver que 
                            contador = 0;
                            int ii = 0, total_talla, total_estilo = 0;
                            if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                foreach (int tall in tallas_comparacion)
                                {
                                    foreach (ratio_tallas ratio in estilo.lista_ratio)
                                    {
                                        if (tall == ratio.id_talla)
                                        {
                                            total_talla = ratio.ratio * estilo.boxes;
                                            sumas_tallas[ii] += total_talla;
                                            total_estilo += total_talla;
                                            datos.Add(Convert.ToString(total_talla));
                                        }
                                        else { datos.Add(" "); }
                                        ii++;
                                    }
                                }
                            }
                            else
                            {
                                foreach (int tall in tallas_comparacion)
                                {
                                    if (tall == estilo.id_talla)
                                    {
                                        total_talla = estilo.boxes;
                                        piezas_estilo = estilo.boxes;
                                        sumas_tallas[ii] += total_talla;
                                        total_estilo += total_talla;
                                        datos.Add(Convert.ToString(total_talla));
                                    }
                                    else { datos.Add(""); }
                                    ii++;
                                }
                            }
                            datos.Add(Convert.ToString(total_estilo));
                            sumas_tallas[ii] += total_estilo; ii++;
                            if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                            {
                                datos.Add("1");
                                sumas_tallas[ii] += 1;
                            }
                            else
                            {
                                int piezas = ds.buscar_cajas_talla_estilo(estilo.id_po_summary, estilo.id_talla);
                                datos.Add(Convert.ToString((piezas_estilo / piezas)));
                                sumas_tallas[ii] += Convert.ToInt32((piezas_estilo / piezas));
                            }
                            celdas_estilos.Add(datos.ToArray());
                        }//ESTILOS                            
                        ws.Cell(r, 2).Value = celdas_estilos;
                        c = 7;           //COLUMNA DONDE TERMINA LOS DATOS BÁSICOS, LUEGO VIENEN LOS DINÁMICOS Y POR ÚLTIMO LA DE PALLETS             
                        if (tiendas != 0) { c++; }
                        c = c + (contador_tallas + 2);
                        ws.Cell(r, c).Value = "1";
                        ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                        r += estilos_total;
                        columnas = c;
                    }//TARIMAS
                    contador = 0;
                    string descripcion_final = "";
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        if (contador == 0)
                        {
                            foreach (estilos estilo in tarimas.lista_estilos)
                            {
                                descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                            }
                            contador++;
                        }
                    }

                    ws.Cell(r, 1).Value = descripcion_final;
                    ws.Cell(r, 1).Style.Font.Bold = true;
                    ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(r, 1, r, 4).Merge();
                    ws.Range(r, 1, r, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


                    ws.Cell(r, 5).Value = "TOTAL";
                    ws.Cell(r, 5).Style.Font.Bold = true;
                    ws.Cell(r, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c = 6;
                    if (tiendas != 0) { c++; }
                    ws.Range(r, 5, r, c).Merge();
                    ws.Range(r, 5, r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    c++;

                    for (int i = 0; i < contador_tallas + 2; i++)
                    {
                        ws.Cell(r, c).Value = sumas_tallas[i];
                        ws.Cell(r, c).Style.Font.Bold = true;
                        ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        c++;
                    }
                    ws.Cell(r, c).Value = pallets;
                    ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    ws.Cell(r, c).Style.Font.Bold = true;
                    ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    filas = r;

                    for (int i = 11; i <= (filas); i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {

                            ws.Cell(i, j).Style.Font.FontSize = 9;
                            ws.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(i, j).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.LeftBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.RightBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.TopBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.BottomBorderColor = XLColor.Black;
                        }
                    }
                    //tabla------------------------------------------------------------------------------------------------BPK

                    filas += 2;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Font.Bold = true;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Font.Bold = true;
                    ws.Range(filas, 2, (filas + 2), 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(filas, 8, (filas + 2), 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filas, 1).Value = "DRIVER NAME:";
                    ws.Cell(filas, 2).Value = item.conductor.driver_name;
                    ws.Cell(filas, 7).Value = "SHIPPING MANAGER:";
                    ws.Cell(filas, 8).Value = item.shipping_manager;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "TRAILER/PLATES:";
                    ws.Cell(filas, 2).Value = item.conductor.tractor + "/" + item.conductor.plates;
                    ws.Cell(filas, 7).Value = "SEAL:";
                    ws.Cell(filas, 8).Value = item.seal;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "CONTAINER/PLATES:";
                    ws.Cell(filas, 2).Value = item.contenedor.eco + "/" + item.contenedor.plates;
                    ws.Cell(filas, 7).Value = "REPLACEMENT:";
                    ws.Cell(filas, 8).Value = item.replacement;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 3).Value = "DOCUMENTO CONTROLADO. ÚNICAMENTE SE PUEDE MODIFICAR POR EL SUPERVISOR DE SHIPPING";
                    ws.Cell(filas, 3).Style.Font.FontColor = XLColor.FromArgb(100, 100, 100);
                    filas += 2;
                    columnas = 2;

                    var porcentajes = new List<String[]>();
                    List<String> p = new List<string>();
                    p.Add("ORIGIN"); p.Add("QTY");
                    porcentajes.Add(p.ToArray());
                    List<Fabricantes> totales_paises_estilo = new List<Fabricantes>();
                    List<Fabricantes> totales_paises = new List<Fabricantes>();
                    int add = 0, total_paises = 0, iguales;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        totales_paises_estilo = ds.buscar_paises_estilos(tarimas.lista_estilos);
                        foreach (Fabricantes fa in totales_paises_estilo)
                        {
                            iguales = 0;
                            if (add == 0)
                            {
                                Fabricantes nf = new Fabricantes();
                                nf.cantidad = fa.cantidad;
                                nf.pais = fa.pais;
                                totales_paises.Add(nf);
                                add++;
                            }
                            else
                            {
                                foreach (Fabricantes f in totales_paises.ToList())
                                {
                                    if (f.pais == fa.pais)
                                    {
                                        f.cantidad = fa.cantidad;
                                        iguales++;
                                    }
                                }
                                if (iguales == 0)
                                {
                                    Fabricantes nf = new Fabricantes();
                                    nf.cantidad = fa.cantidad;
                                    nf.pais = fa.pais;
                                    totales_paises.Add(nf);
                                }
                                add++;
                            }

                        }
                    }
                    foreach (Fabricantes f in totales_paises) { total_paises += f.cantidad; }
                    foreach (Fabricantes f in totales_paises) { f.porcentaje = (f.cantidad * 100) / total_paises; }
                    List<Fabricantes> totales_paises_envio = new List<Fabricantes>();
                    foreach (Fabricantes f in totales_paises)
                    {
                        Fabricantes nf = new Fabricantes();
                        double x = (((sumas_tallas[sumas_tallas.Length - 2]) * f.cantidad) / total_paises);
                        nf.cantidad = Convert.ToInt32(Math.Round(x));
                        nf.pais = f.pais;
                        totales_paises_envio.Add(nf);
                    }
                    foreach (Fabricantes f in totales_paises_envio)
                    {
                        porcentajes.Add(new string[] { f.pais, (f.cantidad).ToString() });
                    }
                    ws.Cell(filas, 2).Value = "%";
                    ws.Cell(filas, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    filas++;
                    ws.Cell(filas, 2).Value = porcentajes;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Packing List " + clave_packing + ".xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();

            }
        }

        public void excel_pk_assort(List<Pk> lista)
        {
            
            string clave_packing = "";
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            {
                int estilos_total = 0, rows = 0, tarima_contador = 0, total_ratio, contador, r, c, total_cajas = 0, filas = 0, columnas = 0, tallas_id, piezas_estilo = 0;
                string tipo = "";
                var ws = libro_trabajo.Worksheets.Add("PK");
                foreach (Pk item in lista)
                {
                    clave_packing = item.packing;
                    /*****INICIO CON DIRECCIONES, LOGO, ETC******/
                    ws.Cell("A2").Value = "FORTUNE FASHIONS BAJA, S.R.L. DE C.V.";
                    ws.Cell("A3").Value = "CALLE TORTUGA No 313-A";
                    ws.Cell("A4").Value = "MANEADERO CP 22790";
                    ws.Cell("A5").Value = "BAJA CALIFORNIA";
                    ws.Style.Font.FontSize = 11;

                    ws.Range("A2:A10").Style.Font.Bold = true;
                    ws.Range("A7:A10").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    //DIRECCIÓN DE ORIGEN
                    ws.Cell("A7").Value = "CUSTOMER: ";
                    ws.Cell("B7").Value = item.customer;

                    int ex_label = ds.contar_labels(item.id_packing_list);
                    if (ex_label != 0)
                    {
                        List<Labels> lista_etiquetas = new List<Labels>();
                        lista_etiquetas = ds.obtener_etiquetas(item.id_packing_list);
                        ws.Cell("A8").Value = "P.O.: ";
                        string label = Regex.Replace(item.pedido, @"\s+", " ") + "(PO# ";
                        foreach (Labels l in lista_etiquetas) { label += " " + l.label; }
                        if (ex_label == 1)
                        {
                            label += " )" + " With UCC Labels " + item.parte;
                        }
                        else
                        {
                            label += " )" + " With TPM Labels " + item.parte;
                        }
                        ws.Cell("B8").Value = label;
                    }
                    else
                    {
                        ws.Cell("A8").Value = "P.O.: ";
                        ws.Cell("B8").Value = Regex.Replace(item.pedido, @"\s+", " ") + " Without UCC Labels " + item.parte;
                    }
                    ws.Cell("A9").Value = "RETAILER: ";
                    ws.Cell("B9").Value = item.customer_po;
                    if (item.tipo != "1")
                    {
                        ws.Cell("A10").Value = "EXAMPLES ";
                        ws.Cell("A10").Style.Font.FontSize = 14;
                    }

                    //IMAGEN AL CENTRO
                    ws.Range(1, 7, 1, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    var imagePath = Server.MapPath("/") + "/Content/img/LOGO FORTUNE.png";
                    //var imagePath = @"C:\Users\melissa\source\repos\karen\FortuneSystem\Content\img\LOGO FORTUNE.png";
                    var image = ws.AddPicture(imagePath).MoveTo(ws.Cell("E1")).Scale(0.30);
                    //PK ABAJO DE LA IMAGEN
                    ws.Cell("D7").Value = "PK: ";
                    ws.Cell("D7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D7").Style.Font.Bold = true;
                    ws.Cell("E7").Value = item.packing;
                    ws.Cell("E7").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range("E7:F7").Merge();
                    ws.Range("E7", "F7").Style.Font.Bold = true;
                    ws.Range("D7", "F7").Style.Font.FontSize = 15;
                    ws.Range("E7:F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:F7").Style.Border.BottomBorderColor = XLColor.Black;
                    //DIRECCION DE DESTINO
                    ws.Cell("L2").Value = "SHIP TO: ";
                    ws.Cell("L3").Value = item.destino.nombre;
                    ws.Cell("L4").Value = item.destino.direccion;
                    ws.Cell("L5").Value = item.destino.ciudad + " " + item.destino.zip;
                    ws.Cell("L8").Value = "DATE:" + item.fecha;
                    ws.Range("L2", "L10").Style.Font.Bold = true;
                    var columna_a = ws.Range("A2", "A10");
                    ws.Rows("6").Height = 30;
                    /****************T*A*B*L*A************************************************/
                    int contador_cabeceras = 0, contador_tallas = 0, pallets = 0, tiendas = 0;
                    //PPK
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();//=  "ID", "P.O. NUM", "TYPE", "COLOR", "DESCRIPTION";
                    titulos.Add("ID"); titulos.Add("P.O. NUM"); titulos.Add("TYPE"); titulos.Add("ASSORTMENT"); titulos.Add("STYLE"); titulos.Add("COLOR"); titulos.Add("DESCRIPTION");
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (Assortment a in t.lista_assortments)
                        {
                            foreach (estilos e in a.lista_estilos)
                            {
                                if (e.store != "N/A" && e.store != "NA") { tiendas++; }
                            }
                        }

                    }
                    if (tiendas != 0) { titulos.Add("STORE"); }
                    foreach (Tarima t in item.lista_tarimas)
                    {
                        foreach (Assortment a in t.lista_assortments)
                        {
                            foreach (estilos e in a.lista_estilos)
                            {
                                if (contador_cabeceras == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (ratio_tallas ra in e.lista_ratio) { titulos.Add(ra.talla); }
                                    contador_cabeceras++;
                                }
                            }
                        }
                    }
                    titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");
                    headers.Add(titulos.ToArray());
                    int total_titulos = (titulos.ToArray()).Length;
                    ws.Cell(11, 1).Value = headers;
                    for (int i = 1; i <= total_titulos; i++)
                    {
                        ws.Cell(11, i).Style.Font.Bold = true;
                        ws.Cell(11, i).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(11, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(11, i).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.LeftBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.RightBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.TopBorderColor = XLColor.Black;
                        ws.Cell(11, i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(11, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    ws.Rows("6").Height = 30;
                    /***********************************************************************************/
                    List<int> tallas_id_temporal = new List<int>();
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            if (contador_tallas == 0)
                            {
                                foreach (estilos e in a.lista_estilos)
                                {
                                    if (contador_tallas == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                    {
                                        foreach (var ratio in e.lista_ratio) { contador_tallas++; }
                                    }
                                }
                            }
                        }
                    }
                    int temporal = 0;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            foreach (estilos e in a.lista_estilos)
                            {
                                if (temporal == 0 && e.tipo != "EXT" && e.tipo != "DMG" && e.tipo != "RPLN" && e.tipo != "ECOM")
                                {
                                    foreach (var ratio in e.lista_ratio) { tallas_id_temporal.Add(ratio.id_talla); }
                                    temporal++;
                                }
                            }
                        }
                    }
                    //tallas_id_temporal.Add(ratio.id_talla);
                    int[] tallas_comparacion = tallas_id_temporal.ToArray();//ARREGLO DE TALLAS PARA COMPARAR
                    foreach (Tarima tarimas in item.lista_tarimas) { pallets++; } //TOTAL DE TARIMAS
                    int[] sumas_tallas = new int[contador_tallas + 2];
                    for (int i = 0; i < contador_tallas + 2; i++) { sumas_tallas[i] = 0; }//totales de tallas +total piezas +total cajas
                    r = 12;
                    int assort = 0;
                    foreach (Tarima tarimas in item.lista_tarimas){
                        int extras = 0;
                        foreach (Assortment a in tarimas.lista_assortments){
                            estilos_total = 0; rows = 0; tarima_contador = 0; rows = 0;
                            foreach (estilos estilo in a.lista_estilos) { estilos_total++; }
                            ws.Cell(r, 1).Value = tarimas.id_tarima;
                            ws.Range(r, 1, (r + estilos_total - 1), 1).Merge();
                            ws.Range(r, 4, (r + estilos_total - 1), 4).Merge();

                            var celdas_estilos = new List<String[]>();
                            int ii = 0;
                            foreach (estilos estilo in a.lista_estilos){
                                List<String> datos = new List<string>();
                                datos.Add(Convert.ToString(estilo.number_po));
                                if (estilo.tipo != "NONE") { datos.Add(estilo.tipo); }
                                else { datos.Add(" "); }
                                datos.Add(a.nombre);//NOMBRE DEL ASSORT
                                datos.Add(estilo.estilo);
                                datos.Add(estilo.color);
                                datos.Add(estilo.descripcion);
                                if (tiendas != 0) { datos.Add(estilo.store); }
                                total_ratio = 0;
                                foreach (ratio_tallas ratio in estilo.lista_ratio) { total_ratio++; }//esperar con esto a ver que 
                                contador = 0;
                                int total_talla, total_estilo = 0; ii = 0;
                                if (estilo.tipo == "DMG" || estilo.tipo == "EXT" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM"){
                                    foreach (int tall in tallas_comparacion){
                                        assort = 0;
                                        foreach (ratio_tallas ratio in estilo.lista_ratio){
                                            if (tall == ratio.id_talla){
                                                total_talla = ratio.ratio;// * estilo.boxes;
                                                sumas_tallas[ii] += total_talla;
                                                total_estilo += total_talla;
                                                assort = ratio.ratio;
                                                //datos.Add(Convert.ToString(total_talla));
                                            }//else { datos.Add(" "); }
                                            //ii++;
                                        }
                                        if (assort != 0) { datos.Add(Convert.ToString(assort)); }
                                        else { datos.Add(" "); }
                                        ii++;
                                    }
                                }else{
                                    foreach (int tall in tallas_comparacion){
                                        assort = 0;
                                        foreach (ratio_tallas ratio in estilo.lista_ratio){
                                            if (tall == ratio.id_talla){
                                                total_talla = ratio.ratio * estilo.boxes;
                                                sumas_tallas[ii] += total_talla;
                                                total_estilo += total_talla;
                                                assort = total_talla;
                                            }
                                        }
                                        if (assort != 0) { datos.Add(Convert.ToString(assort)); }
                                        else { datos.Add(" "); }
                                        ii++;
                                    }
                                }
                                datos.Add(Convert.ToString(total_estilo));
                                sumas_tallas[ii] += total_estilo; ii++;
                                tipo = estilo.tipo;
                                if (estilo.tipo == "EXT" || estilo.tipo == "DMG" || estilo.tipo == "RPLN" || estilo.tipo == "ECOM")
                                {
                                    extras++;
                                }
                                else
                                {
                                    extras = 0;
                                }
                                celdas_estilos.Add(datos.ToArray());
                            }//ESTILOS  
                            if (extras != 0)
                            {
                                sumas_tallas[ii] += 1;
                            }
                            else
                            {
                                sumas_tallas[ii] += a.cartones;
                            }
                            ws.Cell(r, 2).Value = celdas_estilos;
                            c = 8;           //COLUMNA DONDE TERMINA LOS DATOS BÁSICOS, LUEGO VIENEN LOS DINÁMICOS Y POR ÚLTIMO LA DE PALLETS  
                            if (tiendas != 0) { c++; }
                            c = c + (contador_tallas + 1);

                            ws.Cell(r, c).Value = a.cartones;
                            ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                            c++;

                            ws.Cell(r, c).Value = "1";
                            ws.Range(r, c, (r + estilos_total - 1), c).Merge();
                            r += estilos_total;
                            columnas = c;
                        }



                    }//TARIMAS
                    contador = 0;
                    string descripcion_final = "";
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            if (contador == 0)
                            {
                                foreach (estilos estilo in a.lista_estilos)
                                {
                                    descripcion_final += Regex.Replace(estilo.descripcion_final, @"\s+", " ") + " ";
                                }
                                contador++;
                            }
                        }

                    }

                    ws.Cell(r, 1).Value = descripcion_final;
                    ws.Cell(r, 1).Style.Font.Bold = true;
                    ws.Cell(r, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(r, 1, r, 4).Merge();
                    ws.Range(r, 1, r, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);


                    ws.Cell(r, 5).Value = "TOTAL";
                    ws.Cell(r, 5).Style.Font.Bold = true;
                    ws.Cell(r, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    c = 7;
                    if (tiendas != 0) { c++; }
                    ws.Range(r, 5, r, c).Merge();
                    ws.Range(r, 5, r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    c++;

                    for (int i = 0; i < contador_tallas + 2; i++)
                    {
                        ws.Cell(r, c).Value = sumas_tallas[i];
                        ws.Cell(r, c).Style.Font.Bold = true;
                        ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                        ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        c++;
                    }
                    ws.Cell(r, c).Value = pallets;
                    ws.Cell(r, c).Style.Fill.BackgroundColor = XLColor.FromArgb(217, 217, 217);
                    ws.Cell(r, c).Style.Font.Bold = true;
                    ws.Cell(r, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    filas = r;
                    //ws.Column(1).Width = 30;
                    for (int i = 11; i <= (filas); i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            //if (j == 4 || j == 6) { ws.Column(j).Width = 30; }
                            ws.Cell(i, j).Style.Font.FontSize = 9;
                            ws.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            ws.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(i, j).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.LeftBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.RightBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.TopBorderColor = XLColor.Black;
                            ws.Cell(i, j).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell(i, j).Style.Border.BottomBorderColor = XLColor.Black;
                        }
                    }
                    //tabla------------------------------------------------------------------------------------------------BPK

                    filas += 2;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 1, (filas + 2), 1).Style.Font.Bold = true;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Range(filas, 7, (filas + 2), 7).Style.Font.Bold = true;
                    ws.Range(filas, 2, (filas + 2), 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Range(filas, 8, (filas + 2), 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    ws.Cell(filas, 1).Value = "DRIVER NAME:";
                    ws.Cell(filas, 2).Value = item.conductor.driver_name;
                    ws.Cell(filas, 7).Value = "SHIPPING MANAGER:";
                    ws.Cell(filas, 8).Value = item.shipping_manager;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "TRAILER/PLATES:";
                    ws.Cell(filas, 2).Value = item.conductor.tractor + "/" + item.conductor.plates;
                    ws.Cell(filas, 7).Value = "SEAL:";
                    ws.Cell(filas, 8).Value = item.seal;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 1).Value = "CONTAINER/PLATES:";
                    ws.Cell(filas, 2).Value = item.contenedor.eco + "/" + item.contenedor.plates;
                    ws.Cell(filas, 7).Value = "REPLACEMENT:";
                    ws.Cell(filas, 8).Value = item.replacement;
                    ws.Range(filas, 2, filas, 3).Merge();
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 2, filas, 3).Style.Border.BottomBorderColor = XLColor.Black;
                    ws.Range(filas, 8, filas, 11).Merge();
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(filas, 8, filas, 11).Style.Border.BottomBorderColor = XLColor.Black;
                    filas++;
                    ws.Cell(filas, 3).Value = "DOCUMENTO CONTROLADO. ÚNICAMENTE SE PUEDE MODIFICAR POR EL SUPERVISOR DE SHIPPING";
                    ws.Cell(filas, 3).Style.Font.FontColor = XLColor.FromArgb(100, 100, 100);
                    filas += 2;
                    columnas = 2;

                    var porcentajes = new List<String[]>();
                    List<String> p = new List<string>();
                    p.Add("ORIGIN"); p.Add("QTY");
                    porcentajes.Add(p.ToArray());
                    List<Fabricantes> totales_paises_estilo = new List<Fabricantes>();
                    List<Fabricantes> totales_paises = new List<Fabricantes>();
                    int add = 0, total_paises = 0, iguales;
                    foreach (Tarima tarimas in item.lista_tarimas)
                    {
                        foreach (Assortment a in tarimas.lista_assortments)
                        {
                            totales_paises_estilo = ds.buscar_paises_estilos(a.lista_estilos);
                            foreach (Fabricantes fa in totales_paises_estilo)
                            {
                                iguales = 0;
                                if (add == 0)
                                {
                                    Fabricantes nf = new Fabricantes();
                                    nf.cantidad = fa.cantidad;
                                    nf.pais = fa.pais;
                                    totales_paises.Add(nf);
                                    add++;
                                }
                                else
                                {
                                    foreach (Fabricantes f in totales_paises.ToList())
                                    {
                                        if (f.pais == fa.pais)
                                        {
                                            f.cantidad = fa.cantidad;
                                            iguales++;
                                        }
                                    }
                                    if (iguales == 0)
                                    {
                                        Fabricantes nf = new Fabricantes();
                                        nf.cantidad = fa.cantidad;
                                        nf.pais = fa.pais;
                                        totales_paises.Add(nf);
                                    }
                                    add++;
                                }

                            }
                        }

                    }
                    foreach (Fabricantes f in totales_paises) { total_paises += f.cantidad; }
                    foreach (Fabricantes f in totales_paises) { f.porcentaje = (f.cantidad * 100) / total_paises; }
                    List<Fabricantes> totales_paises_envio = new List<Fabricantes>();
                    foreach (Fabricantes f in totales_paises)
                    {
                        Fabricantes nf = new Fabricantes();
                        double x = (((sumas_tallas[sumas_tallas.Length - 2]) * f.cantidad) / total_paises);
                        nf.cantidad = Convert.ToInt32(Math.Round(x));
                        nf.pais = f.pais;
                        totales_paises_envio.Add(nf);
                    }
                    foreach (Fabricantes f in totales_paises_envio)
                    {
                        porcentajes.Add(new string[] { f.pais, (f.cantidad).ToString() });
                    }
                    ws.Cell(filas, 2).Value = "%";
                    ws.Cell(filas, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    filas++;
                    ws.Cell(filas, 2).Value = porcentajes;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Packing List " + clave_packing + ".xlsx\"");

                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                httpResponse.End();

            }
        }

        public JsonResult buscar_informacion_edicion_pk(string pk){
            return Json(ds.obtener_informacion_editar_pk(Convert.ToInt32(pk)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult guardar_informacion_edicion_pk(string id, string sello, string replacement, string conductor, string contenedor,string direccion)
        {
            ds.actualizar_datos_pk(id, sello, replacement, conductor, contenedor,direccion);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //****************************************************************************************************************************************************************
        //*********REPORTES***********************************************************************************************************************************************
        //****************************************************************************************************************************************************************
        //string fecha_inicio, fecha_final;
        public JsonResult fechas_reporte(string inicio, string final){
            Session["fechas"] = inicio + "*" + final;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_reporte_po_abierto(){
            string[] fechas = (Convert.ToString(Session["fechas"])).Split('*');
            
            List<Pk> lista = ds.obtener_pedido_cantidades(fechas[0], fechas[1]);
            int row = 1, column = 2, suma_totales_talla = 0, total_talla, suma_estilo, existe_talla, total_cabeceras = 4;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){
                var ws = libro_trabajo.Worksheets.Add("Open orders");
                ws.Cell(row, column).Value = "OPEN WIP";
                ws.Cell(row, column).Style.Font.FontSize = 22;
                ws.Cell(row, column).Style.Font.Bold= true;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                row++;

                foreach (Pk p in lista){
                    total_cabeceras = 4;
                    List<string> tallas_letras = new List<string>();
                    List<Int32> talla_tempo = new List<Int32>();
                    int[] tallas_ids;
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();
                    titulos.Add("CANCEL DATE"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR");
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            bool isEmpty = !tallas_letras.Any();
                            if (isEmpty){
                                talla_tempo.Add(r.id_talla);
                                tallas_letras.Add(r.talla);
                            }else{
                                existe_talla = 0;
                                foreach (string s in tallas_letras){
                                    if (s == r.talla){
                                        existe_talla++;
                                    }
                                }
                                if (existe_talla == 0){
                                    talla_tempo.Add(r.id_talla);
                                    tallas_letras.Add(r.talla);
                                }
                            }
                        }
                    }
                    foreach (string s in tallas_letras){
                        titulos.Add(s);
                        total_cabeceras++;
                    }
                    tallas_ids = talla_tempo.ToArray();
                    titulos.Add("(+/-)"); total_cabeceras++;
                    headers.Add(titulos.ToArray());
                   
                    int tempo = 0; suma_totales_talla = 0;
                    foreach (estilos e in p.lista_estilos){
                        tempo++;
                        foreach (ratio_tallas r in e.lista_ratio){
                            suma_totales_talla += r.total_talla;
                        }
                    }
                    if (tempo != 0) {
                        if (suma_totales_talla >= 5) {
                            row++;
                            ws.Cell(row, 2).Value = p.pedido;
                            ws.Cell(row, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                            ws.Cell(row, 2).Style.Font.Bold = true;
                            ws.Cell(row, 2).Style.Font.FontSize = 14;
                            row++;
                            ws.Cell(row, 1).Value = headers;
                            ws.Range(row, 1, row, total_cabeceras).Style.Font.Bold = true;
                            ws.Range(row, 1, row, total_cabeceras).Style.Font.FontSize = 14;
                            ws.Range(row, 1, row, total_cabeceras).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                            row++;
                        }                        
                    }
                    suma_totales_talla = 0;
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            suma_totales_talla += r.total_talla;
                        }
                        if (suma_totales_talla >= 5) {                            
                            var celdas_estilos = new List<String[]>();
                            List<String> datos = new List<string>();
                            datos.Add(p.cancel_date);
                            datos.Add(e.estilo);
                            datos.Add(e.descripcion);
                            datos.Add(e.color);
                            suma_estilo = 0;
                            foreach (int i in tallas_ids){
                                total_talla = 0;
                                foreach (ratio_tallas r in e.lista_ratio){
                                    if (r.id_talla == i){
                                        total_talla = r.total_talla;
                                    }
                                }
                                suma_estilo += total_talla;
                                if (total_talla == 0){
                                    datos.Add("0");
                                }else{
                                    datos.Add("-" + (total_talla).ToString());
                                }
                            }
                            datos.Add("-" + (suma_estilo).ToString());
                            celdas_estilos.Add(datos.ToArray());
                            ws.Cell(row, 1).Value = celdas_estilos;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                            row++;
                        }
                       
                    }
                    
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Open orders " + fechas[0] + "-" + fechas[1] + ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        public JsonResult po_reporte(string po){
            Session["po_reporte"] = po ;
            return Json("", JsonRequestBehavior.AllowGet);
        }      
        public void excel_reporte_status(){
            string pedido = Convert.ToString(Session["po_reporte"]);
            int id_pedido = consultas.buscar_pedido(pedido);
            List<Estilo_Pedido> lista_estilos = ds.obtener_estilos_pedido_status(id_pedido);
            int row = 1, aux,cabeceras;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");

                ws.Cell(row, 3).Value = Regex.Replace("ORDER "+pedido, @"\s+", " "); 
                ws.Cell(row, 3).Style.Font.FontSize = 22;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                row+=2;
                foreach (Estilo_Pedido ep in lista_estilos){
                    cabeceras = 7;
                    List<Int32> id_tallas_tempo = new List<Int32>();
                    List<Int32> cantidades_tallas_tempo = new List<Int32>();
                    List<string> tallas_tempo = new List<string>();
                    foreach (Talla t in ep.totales_pedido){
                        bool isEmpty = !id_tallas_tempo.Any();
                        if (isEmpty){
                            id_tallas_tempo.Add(t.id_talla);
                            tallas_tempo.Add(t.talla);
                            cantidades_tallas_tempo.Add(t.total);
                            cabeceras++;
                        }else{
                            aux = 0;
                            foreach (int i in id_tallas_tempo){
                                if (i == t.id_talla) { aux++; }
                            }
                            if (aux == 0){
                                id_tallas_tempo.Add(t.id_talla);
                                tallas_tempo.Add(t.talla);
                                cantidades_tallas_tempo.Add(t.total);
                                cabeceras++;
                            }
                        }
                    }//OBTENER TALLAS DE EL ESTILO
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();
                    titulos.Add("ID"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR"); titulos.Add("TYPE");
                    int titulo_c = 0;
                    foreach (string s in tallas_tempo) { titulo_c++; titulos.Add(s); }
                    titulos.Add("PACKING"); titulos.Add("DATE");
                    if (titulo_c != 0) { 
                        headers.Add(titulos.ToArray());
                        ws.Cell(row, 1).Value = headers;
                        ws.Range(row,1,row, cabeceras).Style.Fill.BackgroundColor = XLColor.FromArgb(230, 184, 183);
                        ws.Range(row, 1, row, cabeceras).Style.Font.FontSize = 13;
                        ws.Range(row, 1, row, cabeceras).Style.Font.Bold = true;
                        row++; //AGHREGAR CABECERA TABLA
                        //AGREGAR ROWS DE INFORMACIÓN POR PK DE CADA ESTILO
                        foreach (Packing_Estilo pe in ep.lista_pk){
                            var celdas = new List<String[]>();
                            List<String> datos = new List<string>();
                            datos.Add((ep.id_estilo).ToString());
                            datos.Add(ep.estilo);
                            datos.Add(ep.descripcion);
                            datos.Add(ep.color);
                            datos.Add(pe.tipo);
                            int j = 0;
                            foreach (int i in id_tallas_tempo){
                                aux = 0;
                                foreach (Talla te in pe.lista_enviados){
                                    if (te.id_talla == i){
                                        aux = te.total;
                                    }
                                }
                                if (aux == 0){
                                    datos.Add(" ");
                                }else{
                                    datos.Add((aux).ToString());
                                    cantidades_tallas_tempo[j] = cantidades_tallas_tempo[j] - aux;
                                }
                                j++;
                            }
                            datos.Add(pe.package);
                            datos.Add(pe.fecha);
                            celdas.Add(datos.ToArray());
                            ws.Cell(row, 1).Value = celdas;
                            ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                            ws.Range(row, 1, row, cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.RightBorderColor = XLColor.White;
                            ws.Range(row, 1, row, cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.TopBorderColor = XLColor.White;
                            ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                            row++;                            
                        }

                        var celdas2 = new List<String[]>();
                        List<String> datos2 = new List<string>();
                        datos2.Add("+/-");
                        foreach (int i in cantidades_tallas_tempo){
                            if (i < 0){
                                datos2.Add("+" + (i * -1).ToString());
                            }else{
                                datos2.Add("-" + i.ToString());
                            }
                        }
                        celdas2.Add(datos2.ToArray());
                        ws.Cell(row, 5).Value = celdas2;
                        ws.Range(row, 1, row, cabeceras).Style.Font.Bold = true;
                        ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                        ws.Range(row, 1, row, cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.RightBorderColor = XLColor.White;
                        ws.Range(row, 1, row, cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.TopBorderColor = XLColor.White;
                        ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                        ws.Range(row, 1, row, cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                        row++; row++;
                    }
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();                
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
    /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Order status " +pedido+ ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        public JsonResult estilo_reporte(string estilo){
            Session["estilo_reporte"] = estilo;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_reporte_por_estilos(){
            string estilo = Convert.ToString(Session["estilo_reporte"]);
            int id_estilo = consultas.obtener_estilo_id(estilo);
            List<Estilo_PO> lista_estilos = ds.obtener_pedidos_po_estilo(id_estilo);
            int row = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");
                ws.Cell(row, 2).Value = Regex.Replace("STYLE: "+estilo, @"\s+", " ");
                ws.Cell(row, 2).Style.Font.FontSize = 22;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                row++;
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR"); titulos.Add("PCS"); titulos.Add("STATUS");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1,row,6).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                ws.Range(row, 1, row, 6).Style.Font.Bold = true;
                row++; //AGHREGAR CABECERA TABLA
                foreach (Estilo_PO e in lista_estilos) {
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(e.pedido);
                    datos.Add(e.estilo);
                    datos.Add(e.descripcion);
                    datos.Add(e.color);
                    if (e.total < 0){
                        datos.Add((e.total * -1).ToString());
                    }else{
                        datos.Add(e.total.ToString());
                    }
                    //datos.Add((e.total).ToString());
                    datos.Add(e.estado);
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    ws.Range(row, 1, row, 6).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.LeftBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 6).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.RightBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 6).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.TopBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 6).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 6).Style.Border.BottomBorderColor = XLColor.White;
                    row++;
                }              
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Orders with " + estilo + ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();

            }

        }
        public JsonResult year_report(string year){
            Session["year_reporte"] = year;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void listado_year() {
            string year = Convert.ToString(Session["year_reporte"]);
            List<Shipping_pk> lista_pk = ds.obtener_listado_packing_year(year);
            excel_listado_packing(lista_pk, "SHIPMENT REPORTS "+year);
        }
        public void listado_diario(){
            List<Shipping_pk> lista_pk = ds.obtener_listado_packing_diario();
            excel_listado_packing(lista_pk,"SHIPMENT REPORTS "+DateTime.Now.ToString("yyyy-MM-dd"));
        }
        public void excel_listado_packing(List<Shipping_pk> lista_pk,string titulo){
            //string year = Convert.ToString(Session["year_reporte"]);
            //List<Shipping_pk> lista_pk = ds.obtener_listado_packing_year(year);
            int row = 1, column = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");
                ws.Cell(row, column).Value = titulo;
                ws.Row(1).Style.Font.FontSize = 20;
                ws.Range(1, 1, 1, 7).Merge();
                ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(1).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                ws.Row(1).Style.Font.Bold = true;
                row++;
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PK"); titulos.Add("PO"); titulos.Add("SHIP TO"); titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("PALLETS");titulos.Add("# SHIPPING");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Row(row).Style.Font.FontSize = 13;
                ws.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                ws.Row(row).Style.Font.Bold = true;
                row++; //AGHREGAR CABECERA TABLA
                foreach (Shipping_pk e in lista_pk){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(e.packing);
                    datos.Add(e.pedido);
                    datos.Add(e.destino);
                    datos.Add((e.piezas).ToString());
                    datos.Add((e.cajas).ToString());
                    datos.Add((e.pallets).ToString());
                    datos.Add((e.num_envio).ToString());
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 7).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.RightBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 7).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.TopBorderColor = XLColor.White;
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorderColor = XLColor.White;
                    row++;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Shipping Report.xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();

            }

        }
        public void excel_reporte_status_orden(){
            string pedido = Convert.ToString(Session["po_reporte"]);
            int id_pedido = consultas.buscar_pedido(pedido);
            List<Pk> lista = ds.obtener_pedido_cantidades_orden(id_pedido);
            int row = 1, column = 1, suma_totales_talla = 0, total_talla, suma_estilo, existe_talla, total_cabeceras ;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){
                var ws = libro_trabajo.Worksheets.Add("Report");
                List<Int32> talla_tempo = new List<Int32>();
                int[] tallas_ids;
                
                foreach (Pk p in lista){
                    total_cabeceras = 5;
                    List<string> tallas_letras = new List<string>();
                    var headers = new List<String[]>();
                    List<String> titulos = new List<string>();
                    titulos.Add("CANCEL DATE"); titulos.Add("STYLE"); titulos.Add("ITEM DESCRIPTION"); titulos.Add("COLOR");
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            bool isEmpty = !tallas_letras.Any();
                            if (isEmpty){
                                talla_tempo.Add(r.id_talla);
                                tallas_letras.Add(r.talla);
                            }else{
                                existe_talla = 0;
                                foreach (string s in tallas_letras){
                                    if (s == r.talla){
                                        existe_talla++;
                                    }
                                }
                                if (existe_talla == 0){
                                    talla_tempo.Add(r.id_talla);
                                    tallas_letras.Add(r.talla);
                                }
                            }
                        }
                    }
                    foreach (string s in tallas_letras){
                        titulos.Add(s);
                        total_cabeceras++;
                    }
                    tallas_ids = talla_tempo.ToArray();
                    titulos.Add("(+/-)");
                    headers.Add(titulos.ToArray());
                    row++;
                    ws.Cell(row, 2).Value ="MISSING PIECES "+ p.pedido;
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                    ws.Row(row).Style.Font.Bold = true;
                    ws.Row(row).Style.Font.FontSize = 14;
                    row++;
                    ws.Cell(row, 1).Value = headers;
                    ws.Range(row, 1, row, total_cabeceras).Style.Font.Bold = true;
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                    ws.Range(row, 1, row, total_cabeceras).Style.Font.FontSize = 14;
                    row++;
                    foreach (estilos e in p.lista_estilos){
                        foreach (ratio_tallas r in e.lista_ratio){
                            suma_totales_talla += r.total_talla;
                        }
                        //if (suma_totales_talla >= 1){
                            var celdas_estilos = new List<String[]>();
                            List<String> datos = new List<string>();
                            datos.Add(p.cancel_date);
                            datos.Add(e.estilo);
                            datos.Add(e.descripcion);
                            datos.Add(e.color);
                            suma_estilo = 0;
                            foreach (int i in tallas_ids){
                                total_talla = 0;
                                foreach (ratio_tallas r in e.lista_ratio){
                                    if (r.id_talla == i){
                                        total_talla = r.total_talla;
                                    }
                                }
                                suma_estilo += total_talla;
                                if (total_talla == 0){
                                    datos.Add("0");
                                }else{
                                    datos.Add("-" + (total_talla).ToString());
                                }
                            }
                            datos.Add("-" + (suma_estilo).ToString());
                            celdas_estilos.Add(datos.ToArray());
                            ws.Cell(row, 1).Value = celdas_estilos;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.LeftBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.RightBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.TopBorderColor = XLColor.White;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            ws.Range(row, 1, row, total_cabeceras).Style.Border.BottomBorderColor = XLColor.White;
                        //}
                        row++;
                    }                    
                    ws.Cell(row, 2).Value = "TOTAL PO";
                    ws.Cell(row, 3).Value = p.total_po;
                    ws.Cell(row, 4).Value = "SHIPPED";
                    ws.Cell(row, 5).Value = p.total_enviado;
                    ws.Cell(row, 6).Value = "TOTAL";
                    if (p.total_po - p.total_enviado > 0){
                        ws.Cell(row, 7).Value = "-" + (p.total_po - p.total_enviado).ToString();
                    }else {
                        ws.Cell(row, 7).Value = ((p.total_po - p.total_enviado)*-1).ToString();
                    }
                    
                    ws.Range(row,1,row,10).Style.Font.FontSize = 13;
                    ws.Row(row).Style.Font.Bold = true;
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                    row++;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Open orders " + pedido + ".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        
        public void estados_pedidos(List<Shipping_pk> lista_pk, string titulo){
            //string year = Convert.ToString(Session["year_reporte"]);
            List<Po> lista_pedidos = ds.obtener_lista_pedidos();
            int row = 1, column = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            { //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Report");
                
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("CUSTOMER PO"); titulos.Add("STYLE"); titulos.Add("CANCEL DATE"); titulos.Add("TOTAL UNITS"); titulos.Add("STATUS"); titulos.Add("CUSTOMER");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row,1,row,7).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                row++; //AGREGAR CABECERA TABLA
                foreach (Po e in lista_pedidos){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(e.pedido);
                    datos.Add(e.customer_po);
                    datos.Add((e.estilos).ToString());
                    datos.Add(e.fecha_cancelacion);
                    datos.Add((e.total).ToString());
                    datos.Add(e.estado);
                    datos.Add(e.customer);
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    if (e.estado == "CANCELLED") {
                        ws.Row(row).Style.Font.FontColor= XLColor.FromArgb(150, 54, 52);
                    }
                    if (e.estado == "COMPLETED") {
                        ws.Row(row).Style.Font.FontColor= XLColor.FromArgb(57, 71, 29);
                    }
                    if (e.estado == "INCOMPLETE") {
                        ws.Row(row).Style.Font.FontColor= XLColor.FromArgb(54, 96, 146);
                    }
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.LeftBorderColor = XLColor.FromArgb(178, 178, 178); 
                    ws.Range(row, 1, row, 7).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.RightBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Range(row, 1, row, 7).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.TopBorderColor = XLColor.FromArgb(178, 178, 178);
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range(row, 1, row, 7).Style.Border.BottomBorderColor = XLColor.FromArgb(178, 178, 178);
                    row++;
                }
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                //ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Shipping Report Orders.xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        public JsonResult buscar_contenedores()
        {
            return Json(ds.obtener_contenedores(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_modificar_contenedor(string id)
        {
            return Json(ds.obtener_contenedor_edicion(id), JsonRequestBehavior.AllowGet);
        }
        public JsonResult eliminar_contenedor(string id)
        {
            ds.borrar_contenedor(id);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_contenedor(string eco, string plates)
        {
            ds.guardar_nuevo_contenedor(eco, plates);
            return Json(ds.obtener_carriers(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_informacion_contenedor_edicion(string id,string eco, string plates)
        {
            ds.guardar_contenedor_edicion(id,eco, plates);
            return Json(ds.obtener_carriers(), JsonRequestBehavior.AllowGet);
        }
        //************E*D*I*C*I*O*N***************************
        //************E*D*I*C*I*O*N***************************
        public JsonResult edicion_pk_completo(string id){
            Session["pk_edicion"] = id;//
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult edicion_pk() { return View(); }

        public JsonResult buscar_informacion_packing(){
            int pedido = ds.obtener_pedido_packink(Convert.ToInt32(Session["pk_edicion"]));
            List<estilo_shipping> e = ds.lista_estilos(Convert.ToString(pedido));
            var result = Json(new{
                estilos=e,
                packing = ds.obtener_informacion_editar_packing_completo(Convert.ToInt32(Session["pk_edicion"])),                
                assorts = ds.lista_assortments_pedido(pedido),
                tallas = ds.obtener_lista_tallas_pedido(e),
                estilos_packing=ds.lista_estilos_packing_edicion(Convert.ToInt32(Session["pk_edicion"]))
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult guardar_edicion_pk_estilos(string pedido,string packing, string summary, string extension, string piece, string box, string size, string store, string type, string dc_summary, string empaque, string dcs_array, string indice)
        {
            string[] summarys = summary.Split('*'), extensiones = extension.Split('*'), piezas = piece.Split('*'), cajas = box.Split('*'), tallas = size.Split('*');
            string[] stores = store.Split('*'), tipos = type.Split('*'), dc_summarys = dc_summary.Split('*'), empaques = empaque.Split('*'), dc_bp = dcs_array.Split(',');
            string[] indices = indice.Split('*');
            int packing_id = Convert.ToInt32(packing);
            int id_pedido = Convert.ToInt32(pedido);
            int total_enviado = ds.obtener_total_enviado_pedido_exclusivo(id_pedido, packing_id), total_pedido = ds.obtener_total_pedido(id_pedido);
            int number_po = ds.obtener_number_po_pedido(id_pedido);
            int shipping_id=0, total_piezas_pk = 0;
            for (int i = 1; i < summarys.Length; i++){
                int id_estilo = consultas.obtener_estilo_summary(Convert.ToInt32(summarys[i]));
                switch (empaques[i]){
                    case "1"://TIPO DE EMPAQUE BLPACK
                        if (empaques[i] == "1" && indices[i] == "0"){//SIN DC
                            total_piezas_pk += Convert.ToInt32(piezas[i]);
                        }
                        if (empaques[i] == "1" && indices[i] != "0"){//CON DC
                            for (int j = 1; j < dc_bp.Length; j++){
                                string[] dcs_filas = dc_bp[j].Split('*'), cabecera_dc = dc_bp[0].Split('*');
                                int columnas = dcs_filas.Length;
                                if (dcs_filas[columnas - 1] == indices[i]){
                                    for (int k = 2; k < (columnas - 1); k++){
                                        int ratio = ds.buscar_piezas_empaque_bull(Convert.ToInt32(summarys[i]), Convert.ToInt32(cabecera_dc[k]));
                                        if (dcs_filas[k] != "0") { total_piezas_pk += (ratio * Convert.ToInt32(dcs_filas[k])); }
                                    }
                                }
                            }
                        }
                        break;
                    case "2":
                        List<ratio_tallas> ratios = ds.obtener_lista_ratio(Convert.ToInt32(summarys[i]), id_estilo, 2);
                        foreach (ratio_tallas r in ratios){
                            total_piezas_pk += (Convert.ToInt32(cajas[i]) * r.ratio);
                        }
                        break;
                    case "3":
                        Assortment a = ds.assortment_id(Convert.ToInt32(summarys[i]), id_pedido);
                        foreach (estilos e in a.lista_estilos){
                            List<ratio_tallas> ratios_a = ds.obtener_lista_ratio_assort_r(e.id_po_summary, e.id_estilo, a.nombre);
                            foreach (ratio_tallas r in ratios_a){
                                total_piezas_pk += (Convert.ToInt32(cajas[i]) * r.ratio);
                            }
                        }
                        break;
                }
            }
            if ((total_enviado + total_piezas_pk) > (total_pedido + 100)){//ERROR
                return Json("1", JsonRequestBehavior.AllowGet);
            }else{
                //BORRAR TOTALES SHIPPING_ID
                ds.eliminar_estilos_packing_list(packing_id);

                for (int i = 1; i < summarys.Length; i++)
                {
                    int id_estilo = consultas.obtener_estilo_summary(Convert.ToInt32(summarys[i]));
                    switch (empaques[i])
                    {
                        case "1"://TIPO DE EMPAQUE BLPACK
                            if (empaques[i] == "1" && indices[i] == "0")
                            {//SIN DC
                                int id_talla = consultas.buscar_talla(tallas[i]);
                                ds.guardar_estilos_paking(piezas[i], "0", packing.ToString(), id_pedido.ToString(), id_estilo.ToString(), number_po.ToString(), "0", summarys[i], id_talla.ToString(), stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                                shipping_id = ds.obtener_ultimo_shipping_registrado();
                                ds.agregar_cantidades_enviadas(summarys[i], id_talla.ToString(), piezas[i], shipping_id.ToString(), tipos[i], "0");
                            }
                            if (empaques[i] == "1" && indices[i] != "0")
                            {//CON DC
                                for (int j = 1; j < dc_bp.Length; j++)
                                {
                                    string[] dcs_filas = dc_bp[j].Split('*'), cabecera_dc = dc_bp[0].Split('*');
                                    int columnas = dcs_filas.Length;
                                    if (dcs_filas[columnas - 1] == indices[i])
                                    {
                                        for (int k = 2; k < (columnas - 1); k++)
                                        {
                                            int ratio = ds.buscar_piezas_empaque_bull(Convert.ToInt32(summarys[i]), Convert.ToInt32(cabecera_dc[k]));
                                            if (dcs_filas[k] != "0")
                                            {
                                                ds.guardar_estilos_paking((ratio * Convert.ToInt32(dcs_filas[k])).ToString(), "0", packing.ToString(), id_pedido.ToString(), id_estilo.ToString(), number_po.ToString(), dcs_filas[0], summarys[i], cabecera_dc[k], stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                                                shipping_id = ds.obtener_ultimo_shipping_registrado();
                                                ds.agregar_cantidades_enviadas(summarys[i], cabecera_dc[k], (ratio * Convert.ToInt32(dcs_filas[k])).ToString(), shipping_id.ToString(), tipos[i], "0");
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case "2":
                            if (tallas[i] == "") { tallas[i] = "0"; }
                            ds.guardar_estilos_paking(cajas[i], "0", packing.ToString(), id_pedido.ToString(), id_estilo.ToString(), number_po.ToString(), dc_summarys[i], summarys[i], tallas[i], stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                            shipping_id = ds.obtener_ultimo_shipping_registrado();
                            List<ratio_tallas> ratios = ds.obtener_lista_ratio(Convert.ToInt32(summarys[i]), id_estilo, 2);
                            foreach (ratio_tallas r in ratios)
                            {
                                ds.agregar_cantidades_enviadas(summarys[i], (r.id_talla).ToString(), (Convert.ToInt32(cajas[i]) * r.ratio).ToString(), shipping_id.ToString(), tipos[i], "0");
                            }
                            break;
                        case "3":
                            if (tallas[i] == "") { tallas[i] = "0"; }
                            ds.guardar_estilos_paking(cajas[i], "0", packing.ToString(), id_pedido.ToString(), "0", number_po.ToString(), dc_summarys[i], summarys[i], tallas[i], stores[i], tipos[i], extensiones[i], empaques[i], indices[i]);
                            shipping_id = ds.obtener_ultimo_shipping_registrado();
                            Assortment a = ds.assortment_id(Convert.ToInt32(summarys[i]), id_pedido);
                            foreach (estilos e in a.lista_estilos)
                            {
                                List<ratio_tallas> ratios_a = ds.obtener_lista_ratio_assort_r(e.id_po_summary, e.id_estilo, a.nombre);
                                foreach (ratio_tallas r in ratios_a)
                                {
                                    ds.agregar_cantidades_enviadas((e.id_po_summary).ToString(), (r.id_talla).ToString(), (Convert.ToInt32(cajas[i]) * r.ratio).ToString(), shipping_id.ToString(), tipos[i], "1");
                                }
                            }
                            break;
                    }//SWITCH
                }//FOR             
                //verificar_estado_pedido(Convert.ToInt32(Session["pedido"]));
                return Json("0", JsonRequestBehavior.AllowGet);
            }//ELSE
        }
        

        public JsonResult guardar_edicion_pk(string packing,string po, string address, string driver, string container, string seal, string replacement, string manager, string tipo, string labels, string type_labels, string num_envio){
            int usuario = Convert.ToInt32(Session["id_usuario"]);
            
            ds.actualizar_datos_pk(packing, seal, replacement, driver, container,address);
            if (labels != "N/A"){
                string[] label = labels.Split('*');
                for (int i = 1; i < label.Length; i++){
                    ds.guardar_pk_labels(label[i], Convert.ToInt32(packing), type_labels);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        /***************************E*X*T*R*A*S**Y**D*A*Ñ*A*D*A*S***********************************************************************/

        public JsonResult buscar_estilos_pk_otros(string id){
            Session["pedido"] = ds.buscar_pedido_pk(id);
            Session["pk"] = id;
            List<estilo_shipping> e = ds.lista_estilos_extras(Convert.ToString(Session["pedido"]),"0");
            var result = Json(new { po_number = ds.buscar_po_number_pk(id),
                estilos = e,
                tallas = ds.obtener_lista_tallas_pedido(e),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_estilos_pk_extras(string id){           
            List<estilo_shipping> e = ds.lista_estilos_extras(Convert.ToString(Session["pedido"]), id);
            var result = Json(new {           
                estilos_busqueda=e,
                tallas = ds.obtener_lista_tallas_pedido(e),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult agregar_otros_estilos(string rows){
            string[] Row = rows.Split(',');
            int number_po = ds.obtener_number_po_pedido(Convert.ToInt32(Session["pedido"]));
            int packing = Convert.ToInt32(Session["pk"]),pedido= Convert.ToInt32(Session["pedido"]);
            int shipping_id = 0;
            for (int i = 1; i < Row.Length; i++) {
                string[] cabeceras = Row[0].Split('*'), datos = Row[i].Split('*');
                int id_estilo = consultas.obtener_estilo_summary(Convert.ToInt32(datos[0]));
                int total = 0;
                //---   0 SIEMPRE ES EL ID_SUMMARY, EL 1 ES EXT Y EL 2 ES EL TIPO, LOS DEMÁS SON LAS TALLAS                
                for (int j = 3; j < datos.Length; j++) { total += Convert.ToInt32(datos[j]); }
                                            //cantidad, id_tarima,  used,             id_pedido,           id_estilo,             number_po,      dc, id_po_summary, id_talla,store, tipo,     ext, tipo_empaque, index_dc
                ds.guardar_estilos_paking(total.ToString(),"0", packing.ToString(), pedido.ToString(), id_estilo.ToString(), number_po.ToString(), "0", datos[0],"0","N/A",datos[2],datos[1],"1","0");
                shipping_id = ds.obtener_ultimo_shipping_registrado();
                for (int j = 3; j < datos.Length; j++){
                    if (datos[j] != "0"){                     
                        ds.guardar_ratio_otros(shipping_id, datos[j], cabeceras[j]);
                    }
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }



















































    }
}