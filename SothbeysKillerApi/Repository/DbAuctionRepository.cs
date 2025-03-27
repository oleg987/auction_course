using System.Data;
using Dapper;
using Npgsql;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;

// Unit Of Work

public class DbAuctionRepository : IAuctionRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _transaction;

    public DbAuctionRepository(IDbConnection connection, IDbTransaction transaction)
    {
        _dbConnection = connection;
        _transaction = transaction;
    }
    
    public IEnumerable<Auction> GetPast()
    {
        var query = @"select * from auctions                         
                        where finish < current_date
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query, transaction: _transaction);

        return auctions;
    }

    public IEnumerable<Auction> GetActive()
    {
        var query = @"select * from auctions                         
                        where start < current_date and finish > current_date 
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query, transaction: _transaction);

        return auctions;
    }

    public IEnumerable<Auction> GetFuture()
    {
        var query = @"select * from auctions                         
                        where start > current_date
                        order by start desc;";

        var auctions = _dbConnection.Query<Auction>(query, transaction: _transaction);

        return auctions;
    }

    public Auction? GetById(Guid id)
    {
        var query = "select * from auctions where id = @Id;";
        
        var auction = _dbConnection.QuerySingleOrDefault<Auction>(query, new { Id = id }, transaction: _transaction);

        return auction;
    }

    public Auction Create(Auction entity)
    {
        var command = $@"insert into auctions (id, title, start, finish) values (@Id, @Title, @Start, @Finish) returning *;";
        
        var auction = _dbConnection.QueryFirst<Auction>(command, entity, transaction: _transaction);
        
        return auction;
    }

    public Auction? Update(Auction entity)
    {
        var updateCommand = "update auctions set start = @Start, finish = @Finish where id = @Id;";

        var auction = _dbConnection.QueryFirst<Auction>(updateCommand, entity, transaction: _transaction);

        return auction;
    }

    public void Delete(Guid id)
    {
        var deleteCommand = "delete from auctions where id = @Id;";

        _dbConnection.ExecuteScalar(deleteCommand, new { Id = id }, transaction: _transaction);
    }
}