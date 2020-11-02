using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Shop.Domain.Infrastructure;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.UI.Infrastructure
{
    public class SessionManager : ISessionManager
    {
        private const string KeyCart = "cart";
        private const string KeyCustomerInfo = "customer-info";
        private readonly ISession _session;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }

        public void AddCustomerInformation(CustomerInformation customer)
        {
            var stringObject = JsonConvert.SerializeObject(customer);
            _session.SetString(KeyCustomerInfo, stringObject);
        }

        public void AddProduct(CartProduct CartProduct)
        {
            var CartList = new List<CartProduct>();
            var stringObject = _session.GetString(KeyCart);

            if (!string.IsNullOrEmpty(stringObject))
            {
                CartList = JsonConvert.DeserializeObject<List<CartProduct>>(stringObject);
            }

            if (CartList.Any(x => x.StockId == CartProduct.StockId))
            {
                CartList.Find(x => x.StockId == CartProduct.StockId).Qty += CartProduct.Qty;
            }

            else
            {
                CartList.Add(CartProduct);
               
            }

            stringObject = JsonConvert.SerializeObject(CartList);
            _session.SetString(KeyCart, stringObject);

        }

        public IEnumerable <TResult> GetCart<TResult>(Func<CartProduct,TResult> selector)
        {
            var stringObject = _session.GetString(KeyCart);

            if (string.IsNullOrEmpty(stringObject))
                return new LinkedList<TResult>();

            var CartList = JsonConvert.DeserializeObject<IEnumerable<CartProduct>>(stringObject);

            return CartList.Select(selector);
        }

        public CustomerInformation GetCustomerInformation()
        {
            var stringObject = _session.GetString(KeyCustomerInfo);

            if (string.IsNullOrEmpty(stringObject))
                return null;

            var customerInformation = JsonConvert.DeserializeObject<CustomerInformation>(stringObject);

            return customerInformation;
        }

        public string GetId() => _session.Id;

        public void ClearCart()
        {
            _session.Remove(KeyCart);
        }

        public void RemoveProduct(int stockId, int qty)
        {
            var CartList = new List<CartProduct>();
            var stringObject = _session.GetString(KeyCart);

            if (string.IsNullOrEmpty(stringObject)) return;         

            CartList = JsonConvert.DeserializeObject<List<CartProduct>>(stringObject);

            if (!CartList.Any(x => x.StockId == stockId)) return;

            var product=CartList.First(x => x.StockId == stockId);
            product.Qty -= (ushort)qty;
            if(product.Qty<=0)
            {
                CartList.Remove(product);
            }

            stringObject = JsonConvert.SerializeObject(CartList);
            _session.SetString(KeyCart, stringObject);
        }
    }
}

