using Shop.Domain.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Application.Cart
{
    [Service]
    public class GetOrder
    {
        private ISessionManager _sessionManager;

        public GetOrder(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public class Response
        {
            public IEnumerable<Product> Products { get; set; }
            public CustomerInformation CustomerInformation { get; set; }

            public long GetTotalCharge() => Products.Sum(x => x.Value * x.Qty);
        }

        public class Product
        {
           public int ProductId { get; set; }
           public ushort Qty { get; set; }
           public int StockId { get; set; }
           public ushort Value { get; set; }
        }

        public class CustomerInformation
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
        }

        public Response Do()
        {
            var listOfProduct = _sessionManager
                .GetCart(x => new Product
                {
                    ProductId = x.ProductId,
                    StockId = x.StockId,
                    Value = (ushort)(x.Value * 100),
                    Qty = x.Qty
                });

            var customerInformation = _sessionManager.GetCustomerInformation();
            
            return new Response
            {

                Products = listOfProduct,
                CustomerInformation = new CustomerInformation
                {
                    FirstName = customerInformation.FirstName,
                    LastName = customerInformation.LastName,
                    Email = customerInformation.Email,
                    PhoneNumber = customerInformation.PhoneNumber,
                    Address1 = customerInformation.Address1,
                    Address2 = customerInformation.Address2,
                    City = customerInformation.City,
                    PostCode = customerInformation.PostCode
                }
            };
        }
    }
}
