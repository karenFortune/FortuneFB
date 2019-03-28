using FortuneSystem.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers.Catalogos
{
    public class CatStatusController : Controller
    {
        CatStatusData objEstados = new CatStatusData();
        // GET: CatStatus
        public ActionResult Index()
        {
            List<CatStatus> listaEstados = new List<CatStatus>();
            listaEstados = objEstados.ListarEstados().ToList();
            return View(listaEstados);
        }
    }
}