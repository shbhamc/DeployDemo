using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DemoFunctionApp;

public class DemoFunctions
{
    private readonly ILogger<DemoFunctions> _logger;

    public DemoFunctions(ILogger<DemoFunctions> logger)
    {
        _logger = logger;
    }

    [Function("ProfileDetailFunctions")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        _logger.LogInformation("ProfileDetailFunctions function processed a request.");
        var details = new
        {
            Name = "Admin",
            Age = 30,
            Email = "admin@noreply.com"
        };
        return new OkObjectResult(details);
    }

    [Function("ProfileFucntion")]
    public IActionResult Profile([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("ProfileFucntion HTTP trigger function processed a request.");
        var authHeader = req.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return new UnauthorizedResult();
        }

        var token = authHeader.Substring("Bearer ".Length);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("ThisIsMySecretKeyForJwtToken12345");

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(key),

                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);


            var details = new
            {
                Name = "Admin",
                Age = 30,
                Email = "admin@noreply.com"
            };
            return new OkObjectResult(details);
        }
        catch
        {
            return new UnauthorizedResult();
        }
    }
}