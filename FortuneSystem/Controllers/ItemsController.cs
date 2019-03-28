using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.POSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class ItemsController : Controller
    {
        DescripcionItemData objItems = new DescripcionItemData();
        CatGeneroData objGenero = new CatGeneroData();
        ItemDescripcionData objEstilo = new ItemDescripcionData();
        // GET: Items
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CrearItems()
        {
            POSummary summary = new POSummary();
            ListaGenero(summary);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearItems([Bind] POSummary descItem)
        {
            if (ModelState.IsValid)
            {
                objItems.AgregarItems(descItem);
                return RedirectToAction("CrearItems");
            }
            return View(descItem);
        }

        public void ListaGenero(POSummary summary)
        {
            List<CatGenero> listaGenero = summary.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();

            ViewBag.listGenero = new SelectList(listaGenero, "IdGender", "Genero", summary.IdGenero);

        }

        [HttpPost]
        public ActionResult RegistrarEstilo([Bind] ItemDescripcion estilo, string ItemEstilo, string DescEstilo)
        {

            estilo.ItemEstilo = ItemEstilo;
            estilo.Descripcion = DescEstilo;
            objEstilo.AgregarItemDescripcion(estilo);
            return RedirectToAction("Index");


        }




    }
}