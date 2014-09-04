using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Pipelines;
using Sitecore.Sites;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace ScPlums.Fixes.NewsMover
{
    public class RedirectWebEdit
    {
        public void Process(PipelineArgs args)
        {
            if (Sitecore.Context.ClientPage.ClientRequest.Parameters != "webedit:save")
            {
                return;
            }
            if (args.GetType().FullName != "Sitecore.Sharedsource.NewsMover.Pipelines.MoveCompletedArgs")
            {
                return;
            }

            var articleItem = (Item) args.GetType().GetProperty("Article").GetValue(args);
            var site = SiteContext.GetSite(WebEditUtil.SiteName);

            using (new SiteContextSwitcher(site))
            {
                using (new LanguageSwitcher(articleItem.Language))
                {
                    var targetUrl = LinkManager.GetItemUrl(articleItem);
                    SheerResponse.Eval("scNavigate(\"" + targetUrl + "\")");
                }
            }
        }
    }
}
