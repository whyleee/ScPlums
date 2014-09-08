using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Appender;
using log4net.spi;
using Microsoft.ApplicationInsights.Tracing;

namespace ScPlums.Vso
{
    // Fixed version of MMA (Microsoft Monitoring Agent) log4net appender, which works with any log4net, including internal Sitecore version.
    // TOUSE: register this appender in <log4net> config section instead of default 'Microsoft.ApplicationInsights.Log4NetAppender.ApplicationInsightsAppender'
    public class AnyLog4NetApplicationInsightsAppender : AppenderSkeleton
    {
        private LoggingController loggingController;

        /// <summary>
        /// Get/set The Application Insights ComponentId for your application. 
        /// </summary>
        /// <remarks>
        /// This is normally pushed from when appender is being initialized.
        /// </remarks>
        public string ComponentId
        {
            get;
            set;
        }

        /// <summary>
        /// The <see cref="T:ScPlums.Vso.AnyLog4NetApplicationInsightsAppender" /> requires a layout.
        /// This appender converts the LoggingEvent it receives into a text string and requires the layout format string to do so.
        /// </summary>
        protected override bool RequiresLayout
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Initializes the Appender and perform ComponentId validation.
        /// </summary>
        public override void ActivateOptions()
        {
            base.ActivateOptions();
            try
            {
                loggingController = LoggingController.CreateLoggingController(ComponentId);
            }
            catch(Exception ex)
            {
                throw new LogException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Append LoggingEvent Application Insights logging framework.
        /// </summary>
        /// <param name="loggingEvent">Events to be logged.</param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                loggingController.LogMessageWithData(base.RenderLoggingEvent(loggingEvent), loggingEvent);
            }
            catch(Exception ex)
            {
                throw new LogException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Releases all resources used by <see cref="T:ScPlums.Vso.AnyLog4NetApplicationInsightsAppender" />.
        /// </summary>
        public override void OnClose()
        {
            if(loggingController != null)
            {
                loggingController.Dispose();
                loggingController = null;
            }
            base.OnClose();
        }
    }
}