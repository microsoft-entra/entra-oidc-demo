using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace entra_oidc_demo.Pages;

public class ProfileEditModel : PageModel
{
    private readonly ILogger<ProfileEditModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    public string AccessToken { get; set; }

    public ProfileEditModel(ILogger<ProfileEditModel> logger, ITokenAcquisition tokenAcquisition)
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
                this.AccessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://graph.microsoft.com/.default" });

                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(this.AccessToken);

                // Get and validate the scope
                Claim scope = jwtSecurityToken.Claims
                    .Where(x => x.Type == "scp")
                    .FirstOrDefault()!;

                if (scope != null && (!scope.Value.ToLower().Contains("user.readwrite.all")))
                {
                    return new RedirectToActionResult("SignInWithUserReadWriteAll", "MyAccount", new { area = "MicrosoftIdentity" });
                }

                ViewData["access_token"] = this.AccessToken;
            }
            catch (System.Exception)
            {

            }
        }

        return Page();

    }
}
