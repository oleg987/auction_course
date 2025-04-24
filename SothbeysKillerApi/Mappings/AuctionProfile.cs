using AutoMapper;
using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Mappings;

public class AuctionProfile : Profile
{
    public AuctionProfile()
    {
        CreateMap<AuctionCreateRequest, Auction>()
            .ForMember(
                d => d.Title,
                opt => opt.MapFrom(s => s.Title.ToUpper()));

        CreateProjection<Auction, AuctionResponse>();
    }
}