using SothbeysKillerApi.Controllers;

namespace SothbeysKillerApi.Services;

public interface IUserService
{
    void RegisterUser(RegisterUserRequest request);
    LoginUserResponse LoginUser(LoginUserRequest request);
}

public class UserService : IUserService
{
    public static List<User> usersStorage = [];
    
    public void RegisterUser(RegisterUserRequest request)
    {
        if (request.Email.Length < 3 || request.Email.Length > 255)
        {
            throw new ArgumentException("Email must be between 3 and 255 characters");
        }

        if (request.Name.Length < 3 || request.Name.Length > 255)
        {
            throw new ArgumentException("Name must be between 3 and 255 characters");
        }

        if (request.Password.Length < 3 || request.Password.Length > 255)
        {
            throw new ArgumentException("Password must be between 3 and 255 characters");
        }
        
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            Password = request.Password
        };
        
        usersStorage.Add(user);
    }
    
    public LoginUserResponse LoginUser(LoginUserRequest request)
    {
        if (request.Email.Length < 3 || request.Email.Length > 255 || request.Password.Length < 3 ||
            request.Password.Length > 255)
        {
            throw new ArgumentException();
        }
        
        if (usersStorage.All(u => u.Email != request.Email))
        {
            throw new UnauthorizedAccessException();
        }
        
        var user = usersStorage
            .Where(u => u.Email == request.Email && u.Password == request.Password)
            .Select(user => new LoginUserResponse(user.Id, user.Name, user.Email))
            .FirstOrDefault();

        if (user is null)
        {
            throw new ArgumentException("Invalid password");
        }
        
        return user;
    }
}