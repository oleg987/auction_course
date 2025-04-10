using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SothbeysKillerApi.Entities;

namespace SothbeysKillerApi.Controllers;

public record SignUpRequest(string Email, string FullName, string Password);

public record SignInRequest(string Email, string Password);

public record SignInResponse(string AccessToken);

// Identity

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly UserManager<AuctionUser> _userManager;
    private readonly SignInManager<AuctionUser> _signInManager;
    private readonly ILogger<UserController> _logger;

    public UserController(UserManager<AuctionUser> userManager, SignInManager<AuctionUser> signInManager, ILogger<UserController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn(SignInRequest request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Unauthorized();
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

        if (result.Succeeded)
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qwertyuiopasdfghjklzxcvbnmm123456789"));
            var sign = new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("userId", user.Id.ToString())
            };

            var accessTokenLifetime = DateTime.UtcNow.AddMinutes(5);
        
            var token = new JwtSecurityToken(
                claims: claims,
                expires: accessTokenLifetime,
                signingCredentials: sign
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token)!;
            
            return Ok(new SignInResponse(accessToken));
        }

        return Unauthorized();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp(SignUpRequest request, CancellationToken ct)
    {
        var user = new AuctionUser()
        {
            FullName = request.FullName,
            Email = request.Email,
            UserName = request.Email
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }

    [HttpGet]
    [Authorize(Policy = "MyPolicy")]
    public async Task<IActionResult> Profile(CancellationToken ct)
    {
        foreach (var claim in User.Claims)
        {
            _logger.LogInformation($"{claim.Type} -> {claim.Value}");
        }
        
        return Ok("super secret data");
    }
}