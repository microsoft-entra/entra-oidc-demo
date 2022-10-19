using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace entra_oidc_demo.Controllers
{
    [AllowAnonymous]
    [Area("MicrosoftIdentity")]
    [Route("[area]/[controller]/[action]")]
    public class MyAccountController : Controller
    {

        [HttpGet("{scheme?}")]
        public IActionResult SignIn([FromRoute] string scheme)
        {
            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
            var redirectUrl = Url.Content("~/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items["source"] = Request.Headers["Referer"];
            return Challenge(properties, scheme);
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignInWithUserReadAll([FromRoute] string scheme)
        {
            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
            var redirectUrl = Url.Content("~/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items["source"] = Request.Headers["Referer"];
            properties.Items["add_scope"] = "User.ReadBasic.All";
            return Challenge(properties, scheme);
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignInWithUserReadWriteAll([FromRoute] string scheme)
        {
            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
            var redirectUrl = Url.Content("~/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items["source"] = Request.Headers["Referer"];
            properties.Items["add_scope"] = "User.ReadWrite.All";
            return Challenge(properties, scheme);
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignInWithGroupReadAll([FromRoute] string scheme)
        {
            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
            var redirectUrl = Url.Content("~/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items["source"] = Request.Headers["Referer"];
            properties.Items["add_scope"] = "Group.Read.All";
            return Challenge(properties, scheme);
        }

        [HttpGet("{scheme?}")]
        public IActionResult SignInForBlobStorage([FromRoute] string scheme)
        {
            scheme ??= OpenIdConnectDefaults.AuthenticationScheme;
            var redirectUrl = Url.Content("~/");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items["source"] = Request.Headers["Referer"];
            properties.Items["add_scope"] = "https://storage.azure.com/user_impersonation";
            return Challenge(properties, scheme);
        }
    }
}

