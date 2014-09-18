using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pipelines.RenderField;
using Sitecore.Xml.Xsl;

namespace ScPlums.WebEdit.Pipelines
{
    public class GetWrappedInTimeTagDateFieldValue : GetDateFieldValue
    {
        protected override DateRenderer CreateRenderer()
        {
            return new TimeTagDateRenderer();
        }
    }
}
