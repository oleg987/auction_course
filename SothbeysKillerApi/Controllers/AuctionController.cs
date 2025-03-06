using Microsoft.AspNetCore.Mvc;

namespace SothbeysKillerApi.Controllers;

public record AuctionCreateRequest(string Title, DateTime Start, DateTime Finish);

public class Auction
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime Start { get; set; }
    public DateTime Finish { get; set; }
}

[ApiController]
[Route("api/v1/[controller]")]
public class AuctionController : ControllerBase
{
    private static List<Auction> _storage = []; // new List<AuctionCreateRequest>();
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult Past()
    {
        return Ok("its alive!");
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult Active()
    {
        return Ok("its alive!");
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult Future()
    {
        return Ok("its alive!");
    }

    [HttpPost]
    public IActionResult Create(AuctionCreateRequest request)
    {
        var auction = new Auction()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Start = request.Start,
            Finish = request.Finish
        };
        
        _storage.Add(auction);
        
        return Ok(new { Id = auction.Id });
    }
}