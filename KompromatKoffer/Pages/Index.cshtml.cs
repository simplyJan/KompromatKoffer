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
using Microsoft.AspNetCore.Authorization;

namespace KompromatKoffer.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IEnumerable<TwitterStreamModel> CompleteDB { get; set; }

        public PaginatedList<TwitterStreamModel> TwitterStreamModel { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string FavCountSort { get; set; }
        public string RetweetCountSort { get; set; }
        public string CreatedAtSort { get; set; }

        public string WarningMessage { get; set; }

        public int TimeRange { get; set; } = Config.Parameter.TwitterStreamDayRange;

        public int TweetLimit { get; set; } = Config.Parameter.IndexTweetLimit;

        public async Task OnGet(string searchString, int? pageIndex, string currentFilter, string sortOrder)
        {
            try
            {
                WarningMessage = Config.Parameter.WarningMessage;

                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    CurrentSort = sortOrder;

                    var col = db.GetCollection<TwitterStreamModel>("TwitterStream");

                    //var completeDB = col.FindAll().Where(s => s.TweetCreatedAt > DateTime.Now.AddDays(TimeRange));

                    var completeDB = col.Find(Query.All(Query.Descending), limit: TweetLimit);

                    CompleteDB = completeDB;

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
                        CompleteDB = CompleteDB.Where(
                            s => s.TweetText != null).Where(s => s.TweetText.ToLower().Contains(searchString.ToLower())
                            );
                    }

                    //TimeRange
                    //TimeRange = timeRange;

                    //Sorting
                    FavCountSort = sortOrder == "FavCount_Desc" ? "FavCount" : "FavCount_Desc";
                    RetweetCountSort = sortOrder == "RetweetCount_Desc" ? "RetweetCount" : "RetweetCount_Desc";
                    CreatedAtSort = sortOrder == "CreatedAtDate_desc" ? "CreatedAtDate" : "CreatedAtDate_desc";

                    switch (sortOrder)
                    {
                        case "FavCount":
                            CompleteDB = CompleteDB.OrderBy(s => s.TweetFavoriteCount);
                            break;
                        case "FavCount_Desc":
                            CompleteDB = CompleteDB.OrderByDescending(s => s.TweetFavoriteCount);
                            break;
                        case "RetweetCount":
                            CompleteDB = CompleteDB.OrderBy(s => s.TweetReTweetCount);
                            break;
                        case "RetweetCount_Desc":
                            CompleteDB = CompleteDB.OrderByDescending(s => s.TweetReTweetCount);
                            break;
                        case "CreatedAtDate":
                            CompleteDB = CompleteDB.OrderBy(s => s.TweetCreatedAt);
                            break;
                        case "CreatedAtDate_Desc":
                            CompleteDB = CompleteDB.OrderByDescending(s => s.TweetCreatedAt);
                            break;
                        default:
                            CompleteDB = CompleteDB.OrderByDescending(s => s.TweetCreatedAt);
                            break;
                    }

                    if (sortOrder != null)
                    {
                        _logger.LogInformation("Sort order is... " + sortOrder);
                    }
                }
                int pageSize = Config.Parameter.ShowEntries;
                TwitterStreamModel = await PaginatedList<TwitterStreamModel>.CreateAsync(
                CompleteDB, pageIndex ?? 1, pageSize);
            }
            catch(ArgumentException ex)
            {
                _logger.LogInformation("Argument Exception... " + ex);
            }
            catch(LiteException ex)
            {
                _logger.LogInformation("Error with LiteDB... " + ex);

            }
            catch(Exception ex)
            {
                _logger.LogInformation("Error with tweetinvi... " + ex);
            }


        }

    


    }
}
