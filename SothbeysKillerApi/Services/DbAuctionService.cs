using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Repository;

namespace SothbeysKillerApi.Services;

public class DbAuctionService : IAuctionService
{
    private readonly IAuctionRepository _auctionRepository;
    
    public DbAuctionService(IAuctionRepository auctionRepository)
    {
        _auctionRepository = auctionRepository;
    }

    public List<AuctionResponse> GetPastAuctions()
    {
        var auctions = _auctionRepository.GetPast();

        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
    }
    
    public List<AuctionResponse> GetActiveAuctions()
    {
        var auctions = _auctionRepository.GetActive();

        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
    }
    
    public List<AuctionResponse> GetFutureAuctions()
    {
        var auctions = _auctionRepository.GetFuture();

        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
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

        var created = _auctionRepository.Create(auction);

        return created.Id;
    }

    public AuctionResponse GetAuctionById(Guid id)
    {
        var auction = _auctionRepository.GetById(id);

        if (auction is null)
        {
            throw new NullReferenceException();
        }
            
        var response = new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish);
            
        return response;
    }

    public void UpdateAuction(Guid id, AuctionUpdateRequest request)
    {
        var auction = _auctionRepository.GetById(id);
        
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

        _auctionRepository.Update(auction);
    }

    public void DeleteAuction(Guid id)
    {
        var auction = _auctionRepository.GetById(id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        _auctionRepository.Delete(id);
    }
}