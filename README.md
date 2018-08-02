<b>компроматkoffer.de - Kompromittiere den Bundestag</b>

ASP.Net Core 2.1 Web Application for displaying all recent tweets from every member of the german parliament. Currently the app outputs a twitter list made by martin fuchs (http://martin-fuchs.org) and right now I am implementing the NoSQL Database (LiteDB) to store  data and generate analytical profiles from each dataset.

Using https://github.com/linvi/tweetinvi - MIT License - Copyright (c) 2017 Thomas Imart

URL: https://kompromatkoffer.de
Author: https://scobiform.com

Icons: https://fontawesome.com
Color Scheme: https://coolors.co/440c50-82204a-e3e7d3-27182b-fbfbfb

<b>You will need a Config.cs to run the app.</b>

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
    }
} 
```

<b>Also a appsettings.json for the MSSQL Connection</b>
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ApplicationContextConnection": "Data Source=SERVERNAME;Integrated Security=False;User ID=USERLOGIN;Password=PASSWORD;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
  }
```


<b>ProjectOverview:</b>
Goal is to publish live tweets from every member of the german bundestag (Parliament) and make twitter data from this group of people available for analyticcal purpose through a database (LiteDB). The website (ASP.Net Core Razor Pages) will be free for all visitors and the project is mostly made as a tool for journalists and people who are interested in politics - to search for informations and see what our german politicians are doing on twitter. Transparency and freedom of open information are the fundmental principles. The source code will be open for everyone on GitHub (https://github.com/Scobiform/KompromatKoffer)

The project is currently in alpha and I am still testing many parts of the project. 
