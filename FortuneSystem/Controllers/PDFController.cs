using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rotativa;
using System.Globalization;

using FortuneSystem.Models.Almacen;
using FortuneSystem.Models.Staging;
using FortuneSystem.Models.Shipping;
using FortuneSystem.Models.Trims;
using FortuneSystem.Models.POSummary;

namespace FortuneSystem.Controllers
{
    public class PDFController : Controller{

        DatosInventario di = new DatosInventario();
        DatosTransferencias dt = new DatosTransferencias();
        DatosTrim dtr = new DatosTrim();
        DatosStaging ds = new DatosStaging();
        DatosShipping dsh = new DatosShipping();
        StagingGeneral sg = new StagingGeneral();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DescripcionItemData de = new DescripcionItemData();
        string filename, footer_alineacion, footer_size,vista;
        private object _service;

        public ActionResult Index(){
            int id_usuario = Convert.ToInt32(Session["id_Empleado"]);
            Session["id_usuario"] = id_usuario;
            Session["turno_stag"] = consultas.obtener_turno_usuario(id_usuario);
            return View();
        }            

        public ActionResult imprimir_etiquetas_recibos(){
            int recibo = Convert.ToInt32(Session["id_recibo_nuevo"]);
            //return View("etiquetas_cajas_recibo", di.lista_recibo_etiqueta(recibo.ToString()));            
            /*return new ViewAsPdf("etiquetas_cajas_recibo", di.lista_recibo_etiqueta(recibo.ToString()))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,               
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--page-offset 0 ",
                PageHeight=40,
                PageWidth=120,
            };*/
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys){
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            return new ViewAsPdf("etiquetas_cajas_recibo", di.lista_recibo_etiqueta(recibo.ToString())){
                FileName = "Etiquetas" + Convert.ToString(recibo) + ".pdf",
                Cookies = cookieCollection,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(5, 5, 5, 5),
                CustomSwitches = "--page-offset 0 ",
                PageHeight = 40,
                PageWidth = 120,
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName
            };
        }
        [AllowAnonymous]
        public ActionResult transfer_ticket(){
            int salida = Convert.ToInt32(Session["id_transfer_ticket"]);
            //return View("transfer_ticket", dt.lista_transfer_ticket(salida));              
            int tipo = dt.buscar_tipo_salida(salida);
            if (tipo == 0){
                /*return new ViewAsPdf("transfer_ticket", dt.lista_transfer_ticket(salida)){
                    FileName = filename,
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                    CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                };*/
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
                foreach (var key in Request.Cookies.AllKeys){
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                return new ViewAsPdf("transfer_ticket", dt.lista_transfer_ticket(salida)){
                    FileName = "Transfer" + Convert.ToString(Session["id_transfer_ticket"]) + ".pdf",
                    Cookies = cookieCollection,
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                    CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                    FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName
                };
            }
            else {
                //return View("transfer_ticket_contratista", dt.lista_transfer_ticket(salida));
                /*return new ViewAsPdf("transfer_ticket_contratista", dt.lista_transfer_ticket(salida)){
                    FileName = filename,
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                    CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                };*/
                Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
                foreach (var key in Request.Cookies.AllKeys){
                    cookieCollection.Add(key, Request.Cookies.Get(key).Value);
                }
                return new ViewAsPdf("transfer_ticket_contratista", dt.lista_transfer_ticket(salida)){
                    FileName = "Transfer" + Convert.ToString(Session["id_transfer_ticket"]) + ".pdf",
                    Cookies = cookieCollection,
                    PageOrientation = Rotativa.Options.Orientation.Landscape,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                    CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                    FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName
                };
            }
        }
       
        [AllowAnonymous]
        public ActionResult papeleta_staging_vacias(){
            int salida = Convert.ToInt32(Session["id_transfer_ticket"]);            
            //ViewBag.color = sg.obtener_color_item(Convert.ToInt32(Session["id_inventario"]));
            //ViewBag.pais = sg.obtener_pais_item(Convert.ToInt32(Session["id_inventario"]));
            /*return new ViewAsPdf("papeleta_staging_vacias", ds.lista_papeleta(Convert.ToInt32(Session["id_inventario"]), Convert.ToInt32(Session["turno"])))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };*/
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys){
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            //return new ViewAsPdf("papeleta_staging_vacias", ds.lista_papeleta(Convert.ToInt32(Session["id_inventario"]), Convert.ToInt32(Session["turno"]))){
            return new ViewAsPdf("papeleta_staging_vacias", ds.lista_papeleta(Convert.ToInt32(Session["id_estilo"]), Convert.ToInt32(Session["id_pedido"]),Convert.ToInt32(Session["turno"]))){
                FileName = "PAPELETA" + Convert.ToString(Convert.ToInt32(Session["id_inventario"])) + ".pdf",
                Cookies = cookieCollection,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName
            };
        }
        //papeleta_staging
        [AllowAnonymous]
        public ActionResult papeleta_staging(){
            /*return new ViewAsPdf("papeleta_staging", ds.lista_papeleta_staging(Convert.ToInt32(Session["id_staging"]), Convert.ToInt32(Session["turno"])))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };*/
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys){
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            return new ViewAsPdf("papeleta_staging", ds.lista_papeleta_staging(Convert.ToInt32(Session["id_staging"]), Convert.ToInt32(Session["turno"]))){
                FileName = "PAPELETA" + Convert.ToString(Convert.ToInt32(Session["id_staging"])) + ".pdf",
                Cookies = cookieCollection,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName
            };
        }
        //*************************
        [AllowAnonymous]
        public ActionResult imprimir_pk(){
            //return View("packing_list", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"])));
            return new ViewAsPdf("packing_list", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"]))){
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(8, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };
        }
        //*************************
        [AllowAnonymous]
        public ActionResult imprimir_bol(){
            //return View("bol", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"])));
            /* return new ViewAsPdf("bol", dsh.obtener_packing_list(Convert.ToInt32(Session["pk"]))){
                 FileName = filename,
                 PageOrientation = Rotativa.Options.Orientation.Portrait,
                 PageSize = Rotativa.Options.Size.Letter,
                 PageMargins = new Rotativa.Options.Margins(8, 10, 15, 10),
                 CustomSwitches = "--page-offset 0 --print-media-type ",
             };*/
            Dictionary<string, string> cookieCollection = new Dictionary<string, string>();
            foreach (var key in Request.Cookies.AllKeys)
            {
                cookieCollection.Add(key, Request.Cookies.Get(key).Value);
            }
            return new ViewAsPdf("bol", dsh.obtener_packing_list_bol(Convert.ToInt32(Session["pk"])))
            {
                FileName = "BOL-PK"+ Convert.ToString(Session["pk"])+".pdf",
                Cookies = cookieCollection,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                FormsAuthenticationCookieName = System.Web.Security.FormsAuthentication.FormsCookieName
            };
           /* var byteArray = abc.BuildPdf(ControllerContext);
            var fileStream = new System.IO.FileStream(Server.MapPath(subPath) + "/abc.pdf", FileMode.Create, FileAccess.Write);
            fileStream.Write(byteArray, 0, byteArray.Length);
            fileStream.Close();*/
        }

        [AllowAnonymous]
        public ActionResult Imprimir_Reporte_PO()
        {
            int id = Convert.ToInt32(Session["idPed"]);
            return new ViewAsPdf("Imprimir_Reporte_PO", de.ListadoInfEstilo(id))
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(5, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };

        }











    }
}