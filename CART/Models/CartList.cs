using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CART.Models
{
    [Serializable]
    public class CartList : List<CartItem>
    {
        public CartList()
        {
            this.cartItems = new List<CartItem>();
        }

        public List<CartItem> cartItems;

        public int Count
        {
            get
            {
                return this.cartItems.Count;
            }
        }
        public decimal TotalAmount
        {
            get
            {
                decimal totalAmount = 0.0m;
                foreach(var cartItem in this.cartItems)
                {
                    totalAmount += cartItem.Amount;
                }
                return totalAmount;
            }
        }

        public bool AddProduct(int ProductId)
        {
            var findItem = this.cartItems
                .Where(e => e.Id == ProductId)
                .Select(e => e)
                .FirstOrDefault();

            if (findItem == default(Models.CartItem))
            {
                using (Models.CARTEntities db = new CARTEntities())
                {
                    var product = db.Products.FirstOrDefault(e => e.Id == ProductId);
                    if (product != default(Models.Product))
                    {
                        this.AddProduct(product);
                    }
                }
            }
            else
            {
                findItem.Quantity += 1;
            }
            return true;
        }

        private bool AddProduct(Product product)
        {
            var cartItem = new Models.CartItem()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = 1,
                ImageURL = product.DefaultImageURL
            };
            this.cartItems.Add(cartItem);
            return true;
        }

        public bool RemoveProduct(int ProductId)
        {
            var findItem = this.cartItems
                .Where(e => e.Id == ProductId)
                .Select(e => e)
                .FirstOrDefault();

            if (findItem == default(Models.CartItem))
            {

            }
            else
            {
                this.cartItems.Remove(findItem);
            }
            return true;
        }

        public bool ClearCart()
        {
            this.cartItems.Clear();
            return true;
        }

        public List<Models.OrderDetail> ToOrderDetailList(int orderId)
        {
            var result = new List<Models.OrderDetail>();
            foreach (var cartItem in this.cartItems)
            {
                result.Add(new Models.OrderDetail()
                {
                    Name = cartItem.Name,
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    OrderId = orderId
                });
            }
            return result;
        }
    }
}