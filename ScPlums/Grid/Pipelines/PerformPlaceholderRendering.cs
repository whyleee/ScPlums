using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.RenderPlaceholder;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace ScPlums.Grid.Pipelines
{
    public class PerformPlaceholderRendering : PerformRendering
    {
        protected override void Render(string placeholderName, TextWriter writer, RenderPlaceholderArgs args)
        {
            // skip not rows
            if (!RowPlaceholderHelper.IsRow(placeholderName))
            {
                base.Render(placeholderName, writer, args);
                return;
            }

            var spots = GetSpotHtmls(placeholderName, args);
            RenderSpots(spots, writer);
        }

        private IList<string> GetSpotHtmls(string placeholderName, RenderPlaceholderArgs args)
        {
            var renderings = GetRenderings(placeholderName, args).ToList();

            if (!renderings.Any())
            {
                return new List<string>();
            }

            var deferredWriters = new List<TextWriter>(
                collection: renderings.Select(r => new StringWriter())
            );

            for (var i = 0; i < renderings.Count; ++i)
            {
                // spots are not actually rendered - we store html to render them further (with wrapper-markup)
                PipelineService.Get().RunPipeline("mvc.renderRendering", new RenderRenderingArgs(renderings[i], deferredWriters[i]));
            }

            var notEmptySpots = deferredWriters
                .Select(wr => wr.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            return notEmptySpots;
        }

        private void RenderSpots(IList<string> spots, TextWriter writer)
        {
            if (!spots.Any())
            {
                return;
            }

            var rowContext = ContextService.Get().GetCurrentOrDefault<RowPlaceholderContext>();
            Func<SpotHtmlContext, HelperResult> spotRenderer =
                spot => new HelperResult(wr => wr.Write(spot.SpotHtml.ToHtmlString()));

            if (rowContext != null && rowContext.SpotRenderer != null)
            {
                spotRenderer = rowContext.SpotRenderer;
            }

            for (var i = 0; i < spots.Count; i++)
            {
                var spotHtml = spots[i];
                var resultHtml = spotRenderer(new SpotHtmlContext
                {
                    Index = i,
                    Count = spots.Count,
                    SpotHtml = new HtmlString(spotHtml)
                });

                writer.Write(resultHtml);
            }
        }
    }
}