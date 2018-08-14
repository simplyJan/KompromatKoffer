
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
    internal class TwitterUserDailyData : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public TwitterUserDailyData(ILogger<TwitterUserDailyData> logger)
        {
            _logger = logger;
        }

        public PaginatedList<TwitterUserDailyModel> TwitterUserDailyModel { get; set; }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Twitter User Daily Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(Config.Parameter.TwitterUserDailyUpdateInterval));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Twitter User Daily Service is working.");

            Task.Delay(Config.Parameter.TwitterUserDailyTaskDelay);

            try
            { 
            //Get all members from TwitterList - Tweetinvi
            var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);
            //var AllMembers = list.GetMembers(5); //Just 5 Records for Debug Reasons
            var AllMembers = list.GetMembers(list.MemberCount);

                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    // Get Datbase Connection 
                    var col = db.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");

                    //foreach user get Data from Twitter and save to database
                    foreach (var x in AllMembers)
                    {

                        //Get timeline for screenname from twitter using Tweetinvi
                        var user = Tweetinvi.User.GetUserFromScreenName(x.ScreenName);
                        var userJson = Tweetinvi.JsonSerializer.ToJson(user);

                        var alreadyUpdated = col.Find(s => s.Screen_name == x.ScreenName).Where(s => s.DateToday == DateTime.Today);

                        if (alreadyUpdated.Count() == 0)
                        {
                            var twitterUserDaily = new TwitterUserDailyModel
                            {
                                Screen_name = x.ScreenName,
                                Statuses_count = x.StatusesCount,
                                Followers_count = x.FollowersCount,
                                Friends_count = x.FriendsCount,
                                Favourites_count = x.FavouritesCount,
                                Listed_count = x.ListedCount,
                                DateToday = DateTime.Today
                            };


                            //Create new database entry for given user
                            col.Insert(twitterUserDaily);
                            _logger.LogInformation("...created new dbentry => " + x.ScreenName + " on " + DateTime.Now.ToString("dd MM yy - hh:mm:ss"));
                        }
                        else
                        {
                            _logger.LogInformation("TU24...already uptodate => " + x.ScreenName);
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
            _logger.LogInformation("Twitter User Daily Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
