using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CART.Models
{
    public class TempProducts
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public static List<TempProducts> GetTempProducts()
        {
            List<TempProducts> result = new List<TempProducts>();

            result.Add(new TempProducts
            {
                ID = 1,
                Name = "自動鉛筆",
                Price = 30.0m
            });

            result.Add(new TempProducts
            {
                ID = 2,
                Name = "記事本",
                Price = 50.0m
            });

            result.Add(new TempProducts
            {
                ID = 3,
                Name = "橡皮擦",
                Price = 10.0m
            });

            return result;
        }
    }

    public partial class Order
    {
        public string GetUserName()
        {
            using (Models.UserEntities db = new Models.UserEntities())
            {
                var result = db.AspNetUsers.FirstOrDefault(e => e.Id == this.UserId).UserName;

                return result;
            }
        }

        public string GetUserId()
        {
            return this.UserId;
        }
    }
}