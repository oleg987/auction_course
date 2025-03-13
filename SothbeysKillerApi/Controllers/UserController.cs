using Microsoft.AspNetCore.Mvc;
using SothbeysKillerApi.Services;

namespace SothbeysKillerApi.Controllers;

public record RegisterUserRequest(string Name, string Email, string Password);
public record LoginUserRequest(string Email, string Password);
public record LoginUserResponse(Guid Id, string Name, string Email);
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService = new UserService();
    
    [HttpPost]
    public IActionResult Signup(RegisterUserRequest request)
    {
        try
        {
            _userService.RegisterUser(request);
            return NoContent();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
    
    [HttpPost]
    public IActionResult Signin(LoginUserRequest request)
    {
        try
        {
            var response = _userService.LoginUser(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (ArgumentException)
        {
            return BadRequest();
        }
    }
    
}