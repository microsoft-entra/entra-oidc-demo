using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(options =>
        {
            options.Instance = builder.Configuration.GetValue<string>("AzureAd:Instance");
            options.TenantId = builder.Configuration.GetValue<string>("AzureAd:TenantId");
            options.ClientId = builder.Configuration.GetValue<string>("AzureAd:ClientId");
            options.ClientSecret = builder.Configuration.GetValue<string>("AzureAd:ClientSecret");
            options.CallbackPath = builder.Configuration.GetValue<string>("AzureAd:CallbackPath");
            options.Events ??= new OpenIdConnectEvents();
            options.Events.OnRedirectToIdentityProvider += context =>
            {
                // Read the custom add_scope parameter
                var add_scope = context.Properties.Items.FirstOrDefault(x => x.Key == "add_scope").Value;

                // If the add_scope exists, add the additional required scope
                if (add_scope != null)
                {

                    if (add_scope == "https://storage.azure.com/user_impersonation")
                    {
                        // If the required scope is Azure Blob storage, remove other scopes
                        //context.ProtocolMessage.Scope = "openid offline_access https://storage.azure.com/user_impersonation";
                    }
                    else
                    {
                        if (!context.ProtocolMessage.Scope.Contains(add_scope))
                            context.ProtocolMessage.Scope += " " + add_scope;
                    }
                }

                // 
                return Task.CompletedTask;
            };
        })
    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "https://graph.microsoft.com/user.read" })
    //.EnableTokenAcquisitionToCallDownstreamApi(new string[] { "https://storage.azure.com/user_impersonation" })
                    .AddInMemoryTokenCaches();

// builder.Services.AddAuthorization(options =>
// {
//     // By default, all incoming requests will be authorized according to the default policy.
//     options.FallbackPolicy = options.DefaultPolicy;
// });

builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI()
    .AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
// app.MapControllers();

app.UseEndpoints(endpoints =>
  {
      endpoints.MapControllers();
      // More here
  });

app.Run();
