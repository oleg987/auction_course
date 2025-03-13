using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Services;

namespace SothbeysKillerApi.Controllers;

public record CreateLotRequest(Guid AuctionId, string Title, string Description, decimal StartPrice, decimal PriceStep);
public record ModifyLotRequest(Guid Id, string Title, string Description, decimal StartPrice, decimal PriceStep);
public record LotResponse(Guid Id, Guid AuctionId, string Title, string Description, decimal StartPrice, decimal PriceStep);
public class Lot
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal StartPrice { get; set; }
    public decimal PriceStep { get; set; }
}

[ApiController]
[Route("api/v1/[controller]")]
public class LotController : ControllerBase
{
    private readonly ILotService _lotService = new LotService();
    
    [HttpGet("{lotId:guid}")]
    public IActionResult LotInfoById(Guid lotId)
    {
        try
        {
            var lot = _lotService.LotInfoById(lotId);
            return Ok(lot);
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
    }
    
    [HttpGet]
    public IActionResult GetLotsByAuctionId(Guid auctionId)
    {
        try
        {
            var lots = _lotService.GetLotsByAuctionId(auctionId);
            return Ok(lots);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
        
    }

    public IActionResult CreateLot(CreateLotRequest request)
    {
        try
        {
            var lot = _lotService.CreateLot(request);
            return Ok(lot);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    public IActionResult ModifyLotById(ModifyLotRequest request)
    {
        try
        {
            _lotService.ModifyLotById(request);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
    }

    public IActionResult DeleteLotById(Guid lotId)
    {
        try
        {
            _lotService.DeleteLotById(lotId);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
        catch (NullReferenceException)
        {
            return NotFound();
        }
    }
}