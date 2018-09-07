<b>компроматkoffer.de - Kompromittiere den Bundestag</b>

ASP.Net Core 2.1 Web Application for displaying all recent tweets from every member of the german parliament. Analytical chart overview for every politician and a global chart overview of all politicians. Currently the app only displays counts - such as followers, likes etc. Next step is a timeline detail view and a realtime userstream, hopefully with a filterbubble overview.

- Using https://github.com/linvi/tweetinvi - MIT License - Copyright (c) 2017 Thomas Imart
- Using https://github.com/chartjs/Chart.js - MIT License - Copyright (c) 2018 Chart.js Contributors

- URL: https://kompromatkoffer.de
- Author: https://scobiform.com

- Icons: https://fontawesome.com
- Color Scheme: https://coolors.co/440c50-82204a-e3e7d3-27182b-fbfbfb

<b>You will need a Config.cs to run the app. Don´t forget the Parameters ;)</b>

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
            //How many tweets would you like to receive
            public static int TweetsRetrieved { get; set; } = 500;
			
            //Twitter Listname and Name of Owner
            public static string ListName { get; set; } = "listname";
            public static string ScreenName { get; set; } = "listuser";
            
			 // Service Switches
            public static bool DoBackup = true;
            public static bool SwitchOnAllServices = false;
            public static bool SaveToDatabase = true;
            public static bool TwitterUserDaily { get; set; } = true;
            public static bool TwitterStream { get; set; } = true;
            public static bool UpdateTwittterCounts { get; set; } = true;

            //Backup
            public static DateTime DBLastBackup { get; set; }
            public static int DBBackupInterval { get; set; } = 8;
            public static int DBBackupSpawn { get; set; } = 5;

            //♣♣♣♣♣♣♣♣♣♣♣♣♣♣ Save TwitterUserData to Database
            public static DateTime DbLastUpdated { get; set; }
            public static int UpdateDelay { get; set; } = 15;
            public static int TwitterUserUpdateInterval { get; set; } = 120;
			public static int TwitterUserWriteDelay { get; set; } = 10000; //ms

            //♣♣♣♣♣♣♣♣♣♣♣♣♣♣ TwitterUserDaily Data
            public static DateTime UserDailyDataLastUpdated { get; set; }
            public static int TwitterUserDailyUpdateInterval { get; set; } = 300;
            public static int TwitterUserDailyTaskDelay { get; set; } = 240000; //ms
            public static int TwitterUserDailyUpdateDelay { get; set; } = 35;
            public static int TwitterUserDailyWriteDelay { get; set; } = 10000; //ms

            //♣♣♣♣♣♣♣♣♣♣♣♣♣♣ TwitterStream
            public static int TwitterStreamCountUpdateInterval { get; set; } = 30; //min
            public static DateTime TwitterStreamUpdated { get; set; }
            public static int TwitterStreamCountUpdateDelay { get; set; } = 30; //min
            public static int TwitterStreamCountWriteDelay { get; set; } = 2000; //ms
            public static int TwitterStreamCountTaskDelay { get; set; } = 50000; //ms
            public static int TwitterStreamCountUpdateLastHours { get; set; } = -24; //h
            public static int TwitterStreamDayRange { get; set; } = -1; //h

            //♣♣♣♣♣♣♣♣♣♣♣♣♣♣ Mail Config		
			public static string Mail_From_Email_Address { get; set; } = "email@email..com";
            public static string Mail_From_Email_DisplayName { get; set; } = "displayname";
            public static string Mail_Host { get; set; } = "mail.mail.com";
            public static int Mail_Port { get; set; } = 993;
            public static string Mail_Email_Login { get; set; } = "email.email.com";
            public static string Mail_Email_Passwort { get; set; } = "password";
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
