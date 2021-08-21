using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CART.Models
{
    public static class Operations
    {
        [WebMethod(EnableSession = true)]
        public static Models.CartList GetCurrentCart()
        {
            if(System.Web.HttpContext.Current != null)
            {
                if(System.Web.HttpContext.Current.Session["Cart"] == null)
                {
                    var order = new CartList();
                    System.Web.HttpContext.Current.Session["Cart"] = order;
                }
                return (CartList)System.Web.HttpContext.Current.Session["Cart"];
            }
            else
            {
                throw new InvalidOperationException("");
            }
        }
    }
}