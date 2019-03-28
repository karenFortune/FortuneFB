using FortuneSystem.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers.Catalogos
{
    public class TipoCamisetaController : Controller
    {
        // GET: TipoCamiseta
        CatTipoCamisetaData objCamiseta = new CatTipoCamisetaData();
        public ActionResult Index()
        {
            List<CatTipoCamiseta> listaCamiseta = new List<CatTipoCamiseta>();
            listaCamiseta = objCamiseta.ListaTipoCamiseta().ToList();
            return View(listaCamiseta);
        }

        [HttpGet]
        public ActionResult CrearCamiseta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCamiseta([Bind] CatTipoCamiseta camisetas)
        {
            if (ModelState.IsValid)
            {
                objCamiseta.AgregarCamiseta(camisetas);
                TempData["camisetaOK"] = "The shirt type was registered correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["camisetaError"] = "The shirt type can not be registered, try it later.";
            }
            return View(camisetas);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTipoCamiseta camisetas = objCamiseta.ConsultarListaCamisetas(id);
            if (camisetas == null)
            {
                return View();
            }
            return View(camisetas);
        }

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTipoCamiseta camisetas = objCamiseta.ConsultarListaCamisetas(id);
            if (camisetas == null)
            {
                return View();
            }

            return View(camisetas);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, [Bind] CatTipoCamiseta camisetas)
        {
            if (id != camisetas.IdTipo)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                objCamiseta.ActualizarCamisetas(camisetas);
                TempData["camisetaEditar"] = "The shirt type was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["camisetaEditarError"] = "The shirt type could not be modified, try it later.";
            }
            return View(camisetas);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatTipoCamiseta camisetas = objCamiseta.ConsultarListaCamisetas(id);


            if (camisetas == null)
            {
                return View();
            }
            return View(camisetas);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objCamiseta.EliminarCamisetas(id);
            TempData["camisetaEliminar"] = "The shirt type was removed correctly.";
            return RedirectToAction("Index");
        }
    }
}