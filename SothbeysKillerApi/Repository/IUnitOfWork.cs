namespace SothbeysKillerApi.Repository;

public interface IUnitOfWork
{
    IAuctionRepository AuctionRepository { get; }
    IAuctionHistoryRepository AuctionHistoryRepository { get; }

    void Commit();

    void Rollback();
}