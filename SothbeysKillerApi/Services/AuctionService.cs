﻿using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public interface IAuctionService
{
    List<AuctionResponse> GetPastAuctions();
    List<AuctionResponse> GetActiveAuctions();
    List<AuctionResponse> GetFutureAuctions();
    Guid CreateAuction(AuctionCreateRequest request);
    AuctionResponse GetAuctionById(Guid id);
    void UpdateAuction(Guid id, AuctionUpdateRequest request);
    void DeleteAuction(Guid id);
}

public class AuctionService : IAuctionService
{
    public static List<Auction> auctionsStorage = [];

    public List<AuctionResponse> GetPastAuctions()
    {
        var auctions = auctionsStorage
            .Where(a => a.Finish < DateTime.Now)
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .OrderByDescending(a => a.Start)
            .ToList();

        return auctions;
    }
    
    public List<AuctionResponse> GetActiveAuctions()
    {
        var auctions = auctionsStorage
            .Where(a => a.Start < DateTime.Now && a.Finish > DateTime.Now)
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .OrderByDescending(a => a.Start)
            .ToList();

        return auctions;
    }
    
    public List<AuctionResponse> GetFutureAuctions()
    {
        var auctions = auctionsStorage
            .Where(a => a.Start > DateTime.Now)
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .OrderByDescending(a => a.Start)
            .ToList();

        return auctions;
    }

    public Guid CreateAuction(AuctionCreateRequest request)
    {
        if (request.Title.Length < 3 || request.Title.Length > 255)
        {
            throw new ArgumentException();
        }

        if (request.Start < DateTime.Now)
        {
            throw new ArgumentException();
        }

        if (request.Finish <= request.Start)
        {
            throw new ArgumentException();
        }
        
        var auction = new Auction()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Start = request.Start,
            Finish = request.Finish
        };
        
        auctionsStorage.Add(auction);

        return auction.Id;
    }

    public AuctionResponse GetAuctionById(Guid id)
    {
        var auction = auctionsStorage.FirstOrDefault(a => a.Id == id);

        if (auction is not null)
        {
            var response = new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish);
            
            return response;
        }

        throw new NullReferenceException();
    }

    public void UpdateAuction(Guid id, AuctionUpdateRequest request)
    {
        var auction = auctionsStorage.FirstOrDefault(a => a.Id == id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }

        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }
        
        if (request.Start < DateTime.Now)
        {
            throw new ArgumentException();
        }

        if (request.Finish <= request.Start)
        {
            throw new ArgumentException();
        }

        auction.Start = request.Start;
        auction.Finish = request.Finish;
    }

    public void DeleteAuction(Guid id)
    {
        var auction = auctionsStorage.FirstOrDefault(a => a.Id == id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        auctionsStorage.Remove(auction);

    }
}