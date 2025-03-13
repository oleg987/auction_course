using System.Data;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public class DbAuctionService : IAuctionService
{
    private static List<Auction> _storage = [];

    private readonly IDbConnection _dbConnection;

    public DbAuctionService()
    {
        _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=auction_db;Username=postgres;Password=123456");
        _dbConnection.Open();
    }

    public List<AuctionResponse> GetPastAuctions()
    {
        var auctions = _storage
            .Where(a => a.Finish < DateTime.Now)
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .OrderByDescending(a => a.Start)
            .ToList();

        return auctions;
    }
    
    public List<AuctionResponse> GetActiveAuctions()
    {
        var auctions = _storage
            .Where(a => a.Start < DateTime.Now && a.Finish > DateTime.Now)
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .OrderByDescending(a => a.Start)
            .ToList();

        return auctions;
    }
    
    public List<AuctionResponse> GetFutureAuctions()
    {
        var auctions = _storage
            .Where(a => a.Start > DateTime.Now)
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .OrderByDescending(a => a.Start)
            .ToList();

        return auctions;
    }

    public Guid CreateAuction(AuctionCreateRequest request)
    {
        if (request.Title.Length < 3 || request.Title.Length > 150)
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

        // sql injection
        
        var command = "insert into auctions (id, title, start, finish) values (@Id, @Title, @Start, @Finish) returning id;";
        
        var id = _dbConnection.ExecuteScalar<Guid>(command, auction);

        return id;
    }

    public AuctionResponse GetAuctionById(Guid id)
    {
        var auction = _storage.FirstOrDefault(a => a.Id == id);

        if (auction is not null)
        {
            var response = new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish);
            
            return response;
        }

        throw new NullReferenceException();
    }

    public void UpdateAuction(Guid id, AuctionUpdateRequest request)
    {
        var auction = _storage.FirstOrDefault(a => a.Id == id);
        
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
        var auction = _storage.FirstOrDefault(a => a.Id == id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        _storage.Remove(auction);

    }
}