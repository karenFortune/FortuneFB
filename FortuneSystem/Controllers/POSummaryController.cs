using FortuneSystem.Models.Arte;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.POSummary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace FortuneSystem.Controllers
{
    public class POSummaryController : Controller
    {
        DescripcionItemData objItems = new DescripcionItemData();
        CatColoresData objColores = new CatColoresData();
        CatGeneroData objGenero = new CatGeneroData();
        ItemDescripcionData objItemsDes = new ItemDescripcionData();
        PedidosData objPedido = new PedidosData();
        ItemTallaData objTalla = new ItemTallaData();
        CatTallaItemData objTallas = new CatTallaItemData();
        CatTelaData objTela = new CatTelaData();
        CatTipoCamisetaData objTipoC = new CatTipoCamisetaData();
        ArteData objArte = new ArteData();
        CatEspecialidadesData objEspecialidad = new CatEspecialidadesData();


        public int IdPedido;
        // GET: POSummary
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CrearItems()
        {

            // ListaGenero(summary);
            //pedidos.ObtenerListas();
            //string genero = "";
            //ListarTallasPorGenero(genero);
            POSummary summary = new POSummary();
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearItems([Bind] POSummary descItem)
        {
            if (ModelState.IsValid)
            {
                objItems.AgregarItems(descItem);
                return RedirectToAction("CrearItems");
            }
            return View(descItem);
        }

        [HttpGet]
        public ActionResult RegistrarNuevoEstilo()
        {

            POSummary summary = new POSummary();
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            ListaEspecialidades(summary);
            summary.PedidosId = Convert.ToInt32(Session["idPedidoRevision"]);

            if (summary == null)
            {

                return View();
            }

            return PartialView(summary);

        }

        [HttpGet]
        public ActionResult RegistrarNuevoEstiloPo()
        {
            POSummary summary = new POSummary();
            ListaGenero(summary);
            ListaTela(summary);
            ListaTipoCamiseta(summary);
            ListaEspecialidades(summary);
            //summary.PedidosId = IDPO;

            if (summary == null)
            {

                return View();
            }

            return PartialView(summary);

        }

        public void ListaGenero(POSummary summary)
        {
            List<CatGenero> listaGenero = summary.ListaGeneros;
            listaGenero = objGenero.ListaGeneros().ToList();
            summary.ListaGeneros = listaGenero;
            ViewBag.listGenero = new SelectList(listaGenero, "GeneroCode", "Genero", summary.IdGenero);

        }

        public void ListaTela(POSummary summary)
        {
            List<CatTela> listaTela = summary.ListaTelas;
            listaTela = objTela.ListaTela().ToList();
            summary.ListaTelas = listaTela;
            ViewBag.listTela = new SelectList(listaTela, "Id_Tela", "Tela", summary.IdTela);

        }

        public void ListaTipoCamiseta(POSummary summary)
        {
            List<CatTipoCamiseta> listaTipoCamiseta = summary.ListaTipoCamiseta;
            listaTipoCamiseta = objTipoC.ListaTipoCamiseta().ToList();
            summary.ListaTipoCamiseta = listaTipoCamiseta;
            ViewBag.listTipoCamiseta = new SelectList(listaTipoCamiseta, "TipoProducto", "DescripcionTipo", summary.TipoCamiseta);

        }

        public void ListaEspecialidades(POSummary summary)
        {
            List<CatEspecialidades> listaEspecialidades = summary.ListaEspecialidades;
            listaEspecialidades = objEspecialidad.ListaEspecialidades().ToList();
            ViewBag.listEspecialidad = new SelectList(listaEspecialidades, "IdEspecialidad", "Especialidad", summary.IdEspecialidad);

        }




        public void RegistrarArte(string EstiloItem)
        {
            IMAGEN_ARTE arte = new IMAGEN_ARTE();
            int idEstilo = objItemsDes.ObtenerIdEstilo(EstiloItem);
            int busquedaId = objArte.BuscarIdEstiloArteImagen(idEstilo);
            int IdItems = Convert.ToInt32(Session["IdItems"]);
            if (busquedaId == 0)
            {
                arte.StatusArte = 3;
                arte.StatusPNL = 4;
                //arte.extensionArte = "";
                //arte.extensionPNL = "";
                arte.IdEstilo = idEstilo;
                objArte.AgregarArteImagen(arte);
                arte = objArte.BuscarEstiloArteImagen(idEstilo);
                objArte.AgregarArte(arte.IdImgArte, IdItems);
            }
            else
            {
                arte = objArte.BuscarEstiloArteImagen(idEstilo);
                objArte.AgregarArte(arte.IdImgArte, IdItems);
            }
            Session["IdArte"] = arte.extensionArte;
        }

        public void RegistrarArtePnl(string EstiloItem, int idSummary)
        {
            IMAGEN_ARTE_PNL arte = new IMAGEN_ARTE_PNL();
            int idEstilo = objItemsDes.ObtenerIdEstilo(EstiloItem);
            int busquedaId = objArte.BuscarIdSummaryArtePnlImagen(idSummary);
            if (busquedaId == 0)
            {
                arte.StatusPNL = 4;
                arte.IdEstilo = idEstilo;
                arte.IdSummary = idSummary;
                objArte.AgregarArtePnlImagen(arte);
                arte = objArte.BuscarEstiloArtePnlImagen(idSummary);
                //objArte.AgregarArte(arte.IdImgArtePnl, IdItems);
            }
            else
            {
                arte = objArte.BuscarEstiloArtePnlImagen(idSummary);
                // objArte.AgregarArte(arte.IdImgArtePnl, IdItems);
            }
            Session["IdArtePnl"] = arte.ExtensionPNL;
        }
        //Agregar imagenes de arte al registrar los estilos de una orden
        public void RegistrarArteNuevo(string EstiloItem)
        {
            IMAGEN_ARTE arte = new IMAGEN_ARTE();
            int idEstilo = objItemsDes.ObtenerIdEstilo(EstiloItem);
            int busquedaId = objArte.BuscarIdEstiloArteImagen(idEstilo);
            int IdItems = Convert.ToInt32(Session["IdItemsNuevo"]);
            if (busquedaId == 0)
            {
                arte.StatusArte = 3;
                arte.StatusPNL = 4;
                //arte.extensionArte = "";
                //arte.extensionPNL = "";
                arte.IdEstilo = idEstilo;
                objArte.AgregarArteImagen(arte);
                arte = objArte.BuscarEstiloArteImagen(idEstilo);
                objArte.AgregarArte(arte.IdImgArte, IdItems);
            }
            else
            {
                arte = objArte.BuscarEstiloArteImagen(idEstilo);
                objArte.AgregarArte(arte.IdImgArte, IdItems);
            }
            Session["IdArte"] = arte.extensionArte;
        }

        [HttpGet]
        public ActionResult RegistrarItemsRev([Bind] POSummary descItem, string EstiloItem, string IdColor, string Cantidad, float Precio, string IdGenero, int IdTela, string TipoCamiseta, int IdEspecialidad)
        {
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            descItem.IdUsuario = noEmpleado;
            descItem.PedidosId = Convert.ToInt32(Session["idPedidoRevision"]);
            descItem.Cantidad = Int32.Parse(Cantidad);
            objItems.AgregarItems(descItem);
            Session["IdItemsRev"] = objItems.Obtener_Utlimo_Item();
            return View(descItem);
        }

        [HttpGet]
        public ActionResult RegistrarNuevoItems([Bind] POSummary descItem, string EstiloItem, string IdColor, string Cantidad, float Precio, string IdGenero, int IdTela, string TipoCamiseta, int IdEspecialidad, int IdPedido)
        {
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            descItem.IdUsuario = noEmpleado;
            descItem.PedidosId = IdPedido;
            descItem.Cantidad = Int32.Parse(Cantidad);
            objItems.AgregarItems(descItem);
            Session["IdItemsNuevo"] = objItems.Obtener_Utlimo_Item();
            this.RegistrarArteNuevo(EstiloItem);
            int IdItems = Convert.ToInt32(Session["IdItemsNuevo"]);
            this.RegistrarArtePnl(EstiloItem, IdItems);
            return View(descItem);
        }

        [HttpPost]
        public JsonResult RegistrarItem([Bind] POSummary descItem, string EstiloItem, string IdColor, string Cantidad, float Precio, string IdGenero, int IdTela, string TipoCamiseta, int IdEspecialidad, List<string> ListaPackSytle)
        {
            int noEmpleado = Convert.ToInt32(Session["id_Empleado"]);
            int PedidosId = Convert.ToInt32(Session["idPedido"]);
            descItem.IdUsuario = noEmpleado;
            descItem.PedidosId = PedidosId;
            descItem.Cantidad = Int32.Parse(Cantidad);
            objItems.AgregarItems(descItem);
            Session["IdItems"] = objItems.Obtener_Utlimo_Item();
            this.RegistrarArte(EstiloItem);
            int IdItems = Convert.ToInt32(Session["IdItems"]);
            this.RegistrarArtePnl(EstiloItem, IdItems);
            this.Obtener_Lista_TipoPacking_Estilo(ListaPackSytle);
            return Json("0", JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateColor(POSummary descItem)
        {
            if (ModelState.IsValid)
            {
                string colorEstilo = descItem.CatColores.CodigoColor;
                string colorDesc = descItem.CatColores.DescripcionColor;
                return View();
            }
            else
                return View("Index");
        }

        [HttpPost]
        public JsonResult Autocomplete_Item_Estilo(string keyword)
        {
            POSummary summary = new POSummary();
            List<ItemDescripcion> listaItems = summary.ListaItems;
            listaItems = objItemsDes.ListaItems().ToList();
            var ItemLista = (from N in listaItems
                             where N.ItemEstilo.StartsWith(keyword.ToUpper())
                             select new { Estilo = N.ItemEstilo, Descr = N.Descripcion, Id = N.ItemId });
            return Json(ItemLista, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Obtener_Lista_Tallas(List<string> ListTalla)
        {
            ItemTalla tallaItem = new ItemTalla();
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> cantidad = ListTalla[1].Split('*').ToList();
            List<string> extras = ListTalla[2].Split('*').ToList();
            List<string> ejemplos = ListTalla[3].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            for (int v = 0; v < i; v++)
            {
                tallaItem.Talla = tallas[v];

                string cantidadT = cantidad[v];
                if (cantidadT == "")
                {
                    cantidadT = "0";
                }
                tallaItem.Cantidad = Int32.Parse(cantidadT);

                string extraT = extras[v];
                if (extraT == "")
                {
                    extraT = "0";
                }
                tallaItem.Extras = Int32.Parse(extraT);

                string ejemploT = ejemplos[v];
                if (ejemploT == "")
                {
                    ejemploT = "0";
                }
                tallaItem.Ejemplos = Int32.Parse(ejemploT);

                int IdItems = Convert.ToInt32(Session["IdItems"]);
                int IdRevItems = Convert.ToInt32(Session["IdItemsRev"]);
                if (IdItems != 0)
                {
                    tallaItem.IdSummary = IdItems;
                }
                else if (IdRevItems != 0)
                {
                    tallaItem.IdSummary = IdRevItems;
                }



                objTalla.RegistroTallas(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult Obtener_Lista_TipoPacking_Estilo(List<string> ListaTipoPacking)
        {
            CatTypePackItem tallaItem = new CatTypePackItem();
            List<string> descPack = ListaTipoPacking[0].Split('*').ToList();
            int i = 0;
            foreach (var item in descPack)
            {
                i++;
            }

            i = i - 1;
            for (int v = 0; v < i; v++)
            {
                string descripcion = descPack[v];
                int tam_var = descripcion.Length;
                string codigoPack = descripcion.Substring((tam_var - 5), 5);
                tallaItem.DescripcionPack = codigoPack;
                int IdItems = Convert.ToInt32(Session["IdItems"]);
                tallaItem.IdSummary = IdItems;
                objTalla.RegistroCatTypePack(tallaItem);

            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Registrar_Lista_Tallas(List<string> ListTalla)
        {
            ItemTalla tallaItem = new ItemTalla();
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> cantidad = ListTalla[1].Split('*').ToList();
            List<string> extras = ListTalla[2].Split('*').ToList();
            List<string> ejemplos = ListTalla[3].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            for (int v = 0; v < i; v++)
            {
                tallaItem.Talla = tallas[v];

                string cantidadT = cantidad[v];
                if (cantidadT == "")
                {
                    cantidadT = "0";
                }
                tallaItem.Cantidad = Int32.Parse(cantidadT);

                string extraT = extras[v];
                if (extraT == "")
                {
                    extraT = "0";
                }
                tallaItem.Extras = Int32.Parse(extraT);

                string ejemploT = ejemplos[v];
                if (ejemploT == "")
                {
                    ejemploT = "0";
                }
                tallaItem.Ejemplos = Int32.Parse(ejemploT);

                int IdItems = Convert.ToInt32(Session["IdItemsNuevo"]);
                if (IdItems != 0)
                {
                    tallaItem.IdSummary = IdItems;
                }



                objTalla.RegistroTallas(tallaItem);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult Actualizar_Lista_Tallas(List<string> ListTalla)
        {
            ItemTalla tallaItem = new ItemTalla();
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> cantidad = ListTalla[1].Split('*').ToList();
            List<string> extras = ListTalla[2].Split('*').ToList();
            List<string> ejemplos = ListTalla[3].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            for (int v = 0; v < i; v++)
            {
                tallaItem.Talla = tallas[v];

                tallaItem.IdSummary = Convert.ToInt32(Session["id_estilo"]);

                string cantidadT = cantidad[v];
                if (cantidadT == "")
                {
                    cantidadT = "0";
                }
                tallaItem.Cantidad = Int32.Parse(cantidadT);

                string extraT = extras[v];
                if (extraT == "")
                {
                    extraT = "0";
                }
                tallaItem.Extras = Int32.Parse(extraT);

                string ejemploT = ejemplos[v];
                if (ejemploT == "")
                {
                    ejemploT = "0";
                }
                tallaItem.Ejemplos = Int32.Parse(ejemploT);
                tallaItem.IdTalla = objTalla.ObtenerIdTalla(tallaItem.Talla, tallaItem.IdSummary);
                tallaItem.Id = objTalla.ObtenerIdTallaEstilo(tallaItem.Talla, tallaItem.IdSummary);

                if (tallaItem.IdTalla == 0 && tallaItem.Id == 0)
                {

                    objTalla.RegistroTallas(tallaItem);
                }
                else
                {
                    objTalla.Actualizar_Tallas_Estilo(tallaItem);
                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult Actualizar_Edicion_Estilo_Rev([Bind] POSummary items, string EstiloItem, string IdColor, string Cantidad, float Precio, string IdGenero, int IdTela, string TipoCamiseta, int IdEspecialidad, string IdEstilo, string PedidoId)
        {
            items.IdItems = Int32.Parse(IdEstilo);
            items.PedidosId = Int32.Parse(PedidoId);
            items.Id_Genero = objGenero.ObtenerIdGenero(IdGenero);
            items.IdCamiseta = objTipoC.ObtenerIdTipoCamiseta(TipoCamiseta);
            items.IdEstilo = objItemsDes.ObtenerIdEstilo(EstiloItem);
            items.ColorId = objColores.ObtenerIdColor(IdColor);
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

        [HttpPost]
        public JsonResult Actualizar_Info_Estilo(List<string> ListTalla, string IdEstilo)
        {
            ItemTalla tallaItem = new ItemTalla();
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> cantidad = ListTalla[1].Split('*').ToList();
            List<string> extras = ListTalla[2].Split('*').ToList();
            List<string> ejemplos = ListTalla[3].Split('*').ToList();
            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            for (int v = 0; v < i; v++)
            {
                tallaItem.Talla = tallas[v];

                tallaItem.IdSummary = Int32.Parse(IdEstilo); ;

                string cantidadT = cantidad[v];
                if (cantidadT == "")
                {
                    cantidadT = "0";
                }
                tallaItem.Cantidad = Int32.Parse(cantidadT);

                string extraT = extras[v];
                if (extraT == "")
                {
                    extraT = "0";
                }
                tallaItem.Extras = Int32.Parse(extraT);

                string ejemploT = ejemplos[v];
                if (ejemploT == "")
                {
                    ejemploT = "0";
                }
                tallaItem.Ejemplos = Int32.Parse(ejemploT);
                tallaItem.IdTalla = objTalla.ObtenerIdTalla(tallaItem.Talla, tallaItem.IdSummary);
                tallaItem.Id = objTalla.ObtenerIdTallaEstilo(tallaItem.Talla, tallaItem.IdSummary);

                if (tallaItem.IdTalla == 0 && tallaItem.Id == 0)
                {

                    objTalla.RegistroTallas(tallaItem);
                }
                else
                {
                    objTalla.Actualizar_Tallas_Estilo(tallaItem);
                }
            }

            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult Actualizar_Edicion_Estilo([Bind] POSummary items, string EstiloItem, string IdColor, string Cantidad, float Precio, string IdGenero, int IdTela, string TipoCamiseta, int IdEspecialidad, string IdEstilo, string PedidoId)
        {
            items.IdItems = Int32.Parse(IdEstilo);
            items.PedidosId = Int32.Parse(PedidoId);
            items.Id_Genero = objGenero.ObtenerIdGenero(IdGenero);
            items.IdCamiseta = objTipoC.ObtenerIdTipoCamiseta(TipoCamiseta);
            items.IdEstilo = objItemsDes.ObtenerIdEstilo(EstiloItem);
            items.ColorId = objColores.ObtenerIdColor(IdColor);
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


        [HttpPost]
        public JsonResult Obtener_Lista_Tallas_UPC(List<string> ListTalla, int IdSummary)
        {

            UPC upcTalla = new UPC();
            List<string> tallas = ListTalla[0].Split('*').ToList();
            List<string> upc = ListTalla[1].Split('*').ToList();

            int i = 0;
            foreach (var item in tallas)
            {
                i++;
            }

            i = i - 1;
            for (int v = 0; v < i; v++)
            {
                upcTalla.Talla = tallas[v];

                string cantidadT = upc[v];
                if (cantidadT == "")
                {
                    cantidadT = "0";
                }
                long number = long.Parse(cantidadT);
                upcTalla.UPC1 = number;


                upcTalla.IdSummary = IdSummary;


                objTalla.RegistroTallasUPC(upcTalla);


            }
            return Json("0", JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public void EliminarTallaPorEstilo(string estilo, string talla)
        {

            int idTalla = objTallas.ObtenerIdTalla(talla);
            int idEstilo = Int32.Parse(estilo);
            objTallas.EliminarTallasIdEstilo(idEstilo, idTalla);


        }

        [HttpPost]
        public JsonResult Autocomplete_Item_Desc(string keyword, string estilo)
        {
            POSummary summary = new POSummary();
            List<ItemDescripcion> listaItems = summary.ListaItems;
            listaItems = objItemsDes.ListaItems().ToList();
            var ItemLista = (from N in listaItems
                             where N.ItemEstilo.StartsWith(keyword.ToUpper())
                             select new
                             {
                                 label = N.ItemEstilo,
                                 val = N.ItemEstilo,
                                 descripcion = N.Descripcion,
                                 id = N.ItemId
                             });
            return Json(ItemLista, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Autocomplete_Color(string keyword)
        {
            POSummary summary = new POSummary();
            List<CatColores> listaColores = summary.ListaColores;
            listaColores = objColores.ListaColores().ToList();
            var Colores = (from N in listaColores
                           where N.CodigoColor.StartsWith(keyword.ToUpper())
                           select new { N.CodigoColor, Color = N.DescripcionColor, Id = N.IdColor });
            return Json(Colores, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Autocomplete_Talla(string keyword)
        {
            POSummary summary = new POSummary();
            List<CatTallaItem> listTallas = summary.ListaTallas;
            listTallas = objTallas.ListaTallas().ToList();
            var TallaLista = (from N in listTallas
                              where N.Talla.StartsWith(keyword.ToUpper())
                              select new { N.Talla });
            return Json(TallaLista, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ListarTallasPorGenero(string Genero)
        {
            POSummary summary = new POSummary();
            List<CatGenero> listaGenero = summary.ListarTallasPorGenero;
            listaGenero = objGenero.ListarTallasPorGenero(Genero).ToList();
            summary.ListarTallasPorGenero = listaGenero;

            return View(summary);

        }

        public JsonResult List(string Genero)
        {
            POSummary summary = new POSummary();
            List<CatGenero> listaGenero = summary.ListarTallasPorGenero;
            listaGenero = objGenero.ListarTallasPorGenero(Genero).ToList();
            summary.ListarTallasPorGenero = listaGenero;
            return Json(listaGenero, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Lista_Tallas_Por_Estilos(int idEstilos)
        {
            Session["id_estilo"] = idEstilos;
            POSummary summary = new POSummary();
            List<ItemTalla> listaTallas = summary.ListarTallasPorEstilo;
            listaTallas = objTalla.ListaTallasPorEstilo(idEstilos).ToList();
            summary.ListarTallasPorEstilo = listaTallas;
            return Json(listaTallas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Lista_Tallas_Por_Estilos_Rev(int? id)
        {
            Session["id_estilo"] = id;
            POSummary summary = new POSummary();
            List<ItemTalla> listaTallas = summary.ListarTallasPorEstilo;
            listaTallas = objTalla.ListaTallasPorEstiloRev(id).ToList();
            summary.ListarTallasPorEstilo = listaTallas;
            var result = Json(new { listaTalla = listaTallas });
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ListarTallasPorGenero(POSummary descItem)
        {
            return View();
        }

    }
}