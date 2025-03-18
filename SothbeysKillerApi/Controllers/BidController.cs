﻿using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Services;

namespace SothbeysKillerApi.Controllers;

public record CreateBidRequest(Guid LotId, Guid UserId, decimal Price);

public record BidResponse(string UserName, decimal Price, DateTime Timestamp);

public class Bid
{
    public Guid Id { get; set; }
    public Guid LotId { get; set; }
    public Guid UserId { get; set; }
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
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
    
    [HttpPost]
    public IActionResult PlaceBidOnLot(CreateBidRequest request)
    {
        try
        {
            var response = _bidService.CreateBid(request);
            return Ok(response);
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
}