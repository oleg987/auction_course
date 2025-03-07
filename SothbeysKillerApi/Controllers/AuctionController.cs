using Microsoft.AspNetCore.Mvc;

namespace SothbeysKillerApi.Controllers;

public record AuctionCreateRequest(string Title, DateTime Start, DateTime Finish);

public record AuctionUpdateRequest(DateTime Start, DateTime Finish);

public record AuctionResponse(Guid Id, string Title, DateTime Start, DateTime Finish);

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
        var auctions = _storage
            .Where(a => a.Start > DateTime.Now)
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .OrderByDescending(a => a.Start)
            .ToList();
        
        return Ok(auctions);
    }

    [HttpPost]
    public IActionResult Create(AuctionCreateRequest request)
    {
        if (request.Title.Length < 3 || request.Title.Length > 255)
        {
            return BadRequest();
        }

        if (request.Start < DateTime.Now)
        {
            return BadRequest();
        }

        if (request.Finish <= request.Start)
        {
            return BadRequest();
        }
        
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

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var auction = _storage.FirstOrDefault(a => a.Id == id);

        if (auction is not null)
        {
            var response = new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish);
            
            return Ok(response);
        }
        
        return NotFound();
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, AuctionUpdateRequest request)
    {
        var auction = _storage.FirstOrDefault(a => a.Id == id);
        
        if (auction is null)
        {
            return NotFound();
        }

        if (auction.Start <= DateTime.Now)
        {
            return BadRequest();
        }
        
        if (request.Start < DateTime.Now)
        {
            return BadRequest();
        }

        if (request.Finish <= request.Start)
        {
            return BadRequest();
        }

        auction.Start = request.Start;
        auction.Finish = request.Finish;
        
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        var auction = _storage.First(a => a.Id == id);
        
        if (auction is null)
        {
            return NotFound();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            return BadRequest();
        }

        _storage.Remove(auction);

        return NoContent();
    }
}