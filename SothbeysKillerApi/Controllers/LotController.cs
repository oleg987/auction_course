using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Services;

namespace SothbeysKillerApi.Controllers;

public record CreateLotRequest(Guid AuctionId, string Title, string Description, decimal StartPrice, decimal PriceStep);
public record ModifyLotRequest(string Title, string Description, decimal StartPrice, decimal PriceStep);
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
    
    [HttpGet("auction/{auctionId:guid}")]
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

    [HttpPost]
    public IActionResult CreateLot([Required] CreateLotRequest request)
    {
        try
        {
            var lotId = _lotService.CreateLot(request);
            return Ok(new { Id = lotId });
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }

    [HttpPut("{id:guid}")]
    public IActionResult ModifyLotById(Guid id, [FromBody] ModifyLotRequest request)
    {
        try
        {
            _lotService.ModifyLotById(id, request);
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
    
    [HttpDelete("{lotId:guid}")]
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