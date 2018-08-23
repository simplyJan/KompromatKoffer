using KompromatKoffer.Areas.Database.Model;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace KompromatKoffer.Pages
{
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IEnumerable<TwitterStreamModel> TweetList;

        public IEnumerable<TwitterStreamModel> CompleteDB { get; set; }

        public PaginatedList<TwitterStreamModel> TwitterStreamModel { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string FavCountSort { get; set; }
        public string CreatedAtSort { get; set; }


        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task OnGet(string sortOrder, string searchString, int? pageIndex, string currentFilter)
        {
            try
            {
                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    CurrentSort = sortOrder;


                    var col = db.GetCollection<TwitterStreamModel>("TwitterStream");
                    var completeDB = col.FindAll();
                    CompleteDB = completeDB;

                    /*
                    var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);

                    //Settings for last 100
                    Tweetinvi.Parameters.GetTweetsFromListParameters getTweetsParameters = new Tweetinvi.Parameters.GetTweetsFromListParameters()
                    {
                        MaximumNumberOfTweetsToRetrieve = Config.Parameter.TweetsRetrieved,
                        IncludeRetweets = false,
                        IncludeEntities = true,
                    };
                    */

                    var tweets = completeDB;

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
                            s => s.TweetText.ToLower().Contains(searchString.ToLower())
                            );

                    }

                    //Sorting
                    FavCountSort = sortOrder == "FavCount_Desc" ? "FavCount" : "FavCount_Desc";
                    CreatedAtSort = sortOrder == "CreatedAtDate_desc" ? "CreatedAtDate" : "CreatedAtDate_desc";

                    switch (sortOrder)
                    {
                        case "FavCount":
                            TweetList = TweetList.OrderBy(s => s.TweetFavoriteCount);
                            break;
                        case "FavCount_Desc":
                            TweetList = TweetList.OrderByDescending(s => s.TweetFavoriteCount);
                            break;
                        case "CreatedAtDate":
                            TweetList = TweetList.OrderBy(s => s.TweetCreatedAt);
                            break;
                        case "CreatedAtDate_Desc":
                            TweetList = TweetList.OrderByDescending(s => s.TweetCreatedAt);
                            break;
                        default:
                            TweetList = TweetList.OrderByDescending(s => s.TweetCreatedAt);
                            break;
                    }

                    if (sortOrder != null)
                    {
                        _logger.LogInformation("Sort order is... " + sortOrder);
                    }
                }

                int pageSize = 50;
                TwitterStreamModel = await PaginatedList<TwitterStreamModel>.CreateAsync(
                TweetList, pageIndex ?? 1, pageSize);



            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error with tweetinvi... " + ex);
            }


        }

    


    }
}
