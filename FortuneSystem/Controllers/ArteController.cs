using FortuneSystem.Models;
using FortuneSystem.Models.Arte;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Item;
using FortuneSystem.Models.Items;
using FortuneSystem.Models.Pedidos;
using FortuneSystem.Models.POSummary;
using FortuneSystem.Models.PrintShop;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace FortuneSystem.Controllers
{
    public class ArteController : Controller
    {
        ArteData objArte = new ArteData();
        CatTallaItemData objItem = new CatTallaItemData();
        ItemDescripcionData objDesc = new ItemDescripcionData();
        PedidosData objPedido = new PedidosData();
        DescripcionItemData objItems = new DescripcionItemData();
        CatEspecialidadesData objEspecialidad = new CatEspecialidadesData();
        private MyDbContext db = new MyDbContext();


        public ActionResult Index()
        {
            List<IMAGEN_ARTE> listaArtes = new List<IMAGEN_ARTE>();
            listaArtes = objArte.ListaInvArtes().ToList();
            return View(listaArtes);

        }

        public ActionResult IndexPNL()
        {
            List<IMAGEN_ARTE_PNL> listaArtes = new List<IMAGEN_ARTE_PNL>();
            listaArtes = objArte.ListaInvArtesPnl().ToList();
            return View(listaArtes);
        }

        public ActionResult CatalogoArte()
        {
            return View();
        }

        public ActionResult FileUpload(int idArte)
        {
            IMAGEN_ARTE IArte = db.ImagenArte.Find(idArte);
            ARTE art = db.Arte.Where(x => x.IdImgArte == idArte).FirstOrDefault();
            CatEspecialidades catEspecialidad = new CatEspecialidades();
            catEspecialidad.IdEspecialidad = objItems.ObtenerEspecialidadPorIdSummary(art.IdSummary);
            if (catEspecialidad.IdEspecialidad == 0)
            {
                catEspecialidad.IdEspecialidad = 12;
            }
            IArte.ListaTecnicas = objEspecialidad.ListaEspecialidades().ToList();
            ViewBag.listEspecialidad = new SelectList(IArte.ListaTecnicas, "IdEspecialidad", "Especialidad", catEspecialidad.IdEspecialidad);
            IArte.CATARTE = art;
            IArte.CatEspecialidades = catEspecialidad;
            ObtenerEstados(IArte.StatusArte, IArte);

            return View(IArte);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileUpload([Bind] IMAGEN_ARTE Arte, HttpPostedFileBase fileArte)
        {
            if (Arte.extensionArte == null)
            {
                fileArte = Arte.FileArte;
                if (fileArte != null)
                {
                    string ext = Path.GetFileName(fileArte.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/imagenesArte"), ext);
                    if (System.IO.File.Exists(path))
                    {
                        Arte.extensionArte = ext;
                    }
                    else
                    {
                        Arte.extensionArte = ext;
                        fileArte.SaveAs(path);
                    }

                    TempData["imagArteOK"] = "The Art image was registered correctly.";
                }
            }

            ObtenerEstadosPorId(Arte);

            if (ModelState.IsValid)
            {
                db.Entry(Arte).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(Arte);
        }

        public ActionResult FileUploadPNL(int id, int idEst, string descItem)
        {
            IMAGEN_ARTE_PNL IArtes = new IMAGEN_ARTE_PNL();
            IMAGEN_ARTE_PNL IArte = db.ImagenArtePnl.Where(x => x.IdSummary == id).FirstOrDefault();
            int? idStatus = 0;
            if (IArte == null)
            {
                IArte = IArtes;
                IArte.StatusPNL = 4;
                idStatus = IArte.StatusPNL;
                IArte.IdSummary = id;
                IArte.EstadosPNL = 0;
                IArte.IdEstilo = idEst;
                IArte.DescripcionEstilo = descItem;

            }
            else
            {
                idStatus = IArte.StatusPNL;
                IArte.DescripcionEstilo = descItem;
            }
            IArte.Tienda = objArte.ObtenerclienteSummary(id);
            Regex kohl = new Regex("KOHL");
            Regex walmart = new Regex("WAL-");
            IArte.ResultadoK = kohl.Matches(IArte.Tienda);
            IArte.ResultadoW = walmart.Matches(IArte.Tienda);
            IArte.Estilo = objDesc.ObtenerEstiloPorId(IArte.IdEstilo);
            ObtenerEstadosPNL(IArte.StatusPNL, IArte);
            if (IArte.IdImgArtePNL == 0)
            {
                objArte.AgregarArtePnlImagen(IArte);
                IArte.IdImgArtePNL = objItems.Obtener_Utlimo_Id_Arte_Pnl();
            }
            return PartialView(IArte);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileUploadPNL([Bind] IMAGEN_ARTE_PNL artePNL, HttpPostedFileBase filePNL)
        {
            if (artePNL.ExtensionPNL == null)
            {
                filePNL = artePNL.FilePNL;
                if (filePNL != null)
                {
                    string ext = Path.GetFileName(filePNL.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/imagenesPNL"), ext);
                    if (System.IO.File.Exists(path))
                    {
                        //System.IO.File.Delete(path);
                        artePNL.ExtensionPNL = ext;
                    }
                    else
                    {
                        artePNL.ExtensionPNL = ext;
                        filePNL.SaveAs(path);
                    }

                    TempData["imagPnlOK"] = "The PNL image was registered correctly.";
                }
            }

            if (artePNL.EstadosPNL == EstatusImgPNL.APPROVED)
            {
                artePNL.StatusPNL = 1;
            }
            else if (artePNL.EstadosPNL == EstatusImgPNL.INHOUSE)
            {
                artePNL.StatusPNL = 2;
            }
            else if (artePNL.EstadosPNL == EstatusImgPNL.REVIEWED)
            {
                artePNL.StatusPNL = 3;
            }
            else if (artePNL.EstadosPNL == EstatusImgPNL.PENDING)
            {
                artePNL.StatusPNL = 4;
            }
            artePNL.Tienda = objArte.ObtenerclienteSummary(artePNL.IdSummary);
            Regex kohl = new Regex("KOHL");
            Regex walmart = new Regex("WAL-");
            artePNL.ResultadoK = kohl.Matches(artePNL.Tienda);
            artePNL.ResultadoW = walmart.Matches(artePNL.Tienda);
            int idPedido = objPedido.Obtener_Id_Pedido(artePNL.IdSummary);
            if (ModelState.IsValid)
            {
                db.Entry(artePNL).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Detalles", "Pedidos", new { id = idPedido });
            }

            return View(artePNL);
        }

        [HttpPost]
        public ActionResult FileUploadEstilo(HttpPostedFileBase FileArte)
        {
            POSummary arte = new POSummary();
            if (arte.ExtensionArte == null)
            {
                FileArte = arte.FileArte;
                if (FileArte != null)
                {
                    string ext = Path.GetFileName(FileArte.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/imagenesEstilos"), ext);
                    arte.ExtensionArte = ext;
                    FileArte.SaveAs(path);
                    TempData["imagArteOK"] = "The Art image was registered correctly.";
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {

                        HttpPostedFileBase file = files[i];
                        string fname;

                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER" || Request.Browser.Browser.ToUpper() == "FF")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        fname = Path.Combine(Server.MapPath("~/Content/imagenesEstilos"), fname);
                        file.SaveAs(fname);
                    }
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        // GET: Arte

        public ActionResult ListaImgArte(int id)
        {
            List<IMAGEN_ARTE> listaArtes = new List<IMAGEN_ARTE>();
            listaArtes = objArte.ListaArtes(id).ToList();
            return PartialView(listaArtes);
        }

        public ActionResult ListaImgArtePNL(int id)
        {
            List<IMAGEN_ARTE_PNL> listaArtes = new List<IMAGEN_ARTE_PNL>();
            listaArtes = objArte.ListaArtesPNL(id).ToList();
            return PartialView(listaArtes);
        }

        public ActionResult ConvertirImagen(int arteCodigo)
        {
            var arte = db.ImagenArte.Where(x => x.IdImgArte == arteCodigo).FirstOrDefault();
            if (arte != null)
            {
                if (arte.extensionArte != null && arte.extensionArte != "")
                {

                    switch (arte.extensionArte.ToLower())
                    {
                        case "gif":
                            return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Image.Gif);
                        case "jpeg":
                            return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                        default:
                            return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Application.Octet);
                    }

                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }

            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }

        }


        public ActionResult ConvertirImagenArte(string extensionArte)
        {
            IMAGEN_ARTE arte = new IMAGEN_ARTE() { extensionArte = extensionArte };

            if (arte.extensionArte != null && arte.extensionArte != "")
            {
                switch (arte.extensionArte.ToLower())
                {
                    case "gif":
                        return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Image.Gif);
                    case "jpeg":
                        return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                    default:
                        return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Application.Octet);
                }

            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }

        }


        public ActionResult ConvertirImagenArtePNL(string extensionPnl)
        {
            IMAGEN_ARTE_PNL arte = new IMAGEN_ARTE_PNL() { ExtensionPNL = extensionPnl };

            if (arte.ExtensionPNL != null && arte.ExtensionPNL != "")
            {
                switch (arte.ExtensionPNL.ToLower())
                {
                    case "gif":
                        return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Image.Gif);
                    case "jpeg":
                        return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                    default:
                        return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Application.Octet);
                }

            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }


        }

        public ActionResult ConvertirImagenPNL(int pnlCodigo)
        {
            var arte = db.ImagenArtePnl.Where(x => x.IdImgArtePNL == pnlCodigo).FirstOrDefault();
            if (arte != null)
            {
                if (arte.ExtensionPNL != null && arte.ExtensionPNL != "")
                {
                    switch (arte.ExtensionPNL.ToLower())
                    {
                        case "gif":
                            return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Image.Gif);
                        case "jpeg":
                            return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                        default:
                            return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Application.Octet);
                    }
                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }
            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }
        }

        public ActionResult ObtenerIdEstilo(int id)
        {
            Session["idSummary"] = id;
            return View();
        }

        public ActionResult ConvertirImagenArteEstilo(string nombreEstilo)
        {
            int idEstilo = objDesc.ObtenerIdEstilo(nombreEstilo);
            var arte = db.ImagenArte.Where(x => x.IdEstilo == idEstilo).FirstOrDefault();
            if (arte != null)
            {
                if (arte.extensionArte != null && arte.extensionArte != "")
                {
                    switch (arte.extensionArte.ToLower())
                    {
                        case "gif":
                            return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Image.Gif);
                        case "jpeg":
                            return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                        default:
                            return new FilePathResult("~/Content/imagenesArte/" + arte.extensionArte, System.Net.Mime.MediaTypeNames.Application.Octet);
                    }

                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }

            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }

        }

        public ActionResult ConvertirImagenPNLEstilo(string nombreEstilo)
        {
            int idEstilo = objDesc.ObtenerIdEstilo(nombreEstilo);
            var arte = db.ImagenArtePnl.Where(x => x.IdEstilo == idEstilo).FirstOrDefault();
            if (arte != null)
            {
                if (arte.ExtensionPNL != null && arte.ExtensionPNL != "")
                {
                    switch (arte.ExtensionPNL.ToLower())
                    {
                        case "gif":
                            return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Image.Gif);
                        case "jpeg":
                            return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Image.Jpeg);
                        default:
                            return new FilePathResult("~/Content/imagenesPNL/" + arte.ExtensionPNL, System.Net.Mime.MediaTypeNames.Application.Octet);
                    }
                }
                else
                {
                    return File("~/Content/img/noImagen.png", "image/png");
                }

            }
            else
            {
                return File("~/Content/img/noImagen.png", "image/png");
            }

        }

        public ActionResult Create(int? id, int idArte)
        {
            IMAGEN_ARTE IArte = db.ImagenArte.Find(idArte);

            ARTE art = db.Arte.Where(x => x.IdImgArte == idArte).FirstOrDefault();
            Session["id"] = id;
            int Summary = Convert.ToInt32(Session["id"]);
            art.IdEstilo = Summary;
            IArte.CATARTE = art;
            IArte.Tienda = objArte.ObtenerclienteEstilo(id, idArte);
            Regex kohl = new Regex("KOHL");
            Regex walmart = new Regex("WAL-");
            IArte.ResultadoK = kohl.Matches(IArte.Tienda);
            IArte.ResultadoW = walmart.Matches(IArte.Tienda);
            ObtenerEstados(IArte.StatusArte, IArte);

            return View(IArte);
        }

        public ActionResult EditarArtePNL(int? id, int idArte)
        {
            IMAGEN_ARTE_PNL IArte = db.ImagenArtePnl.Find(idArte);

            Session["id"] = id;
            int Summary = Convert.ToInt32(Session["id"]);
            IArte.IdSummary = id;
            IArte.Tienda = objArte.ObtenerclienteSummary(IArte.IdSummary);
            Regex kohl = new Regex("KOHL");
            Regex walmart = new Regex("WAL-");
            IArte.ResultadoK = kohl.Matches(IArte.Tienda);
            IArte.ResultadoW = walmart.Matches(IArte.Tienda);
            IArte.Estilo = objDesc.ObtenerEstiloPorId(IArte.IdEstilo);
            ObtenerEstadosPNL(IArte.StatusPNL, IArte);

            return View(IArte);
        }

        public ActionResult ActualizarImagenPNL(int? id, int idArte)
        {
            IMAGEN_ARTE_PNL IArte = db.ImagenArtePnl.Find(idArte);

            IArte.ExtensionPNL = objArte.BuscarExtensionPNLPorId(IArte.IdImgArtePNL);

            return View(IArte);
        }

        public ActionResult ActualizarImagenArt(/*int? id,*/ int idArte, string status, int idEspecialidad)
        {
            IMAGEN_ARTE IArte = db.ImagenArte.Find(idArte);
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;

                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER" || Request.Browser.Browser.ToUpper() == "FF")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        string ext = Path.GetFileName(file.FileName);
                        fname = Path.Combine(Server.MapPath("~/Content/imagenesArte"), ext);
                        if (System.IO.File.Exists(ext))
                        {

                            System.IO.File.Replace(IArte.extensionArte, ext, ext);
                            IArte.extensionArte = ext;
                            file.SaveAs(fname);
                        }
                        else
                        {
                            IArte.extensionArte = ext;
                            file.SaveAs(fname);
                        }


                    }
                    ActualizarInfoImagenArte(idArte, status, idEspecialidad, IArte);
                    TempData["imgArteOK"] = "The Art was modified correctly.";
                    return Json(new
                    {
                        redirectUrl = Url.Action("Index", "Arte"),
                        isRedirect = true
                    });
                }
                catch (Exception ex)
                {
                    TempData["imgArteError"] = "The Art could not be modified, try it later." + ex.Message;
                    return Json(new
                    {
                        redirectUrl = Url.Action("Index", "Arte"),
                        isRedirect = true
                    });
                }
            }
            else
            {
                ActualizarInfoImagenArte(idArte, status, idEspecialidad, IArte);
                TempData["imgArteOK"] = "The Art was modified correctly.";
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Arte"),
                    isRedirect = true
                });
            }


            //IArte.ExtensionL = objArte.BuscarExtensionPNLPorId(IArte.IdImgArtePNL);

            //return View(IArte);
        }

        public void ActualizarInfoImagenArte(int idArte, string status, int idEspecialidad, IMAGEN_ARTE IArte)
        {

            if (status == "APPROVED")
            {
                IArte.StatusArte = 1;

            }
            else if (status == "REVIEWED")
            {
                IArte.StatusArte = 2;

            }
            else if (status == "PENDING")
            {
                IArte.StatusArte = 3;
            }
            objArte.ActualizarImagen(IArte);
            List<int> listado = objArte.ListaEstilosPorImagenesArte(idArte).ToList();
            foreach (int id in listado)
            {
                objArte.ActualizarEspecialidadImagenArte(id, idEspecialidad);
            }
        }

        public void ObtenerEstadosPNL(int? idStatus, IMAGEN_ARTE_PNL IArte)
        {
            //Obtener el idEstado PNL
            if (idStatus == 1)
            {
                IArte.EstadosPNL = EstatusImgPNL.APPROVED;
            }
            else if (idStatus == 2)
            {
                IArte.EstadosPNL = EstatusImgPNL.INHOUSE;
            }
            else if (idStatus == 3)
            {
                IArte.EstadosPNL = EstatusImgPNL.REVIEWED;
            }
            else if (idStatus == 4)
            {
                IArte.EstadosPNL = EstatusImgPNL.PENDING;
            }
        }

        public void ObtenerEstados(int? idEstadoArte, IMAGEN_ARTE arte)
        {
            //Obtener el idEstado Arte 
            if (idEstadoArte == 1)
            {
                arte.EstadosArte = EstatusArte.APPROVED;
            }
            else if (idEstadoArte == 2)
            {
                arte.EstadosArte = EstatusArte.REVIEWED;
            }
            else if (idEstadoArte == 3)
            {
                arte.EstadosArte = EstatusArte.PENDING;
            }
        }

        public void ObtenerEstadosPorId(IMAGEN_ARTE Arte)
        {
            if (Arte.EstadosArte == EstatusArte.APPROVED)
            {
                Arte.StatusArte = 1;
            }
            else if (Arte.EstadosArte == EstatusArte.REVIEWED)
            {
                Arte.StatusArte = 2;
            }
            else if (Arte.EstadosArte == EstatusArte.PENDING)
            {
                Arte.StatusArte = 3;
            }

            if (Arte.EstadosPNL == EstatusPNL.APPROVED)
            {
                Arte.StatusPNL = 1;
            }
            else if (Arte.EstadosPNL == EstatusPNL.INHOUSE)
            {
                Arte.StatusPNL = 2;
            }
            else if (Arte.EstadosPNL == EstatusPNL.REVIEWED)
            {
                Arte.StatusPNL = 3;
            }
            else if (Arte.EstadosPNL == EstatusPNL.PENDING)
            {
                Arte.StatusPNL = 4;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind] IMAGEN_ARTE Arte)
        {

            return View(Arte);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarArtePNL([Bind] IMAGEN_ARTE_PNL artePNL, HttpPostedFileBase filePNL)
        {
            if (artePNL.ExtensionPNL == null)
            {
                filePNL = artePNL.FilePNL;
                if (filePNL != null)
                {
                    string ext = Path.GetFileName(filePNL.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/imagenesPNL"), ext);
                    if (System.IO.File.Exists(path))
                    {
                        //System.IO.File.Delete(path);
                        artePNL.ExtensionPNL = ext;
                    }
                    else
                    {
                        artePNL.ExtensionPNL = ext;
                        filePNL.SaveAs(path);
                    }

                    TempData["imagArtePNLOK"] = "The PNL image was modified correctly.";
                    objArte.ActualizarImagenPNL(artePNL);
                }
            }

            IMAGEN_ARTE_PNL IArte = db.ImagenArtePnl.Find(artePNL.IdImgArtePNL);

            //Session["id"] = id;
            int Summary = Convert.ToInt32(Session["id"]);
            artePNL.IdSummary = IArte.IdSummary;
            artePNL.Tienda = objArte.ObtenerclienteSummary(IArte.IdSummary);
            Regex kohl = new Regex("KOHL");
            Regex walmart = new Regex("WAL-");
            artePNL.ResultadoK = kohl.Matches(artePNL.Tienda);
            artePNL.ResultadoW = walmart.Matches(artePNL.Tienda);
            artePNL.Estilo = objDesc.ObtenerEstiloPorId(IArte.IdEstilo);
            if (artePNL.EstadosPNL == EstatusImgPNL.APPROVED)
            {
                artePNL.StatusPNL = 1;
            }
            else if (artePNL.EstadosPNL == EstatusImgPNL.INHOUSE)
            {
                artePNL.StatusPNL = 2;
            }
            else if (artePNL.EstadosPNL == EstatusImgPNL.REVIEWED)
            {
                artePNL.StatusPNL = 3;
            }
            else if (artePNL.EstadosPNL == EstatusImgPNL.PENDING)
            {
                artePNL.StatusPNL = 4;
            }
            ObtenerEstadosPNL(artePNL.StatusPNL, artePNL);

            objArte.ActualizarEstadoImagenPNL(artePNL);
            TempData["imagArtePNLOK"] = "The PNL image was modified correctly.";
            return RedirectToAction("IndexPNL");
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            IMAGEN_ARTE arte = db.ImagenArte.Find(id);
            arte.Tienda = objArte.ObtenerclienteEstilo(id, arte.IdImgArte);
            if (arte == null)
            {
                return HttpNotFound();
            }

            return View(arte);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind] IMAGEN_ARTE Arte, HttpPostedFileBase imgArte, HttpPostedFileBase imgPNL)
        {
            imgArte = Arte.FileArte;

            if (imgArte != null)
            {
                string ext = Path.GetFileName(imgArte.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/imagenesArte"), ext);
                Arte.extensionArte = ext;
                imgArte.SaveAs(path);
            }

            imgPNL = Arte.FilePNL;
            if (imgPNL != null)
            {
                string ext = Path.GetFileName(imgPNL.FileName);
                string path = Path.Combine(Server.MapPath("~/Content/imagenesArte"), ext);
                Arte.extensionPNL = ext;
                imgPNL.SaveAs(path);
            }
            if (ModelState.IsValid)
            {
                //db.Entry(Arte).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Arte);
        }


        public JsonResult Lista_Tallas_Estilo()
        {
            IMAGEN_ARTE arte = new IMAGEN_ARTE();
            int idEstilo = Convert.ToInt32(Session["id"]);
            List<CatTallaItem> listaT = objItem.Lista_tallas_Estilo_Arte(idEstilo).ToList();
            arte.ListaTallas = listaT;

            List<UPC> listaU = objItem.Lista_tallas_upc(idEstilo).ToList();
            var result = Json(new { listaTalla = listaT, listaUPC = listaU });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Lista_Tallas_Estilo_Arte_Pnl(int idEstilo)
        {
            IMAGEN_ARTE_PNL arte = new IMAGEN_ARTE_PNL();
            List<CatTallaItem> listaT = objItem.Lista_tallas_Estilo_Arte(idEstilo).ToList();
            arte.ListaTallas = listaT;

            List<UPC> listaU = objItem.Lista_tallas_upc(idEstilo).ToList();
            var result = Json(new { listaTalla = listaT, listaUPC = listaU });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Capture()
        {
            string CapturedFilePath = "";
            try
            {
                Bitmap bitmap = new Bitmap
              (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);

                Graphics graphics = Graphics.FromImage(bitmap as System.Drawing.Image);
                graphics.CopyFromScreen(25, 25, 25, 25, bitmap.Size);

                bitmap.Save(CapturedFilePath, ImageFormat.Bmp);
            }
            catch (Exception ex)
            {

            }
            return View();
        }



    }

}