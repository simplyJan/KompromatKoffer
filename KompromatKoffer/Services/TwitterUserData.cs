
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
            _logger.LogInformation("Twitter User Data Service is starting.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(Config.Parameter.TwitterUserUpdateInterval));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Twitter User Data Service - " + DateTime.Now);

            Task.Delay(Config.Parameter.TaskDelay);

            try
            {
                var dbLastUpdated = Config.Parameter.DbLastUpdated;

                //Updatedelay reached => New Update => wait for new Update
                if (dbLastUpdated.AddMinutes(Config.Parameter.UpdateDelay) < DateTime.Now)
                {
                    //Set database was last updated
                    Config.Parameter.DbLastUpdated = DateTime.Now;

                    //Get all members from TwitterList - Tweetinvi
                    var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);
                    //var AllMembers = list.GetMembers(5); //Just 5 Records for Debug Reasons
                    var AllMembers = list.GetMembers(list.MemberCount);

                    //foreach user get json from Twitter and save to disk
                    foreach (var x in AllMembers)
                    {
                        //Get timeline for screenname from twitter using Tweetinvi
                        var user = Tweetinvi.User.GetUserFromScreenName(x.ScreenName);

                        using (var db = new LiteDatabase("TwitterData.db"))
                        {

                            var mapper = BsonMapper.Global;

                            mapper.Entity<TwitterUserModel>()
                                .Id(y => y.Id); // set your document ID

                            var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                            //Search for the Name
                            var id = col.FindOne(a => a.Id == x.Id);

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
                                _logger.LogInformation("...created new dbentry => " + x.ScreenName);

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
                                        PoliticalParty = "filloutbyhandfornow"
                                    };

                                    //Update User if name is not null and if the saveinterval is reached^^
                                    col.Update(twitterUser);
                                    _logger.LogInformation("...updated dbentry => " + x.ScreenName);

                                }
                                else
                                {
                                    if (id != null)
                                    {
                                        _logger.LogInformation("TUD...already updated => " + x.ScreenName);
                                    }
                                    else
                                    {
                                        _logger.LogInformation("...not found => " + x.ScreenName);
                                    }
                                }
                            }
                        }

                        Task.Delay(2500);
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

