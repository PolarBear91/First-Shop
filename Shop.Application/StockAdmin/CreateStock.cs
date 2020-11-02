﻿using Shop.Domain.Infrastructure;
using Shop.Domain.Models;
using System.Threading.Tasks;

namespace Shop.Application.StockAdmin
{
    [Service]
    public class CreateStock
    {
        private IStockManager _stockManager;

        public CreateStock(IStockManager stockManager)
        {
            _stockManager = stockManager;
        }

        public async Task<Response> Do(Request request)
        {
            var stock = new Stock
            {
                Description = request.Description,
                Qty = request.Qty,
                ProductId = request.ProductId

            };

            await _stockManager.CreateStock(stock);
       
            return new Response
            {
                Id = stock.Id,
                Description=stock.Description,
                Qty = stock.Qty
            };
        }

        public class Request
        {
            public int ProductId { get; set; }
            public string Description { get; set; }
            public ushort Qty { get; set; }

        }

        public class Response 
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public ushort Qty { get; set; }
        }

    }
}