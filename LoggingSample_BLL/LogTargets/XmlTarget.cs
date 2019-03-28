using System;
using System.IO;
using System.Xml.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace LoggingSample_BLL.LogTargets
{
    [Target("XmlTarget")]
    public sealed class XmlTarget : TargetWithLayout
    {
        private static readonly object Locker = new object();

        public XmlTarget()
        {
            this.Host = Environment.MachineName;
        }

        [RequiredParameter]
        public string Host { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            var xElement = new XElement("logMessage",
                new XElement("machineName", this.Host),
                new XElement("exception", logEvent.Exception?.ToString()),
                new XElement("loggerName", logEvent.LoggerName),
                new XElement("level", logEvent.Level.ToString()),
                new XElement("message", logEvent.Message),
                new XElement("messageSource", logEvent.CallerFilePath),
                new XElement("timeStamp", logEvent.TimeStamp));

            string filePath = LogManager.Configuration.Variables["xmlFilePath"].Render(logEvent);
            lock(Locker)
            {
                if (!File.Exists(filePath))
                {
                    var document = new XDocument(new XElement("logMessages", xElement));
                    document.Save(filePath);
                }
                else
                {
                    var document = XDocument.Load(filePath);
                    XElement root = document.Element("logMessages");
                    root.Add(xElement);
                    document.Save(filePath);
                }
            }
        }
    }
}