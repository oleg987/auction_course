using Microsoft.EntityFrameworkCore;
using SothbeysKillerApi.Contexts;
using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Exceptions;
using SothbeysKillerApi.Repository;

namespace SothbeysKillerApi.Services;

public class DbAuctionService : IAuctionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuctionDbContext _auctionDbContext;

    public DbAuctionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public List<AuctionResponse> GetPastAuctions()
    {
        var auctions = _unitOfWork.AuctionRepository.GetPast();

        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
    }
    
    public List<AuctionResponse> GetActiveAuctions()
    {
        var auctions = _unitOfWork.AuctionRepository.GetActive();
        
        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
    }
    
    public List<AuctionResponse> GetFutureAuctions()
    {
        var auctions = _unitOfWork.AuctionRepository.GetFuture();

        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
    }

    public Guid CreateAuction(AuctionCreateRequest request)
    {
        if (request.Title.Length < 3 || request.Title.Length > 255)
        {
            throw new AuctionValidationException(nameof(request.Title), "Invalid length.");
        }
        
        if (request.Start < DateTime.Now)
        {
            throw new AuctionValidationException(nameof(request.Start), "Invalid start.");
        }
        
        if (request.Finish <= request.Start)
        {
            throw new AuctionValidationException(nameof(request.Finish), "Invalid finish.");
        }
        
        var auction = new Auction()
        {
            Title = request.Title,
            Start = request.Start,
            Finish = request.Finish
        };

        _auctionDbContext.Type.Add(auction);

        _auctionDbContext.SaveChanges();
        
        return auction.Id;
    }
    
    public AuctionResponse GetAuctionById(Guid id)
    {
        var auction = _unitOfWork.AuctionRepository.GetById(id);

        if (auction is null)
        {
            throw new NullReferenceException();
        }
            
        var response = new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish);
            
        return response;
    }

    public void UpdateAuction(Guid id, AuctionUpdateRequest request)
    {
        var auction = _auctionDbContext.Type.FirstOrDefault(a => a.Id == id);
        
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

        _auctionDbContext.Type.Update(auction); // 1

        _auctionDbContext.Entry(auction).State = EntityState.Unchanged;
        
        auction.Start = request.Start;
        auction.Finish = request.Finish;

        _auctionDbContext.SaveChanges();
    }

    public void DeleteAuction(Guid id)
    {
        var auction = _auctionDbContext.Type.FirstOrDefault(a => a.Id == id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        _auctionDbContext.Type.Remove(auction);

        _auctionDbContext.SaveChanges();
    }
}