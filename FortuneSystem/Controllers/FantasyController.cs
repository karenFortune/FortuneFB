using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Shipping;
using FortuneSystem.Models.Fantasy;
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
    public class FantasyController : Controller
    {
        DatosInventario di = new DatosInventario();
        DatosReportes dr = new DatosReportes();
        DatosShipping ds = new DatosShipping();
        DatosFantasy df = new DatosFantasy();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosTransferencias dt = new DatosTransferencias();
        QRCodeController qr = new QRCodeController();
        PDFController pdf = new PDFController();
        int sm, md, lg, xl, xx, registro,total,id_estilo;
        string[] tallas = { "SM", "MD", "LG", "XL", "2X" };
        public ActionResult Index() {
            // Session["id_usuario"] = 2;
            // Session["id_usuario"] = consultas.buscar_id_usuario(Convert.ToString(Session["usuario"]));
            /*if (Session["usuario"] != null){
                return View();
            }else {
                return View();
            }  */
            int id_usuario = Convert.ToInt32(Session["id_Empleado"]);
            Session["id_usuario"] = id_usuario;
            return View();
        }

        public JsonResult buscar_clientes_fantasy(){
            return Json(df.obtener_lista_clientes(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult agregar_cliente(string cliente) {
            df.agregar_nuevo_cliente(cliente);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult editar_nombre_cliente(string id, string cliente) {
            df.editar_cliente(id, cliente);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult editar_estados(string ids, string estados){
            df.cambiar_estado_cliente(ids, estados);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_estilos(string term){
            var items = consultas.Lista_styles();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_estilos_fantasy(){
            return Json(df.obtener_lista_estilos(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult agregar_estilo(string estilo,string cliente,string color){
            df.guardar_nuevo_estilo(estilo,cliente,color);
            df.crear_estilo_inventario(estilo);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult editar_estilo(string id,string estilo,string cliente,string color){
            df.editar_estilo_cliente(id,estilo,cliente,color);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult editar_estados_estilos(string ids, string estados){
            df.cambiar_estado_estilo(ids, estados);
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_colores_codigos(string term){
            var items = consultas.Lista_colores_codigos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult lista_pos(string term){
            return Json(consultas.Lista_po_abiertos(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_po(string term){
            var items = consultas.Lista_po_abiertos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult agregar_inventario_fantasy(string estilo, string talla_sm, string talla_md, string talla_lg, string talla_xl, string talla_xx,  string pedido){
            string[] estilos = estilo.Split('*'), tallas_sm = talla_sm.Split('*'), tallas_md = talla_md.Split('*'); id_estilo = 0;
            string[] tallas_lg = talla_lg.Split('*'), tallas_xl = talla_lg.Split('*'), tallas_xx = talla_xx.Split('*'), pedidos = pedido.Split('*');
            for (int j = 1; j < estilos.Length; j++) {//tallas por 6 porque meten cajas
                sm = Convert.ToInt32(tallas_sm[j])*6; md = Convert.ToInt32(tallas_md[j]) * 6; lg = Convert.ToInt32(tallas_lg[j]) * 6; xl = Convert.ToInt32(tallas_xl[j]) * 6; xx = Convert.ToInt32(tallas_xx[j]) * 6;
                total = sm + md + lg + xl + xx;
                int[] cantidades_talla = { sm, md, lg, xl, xx }, cantidades_batch = { 0, 0, 0, 0, 0 }, cantidades_restantes = cantidades_talla;
                id_estilo = Convert.ToInt32(estilos[j]);
                df.agregar_registro_inventory(id_estilo, total, "STOCK", "", "", pedidos[j]);
                registro = df.obtener_ultimo_registro();
                for (int t = 0; t < cantidades_talla.Length; t++){
                    df.agregar_cantidades_fantasy(registro, cantidades_talla[t], 0, tallas[t], id_estilo);
                    df.agregar_inventario(id_estilo, tallas[t],cantidades_talla[t]);
                }
                
                List<Cantidades> lista_batch = df.obtener_lista_batch_pendientes(id_estilo);
                List<InventarioFantasy> i = df.obtener_inventario_estilo(id_estilo);
                foreach (Cantidades c in lista_batch) {                    
                    int talla_contador = 0;
                    foreach (InventarioFantasy fantasy in i){
                        if (fantasy.talla == c.talla){
                            if (fantasy.total >= c.total){
                                df.actualizar_batch(c.id_cantidad,0);
                                cantidades_restantes[talla_contador] = fantasy.total-c.total;
                                df.actualizar_inventario(id_estilo, fantasy.talla, cantidades_restantes[talla_contador]);
                            }else{
                                if (fantasy.total > 0) {
                                    cantidades_restantes[talla_contador] = 0;
                                    df.actualizar_batch(c.id_cantidad, (cantidades_talla[talla_contador] - fantasy.total));
                                    df.actualizar_inventario(id_estilo, fantasy.talla, 0);
                                }                               
                            }
                        }
                        talla_contador++;
                    }
                    df.verificar_batch(c.id_registro);
                }
                List<InventarioFantasy> iventario = df.obtener_inventario_estilo(id_estilo);
                df.agregar_registro_inventory(id_estilo, cantidades_restantes.Sum(), "REMAINING", "COMPLETED", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), pedidos[j]);
                registro = df.obtener_ultimo_registro();
                foreach (InventarioFantasy fantasy in iventario) {
                    df.agregar_cantidades_fantasy(registro, fantasy.total, 0, fantasy.talla, id_estilo);
                }
                /*for (int t = 0; t < tallas.Length; t++){
                    df.agregar_cantidades_fantasy(registro, cantidades_restantes[t], 0, tallas[t], id_estilo);
                }*/
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        public ActionResult nueva_orden(string cliente, string estilo, string instruccion, string talla_sm, string talla_md, string talla_lg, string talla_xl, string talla_xx, string ship_date, string pedido)
        {
            string[] estilos = estilo.Split('*'), instrucciones = instruccion.Split('*'), tallas_sm = talla_sm.Split('*'), tallas_md = talla_md.Split('*');
            string[] tallas_lg = talla_lg.Split('*'), tallas_xl = talla_xl.Split('*'), tallas_xx = talla_xx.Split('*'), pedidos = pedido.Split('*');
            string r = "";
            int b = 0;
            ship_date += " 08:00:00";
            for (int j = 1; j < estilos.Length; j++){
                sm = Convert.ToInt32(tallas_sm[j]); md = Convert.ToInt32(tallas_md[j]); lg = Convert.ToInt32(tallas_lg[j]); xl = Convert.ToInt32(tallas_xl[j]); xx = Convert.ToInt32(tallas_xx[j]);
                total = sm + md + lg + xl + xx;
                //id_estilo = consultas.obtener_estilo_id(estilo);
                id_estilo = Convert.ToInt32(estilos[j]);
                int[] cantidades_talla = { sm, md, lg, xl, xx }, cantidades_batch = { 0, 0, 0, 0, 0 }, cantidades_restantes = { 0, 0, 0, 0, 0 };
                if (instrucciones[j] == "INVENTORY"){
                    df.agregar_registro_inventory(id_estilo, total, "INVENTORY", "WAITING", ship_date, pedidos[j]);
                    registro = df.obtener_ultimo_registro();
                    df.agregar_cantidades_fantasy(registro, sm, sm, "SM", id_estilo);
                    df.agregar_cantidades_fantasy(registro, md, md, "MD", id_estilo);
                    df.agregar_cantidades_fantasy(registro, lg, lg, "LG", id_estilo);
                    df.agregar_cantidades_fantasy(registro, xl, xl, "XL", id_estilo);
                    df.agregar_cantidades_fantasy(registro, xx, xx, "2X", id_estilo);
                }else if (instrucciones[j] == "PULL"){
                    //df.agregar_registro_inventory(id_estilo, total, "ORDER", "COMPLETED", ship_date, pedidos[j]);
                    df.agregar_registro_inventory(id_estilo, total, "PULL", "COMPLETED", ship_date, pedidos[j]);
                    registro = df.obtener_ultimo_registro();
                    /*for (int t = 0; t < cantidades_talla.Length; t++){
                        df.agregar_cantidades_fantasy(registro, cantidades_talla[t], 0, tallas[t], id_estilo);
                    }*/
                    int batch = 0;
                    List<InventarioFantasy> i = df.obtener_inventario_estilo(id_estilo);
                    for (int t = 0; t < cantidades_talla.Length; t++){
                        df.agregar_cantidades_fantasy(registro, cantidades_talla[t], 0, tallas[t], id_estilo);
                        foreach (InventarioFantasy fantasy in i){
                            if (fantasy.talla == tallas[t]){
                                if (cantidades_talla[t] != 0){
                                    if (fantasy.total >= cantidades_talla[t] && fantasy.total != 0){
                                        //df.agregar_cantidades_fantasy(registro, cantidades_talla[t], 0, fantasy.talla, id_estilo);
                                        cantidades_restantes[t] = fantasy.total - cantidades_talla[t];
                                        df.actualizar_inventario(id_estilo, fantasy.talla, (fantasy.total - cantidades_talla[t]));
                                    }else{
                                        batch++;
                                        cantidades_restantes[t] = 0;
                                        //df.agregar_cantidades_fantasy(registro, cantidades_talla[t], 0, fantasy.talla, id_estilo);
                                        cantidades_batch[t] = (cantidades_talla[t] - fantasy.total) + 30;
                                        df.actualizar_inventario(id_estilo, fantasy.talla, 0);
                                    }
                                    df.verificar_batch(registro);
                                }
                            }
                        }
                    }
                    List<InventarioFantasy> iventario = df.obtener_inventario_estilo(id_estilo);
                    df.agregar_registro_inventory(id_estilo, cantidades_restantes.Sum(), "REMAINING", "COMPLETED", ship_date, pedidos[j]);
                    registro = df.obtener_ultimo_registro();
                    foreach (InventarioFantasy fantasy in iventario){
                        df.agregar_cantidades_fantasy(registro, fantasy.total, 0, fantasy.talla, id_estilo);
                    }
                    /*for (int t = 0; t < cantidades_talla.Length; t++){
                        df.agregar_cantidades_fantasy(registro, cantidades_restantes[t], 0, tallas[t], id_estilo);
                    }*/

                    if (batch > 0){
                        b++;
                        df.agregar_registro_inventory(id_estilo, cantidades_batch.Sum(), "BATCH", "WAITING", ship_date, pedidos[j]);
                        registro = df.obtener_ultimo_registro();
                        r += "*" + registro.ToString();
                        for (int t = 0; t < cantidades_talla.Length; t++){
                            df.agregar_cantidades_fantasy(registro, cantidades_batch[t], cantidades_batch[t], tallas[t], id_estilo);
                        }
                    }
                }
            }
            if (b != 0)
            {
                Session["registros"] = r;
            }
            else
            {
                Session["registros"] = null;
            }
            return Json(b, JsonRequestBehavior.AllowGet);
        }

        public void batch_excel(){
            
                string r = Convert.ToString(Session["registros"]);

                List<Registro> lista = df.obtener_registro_completo(Convert.ToString(Session["registros"]));
                List<Cliente> lista_clientes = df.obtener_lista_clientes();
                using (XLWorkbook libro_trabajo = new XLWorkbook()){
                    var ws = libro_trabajo.Worksheets.Add("Batch");
                    int row = 1, column = 4, existe_estilo = 0, total_piezas;
                    int red = 0, green = 0, blue = 0, registros;
                    foreach (Cliente c in lista_clientes)
                    {
                        existe_estilo = 0;
                        foreach (Registro reg in lista)
                        {
                            if (reg.cliente.id_cliente == c.id_cliente)
                            {
                                existe_estilo++;
                            }
                        }
                        registros = 0;
                        row++;
                        if (existe_estilo > 0)
                        {//bravado,fea,merch
                            foreach (Registro reg in lista)
                            {
                                if (reg.cliente.id_cliente == c.id_cliente)
                                {
                                    if (registros == 0)
                                    {
                                        if ((reg.cliente.nombre).Contains("BRAVADO")) { red = 255; green = 192; blue = 0; }
                                        if ((reg.cliente.nombre).Contains("FEA")) { red = 255; green = 255; blue = 0; }
                                        if ((reg.cliente.nombre).Contains("MERCH")) { red = 255; green = 153; blue = 255; }

                                        ws.Cell(row, column).Value = reg.cliente.nombre + " REPLENISHMENT";
                                        ws.Cell(row, column).Style.Font.Bold = true;
                                        ws.Cell(row, column).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                        ws.Range(row, column, row, 6).Merge();
                                        ws.Range(row, column, row, 6).Style.Fill.BackgroundColor = XLColor.FromArgb(red, green, blue);
                                        ws.Range(row, column, row, 6).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, column, row, 6).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, column, row, 6).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Range(row, column, row, 6).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, column, row, 6).Style.Border.RightBorderColor = XLColor.Black;
                                        ws.Range(row, column, row, 6).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, column, row, 6).Style.Border.TopBorderColor = XLColor.Black;
                                        ws.Range(row, column, row, 6).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, column, row, 6).Style.Border.BottomBorderColor = XLColor.Black;

                                        row++;
                                        var headers = new List<String[]>();
                                        List<String> titulos = new List<string>();
                                        titulos.Add("PO"); titulos.Add("STYLE"); titulos.Add("CLR"); titulos.Add("DESCRIPTION"); titulos.Add("SM"); titulos.Add("MD");
                                        titulos.Add("LG"); titulos.Add("XL"); titulos.Add("2X"); titulos.Add("PCS"); titulos.Add("BXS"); titulos.Add("COMMENTS");
                                        headers.Add(titulos.ToArray());
                                        ws.Cell(row, 1).Value = headers;
                                        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromArgb(51, 153, 255);
                                        ws.Range(row, 1, row, 12).Style.Font.Bold = true;
                                        ws.Range(row, 1, row, 12).Style.Fill.BackgroundColor = XLColor.FromArgb(51, 153, 255);//cabecera azul                                       
                                        ws.Range(row, 1, row, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, 1, row, 12).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, 1, row, 12).Style.Border.LeftBorderColor = XLColor.Black;
                                        ws.Range(row, 1, row, 12).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, 1, row, 12).Style.Border.RightBorderColor = XLColor.Black;
                                        ws.Range(row, 1, row, 12).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, 1, row, 12).Style.Border.TopBorderColor = XLColor.Black;
                                        ws.Range(row, 1, row, 12).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                                        ws.Range(row, 1, row, 12).Style.Border.BottomBorderColor = XLColor.Black;
                                        row = row + 1;
                                    }//cabecera

                                    //DATOS
                                    var celdas_estilos = new List<String[]>();
                                    List<String> datos = new List<string>();
                                    datos.Add(reg.pedido);
                                    datos.Add(reg.estilo.estilo);
                                    datos.Add(reg.estilo.color);
                                    datos.Add(reg.estilo.descripcion);
                                    int total_temporal_piezas = 0;
                                    total_piezas = 0;
                                    foreach (Cantidades cantidad in reg.lista_cantidades)
                                    {
                                        datos.Add((cantidad.total).ToString());
                                        total_piezas += cantidad.total;
                                    }
                                    datos.Add(total_piezas.ToString());
                                    datos.Add((total_piezas / 6).ToString());
                                    datos.Add("BATCH");
                                    celdas_estilos.Add(datos.ToArray());
                                    ws.Cell(row, 1).Value = celdas_estilos;
                                    ws.Range(row, 1, row, 12).Style.Font.Bold = true;
                                    ws.Range(row, 1, row, 12).Style.Fill.BackgroundColor = XLColor.FromArgb(red, green, blue);
                                    ws.Range(row, 1, row, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.LeftBorderColor = XLColor.Black;
                                    ws.Range(row, 1, row, 12).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.RightBorderColor = XLColor.Black;
                                    ws.Range(row, 1, row, 12).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.TopBorderColor = XLColor.Black;
                                    ws.Range(row, 1, row, 12).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.BottomBorderColor = XLColor.Black;
                                    row++;
                                    var headers_vacio = new List<String[]>();
                                    headers_vacio.Add(new String[] { " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " ", " " });
                                    ws.Cell(row, 1).Value = headers_vacio;
                                    ws.Range(row, 1, row, 12).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 255);
                                    ws.Range(row, 1, row, 12).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.LeftBorderColor = XLColor.Black;
                                    ws.Range(row, 1, row, 12).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.RightBorderColor = XLColor.Black;
                                    ws.Range(row, 1, row, 12).Style.Border.TopBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.TopBorderColor = XLColor.Black;
                                    ws.Range(row, 1, row, 12).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                                    ws.Range(row, 1, row, 12).Style.Border.BottomBorderColor = XLColor.Black;
                                    row++;
                                    registros++;
                                }//si es el mismo
                            }
                        }
                    }
                    ws.Rows().AdjustToContents();
                    ws.Columns().AdjustToContents();
                    //ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    /***********D*O*W*N*L*O*A*D*************************************************************************************************************************************************************************/
                    HttpResponse httpResponse = System.Web.HttpContext.Current.Response;
                    httpResponse.Clear();
                    httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    httpResponse.AddHeader("content-disposition", "attachment;filename=\"Batch " + Convert.ToString(Session["batch"]) + ".xlsx\"");
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
        public JsonResult datos_tabla_inicio(string cliente){            
            return Json(df.obtener_registros_inicio(Convert.ToInt32(cliente)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_movimientos_estilo(string estilo){            
            return Json(df.obtener_registros_estilo(Convert.ToInt32(estilo)), JsonRequestBehavior.AllowGet);
        }
























    }
}