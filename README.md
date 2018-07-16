<b>компроматkoffer.de - Kompromittiere den Bundestag</b>

NetCore 2.1 Web Application for displaying all recent tweets from every member of the german parliament. Currently the app just outputs a twitter list made by martin fuchs (http://martin-fuchs.org). In the near future the web app will provide search, scan, user accounts, watchlists, screenshots.

URL: https://kompromatkoffer.de
Author: https://scobiform.com

You will need a Config.cs to run the app.

```C#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace KompromatKoffer
{
    public class Config
    {

        public static class Credentials
        {
            public static string CONSUMER_KEY = "";
            public static string CONSUMER_SECRET = "";
            public static string ACCESS_TOKEN = "";
            public static string ACCESS_TOKEN_SECRET = "";

            public static ITwitterCredentials GenerateCredentials()
            {
                return new TwitterCredentials(CONSUMER_KEY, CONSUMER_SECRET, ACCESS_TOKEN, ACCESS_TOKEN_SECRET);
            }

        }

        public static class Parameter
        {
            public static int TweetsRetrieved { get; set; } = 100;
            public static string ListName { get; set; } = "mdb-bundestag";
            public static string ScreenName { get; set; } = "wahl_beobachter";
        }
    }
} ```