using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

/********************************************************************************* 
 * AUTHOR: Bryant Hankins 
 * URL: http://bryanthankins.com/techblog
 * COMMENTS: This is a quickie (functional, but incomplete) port of the RubyGem cheats. 
 * It allows you to easily get a a cheat sheet for a technology.
 * I ported it to make it easier for windows users who don't have ruby.
 * See http://cheat.errtheblog.com for more details about the original vision.
 **********************************************************************************/
namespace Cheat
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                HttpGet(String.Join("_", args));
            }
            else
            {
                Console.WriteLine("Looking for help? Try 'cheat cheat'");
            }
        }
        
        private static void HttpGet(string parm)
        {
            try
            {
                string urlToGet = "";
                if (parm == "sheets")
                {
                    urlToGet = "http://cheat.errtheblog.com/ya";
                }
                else
                {
                    urlToGet = string.Format("http://cheat.errtheblog.com/y/{0}", parm);
                }

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create
                    (urlToGet);
                WebReq.Method = "GET";
                HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
                Stream Answer = WebResp.GetResponseStream();
                StreamReader _Answer = new StreamReader(Answer);
                string ScrubbedAnswer = _Answer.ReadToEnd().Replace("\\r\\n", System.Environment.NewLine);
                ScrubbedAnswer = ScrubbedAnswer.Replace("\\n", System.Environment.NewLine);
                Console.WriteLine(ScrubbedAnswer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Whoa, some kind of Internets error!");
            }

        }  
    }
}
