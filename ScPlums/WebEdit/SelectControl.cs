using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScPlums.Aids;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Pipelines.RenderField;

namespace ScPlums.WebEdit
{
    public class SelectControl
    {
        public string Render(RenderFieldArgs args)
        {
            var field = args.GetField();
            var hierarchical = args.FieldTypeKey == "droptree";

            var language = Sitecore.Context.ContentLanguage;

            if (string.IsNullOrEmpty(language.Name))
            {
                language = Sitecore.Context.Language;
            }

            var database = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;

            var html = new StringBuilder();
            var onchangeJs = "var chromeId = $(this).prev().attr('id').replace('_edit', '');" +
                "Sitecore.PageModes.ChromeManager.updateField(chromeId, null, $(this).val(), true);";

            html.AppendFormat("<select id=\"{0}\" class=\"scEnabledChrome\" sc-part-of=\"field\" onchange=\"{1}\">",
                SitecoreUtil.GetWebEditingControlId(field), onchangeJs
            );

            var rootPath = GetRootPath(field);
            var rootItem = database.GetItem(rootPath, language);
            var selectedOption = args.Item[args.FieldName];

            RenderOptions(rootItem.GetChildren(), html, field, selectedOption, hierarchical, level: 1);
            html.Append("</select>");

            return html.ToString();
        }

        private void RenderOptions(IEnumerable<Item> options, StringBuilder html, Field field, string selectedOption, bool hierarchical, int level)
        {
            if (level == 1)
            {
                var isNoValueSelected = string.IsNullOrEmpty(selectedOption) ? " selected" : "";
                var fieldNameLowered = (field.DisplayName.IfNotNullOrEmpty() ?? field.Name).ToLower();

                html.AppendFormat("<option value=\"\"{0}>[No {1}]</option>", isNoValueSelected, fieldNameLowered);
            }

            foreach (var option in options)
            {
                var displayText = GetDisplayText(option, field);
                var indent = "";

                for (int i = 1; i < level; i++)
                {
                    indent += "&nbsp;&nbsp;&nbsp;";
                }

                if (hierarchical)
                {
                    var childOptions = option.GetChildren();

                    if (childOptions.Any())
                    {
                        html.AppendFormat("<optgroup label=\"{0}\">", indent + displayText);
                        RenderOptions(childOptions, html, field, selectedOption, hierarchical, level + 1);
                        html.Append("</optgroup>");

                        continue;
                    }
                }

                RenderOption(option, html, selectedOption, indent + displayText);
            }
        }

        private void RenderOption(Item option, StringBuilder html, string selectedOption, string displayText)
        {
            var isSelected = option.ID.ToString() == selectedOption;
            html.AppendFormat("<option value=\"{0}\"{1}>{2}</option>", option.ID, isSelected ? " selected" : "", displayText);
        }

        private string GetRootPath(Field field)
        {
            var @params = field.Source.Split('&');

            foreach (var param in @params)
            {
                if (param.StartsWith("/"))
                {
                    return param;
                }
                else if (param.StartsWith("DataSource="))
                {
                    return param.Replace("DataSource=", "");
                }
            }

            return null;
        }

        private string GetDisplayText(Item option, Field field)
        {
            string displayText = null;
            var displayFieldName = field.Source.Split('&')
                .FirstOrDefault(param => param.StartsWith("DisplayFieldName="));

            if (!string.IsNullOrEmpty(displayFieldName))
            {
                displayText = option[displayFieldName.Replace("DisplayFieldName=", "")];
            }

            return displayText.IfNotNullOrEmpty() ?? option.DisplayName.IfNotNullOrEmpty() ?? option.Name;
        }
    }
}
