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

        public LiteCollection<TwitterStreamModel> TwitterStreamData;

        [BindProperty]
        public string CurrentUserScreenname { get; set; }

        public int SinceDays { get; set; } = -7;

        public int DaysRange { get; set; }
        public int CurrentRange { get; set; }

        //Sorting
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string FavCountSort { get; set; }
        public string CreatedAtSort { get; set; }
        public string RetweetCountSort { get; set; }

        public void OnGet(string screenname, string sortOrder, string currentFilter, int sinceDays)
        {
            //Set Screenname if null - doh!
            if (screenname == null)
            {
                screenname = "swagenknecht";
            }
            CurrentUserScreenname = screenname;


            #region Database Connection - get TwitterUserDailyCollection

            //Getting the collections
            try
            {
                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    var col = db.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");
                    CompleteDB = col;

                    var col2 = db.GetCollection<TwitterUserModel>("TwitterUser");
                    TwitterUserData = col2;

                    //var col3 = db.GetCollection<TwitterUserTimelineModel>("TwitterUserTimeline");
                    //TwitterUserTimelineData = col3;

                    var col4 = db.GetCollection<TwitterStreamModel>("TwitterStream");
                    TwitterStreamData = col4;
                }
            }
            catch (LiteException ex)
            {
                _logger.LogInformation("LiteDB Exception " + ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception " + ex);
            }

            #endregion

            SinceDays = sinceDays;

            /*
            //Sorting - CUrrently off
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
            */
        }

    }
}