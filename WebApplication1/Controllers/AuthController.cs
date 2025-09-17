using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Repositories;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly AuthService _authService;

    public AuthController(IUserRepository userRepo, AuthService authService)
    {
        _userRepo = userRepo;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _userRepo.GetByUserNameAsync(dto.UserName);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "نام کاربری یا رمز عبور اشتباه است" });

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { Token = token });
    }
}

