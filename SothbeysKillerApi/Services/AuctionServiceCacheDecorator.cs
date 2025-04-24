using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Infrastructure;

namespace SothbeysKillerApi.Services;

public class AuctionServiceCacheDecorator : IAuctionService
{
    private readonly DbAuctionService _service;
    private readonly IHybridCache _hybridCache;

    public AuctionServiceCacheDecorator(DbAuctionService service, IHybridCache hybridCache)
    {
        _service = service;
        _hybridCache = hybridCache;
    }

    public List<AuctionResponse> GetPastAuctions()
    {
        return _service.GetPastAuctions();
    }

    public List<AuctionResponse> GetActiveAuctions()
    {
        return _service.GetActiveAuctions();
    }

    public List<AuctionResponse> GetFutureAuctions()
    {
        return _service.GetFutureAuctions();
    }

    public Guid CreateAuction(AuctionCreateRequest request)
    {
        return _service.CreateAuction(request);
    }

    public AuctionResponse GetAuctionById(Guid id)
    {
        string key = $"auction_{id}";

        var response = _hybridCache.Get<AuctionResponse>(key);

        if (response is not null)
        {
            return response;
        }

        response = _service.GetAuctionById(id);

        _hybridCache.Set(key, response, new HybridCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
        });

        return response;
    }

    public void UpdateAuction(Guid id, AuctionUpdateRequest request)
    {
        _service.UpdateAuction(id, request);

        var response = _service.GetAuctionById(id);
        
        string key = $"auction_{id}";
        
        _hybridCache.Set(key, response, new HybridCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(45)
        });
    }

    public void DeleteAuction(Guid id)
    {
        _service.DeleteAuction(id);
        
        string key = $"auction_{id}";
        
        _hybridCache.Remove(key);
    }
}