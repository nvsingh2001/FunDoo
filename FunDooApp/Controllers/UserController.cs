using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTOs;
using ModelLayer.Utilities;

namespace FunDooApp.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserService userService, ILogger<UserController> logger): ControllerBase
{
    /// <summary>
    /// Register a new User
    /// </summary>
    /// <param name="userRequestDto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> RegisterUser([FromBody] UserRequestDto userRequestDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            return BadRequest(new ApiResponse<UserResponseDto>(
                false,
                "Validation Error",
                errors
            ));
        }
        try
        {
            var createdUser = await userService.RegisterUserAsync(userRequestDto);

            var response = new ApiResponse<UserResponseDto>(
                true,
                "User registration successful",
                createdUser
            );

            return CreatedAtAction(nameof(GetUserById),
                new { id = createdUser.UserId },
                response
            );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning($"Conflict while creating user: {ex.Message}");
            return Conflict(new ApiResponse<UserResponseDto>(
                false, 
                ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating user: {ex.Message}");
            return StatusCode(500, new ApiResponse<UserResponseDto>(
                false, 
                "An error occurred while creating user")
            );
        }
    }

    /// <summary>
    /// Get User By Id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(int id)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(id);

            var response = new ApiResponse<UserResponseDto>(
                true,
                "User retrieved successfully",
                user
            );

            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogWarning($"Key not found: {ex.Message}");
            return NotFound(new ApiResponse<UserResponseDto>(
                    false,
                    ex.Message
                )
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error retrieving user: {ex.Message}");
            return StatusCode(500, new ApiResponse<UserResponseDto>(
                    false,
                    ex.Message
                )
            );
        }
    }
    
    /// <summary>
    /// Request Password Reset Link
    /// </summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse<string>))]
    public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromQuery] string email)
    {
        if (string.IsNullOrEmpty(email))
            return BadRequest(new ApiResponse<string>(
                    false,
                    "Email is required"
                )
            );

        try
        {
            await userService.ForgotPassword(email);
            return Ok(new ApiResponse<string>(
                    true,
                    "Password reset link sent to your email"
                )
            );
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning($"Forgot Password failed: {ex.Message}");
            return NotFound(new ApiResponse<string>(
                    false,
                    ex.Message
                )
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in ForgotPassword: {ex.Message}");
            return StatusCode(500, new ApiResponse<string>(
                    false,
                    "An error occurred"
                )
            );
        }
    }

    /// <summary>
    /// Reset Password using Token
    /// </summary>
    [HttpPost("reset-password")]
    [Authorize] 
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] string newPassword)
    {
        var purposeClaim = User.Claims.FirstOrDefault(c => c.Type == "Purpose")?.Value;
        if (purposeClaim != "PasswordReset")
        {
            return StatusCode(403, new ApiResponse<string>(
                    false,
                    "Invalid token type. This token cannot be used for password reset."
                )
            );
        }

        var userIdClaim = User.Claims.FirstOrDefault(c => System.Security.Claims.ClaimTypes.NameIdentifier == c.Type)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new ApiResponse<string>(
                    false,
                    "Invalid Token: User ID missing"
                )
            );
        }

        try
        {
            await userService.ResetPassword(userId, newPassword);
            return Ok(new ApiResponse<string>(
                    true,
                    "Password has been reset successfully"
                )
            );
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in ResetPassword: {ex.Message}");
            return StatusCode(500, new ApiResponse<string>(
                    false,
                    "An error occurred while resetting password"
                )
            );
        }
    }
}