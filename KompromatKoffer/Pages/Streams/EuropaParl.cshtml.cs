using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tweetinvi.Models;

namespace KompromatKoffer.Pages.Streams
{
    public class EuropaParlModel : PageModel
    {
        public IEnumerable<ITweet> TweetList;

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string FavCountSort { get; set; }
        public string CreatedAtSort { get; set; }

        public void OnGet(string sortOrder, string searchString, int? pageIndex, string currentFilter)
        {

            CurrentSort = sortOrder;


            var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListNameMEP, Config.Parameter.ScreenNameMEP);

            //Settings for last 100
            Tweetinvi.Parameters.GetTweetsFromListParameters getTweetsParameters = new Tweetinvi.Parameters.GetTweetsFromListParameters()
            {
                MaximumNumberOfTweetsToRetrieve = Config.Parameter.TweetsRetrieved,
                IncludeRetweets = false,
                IncludeEntities = true,
            };

            var tweets = list.GetTweets(getTweetsParameters);

            TweetList = tweets;


            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            //Search Filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                TweetList = TweetList.Where(
                    s => s.CreatedBy.Name.ToLower().Contains(searchString.ToLower())
                    );

            }

            //Sorting
            FavCountSort = sortOrder == "FavCount" ? "FavCount" : "FavCount_Desc";
            CreatedAtSort = sortOrder == "CreatedAtDate" ? "CreatedAtDate_Desc" : "CreatedAtDate";

            switch (sortOrder)
            {
                case "FavCount":
                    TweetList = TweetList.OrderBy(s => s.FavoriteCount);
                    break;
                case "FavCount_Desc":
                    TweetList = TweetList.OrderByDescending(s => s.FavoriteCount);
                    break;
                case "CreatedAtDate":
                    TweetList = TweetList.OrderBy(s => s.CreatedAt);
                    break;
                case "CreatedAtDate_Desc":
                    TweetList = TweetList.OrderByDescending(s => s.CreatedAt);
                    break;
                default:
                    TweetList = TweetList.OrderByDescending(s => s.CreatedAt);
                    break;
            }
        }
    }
}