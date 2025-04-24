using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using SothbeysKillerApi.Contexts;
using SothbeysKillerApi.Controllers;
using SothbeysKillerApi.Extensions;

namespace SothbeysKillerApi.Services;

// SOLID

// Single Responsibility Principle

// Decorator

public class DbAuctionService : IAuctionService
{
    private readonly AuctionDbContext _auctionDbContext;
    private readonly IValidator<Auction> _validator;
    private readonly IMapper _mapper;

    public DbAuctionService(AuctionDbContext auctionDbContext, IValidator<Auction> validator, IMapper mapper)
    {
        _auctionDbContext = auctionDbContext;
        _validator = validator;
        _mapper = mapper;
    }

    public List<AuctionResponse> GetPastAuctions()
    {
        throw new NotImplementedException();
    }
    
    public List<AuctionResponse> GetActiveAuctions()
    {
        throw new NotImplementedException();
    }
    
    public List<AuctionResponse> GetFutureAuctions()
    {
        throw new NotImplementedException();
    }

    public Guid CreateAuction(AuctionCreateRequest request)
    {
        var auction = _mapper.Map<Auction>(request);

        var validationResult = _validator.Validate(auction);
        
        validationResult.ThrowIfFail();

        _auctionDbContext.Type.Add(auction);

        _auctionDbContext.SaveChanges();
        
        return auction.Id;
    }
    
    public AuctionResponse GetAuctionById(Guid id)
    {
        var auction = _auctionDbContext.Type
            .Where(a => a.Id == id)
            .ProjectTo<AuctionResponse>(_mapper.ConfigurationProvider)
            .FirstOrDefault();

        if (auction is null)
        {
            throw new NullReferenceException();
        }
            
        return auction;
    }

    public void UpdateAuction(Guid id, AuctionUpdateRequest request)
    {
        var auction = _auctionDbContext.Type.FirstOrDefault(a => a.Id == id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }

        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        auction.Start = request.Start;
        auction.Finish = request.Finish;
        
        var validationResult = _validator.Validate(auction);

        validationResult.ThrowIfFail();

        _auctionDbContext.Type.Update(auction);

        _auctionDbContext.SaveChanges();
    }

    public void DeleteAuction(Guid id)
    {
        var auction = _auctionDbContext.Type.FirstOrDefault(a => a.Id == id);
        
        if (auction is null)
        {
            throw new NullReferenceException();
        }
        
        if (auction.Start <= DateTime.Now)
        {
            throw new ArgumentException();
        }

        _auctionDbContext.Type.Remove(auction);

        _auctionDbContext.SaveChanges();
    }
}