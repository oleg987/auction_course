using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Services;

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
    private readonly IAuctionService _auctionService;

    public AuctionController(IAuctionService auctionService)
    {
        _auctionService = auctionService;
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult Past()
    {
        var auctions = _auctionService.GetPastAuctions();
        
        return Ok(auctions);
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult Active()
    {
        var auctions = _auctionService.GetActiveAuctions();
        
        return Ok(auctions);
    }
    
    [HttpGet]
    [Route("[action]")]
    public IActionResult Future()
    {
        var auctions = _auctionService.GetFutureAuctions();
        
        return Ok(auctions);
    }

    [HttpPost]
    public IActionResult Create(AuctionCreateRequest request)
    {
        try
        {
            var id = _auctionService.CreateAuction(request);
            
            return Ok(new { Id = id });
        }
        catch (Exception)
        {
            return StatusCode(500, "Oups...");
        }
        
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        try
        {
            var auction = _auctionService.GetAuctionById(id);

            return Ok(auction);
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, AuctionUpdateRequest request)
    {
        try
        {
            _auctionService.UpdateAuction(id, request);

            return NoContent();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        try
        {
            _auctionService.DeleteAuction(id);

            return NoContent();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
}