using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KompromatKoffer.Areas.Database.Model;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections;

namespace KompromatKoffer.Pages.Database
{
    [Authorize]
    public class GlobalModel : PageModel
    {

        private readonly ILogger<GlobalModel> _logger;

        public GlobalModel(ILogger<GlobalModel> logger)
        {
            _logger = logger;
        }

        public LiteCollection<TwitterUserDailyModel> CompleteDB;

        [BindProperty]
        public string CurrentUserScreenname { get; set; }

        public string LittleTuple;

        public int SinceDays { get; set; } = -7;

        public int DaysRange { get; set; }
        public int CurrentRange { get; set; }

        public IEnumerable<string> DistinctNames { get; set;}
        public IEnumerable<IGrouping <string, TwitterUserDailyModel>> AllEntries { get; set; }
        public IEnumerable<int> FollowersCountAll { get; set; }
        public IEnumerable<int> FriendsCountAll { get; set; }
        public IEnumerable<int> StatusesCountAll { get; set; }
        public IEnumerable<int> FavouritesCountAll { get; set; }

        public async Task OnGet(int sinceDays)
        {
            
            try
            {
                //Getting Collection from LiteDB
                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    var col = db.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");
                    CompleteDB = col;

                }

                

                // Get all Rows from Collection since x days
                var name = CompleteDB.Find(s => s.DateToday > DateTime.Today.AddDays(sinceDays));

                //Get follower count sorted by Screen_name
                FollowersCountAll = name.GroupBy(s => s.Screen_name).Select(s => s.Select(item => item.Followers_count).Last() - s.Select(item => item.Followers_count).First());
                FriendsCountAll = name.GroupBy(s => s.Screen_name).Select(s => s.Select(item => item.Friends_count).Last() - s.Select(item => item.Friends_count).First());
                StatusesCountAll = name.GroupBy(s => s.Screen_name).Select(s => s.Select(item => item.Statuses_count).Last() - s.Select(item => item.Statuses_count).First());
                FavouritesCountAll = name.GroupBy(s => s.Screen_name).Select(s => s.Select(item => item.Favourites_count).Last() - s.Select(item => item.Favourites_count).First());

                //Sort all entries by Screen_name
                AllEntries = name.GroupBy(s => s.Screen_name);
                

                //new list for allScreennames
                List<string> allScreenNames = new List<string>();

                //Add all to List
                foreach (var item in AllEntries)
                {

                    foreach (var items in item)
                    {
                        allScreenNames.Add(items.Screen_name);

                    }

                }

                //Distinct Screennames
                var distinctScreenNames = allScreenNames.Distinct();

                DistinctNames = distinctScreenNames;

                //Put both in one --concat --combine



            }
            catch (LiteException ex)
            {
                _logger.LogInformation("LiteDB Exception..." + ex);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception " + ex);
            }

            

            SinceDays = sinceDays;


            await Task.Delay(1);

        }







    }
}