using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace entra_oidc_demo.Pages;


public class SearchProfileWithScopeModel : PageModel
{
    private readonly ILogger<SearchProfileWithScopeModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    public string AccessToken { get; set; }
    public string UPN { get; set; }

    public SearchProfileWithScopeModel(ILogger<SearchProfileWithScopeModel> logger, ITokenAcquisition tokenAcquisition)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        AccessToken = "";
        
    }

    [AuthorizeForScopes(Scopes = new[] {"https://graph.microsoft.com/user.read.all"})]
    public async Task OnGetAsync()
    {
        if (User.Identity!.IsAuthenticated)
        {
            try
            {
                this.AccessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://graph.microsoft.com/user.read.all" });
                ViewData["access_token"] = this.AccessToken;
            }
            catch (System.Exception ex)
            {

            }

        }

        if (this.Request.Query.Keys.Contains("upn"))
        {
            this.UPN = Request.Query["upn"][0];
        }

    }
}
