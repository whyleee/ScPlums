using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using Sitecore.Shell.Applications.ContentEditor;

namespace ScPlums.ContentEditor
{
    public class HideEmptyOptionForRequiredFieldValueLookupEx : ValueLookupEx
    {
        public string FieldID { get; set; }

        // FIX: if required validator applied to the field - do not render empty option
        protected override void DoRender(HtmlTextWriter output)
        {
            var buffer = new HtmlTextWriter(new StringWriter());
            base.DoRender(buffer);

            var renderedHtml = buffer.InnerWriter.ToString();
            var fieldItem = Sitecore.Context.ContentDatabase.GetItem(FieldID);
            var requiredValidatorId = "{59D4EE10-627C-4FD3-A964-61A88B092CBC}";

            if (fieldItem["Quick Action Bar"].Contains(requiredValidatorId) ||
                fieldItem["Validate Button"].Contains(requiredValidatorId) ||
                fieldItem["Validator Bar"].Contains(requiredValidatorId) ||
                fieldItem["Workflow"].Contains(requiredValidatorId))
            {
                renderedHtml = renderedHtml.Replace("<option value=\"\"></option>", "");
            }

            output.Write(renderedHtml);
        }
        // FIX END
    }
}
