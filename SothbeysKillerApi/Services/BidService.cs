using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public interface IBidService
{
    List<BidResponse> GetBidsByLotId(Guid lotId);
    BidResponse PlaceBidOnLot(PlaceBidOnLotRequest request);
}

public class BidService : IBidService
{
    private static List<Bid> _storage = [];
    
    public List<BidResponse> GetBidsByLotId(Guid lotId)
    {
        var bid = _storage
            .Where(bid => bid.LotId == lotId)
            .Select(bid => new BidResponse(bid.UserName, bid.Price, bid.Timestamp))
            .OrderBy(bid => bid.Timestamp)
            .ToList();

        if (bid.Count == 0)
        {
            throw new NullReferenceException();
            
        }
            
        return bid;
    }
    
    [HttpPost]
    public BidResponse PlaceBidOnLot(PlaceBidOnLotRequest request)
    {
        if (request.UserName.Length < 3 || request.UserName.Length > 255)
        {
            throw new ArgumentException();
        }

        if (request.Price <= 0)
        {
            throw new ArgumentException();
        }
        
        var bid = new Bid()
        {
            LotId = request.LotId, 
            UserName = request.UserName, 
            Price = request.Price, 
            Timestamp = DateTime.UtcNow
        };
        
        _storage.Add(bid);
        
        var response = new BidResponse(bid.UserName, bid.Price, bid.Timestamp);
        
        return response;
    }
}