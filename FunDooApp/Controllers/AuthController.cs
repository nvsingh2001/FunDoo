using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace FunDooApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IUserService userService, ILogger<AuthController> logger): ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> LoginAsync([FromBody] LoginRequestDto loginRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            return BadRequest(new ApiResponse<LoginResponseDto>(
                false,
                "Validation Error",
                errors
            ));
        }

        try
        {
            var loginResponse = await userService.LoginUserAsync(loginRequestDto);

            var response = new ApiResponse<LoginResponseDto>(
                true,
                "User logged in",
                loginResponse
            );

            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning($"Error while logging in {ex.Message}");
            return Unauthorized(new ApiResponse<LoginResponseDto>(
                    false,
                    ex.Message
                )
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating user: {ex.Message}");
            return StatusCode(500, new ApiResponse<LoginResponseDto>(
                false, 
                "An error occurred while logging in")
            );
        }
    }
}
