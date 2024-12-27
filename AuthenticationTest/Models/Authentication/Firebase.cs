namespace AuthenticationTest.Models.Authentication;


public class LoginInfo
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class FireBaseLoginInfo
{
    public string Email { get; set; }
    public string Password { get; set; }
    public bool ReturnSecureToken { get; set; } = true;
}

public class GoogleToken
{
    public string kind { get; set; }
    public string localId { get; set; }
    public string email { get; set; }
    public string displayName { get; set; }
    public string idToken { get; set; }
    public bool registered { get; set; }
    public string refreshToken { get; set; }
    public string expiresIn { get; set; }
}

public class Token
{
    internal string refresh_token;

    public string token_type { get; set; }
    public int expires_in { get; set; }
    public int ext_expires_in { get; set; }
    public string access_token { get; set; }
    public string id_token { get; set; }
}

public class LoginDetail
{
    public string FirebaseId { get; set; }
    public string AspNetIdentityId { get; set; }
    public string Email { get; set; }
    public DateTime RespondedAt { get; set; }
}