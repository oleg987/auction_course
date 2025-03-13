using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Services;

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
    private readonly IBidService _bidService = new BidService();
    
    [HttpGet("{lotId:guid}")]
    public IActionResult GetBidsByLotId(Guid lotId)
    {
        try
        {
            var bids = _bidService.GetBidsByLotId(lotId);
            return Ok(bids);
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
    }
    
    [HttpPost]
    public IActionResult PlaceBidOnLot(PlaceBidOnLotRequest request)
    {
        try
        {
            var response = _bidService.PlaceBidOnLot(request);
            return Ok(response);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
}