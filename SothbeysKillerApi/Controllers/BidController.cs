using Microsoft.AspNetCore.Mvc;

namespace SothbeysKillerApi.Controllers;

public record PlaceBidOnLotRequest(Guid LotId, string UserName, decimal Price);

public record BidResponse(string UserName, decimal Price, DateTime Timestamp);

public class Bid
{
    public Guid LotId { get; set; }
    public string UserName { get; set; }
    public decimal Price { get; set; }
    public DateTime Timestamp { get; set; }
}

[ApiController]
[Route("api/v1/[controller]")]
public class BidController : ControllerBase
{
    private static List<Bid> _storage = [];
    
    [HttpGet("{lotId:guid}")]
    public IActionResult GetById(Guid lotId)
    {
        var bid = _storage
            .Where(bid => bid.LotId == lotId)
            .Select(bid => new BidResponse(bid.UserName, bid.Price, bid.Timestamp))
            .OrderBy(bid => bid.Timestamp)
            .ToList();

        if (bid.Count == 0)
        {
            return NotFound();
            
        }
            
        return Ok(bid);
    }
    
    [HttpPost]
    public IActionResult PlaceBidOnLot(PlaceBidOnLotRequest request)
    {
        if (request.UserName.Length < 3 || request.UserName.Length > 255)
        {
            return BadRequest();
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
        
        return Ok(response);
    }
}