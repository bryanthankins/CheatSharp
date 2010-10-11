using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Xml.Linq;

/********************************************************************************* 
 * AUTHOR: Bryant Hankins 
 * URL: http://bryanthankins.com/techblog
 * 
 * COMMENTS: 
 * This is a quickie (functional, but incomplete) port of the RubyGem cheats. 
 * It allows you to easily get a a cheat sheet for a technology.
 * I ported it to make it easier for windows users who don't have ruby.
 * See http://cheat.errtheblog.com for more details about the original vision.
 * I also added support for asp.net sheets using a diff server repository.
 * If you have a cheat sheet that you'd like added just email me:
 * bryanthankins [at] gmail.com
 * 
 * USAGE: 1) Compile the exe and place in c:\windows
 *        2) Run through cmd prompt, vim or VS.NET (using external tools)
 *              cheat -r sheets: list ruby cheat sheets
 *              cheat -r <sheet name>: ruby cheat sheet system
 *              cheat sheets: to see all asp.net cheat sheets
 *              cheat <sheet name>: asp.net cheat systems
 *              
 * ISSUES: 
 * If it's not working, make sure you have the app.config (or cheat.exe.config) 
 * deployed with the app. This allows it to get through a proxy
 * 
 **********************************************************************************/

namespace Cheat
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                // test for rails mode
                if(args[0] == "-r")  
                {
                    if (args.Length > 1)
                    {
                        HttpGetRails(args[1]);
                    }
                    else
                    { 
                        Console.WriteLine("Looking for help? Try 'cheat cheat'");
                    }

                }
                else
                {
                    HttpGetAsp(args[0]);
                }
            }
            else
            {
                Console.WriteLine("Looking for help? Try 'cheat cheat'");
            }
        }
        private static void HttpGetAsp(string parm)
        {
            const string BASE_URL = "http://aspcheats.heroku.com/cheatsheets";

            string urlToGet = "";
            if (parm == "sheets")
            {
                urlToGet = string.Format(BASE_URL + ".xml");
            }
            else
            {
               urlToGet = string.Format(BASE_URL + "/{0}.xml", parm);
            }

            Console.WriteLine(HttpGet(urlToGet, true));
            //Console.Read();
        }
        
        private static void HttpGetRails(string parm)
        {
            const string BASE_URL = "http://cheat.errtheblog.com";

            string urlToGet = "";
            if (parm == "sheets")
            {
               urlToGet = BASE_URL + "/ya";
            }
            else
            {
               urlToGet = string.Format(BASE_URL + "/y/{0}", parm);
            }

            Console.WriteLine(HttpGet(urlToGet, false));
            //Console.Read();

        }  
        private static string HttpGet(string urlToGet, bool processXml)
        {
            try
            {
                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create
                    (urlToGet);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                Stream Answer = WebResp.GetResponseStream();
                StreamReader _Answer = new StreamReader(Answer);
       
                string ScrubbedAnswer = _Answer.ReadToEnd().Replace("\\r\\n", System.Environment.NewLine);
                ScrubbedAnswer = ScrubbedAnswer.Replace("\\n", System.Environment.NewLine);
                string finalAnswer = String.Empty;
                if (processXml)
                {
                    //handle sheets data
                    if (ScrubbedAnswer.IndexOf("<cheatsheets>") != -1)
                    {
                        XElement elements = XElement.Parse(ScrubbedAnswer);
                        var sheets = from item in elements.Descendants("sheet")
                                     select new
                                     {
                                         Name = item.Element("name").Value
                                     };

                        foreach (var sheet in sheets)
                        {
                            finalAnswer += sheet.Name + "\r\n";
                        }


                    }
                    else
                    {
                        int begin = ScrubbedAnswer.IndexOf("<content>") + 9;
                        int end = ScrubbedAnswer.IndexOf("</content>");
                        int length = end - begin;
                        ScrubbedAnswer = ScrubbedAnswer.Substring(begin, length);
                        finalAnswer = HttpUtility.HtmlDecode(ScrubbedAnswer);
                    }
                }
                else
                {
                    finalAnswer = ScrubbedAnswer;
                }
                return finalAnswer;
                
            }
            catch (Exception)
            {
                Console.WriteLine("Could not retrieve cheatsheet...you're on your own!");
            }
            return "";
        }
    }
}
