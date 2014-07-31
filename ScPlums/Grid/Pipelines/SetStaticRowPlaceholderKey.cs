using System;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;

namespace ScPlums.Grid.Pipelines
{
    public class SetStaticRowPlaceholderKey
    {
        public void Process(PipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var placeholderKey = args.CustomData["placeHolderKey"] as string;

            if (placeholderKey != null && RowPlaceholderHelper.IsRow(placeholderKey))
            {
                args.CustomData["placeHolderKey"] = RowPlaceholderHelper.GetStaticKey(placeholderKey);
            }
        }
    }
}