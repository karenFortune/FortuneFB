using FortuneSystem.Models.Roles;
using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class RolesController : Controller
    {
        CatRolesData objCatRol = new CatRolesData();
        // GET: Roles
        public ActionResult Index()
        {
            //  Roles
            List<CatRoles> listaRoles = new List<CatRoles>();
            listaRoles = objCatRol.ListaRoles().ToList();
            //  ViewBag.UsuariosId = new SelectList(listaUsuarios, "Id", "Name", employee.ManagerId);
            return View(listaRoles);
        }



        [HttpGet]
        public ActionResult CrearRoles()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearRoles([Bind] CatRoles roles)
        {
            if (roles.Id == 0)
            {
                objCatRol.AgregarRoles(roles);
                TempData["rolesOK"] = "The role was registered correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["rolesError"] = "The role can not be registered, try it later.";
            }
            return View(roles);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatRoles roles = objCatRol.ConsultarListaRoles(id);
            if (roles == null)
            {
                return View();
            }
            return View(roles);


        }

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatRoles roles = objCatRol.ConsultarListaRoles(id);
            if (roles == null)
            {
                return View();
            }

            return View(roles);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, [Bind] CatRoles roles)
        {
            if (id != roles.Id)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                objCatRol.ActualizarRoles(roles);
                TempData["rolesEditar"] = "The role was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["rolesEditarError"] = "The role could not be modified, try later.";
            }
            return View(roles);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatRoles roles = objCatRol.ConsultarListaRoles(id);

            if (roles == null)
            {
                return View();
            }
            return View(roles);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objCatRol.EliminarRol(id);
            TempData["rolesEliminar"] = "The role was successfully deleted.";
            return RedirectToAction("Index");
        }

    }
}