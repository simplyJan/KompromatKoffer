
using KompromatKoffer.Areas.Database.Model;
using KompromatKoffer.Models;
using KompromatKoffer.Pages;
using LiteDB;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace KompromatKoffer.Services
{
    public class TwitterTrackStreamService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public TwitterTrackStreamService(ILogger<TwitterTrackStreamService> logger)
        {
            _logger = logger;
        }

        public PaginatedList<TwitterUserDailyModel> TwitterStreamData { get; set; }

        public IEnumerable<IUser> AllListMembers;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("===========> TwitterUserStreamService is starting..." + DateTime.Now);

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(Config.Parameter.TwitterTrackStreamUpdateInterval));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("===========> TwitterUserData Service - " + DateTime.Now.ToString("dd.MM.yy - hh:mm"));

            try
            {
                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    // Get Datbase Connection 
                    var colTS = db.GetCollection<TwitterStreamModel>("TwitterStream");

                    //Check for Rate Limits
                    RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

                    RateLimit.QueryAwaitingForRateLimit += (sender, args) =>
                    {
                        _logger.LogInformation("===========> Is awaiting for rate limits... " + args.Query);
                    };

                    //Get TwitterList
                    var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);

                    var AllMembers = list.GetMembers(list.MemberCount);
                    AllListMembers = AllMembers;

                    _logger.LogInformation("===========> Member to Follow: " + AllMembers.Count());

                    //Create new Stream
                    var stream = Tweetinvi.Stream.CreateFilteredStream();

                    _logger.LogInformation("===========> Start UserStreams");

                    //Foreach Member in List addfollow stream
                    foreach (var item in AllMembers.Select((value, index) => new { value, index }))
                    {
                        stream.AddFollow(item.value.UserIdentifier);
                        //_logger.LogInformation("{1} Added User {0} to stream...", item.value.UserIdentifier, item.index);
                    }

                    // Get notfified about shutdown of the stream
                    stream.StallWarnings = true;

                    //Only Match the addfollows
                    stream.MatchOn = MatchOn.Follower;

                    #region
                    
                    stream.KeepAliveReceived += async (sender, args) =>
                    {
                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                        await Task.Delay(1);
                    };

                    //Check if this is firing
                    stream.StreamStarted += async (sender, args) =>
                    {
                        _logger.LogWarning("===========> Stream has started...");

                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                        _logger.LogInformation("#### StreamState #### => " + stream.StreamState);
                        await Task.Delay(1);

                    };

                    stream.StreamResumed += async  (sender, args) =>
                    {
                        _logger.LogWarning("===========> Resumded to stream...");

                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                        _logger.LogInformation("#### StreamState #### => " + stream.StreamState);

                        await Task.Delay(1);
                    };

                    stream.StreamStopped += async (sender, args) =>
                    {
                        var exceptionThatCausedTheStreamToStop = args.Exception;
                        var twitterDisconnectMessage = args.DisconnectMessage;
                        _logger.LogWarning("===========> Stream has stopped unexpectedly..." + exceptionThatCausedTheStreamToStop + "\n" + twitterDisconnectMessage);

                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                        _logger.LogInformation("#### StreamState #### => " + stream.StreamState);

                        await Task.Delay(1);

                    };

                    stream.WarningFallingBehindDetected += async (sender, args) =>
                    {
                        _logger.LogWarning("===========> Stream Warning... " + args.WarningMessage);
                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);

                        await Task.Delay(1);

                    };

                    stream.UnmanagedEventReceived += async (sender, args) =>
                    {
                        _logger.LogWarning("===========> Stream UnmanagedEventReceived... " + args.JsonMessageReceived);
                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);

                        await Task.Delay(1);
                    };

                    stream.LimitReached += async (sender, args) =>
                    {
                        _logger.LogWarning("===========> Stream Warning... " + args.NumberOfTweetsNotReceived);
                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);

                        await Task.Delay(1);
                    };

                    stream.DisconnectMessageReceived += async (sender, args) =>
                    {
                        _logger.LogWarning("===========> Stream got disconnected... " + args.DisconnectMessage);

                        stream.StopStream();
                        await Task.Delay(5 * 60 * 1000);
                        stream.StartStreamMatchingAllConditions();
                        _logger.LogWarning("!RESTART!===========> Stream restarted at " + DateTime.Now);
                        Config.Parameter.StreamState = Convert.ToString(stream.StreamState);

                        await Task.Delay(1);
                    };
                    #endregion


                    stream.MatchingTweetReceived += async (sender, args) =>
                    {
                        if (args.MatchOn == stream.MatchOn)
                        {

                            if (args.Tweet.IsRetweet == true)
                            {
                                _logger.LogInformation(">> Skipped ReTweet...");

                                Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                            }
                            else
                            {
                                var tweet = Tweet.GetTweet(args.Tweet.Id);

                                if (tweet.ExtendedTweet != null)
                                {

                                    var tweetDB = new TwitterStreamModel
                                    {
                                        //Tweet Details
                                        TweetID = tweet.Id,
                                        TweetUserID = tweet.CreatedBy.Id,
                                        TweetUser = tweet.CreatedBy.ScreenName,
                                        TweetUserName = tweet.CreatedBy.Name,
                                        TweetUserDesc = tweet.CreatedBy.Description,
                                        TweetUserPicture = tweet.CreatedBy.ProfileImageUrlHttps,
                                        TweetCreatedAt = tweet.CreatedAt,
                                        TweetText = tweet.ExtendedTweet.Text,
                                        TweetHashtags = tweet.ExtendedTweet.LegacyEntities.Hashtags,
                                        TweetReTweetCount = tweet.RetweetCount,
                                        TweetFavoriteCount = tweet.FavoriteCount,
                                        TweetUrl = tweet.Url

                                    };

                                    _logger.LogInformation(">> New ExtendedTweet posted..." + tweet.Id);
                     
                                    //Insert Tweet in DB
                                    colTS.Insert(tweetDB);

                                    Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                                }
                                else
                                {
                                    var tweetDB = new TwitterStreamModel
                                    {
                                        //Tweet Details
                                        TweetID = tweet.Id,
                                        TweetUserID = tweet.CreatedBy.Id,
                                        TweetUser = tweet.CreatedBy.ScreenName,
                                        TweetUserName = tweet.CreatedBy.Name,
                                        TweetUserDesc = tweet.CreatedBy.Description,
                                        TweetUserPicture = tweet.CreatedBy.ProfileImageUrlHttps,
                                        TweetCreatedAt = tweet.CreatedAt,
                                        TweetText = tweet.Text,
                                        TweetHashtags = tweet.Hashtags,
                                        TweetReTweetCount = tweet.RetweetCount,
                                        TweetFavoriteCount = tweet.FavoriteCount,
                                        TweetUrl = tweet.Url
                                    };

                                    _logger.LogInformation(">> New Tweet posted..." + tweet.Id);
                                    
                                    //Insert Tweet in DB
                                    colTS.Insert(tweetDB);


                                    Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                                }

                            }

                        }
                        else
                        {
                            _logger.LogInformation(">> Tweet not matched..." + args.Tweet.Id);

                            Config.Parameter.StreamState = Convert.ToString(stream.StreamState);
                        }
                    };

                    stream.StartStreamMatchingAllConditions();

                    
                }
            }
            catch (TwitterException ex)
            {
                _logger.LogInformation("Twitter has problems..." + ex);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation("ArgumentException..." + ex);

            }
            catch (LiteException ex)
            {
                _logger.LogInformation("LiteDB Exception..." + ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception..." + ex);
            }


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("===========> TwitterUserData Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();


        }

    }
}

