using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Almacen;
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
using System.Data.SqlClient;


namespace FortuneSystem.Controllers.Catalogos
{
    public class AlmacenController : Controller
    {
        // GET: Almacen
        DatosInventario di = new DatosInventario();
        DatosReportes dr = new DatosReportes();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        QRCodeController qr = new QRCodeController();
        PDFController pdf = new PDFController();
        int id_sucursal;
        public ActionResult Index(){
            //Session["id_usuario"] = consultas.buscar_id_usuario(Convert.ToString(Session["usuario"]));
            //Session["id_usuario"] = 2;
            /*if (Session["idUsuario"] == null){
                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
                //return View(di.ListaInventario());
            }else {
                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
                //return View(di.ListaInventario());
            }*/
            int id_usuario = Convert.ToInt32(Session["id_Empleado"]);
            Session["id_usuario"] = id_usuario;
            Session["id_sucursal"] = consultas.obtener_sucursal_id_usuario(Convert.ToInt32(Session["id_usuario"]));
            return View();
        }
        public ActionResult recibo_items(){
            return View();
        }
        public ActionResult formulario_nuevo_item(){
            return View();
        }

        public JsonResult buscar_items_tabla_inventario(string busqueda){
            return Json(di.ListaInventario(busqueda), JsonRequestBehavior.AllowGet);
        }


