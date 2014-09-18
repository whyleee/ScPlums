using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ScPlums.Aids
{
    internal static class SitecoreUtil
    {
        // Decompiled from "Sitecore.Pipelines.RenderField.RenderWebEditing", "Process" method.
        public static string GetWebEditingControlId(Field field)
        {
            Item item = field.Item;
            string text = item[FieldIDs.Revision].Replace("-", string.Empty);
            string controlID = string.Concat(new object[]
            {
                "fld_",
                item.ID.ToShortID(),
                "_",
                field.ID.ToShortID(),
                "_",
                item.Language,
                "_",
                item.Version,
                "_",
                text,
                "_",
                MainUtil.GetSequencer()
            });

            return controlID;
        }
    }
}
