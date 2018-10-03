using KompromatKoffer.Areas.Database.Model;
using KompromatKoffer.Pages;
using LiteDB;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Exceptions;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace KompromatKoffer.Services
{    
    internal interface TwitterStreamService
    {
        void DoWork();
    }

    internal class ScopedProcessingService : TwitterStreamService
    {
        private readonly ILogger _logger;

        public ScopedProcessingService(ILogger<ScopedProcessingService> logger)
        {
            _logger = logger;
        }

        public PaginatedList<TwitterUserDailyModel> TwitterStreamData { get; set; }

        public IEnumerable<IUser> AllListMembers;

        public async void DoWork()
        {
            _logger.LogInformation("===========> TwitterStream Service is working " + DateTime.Now.ToString("dd.MM.yy - hh:mm"));

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
                        _logger.LogInformation(">> Is awaiting for rate limits... " + args.Query);
                    };

                    //Get TwitterList
                    var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);

                    var AllMembers = list.GetMembers(list.MemberCount);
                    AllListMembers = AllMembers;

                    _logger.LogInformation("===========> Members to Follow: " + AllMembers.Count());

                    //Create Stream
                    var stream = Tweetinvi.Stream.CreateFilteredStream();

                    _logger.LogInformation("===========> Start UserStreams");

                    //Foreach Member in List addfollow stream
                    foreach (var item in AllMembers.Select((value, index) => new { value, index }))
                    {
                        stream.AddFollow(item.value.UserIdentifier);
                        //_logger.LogInformation("{1} Added User {0} to stream...", item.value.UserIdentifier, item.index);
                    }

                    //Only Match the addfollows
                    stream.MatchOn = MatchOn.Follower;

                    stream.MatchingTweetReceived += async (sender, args) =>
                    {
                        if (args.MatchOn == stream.MatchOn)
                        {
                            if (args.Tweet.IsRetweet == true)
                            {
                                _logger.LogInformation(">> Skipped ReTweet...");
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
                                    await Task.Delay(1);
                                    //Insert Tweet in DB
                                    colTS.Insert(tweetDB);
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
                                    await Task.Delay(1);
                                    //Insert Tweet in DB
                                    colTS.Insert(tweetDB);

                                }

                            }

                        }
                        else
                        {
                            _logger.LogInformation(">> Tweet not matched..." + args.Tweet.Id);
                        }
                    };

                    stream.StartStreamMatchingAllConditions();

                }
            }
            catch (TwitterException ex)
            {
                _logger.LogInformation("Twitter Exception", ex);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation("Argument Exception", ex);
            }
            catch (LiteException ex)
            {
                _logger.LogInformation("LiteDB Exception", ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exceptions", ex);
            }

            await Task.Delay(1);


        }
    }


}
