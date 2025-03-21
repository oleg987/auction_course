using System.Data;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;

public class DbAuctionRepository : IAuctionRepository, IDisposable
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<DbAuctionRepository> _logger;

    public DbAuctionRepository(ILogger<DbAuctionRepository> logger)
    {
        _logger = logger;
        _dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=auction_db;Username=postgres;Password=123456");
        _dbConnection.Open();
        _logger.LogInformation($"{DateTime.Now}: connection state: {_dbConnection.State}.");
    }
    
    public IEnumerable<Auction> GetPast()
    {
        var query = @"select * from auctions                         
                        where finish < current_date
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query);

        return auctions;
    }

    public IEnumerable<Auction> GetActive()
    {
        using var dbConnection = new NpgsqlConnection("Server=localhost;Port=5432;Database=auction_db;Username=postgres;Password=123456");

        
        var query = @"select * from auctions                         
                        where start < current_date and finish > current_date 
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query);

        return auctions;
    }

    public IEnumerable<Auction> GetFuture()
    {
        var query = @"select * from auctions                         
                        where start > current_date
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query);

        return auctions;
    }

    public Auction? GetById(Guid id)
    {
        var query = "select * from auctions where id = @Id;";
        
        var auction = _dbConnection.QuerySingleOrDefault<Auction>(query, new { Id = id });

        return auction;
    }

    public Auction Create(Auction entity)
    {
        var command = $@"insert into auctions (id, title, start, finish) values (@Id, @Title, @Start, @Finish) returning *;";
        
        var auction = _dbConnection.QueryFirst<Auction>(command, entity);

        return auction;
    }

    public Auction? Update(Auction entity)
    {
        var updateCommand = "update auctions set start = @Start, finish = @Finish where id = @Id;";

        var auction = _dbConnection.QueryFirst<Auction>(updateCommand, entity);

        return auction;
    }

    public void Delete(Guid id)
    {
        var deleteCommand = "delete from auctions where id = @Id;";

        _dbConnection.ExecuteScalar(deleteCommand, new { Id = id });
    }

    public void Dispose()
    {
        _logger.LogInformation($"{DateTime.Now}: {nameof(DbAuctionRepository)} disposed.");
        _dbConnection.Dispose();
        
        _logger.LogInformation($"{DateTime.Now}: connection state: {_dbConnection.State}.");
    }
}