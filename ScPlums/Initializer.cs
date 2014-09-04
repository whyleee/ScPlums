using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using ScPlums.Fixes;

namespace ScPlums
{
    public static class Initializer
    {
        public static void ApplyCodeFixes()
        {
            FixIE11SitecoreDetection();
        }

        private static void FixIE11SitecoreDetection()
        {
            HttpCapabilitiesBase.BrowserCapabilitiesProvider = new Ie11UserAgentFixForSitecore();
        }
    }
}
