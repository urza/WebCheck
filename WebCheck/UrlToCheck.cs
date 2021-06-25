using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCheck
{
    class UrlToCheck
    {

        public UrlToCheck(string Url, string Expect = null)
        {
            this.Url = Url;
            this.Expect = Expect;
        }

        public string Url { get; set; }

        /// <summary>
        /// What should be present in result (e.g. html source), if not found in result, report as error
        /// </summary>
        public string Expect { get; set; }

        public override string ToString()
        {
            return Url + (string.IsNullOrEmpty(Expect) ? "" : " + EXPECTING " + Expect);
        }
    }
}
