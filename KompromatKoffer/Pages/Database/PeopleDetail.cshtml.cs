using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tweetinvi;
using Tweetinvi.Parameters;
using Tweetinvi.Exceptions;
using Microsoft.Extensions.Logging;
using System.Linq;
using Tweetinvi.Models;
using LiteDB;
using KompromatKoffer.Areas.Database.Model;

namespace KompromatKoffer.Pages.Database
{
    [Authorize]
    public class PeopleDetailModel : PageModel
    {

        private readonly ILogger<PeopleDetailModel> _logger;

        public PeopleDetailModel(ILogger<PeopleDetailModel> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Tweetinvi.Models.ITweet> TwitterUserTimeline;
        public Tweetinvi.Models.IUser CurrentTwitterUser;
        public List<String> TweetHistoryDates { get; set; }
        public List<ITweet> AllTweetsFromUser;

        public LiteCollection<TwitterUserDailyModel> CompleteDB;

        public LiteCollection<TwitterUserModel> TwitterUserData;

        public LiteCollection<TwitterUserTimelineModel> TwitterUserTimelineData;

        [BindProperty]
        public string CurrentUserScreenname { get; set; }

        //Sorting
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string FavCountSort { get; set; }
        public string CreatedAtSort { get; set; }
        public string RetweetCountSort { get; set; }

        public void OnGet(string screenname, string sortOrder, string currentFilter)
        {
            //Set Screenname if null - doh!
            if (screenname == null)
            {
                screenname = "swagenknecht";
            }
            CurrentUserScreenname = screenname;


            #region Database Connection - get TwitterUserDailyCollection

            try
            {


                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    var col = db.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");
                    CompleteDB = col;

                    var col2 = db.GetCollection<TwitterUserModel>("TwitterUser");
                    TwitterUserData = col2;

                    var col3 = db.GetCollection<TwitterUserTimelineModel>("TwitterUserTimeline");
                    TwitterUserTimelineData = col3;


                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation("Exception " + ex);
            }

            #endregion


            //Get UserTimeline with screenname - doh!
            //await GetUserTimeline(screenname, sortOrder);

        }

        public async Task GetUserTimeline(string currentUserScreenname, string sortOrder)
        {
            ExceptionHandler.SwallowWebExceptions = false;
            try
            {
                //Sorting
                CurrentSort = sortOrder;

                //Check for Rate Limits
                RateLimit.RateLimitTrackerMode = RateLimitTrackerMode.TrackAndAwait;

                RateLimit.QueryAwaitingForRateLimit += (sender, args) =>
                {
                    _logger.LogInformation("Is awaiting for rate limits... " + args.Query);
                };

                    // Get user from twitter using screenname
                var user = Tweetinvi.User.GetUserFromScreenName(currentUserScreenname);
                CurrentTwitterUser = user;
                var userIdentifier = user.UserIdentifier;

                CurrentUserScreenname = currentUserScreenname;

                #region Old Grab Timeline from Twitter
                /*

                var lastTweets = Timeline.GetUserTimeline(currentUserScreenname, 15).ToArray();

                var allTweets = new List<ITweet>(lastTweets);
                var beforeLast = allTweets;

                //Get more Tweets from User
                while (lastTweets.Length > 0 && allTweets.Count <= 30)
                {
                    var idOfOldestTweet = lastTweets.Select(x => x.Id).Min();

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


                    lastTweets = Timeline.GetUserTimeline(currentUserScreenname, userTimelineParameters).ToArray();
                    allTweets.AddRange(lastTweets);

                    //var userTimeline = Timeline.GetUserTimeline(userIdentifier, userTimelineParameters);
                    //TwitterUserTimeline = userTimeline;

                }

                AllTweetsFromUser = allTweets;
                */


                #endregion


                //Sorting
                FavCountSort = sortOrder == "FavCount_Desc" ? "FavCount" : "FavCount_Desc";
                CreatedAtSort = sortOrder == "CreatedAtDate_desc" ? "CreatedAtDate" : "CreatedAtDate_desc";
                RetweetCountSort = sortOrder == "RetweetCount_Desc" ? "RetweetCount" : "RetweetCount_Desc";

                switch (sortOrder)
                {
                    case "FavCount":
                        AllTweetsFromUser = AllTweetsFromUser.OrderBy(s => s.FavoriteCount).ToList();
                        break;
                    case "FavCount_Desc":
                        AllTweetsFromUser = AllTweetsFromUser.OrderByDescending(s => s.FavoriteCount).ToList();
                        break;
                    case "RetweetCount":
                        AllTweetsFromUser = AllTweetsFromUser.OrderBy(s => s.RetweetCount).ToList();
                        break;
                    case "RetweetCount_Desc":
                        AllTweetsFromUser = AllTweetsFromUser.OrderByDescending(s => s.RetweetCount).ToList();
                        break;
                    case "CreatedAtDate":
                        AllTweetsFromUser = AllTweetsFromUser.OrderBy(s => s.CreatedAt).ToList();
                        break;
                    case "CreatedAtDate_Desc":
                        AllTweetsFromUser = AllTweetsFromUser.OrderByDescending(s => s.CreatedAt).ToList();
                        break;
                    default:
                        AllTweetsFromUser = AllTweetsFromUser.OrderByDescending(s => s.CreatedAt).ToList();
                        break;
                }

                await Task.Delay(1);

            }
            catch (TwitterException ex)
            {
                _logger.LogInformation("Error with tweetinvi... " + ex);
            }
            catch (ArgumentException ex)
            {
                _logger.LogInformation("ArgumentException... " + ex);
            }
            catch (Exception error)
            {
                _logger.LogInformation("Error... " + error);
            }
        }



    }
}