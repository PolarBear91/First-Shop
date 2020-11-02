using Shop.Domain.Infrastructure;
using System.Collections.Generic;

namespace Shop.Application.Cart
{
    [Service]
    public class GetCart
    {
        private ISessionManager _sessionManager;
        public GetCart(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public class Response
        {
            public string Name { get; set; }
            public ushort Value { get; set; }

            public ushort RealValue { get; set; }
            public int StockId { get; set; }
            public ushort Qty { get; set; }
        }

        public IEnumerable <Response> Do()
        {          
            return _sessionManager
                .GetCart(x => new Response
                {
                    Name = x.ProductName,
                    Value = x.Value,
                    RealValue = x.Value,
                    StockId = x.StockId,
                    Qty = x.Qty
                });
        }
    }
}

