using System;
using System.Web;
using System.Web.WebPages;

namespace ScPlums.Grid
{
    public class RowPlaceholderContext
    {
        public Func<SpotHtmlContext, HelperResult> SpotRenderer { get; set; }
    }

    public class SpotHtmlContext
    {
        public int Index { get; set; }

        public int Count { get; set; }

        public IHtmlString SpotHtml { get; set; }
    }
}