        public ActionResult nueva_transferencia()
        {
            return View();
        }
        public ActionResult nueva_transferencia_qr()
        {
            return View();
        }
        //AUTOCOMPLETADOS
        public ActionResult Autocomplete_po(string term) {
            var items = consultas.Lista_po_abiertos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_estilos(string term)
        {
            var items = consultas.Lista_styles();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_paises(string term)
        {
            var items = consultas.Lista_paises();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_fabricantes(string term)
        {
            var items = consultas.Lista_fabricantes();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_colores(string term)
        {
            var items = consultas.Lista_colores();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_colores_codigos(string term)
        {
            var items = consultas.Lista_colores_codigos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_body_type(string term)
        {
            var items = consultas.Lista_body_types();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_tallas(string term)
        {
            var items = consultas.Lista_tallas();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_generos(string term)
        {
            var items = consultas.Lista_generos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_telas(string term)
        {
            var items = consultas.Lista_telas();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_percents(string term)
        {
            var items = consultas.Lista_porcentajes();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_customer_name(string term)
        {
            var items = consultas.Lista_customers();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_cliente_final(string term)
        {
            var items = consultas.Lista_clientes_finales();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_ubicacion(string term)
        {
            var items = consultas.Lista_ubicaciones();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Autocomplete_unidades(string term)
        {
            var items = consultas.Lista_units();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_locacion(string term)
        {
            var items = consultas.Lista_customers();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_family(string term)
        {
            var items = consultas.Lista_family();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_trims(string term)
        {
            var items = consultas.Lista_trims();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_yarns(string term)
        {
            var items = consultas.Lista_yarn();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_units(string term)
        {
            var items = consultas.Lista_units();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        //AUTOCOMPLETADOS
        public ActionResult lista_pos(string term) {
            return Json(consultas.Lista_po_abiertos(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult lista_estilos_dropdown(string ID) {
            //var items = di.Lista_po();
            //var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            Session["po"] = ID;
            return Json(consultas.Lista_estilos_dropdown(ID), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult lista_lugares(){
            List<int> lista = dt.qrs();
            foreach (int i in lista) {
                GenerateMyQCCode("ubicacion_"+Convert.ToString(i));
            }
            return Json(dt.lista_lugares_transfer(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult lista_lugares_destino(string ID){
            return Json(dt.lista_lugares_transfer_destino(ID), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult inventario_sucursal(string ID, string busqueda){
            return Json(dt.obtener_inventario_sucursal(ID, busqueda), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult guardar_nuevo_sello(string inicio, string final, string sucursal) {
            di.guardar_sello(inicio, final, sucursal);
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult lista_sellos(string ID) {
            return Json(consultas.buscar_sello_nuevo(Convert.ToInt32(ID)), JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        public JsonResult guardar_transferencia(string ids, string cantidades, string fecha, string persona, string sello, string origen, string destino, string driver, string pallet, string envio, string id_sello, string pos, string estilos,string caja,string carro,string placas,string codigos)
        {
            int id_transferencia = 0, total = 0, id_sucursal = 0;
            string[] Cantidades, Ids, Pos, Estilos,Cajas,Codigo;
            Cantidades = cantidades.Split('*');
            Ids = ids.Split('*');
            Pos = pos.Split('*');
            Estilos = estilos.Split('*');
            Cajas = caja.Split('*');
            Codigo= codigos.Split('*');
            for (int i = 1; i < Cantidades.Length; i++){
                total += Convert.ToInt32(Cantidades[i]);
            }
            dt.guardar_transferencia_inventario(fecha, persona, sello, origen, destino, driver, pallet, envio, total, Convert.ToInt32(Session["id_usuario"]), id_sello,carro,placas);
            id_transferencia = dt.obtener_ultima_transferencia();
            id_sucursal = consultas.buscar_id_sucursal_usuario(Convert.ToInt32(Session["id_usuario"]));
            if (sello != "0"){
                dt.aumentar_sellos(sello, id_sucursal);
            }
            dt.revisar_sellos(id_sello, sello);
            for (int i = 1; i < Cantidades.Length; i++) {
                int pedido = consultas.buscar_pedido(Pos[i]);
                int estilo = consultas.obtener_estilo_id(Estilos[i]);
                dt.guardar_items_inventario(id_transferencia, Ids[i], Cantidades[i], pedido, estilo,Cajas[i],Codigo[i]);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult lista_transferencias() {
            return PartialView(dt.ListaTransferencias());
        }
        [HttpGet]
        public ActionResult lista_recibos() {
            string id_sucursal = consultas.obtener_sucursal_id_usuario(Convert.ToInt32(Session["id_usuario"]));
            Session["id_sucursal"] = id_sucursal;
            return PartialView();
        }
        [HttpPost]
        public JsonResult aprobar_transferencia(string ID){
            dt.aprobar_transferencia_inventario(ID);
            GenerateMyQCCode("transferencia_" + ID);
            dt.aprobar_transferencia_items(ID);
            return Json(dt.ListaTransferencias(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult desaprobar_transferencias(string ID) {
            dt.negar_transferencia_inventario(ID);
            return Json(ID, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult buscar_transferencia(string ID) {
            return Json(dt.obtener_informacion_transferencia(ID), JsonRequestBehavior.AllowGet);
        }        
        [HttpGet]
        public ActionResult edicion_inventario(int id)
        {
            Session["id_inventario_editar"] = id;
           return PartialView(dt.obtener_informacion_inventario(id));
           // return Json(dt.obtener_informacion_inventario(id), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult get_item_data() {
            return Json(dt.obtener_item_editar(Convert.ToInt32(Session["id_inventario_editar"])), JsonRequestBehavior.AllowGet);
        }
        /****************************************************************************************************************************************/
        [HttpPost]
        public JsonResult guardar_edicion_inventario_trim(string id, string estilo, string tipo, string po,  string unit, string company, string cantidad, string descripcion, string familia, string minimo)
        {
            if (estilo == "undefined") estilo = "0";
            di.obtener_datos_trim(0,Convert.ToInt32(Session["id_usuario"]), estilo, tipo, po,  unit, company, cantidad, descripcion, familia, minimo);
            di.cantidad = Convert.ToInt32(cantidad); 
            di.id_inventario = Convert.ToInt32(id);
            di.guardar_edicion_trim_po();
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult guardar_edicion_inventario_blank(string datos)
        {
            string[] data = datos.Split('*');
            if (data[0] == "undefined") data[0] = "0"; // Estilo
            di.id_inventario = Convert.ToInt32(data[17]);
            int customer_final = consultas.buscar_customer_final(data[13]);
            di.color_aux = data[5];
            di.obtener_datos_blank(0,Convert.ToInt32(Session["id_usuario"]), data[0], "Blanks", data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12], customer_final, data[14], data[15],data[16], "0", "0");
            di.guardar_edicion_blank();
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        public JsonResult cambiar_sello_transferencia(string ID)
        {
            salidas salida = new salidas();
            salidas salida_sello_nuevo = new salidas();
            salida = dt.obtener_datos_cambio_sello(ID);//SUCURSAL, ID_SELLO, SELLO
            salida_sello_nuevo = consultas.buscar_sello_nuevo(salida.id_sucursal); //SELLO ID_SELLO
            dt.cambiar_sello(Convert.ToInt32(ID), salida_sello_nuevo.sello, salida_sello_nuevo.id_sello);
            dt.aumentar_sellos(Convert.ToString(salida_sello_nuevo.sello), salida.id_sucursal);
            dt.revisar_sellos(Convert.ToString(salida_sello_nuevo.id_sello), Convert.ToString(salida_sello_nuevo.sello));
            return Json("", JsonRequestBehavior.AllowGet);
        }
        //*****************************************************************************************************************************************************
        [HttpGet]
        public ActionResult lista_recepcion_transferencias(){
            return PartialView(dt.lista_transferencias_por_recibir(Convert.ToString(Session["id_sucursal"])));
        }      

        

        public JsonResult obtener_categorias_inventario() {
            return Json(consultas.buscar_categorias_inventario(), JsonRequestBehavior.AllowGet);
        }
        /******************************************************************************************************************************************/
        [HttpPost]
        public JsonResult agregar_item_catalogo(string actionData,string unit,string minimo){
            string[] datos = actionData.Split('*');
            string[] tallas = datos[3].Split('+');
            
            for (int i = 0; i < tallas.Length; i++) {
                int existencia = di.buscar_existencia_item(actionData,tallas[i]);
                if (existencia == 0){
                    di.guardar_item_nuevo(actionData,tallas[i],unit,minimo);
                    //if () {
                    //}
                }
            }            
            
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult formulario_recibo() {
            string id_sucursal = consultas.obtener_sucursal_id_usuario(Convert.ToInt32(Session["id_usuario"]));
            Session["id_sucursal"] = id_sucursal;
            return PartialView();
        }
        //BUSQUEDA DINAMICA
        public JsonResult busqueda_dinamica_items(string item,string descripcion,string color, string size,string gender) {
            return Json(di.lista_items_para_recibir(item,descripcion,color,size,gender), JsonRequestBehavior.AllowGet);
        }
        //G U A R D A R --- R E C I B O 
        public int obtener_cantidad_item(string caja, string cantidad) {
            string[] cajas = caja.Split('&'), cantidades = cantidad.Split('&');
            int total = 0;
            for (int i = 1; i < cajas.Length; i++) {
                total += Convert.ToInt32(cajas[i]) * Convert.ToInt32(cantidades[i]);
            }
            return total;
        }
        [HttpPost]
        public JsonResult guardar_recibo_inventario(string id, string item, string po, string style, string mill, string po_r, string locacion, string country, string customer, string caja, string cantidad) {
            //SEPARAR INFORMACIÓN
            //Session["id_usuario"] = 2;
            string[] ids = id.Split('*'), items = item.Split('*'), pos = po.Split('*'), styles = style.Split('*'), locaciones = locacion.Split('*');
            string[] countries = country.Split('*'), customers = customer.Split('*'), cajas_item = caja.Split('*'), cantidades_item = cantidad.Split('*');
            string qty_item = "", ids_inventario = "", trims_inventario = "", trims_cantidades = "", trims_item = ""; ;
            int existencia = 0;
            //POR CADA ITEM
            int total_item = 0, total_recibo = 0;
            for (int i = 1; i < ids.Length; i++) {
                di.id_tipo = consultas.buscar_tipo_inventario_item(ids[i]);
                di.mill_po = mill; di.po_referencia = po_r;
                switch (di.id_tipo) {
                    case 2:
                        //int id_usuario, string estilo, string tipo, string po, string mill_po, string amt, string unit, string company, string cantidad, string descripcion_trim, string familia, string minimo,string referencia
                        consultas.buscar_informacion_trim_item(ids[i]);
                        total_item = obtener_cantidad_item(cajas_item[i], cantidades_item[i]);
                        qty_item += "*" + total_item.ToString();
                        total_recibo += total_item;
                        di.cantidad = total_item;
                        di.obtener_datos_trim(Convert.ToInt32(ids[i]), Convert.ToInt32(Session["id_usuario"]), styles[i], "Trims", pos[i], consultas.unit, customers[i], total_item.ToString(), consultas.descripcion, consultas.family, "0");
                       
                        existencia = di.buscar_existencia_trim_inventario();
                        if (existencia == 0){
                            di.guardar_trim_po();
                            di.id_inventario = di.obtener_ultimo_inventario();
                            ids_inventario += "*" + di.id_inventario.ToString();
                        }else{
                            di.sumar_existencia_trim(existencia);
                            ids_inventario += "*" + existencia.ToString();
                        }
                        trims_inventario += "*"+ consultas.obtener_po_summary(di.id_pedido, di.id_estilo);
                        trims_cantidades+= "*"+ total_item.ToString();
                        trims_item+= "*"+ ids[i];
                        break;
                    case 1:
                        consultas.buscar_informacion_blank_item(ids[i]);
                        int customer_final = consultas.buscar_cliente_final_po(pos[i]);
                        di.obtener_datos_blank(Convert.ToInt32(ids[i]), Convert.ToInt32(Session["id_usuario"]), styles[i], "Blanks", pos[i], countries[i], consultas.fabricante, consultas.color, consultas.body_type, consultas.size, consultas.gender, consultas.fabric_type, consultas.fabric_percent, customers[i], locaciones[i], customer_final, "N/A", "N/A", "N/A", cajas_item[i], cantidades_item[i]);
                        qty_item += "*" + di.quantity.ToString();
                        total_recibo += di.quantity;
                        
                        existencia = di.buscar_existencia_blank_inventario();
                        if (existencia == 0){
                            di.guardar_blank();
                            di.id_inventario = di.obtener_ultimo_inventario();
                            ids_inventario += "*" + di.id_inventario.ToString();
                        }else{
                            di.sumar_existencia_blank(existencia);
                            ids_inventario += "*" + existencia.ToString();
                        }
                        break;
                }//switch
            }//FOR
            di.guardar_recibo(total_recibo, 0, mill, po_r);
            di.id_recibo = di.obtener_ultimo_recibo();
            GenerateMyQCCode("recibo_" + di.id_recibo.ToString());
            string[] trimsInventario = trims_inventario.Split('*'), trimsCantidad = trims_cantidades.Split('*'), trimsItem= trims_item.Split('*');
            //REVISAR TRIMS 
            /*for (int tr = 1; tr < trimsInventario.Length; tr++){
                di.buscar_item_trim_request(Convert.ToInt32(trimsInventario[tr]), Convert.ToInt32(trimsCantidad[tr]), Convert.ToInt32(trimsItem[tr]), di.id_recibo);
            }*/
            //TRIMS
            string[] inventarios = ids_inventario.Split('*'), totales_items = qty_item.Split('*');
            for (int j = 1; j < inventarios.Length; j++){
                di.guardar_recibo_item(di.id_recibo, inventarios[j], totales_items[j]);
                di.id_recibo_item = di.obtener_ultimo_recibo_item();
                string[] cajas = cajas_item[j].Split('&'), cantidades = cantidades_item[j].Split('&');
                for (int k = 1; k < cajas.Length; k++) {
                    for (int h = 0; h < Convert.ToInt32(cajas[k]); h++) {
                        di.guardar_caja(di.id_recibo_item, inventarios[j], cantidades[k], cantidades[k]);
                        di.id_caja = di.obtener_id_ultima_caja();
                        GenerateMyQCCode("caja_" + Convert.ToString(di.id_caja));
                    }
                }
            }
            Session["id_recibo_nuevo"]= di.id_recibo;
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        private void GenerateMyQCCode(string QCText) {
            var QCwriter = new BarcodeWriter();
            QCwriter.Format = BarcodeFormat.QR_CODE;
            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
            };
            var result = QCwriter.Write(QCText);
            string path = Server.MapPath("~/Content/img/QR/" + QCText + ".jpg");
            //string nombre = "caja_" + QCText + ".jpg";
            var barcodeBitmap = new Bitmap(result, new Size(200, 200));
            using (MemoryStream memory = new MemoryStream()) {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite)) {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
        }
        [HttpPost]
        public JsonResult imprimir_etiquetas_cajas(string ID) {
            Session["id_recibo_nuevo"] = ID;
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        private void ReadQRCode(string imagen) {
            var QCreader = new BarcodeReader();
            string QCfilename = Path.Combine(Request.MapPath
               ("~/Content/img/QR/"), imagen);
            var QCresult = QCreader.Decode(new Bitmap(QCfilename));
            if (QCresult != null) {
                string x = "My QR Code: " + QCresult.Text;
            }
        }
        public JsonResult obtener_recibos_lista() {
            return Json(di.Listarecibos(), JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult guardar_mp_recibo(string recibo, string mp)
        {
            di.agregar_mp_recibo(recibo, mp);
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_recibo(string recibo)
        {
            return Json(di.lista_recibo_detalles(recibo), JsonRequestBehavior.AllowGet);
        }

        public void excel_diario(){            
            using (XLWorkbook libro_trabajo = new XLWorkbook())
            {
                var ws = libro_trabajo.Worksheets.Add("FORTUNE").SetTabColor(XLColor.FromArgb(146, 208, 80));
                ws.Range(1, 1, 1, 27).Style.Fill.BackgroundColor = XLColor.FromArgb(146, 208, 80);
                ws.Cell(1,28).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                ws.Range(1, 1, 1, 30).SetAutoFilter();                                
                var headers = new List<String[]>();
                headers.Add(new String[] { "MANUFACTURER   (2 LETTER CODE) FABRICANTE DE CAMISA","COUNTRY OF ORIGIN","AMT STYLE","AMT COLOR ","BODY TYPE","COLOR","SIZE","GENDER",
                    "BLANKS (B) NORMAL (N) TRIM (T) DAMAGES (D)","FABRIC TYPE","FABRIC %","WAREHOUSE","LOCATION","QTY","FISHBOWL","ORDER REF #","DATE OF RECEIPT (AGING)","AGE (days)",
                    "CANCEL DATE W.O","MILL PO","CUSTOMER NAME","# PALLET","AMT STYLE DESCRIPTION","DATE OF COMMENT","COMMENTS","PURCHASED FOR"});
                ws.Cell(1, 1).AsRange().AddToNamed("Titles");
                ws.Cell(1, 1).Value = headers;
                var row1 = ws.Row(1);
                row1.Height = 80;
                row1.Style.Font.Bold = true;
                row1.Style.Alignment.WrapText = true;
                ws.Cell("R1").Style.Font.FontColor = XLColor.FromArgb(255,0,102);
                ws.Cell("S1").Style.Font.FontColor = XLColor.FromArgb(112,48,160);
                row1.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                row1.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Style.Font.FontSize = 10;
                ws.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                ws.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell(1,1).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell(1,1).Style.Border.LeftBorderColor = XLColor.Blue;

                var inventario_stock = new List<string[]>();
                List<Inventario> lista_inventario_stock = new List<Inventario>();
                lista_inventario_stock=dr.obtener_item_diario(1);//AQUI VA EL ID DE LA SUCURSAL
                foreach(Inventario i in lista_inventario_stock) {
                    string fecha_recibo = consultas.obtener_fecha_recibo(i.id_inventario);
                    inventario_stock.Add(new string[] {i.fabricante,i.pais,i.amt_item,i.codigo_color,i.body_type,i.color,i.size,i.genero,i.categoria_inventario,
                        i.fabric_type,i.fabric_percent,"FORT",i.location,Convert.ToString(i.total),"ADD",i.po,fecha_recibo,consultas.item_age.ToString(),"",consultas.mill_po,
                        i.customer,i.date_comment,i.comment,"STOCK",i.notas });
                }
                ws.Cell(2, 1).Value = inventario_stock;

        /***********F*I*R*S*T**T*A*B***E*N*D****************************************************************************************************************************************************************/

                var mac = libro_trabajo.Worksheets.Add("MOVE AND CHANGE").SetTabColor(XLColor.FromArgb(255, 0, 0));
                mac.Range(1, 1, 1,30).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 0, 0);
                mac.Range(1, 1, 1, 30).SetAutoFilter();
                var headers_MAC = new List<String[]>();
                headers_MAC.Add(new String[] { "MANUFACTURER   (2 LETTER CODE) FABRICANTE DE CAMISA","COUNTRY OF ORIGIN","AMT STYLE","AMT COLOR ","BODY TYPE","COLOR","SIZE","GENDER",
                    "BLANKS (B) NORMAL (N) TRIM (T) DAMAGES (D)","FABRIC TYPE","FABRIC %","WAREHOUSE","LOCATION","QTY","","ORDER REF #","DATE OF RECEIPT (AGING)","AGE (days)",
                    "CANCEL DATE W.O","MILL PO","CUSTOMER NAME","AMT STYLE # (16 DIGITS)","AMT STYLE DESCRIPTION","DATE OF COMMENT","COMMENTS","","CODIGO DE IDENTIFICACION","MOVEMENT DATE","NOTES","MOVEMENT TYPE"});
                mac.Cell(1, 1).AsRange().AddToNamed("Titles");
                mac.Cell(1, 1).Value = headers;
                var row1_mac = mac.Row(1);
                row1_mac.Height = 80;
                row1_mac.Style.Font.Bold = true;
                row1_mac.Style.Alignment.WrapText = true;
                row1_mac.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                row1_mac.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                mac.Style.Font.FontSize = 10;
                mac.Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                mac.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                mac.Cell(1, 1).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                mac.Cell(1, 1).Style.Border.LeftBorderColor = XLColor.Blue;
                mac.Column("AA").Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0);
                var move_change = new List<string[]>();//TRANSFERENCIAS POR MES
                List<Inventario> lista_move_change = new List<Inventario>();
                lista_move_change = dr.obtener_inventario_transferencia_mes(Convert.ToInt32(Session["id_sucursal"]));//AQUI VA EL ID DE LA SUCURSAL
                if (dr.fecha_salidas != null)
                {
                    string[] fecha_salida = dr.fecha_salidas.Split('*'), id_salidas = dr.ids_salidas.Split('*');
                    int x = 1;
                    foreach (Inventario i in lista_move_change)
                    {
                        string fecha_recibo = consultas.obtener_fecha_recibo(i.id_inventario);
                        move_change.Add(new string[] {i.fabricante,i.pais,i.amt_item,i.codigo_color,i.body_type,i.color,i.size,i.genero,i.categoria_inventario,
                        i.fabric_type,i.fabric_percent,"FORT",i.location,Convert.ToString(i.total),"ADD",i.po,fecha_recibo,consultas.item_age.ToString(),"",consultas.mill_po,
                        i.customer,i.date_comment,i.comment,"HOUSE","STOCK",fecha_salida[x],dr.buscar_datos_transferencia(id_salidas[x]),"MOVE" });
                        x++;
                    }
                }                
                mac.Cell(2, 1).Value = move_change;
                dr.fecha_salidas = null;
                dr.ids_salidas = null;
        /***********S*E*C*O*N*D**T*A*B**E*N*D****************************************************************************************************************************************************************/

  
                

        /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Inventory MDE "+ DateTime.Now.ToString("MMMM dd, yyyy")+".xlsx\"");
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

        public void camreader() {
        }
        public JsonResult Scan(HttpPostedFileBase file){
            /*string barcode = "";
            try{
                string path = "";
                if (file.ContentLength > 0){
                    var fileName = Path.GetFileName(file.FileName);
                    path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                    file.SaveAs(path);
                }
                // Now we try to read the barcode
                // Instantiate BarCodeReader object
                BarCodeReader reader = new BarCodeReader(path, BarCodeReadType.Code39Standard);
                System.Drawing.Image img = System.Drawing.Image.FromFile(path);
                System.Diagnostics.Debug.WriteLine("Width:" + img.Width + " - Height:" + img.Height);
                try{
                    // read Code39 bar code
                    while (reader.Read()){
                        // detect bar code orientation
                        ViewBag.Title = reader.GetCodeText();
                        barcode = reader.GetCodeText();
                    }
                    reader.Close();
                }catch (Exception exp){
                    System.Console.Write(exp.Message);
                }
            }catch (Exception ex){
                ViewBag.Title = ex.Message;
            }*/
            //return Json(barcode);
            return Json("0");
        }

        //[HttpPost]
        public JsonResult datos_imprimir_transfer(string ID)
        {
            Session["id_transfer_ticket"] = ID;            
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        //agregar_nuevo_lugar
        public JsonResult agregar_nuevo_lugar(string nombre, string direccion, string tipo)
        {
            int existencia=dt.revisar_existencia_lugar(nombre);
            if (existencia == 0)
            {
                dt.guardar_nuevo_lugar(nombre, direccion, tipo);
            }            
            return Json(existencia, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_datos_grafica(){
            return Json(dt.obtener_lista_grafica(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_datos_grafica_transferencias()
        {
            return Json(dt.obtener_lista_grafica_transferencias(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_productos_ubicacion(string ID){
            string[] cadena = ID.Split('_');
            return Json(dt.buscar_lista_productos_ubicacion(cadena[1]), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_productos_caja(string ID){
            string[] cadena = ID.Split('_');
            return Json(dt.buscar_lista_productos_cajas(cadena[1]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_estilo_cajas(string inventario){
            return Json(dt.buscar_estilo_inventario(Convert.ToInt32(inventario))+"*"+ dt.buscar_cajas_inventario(Convert.ToInt32(inventario)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult aceptar_transferencia(string id){
            int id_destino = 0, existencia = 0;
            //int cantidad_temporal, total_caja, identificador_caja;
            List<salidas> transferencia = dt.obtener_informacion_transferencia(id);
            foreach (salidas item in transferencia){
                id_destino = item.id_destino;
                foreach (salidas_item i in item.lista_salidas_item){
                    Inventario inventario = new Inventario();
                    inventario = dt.consultar_item(i.id_inventario);
                    //buscar si en el destino de la transferencia ya existen esos item
                    existencia = dt.comparar_inventario(inventario, id_destino);
                    if (existencia != 0){
                        //si existen se suma 
                        di.update_stock(existencia, i.cantidad, id_destino);
                    }else{
                        //si no se agregan con la  sucursal de destino
                        dt.agregar_inventario_desde_transferencia(inventario, id_destino, i.cantidad);
                        existencia = di.obtener_ultimo_inventario();
                    }
                    dt.agregar_id_inventario_nuevo_transferencia(i.id_salida_item, existencia);
                    string[] codigo = (i.codigo).Split('_');
                    if ((i.codigo).Contains("caja")){
                        int cantidad_caja = dt.obtener_contenido_caja(Convert.ToInt32(codigo[1]));
                        if ((i.codigo).Contains("caja") && (cantidad_caja == i.cantidad)){
                            dt.cambiar_id_inventario_caja(existencia, codigo[1]);
                        }
                    }else{
                        //crear una nueva caja con ese id de inventario y cantidad
                        di.guardar_caja(i.id_salida_item, (existencia).ToString(), (i.cantidad).ToString(), (i.cantidad).ToString());
                    }
                }
            }
            dt.actualizar_transferencia(Convert.ToInt32(Session["id_usuario"]), Convert.ToInt32(id));
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult guardar_id_salida(string id){
            Session["id_salida_editar"] = id;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_informacion_salida(){
            return Json(dt.obtener_informacion_transferencia(Convert.ToString(Session["id_salida_editar"])), JsonRequestBehavior.AllowGet);
        }
        public ActionResult abrir_edicion(){
            return View("editar_transferencia");
        }
        public JsonResult guardar_edicion_transferencia(string informacion,string estado,string items_salida){
            // 0      1            2     3       4       5       6       7       8       9      10       11    12      13     14     15       16
            //ids, cantidades, fecha, persona, sello, origen, destino, driver, pallet, envio, id_sello, pos, estilos, caja, carro, placas, codigos
            string[] datos= informacion.Split('+');
            int id_transferencia = Convert.ToInt32(Session["id_salida_editar"]), total = 0, id_sucursal = 0;
            string[] Cantidades, Ids, Pos, Estilos, Cajas, Codigo;
            Cantidades = datos[1].Split('*');
            Ids = datos[0].Split('*');
            Pos = datos[11].Split('*');
            Estilos = datos[12].Split('*');
            Cajas = datos[13].Split('*');
            Codigo = datos[16].Split('*');
            for (int i = 1; i < Cantidades.Length; i++){
                total += Convert.ToInt32(Cantidades[i]);
            }
            dt.guardar_edicion_transferencia_inventario(id_transferencia,datos[2], datos[3], datos[4], datos[5], datos[6], datos[7], datos[8], datos[9], total, Convert.ToInt32(Session["id_usuario"]), datos[10], datos[14], datos[15]);
            id_sucursal = consultas.buscar_id_sucursal_usuario(Convert.ToInt32(Session["id_usuario"]));
            
            if (datos[4] != "0"){
                dt.aumentar_sellos(datos[4], id_sucursal);
            }
            string[] salidasitems = items_salida.Split('*');
            if (estado == "1") {//si esta aprobada                
                for(int ii= 1; ii < salidasitems.Length; ii++){
                    salidas_item sa = dt.obtener_datos_salida_item(Convert.ToInt32(salidasitems[ii]));
                    //regresar_inventario
                    di.update_inventario(sa.id_inventario, sa.cantidad);
                    //regresar cajas
                    dt.regresar_datos_cajas(sa.id_inventario, sa.cantidad);                   
                }
            }
            //borrar
            for (int ii = 1; ii < salidasitems.Length; ii++){
                dt.eliminar_salida_item(Convert.ToInt32(salidasitems[ii]));
            }

            for (int i = 1; i < Cantidades.Length; i++){
                int pedido = consultas.buscar_pedido(Pos[i]);
                int estilo = consultas.obtener_estilo_id(Estilos[i]);
                dt.guardar_items_inventario(id_transferencia, Ids[i], Cantidades[i], pedido, estilo, Cajas[i], Codigo[i]);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }


























    }
}