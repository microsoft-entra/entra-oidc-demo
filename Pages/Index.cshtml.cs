using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace entra_oidc_demo.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    public string AccessToken { get; set; }
    public bool ShowEdit { get; set; }


    public IndexModel(ILogger<IndexModel> logger, ITokenAcquisition tokenAcquisition)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        AccessToken = "";
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (this.Request.Cookies.ContainsKey("source"))
        {
            string source = this.Request.Cookies["source"];
            string currentPage = this.Request.Scheme + "://" + Request.Host + Request.Path;

            if (source != "" && source!.ToLower().Contains("microsoft") == false && source.ToLower() != currentPage.ToLower())
            {
                Response.Cookies.Delete("source");
                return new RedirectResult(source);
            }
        }

        try
        {
            this.AccessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "api://4ec7fcdd-1ef9-47fc-a60c-9ef3ac716cb9/Read" });
            ViewData["access_token"] = this.AccessToken;


            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = handler.ReadJwtToken(this.AccessToken);

            List<Claim> roles = jwtSecurityToken.Claims
                .Where(x => x.Type == "roles").ToList<Claim>();

            foreach (Claim role in roles)
            {
                if (roles != null && (role.Value.ToLower().Contains("headteacher")))
                {
                    ShowEdit = true;
                    break;
                }
            }

        }
        catch (System.Exception ex)
        {

        }

        return Page();
    }
}
