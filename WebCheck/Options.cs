using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCheck
{
    class Options
    {
        public string UrlsFile { get; set; } = "urls.txt";

        /// <summary>
        /// check webs every {Interval} minutes
        /// </summary>
        public int Interval { get; set; } = 5;

        /// <summary>
        /// Where to log errors
        /// </summary>
        public string LogFailuresFile { get; set; } = "errors.txt";
        public bool LogFailuresToFile { get; set; } = false;

        /// <summary>
        /// sending emails on error
        /// </summary>
        public bool SentEmailOnError { get; set; } = false;
        public string EmailTo { get; set; }
        public string Smtp { get;set; }
        public string SmtpPassword { get; set; }
        public string SmtpUsername { get; set; }
        public string EmailFrom { get; set; }
        public string EmailFromName { get; set; }
        public string EmailSubject { get; set; } = "webcheck error detected";
        //public string EmailBody { get; set; }
    }
}
