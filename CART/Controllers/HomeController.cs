using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CART.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Index2(string id)
        {
            return Content(String.Format("id:{0}", id));
        }
        public ActionResult Index3()
        {
            //var result = Models.TempProducts.GetTempProducts();

            var resultFromDB = new List<Models.Product>();

            ViewBag.ResultMessage = TempData["ResultMessage"];

            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                resultFromDB = db.Products.Select(e => e).ToList();

                return View(resultFromDB);
            }
        }

        public ActionResult SellPage()
        {
            //var result = Models.TempProducts.GetTempProducts();

            var resultFromDB = new List<Models.Product>();

            ViewBag.ResultMessage = TempData["ResultMessage"];

            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                resultFromDB = db.Products.Select(e => e).ToList();

                return View(resultFromDB);
            }
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Models.Product postback)
        {
            if(this.ModelState.IsValid)
            {
                using (Models.CARTEntities db = new Models.CARTEntities())
                {
                    db.Products.Add(postback);

                    db.SaveChanges();

                    TempData["ResultMessage"] = String.Format("商品[{0}]成功建立", postback.Name);

                    return RedirectToAction("Index3");
                }
            }
            ViewBag.ResultMessage = "資料有誤，請檢查";

            return View(postback);
        }

        public ActionResult Edit(int id)
        {
            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                var result = db.Products.FirstOrDefault(e => e.Id == id);
                if( result != default(Models.Product))
                {
                    return View(result);
                }
                else
                {
                    TempData["ResultMessage"] = "資料有誤，請重新操作";
                    return RedirectToAction("Index3");
                }
            }
        }

        [HttpPost]
        public ActionResult Edit(Models.Product postback)
        {
            if (this.ModelState.IsValid)
            {
                using (Models.CARTEntities db = new Models.CARTEntities())
                {
                    var result = db.Products.FirstOrDefault(e => e.Id == postback.Id);

                    result.Id = result.Id;
                    result.Name = postback.Name;
                    result.Description = postback.Description;
                    result.CategoryId = postback.CategoryId;
                    result.Price = postback.Price;
                    result.PublishDate = postback.PublishDate;
                    result.Status = postback.Status;
                    result.DefaultImageId = postback.DefaultImageId;
                    result.Quantity = postback.Quantity;
                    result.DefaultImageURL = postback.DefaultImageURL;
                    db.SaveChanges();

                    TempData["ResultMessage"] = String.Format("商品[{0}]成功編輯", postback.Name);

                    return RedirectToAction("Index3");
                }
            }
            else
            {
                return View(postback);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id = 0)
        {
            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                var result = db.Products.FirstOrDefault(e => e.Id == id);
                if (result != default(Models.Product))
                {
                    db.Products.Remove(result);
                    db.SaveChanges();
                    TempData["ResultMessage"] = String.Format("商品[{0}]成功刪除", result.Name);
                }
                else
                {
                    TempData["ResultMessage"] = "指定資料不存在，無法刪除，請重新操作";
                }
                return RedirectToAction("Index3");
            }
        }

        public ActionResult memberList()
        {
            ViewBag.ResultMessage = TempData["ResultMessage"];

            using (Models.UserEntities db = new Models.UserEntities())
            {
                var result = db.AspNetUsers.Select(e => e).ToList();

                return View(result);
            }
        }

        public ActionResult memberEdit(string id)
        {
            using (Models.UserEntities db = new Models.UserEntities())
            {
                var result = db.AspNetUsers.FirstOrDefault(e => e.Id == id);
                if (result != default(Models.AspNetUser))
                {
                    return View(result);
                }
                else
                {
                    TempData["ResultMessage"] = "資料有誤，請重新操作";
                    return RedirectToAction("memberList");
                }
            }
        }

        [HttpPost]
        public ActionResult memberEdit(Models.AspNetUser postback)
        {
            if (this.ModelState.IsValid)
            {
                using (Models.UserEntities db = new Models.UserEntities())
                {
                    var result = db.AspNetUsers.FirstOrDefault(e => e.Id == postback.Id);
                    
                    result.UserName = postback.UserName;
                    result.Email = postback.Email;

                    db.SaveChanges();

                    TempData["ResultMessage"] = String.Format("會員[{0}]成功編輯", postback.UserName);

                    return RedirectToAction("memberList");
                }
            }
            else
            {
                return View(postback);
            }
        }

        public ActionResult GetCart()
        {
            var cart = Models.Operations.GetCurrentCart();
            cart.AddProduct(1);
            return Content(string.Format("目前購物車總共: {0}元",cart.TotalAmount));
        }

        public ActionResult GetCartPartial()
        {
            return PartialView("_CartPartial");
        }

        public ActionResult AddToCartPartial(int id)
        {
            var currentCart = Models.Operations.GetCurrentCart();
            currentCart.AddProduct(id);
            return PartialView("_CartPartial");
        }

        public ActionResult RemoveFromCart(int id)
        {
            var currentCart = Models.Operations.GetCurrentCart();
            currentCart.RemoveProduct(id);
            return PartialView("_CartPartial");
        }

        public ActionResult ClearCart()
        {
            var currentCart = Models.Operations.GetCurrentCart();
            currentCart.ClearCart();
            return PartialView("_CartPartial");
        }

        public ActionResult OrderIndex()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OrderIndex(Models.Ship postback)
        {
            if(this.ModelState.IsValid)
            {
                var currentcart = Models.Operations.GetCurrentCart();
                var identity = HttpContext.User.Identity;
                var userId = HttpContext.User.Identity.GetUserId();

                using (Models.CARTEntities db = new Models.CARTEntities())
                {
                    var order = new Models.Order()
                    {
                        UserId = userId,
                        RecieverName = postback.RecieverName,
                        RecieverPhone = postback.RecieverPhone,
                        RecieverAddress = postback.RecieverAddress
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    var orderDetails = currentcart.ToOrderDetailList(order.Id);

                    db.OrderDetails.AddRange(orderDetails);
                    db.SaveChanges();
                }
                return Content("訂購成功");
            }
            return View();
        }

        public ActionResult ManageOrderIndex(string userId, string userName)
        {
            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                if (!string.IsNullOrEmpty(userId))
                {
                    var result = db.Orders.Where(e => e.UserId == userId).ToList();
                    return View(result);
                }
                else if(!string.IsNullOrEmpty(userName))
                {
                    using (Models.UserEntities db2 = new Models.UserEntities())
                    {
                        var userFromName = db2.AspNetUsers.FirstOrDefault(u => u.UserName == userName);
                        if (userFromName != null)
                        {
                            var result = db.Orders.Where(e => e.UserId == userFromName.Id).ToList();
                            return View(result);
                        }
                        else
                        {
                            return View();
                        }
                    }
                }
                else
                {
                    var result = db.Orders.ToList();
                    return View(result);
                }
            }
        }

        public ActionResult ManageOrderDetails(int id)
        {
            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                var result = db.OrderDetails.Where(e => e.OrderId == id).ToList();

                if(result.Count == 0)
                {
                    return RedirectToAction("ManageOrderIndex");
                }
                else
                {
                    return View(result);
                }
                
            }
        }

        public ActionResult MyOrder()
        {
            var userId = HttpContext.User.Identity.GetUserId();
            
            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                var result = db.Orders.Where(e => e.UserId == userId).ToList();
                return RedirectToAction("ManageOrderIndex", new { userId = userId });
            }
        }

        public ActionResult Details(int id)
        {
            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                var result = db.Products.Where(e => e.Id == id).FirstOrDefault();

                if (result == default(Models.Product))
                {
                    return RedirectToAction("SellPage");
                }
                else
                {
                    return View(result);
                }
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddComment(int id, string content)
        {
            var userId = HttpContext.User.Identity.GetUserId();

            var comment = new Models.ProductComment()
            {
                Product = id.ToString(),
                Content = content,
                UserId = userId,
                CreateDate = DateTime.Now
            };

            using (Models.CARTEntities db = new Models.CARTEntities())
            {
                db.ProductComments.Add(comment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = id });
        }
    }
}
