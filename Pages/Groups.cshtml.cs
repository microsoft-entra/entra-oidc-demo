using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace entra_oidc_demo.Pages;

public class GroupsModel : PageModel
{
    private readonly ILogger<GroupsModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    public string AccessToken { get; set; }
    public string UPN { get; set; }

    public GroupsModel(ILogger<GroupsModel> logger, ITokenAcquisition tokenAcquisition)
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

                Claim scope = jwtSecurityToken.Claims
                    .Where(x => x.Type == "scp")
                    .FirstOrDefault()!;

                if (scope != null && (!scope.Value.ToLower().Contains("group.read.all")))
                {
                    return new RedirectToActionResult("SignInWithGroupReadAll", "MyAccount", new { area = "MicrosoftIdentity" });
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
