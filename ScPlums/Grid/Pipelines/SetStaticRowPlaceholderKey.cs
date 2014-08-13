using System;
using System.Reflection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Pipelines.GetPlaceholderRenderings;

namespace ScPlums.Grid.Pipelines
{
    public class SetStaticRowPlaceholderKey
    {
        public void Process(PipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var placeholderKey = GetPlaceholderKey(args);

            if (placeholderKey != null && RowPlaceholderHelper.IsRow(placeholderKey))
            {
                SetPlaceholderKey(args, RowPlaceholderHelper.GetStaticKey(placeholderKey));
            }
        }

        private string GetPlaceholderKey(PipelineArgs args)
        {
            if (args is GetPlaceholderRenderingsArgs)
            {
                return ((GetPlaceholderRenderingsArgs) args).PlaceholderKey;
            }
            else
            {
                return args.CustomData["placeHolderKey"] as string;
            }
        }

        private void SetPlaceholderKey(PipelineArgs args, string key)
        {
            if (args is GetPlaceholderRenderingsArgs)
            {
                typeof(GetPlaceholderRenderingsArgs)
                    .GetField("placeholderKey", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(args, key);
            }
            else
            {
                args.CustomData["placeHolderKey"] = key;
            }
        }
    }
}