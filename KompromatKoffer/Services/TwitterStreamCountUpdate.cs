
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
using Tweetinvi.Core.Extensions;
using Tweetinvi.Exceptions;

namespace KompromatKoffer.Services
{
    internal class TwitterStreamCountUpdate : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public TwitterStreamCountUpdate(ILogger<TwitterStreamCountUpdate> logger)
        {
            _logger = logger;
        }

        public PaginatedList<TwitterStreamModel> TwitterStreamModel { get; set; }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TwitterStreamCountUpdate Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(Config.Parameter.TwitterStreamCountUpdateInterval));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("TwitterStreamCountUpdate Service is working.");

            await Task.Delay(Config.Parameter.TwitterStreamCountTaskDelay);

            try
            {
                var dbLastUpdated = Config.Parameter.TwitterStreamUpdated;


                if (dbLastUpdated.AddMinutes(Config.Parameter.TwitterStreamCountUpdateDelay) < DateTime.Now)
                {

                    using (var db = new LiteDatabase("TwitterData.db"))
                    {
                        // Get Datbase Connection 
                        var colTS = db.GetCollection<TwitterStreamModel>("TwitterStream");

                        var willBeUpdated = colTS.FindAll().Where(s => s.TweetCreatedAt < DateTime.Now);

                        foreach (var x in willBeUpdated)
                        {
                            var tweet = Tweetinvi.Tweet.GetTweet(x.TweetID);

                            if (tweet != null)
                            {
                                var tweetModel = new TwitterStreamModel
                                {
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

                                //tweetModel.TweetReTweetCount = tweet.RetweetCount;
                                //tweetModel.TweetFavoriteCount = tweet.FavoriteCount;

                                _logger.LogInformation("Updated Counts for " + x.TweetID);
                                colTS.Update(tweetModel);
                                await Task.Delay(10000);
                            }


                        }
                    }
                }
            }
            catch (TwitterException ex)
            {
                _logger.LogInformation("Twitter has problems..." + ex);
            }
            catch (ArgumentException ex )
            {
                _logger.LogInformation("ArgumentException..." + ex);

            }
            catch(LiteException ex)
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
            _logger.LogInformation("TwitterStreamCountUpdate Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
