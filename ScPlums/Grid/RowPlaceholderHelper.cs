using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.WebPages;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Helpers;
using Sitecore.Mvc.Presentation;

namespace ScPlums.Grid
{
    public static class RowPlaceholderHelper
    {
        private const string ROW_PL_NAME_PREFIX = "row_";
        private const string ROW_PL_KEY_REGEX = @"^(.*/row)_\w{8}$";

        public static IHtmlString RowPlaceholder(this SitecoreHelper helper, Func<SpotHtmlContext, HelperResult> spotRenderer = null)
        {
            using (ContextService.Get().Push(new RowPlaceholderContext
            {
                SpotRenderer = spotRenderer
            }))
            {
                return helper.Placeholder(GenerateUniqueRowPlaceholderName());
            }
        }

        private static string GenerateUniqueRowPlaceholderName()
        {
            // generate unique placeholder name like 'row_abcd1234'
            var uniqueId = RenderingContext.Current.Rendering.UniqueId.ToString().GetHashCode().ToString("x");
            return ROW_PL_NAME_PREFIX + uniqueId;
        }

        public static bool IsRow(string placeholderNameOrKey)
        {
            if (placeholderNameOrKey.Contains("/")) // key
            {
                return Regex.IsMatch(placeholderNameOrKey, ROW_PL_KEY_REGEX);
            }
            else // name
            {
                return placeholderNameOrKey.StartsWith(ROW_PL_NAME_PREFIX);
            }
            
        }

        public static string GetStaticKey(string rowPlaceholderKey)
        {
            return Regex.Replace(rowPlaceholderKey, ROW_PL_KEY_REGEX, "$1");
        }
    }
}