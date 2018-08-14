
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
using Tweetinvi.Models.DTO;
using Tweetinvi.Parameters;

namespace KompromatKoffer.Services
{
    internal class TwitterUserTimelineData : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public TwitterUserTimelineData(ILogger<TwitterUserTimelineData> logger)
        {
            _logger = logger;
        }

        public PaginatedList<TwitterUserTimelineModel> TwitterUserTimelineModel { get; set; }

        public List<ITweet> AllTweetsFromUser;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Twitter User Timeline Data Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(Config.Parameter.TwitterUserTimelineUpdateInterval));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Twitter User Timeline Data Service - " + DateTime.Now);

            Task.Delay(Config.Parameter.TwitterUserTimelineTaskDelay);

            try
            {

                var dbLastUpdated = Config.Parameter.TimelineDbLastUpdated;

                //Updatedelay reached => New Update => wait for new Update
                if (dbLastUpdated.AddMinutes(Config.Parameter.TimelineUpdateDelay) < DateTime.Now)
                {
                    //Set database was last updated
                    Config.Parameter.TimelineDbLastUpdated = DateTime.Now;

                    //Get all members from TwitterList - Tweetinvi
                    var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);
                    //var AllMembers = list.GetMembers(5); //Just 5 Records for Debug Reasons
                    var AllMembers = list.GetMembers(list.MemberCount);

                    foreach (var x in AllMembers)
                    {
                        //Check for Rate Limits
                        RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

                        RateLimit.QueryAwaitingForRateLimit += (sender, args) =>
                        {
                            _logger.LogInformation("Is awaiting for rate limits... " + args.Query);
                        };

                        //Get timeline for screenname from twitter using Tweetinvi
                        var user = Tweetinvi.User.GetUserFromScreenName(x.ScreenName);

                        var lastTweets = Timeline.GetUserTimeline(x.ScreenName, 15).ToArray();

                        var allTweets = new List<ITweet>(lastTweets);
                        var beforeLast = allTweets;

                        //Get more Tweets from User
                        while (lastTweets.Length > 0 && allTweets.Count <= 30)
                        {
                            var idOfOldestTweet = lastTweets.Select(y => y.Id).Min();

                            var numberOfTweetsToRetrieve = allTweets.Count > 15 ? 30 - allTweets.Count : 15;

                            // Get the UserTimeline
                            // Get more control over the request with a UserTimelineParameters
                            var userTimelineParameters = new UserTimelineParameters()
                            {
                                MaximumNumberOfTweetsToRetrieve = numberOfTweetsToRetrieve,
                                IncludeRTS = false,
                                ExcludeReplies = true,
                                MaxId = idOfOldestTweet - 1
                            };


                            lastTweets = Timeline.GetUserTimeline(x.ScreenName, userTimelineParameters).ToArray();
                            allTweets.AddRange(lastTweets);

                            //var userTimeline = Timeline.GetUserTimeline(userIdentifier, userTimelineParameters);
                            //TwitterUserTimeline = userTimeline;

                        }

                        AllTweetsFromUser = allTweets;

                        using (var db = new LiteDatabase("TwitterData.db"))
                        {

                            var mapper = BsonMapper.Global;

                            mapper.Entity<TwitterUserModel>()
                                .Id(y => y.Id); // set your document ID

                            var col = db.GetCollection<TwitterUserTimelineModel>("TwitterUserTimeline");


                                //Create UserModel for User
                                var twitterUserTimeline = new TwitterUserTimelineModel
                                {
                                    Id = x.Id,

                                    Screen_name = x.ScreenName,

                                    AllTweets = allTweets,

                                    UserUpdated = DateTime.Now


                                };

                                //Create new database entry for given user
                                col.Upsert(twitterUserTimeline);
                                _logger.LogInformation("...insert/upsert dbentry => " + x.ScreenName);
               
                        }

                        Task.Delay(Config.Parameter.TaskDelay);
                    }
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
            _logger.LogInformation("Twitter User Data Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();


        }

    }
}

