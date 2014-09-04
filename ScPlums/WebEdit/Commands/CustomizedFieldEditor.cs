using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.WebEdit;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace ScPlums.WebEdit.Commands
{
    // TODO: add support for external items (need to hack FieldEditor logic too)
    public class CustomizedFieldEditor : FieldEditor
    {
        protected override PageEditFieldEditorOptions GetOptions(ClientPipelineArgs args, NameValueCollection form)
        {
            // Correct param override (Sitecore merges overridden param and default param with ',')
            if (args.Parameters["uri"].Contains(","))
            {
                args.Parameters["uri"] = args.Parameters["uri"].Split(',').First();
            }

            var uri = ItemUri.Parse(args.Parameters["uri"]);
            var item = Database.GetItem(uri);
            Assert.IsNotNull(item, "item");

            var commandId = args.Parameters["command"];
            Assert.IsNotNullOrEmpty(commandId, "Field Editor command expects 'command' parameter");
            var commandItem = Client.CoreDatabase.GetItem(commandId);
            Assert.IsNotNull(commandItem, "command item");

            var specifiedFields = new ListString(args.Parameters["fields"] ?? "");
            var specifiedSections = new ListString(args.Parameters["sections"] ?? "");

            Assert.IsTrue(specifiedFields.Any() || specifiedSections.Any(),
                "Field Editor command expects either 'fields' or 'sections' parameter"
            );

            var fields = specifiedFields
                .Where(fieldName => item.Fields[fieldName] != null)
                .Select(fieldName => new FieldDescriptor(item, fieldName))
                .ToList();

            if (specifiedSections.Any())
            {
                var allFields = item.Fields;
                allFields.ReadAll();
                allFields.Sort();

                fields.AddRange(allFields
                    .Where(field => specifiedSections.Contains(field.Section))
                    .Select(field => new FieldDescriptor(item, field.Name))
                );
            }

            var title = commandItem["Header"] ?? commandItem.DisplayName;
            var preserveSections = specifiedSections.Any() || MainUtil.GetBool(args.Parameters["preserve-sections"], false);

            return new PageEditFieldEditorOptions(form, fields)
            {
                Title = title,
                DialogTitle = title,
                Icon = commandItem["Icon"],
                Text = commandItem["Tooltip"],
                PreserveSections = preserveSections
            };
        }
    }
}
