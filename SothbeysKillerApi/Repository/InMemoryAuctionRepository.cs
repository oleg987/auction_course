using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Repository;

public class InMemoryAuctionRepository: IAuctionRepository
{
    public IEnumerable<Auction> GetPast()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Auction> GetActive()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Auction> GetFuture()
    {
        throw new NotImplementedException();
    }

    public Auction? GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Auction Create(Auction entity)
    {
        throw new NotImplementedException();
    }

    public Auction? Update(Auction entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }
}