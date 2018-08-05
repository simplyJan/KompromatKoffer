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

        [BindProperty]
        public string CurrentUserScreenname { get; set; }

        public async Task OnGetAsync(string screenname)
        {

            if (screenname == null)
            {
                screenname = "swagenknecht";
            }

            
            CurrentUserScreenname = screenname;


            await GetUserTimeline(screenname);


        }


        public async Task GetUserTimeline(string currentUserScreenname)
        {
            ExceptionHandler.SwallowWebExceptions = false;
            try
            {
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

                var lastTweets = Timeline.GetUserTimeline(currentUserScreenname, 50).ToArray();

                var allTweets = new List<ITweet>(lastTweets);
                var beforeLast = allTweets;

                //Get more Tweets from User
                while (lastTweets.Length > 0 && allTweets.Count <= 100)
                {
                    var idOfOldestTweet = lastTweets.Select(x => x.Id).Min();

                    var numberOfTweetsToRetrieve = allTweets.Count > 50 ? 100 - allTweets.Count : 50;

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