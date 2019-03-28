using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.PNL;
using FortuneSystem.Models.PrintShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PNLController : Controller
    {
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        CatTallaItemData objTalla = new CatTallaItemData();
        PnlData objPnl = new PnlData();
        PrintShopData objPrint = new PrintShopData();
        // GET: PNL
        public ActionResult Index()
        {
            List<OrdenesCompra> listaPedidos = new List<OrdenesCompra>();
            listaPedidos = objPedido.ListaOrdenCompra().ToList();
            return View(listaPedidos);
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

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Pnl(List<string> ListTalla, int TurnoID, int EstiloID, int MaquinaID, string StatusID, string Comentarios)
        {
            Pnl tallaItem = new Pnl();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            tallaItem.TipoTurno = TurnoID;
            tallaItem.IdSummary = EstiloID;
            tallaItem.Maquina = MaquinaID;
            tallaItem.Comentarios = Comentarios;
            if (StatusID != null)
            {
                tallaItem.EstadoPallet = Convert.ToBoolean(StatusID);
            }
            int numBatch = objPnl.ObtenerIdBatch(EstiloID);
            tallaItem.IdBatch = numBatch + 1;
            int i = 1;
            foreach (var item in ListTalla)
            {
                i++;
            }

            i = i - 2;
            for (int v = 0; v < i; v++)
            {
                List<string> tallas = ListTalla[v].Split('*').ToList();
                string talla;
                string printed;
                string misPrint;
                string defecto;
                string repair;
                int c = 0;


                foreach (var item in tallas)
                {
                    if (c == 0)
                    {
                        talla = tallas[0];
                        printed = tallas[1];
                        misPrint = tallas[2];
                        defecto = tallas[3];
                        repair = tallas[4];
                        tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                        tallaItem.Printed = Int32.Parse(printed);
                        tallaItem.MisPrint = Int32.Parse(misPrint);
                        tallaItem.Defect = Int32.Parse(defecto);
                        tallaItem.Repair = Int32.Parse(repair);


                        objPnl.AgregarTallasPnl(tallaItem);

                    }
                    c++;
                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Actualizar_Lista_Tallas_Batch(List<string> ListTalla, int TurnoID, int EstiloID, int IdBatch, int MaquinaID, string StatusID, string Comentarios)
        {
            Pnl tallaItem = new Pnl();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.UsuarioModif = noEmpleado;
            tallaItem.TipoTurno = TurnoID;
            tallaItem.IdSummary = EstiloID;
            tallaItem.IdBatch = IdBatch;
            tallaItem.Maquina = MaquinaID;
            tallaItem.Comentarios = Comentarios;
            if (StatusID != null)
            {
                tallaItem.EstadoPallet = Convert.ToBoolean(StatusID);
            }
            int i = 1;
            foreach (var item in ListTalla)
            {
                i++;
            }

            i = i - 2;
            for (int v = 0; v < i; v++)
            {
                List<string> tallas = ListTalla[v].Split('*').ToList();
                string talla;
                string printed;
                string misPrint;
                string defecto;
                string repair;
                int c = 0;


                foreach (var item in tallas)
                {
                    if (c == 0)
                    {
                        talla = tallas[0];
                        printed = tallas[1];
                        misPrint = tallas[2];
                        defecto = tallas[3];
                        repair = tallas[4];
                        tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                        tallaItem.Printed = Int32.Parse(printed);
                        tallaItem.MisPrint = Int32.Parse(misPrint);
                        tallaItem.Defect = Int32.Parse(defecto);
                        tallaItem.Repair = Int32.Parse(repair);
                        tallaItem.IdPnl = objPnl.ObtenerIdPnlPorBatchEstilo(tallaItem.IdBatch, tallaItem.IdSummary, tallaItem.IdTalla);
                        tallaItem.Usuario = objPnl.ObtenerIdUsuarioPorBatchPNLEstilo(tallaItem.IdBatch, tallaItem.IdSummary, tallaItem.IdTalla);
                        objPnl.ActualizarTallasPnl(tallaItem);

                    }
                    c++;
                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Lista_Batch_Estilo(int? id)
        {
            List<Pnl> listaBatch = objPnl.ListaBatch(id).ToList();

            var result = Json(new { listaTalla = listaBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Lista_Tallas_Pnl_IdEstilo_IdBatch(int? idEstilo, int idBatch)
        {
            List<Pnl> listaCantidadesTallasBatch = objPnl.ListaCantidadesTallaPorIdBatchEstilo(idEstilo, idBatch).ToList();
            List<int> listaTallasTBatch = objPnl.ListaTotalTallasPNLBatchEstilo(idEstilo).ToList();
            var result = Json(new { listaTalla = listaCantidadesTallasBatch, listaPrint = listaTallasTBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Batch(int? id)
        {
            List<Pnl> listaTallas = objPnl.ListaTallasBatchId(id).ToList();

            var result = Json(new { listaTalla = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Por_Estilo(int? id)
        {
            List<Pnl> listaTallasEstilo = objPnl.ObtenerTallas(id).ToList();

            var result = Json(new { listaTalla = listaTallasEstilo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Total_Tallas_Batch(int id)
        {
            List<int> listaTallas = objPnl.ListaTotalTallasPNLBatchEstilo(id).ToList();

            var result = Json(new { listaTallaBatch = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}