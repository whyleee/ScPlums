using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Pipelines.RenderField;

namespace ScPlums.WebEdit.Pipelines
{
    public class GetDropTreeFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            if (args.FieldTypeKey != "droptree")
            {
                return;
            }

            args.DisableWebEditContentEditing = true;
            args.DisableWebEditFieldWrapping = true;

            args.Result.FirstPart = new SelectControl().Render(args);
        }
    }
}
