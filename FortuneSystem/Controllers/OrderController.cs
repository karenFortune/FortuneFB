using FortuneSystem.Models.OrderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FortuneSystem.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult PlaceOrder()
        {
            List<OrderModel> objOrder = new List<OrderModel>()
            {
                 new OrderModel {ProductCode="AOO1",ProductName="Windows Mobile",Qty=1,Price=45550.00,TotalAmount=45550.00 },
                new OrderModel {ProductCode="A002",ProductName="Laptop",Qty=1,Price=67000.00,TotalAmount=67000.00 },
                new OrderModel {ProductCode="A003",ProductName="LCD Television",Qty=2,Price=15000.00,TotalAmount=30000.00 },
                new OrderModel {ProductCode="A004",ProductName="CD Player",Qty=4,Price=10000.00,TotalAmount=40000.00 }
            };

            OrderDetail ObjOrderDetails = new OrderDetail();
            ObjOrderDetails.OrderDetails = objOrder;
            return View(ObjOrderDetails);
        }
        /// <summary>  
        /// Get list of records from view  
        /// </summary>  
        /// <param name="Order"></param>  
        /// <returns></returns>  
        [HttpPost]
        public ActionResult PlaceOrder(OrderDetail Order)
        {


            return View();
        }
    }
}