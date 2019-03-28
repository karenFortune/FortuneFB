using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Trims;
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
using Rotativa;
using System.Text.RegularExpressions;

namespace FortuneSystem.Controllers
{
    public class TrimsController : Controller
    {
        DatosInventario di = new DatosInventario();
        DatosTrim dtrim = new DatosTrim();
        DatosReportes dr = new DatosReportes();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        QRCodeController qr = new QRCodeController();
        PDFController pdf = new PDFController();
        string filename, footer_alineacion, footer_size, vista;

        public ActionResult Index(){
            //Session["id_usuario"] = consultas.buscar_id_usuario(Convert.ToString(Session["usuario"]));
            int id_usuario = Convert.ToInt32(Session["id_Empleado"]);
            Session["id_usuario"] = id_usuario;
            if (Session["usuario"] == null){
                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
                //return View(di.ListaInventario());
            }else{
                bool isExcelInstalled = Type.GetTypeFromProgID("Excel.Application") != null ? true : false;
                //return View(di.ListaInventario());
            }
            return View();
        }
        //AUTOCOMPLETADOS
        public ActionResult Autocomplete_po(string term)
        {
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
        public JsonResult buscar_informacion_recibos_trims(string busqueda){
            return Json(dtrim.obtener_lista_recibos_trim(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_pedidos(){
            return Json(dtrim.lista_ordenes(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos(string pedido){
            var result = Json(new
            {
                trims_anteriores = dtrim.obtener_trims_anteriores_pedido(pedido),//buscar las tallas del esa
                estilos=dtrim.lista_estilos_dropdown(pedido)
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_tallas_estilo(string estilo){
            return Json(dtrim.lista_tallas_dropdown(estilo), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_inventario_items_trim(){
            return Json(dtrim.lista_trim_items(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_total_estilo(string summary,string talla,string item){            
            var result = Json(new {
                total = dtrim.obtener_total_estilo(summary, talla),
                categoria = dtrim.obtener_family_trim_item(item),
                cajas=dtrim.obtener_cajas_estilo(summary,talla) , 
                //trims_anteriores=dtrim.obtener_trims_anteriores(summary)//buscar las tallas del esa
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_anteriores_pedido(string pedido){
            var result = Json(new{               
                trims_anteriores = dtrim.obtener_trims_anteriores_pedido(pedido)//buscar las tallas del esa
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_datos_request(string total,string estilo,string item,string talla,string cantidad,string blank,string request){
            string[] totales = total.Split('*'), estilos = estilo.Split('*'), items = item.Split('*'), tallas = talla.Split('*');
            string[] cantidades = cantidad.Split('*'), blanks = blank.Split('*'), requests = request.Split('*');
            for (int i = 1; i < totales.Length; i++) {
                if (requests[i] != "0") {
                    dtrim.eliminar_trim_request(requests[i]);
                }
                dtrim.guardar_request(totales[i],estilos[i],items[i],tallas[i],Convert.ToInt32(Session["id_usuario"]),cantidades[i],blanks[i]);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void excel_material_pedir(){
            //string year = Convert.ToString(Session["year_reporte"]);
            List<Trim> lista = dtrim.lista_trims_por_pedir();
            int row = 1;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Material list");
                //CABECERAS TABLA
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("STYLE"); titulos.Add("ITEM"); titulos.Add("TRIM"); titulos.Add("TOTAL"); titulos.Add("DATE");titulos.Add("USER");
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1, row, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(196, 215, 155);
                row++; //AGREGAR CABECERA TABLA
                foreach (Trim t in lista){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(t.po.pedido);
                    datos.Add(t.estilo.estilo);
                    datos.Add(t.item.componente);
                    datos.Add(t.item.descripcion);
                    datos.Add((t.item.total).ToString());
                    datos.Add(t.item.fecha);
                    datos.Add(t.usuario);
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
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
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"Material list to order.xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream())                {
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }
        
        public JsonResult buscar_lista_trims_recibidos(){
            return Json(dtrim.lista_trim_recibidos(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult enviar_datos_auditoria(string salidas,string originales,string nuevas){
            string[] inventario = salidas.Split('*'), original = originales.Split('*'), nueva = nuevas.Split('*');            
            for (int i = 1; i < inventario.Length; i++){                
                dtrim.actualizar_cantidad_inventario(Convert.ToInt32(inventario[i]), Convert.ToInt32(nueva[i]));
                consultas.insertar_registro(Convert.ToInt32(inventario[i]),Convert.ToInt32(Session["id_usuario"]),"Audit","UPDATE");
                dtrim.cambiar_estado_auditoria_inventario(inventario[i]);
                //actualizar cantidades en trims y el estado
                dtrim.revisar_estados_cantidades_trim(Convert.ToInt32(inventario[i]), Convert.ToInt32(nueva[i]));
            }
            return Json(dtrim.comparacion_inventario_trim(salidas), JsonRequestBehavior.AllowGet);
        }
        public JsonResult po_session(string po){
            Session["po_trim"] = po;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult trim_card(){
            //return View("trim_card", dtrim.obtener_datos_trim_card(Convert.ToString(Session["po_trim"]), Convert.ToString(Session["id_usuario"])));
            return new ViewAsPdf("trim_card", dtrim.obtener_datos_trim_card(Convert.ToString(Session["po_trim"]), Convert.ToString(Session["id_usuario"]))){
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--page-offset 0  ",
            };
        }

        public JsonResult buscar_items_trims(){
            return Json(dtrim.lista_descripciones_trims(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_informacion_trim(string id){
            var result = Json(new{
                trim = dtrim.informacion_editar_item_trim(id),                
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult editar_trim_catalogo(string id,string item,string minimo,string descripcion,string family,string unidad){
            dtrim.editar_informacion_trim(id,item,minimo,descripcion,family,unidad);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_tabla_inicio(){
            return Json(dtrim.lista_trims_tabla_inicio(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_ordenes_tabla_inicio(){
            return Json(dtrim.obtener_lista_ordenes_estados("0"), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_recibidos_tabla_inicio(string busqueda,string fecha){
            var result = Json(new{
                tabla = dtrim.obtener_lista_trims_inicio(busqueda, fecha),
                mps = dtrim.buscar_mp_recibos_hoy()
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_especifico_ordenes_tabla_inicio(string busqueda){
            return Json(dtrim.obtener_lista_ordenes_estados(busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult obtener_informacion_recibo(string inventario,string total){
            int recibo = dtrim.obtener_id_recibo_inventario(Convert.ToInt32(inventario), Convert.ToInt32(total));
            return Json(di.lista_recibo_detalles(Convert.ToString(recibo)), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_trims_pedido(string pedido){
            var result = Json(new{
                trims_anteriores = dtrim.obtener_trims_anteriores_pedido(pedido),
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }   
        public JsonResult guardar_entrega(string pedido,string request,string cantidad,string entrega,string recibe,string original){
            string[] requests = request.Split('*'), cantidades = cantidad.Split('*'),originales=original.Split('*');
            dtrim.guardar_entrega(pedido,entrega,recibe);
            int id_entrega = dtrim.obtener_ultima_entrega();
            for (int i = 1; i < requests.Length; i++) {
                dtrim.guardar_entrega_item(id_entrega,requests[i],cantidades[i]);
                dtrim.actualizar_estado_entrega_request(requests[i],cantidades[i]);
                if (Convert.ToInt32(originales[i]) == Convert.ToInt32(cantidades[i])) {
                    int inventario = dtrim.obtener_id_inventario_request(requests[i]);
                    dtrim.actualizar_inventario(inventario,cantidades[i]);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_entregas(string inicio,string final){
            return Json(dtrim.obtener_lista_entregas_fechas(inicio,final), JsonRequestBehavior.AllowGet);
        }
        //ARCHIVO DE "COLORES"
        public JsonResult obtener_estado_total_pedido(string pedido){
            return Json(dtrim.buscar_estado_total_pedido(Convert.ToInt32(pedido)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult sesiones_fechas(string inicio,string final){
            Session["inicio_excel"] = inicio;
            Session["final_excel"] = final;
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public void excel_trims_estados_fechas(){
            string inicio = Convert.ToString(Session["inicio_excel"]);            
            string final = Convert.ToString(Session["final_excel"]);
            List<Pedidos_trim> lista_pedidos = dtrim.obtener_pedidos_fechas(inicio,final);
            List<Family_trim> lista_familias = dtrim.obtener_familias();
            int row = 1,col=0;
            using (XLWorkbook libro_trabajo = new XLWorkbook()){ //Regex.Replace(pedido, @"\s+", " "); 
                var ws = libro_trabajo.Worksheets.Add("Material list");
                //CABECERAS TABLA
                //ws.Worksheets.Add("AutoFilter");
                var headers = new List<String[]>();
                List<String> titulos = new List<string>();
                titulos.Add("PO"); titulos.Add("CUSTOMER"); titulos.Add("SHIP DATE"); titulos.Add("GENDER");
                foreach (Family_trim f in lista_familias) { titulos.Add(f.family_trim); }
                headers.Add(titulos.ToArray());
                ws.Cell(row, 1).Value = headers;
                ws.Range(row, 1, row, 40).Style.Fill.BackgroundColor = XLColor.FromArgb(52, 121, 191);
                ws.Range(row, 1, row, 40).SetAutoFilter();
                ws.Range(row, 1, row, 40).Style.Font.FontSize = 12;
                ws.Range(row, 1, row, 40).Style.Font.Bold = true;
                for (int i = 1; i < 41; i++){
                    ws.Cell(row, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }
                row++; //AGREGAR CABECERA TABLA
                foreach (Pedidos_trim p in lista_pedidos){
                    var celdas = new List<String[]>();
                    List<String> datos = new List<string>();
                    datos.Add(Regex.Replace(p.pedido, @"\s+", " "));
                    datos.Add(Regex.Replace(p.customer, @"\s+", " "));
                    datos.Add(p.ship_date);
                    datos.Add(p.gender);
                    if (p.id_customer == 1) { ws.Cell(row,2).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 203, 229); }
                    col = 5;                     
                    foreach (Family_trim f in p.lista_families){
                        string texto = "",comentario= " Receives:\n ";
                        var c = ws.Cell(row,col);
                        int estado_1 = 0, estado_2 = 0, estado_3 = 0,recibo=0;
                        List<int> lista_estados = new List<int>();
                        foreach (Trim_requests t in f.lista_requests) {
                            texto = t.estilo + " "+t.item+" " + t.talla+" "+t.cantidad+"/"+t.total+"";
                            lista_estados.Add(t.id_estado);
                            c.RichText.AddText(Regex.Replace(texto, @"\s+", " "));
                            c.RichText.AddText(Environment.NewLine);
                            if (t.recibo != 0) {
                                recibo++;
                                comentario += t.recibo_item.fecha + " MP " + t.recibo_item.mp_number + "\n";
                            }
                        }
                        foreach (int i in lista_estados) { if (i == 1) { estado_1++; } if (i == 2) { estado_2++; } if (i == 3) { estado_3++; } }                        
                        if (estado_1 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 122); }
                        if (estado_3 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(174, 252, 174); }
                        if (estado_2 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 133, 133); }
                        if (estado_1 != 0 && estado_2 != 0 && estado_3 != 0) { ws.Cell(row, col).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 255); }
                        //datos.Add(texto);
                        if (recibo!=0){ ws.Cell(row, col).Comment.AddText(comentario); }
                        ws.Cell(row, col).Style.Alignment.WrapText = true;                        
                        col++;
                    }//TRIMS
                    celdas.Add(datos.ToArray());
                    ws.Cell(row, 1).Value = celdas;
                    for (int i = 1; i < 41; i++) {
                        ws.Cell(row,i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(row, i).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }                    
                    row++;
                }//PEDIDOS
                ws.SheetView.FreezeColumns(4);
                ws.Rows().AdjustToContents();
                ws.Columns().AdjustToContents();
                //ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                httpResponse.Clear();
                httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                httpResponse.AddHeader("content-disposition", "attachment;filename=\"TRIM "+inicio+" to "+final+".xlsx\"");
                // Flush the workbook to the Response.OutputStream
                using (MemoryStream memoryStream = new MemoryStream()){
                    libro_trabajo.SaveAs(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }
                httpResponse.End();
            }
        }















    }
}