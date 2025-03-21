using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;

public interface IAuctionRepository
{
    IEnumerable<Auction> GetPast();
    IEnumerable<Auction> GetActive();
    IEnumerable<Auction> GetFuture();
    Auction? GetById(Guid id);
    Auction Create(Auction entity);
    Auction? Update(Auction entity);
    void Delete(Guid id);
}