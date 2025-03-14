using System.Data;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public class DbAuctionService : IAuctionService
{
    private readonly IDbConnection _dbConnection;

    public DbAuctionService()
    {
        _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=auction_db;Username=postgres;Password=123456");
        _dbConnection.Open();
    }

    public List<AuctionResponse> GetPastAuctions()
    {
        var query = @"select * from auctions                         
                        where finish < current_date
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query);

        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
    }
    
    public List<AuctionResponse> GetActiveAuctions()
    {
        var query = @"select * from auctions                         
                        where start < current_date and finish > current_date 
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query);

        return auctions
            .Select(auction => new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish))
            .ToList();
    }
    
    public List<AuctionResponse> GetFutureAuctions()
    {
        var query = @"select * from auctions                         
                        where start > current_date
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query);

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

        // sql injection
        
        var command = $@"insert into auctions (id, title, start, finish) values (@Id, @Title, @Start, @Finish) returning id;";
        
        var id = _dbConnection.ExecuteScalar<Guid>(command, auction);

        return id;
    }

    public AuctionResponse GetAuctionById(Guid id)
    {
        var query = "select * from auctions where id = @Id;";

        try
        {
            var auction = _dbConnection.QuerySingleOrDefault<Auction>(query, new { Id = id });

            if (auction is null)
            {
                throw new NullReferenceException();
            }
            
            var response = new AuctionResponse(auction.Id, auction.Title, auction.Start, auction.Finish);
            
            return response;
        }
        catch (Exception e)
        {
            throw new NullReferenceException();
        }
    }

    public void UpdateAuction(Guid id, AuctionUpdateRequest request)
    {
        var select = "select * from auctions where id = @Id;";
        
        var auction = _dbConnection.QuerySingleOrDefault<Auction>(select, new { Id = id });
        
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

        var updateCommand = "update auctions set start = @Start, finish = @Finish where id = @Id;";

        _dbConnection.ExecuteScalar(updateCommand, new { Id = id, Start = request.Start, Finish = request.Finish });
    }

    public void DeleteAuction(Guid id)
    {
        var select = "select * from auctions where id = @Id;";
        
        var auction = _dbConnection.QuerySingleOrDefault<Auction>(select, new { Id = id });
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        var deleteCommand = "delete from auctions where id = @Id;";

        _dbConnection.ExecuteScalar(deleteCommand, new { Id = id });
    }
}