using System.Security.Claims;
using AuthenticationTest.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationTest.Controllers;

[ApiController]
[Authorize]
[Route("[controller]/[action]")]
public class UserController
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<LoginDetail> GetAuthenticatedUserDetail()
    {
        var claimsPrincipal = _httpContextAccessor.HttpContext.User;
        var firebaseId = claimsPrincipal.Claims.First(x => x.Type == "user_id").Value;
        var email = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.Email).Value;
        
        return new()
        {
            FirebaseId = firebaseId,
            Email = claimsPrincipal.Claims.First(x => x.Type == ClaimTypes.Email).Value,
            AspNetIdentityId = _userManager.FindByEmailAsync(email).Result.Id,
            RespondedAt = DateTime.Now,
        };
    }
}