using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Rotativa;
using System.Web.Routing;
using FortuneSystem.Models.Staging;
using FortuneSystem.Models.Almacen;
namespace FortuneSystem.Controllers
{
    public class StagingController : Controller
    {
        // GET: Staging
        StagingGeneral stag = new StagingGeneral();
        FuncionesInventarioGeneral consultas = new FuncionesInventarioGeneral();
        DatosStaging ds = new DatosStaging();

        public ActionResult Index(){
            //Session["id_usuario"] = consultas.buscar_id_usuario(Convert.ToString(Session["usuario"]));
            //Session["id_usuario"] = 2;
            //Session["id_sucursal"] = consultas.obtener_sucursal_id_usuario(Convert.ToInt32(Session["id_usuario"]));
            int id_usuario = Convert.ToInt32(Session["id_Empleado"]);
            Session["id_usuario"] = id_usuario;           
            //Session["id_usuario"] = 2;           
            Session["id_sucursal"] = consultas.obtener_sucursal_id_usuario(Convert.ToInt32(Session["id_usuario"]));
            Session["turno"] = consultas.obtener_turno_usuario(Convert.ToInt32(Session["id_usuario"]));            
            return View();
        }
        public ActionResult Autocomplete_paises(string term)
        {
            var items = consultas.Lista_paises();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_colores(string term)
        {
            var items = consultas.Lista_colores();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_colores_codigos(string term)
        {
            var items = consultas.Lista_colores_codigos();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_tallas(string term)
        {
            var items = consultas.Lista_tallas();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_percents(string term)
        {
            var items = consultas.Lista_porcentajes();
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Autocomplete_empleados(string term)
        {
            var items = consultas.Lista_empleados(Convert.ToInt32(Session["turno"]),1);
            var filteredItems = items.Where(item => item.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return Json(filteredItems, JsonRequestBehavior.AllowGet);
        }

        
       
        [HttpPost]
        public JsonResult imprimir_papeleta_vacia_staging(string datos)
        {
            string[] data = datos.Split('&');
            //inventario//estilo//pedido
            Session["id_inventario"] = data[0];
            Session["id_estilo"] = data[1];
            Session["id_pedido"] = data[2];

            return Json("0", JsonRequestBehavior.AllowGet);
        }

        
        public JsonResult guardar_stag_bd(string po,string estilo,string size,string quantity, string employee,string fabric_percent,string country,string color,string comentario) {
            string[] tallas = size.Split('*'), cantidades = quantity.Split('*'), empleados = employee.Split('*'), porcentajes = fabric_percent.Split('*'), paises = country.Split('*'), colores = color.Split('*');
            int total = 0,id_size,id_color,id_pais,id_percent;
            int summary = ds.buscar_id_summary(po, estilo);
            for (int i = 1; i < cantidades.Length; i++) {
                total += Convert.ToInt32(cantidades[i]);
            }
            //int id_empleado;
            ds.guardar_stag_bd(po,estilo,total,Convert.ToInt32(Session["id_usuario"]),summary,comentario);
            int id_stag = ds.obtener_ultimo_stag();
            for (int i = 1; i < cantidades.Length; i++){
               // id_empleado = stag.obtener_id_empleado(empleados[i]);
                id_size = consultas.buscar_talla(tallas[i]);
                id_color = consultas.buscar_color(colores[i]);
                id_pais = consultas.buscar_id_pais(paises[i]);
                id_percent = consultas.buscar_percent(porcentajes[i]);
                ds.guardar_stag_conteos(id_stag, id_size, id_pais,id_color,id_percent,Convert.ToInt32(cantidades[i]), empleados[i]);
            }
            Session["id_staging"] = id_stag;
            return Json("",JsonRequestBehavior.AllowGet);
        }
        //buscar_staging_inicio
        public JsonResult buscar_staging_inicio()
        {
            return Json(ds.obtener_staging_inicio(), JsonRequestBehavior.AllowGet);
        }
        //abrir_papeleta_stag
        public JsonResult abrir_papeleta_stag(string id){
            Session["id_staging"] = id;
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult buscar_datos_grafica() {
            return Json(stag.obtener_lista_staging_grafica(), JsonRequestBehavior.AllowGet);
        }
/***********************************************************************************************************************************************************************/
        public JsonResult buscar_pedidos_inicio(string busqueda){
            return Json(ds.buscar_pedidos_recibo(Convert.ToInt32(Session["id_sucursal"]),busqueda), JsonRequestBehavior.AllowGet);
        }
        public JsonResult buscar_pedido_estilo_tallas(string datos) {
            string[] data = datos.Split('*');
            //summary//estilo//pedido
            
            Session["id_estilo_count"] = data[0];
            Session["id_pedido_count"] = data[1];
            Session["id_summary_count"] = data[2];           
            var resultado = Json(new {
                result = ds.lista_papeleta(Convert.ToInt32(Session["id_estilo_count"]), Convert.ToInt32(Session["id_pedido_count"]), Convert.ToInt32(Session["turno"])),
            });
            return Json(resultado, JsonRequestBehavior.AllowGet);
           
        }

        public JsonResult buscar_conteos_estilo(){
            List<Talla_staging> lista_tallas = ds.obtener_cantidades_tallas_estilo(Convert.ToInt32(Session["id_summary_count"]));
            List<Talla_staging> totales_orden = ds.obtener_cantidades_tallas_estilo(Convert.ToInt32(Session["id_summary_count"]));
            List<Talla_staging> totales_stagin = ds.obtener_cantidades_tallas_estilo_staging(lista_tallas, Convert.ToInt32(Session["id_summary_count"]));
            string nombre = consultas.obtener_estilo(Convert.ToInt32(Session["id_estilo_count"])) + " " + consultas.buscar_descripcion_estilo(Convert.ToInt32(Session["id_estilo_count"]));
            var resultado = Json(new{
                result = ds.lista_papeleta(Convert.ToInt32(Session["id_estilo_count"]), Convert.ToInt32(Session["id_pedido_count"]), Convert.ToInt32(Session["turno"])),
                lista_totales_orden = totales_orden,
                lista_staging = totales_stagin,
                estilo = nombre
            });
            return Json(resultado, JsonRequestBehavior.AllowGet);

        }




























































    }
}