using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace entra_oidc_demo.Pages;

public class TerminalModel : PageModel
{
    private readonly ILogger<TerminalModel> _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    public string AccessTokenError { get; set; }
    public string AccessToken { get; set; }
    public bool ReadyForGraph { get; set; }

    // Query string parameters
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    private static readonly HttpClient client = new HttpClient();

    public TerminalModel(ILogger<TerminalModel> logger, ITokenAcquisition tokenAcquisition)
    {
        _logger = logger;
        _tokenAcquisition = tokenAcquisition;
        AccessToken = "";
        ReadyForGraph = false;
    }

    public void OnGetAsync()
    {
    }

    public async Task OnPostAsync()
    {
        // Get the HTTP POST parameters for resend (in case of error)
        if (this.Request.Form.Keys.Contains("TenantId"))
        {
            this.TenantId = Request.Form["TenantId"][0];
        }

        if (this.Request.Form.Keys.Contains("ClientId"))
        {
            this.ClientId = Request.Form["ClientId"][0];
        }

        if (this.Request.Form.Keys.Contains("ClientSecret"))
        {
            this.ClientSecret = Request.Form["ClientSecret"][0];
        }
        
        // Start client credentials flow
        var values = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", this.Request.Form["ClientId"] },
                { "client_secret", this.Request.Form["ClientSecret"] },
                { "scope", "https://graph.microsoft.com/.default" }
            };

        var content = new FormUrlEncodedContent(values);

        var response = await client.PostAsync($"https://login.microsoftonline.com/{this.Request.Form["tenantId"]}/oauth2/v2.0/token", content);

        var responseString = await response.Content.ReadAsStringAsync();

        // Get the access token from the IDP (Azure AD)
        if (responseString.Contains("access_token"))
        {
            this.AccessToken = (JsonSerializer.Deserialize<AccessTokenModel>(responseString)).access_token;
            ViewData["access_token"] = this.AccessToken;
            ReadyForGraph =  true;
        }
        else
        {
            // Return the error code
            this.AccessTokenError = responseString;
        }
    }

}

public class AccessTokenModel
{
    public string token_type { get; set; }
    public int expires_in { get; set; }
    public int ext_expires_in { get; set; }
    public string access_token { get; set; }
}

