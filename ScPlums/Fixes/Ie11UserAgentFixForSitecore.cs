using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace ScPlums.Fixes
{
    public class Ie11UserAgentFixForSitecore : HttpCapabilitiesDefaultProvider
    {
        public override HttpBrowserCapabilities GetBrowserCapabilities(HttpRequest request)
        {
            var browser = base.GetBrowserCapabilities(request);

            // FIX: Sitecore still checks for "IE", but new ASP.NET returns "InternetExplorer"
            if (browser.Browser == "InternetExplorer")
            {
                browser.Capabilities["browser"] = "IE";
                // refresh cached value
                typeof(HttpCapabilitiesBase)
                    .GetField("_havebrowser", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(browser, false);
            }

            return browser;
        }
    }
}
