using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using Sitecore.Web;

namespace ScPlums.Fixes
{
    public class RedirectBackFromLoginPage : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            var isEditOrPreviewMode = Sitecore.Context.PageMode.IsPageEditor || Sitecore.Context.PageMode.IsPreview;

            if (isEditOrPreviewMode && !Sitecore.Context.User.IsAuthenticated)
            {
                var currentUrl = args.Context.Request.RawUrl;
                var loginPageUrl = SiteContext.GetSite("shell").LoginPage;
                var redirectUrl = string.Format("{0}?returnUrl={1}", loginPageUrl, currentUrl);

                WebUtil.Redirect(redirectUrl, allowSame: false);
            }
        }
    }
}
