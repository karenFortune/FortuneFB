using FortuneSystem.Models.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers.Catalogos
{
    public class ClienteController : Controller
    {
        // GET: Cliente
        CatClienteData objCliente = new CatClienteData();
        public ActionResult Index()
        {
            List<CatCliente> listaClientes = new List<CatCliente>();
            listaClientes = objCliente.ListaClientes().ToList();
            return View(listaClientes);
        }

        [HttpGet]
        public ActionResult CrearCliente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCliente([Bind] CatCliente clientes)
        {
            if (ModelState.IsValid)
            {
                objCliente.AgregarClientes(clientes);
                TempData["clienteOK"] = "The customer was registered correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["clienteError"] = "The customer can not be registered, try it later.";
            }
            return View(clientes);
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatCliente clientes = objCliente.ConsultarListaClientes(id);
            if (clientes == null)
            {
                return View();
            }
            return View(clientes);
        }

        [HttpGet]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatCliente clientes = objCliente.ConsultarListaClientes(id);
            if (clientes == null)
            {
                return View();
            }

            return View(clientes);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, [Bind] CatCliente clientes)
        {
            if (id != clientes.Customer)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                objCliente.ActualizarCliente(clientes);
                TempData["clienteEditar"] = "The customer was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["clienteEditarError"] = "The client could not be modified, try it later.";
            }
            return View(clientes);
        }

        [HttpGet]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return View();
            }

            CatCliente clientes = objCliente.ConsultarListaClientes(id);

            if (clientes == null)
            {
                return View();
            }
            return View(clientes);

        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public ActionResult ConfimacionEliminar(int? id)
        {
            objCliente.EliminarCliente(id);
            TempData["clienteEliminar"] = "The customer was successfully deleted.";
            return RedirectToAction("Index");
        }



    }
}