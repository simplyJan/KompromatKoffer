
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
using Tweetinvi.Models;

namespace KompromatKoffer.Services
{
    internal class TwitterUserData : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private Timer _timer;

        public TwitterUserData(ILogger<TwitterUserData> logger)
        {
            _logger = logger;
        }

        public PaginatedList<TwitterUserDailyModel> TwitterUserDailyModel { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("===========> TwitterUserData Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(Config.Parameter.TwitterUserUpdateInterval));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            _logger.LogInformation("===========> TwitterUserData Service - " + DateTime.Now.ToString("dd.MM.yy - hh:mm"));

            //Perform Task Delay
            await Task.Delay(Config.Parameter.UpdateDelay*60*1000);

            try
            {
                    //Get all members from TwitterList - Tweetinvi
                    var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);
                    //var AllMembers = list.GetMembers(5); //Just 5 Records for Debug Reasons
                    var AllMembers = list.GetMembers(list.MemberCount);

                    using (var db = new LiteDatabase("TwitterData.db"))
                    {

                        var mapper = BsonMapper.Global;

                        mapper.Entity<TwitterUserModel>()
                            .Id(y => y.Id); // set your document ID

                        var col = db.GetCollection<TwitterUserModel>("TwitterUser");


                        //foreach user make the database update
                        foreach (var x in AllMembers)
                        {
                            
                            //Get timeline for screenname from twitter using Tweetinvi
                            //var user = Tweetinvi.User.GetUserFromScreenName(x.ScreenName);
     
                            //Search for the TweetUser ID
                            var id = col.FindOne(a => a.Id == x.Id);

                            var politicalParty = id.PoliticalParty;

                            if (id == null)
                            {
                                //Create UserModel for User
                                var twitterUser = new TwitterUserModel
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Screen_name = x.ScreenName,
                                    Description = x.Description,
                                    Created_at = x.CreatedAt,
                                    Location = x.Location,
                                    Geo_enabled = x.GeoEnabled,
                                    Url = x.Url,
                                    Statuses_count = x.StatusesCount,
                                    Followers_count = x.FollowersCount,
                                    Friends_count = x.FriendsCount,
                                    Verified = x.Verified,
                                    Profile_image_url_https = x.ProfileImageUrlHttps,
                                    Favourites_count = x.FavouritesCount,
                                    Listed_count = x.ListedCount,
                                    UserUpdated = DateTime.Now,
                                    PoliticalParty = "filloutbyhandfornow"
                                    
                                };

                                //Create new database entry for given user
                                col.Insert(twitterUser);
                                _logger.LogInformation(">> ...created new dbentry => " + x.ScreenName);

                            }
                            else
                            {
                                if (id.UserUpdated.AddMinutes(Config.Parameter.TwitterUserUpdateInterval) < DateTime.Now)
                                {
                                    //Create UserModel for User
                                    var twitterUser = new TwitterUserModel
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Screen_name = x.ScreenName,
                                        Description = x.Description,
                                        Created_at = x.CreatedAt,
                                        Location = x.Location,
                                        Geo_enabled = x.GeoEnabled,
                                        Url = x.Url,
                                        Statuses_count = x.StatusesCount,
                                        Followers_count = x.FollowersCount,
                                        Friends_count = x.FriendsCount,
                                        Verified = x.Verified,
                                        Profile_image_url_https = x.ProfileImageUrlHttps,
                                        Favourites_count = x.FavouritesCount,
                                        Listed_count = x.ListedCount,
                                        UserUpdated = DateTime.Now,
                                        PoliticalParty = politicalParty
                                    };

                                    //Update User if name is not null and if the saveinterval is reached^^
                                    col.Update(twitterUser);
                                    _logger.LogInformation(">> ...updated dbentry => " + x.ScreenName);

                                }
                                else
                                {
                                    if (id != null)
                                    {
                                        _logger.LogInformation(">> TUD...already updated => " + x.ScreenName);
                                    }
                                    else
                                    {
                                        _logger.LogInformation(">> ...not found => " + x.ScreenName);
                                    }
                                }
                            }
                            await Task.Delay(Config.Parameter.TwitterUserWriteDelay*1000);
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

