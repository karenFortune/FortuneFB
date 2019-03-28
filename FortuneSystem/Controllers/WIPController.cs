using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class WIPController : Controller
    {
        PedidosData objPedido = new PedidosData();
        CatComentariosData objComent = new CatComentariosData();
        public ActionResult Index()
        {
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompraWIP().ToList();
            return View();
        }

        public JsonResult GetListadoPedido(string sidx, string sort, int page, int rows)
        {
            sort = (sort == null) ? "" : sort;
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompraWIP().ToList();
            int totalRecords = listaPedidos.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = listaPedidos
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult ListadoPedido()
        {

            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompraWIP().ToList();
            return Json(listaPedidos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListadoComentarios()
        {

            List<CatComentarios> listaComentarios = new List<CatComentarios>();
            listaComentarios = objComent.ListadoAllComentarios().ToList();

            return Json(listaComentarios, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void RegistrarCometarioWIP(string Comentario, int IdSummary)
        {
            DateTime fecha = DateTime.Now;
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            CatComentarios catComentario = new CatComentarios()
            {
                FechaComentario = fecha,
                IdSummary = IdSummary,
                Comentario = Comentario,
                IdUsuario = noEmpleado
            };


            objComent.AgregarComentario(catComentario);

        }
    }
}