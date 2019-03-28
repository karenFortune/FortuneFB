using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Roles;
using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class UsuariosController : Controller
    {
        CatUsuarioData objCatUser = new CatUsuarioData();
        CatRolesData objCaRoles = new CatRolesData();
        CatUsuario usuario = new CatUsuario();
        CatSucursalData objSucursal = new CatSucursalData();
        // GET: Usuarios
        public ActionResult Index()
        {
            //  Lista de Roles
            List<CatUsuario> listaUsuarios = new List<CatUsuario>();
            listaUsuarios = objCatUser.ListaUsuarios().ToList();

            return View(listaUsuarios);
        }

        public void ListaSucursal(CatUsuario suc)
        {
            List<CatSucursal> listaSucursal = suc.ListaSucursal;
            listaSucursal = objSucursal.ListaSucursales().ToList();

            ViewBag.listSucursal = new SelectList(listaSucursal, "IdSucursal", "Sucursal", suc.IdSucursal);

        }

        [HttpGet]
        public ActionResult CrearUsuario()
        {
            CatUsuario usuario = new CatUsuario();

            List<CatRoles> listaRoles = usuario.ListaRoles;
            listaRoles = objCaRoles.ListaRoles().ToList();
            usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
            usuario.CatRoles.Id = usuario.Cargo;
            ViewBag.listRoles = new SelectList(listaRoles, "Id", "rol", usuario.Cargo);

            List<CatSucursal> listaSucursal = usuario.ListaSucursal;
            listaSucursal = objSucursal.ListaSucursales().ToList();
            usuario.CatSucursal = objSucursal.ConsultarListaSucursal(usuario.IdSucursal);
            usuario.CatSucursal.IdSucursal = usuario.IdSucursal;
            ViewBag.listSucursal = new SelectList(listaSucursal, "IdSucursal", "Sucursal", usuario.IdSucursal);


            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearUsuario([Bind] CatUsuario usuario)
        {
            string rol = Request.Form["listRoles"].ToString();
            usuario.Cargo = Int32.Parse(rol);
            string suc = Request.Form["listSucursal"].ToString();
            usuario.IdSucursal = Int32.Parse(suc);
            usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
            usuario.CatSucursal = objSucursal.ConsultarListaSucursal(usuario.IdSucursal);
            usuario.Turnos.ToString();
            /* if (ModelState.IsValid)
             { */
            string infTurno = usuario.Turnos.ToString();

            if (infTurno == "First")
            {
                usuario.TipoTurno = 1;
            }
            else
            {
                usuario.TipoTurno = 2;
            }

            objCatUser.AgregarUsuarios(usuario);
            TempData["usuarioOK"] = "The user was registered correctly.";
            return RedirectToAction("Index");
            /* }else
             {
                 TempData["usuarioError"] = "The user can not be registered, try it later.";
             }*/
            // return View(usuario);
        }


        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatUsuario usuario = objCatUser.ConsultarListaUsuarios(id);
            usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
            usuario.CatSucursal = objSucursal.ConsultarListaSucursal(usuario.IdSucursal);


            usuario.Turnos = (Turno)usuario.TipoTurno;
            /*if (usuario.TipoTurno == 1)
            {
                usuario.Turnos.ToString();
            }
            else
            {
                usuario.Turnos.ToString();
            }*/

            if (usuario == null)
            {
                return View();
            }
            return View(usuario);


        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View();
            }


            CatUsuario usuario = objCatUser.ConsultarListaUsuarios(id);
            List<CatRoles> listaRoles = usuario.ListaRoles;
            listaRoles = objCaRoles.ListaRoles().ToList();
            usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
            usuario.CatRoles.Id = usuario.Cargo;
            ViewBag.listRoles = new SelectList(listaRoles, "Id", "rol", usuario.Cargo);

            List<CatSucursal> listaSucursal = usuario.ListaSucursal;
            listaSucursal = objSucursal.ListaSucursales().ToList();
            usuario.CatSucursal = objSucursal.ConsultarListaSucursal(usuario.IdSucursal);
            usuario.CatSucursal.IdSucursal = usuario.IdSucursal;
            ViewBag.listSucursal = new SelectList(listaSucursal, "IdSucursal", "Sucursal", usuario.IdSucursal);

            usuario.Turnos = (Turno)usuario.TipoTurno;

            if (usuario == null)
            {

                return View();
            }

            return View(usuario);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind] CatUsuario usuarios)
        {
            if (id != usuarios.Id)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                string rol = Request.Form["Rol"].ToString();
                usuarios.Cargo = Int32.Parse(rol);
                string sucursal = Request.Form["Sucursal"].ToString();
                usuarios.IdSucursal = Int32.Parse(sucursal);
                usuarios.Turnos.ToString();
                //usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
                string infTurno = usuarios.Turnos.ToString();

                if (infTurno == "First")
                {
                    usuarios.TipoTurno = 1;
                }
                else
                {
                    usuarios.TipoTurno = 2;
                }
                objCatUser.ActualizarUsuarios(usuarios);
                TempData["usuarioEditar"] = "The user was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["usuarioEditarError"] = "The user could not be modified, try it later.";
            }
            return View(usuarios);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatUsuario usuarios = objCatUser.ConsultarListaUsuarios(id);
            usuarios.CatRoles = objCaRoles.ConsultarListaRoles(usuarios.Cargo);
            usuarios.CatSucursal = objSucursal.ConsultarListaSucursal(usuarios.IdSucursal);
            usuarios.Turnos = (Turno)usuarios.TipoTurno;
            if (usuarios == null)
            {
                return View();
            }
            return View(usuarios);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objCatUser.EliminarUsuario(id);
            TempData["usuarioEliminar"] = "The user was successfully deleted.";
            return RedirectToAction("Index");
        }

    }
}