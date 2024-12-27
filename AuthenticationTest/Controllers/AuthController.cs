using System.Text.Json;
using AuthenticationTest.Models.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationTest.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class AuthController : Controller
{
    [HttpPost]
    public async Task<ActionResult> GetToken([FromForm] LoginInfo loginInfo)
    {
        string uri =
            "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key={{YOUR-API-KEY}}";
        using (HttpClient client = new HttpClient())
        {
            FireBaseLoginInfo fireBaseLoginInfo = new FireBaseLoginInfo
            {
                Email = loginInfo.Username,
                Password = loginInfo.Password
            };
            var result = await client.PostAsJsonAsync(uri, fireBaseLoginInfo,
                new JsonSerializerOptions()
                    { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            
            var encoded = await result.Content.ReadFromJsonAsync<GoogleToken>();
            Token token = new Token
            {
                token_type = "Bearer",
                access_token = encoded.idToken,
                id_token = encoded.idToken,
                expires_in = int.Parse(encoded.expiresIn),
                refresh_token = encoded.refreshToken
            };
            return Ok(token);
        }
    }
}