using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.POSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers.Catalogos
{
    public class TallasController : Controller
    {
        // GET: Tallas
        CatTallaItemData objTalla = new CatTallaItemData();
        public ActionResult Index()
        {
            List<CatTallaItem> listaTalla = new List<CatTallaItem>();
            listaTalla = objTalla.ListaTallas().ToList();
            return View(listaTalla);
        }

        [HttpGet]
        public ActionResult CrearTalla()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTalla([Bind] CatTallaItem tallas)
        {
            if (ModelState.IsValid)
            {
                objTalla.AgregarTallas(tallas);
                TempData["tallaOK"] = "The size was registered correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["tallaError"] = "The size can not be registered, try it later.";
            }
            return View(tallas);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTallaItem tallas = objTalla.ConsultarListaTallas(id);
            if (tallas == null)
            {
                return View();
            }
            return View(tallas);
        }

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTallaItem tallas = objTalla.ConsultarListaTallas(id);
            if (tallas == null)
            {
                return View();
            }

            return View(tallas);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, [Bind] CatTallaItem tallas)
        {
            if (id != tallas.Id)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                objTalla.ActualizarTallas(tallas);
                TempData["tallaEditar"] = "The size was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["tallaEditarError"] = "The size could not be modified, try it later.";
            }
            return View(tallas);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTallaItem tallas = objTalla.ConsultarListaTallas(id);


            if (tallas == null)
            {
                return View();
            }
            return View(tallas);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objTalla.EliminarTallas(id);
            TempData["tallaEliminar"] = "The size was removed correctly.";
            return RedirectToAction("Index");
        }

        public ActionResult Lista_Tallas()
        {
            POSummary summary = new POSummary();
            List<CatTallaItem> listaTallas = summary.ListaTallas;
            listaTallas = objTalla.ListaTallas().ToList();
            summary.ListaTallas = listaTallas;
            return Json(listaTallas, JsonRequestBehavior.AllowGet);
        }

      


    }
}