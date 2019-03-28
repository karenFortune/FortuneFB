using FortuneSystem.Models;
using FortuneSystem.Models.Catalogos;
using FortuneSystem.Models.Login;
using FortuneSystem.Models.Roles;
using FortuneSystem.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FortuneSystem.Controllers
{
    public class LoginController : Controller
    {

        CatUsuarioData objUsr = new CatUsuarioData();
        CatSucursalData objSucursal = new CatSucursalData();
        CatRolesData objCaRoles = new CatRolesData();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            CatUsuario suc = new CatUsuario();
            List<CatSucursal> listaSucursal = suc.ListaSucursal;
            listaSucursal = objSucursal.ListaSucursales().ToList();

            ViewBag.listSucursal = new SelectList(listaSucursal, "IdSucursal", "Sucursal", suc.IdSucursal);
            return View();
        }

        [HttpPost]
        public ActionResult Login(CatUsuario usuario)
        {
            LoginData objData = new LoginData();
            string empleado = usuario.NoEmpleado.ToString();
            string actionName = "";
            string nameController = "";
            if (ModelState.IsValid == false)
            {
                if (empleado != "0" && usuario.Contrasena != null)
                {
                    objData.IsValid(empleado, usuario.Contrasena, usuario);
                    usuario.Nombres = objUsr.Obtener_Nombre_Usuario(empleado);
                    Session["nombre"] = usuario.Nombres;
                    int noEmpleado = objUsr.Obtener_Datos_Usuarios(empleado);
                    Session["id_Empleado"] = noEmpleado;
                    Session["idCargo"] = usuario.Cargo;
                    usuario.CatRoles = objCaRoles.ConsultarListaRoles(usuario.Cargo);
                    Session["rolUser"] = usuario.CatRoles.Rol;
                    string pass = objUsr.Obtener_Contraseña_Usuario(empleado);
                    if (noEmpleado != 0 && pass.CompareTo(usuario.Contrasena) == 0)
                    {
                        if (usuario.Cargo == 1)
                        {
                            actionName = "Index";
                            nameController = "Pedidos";

                        }
                        else if (usuario.Cargo == 4)
                        {
                            actionName = "Index";
                            nameController = "Almacen";

                        }
                        else if (usuario.Cargo == 5)
                        {
                            actionName = "Index";
                            nameController = "PrintShop";

                        }
                        else if (usuario.Cargo == 6)
                        {
                            actionName = "Index";
                            nameController = "Shipping";

                        }
                        else if (usuario.Cargo == 7)
                        {
                            actionName = "Index";
                            nameController = "Staging";

                        }
                        else if (usuario.Cargo == 8)
                        {
                            actionName = "Index";
                            nameController = "PNL";

                        }
                        else if (usuario.Cargo == 9)
                        {
                            actionName = "Index";
                            nameController = "Packing";

                        }
                        else if (usuario.Cargo == 12)
                        {
                            actionName = "Index";
                            nameController = "Arte";

                        }
                        else if (usuario.Cargo == 15)
                        {
                            actionName = "Index";
                            nameController = "Pedidos";

                        }
                        else if (usuario.Cargo == 0)
                        {
                            actionName = "Login";
                            nameController = "Login";

                        }
                    }
                    else
                    {
                        actionName = "Login";
                        nameController = "Login";
                        TempData["loginError"] = "The employee number or password is incorrect.";
                    }

                }
                else
                {
                    actionName = "Login";
                    nameController = "Login";
                    TempData["loginError"] = "Please enter your employee number and password.";
                }
                return RedirectToAction(actionName, nameController);


            }


            if (ModelState.IsValid)
            {
                if (objData.IsValid(empleado, usuario.Contrasena, usuario))
                {
                    //FormsAuthentication.SetAuthCookie(usuario.Nombres, usuario.Contrasena);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Login incorrecto!");
                }
            }
            return View(usuario);

        }

        public ActionResult IniciarSesion()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}