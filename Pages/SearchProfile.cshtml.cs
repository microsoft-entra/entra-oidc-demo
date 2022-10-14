using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace entra_oidc_demo.Pages;

public class SearchProfileModel : PageModel
{
    private readonly ILogger<SearchProfileModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    public string AccessToken { get; set; }
    public string UPN { get; set; }

    public SearchProfileModel(ILogger<SearchProfileModel> logger, ITokenAcquisition tokenAcquisition)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        AccessToken = "";
        
    }

    public async Task OnGetAsync()
    {
        if (User.Identity!.IsAuthenticated)
        {
            try
            {
                this.AccessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://graph.microsoft.com/.default" });
                ViewData["access_token"] = this.AccessToken;
            }
            catch (System.Exception)
            {

            }

        }

        if (this.Request.Query.Keys.Contains("upn"))
        {
            this.UPN = Request.Query["upn"][0];
        }

    }
}
