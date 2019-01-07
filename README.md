# <b>компроматkoffer.de - Kompromittiere den Bundestag</b>

![kompromatkoffer.de](https://github.com/Scobiform/KompromatKoffer/blob/master/Images/kompromatkoffer.png)

- Using https://github.com/linvi/tweetinvi - MIT License - Copyright (c) 2017 Thomas Imart
- Using https://github.com/chartjs/Chart.js - MIT License - Copyright (c) 2018 Chart.js Contributors
- Using https://github.com/codehollow/FeedReader - MIT License - Copyright (c) 2017 Armin Reiter

- URL: https://kompromatkoffer.de
- Author: https://scobiform.com

- Icons: https://fontawesome.com
- Color Scheme: https://coolors.co/440c50-82204a-e3e7d3-27182b-fbfbfb

Case study for building a web app to analyze a special watchgroup on
twitter. In this case all members of the german parliament (MDB)

If you want to participate in this project you can contact the author.

## компроматkoffer.de - Index of Contents

1.  Why I started the project
2.  Core use-case / Intent

    2.1 Business Model?! There is no busines model!

3.  Analytical Purpose

    3.1 Overview

    3.2 Empirical research

4.  Architecture / Design

    4.1 Backend

    4.2 Frontend

5.  Database Design

    5.1 MSSQL

    5.2 LiteDB

6.  User Interactions
7.  Frontend use of twitter data / how the data will be displayed

    7.1 Color palette

### 1\. Why I started the project

The intial idea to start the project came from a talk at the 34C3
congress in germany - \"34C3 - Social Bots, Fake News und Filterblasen\"
[^1]. Michael Kreil showed how to use the twitter api for analyzing data
from twitter and discussed ways to find fake news and social bots. He
couldn´t find a significant amount of bot-activity for example in the US
election - from 12 flagged bot-accounts were only 2 most likely bots and
this automated accounts had no impact at all on any person on the social
plattform. Different methods were shown in Kreils Talk and the lack of
methodic correct research-papers on this topic inspired me to start my
own search for answers.

Also, big data is an interesting topic for me as a junior developer and
my intention was to learn the twitter api and investigate how people
communicate on twitter. With my programming language C\# and the use of
Tweetinvi [^2] by Thomas Imart I was able to put a basic concept
together for receiving tweets and start basic analysis of the stream I
was getting from twitter. After awhile I decided to build a web app for
journalists arround the basic concept to watch over every member of the
german parliament.

The intention of the project is to help people analyze their politicians
(or other groups of interest) on twitter and enable them to easily
interact with their parliament members. The Twitter API is a fantastic
opportunity to analyse data on a large scale and an easy to use web app
can help journalists to get the informations they are seeking for.

Also the topic \"fake news\" is still in the daily media, because some
politicians like don´t trust social media. The russian federation is
also considered to actively manipulating elections via twitter. In this
discussion there is an enourmous amount of fake data in the internet.
Politico author Laurent Sacharoff [^3] is talking about 10 million bots
on twitter ordered by the russian goverment in his article -- I consider
this as fake news because there is clearly no evidence for this number
-- it is basicly made up by the writer.

My research flys arround this topics\...

### 2\. Core use-case / Intent

The web app is free of charge for interested journalist and other people
who want to analyse data from a watchgroup that is generated from a
basic twitter list. In this case the web app is streaming all recent
tweets from every member of the german parliament on the frontend. In
the backend there is a database that will store all recent tweets and
make basic queries avaiable. LiteDB [^4] by Mauricio David was a good
choice for this goal, as it is light and stable.

Also all the code is opensource on github, with the project anyone can
compile a similar web app to analyse a watchgroup of choice. It would be
great if there were several similar projects to embrace people to take
part in local, national and global politics and in the search for good
methodics to find real bots and demask fake news on twitter. Since all
papers that are currently out spread mostly fake beliefs and manipulated
values -- there can´t be any real evidence that bots have impact on the
individuals in a certain watchgroup.

#### 2.1 Business Model?! There is no business model!

Actually there is no real business model planned. If a open source or a
transparency foundation wants to help the project with money it would be
greatly tolerated -- but it would still depend on the openess for
foundamental democracy values of that foundation. Donations for the work
I am putting to the project are happily taken.

Also advertisment is not planned in any way, because nothing is more
shameful as commercials flackering over the screen of a web app and it
will hurt the clean- and openness of viewing the data.

### 3\. Analytical Purpose

#### 3.1 Overview

##### 3.1.1 Current Status

-   How much is a individual tweeting
-   How many followers, likes etc. is the individual gaining
-   There is a individual graph to show the data over time
-   There is a global graph to compare the individual data
-   Hashtag usage for the individual profiles
-   Toplists of the watchgroup
-   At which hour of the day is a individual tweeting.
-   An archive of tweets of every individual even the deleted ones.

##### 3.1.2 Future Status / may be or not integrated

-   Analysis of relationships between individuals (who follows who /
    political filter bubble overview)
-   Political campaign analysis
-   Who is posting fake news and who is retweeting fake news
-   Meme analysis, which memes have the most impact
-   Which political party is the strongest on twitter and how does this
    data compare to the surveys of opinion research institutes
-   How much impact has media on the watchgroup
-   Which media is likely trusted by which party
-   What was the top topic in each week

### 3.2 Empirical research

The main questions is: Are political parties in germany using startegies
to manipulate the opinion of people on twitter? Which political party
uses fake news and bot-activity to influence people on twitter? And how
do members of the german paliament use twitter. Finally who is gaining
and reaching audience on twitter.

### 4\. Architecture / Design

#### 4.1.1 Backend

#### 4.1.2 Frontend

### 5\. Database Design

#### MSSQL

Micorsoft SQL database is used to store all user account related data.
Login process and twitter tokens are manged through the twitter
authentication for ASP.NET apps in combination with tweetinvi.

#### LiteDB

The database currently consists of 3 collections to store data from
twitter for analysis. LiteDB is a Serverless NoSQL Document Store, a
simple API similar to MongoDB (1).

#### TwitterStream

Every tweet that is matching criteria is saved to database.

-   long TweetID { get; set; }
-   string TweetUser { get; set; }
-   string TweetUserName { get; set; }
-   long TweetUserID { get; set; }
-   string TweetUserPicture { get; set; }
-   string TweetUserDesc { get; set; }
-   string TweetText { get; set; }
-   List\<Tweetinvi.Models.Entities.IHashtagEntity\> TweetHashtags {
    get; set; }
-   int TweetReTweetCount { get; set; }
-   int TweetFavoriteCount { get; set; }
-   DateTime TweetCreatedAt { get; set; }
-   string TweetUrl { get; set; }

#### TwitterUser

Listmember will pe put each interval to database -- this collection gets
overwritten every interval.

-   long Id { get; set; }
-   string Name { get; set; }
-   string Screen\_name { get; set; }
-   string Description { get; set; }
-   DateTime Created\_at { get; set; }
-   string Location { get; set; }
-   bool Geo\_enabled { get; set; }
-   object Url { get; set; }
-   int Statuses\_count { get; set; }
-   int Followers\_count { get; set; }
-   int Friends\_count { get; set; }
-   bool Verified { get; set; }
-   string Profile\_image\_url\_https { get; set; }
-   int Favourites\_count { get; set; }
-   int Listed\_count { get; set; }
-   DateTime UserUpdated { get; set; }
-   string PoliticalParty { get; set; }

#### TwitterUserDaily

If today no user was saved to database -- the service will insert a new
entry into the collection.

-   long Id { get; set; }
-   string Screen\_name { get; set; }
-   int Statuses\_count { get; set; }
-   int Followers\_count { get; set; }
-   int Friends\_count { get; set; }
-   int Favourites\_count { get; set; }
-   int Listed\_count { get; set; }
-   DateTime DateToday { get; set; }
-   long TwitterId { get; set; }
-   string TwitterName { get; set; }

### 6\. User Interactions

Currently there are no user interactions. In the future it is might be
possible to easily add all members of the watchlist to the personal
twitter account and help users to build own watchlists through the app.
But currently nothing is planned and is very unlikely that it will be
implented because there is no real need for such processes.

### 7\. Frontend use of twitter data / how the data will be displayed

The tweets are displayed in row fashion like on twitter with a seperate
design to match the app. The TwitterLogo is shown in the bottom-left of
every tweet. Every tweet directly links to twitter. The Design is a
rough base and for sure it is possible to match more of the guidelines
by twitter.

![](https://github.com/Scobiform/KompromatKoffer/blob/master/Images/tweet_on_kk.png)

Tweets are displayed on the IndexView, PeopleDetailView,
SearchAllTweetsView.

#### 7.1 Color palette

![](https://github.com/Scobiform/KompromatKoffer/blob/master/Images/kkcolors.png)


______________________________________________________________________________

[^1]:  Angriff der Meinungsroboter" und „Gefangen in der Filterblase"
    titelten die deutschen Medien. Doch was ist wirklich daran? Michael
    Kreil - 34C3 - Social Bots, Fake News und Filterblasen -
    <https://www.youtube.com/watch?v=6jNWl5d_DOk>

[^2]:  Tweetinvi -- Thomas Imart -- MIT License
    <https://github.com/linvi/tweetinvi>

[^3]:  Russia Gave Bots a Bad Name. Here's Why We Need Them More Than
    Ever.
    <https://www.politico.com/magazine/story/2018/08/14/russia-gave-bots-a-bad-name-heres-why-we-need-them-more-than-ever-219359>

[^4]:  LiteDB by Mauricio David - MIT License -
    https://github.com/mbdavid/LiteDB

