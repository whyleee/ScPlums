using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Xml.Xsl;

namespace ScPlums.WebEdit
{
    public class TimeTagDateRenderer : DateRenderer
    {
        // TODO: make it configurable, whether I want to render the datetime in the <time> tag
        public override RenderFieldResult Render()
        {
            var result = base.Render();

            if (!string.IsNullOrEmpty(result.FirstPart))
            {
                var dateTime = DateUtil.IsoDateToDateTime(FieldValue, DateTime.MinValue);
                var validHtmlDateTime = dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mmzzz");

                result.FirstPart = string.Format("<time datetime=\"{0}\">{1}</time>",
                    validHtmlDateTime, result.FirstPart
                );
            }

            return result;
        }
    }
}
