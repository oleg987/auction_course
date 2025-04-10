using Microsoft.AspNetCore.Identity;

namespace SothbeysKillerApi.Entities;

public class AuctionUser : IdentityUser<Guid>
{
    public string FullName { get; set; }
}