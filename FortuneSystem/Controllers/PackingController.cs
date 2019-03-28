using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Packing;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.POSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class PackingController : Controller
    {
        PedidosData objPedido = new PedidosData();
        CatClienteData objCliente = new CatClienteData();
        CatClienteFinalData objClienteFinal = new CatClienteFinalData();
        CatTallaItemData objTalla = new CatTallaItemData();
        PackingData objPacking = new PackingData();
        ItemTallaData objTallas = new ItemTallaData();
        DescripcionItemData objSummary = new DescripcionItemData();
        // GET: Packing
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
            PackingM packing = new PackingM();
            PackingTypeSize packSize = new PackingTypeSize();
            OrdenesCompra pedido = objPedido.ConsultarListaPO(id);
            pedido.Packing = packing;
            pedido.Packing.PackingTypeSize = packSize;
            pedido.ListItems = objPedido.ListaItemEstilosPorIdPedido(id).ToList();
            Session["id_Pedido"] = id;
            pedido.CatCliente = objCliente.ConsultarListaClientes(pedido.Cliente);
            pedido.CatClienteFinal = objClienteFinal.ConsultarListaClientesFinal(pedido.ClienteFinal);
            if(pedido.NombreClienteFinal == null)
            {
                pedido.NombreClienteFinal = "";
            }                       
            pedido.NombreClienteFinal = pedido.CatClienteFinal.NombreCliente.TrimEnd();

            ListasEstilos(pedido, id);
            ListaPackingRegistrados(pedido, id);

            pedido.IdPedido = Convert.ToInt32(id);
            int cont = 0;
            Session["id_Block"] = objPacking.ObtenerNumBlock(pedido.ListItems);
            int numIdBlock = Convert.ToInt32(Session["id_Block"])+1;
            string namePack= "PACK-" + numIdBlock;
            string nameAssort = "ASMT-" + numIdBlock;
            pedido.Packing.PackingTypeSize.PackingName = namePack;
            pedido.Packing.PackingTypeSize.AssortName = nameAssort;
            //Session[""] = objPacking.ObtenerNumBlock();

            foreach (var item in pedido.ListItems)
            {
                
              List<PackingTypeSize> estilo = objPacking.BuscarPackingTypeSizePorEstilo(item.IdSummary);
            
                /*if(estilo.Count != 0)
                {
                    cont++;
                }*/
            }           

            pedido.HistorialPacking = cont;

            if (pedido == null)
            {
                return View();
            }
            return View(pedido);
        }

        public void ListasEstilos(OrdenesCompra pedido, int? id)
        {
            List<ItemDescripcion> listaEstilos = pedido.ListItems;
            listaEstilos = objPedido.ListaEstilosPorIdPedido(id).ToList();
            /*List<ItemDescripcion> listaEstilos = new List<ItemDescripcion>();
            foreach (var item in listEstilos)
            {
                List<PackingTypeSize> estilo = objPacking.BuscarPackingTypeSizePorEstilo(item.IdSummary);
                if (estilo.Count == 0)
                {
                    listaEstilos.Add(item);
                }
            }*/
            ViewBag.listEstilo = new SelectList(listaEstilos, "IdSummary", "ItemEstilo", pedido.IdEstilo);           
        }

        public void ListaPackingRegistrados(OrdenesCompra pedido, int? id)
        {
            List<PackingTypeSize> listPacking = pedido.ListPack;
            List<ItemDescripcion> listaEstilos = objPedido.ListaEstilosPorIdPedido(id).ToList();
            listPacking = objPacking.ListaPackingName(listaEstilos);
          
            ViewBag.listPack = new SelectList(listPacking, "PackingRegistrado", "PackingRegistrado", pedido.Packing.PackingTypeSize.PackingRegistrado);

        }

        public JsonResult ListadoPackingRegistrados(OrdenesCompra pedido, int? id)
        {
            PackingM packing = new PackingM();
            PackingTypeSize packSize = new PackingTypeSize();
            List<PackingTypeSize> listPacking = pedido.ListPack;
            List<ItemDescripcion> listaEstilos = objPedido.ListaEstilosPorIdPedido(id).ToList();
            listPacking = objPacking.ListaPackingName(listaEstilos);
            /*List<ItemDescripcion> listaEstilos = new List<ItemDescripcion>();
            foreach (var item in listEstilos)
            {
                List<PackingTypeSize> estilo = objPacking.BuscarPackingTypeSizePorEstilo(item.IdSummary);
                if (estilo.Count == 0)
                {
                    listaEstilos.Add(item);
                }

            }*/
            //ViewBag.listEstilo = new SelectList(listaEstilos, "IdSummary", "ItemEstilo", pedido.IdEstilo);
            var result = Json(new { listEstilo = listPacking });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListadoEstilos(OrdenesCompra pedido, int? id)
        {
            PackingM packing = new PackingM();
            PackingTypeSize packSize = new PackingTypeSize();
            List<ItemDescripcion> listaEstilos = pedido.ListItems;
            listaEstilos = objPedido.ListaEstilosPorIdPedido(id).ToList();
            Session["id_Block"] = objPacking.ObtenerNumBlock(listaEstilos);
            int NumBlock = Convert.ToInt32(Session["id_Block"])+1;
            string datosBlock = "PACK-" + NumBlock;      
            string nameAssort = "ASMT-" + NumBlock;
            /*List<ItemDescripcion> listaEstilos = new List<ItemDescripcion>();
            foreach (var item in listEstilos)
            {
                List<PackingTypeSize> estilo = objPacking.BuscarPackingTypeSizePorEstilo(item.IdSummary);
                if (estilo.Count == 0)
                {
                    listaEstilos.Add(item);
                }

            }*/
            //ViewBag.listEstilo = new SelectList(listaEstilos, "IdSummary", "ItemEstilo", pedido.IdEstilo);
            var result = Json(new { listEstilo = listaEstilos, Block = datosBlock, AssortBlock = nameAssort });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing(List<string> ListTalla, int EstiloID)
        {
            PackingM tallaItem = new PackingM();
            PackingSize packingSize = new PackingSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingSize.IdSummary = EstiloID;
            //int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> calidad = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }
            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla= tallas[v];
                packingSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string calidadT = calidad[v];
                if (calidadT == "")
                {
                    calidadT = "0";
                }
                packingSize.Calidad = Int32.Parse(calidadT);
                tallaItem.PackingSize = packingSize;

                objPacking.AgregarTallasP(tallaItem);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Bulk(List<string> ListTalla, int EstiloID, string TipoPackID)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = Int32.Parse(TipoPackID);
            packingTSize.PackingName = "";
            packingTSize.AssortName = "";
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> piezas = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }
            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string piezasT = piezas[v];
                if (piezasT == "")
                {
                    piezasT = "0";
                }
                packingTSize.Pieces = Int32.Parse(piezasT);
                tallaItem.PackingTypeSize = packingTSize;                

                objPacking.AgregarTallasTypePack(tallaItem);
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_PPK(List<string> ListTalla, int EstiloID, string TipoPackID)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = Int32.Parse(TipoPackID);
            packingTSize.PackingName = "";
            packingTSize.AssortName = "";
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> ratio = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string ratioT = ratio[v];
                if (ratioT == "")
                {
                    ratioT = "0";
                }
                packingTSize.Ratio = Int32.Parse(ratioT);
                tallaItem.PackingTypeSize = packingTSize;


                objPacking.AgregarTallasTypePack(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Assort(List<string> ListTalla, int EstiloID, string PackingName, string AssortName, int NumQty, int NumCart, int TotalUnidades)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = 3;
            packingTSize.PackingName = PackingName;
            packingTSize.AssortName = AssortName;
            packingTSize.Pieces = NumQty;
            packingTSize.TotalCartones = NumCart;
            packingTSize.TotalUnits = TotalUnidades;
            packingTSize.IdBlockPack = Convert.ToInt32(Session["id_Block"])+ 1;
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> ratio = ListTalla[1].Split('*').ToList();
            List<string> piezas = ListTalla[2].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string ratioT = ratio[v];
                if (ratioT == "")
                {
                    ratioT = "0";
                }
                packingTSize.Ratio = Int32.Parse(ratioT);

                string piezasT = piezas[v];
                if (piezasT == "")
                {
                    piezasT = "0";
                }
                packingTSize.TotalPieces = Int32.Parse(piezasT);
                tallaItem.PackingTypeSize = packingTSize;


                objPacking.AgregarTallasTypePack(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Pallet(List<string> ListTalla, int EstiloID, int TipoTurnoID, int NumCaja, string TipoEmpaque)
        {
            PackingM tallaItem = new PackingM();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            tallaItem.IdSummary = EstiloID;
            tallaItem.IdTurno = TipoTurnoID;//Int32.Parse(TipoTurnoID);           
            int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            tallaItem.IdBatch = numBatch + 1;
            List<string> idPack = ListTalla[0].Split('*').ToList();
            List<string> tallas = ListTalla[1].Split('*').ToList();
            List<string> cajas = new List<string>();
            List<string> piezas = new List<string>(); 
            List<string> totales = new List<string>();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }
            i = i - 1;
            string talla;
            string idPacking;
            if (TipoEmpaque == "BULK")
            {
                
                cajas = ListTalla[2].Split('*').ToList();
                piezas = ListTalla[3].Split('*').ToList();
                totales = ListTalla[4].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    idPacking = idPack[v];
                    tallaItem.IdPackingTypeSize= Int32.Parse(idPacking);
                    string cantBox = cajas[v];
                    if (cantBox == "")
                    {
                        cantBox = "0";
                    }
                    tallaItem.CantBox = Int32.Parse(cantBox);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);


                    objPacking.AgregarTallasPacking(tallaItem);


                }
            } else if(TipoEmpaque == "PPK")
            {
                int nBox = NumCaja;
                piezas = ListTalla[2].Split('*').ToList();
                totales = ListTalla[3].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    tallaItem.CantBox = NumCaja;
                    idPacking = idPack[v];
                    tallaItem.IdPackingTypeSize = Int32.Parse(idPacking);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);              


                objPacking.AgregarTallasPacking(tallaItem);


                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Assort_Pallet(int TipoTurnoID, int NumCartones, int numTotalP, string packName, int numBlock, int idPedido)
        {
            PackingM tallaItem = new PackingM();
            PackingAssortment packAssort = new PackingAssortment();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            packAssort.Usuario = noEmpleado;           
            packAssort.IdTurno = TipoTurnoID;
            packAssort.PackingName = packName;
            packAssort.CantCartons = NumCartones;
            packAssort.TotalPiezas = numTotalP;
            packAssort.IdBlock = numBlock;
            packAssort.IdPedido = idPedido;
            //Int32.Parse(TipoTurnoID);           
             int numBatch = objPacking.ObtenerIdBatchAssort(idPedido, numBlock);
             packAssort.IdBatch = numBatch + 1;

            tallaItem.PackingAssort = packAssort;

             objPacking.AgregarPackingAssort(tallaItem);


            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_HT_Pallet(List<string> ListTalla, int EstiloID, int TipoTurnoID, int NumeroPO, string TipoEmpaque, int NumeroCaja, int NumeroPPK)
        {
            PackingM tallaItem = new PackingM();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            tallaItem.IdSummary = EstiloID;
            tallaItem.IdTurno = TipoTurnoID;//Int32.Parse(TipoTurnoID);           
            int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            tallaItem.IdBatch = numBatch + 1;  
            // List<string> idPack = ListTalla[0].Split('*').ToList();
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> cajas = new List<string>();
            List<string> piezas = new List<string>();
            List<string> totales = new List<string>();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }
            i = i - 1;
            string talla;
           
            
            if (TipoEmpaque == "BULK")
            {

                cajas = ListTalla[1].Split('*').ToList();
               // piezas = ListTalla[3].Split('*').ToList();
                //totales = ListTalla[4].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    tallaItem.IdPackingTypeSize = objPacking.ObtenerIdPackingtypeSize(1, tallaItem.IdSummary, tallaItem.IdTalla, NumeroPO);
                    //int caja= objPacking.ObtenerNoCajasBulkHT(tallaItem.IdSummary, tallaItem.IdTalla, NumeroPO, 1);
                    
                    string cantBox = cajas[v];
                    if (cantBox == "")
                    {
                        cantBox = "0";
                    }
                    tallaItem.CantBox = Int32.Parse(cantBox);
                    /*if (caja > 0)
                    {
                        tallaItem.CantBox = 0;
                    }*/
                    
                    objPacking.AgregarTallasPacking(tallaItem);


                }
            }
            else if (TipoEmpaque == "PPK")
            {
                
             //   piezas = ListTalla[2].Split('*').ToList();
                totales = ListTalla[2].Split('*').ToList();
                for (int v = 0; v < i-1; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    tallaItem.CantBox = NumeroCaja;
                    tallaItem.CantidadPPKS = NumeroPPK;
                    tallaItem.IdPackingTypeSize = objPacking.ObtenerIdPackingtypeSize(2, tallaItem.IdSummary, tallaItem.IdTalla, NumeroPO);
                    //idPacking = idPack[v];
                    // tallaItem.IdPackingTypeSize = Int32.Parse(idPacking);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);


                    objPacking.AgregarTallasPacking(tallaItem);


                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_Bulk_HT(List<string> ListTalla, int EstiloID, string FormaPackID, int NumberPOID)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = 1;
            packingTSize.IdFormaEmpaque= Int32.Parse(FormaPackID);
            packingTSize.NumberPO = NumberPOID;
            packingTSize.PackingName = "";
            packingTSize.AssortName = "";
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> cantidad = ListTalla[1].Split('*').ToList();
            List<string> cartones = new List<string>();
            List<string> partiales = new List<string>();
            List<string> totalCartones = new List<string>();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string cantidadT = cantidad[v];
                if (cantidadT == "")
                {
                    cantidadT = "0";
                }
                packingTSize.Cantidad = Int32.Parse(cantidadT);
                cartones = ListTalla[2].Split('*').ToList();
                string cartonesT = cartones[v];
                if(cartonesT == "")
                {
                    cartonesT = "0";
                }
                packingTSize.Cartones = Int32.Parse(cartonesT);
                partiales = ListTalla[3].Split('*').ToList();
                string parcialesT = partiales[v];
                if (parcialesT == "")
                {
                    parcialesT = "0";
                }
                packingTSize.PartialNumber = Int32.Parse(parcialesT);
                totalCartones = ListTalla[4].Split('*').ToList();
                string cartonesTotal = totalCartones[v];
                if (cartonesTotal == "")
                {
                    cartonesTotal = "0";
                }
                packingTSize.TotalCartones = Int32.Parse(cartonesTotal);
                tallaItem.PackingTypeSize = packingTSize;

                objPacking.AgregarTallasTypePack(tallaItem);

            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_Packing_PPK_HT(List<string> ListTalla, int EstiloID, int NumberPOID, int NumberTotU)
        {
            PackingM tallaItem = new PackingM();
            PackingTypeSize packingTSize = new PackingTypeSize();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.Usuario = noEmpleado;
            packingTSize.IdSummary = EstiloID;
            packingTSize.IdTipoEmpaque = 2;
            packingTSize.NumberPO = NumberPOID;
            packingTSize.TotalUnits = NumberTotU;
            packingTSize.PackingName = "";
            packingTSize.AssortName = "";
            // int numBatch = objPacking.ObtenerIdBatch(EstiloID);
            // tallaItem.IdBatch = numBatch + 1;
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> ratio = ListTalla[1].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            string talla;
            for (int v = 0; v < i; v++)
            {
                talla = tallas[v];
                packingTSize.IdTalla = objTalla.ObtenerIdTalla(talla);
                string ratioT = ratio[v];
                if (ratioT == "")
                {
                    ratioT = "0";
                }
                packingTSize.Ratio = Int32.Parse(ratioT);
                tallaItem.PackingTypeSize = packingTSize;


                objPacking.AgregarTallasTypePack(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Actualizar_Lista_Tallas_Batch(List<string> ListTalla, int TipoTurnoID, int EstiloID, int IdBatch, int NumCaja, string TipoEmpaque)
        {
            PackingM tallaItem = new PackingM();
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            tallaItem.UsuarioModif = noEmpleado;
            tallaItem.Usuario = noEmpleado;
            tallaItem.IdSummary = EstiloID;
            tallaItem.IdTurno = TipoTurnoID;//Int32.Parse(TipoTurnoID);           
            tallaItem.IdBatch = IdBatch;
            List<string> idPack = ListTalla[0].Split('*').ToList();
            List<string> tallas = ListTalla[1].Split('*').ToList();
            List<string> cajas = new List<string>();
            List<string> piezas = new List<string>();
            List<string> totales = new List<string>();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }
            i = i - 1;
            string talla;
            string idPacking;
            if (TipoEmpaque == "BULK")
            {
                cajas = ListTalla[2].Split('*').ToList();
                piezas = ListTalla[3].Split('*').ToList();
                totales = ListTalla[4].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    idPacking = idPack[v];
                    tallaItem.IdPacking = Int32.Parse(idPacking);
                    tallaItem.IdPackingTypeSize = objPacking.ObtenerIdPackingSize(tallaItem.IdPacking);
                    tallaItem.Usuario = objPacking.ObtenerIdUsuarioPorBatchEstilo(tallaItem.IdBatch, tallaItem.IdSummary, tallaItem.IdTalla);
                    string cantBox = cajas[v];
                    if (cantBox == "")
                    {
                        cantBox = "0";
                    }
                    tallaItem.CantBox = Int32.Parse(cantBox);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);


                    objPacking.ActualizarTallasPacking(tallaItem);


                }
            }
            else if (TipoEmpaque == "PPK")
            {
                int nBox = NumCaja;
                piezas = ListTalla[2].Split('*').ToList();
                totales = ListTalla[3].Split('*').ToList();
                for (int v = 0; v < i; v++)
                {
                    talla = tallas[v];
                    tallaItem.IdTalla = objTalla.ObtenerIdTalla(talla);
                    tallaItem.CantBox = NumCaja;
                    idPacking = idPack[v];
                    tallaItem.IdPacking = Int32.Parse(idPacking);
                    tallaItem.IdPackingTypeSize = objPacking.ObtenerIdPackingSize(tallaItem.IdPacking);
                    tallaItem.Usuario = objPacking.ObtenerIdUsuarioPorBatchEstilo(tallaItem.IdBatch, tallaItem.IdSummary, tallaItem.IdTalla);
                    string totalP = totales[v];
                    if (totalP == "")
                    {
                        totalP = "0";
                    }
                    tallaItem.TotalPiezas = Int32.Parse(totalP);


                    objPacking.ActualizarTallasPacking(tallaItem);


                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Lista_Batch_Estilo(int? id, int tipoEmpaque)
        {
            List<PackingM> listaBatch = objPacking.ListaBatch(id, tipoEmpaque).ToList();
            int cargo = Convert.ToInt32(Session["idCargo"]);
            var result = Json(new { listaPO = listaBatch, cargoUser = cargo });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Batch_Assort(int numBlock, int idPedido)
        {

            List<PackingAssortment> listaBatch = objPacking.ListaBatchAssort(numBlock, idPedido).ToList();

            var result = Json(new { listaTalla = listaBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Batch_HT_Estilo(int? id)
        {
            List<PackingM> listaBatch = objPacking.ListaBatchHT(id).ToList();

            var result = Json(new { listaPO = listaBatch });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Packing_IdEstilo_IdBatch(int? idEstilo, int idBatch)
        {
            List<PackingM> listaCantidadesTallasBatch = objPacking.ListaCantidadesTallaPorIdBatchEstilo(idEstilo, idBatch).ToList();
            List<int> listaTallasTBatch = objPacking.ListaTotalTallasPackingBatchEstilo(idEstilo).ToList();
            List<PackingTypeSize> listaTallasEmpaque = objPacking.ObtenerCajasPackingPorEstilo(idEstilo, idBatch);
            var result = Json(new { listaTalla = listaCantidadesTallasBatch, listaPrint = listaTallasTBatch, listaEmpaqueTallas = listaTallasEmpaque, });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        

        [HttpPost]
        public JsonResult Lista_Tallas_Packing_PPK_IdEstilo_IdBatch(int? idEstilo, int idBatch)
        {
            List<PackingM> listaCantidadesTallasBatch = objPacking.ListaCantidadesTallaPorIdBatchEstilo(idEstilo, idBatch).ToList();
            List<int> listaTallasTBatch = objPacking.ListaTotalTallasPackingBatchEstilo(idEstilo).ToList();
            List<PackingTypeSize> listaTallasEmpaque = objPacking.ObtenerCajasPackingPPKPorEstilo(idEstilo, idBatch);
            var result = Json(new { listaTalla = listaCantidadesTallasBatch, listaPrint = listaTallasTBatch, listaEmpaqueTallas = listaTallasEmpaque, });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Lista_Tallas_Batch(int? id)
        {
            List<PackingM> listaTallas = objPacking.ListaTallasBatchId(id).ToList();

            var result = Json(new { listaPO = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Por_Estilo(int? id)
        {
            List<PackingM> listaTallasEstilo = objPacking.ObtenerTallas(id).ToList();
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            List<int> listaCajasPacking = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();
            List<PackingSize> listaTallasPacking = objPacking.ObtenerListaPackingSizePorEstilo(id).ToList();
            List<PackingTypeSize> listaTallasEmpaque = objPacking.ObtenerListaPackingTypeSizePorEstilo(id).ToList();
            List<PackingTypeSize> listaTotalPiezasTallas = objPacking.ListaTotalPiezasTallasAssortPorEstilo(id).ToList();
            List<ItemTalla> listCantidadesTallas = objTallas.ListaCantidadesTallasPorEstilo(id).ToList();
            List<int> listaTallasCBatch = new List<int>();
            if (listaCajasPacking.Count != 0)
            {
                listaTallasCBatch = objPacking.ListaTotalCajasTallasBatchEstilo(id).ToList();
            }
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }
            int idPedido = Convert.ToInt32(Session["id_Pedido"]);
            int totalPiezasEstilos = objSummary.ObtenerPiezasTotalesEstilos(idPedido);
            int totalPiezasPack = objSummary.ObtenerPiezasTotalesPorPackAssort(id);
            int cargo = Convert.ToInt32(Session["idCargo"]);
            var result = Json(new { lista=listaTallas, listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking,
                                    listaEmpaqueTallas = listaTallasEmpaque, listaTotalCajasPack = listaCajasPacking,
                                    listaCajasT = listaTallasCBatch, estilos = estilo, cargoUser = cargo,
                                    numTPSyle = totalPiezasEstilos, numTPack = totalPiezasPack,
                                    listaTotalPiezas = listaTotalPiezasTallas, listCantTalla = listCantidadesTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Empaque_Por_Estilo(int? id)
        {
            List<PackingSize> listaTallasEstilo = objPacking.ListaTallasCalidadPack(id).ToList();
            List<PackingTypeSize> listaTallasPacking = objPacking.ObtenerListaPackingTypeSizePorEstilo(id).ToList();
            var result = Json(new { listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Empaque_HT_Por_Estilo(int? estiloId, int nPO, int tEmpaque)
        {
            List<ItemTalla> listaTallasPO = objTallas.ListaTallasPorEstilo(estiloId).ToList();
            List<PackingSize> listaTallasEstilo = objPacking.ListaTallasCalidadPack(estiloId).ToList();
            List<PackingTypeSize> listaTallasPacking = objPacking.ObtenerListaPackingTypeHTPorEstilo(estiloId, nPO, tEmpaque).ToList();
            List<PackingTypeSize> listaTallasPackingBox = objPacking.ObtenerListaPackingTypeBulkHTBox(estiloId, nPO, tEmpaque).ToList();

            
            var result = Json(new { lista = listaTallasPO, listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking, listaPackingBox = listaTallasPackingBox });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Empaque_HT_Registrar_Por_Estilo(int? idEst)
        {
            List<ItemTalla> listaTallasPO = objTallas.ListaTallasPorEstilo(idEst).ToList();
          
            var result = Json(new { lista = listaTallasPO});
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Empaque_HT_PPK_Por_Estilo(int? estiloId, int nPO, int tEmpaque)
        {
            List<ItemTalla> listaTallasPO = objTallas.ListaTallasPorEstilo(estiloId).ToList();
            List<PackingSize> listaTallasEstilo = objPacking.ListaTallasCalidadPack(estiloId).ToList();
            List<PackingTypeSize> listaTallasPacking = objPacking.ObtenerListaPackingTypeHTPorEstilo(estiloId, nPO, tEmpaque).ToList();
            List<PackingTypeSize> listaTallasPackingBox = objPacking.ObtenerListaPackingTypeBulkHTBox(estiloId, nPO, tEmpaque).ToList();


            var result = Json(new { lista = listaTallasPO, listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking, listaPackingBox = listaTallasPackingBox });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Estilos_Empaque_Assort(string packingName)
        {
            int idPedido = Convert.ToInt32(Session["id_Pedido"]);
            List<PackingTypeSize> listaEstilos = objPacking.ListadoPackingPorIdEstilo(idPedido, packingName);
            int numBlock = objPacking.ObtenerIdBlock(idPedido, packingName);
            int numTotalCart = objPacking.ObtenerTotalCartonesAssort(idPedido, packingName);
            int numTotalPiezas = objPacking.ObtenerTotalPiezasAssort(idPedido, packingName);          
            int tCartonesFalt = objPacking.ObtenerTotalCartonesFaltantesAssort(idPedido, numBlock, numTotalCart);
            int tPiezasFalt = objPacking.ObtenerTotalPiezasFaltantesAssort(idPedido, numBlock, numTotalPiezas);
            var result = Json(new {listaPackingBox = listaEstilos, numTotalCartonesFalt = tCartonesFalt, numTotalPiezasFat= tPiezasFalt });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Listado_Packing_Assort()
        {
            int idPedido = Convert.ToInt32(Session["id_Pedido"]);
            int numRegistros= objPacking.ListadoPackingTypeAssort(idPedido);
            int cargo = Convert.ToInt32(Session["idCargo"]);
            int totalPiezasEstilos = objSummary.ObtenerPiezasTotalesEstilos(idPedido);
            int totalPiezasPack = objSummary.ObtenerPiezasTotalesPorPack(idPedido);
            var result = Json(new {totalRegistros = numRegistros, cargoUser = cargo, numTPSyle = totalPiezasEstilos, numTPack = totalPiezasPack });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Empaque_Assort(int? estiloId)
        {
           
            List<ItemTalla> listaTallasPO = objTallas.ListaTallasPorEstilo(estiloId).ToList();
            List<ItemTalla> listCantidadesTallas = objTallas.ListaCantidadesTallasPorEstilo(estiloId).ToList();
            List<PackingSize> listaTallasPacking = objPacking.ObtenerListaPackingSizePorEstilo(estiloId).ToList();
            /*List<PackingSize> listaTallasEstilo = objPacking.ListaTallasCalidadPack(estiloId).ToList();
            List<PackingTypeSize> listaTallasPacking = objPacking.ObtenerListaPackingTypeHTPorEstilo(estiloId, nPO, tEmpaque).ToList();
            */
            //List<ItemDescripcion> listaTallasPackingBox = objPedido.ListaItemEstilosPorIdPedido(idEstilo).ToList();
            //List<PackingTypeSize> listaTallasPacking = objPacking.BuscarPackingTypeSizePorEstilo(estiloId);

            var result = Json(new { lista = listaTallasPO, listCantTalla = listCantidadesTallas, listaPackingS = listaTallasPacking/*, listaTalla = listaTallasEstilo, listaPackingS = listaTallasPacking, listaPackingBox = listaTallasPackingBox */});
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_HT_Por_Estilo(int? id)
        {
            List<PackingM> listaTallasEstilo = objPacking.ObtenerTallas(id).ToList();
            List<ItemTalla> listaTallas = objTallas.ListaTallasPorEstilo(id).ToList();
            List<int> listaPiezasPackingBulk = objPacking.ListaTallasPackingBulkEstilo(id).ToList();
            List<int> listaPiezasPackingPPK = objPacking.ListaTallasPackingPPKEstilo(id).ToList();
            //List<PackingSize> listaTallasPacking = objPacking.ObtenerListaPackingSizePorEstilo(id).ToList();
            //List<PackingTypeSize> listaTallasEmpaquePPK = objPacking.ObtenerListaPackingPPKPorEstilo(id).ToList();
            List<PackingTypeSize> listaTallasEmpaquePPK = objPacking.ObtenerListaPackingPPK(id).ToList();
            List<int> listaTallasCBatch = new List<int>();
            if (listaPiezasPackingBulk.Count != 0)
            {
                listaTallasCBatch = objPacking.ListaTotalCajasTallasBatchBulkHTEstilo(id).ToList();
            }
            string estilo = "";
            foreach (var item in listaTallas)
            {
                estilo = item.Estilo;

            }

            var result = Json(new
            {
                lista = listaTallas,
                //listaTalla = listaTallasEstilo,
                //listaPackingS = listaTallasPacking,
                estilos = estilo,
                listaPTBulk = listaPiezasPackingBulk,
                listaPTPPK = listaPiezasPackingPPK,
                listaEmpPPK = listaTallasEmpaquePPK,
                listaTotalCajasPack = listaTallasCBatch
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Lista_Tallas_Assort_Por_Estilo(int? idPedido, int numBlock, string namePack )
        {
            List<PackingM> listaTallasPO = objPacking.ObtenerTallasAssort(idPedido, namePack).ToList();
            List<ItemTalla> listaTallas = objTallas.ListaTallasAssortPorEstilo(idPedido, namePack).ToList();
            List<PackingTypeSize> listaRatios = objPacking.ListaRatiosPackAssort(idPedido, namePack, numBlock).ToList();
            int numTotalCartones = objPacking.ObtenerCantCartonesAssort(idPedido, numBlock);
             var result = Json(new
            {
                lista = listaTallas,
                listaPO = listaTallasPO,
                listRatio = listaRatios,
                numCartones = numTotalCartones
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPost]
        public JsonResult Lista_Total_Tallas_Batch(int id)
        {
            List<int> listaTallas = objPacking.ListaTotalTallasPackingBatchEstilo(id).ToList();

            var result = Json(new { listaTallaBatch = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ActualizarPackingAssort(string qty, string cart, string totalUnits, string packName)
        {
            int numCant = Int32.Parse(qty);
            int numCart = Int32.Parse(cart);
            int numTU = Int32.Parse(totalUnits);
            objPacking.ActualizarCantidadesPackAssort(numCant, numCart, numTU, packName);

            return Json("0", JsonRequestBehavior.AllowGet);
        } 

    }
}