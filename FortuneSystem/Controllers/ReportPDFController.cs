using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using FortuneSystem.Models.Pedidos;
using Rotativa.AspNetCore;
using Rotativa;

namespace FortuneSystem.Controllers
{
    public class ReportPDFController : Controller
    {

        string filename, footer_alineacion, footer_size, vista;

       public ActionResult transfer_ticket()
        {
            int salida = Convert.ToInt32(Session["id_transfer_ticket"]);
            //return View("transfer_ticket", dt.lista_transfer_ticket(salida));              
            filename = Convert.ToString(Session["nombre_pdf"]) + ".pdf";
            return new Rotativa.ViewAsPdf("transfer_ticket", filename)
            {
                FileName = filename,
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(15, 10, 15, 10),
                CustomSwitches = "--page-offset 0 --footer-right [page]/[toPage] --footer-font-size 9 ",
            };
        }
    }
}