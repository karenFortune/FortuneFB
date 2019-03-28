using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Packing;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.PNL;
using FortuneSystem.Models.POSummary;
using FortuneSystem.Models.PrintShop;
using FortuneSystem.Models.Revisiones;
using FortuneSystem.Models.Staging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PedidosController : Controller
    {
        // GET: Pedido
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        CatStatusData objEstados = new CatStatusData();
        CatGeneroData objGenero = new CatGeneroData();
        CatColoresData objColores = new CatColoresData();
        DescripcionItemData objItems = new DescripcionItemData();
        CatTelaData objTela = new CatTelaData();
        CatTipoCamisetaData objTipoC = new CatTipoCamisetaData();
        ItemTallaData objTallas = new ItemTallaData();
        RevisionesData objRevision = new RevisionesData();
        ItemDescripcionData objEst = new ItemDescripcionData();
        PrintShopData objPrint = new PrintShopData();
        PackingData objPacking = new PackingData();
        PnlData objPnl = new PnlData();
        PDFController pdf = new PDFController();
        CatEspecialidadesData objEspecialidad = new CatEspecialidadesData();
        CatTipoOrdenData objTipoOrden = new CatTipoOrdenData();

        public int estado;
        public int IdPO;
        public int pedidos;

        public ActionResult Index()
        {
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompra().ToList();
            return View(listaPedidos);
        }

        public void Reporte(int? id)
        {
            Session["idPed"] = id;
            pdf.Imprimir_Reporte_PO();
        }

        [HttpPost]
        public JsonResult Imprimir_Reporte_PO(int id)
        {
            //pedido        
            Session["idPed"] = id;

            return Json("0", JsonRequestBehavior.AllowGet);
        }

        /*  [ChildActionOnly]
          public ActionResult StudentList()
          {
              lista = objPedido.ListaOrdenCompra().ToList();
              return PartialView(lista);
          }*/

        /* public ActionResult Lista_Pedido_Por_Fecha(DateTime? fechaCancel, DateTime? fechaOrden)
         {
             List<OrdenesCompra> lista = objPedido.ListaOrdenCompra(fechaCancel, fechaOrden).ToList();
             return PartialView(lista);
         }*/

        [HttpPost]
        public JsonResult Lista_Estilos_PO(int? id)
        {

            List<POSummary> listaItems = objItems.ListaItemsPorPO(id).ToList();

            var result = Json(new { listaItem = listaItems });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult Listado_Estilos_PO()
        {
            int? id = Convert.ToInt32(Convert.ToInt32(Session["idPedidoNuevo"])); //id_pedido

            List<POSummary> listaItems = objItems.ListaItemsPorPO(id).ToList();

            return PartialView(listaItems);
        }

        [ChildActionOnly]
        public ActionResult Listado_Tallas_Estilo(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();

            return PartialView(listaTallas);
        }

        public ActionResult HistorialPedidos(int id)
        {
            List<OrdenesCompra> listaPedidosRev = new List<OrdenesCompra>();
            listaPedidosRev = objPedido.ListaRevisionesPO(id).ToList();

            return View(listaPedidosRev);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Listado_Tallas_Estilos(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListadoTallasPorEstilos(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_PrintShop(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstiloPrint(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<StagingDatos> listaDatosStaging = objTallas.ListaTallasStagingDatosPorEstilo(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listDatosStaging = listaDatosStaging,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_Det(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstiloRev(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasPBatch = new List<int>();
            List<int> listaTallasMPBatch = new List<int>();
            List<int> listaTallasDBatch = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTBatch.Count != 0)
            {
                listaTallasPBatch = objPrint.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatch = objPrint.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatch = objPrint.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPrint.ListaTotalRepTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatch = listaTallasPBatch,
                listaTallasTotalMBatch = listaTallasMPBatch,
                listaTallasTotalDBatch = listaTallasDBatch,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_Pnl(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilopnl(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<StagingDatos> listaDatosStaging = objTallas.ListaTallasStagingDatosPorEstilo(id).ToList();
            List<int> listaTallasTPnlBatch = objPnl.ListaTotalTallasPNLBatchEstilo(id).ToList();
            List<int> listaTallasPBatchPnl = new List<int>();
            List<int> listaTallasMPBatchPnl = new List<int>();
            List<int> listaTallasDBatchPnl = new List<int>();
            List<int> listaTallasRBatch = new List<int>();
            if (listaTallasTPnlBatch.Count != 0)
            {
                listaTallasPBatchPnl = objPnl.ListaTotalPrintedTallasBatchEstilo(id).ToList();
                listaTallasMPBatchPnl = objPnl.ListaTotalMPTallasBatchEstilo(id).ToList();
                listaTallasDBatchPnl = objPnl.ListaTotalDefTallasBatchEstilo(id).ToList();
                listaTallasRBatch = objPnl.ListaTotalRepTallasBatchEstilo(id).ToList();
            }
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listDatosStaging = listaDatosStaging,
                listaTallasTotalPnlBatch = listaTallasTPnlBatch,
                listaTallasTotalBatch = listaTallasTBatch,
                listaTallasTotalPBatchPNL = listaTallasPBatchPnl,
                listaTallasTotalMBatchPnl = listaTallasMPBatchPnl,
                listaTallasTotalDBatchPnl = listaTallasDBatchPnl,
                listaTallasTotalRBatch = listaTallasRBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Lista_Tallas_Estilo_Packing(int? id)
        {
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            List<StagingD> listaTallasStaging = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            List<int> listaTallasTPrintShopBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            List<int> listaTallasTPnlBatch = objPnl.ListaTotalTallasPNLBatchEstilo(id).ToList();
            List<int> listaTallasTPackingBatch = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();
            List<int> listaTallasPBatchPacking = new List<int>();
            List<int> listaTallasEBatchPacking = new List<int>();
            List<int> listaTallasDBatchPacking = new List<int>();
            if (listaTallasTPackingBatch.Count != 0)
            {
                listaTallasPBatchPacking = objPacking.ListaTotalCajasTallasBatchEstilo(id).ToList();
                listaTallasEBatchPacking = objPacking.ListaTotalETallasBatchEstilo(id).ToList();
                listaTallasDBatchPacking = objPacking.ListaTotalDefTallasBatchEstilo(id).ToList();
            }

            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            var result = Json(new
            {
                listaTalla = listaTallas,
                estilos = estilo,
                listTallaStaging = listaTallasStaging,
                listaTallasTotalPnlBatch = listaTallasTPnlBatch,
                listaTallasTotalBatch = listaTallasTPrintShopBatch,
                listaTallasTotalPackingBatch = listaTallasTPackingBatch,
                listaTallasTotalPBatchPacking = listaTallasPBatchPacking,
                listaTallasTotalEBatchPacking = listaTallasEBatchPacking,
                listaTallasTotalDBatchPacking = listaTallasDBatchPacking
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Staging_Estilo(int? id)
        {
            List<StagingD> listaTallas = objTallas.ListaTallasStagingPorEstilo(id).ToList();
            /*string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }*/
            var result = Json(new { listaTalla = listaTallas/*, estilos = estilo*/ });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_PrintShop_Estilo(int? id)
        {
            List<PrintShopC> listaTallas = objPrint.ListaTallasPrintShop(id).ToList();
            List<PrintShopC> listaTallasEstilo = objPrint.ObtenerTallas(id).ToList();
            List<int> listaTallasTBatch = objPrint.ListaTotalTallasBatchEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallas, listaEstiloTallas = listaTallasEstilo, listaPrint = listaTallasTBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Pnl_Estilo(int? id)
        {
            List<Pnl> listaTallas = objPnl.ListaTallasPnl(id).ToList();
            List<Pnl> listaTallasEstilo = objPnl.ObtenerTallas(id).ToList();
            List<int> listaTallasTBatch = objPnl.ListaTotalTallasPNLBatchEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallas, listaEstiloTallas = listaTallasEstilo, listaPrint = listaTallasTBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Packing_Estilo(int? id)
        {
            List<PackingM> listaTallas = objPacking.ListaTallasPacking(id).ToList();
            List<PackingM> listaTallasEstilo = objPacking.ObtenerTallas(id).ToList();
            List<int> listaTallasTBatch = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallas, listaEstiloTallas = listaTallasEstilo, listaPrint = listaTallasTBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Total_PrintShop_Estilo(int? id)
        {
            List<PrintShopC> listaTallas = objPrint.ListaTallasTotalPrintShop(id).ToList();
            /*string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }*/
            var result = Json(new { listaTalla = listaTallas/*, estilos = estilo*/ });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult CrearPO()
        {
            OrdenesCompra pedido = new OrdenesCompra();
            POSummary summary = new POSummary();
            ListasClientes(pedido);
            ListaEstados(pedido);
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            ListaEspecialidades(summary);
            ListaTipoOrden(pedido);
            return View();
        }
        [HttpPost]
        public ActionResult RegistrarPO([Bind] OrdenesCompra ordenCompra, string po, string VPO, DateTime FechaCancel, DateTime FechaOrden, int Cliente, int Clientefinal, int TotalUnidades, int IdTipoOrden)
        {
            ListaEstados(ordenCompra);
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            ordenCompra.Usuario = noEmpleado;
            objPedido.AgregarPO(ordenCompra);
            Session["idPedido"] = objPedido.Obtener_Utlimo_po();

            return View(ordenCompra);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearPO([Bind] OrdenesCompra ordenCompra)
        {
            if (ModelState.IsValid)
            {
                ObtenerIdClientes(ordenCompra);
                ListaEstados(ordenCompra);
                //objPedido.AgregarPO(pedido);
            }

            return View();
        }

        [HttpGet]
        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return View();
            }

            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.IdPedido = Convert.ToInt32(id);

            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }

        [HttpGet]
        public ActionResult DetallesRev(int? id)
        {
            if (id == null)
            {
                return View();
            }

            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.IdPedido = Convert.ToInt32(id);

            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }

        [HttpGet]
        public int ObtenerPORevision(int? id)
        {
            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            SeleccionarClientes(pedido);
            SeleccionarClienteFinal(pedido);
            /*int revisiones = objRevision.ObtenerNumeroRevisiones(id);
            int identificador = 0;
            string rev;
            if (revisiones == 0 && pedido.IdStatus != 3)
            {
                identificador = revisiones + 1;
                rev = pedido.PO + "-REV" + identificador;
            }
            else
            {
                identificador = revisiones + 1;
                rev = pedido.PO + "-REV" + identificador;
            }pedido.PO = rev.Replace(" ", "");*/


            // pedido.IdPedido = Convert.ToInt32(id);
            pedido.FechaOrden = DateTime.Today;
            Session["id_pedido"] = id;

            ObtenerEstadoRevisado(pedido);

            objPedido.AgregarPO(pedido);
            Session["idPedidoNuevo"] = objPedido.Obtener_Utlimo_po();
            //Cambia estado pedido original a 5
            objPedido.ActualizarEstadoPO(Convert.ToInt32(Session["id_pedido"]));

            //Registrar en Revisado el Pedido Nuevo 
            //int PedidosId = objPedido.Obtener_Utlimo_po();
            //Session["idPedidoNuevo"] = PedidosId;
            int PedidoNuevo = Convert.ToInt32(Session["idPedidoNuevo"]);
            if (PedidoNuevo != 0)
            {
                Revision revisionPO = new Revision()
                {
                    IdPedido = Convert.ToInt32(Session["id_pedido"]),
                    IdRevisionPO = Convert.ToInt32(Session["idPedidoNuevo"]),
                    FechaRevision = DateTime.Today,
                    IdStatus = pedido.IdStatus

                };
                objRevision.AgregarRevisionesPO(revisionPO);
            }
            //Obtener los estilos por ID Pedido Anterior
            List<POSummary> listaItems = objItems.ListaEstilosPorPO(Convert.ToInt32(Session["id_pedido"])).ToList();
            POSummary estilos = new POSummary();
            foreach (var item in listaItems)
            {
                estilos.EstiloItem = item.EstiloItem;
                estilos.IdColor = item.CatColores.CodigoColor;
                estilos.Cantidad = item.Cantidad;
                estilos.Precio = item.Precio;
                estilos.PedidosId = item.PedidosId;
                estilos.IdGenero = item.CatGenero.GeneroCode;
                estilos.IdTela = item.IdTela;
                estilos.TipoCamiseta = item.CatTipoCamiseta.TipoProducto;
                estilos.IdItems = item.IdItems;
                estilos.IdEspecialidad = item.CatEspecialidades.IdEspecialidad;
                Session["id_estilo"] = estilos.IdItems;
                int? idEstilo = Convert.ToInt32(Convert.ToInt32(Session["id_estilo"]));
                estilos.PedidosId = Convert.ToInt32(Session["idPedidoNuevo"]);
                objItems.AgregarItems(estilos);
                Session["estiloIdItem"] = objItems.Obtener_Utlimo_Item();
                //Obtener la lista de tallas del item
                List<ItemTalla> listaTallas = objTallas.ListaTallasPorSummary(idEstilo).ToList();
                ItemTalla tallas = new ItemTalla();

                foreach (var itemT in listaTallas)
                {

                    tallas.Talla = itemT.Talla;
                    tallas.Cantidad = itemT.Cantidad;
                    tallas.Extras = itemT.Extras;
                    tallas.Ejemplos = itemT.Ejemplos;
                    tallas.IdSummary = Convert.ToInt32(Session["estiloIdItem"]);

                    objTallas.RegistroTallas(tallas);
                }

            }
            return Convert.ToInt32(Session["idPedidoNuevo"]);
        }

        [HttpGet]
        public ActionResult Revision(int? id)
        {
            int idPedido = ObtenerPORevision(id);
            Session["idPedidoRevision"] = idPedido;
            POSummary summary = new POSummary();
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            if (id == null)
            {
                return View();
            }
            OrdenesCompra pedidos = objPedido.ConsultarListaPO(idPedido);
            ListasClientes(pedidos);


            /* if(id != null)
             {
                 RegistrarRevisionPO(pedidos);
             }   */


            if (pedidos == null)
            {
                return View();
            }

            return View(pedidos);

        }
        [HttpPost]
        public ActionResult RegistrarRevisionPO([Bind] OrdenesCompra pedido)
        {
            List<POSummary> listaItems = objItems.ListaItemsPorPO(pedido.IdPedido).ToList();


            // List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            return View(pedido);
        }

        public ActionResult CancelarPO(int id)
        {
            objPedido.ActualizarEstadoPOCancelado(id);
            TempData["cancelarPO"] = "The purchase order was canceled correctly.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revision(int id, [Bind] OrdenesCompra pedido)
        {
            string cliente = Request.Form["Nombre"].ToString();
            pedido.Cliente = Int32.Parse(cliente);

            string clienteFinal = Request.Form["NombreCliente"].ToString();
            pedido.ClienteFinal = Int32.Parse(clienteFinal);
            /*if (id != pedido.IdPedido)
            {
                return View();
            }*/
            /* if (ModelState.IsValid)
             {
                 objPedido.ActualizarPedidos(pedido);
                 TempData["pedidoRevision"] = "Se registro correctamente la revisión de la orden de compra .";
                 return RedirectToAction("Index");
             }
             else
             {
                 TempData["pedidoRevisionError"] = "No se pudo registrar la revisión de la orden de compra, intentelo más tarde.";
             }*/
            return View(pedido);
        }



        [HttpGet]
        public ActionResult EditarEstilo(int? id)
        {
            if (id == null)
            {
                return View();
            }


            POSummary items = objItems.ConsultarListaEstilos(id);
            ListaGenero(items);
            ListaTela(items);
            ListaTipoCamiseta(items);
            ListaEspecialidades(items);
            items.CatColores = objColores.ConsultarListaColores(items.ColorId);
            items.ItemDescripcion = objEst.ConsultarListaItemDesc(items.IdItems);
            items.CatEspecialidades = objEspecialidad.ConsultarListaEspecialidad(items.IdEspecialidad);
            items.PedidosId = items.PedidosId;
            SeleccionarGenero(items);
            SeleccionarTela(items);
            SeleccionarTipoCamiseta(items);
            SeleccionarTipoEspecialidad(items);


            if (items == null)
            {

                return View();
            }

            return PartialView(items);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEstilo(int id, [Bind] POSummary items)
        {
            items.IdItems = id;
            /* if (id != items.IdItems)
             {
                 return View();
             }*/
            string genero = Request.Form["Genero"].ToString();
            items.Id_Genero = objGenero.ObtenerIdGenero(genero);
            string tipoCamiseta = Request.Form["DescripcionTipo"].ToString();
            items.IdCamiseta = objTipoC.ObtenerIdTipoCamiseta(tipoCamiseta);
            string tela = Request.Form["Tela"].ToString();
            items.IdTela = Int32.Parse(tela);
            string estilo = items.ItemDescripcion.ItemEstilo;
            items.IdEstilo = objEst.ObtenerIdEstilo(estilo);
            string color = items.CatColores.CodigoColor;
            items.ColorId = objColores.ObtenerIdColor(color);
            if (items.IdItems != 0)
            {
                objItems.ActualizarEstilos(items);
                TempData["itemEditar"] = "The style was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["itemEditarError"] = "The style could not be modified, try it later.";
            }
            return View(items);
        }

        [HttpGet]
        public ActionResult EditarEstiloNuevo(int? id)
        {
            if (id == null)
            {
                return View();
            }

            POSummary items = objItems.ConsultarListaEstilos(id);
            ListaGenero(items);
            ListaTela(items);
            ListaTipoCamiseta(items);
            ListaEspecialidades(items);
            items.CatColores = objColores.ConsultarListaColores(items.ColorId);
            items.ItemDescripcion = objEst.ConsultarListaItemDesc(items.IdItems);
            items.CatEspecialidades = objEspecialidad.ConsultarListaEspecialidad(items.IdEspecialidad);
            items.PedidosId = items.PedidosId;
            SeleccionarGenero(items);
            SeleccionarTela(items);
            SeleccionarTipoCamiseta(items);
            SeleccionarTipoEspecialidad(items);

            if (items == null)
            {

                return View();
            }

            return PartialView(items);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEstiloNuevo(int id, [Bind] POSummary items)
        {
            items.IdItems = id;
            /* if (id != items.IdItems)
             {
                 return View();
             }*/
            string genero = Request.Form["Genero"].ToString();
            items.Id_Genero = objGenero.ObtenerIdGenero(genero);
            string tipoCamiseta = Request.Form["DescripcionTipo"].ToString();
            items.IdCamiseta = objTipoC.ObtenerIdTipoCamiseta(tipoCamiseta);
            string tela = Request.Form["Tela"].ToString();
            items.IdTela = Int32.Parse(tela);
            string estilo = items.ItemDescripcion.ItemEstilo;
            items.IdEstilo = objEst.ObtenerIdEstilo(estilo);
            string color = items.CatColores.CodigoColor;
            items.ColorId = objColores.ObtenerIdColor(color);
            if (items.IdItems != 0)
            {
                objItems.ActualizarEstilos(items);
                TempData["itemEditar"] = "The style was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["itemEditarError"] = "The style could not be modified, try it later.";
            }
            return View(items);
        }

        [HttpGet]
        public ActionResult EditarPO(int? id)
        {
            if (id == null)
            {
                return View();
            }

            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.FechaCancelada = String.Format("{0:MM/dd/yyyy}", pedido.FechaCancel);
            pedido.FechaOrdenFinal = String.Format("{0:MM/dd/yyyy}", pedido.FechaOrden);
            pedido.NombrePO = pedido.PO.TrimEnd(' ');
            pedido.PO = pedido.NombrePO;
            ListasClientes(pedido);
            ListaTipoOrden(pedido);
            pedido.IdEstilo = pedido.IdPedido;
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.CatTipoOrden = objTipoOrden.ConsultarListaTipoOrden(pedido.IdTipoOrden);

            if (pedido == null)
            {

                return View();
            }

            return View(pedido);

        }

        [HttpGet]
        public ActionResult ActualizarInfPO([Bind] OrdenesCompra pedido, int id, string po, string VPO, DateTime FechaCancel, DateTime FechaOrden, int Cliente, int Clientefinal, int TotalUnidades, int IdTipoOrden)
        {
            pedido.IdPedido = id;

            if (pedido.IdPedido != 0)
            {
                objPedido.ActualizarPedidos(pedido);
                TempData["itemEditar"] = "The purchase order was modified correctly.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["itemEditarError"] = "The purchase order could not be modified, try it later.";
            }
            return View(pedido);
        }


        [HttpPost]
        public ActionResult Eliminar(int? id)
        {
            objItems.EliminarTallasEstilo(id);
            objItems.EliminarEstilos(id);
            TempData["eliminarEstilo"] = "The style was removed correctly.";
            return View();
        }



        public void SeleccionarClientes(OrdenesCompra pedido)
        {
            List<CatCliente> listaClientes = pedido.LCliente;
            listaClientes = objCliente.ListaClientes().ToList();
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatCliente.Customer = pedido.Cliente;
            ViewBag.listCliente = new SelectList(listaClientes, "Customer", "Nombre", pedido.Cliente);


        }

        public void SeleccionarClienteFinal(OrdenesCompra pedido)
        {

            List<CatClienteFinal> listaClientesFinal = pedido.LClienteFinal;
            listaClientesFinal = objClienteFinal.ListaClientesFinal().ToList();
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            pedido.CatClienteFinal.CustomerFinal = pedido.ClienteFinal;
            ViewBag.listClienteFinal = new SelectList(listaClientesFinal, "CustomerFinal", "NombreCliente", pedido.ClienteFinal);

        }

        public void SeleccionarGenero(POSummary items)
        {

            List<CatGenero> listaGenero = items.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();
            items.CatGenero = objGenero.ConsultarListaGenero(items.Id_Genero);
            items.CatGenero.IdGender = items.Id_Genero;
            ViewBag.listGenero = new SelectList(listaGenero, "GeneroCode", "Genero", items.CatGenero.GeneroCode);

        }

        public void SeleccionarTela(POSummary items)
        {

            List<CatTela> listaTela = items.ListaTelas;
            listaTela = objTela.ListaTela().ToList();
            items.CatTela = objTela.ConsultarListaTelas(items.IdTela);
            items.CatTela.Id_Tela = items.IdTela;
            ViewBag.listTela = new SelectList(listaTela, "Id_Tela", "Tela", items.IdTela);
        }

        public void SeleccionarTipoCamiseta(POSummary items)
        {

            List<CatTipoCamiseta> listaTipoCamiseta = items.ListaTipoCamiseta;
            listaTipoCamiseta = objTipoC.ListaTipoCamiseta().ToList();
            items.CatTipoCamiseta = objTipoC.ConsultarListaCamisetas(items.IdCamiseta);
            items.CatTipoCamiseta.IdTipo = items.IdCamiseta;
            ViewBag.listTipoCamiseta = new SelectList(listaTipoCamiseta, "TipoProducto", "DescripcionTipo", items.CatTipoCamiseta.TipoProducto);
        }

        public void SeleccionarTipoEspecialidad(POSummary items)
        {

            List<CatEspecialidades> listaEspecialidades = items.ListaEspecialidades;
            listaEspecialidades = objEspecialidad.ListaEspecialidades().ToList();
            items.CatEspecialidades = objEspecialidad.ConsultarListaEspecialidad(items.IdEspecialidad);
            items.CatEspecialidades.IdEspecialidad = items.IdEspecialidad;
            ViewBag.listEspecialidad = new SelectList(listaEspecialidades, "IdEspecialidad", "Especialidad", items.IdEspecialidad);
        }

        public void ListasClientes(OrdenesCompra pedido)
        {
            List<CatCliente> listaClientes = pedido.ListaClientes;
            listaClientes = objCliente.ListaClientes().ToList();

            ViewBag.listCliente = new SelectList(listaClientes, "Customer", "Nombre", pedido.Cliente);

            List<CatClienteFinal> listaClientesFinal = pedido.ListaClientesFinal;
            listaClientesFinal = objClienteFinal.ListaClientesFinal().ToList();
            ViewBag.listClienteFinal = new SelectList(listaClientesFinal, "CustomerFinal", "NombreCliente", pedido.ClienteFinal);
        }

        public void ListaTipoOrden(OrdenesCompra pedido)
        {
            List<CatTipoOrden> listaTipoOrden = pedido.ListadoTipoOrden;
            listaTipoOrden = objTipoOrden.ListaTipoOrden().ToList();

            ViewBag.listTipoOrden = new SelectList(listaTipoOrden, "IdTipoOrden", "TipoOrden", pedido.IdTipoOrden);

        }

        public void ObtenerIdClientes(OrdenesCompra pedido)
        {
            string cliente = Request.Form["listCliente"].ToString();
            pedido.Cliente = Int32.Parse(cliente);
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);


            string clienteFinal = Request.Form["listClienteFinal"].ToString();
            pedido.ClienteFinal = Int32.Parse(clienteFinal);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);



        }

        public void ListaGenero(POSummary summary)
        {
            List<CatGenero> listaGenero = summary.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();

            ViewBag.listGenero = new SelectList(listaGenero, "GeneroCode", "Genero", summary.IdGenero);

        }

        public void ListaTela(POSummary summary)
        {
            List<CatTela> listaTela = summary.ListaTelas;
            listaTela = objTela.ListaTela().ToList();

            ViewBag.listTela = new SelectList(listaTela, "Id_Tela", "Tela", summary.IdTela);

        }

        public void ListaTipoCamiseta(POSummary summary)
        {
            List<CatTipoCamiseta> listaTipoCamiseta = summary.ListaTipoCamiseta;
            listaTipoCamiseta = objTipoC.ListaTipoCamiseta().ToList();

            ViewBag.listTipoCamiseta = new SelectList(listaTipoCamiseta, "TipoProducto", "DescripcionTipo", summary.TipoCamiseta);

        }

        public void ListaEspecialidades(POSummary summary)
        {
            List<CatEspecialidades> listaEspecialidades = summary.ListaEspecialidades;
            listaEspecialidades = objEspecialidad.ListaEspecialidades().ToList();

            ViewBag.listEspecialidad = new SelectList(listaEspecialidades, "IdEspecialidad", "Especialidad", summary.IdEspecialidad);

        }



        public void ListaEstados(OrdenesCompra pedido)
        {
            List<CatStatus> listaEstados = pedido.ListaCatStatus;
            listaEstados = objEstados.ListarEstados().ToList();

            ViewBag.listEstados = new SelectList(listaEstados, "IdStatus", "Estado", pedido.IdStatus);
            foreach (var item in listaEstados)
            {
                if (item.IdStatus == 1)
                {
                    pedido.IdStatus = item.IdStatus;
                }

            }

        }
        public void ObtenerEstadoRevisado(OrdenesCompra pedido)
        {
            List<CatStatus> listaEstados = pedido.ListaCatStatus;
            listaEstados = objEstados.ListarEstados().ToList();

            ViewBag.listEstados = new SelectList(listaEstados, "IdStatus", "Estado", pedido.IdStatus);
            foreach (var item in listaEstados)
            {
                if (item.IdStatus == 1)
                {
                    pedido.IdStatus = item.IdStatus;
                }
            }
        }

    }
}