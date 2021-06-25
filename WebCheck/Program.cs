using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace WebCheck
{
    class Program
    {
        private static Options options = new ();
        private static List<UrlToCheck> urls = new ();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting...");

            try
            {
                ParseOptions();
            }
            catch (Exception ex)
            {
                PrintRed($"ERROR PARSING OPTIONS: {ex.Message}");
                return;
            }

            if (!File.Exists(options.UrlsFile))
            {
                PrintRed($"MISSING {options.UrlsFile}");
                return;
            }

            try
            {
                urls = ParseUrls();
            }
            catch (Exception ex)
            {
                PrintRed(ex.Message);
                return;
            }

            using var c = new HttpClient();


            while (true)
            {
                foreach (var url in urls)
                {
                    try
                    {
                        await Task.Delay(500);
                        var result = await c.GetAsync(url.Url);

                        //detect failures
                        if (!result.IsSuccessStatusCode)
                        {
                            ReportFailure(url.Url, result.StatusCode.ToString());
                            continue;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(url.Expect))
                            {
                                var content = await result.Content.ReadAsStringAsync();
                                if (!content.Contains(url.Expect))
                                {
                                    ReportFailure(url.Url, $"Expected but NOT FOUND {url.Expect} in {url.Url}");
                                    continue;
                                }
                            }
                        }

                        //above we reported failures, and jumped to another foreach round, if we are here, no failers detected, so we can say success
                        Console.WriteLine($"{DateTime.Now.ToShortTimeString()} OK: {url}");
                    }
                    catch (Exception ex)
                    {
                        ReportFailure(url.Url, ex.Message);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(options.Interval));
            }
        }

        private static List<UrlToCheck> ParseUrls()
        {
            Console.WriteLine("parsing urls");
            var list = new List<UrlToCheck>();
            var urls = File.ReadAllLines(options.UrlsFile);
            foreach (var url in urls)
            {
                
                var items = url.Trim().Split(' ',2,StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 2)
                {
                    list.Add(new UrlToCheck(items[0], items[1]));
                }
                else if (items.Length == 1)
                {
                    list.Add(new UrlToCheck(items[0]));
                }
                else
                {
                    throw new ArgumentException("error parsing urls: " + url);
                }
            }
            Console.WriteLine($"{list.Count} urls parsed");

            return list;
        }

        private static void ParseOptions()
        {
            Console.WriteLine("parsing options");
            if (File.Exists("webcheck.config"))
            {
                Console.WriteLine("webcheck.config found, parsing options..");
                var lines = File.ReadAllLines("webcheck.config").Where(line => !string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#"));

                foreach (var line in lines)
                {
                    var items = line.Split('=');
                    if (items.Length == 2)
                    {
                        var property = items[0].ToLower();
                        var value = items[1];

                        if (property == "UrlsFile".ToLower())
                            options.UrlsFile = value;
                        if (property == "Interval".ToLower())
                            options.Interval = int.Parse(value);
                        if (property == "LogFailuresToFile".ToLower())
                            options.LogFailuresToFile = bool.Parse(value);
                        if (property == "LogFailuresFile")
                            options.LogFailuresFile = value;
                        if (property == "SentEmailOnError".ToLower())
                            options.SentEmailOnError = bool.Parse(value);
                        if (property == "EmailTo".ToLower())
                            options.EmailTo = value;
                        if (property == "Smtp".ToLower())
                            options.Smtp = value;
                        if (property == "SmtpPassword".ToLower())
                            options.SmtpPassword = value;
                        if (property == "SmtpUsername".ToLower())
                            options.SmtpUsername = value;
                        if (property == "EmailFrom".ToLower())
                            options.EmailFrom = value;
                        if (property == "EmailFromName".ToLower())
                            options.EmailFromName = value;
                        if (property == "EmailSubject".ToLower())
                            options.EmailSubject = value;
                        //if (property == "EmailBody".ToLower())
                        //    options.EmailBody = value;
                    }
                }
                Console.WriteLine("options parsed");
            }
            else
            {
                Console.WriteLine("no webcheck.config found, using default options");
            }
        }

        private static void ReportFailure(string url, string errMessage)
        {
            var errmsg = $"{DateTime.Now.ToShortTimeString()} {url} ERROR: {errMessage}";
            PrintRed(errmsg);

            if (options.LogFailuresToFile)
            {
                try
                {
                    if (!File.Exists(options.LogFailuresFile))
                        File.Create(options.LogFailuresFile);

                    lock (options.LogFailuresFile)
                    {
                        File.AppendAllText(options.LogFailuresFile, Environment.NewLine + errmsg);
                    }
                }
                catch (Exception ex)
                {
                    PrintRed("EROR LOGGING TO FILE" + ex.Message);
                }
            }

            if(options.SentEmailOnError)
            {
                try
                {
                    SendMail.sendEmail(sentFrom: options.EmailFrom,
                                       fromName: options.EmailFromName,
                                       emailTo: options.EmailTo,
                                       subject: options.EmailSubject,
                                       htmlBody: $"webcheck: failure getting url:{url}, errMessage{errMessage}",
                                       smtp: options.Smtp,
                                       smtpUserName: options.SmtpUsername,
                                       smtpPassword: options.SmtpPassword);
                }
                catch (Exception ex)
                {
                    PrintRed("EROR SENDING EMAIL" + ex.Message);
                }
            }
        }

        private static void PrintRed(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
