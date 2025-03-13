using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public interface IBidService
{
    List<BidResponse> GetBidsByLotId(Guid lotId);
    BidResponse CreateBid(CreateBidRequest request);
}

public class BidService : IBidService
{
    private static List<Bid> _bidsStorage = [];
    
    public List<BidResponse> GetBidsByLotId(Guid lotId)
    {
        var lot = LotService.lotsStorage.FirstOrDefault(lot => lot.Id == lotId);
        if (lot is null)
        {
            throw new ArgumentException();
        }
        
        var auction = AuctionService.auctionsStorage.FirstOrDefault(auction => auction.Id == lot.AuctionId);
        if (auction is null)
        {
            throw new ArgumentException();
        }
        
        if (auction.Start >= DateTime.Now)
        {
            throw new ArgumentException();
        }
        
        var bids = _bidsStorage.Where(bid => bid.LotId == lotId).ToList();
        
        if (!bids.Any())
        {
            return new List<BidResponse>();
        }
        
        var bidResponses = bids
            .Select(bid =>
            {
                var user = UserService.usersStorage.FirstOrDefault(user => user.Id == bid.UserId);
                return new BidResponse(user?.Name ?? "Невідомий користувач", bid.Price, bid.Timestamp);
            })
            .ToList();

        return bidResponses;
    }

    
    [HttpPost]
    public BidResponse CreateBid(CreateBidRequest request)
    {
        var lot = LotService.lotsStorage.FirstOrDefault(lot => lot.Id == request.LotId);
        if (lot is null)
        {
            throw new ArgumentException();
        }

        var user = UserService.usersStorage.FirstOrDefault(user => user.Id == request.UserId);
        if (user is null)
        {
            throw new ArgumentException();
        }
        
        var lastBidPrice = _bidsStorage
            .Where(bid => bid.LotId == request.LotId)
            .Select(bid => bid.Price)
            .DefaultIfEmpty(0)
            .Max();
        
        if (request.Price < lot.StartPrice)
        {
            throw new ArgumentException();
        }

        if (request.Price < lastBidPrice + lot.PriceStep)
        {
            throw new ArgumentException();
        }
        
        var bid = new Bid()
        {
            Id = Guid.NewGuid(),
            LotId = request.LotId,
            Price = request.Price,
            UserId = request.UserId
        };

        _bidsStorage.Add(bid);
        
        return new BidResponse(user.Name, request.Price, DateTime.Now);
    }

}