using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FortuneSystem.Models.OrderModel
{
    public class OrderModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Int16 Qty { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
    }
    public class OrderDetail
    {
        /// <summary>  
        /// To hold list of orders  
        /// </summary>  
        public List<OrderModel> OrderDetails { get; set; }

    }
}