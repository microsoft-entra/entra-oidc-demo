using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace entra_oidc_demo.Pages;

public class FilesModel : PageModel
{
    private readonly ILogger<FilesModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    public string AccessToken { get; set; }
    public string UPN { get; set; }

    public FilesModel(ILogger<FilesModel> logger, ITokenAcquisition tokenAcquisition)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        AccessToken = "";

    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (User.Identity!.IsAuthenticated)
        {
            try
            {
                this.AccessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://storage.azure.com/user_impersonation" });

                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(this.AccessToken);

                Claim scope = jwtSecurityToken.Claims
                    .Where(x => x.Type == "scp")
                    .FirstOrDefault()!;

                if (scope != null && !jwtSecurityToken.Audiences.Contains("https://storage.azure.com"))
                {
                    return new RedirectToActionResult("SignInForBlobStorage", "MyAccount", new { area = "MicrosoftIdentity" });
                }

                ViewData["access_token"] = this.AccessToken;
            }
            catch (System.Exception ex)
            {

            }
        }
        else
        {
            // If the user is not authenticated
            return new RedirectToActionResult("SignInForBlobStorage", "MyAccount", new { area = "MicrosoftIdentity" });
        }

        return Page();
    }
}
