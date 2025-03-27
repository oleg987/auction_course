using System.Data;
using Dapper;

namespace SothbeysKillerApi.Repository;

public class AuctionHistory
{
    public int Id { get; set; }
    public Guid AuctionId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface IAuctionHistoryRepository
{
    int Create(AuctionHistory entity);
}

public class AuctionHistoryRepository : IAuctionHistoryRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IDbTransaction _transaction;

    public AuctionHistoryRepository(IDbConnection dbConnection, IDbTransaction transaction)
    {
        _dbConnection = dbConnection;
        _transaction = transaction;
    }

    public int Create(AuctionHistory entity)
    {
        var command =
            "insert into auction_history (auction_id, created_at) values (@AuctionId, @CreatedAt) returning id;";

        return _dbConnection.ExecuteScalar<int>(command, entity, _transaction);
    }
}