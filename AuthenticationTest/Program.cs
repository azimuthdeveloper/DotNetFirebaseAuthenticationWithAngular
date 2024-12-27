using AuthenticationTest.Context;
using AuthenticationTest.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


var client = new HttpClient();
var keys = client
    .GetStringAsync(
        "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com")
    .Result;
var originalKeys = new JsonWebKeySet(keys).GetSigningKeys();
var additionalkeys = client
    .GetStringAsync(
        "https://www.googleapis.com/service_accounts/v1/jwk/securetoken@system.gserviceaccount.com")
    .Result;
var morekeys = new JsonWebKeySet(additionalkeys).GetSigningKeys();
var totalkeys = originalKeys.Concat(morekeys);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=testauth.db"));
builder.Services.AddDefaultIdentity<ApplicationUser>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddHttpContextAccessor();


var projectId = "{{YOUR-PROJECT-ID}}";
// Configure Firebase Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.IncludeErrorDetails = true;
        options.Authority = $"https://securetoken.google.com/{projectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{projectId}",
            ValidateAudience = true,
            ValidAudience = projectId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = totalkeys
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                // Receive the JWT token that firebase has provided
                var firebaseToken = context.SecurityToken as Microsoft.IdentityModel.JsonWebTokens.JsonWebToken;
                // Get the Firebase UID of this user
                var firebaseUid = firebaseToken?.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                if (!string.IsNullOrEmpty(firebaseUid))
                {
                    // Use the Firebase UID to find or create the user in your Identity system
                    var userManager = context.HttpContext.RequestServices
                        .GetRequiredService<UserManager<ApplicationUser>>();

                    var user = await userManager.FindByNameAsync(firebaseUid);

                    if (user == null)
                    {
                        // Create a new ApplicationUser in your database if the user doesn't exist
                        user = new ApplicationUser
                        {
                            UserName = firebaseUid,
                            Email = firebaseToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                            FirebaseUserId = firebaseUid,
                            Name = firebaseToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ??
                                   $"Planner {firebaseUid}",
                        };
                        await userManager.CreateAsync(user);
                    }
                }
            }
        };
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,

        Flows = new OpenApiOAuthFlows
        {
            Password = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri("/v1/auth", UriKind.Relative),
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    { "returnSecureToken", new OpenApiBoolean(true) },
                },
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header,
            },
            new List<string> { "openid", "email", "profile" }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(x =>
{
    x.AllowAnyHeader();
    x.AllowAnyMethod();
    x.AllowAnyOrigin();
});

app.Run();