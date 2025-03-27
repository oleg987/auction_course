using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Repository;

namespace SothbeysKillerApi.Services;

public class DbAuctionService : IAuctionService
{
    private readonly IUnitOfWork _unitOfWork;

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

        var created = _unitOfWork.AuctionRepository.Create(auction);

        var historyRecord = new AuctionHistory()
        {
            AuctionId = created.Id,
            CreatedAt = DateTime.Now
        };

        var historyRecordId = _unitOfWork.AuctionHistoryRepository.Create(historyRecord);

        if (historyRecordId % 2 == 0)
        {
            _unitOfWork.Rollback();
            throw new Exception();
        }
        else
        {
            _unitOfWork.Commit();
            return created.Id;
        }
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
        var auction = _unitOfWork.AuctionRepository.GetById(id);
        
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

        _unitOfWork.AuctionRepository.Update(auction);
        
        _unitOfWork.Commit();
    }

    public void DeleteAuction(Guid id)
    {
        var auction = _unitOfWork.AuctionRepository.GetById(id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        _unitOfWork.AuctionRepository.Delete(id);
        
        _unitOfWork.Commit();
    }
}