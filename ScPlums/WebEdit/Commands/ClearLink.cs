using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace ScPlums.WebEdit.Commands
{
    public class ClearLink : WebEditCommand
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            var formValue = WebUtil.GetFormValue("scPlainValue");
            context.Parameters.Add("fieldValue", formValue);
            Sitecore.Context.ClientPage.Start(this, "Run", context.Parameters);
        }

        protected static void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            var value = args.Parameters["fieldValue"];

            if (string.IsNullOrEmpty(value))
            {
                SheerResponse.Alert("There is no link to remove.");
                return;
            }

            var controlId = args.Parameters["controlid"];

            args.Result = "";
            var result = typeof(EditLink).GetMethod("RenderLink", BindingFlags.Static | BindingFlags.NonPublic)
                .Invoke(null, new object[] { args })
                .ToString();

            SheerResponse.SetAttribute("scHtmlValue", "value", result);
            SheerResponse.SetAttribute("scPlainValue", "value", string.Empty);
            SheerResponse.Eval("scSetHtmlValue('" + controlId + "', false, true)");
        }
    }
}
