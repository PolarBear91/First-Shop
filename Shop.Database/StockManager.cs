using System;
using Shop.Domain.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Infrastructure;
using System.Collections.Generic;

namespace Shop.Database
{
    public class StockManager : IStockManager
        {
            private ApplicationDbContext _ctx;

            public StockManager(ApplicationDbContext ctx)
            {
                _ctx = ctx;
            }

        public Task<int> CreateStock(Stock stock)
        {
            _ctx.Stock.Add(stock);
            return _ctx.SaveChangesAsync();
        }

        public Task<int> DeleteStock(int id)
        {
            var stock = _ctx.Stock.FirstOrDefault(x => x.Id == id);

            _ctx.Stock.Remove(stock);

            return _ctx.SaveChangesAsync();

        }

        public Task<int> UpdateStockRange(List<Stock> stocksList)
        {
            _ctx.Stock.UpdateRange(stocksList);
            return _ctx.SaveChangesAsync();
        }

        public bool EnoughStock(int stockId, int qty)
            {
                return _ctx.Stock.FirstOrDefault(x => x.Id == stockId).Qty >= qty;
            }

            public Stock GetStockWithProduct(int stockId)
            {
               return _ctx.Stock
                .Include(x => x.Product)
                .FirstOrDefault(x => x.Id == stockId);
            }

            public Task PutStockOnHold(int stockId, int qty, string sessionId)
            {
                _ctx.Stock.FirstOrDefault(x => x.Id == stockId).Qty -= (ushort)qty;

                var stockOnHold = _ctx.StockOnHold
                    .Where(x => x.SessionId == sessionId)
                    .ToList();


                if (stockOnHold.Any(x => x.StockId == stockId))
                {

                    stockOnHold.Find(x => x.StockId == stockId).Qty += (ushort)qty;
                }
                else
                {
                    _ctx.StockOnHold.Add(new StockOnHold
                    {
                        StockId = stockId,
                        SessionId = sessionId,
                        Qty = (ushort)qty,
                        ExpiryData = DateTime.Now.AddMinutes(20)
                    });
                }

                foreach (var stock in stockOnHold)
                {
                    stock.ExpiryData = DateTime.Now.AddMinutes(20);
                }

                return _ctx.SaveChangesAsync();
            }

        public Task RemoveStockOnHold(string sessionId)
        {
            var stocksOnHold = _ctx.StockOnHold
                .Where(x => x.SessionId == sessionId)
                .ToList();

            _ctx.StockOnHold.RemoveRange(stocksOnHold);

            return _ctx.SaveChangesAsync();
        }

        public Task RemoveStockOnHold(int stockId, int qty, string sessionId)
                {
                    var stockOnHold = _ctx.StockOnHold
                        .FirstOrDefault(x => x.StockId == stockId
                                   && x.SessionId == sessionId);
                    var stock = _ctx.Stock.FirstOrDefault(x => x.Id == stockId);
                    stock.Qty += (ushort)qty;
                    stockOnHold.Qty -= (ushort)qty;

                    if (stockOnHold.Qty <= 0)
                    {
                        _ctx.Remove(stockOnHold);
                    }

                    return _ctx.SaveChangesAsync();
                }

        public Task RetrieveExpiredStockOnHold()
        {
            var stocksOnHold = _ctx.StockOnHold.Where(x => x.ExpiryData < DateTime.Now).ToList();

            if (stocksOnHold.Count > 0)
            {
                var stockToReturn = _ctx.Stock.Where(x => stocksOnHold.Any(y => y.StockId == x.Id)).ToList();

                foreach (var stock in stockToReturn)
                {
                    stock.Qty = (ushort)(stock.Qty + stocksOnHold.FirstOrDefault(x => x.StockId == stock.Id).Qty);
                }

                _ctx.StockOnHold.RemoveRange(stocksOnHold);

                return _ctx.SaveChangesAsync();
            }

            return Task.CompletedTask;
        }    
    }   
}
