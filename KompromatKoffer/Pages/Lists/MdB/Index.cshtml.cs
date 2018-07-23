using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using KompromatKoffer.Model;
using KompromatKoffer.Models;
using Microsoft.AspNetCore.Authorization;

namespace KompromatKoffer.Pages.Lists.MdB
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private KompromatKoffer.Models.MdBContext _context;

        public IndexModel(KompromatKoffer.Models.MdBContext context)
        {
            _context = context;

            
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string StatusCountSort { get; set; }
        public string FollowersCountSort { get; set; }
        public string FriendsCountSort { get; set; }
        public string FavCountSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public IList<MdBModel> MdBModel { get;set; }

        public async Task OnGetAsync(string searchString, string sortOrder, string searchStringLastStatus)
        {

            //Update Database MOVE Out
            //await GetTwitterData();

            //Update some Data
            //await GetStatusData();

            //var mdbs = from Twittername in _context.MdBModel
            //             select Twittername;

            IQueryable<MdBModel> mdbs = from s in _context.MdBModel
                                            select s;

            //Search Filtering
            if (!String.IsNullOrEmpty(searchString))
            {
                mdbs = mdbs.Where(
                    s => s.TwitterName.Contains(searchString)
                    );

            }

            if (!String.IsNullOrEmpty(searchStringLastStatus))
            {
                mdbs = mdbs.Where(
                    s => s.LastStatus.Contains(searchStringLastStatus)
                    );

            }

            //Sorting
            NameSort = sortOrder == "TwitterName" ? "TwitterName_Desc" : "TwitterName";
            StatusCountSort = sortOrder == "StatusesCount" ? "StatusesCount_Desc" : "StatusesCount";
            FollowersCountSort = sortOrder == "FollowersCount" ? "FollowersCount_Desc" : "FollowersCount";
            FriendsCountSort = sortOrder == "FriendsCount" ? "FriendsCount_Desc" : "FriendsCount";
            FavCountSort = sortOrder == "FavCounts" ? "FavCounts_Desc" : "FavCounts";
            //DateSort = sortOrder == "Date" ? "date_desc" : "Date";
              
            switch (sortOrder)
            {
                case "TwitterName":
                mdbs = mdbs.OrderBy(s => s.TwitterName);
                    break;
                case "TwitterName_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.TwitterName);
                    break;
                case "StatusesCount":
                mdbs = mdbs.OrderBy(s => s.StatusesCount);
                    break;
                case "StatusesCount_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.StatusesCount);
                    break;
                case "FollowersCountSort":
                mdbs = mdbs.OrderBy(s => s.FollowersCount);
                    break;
                case "FollowersCount_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.FollowersCount);
                    break;
                case "FriendsCountSort":
                mdbs = mdbs.OrderBy(s => s.FriendsCount);
                    break;
                case "FriendsCount_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.FriendsCount);
                    break;
                case "FavCountSort":
                mdbs = mdbs.OrderBy(s => s.FavCounts);
                    break;
                case "FavCounts_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.FavCounts);
                    break;
                default:
                mdbs = mdbs.OrderBy(s => s.TwitterName);
                    break;
            }

            MdBModel = await mdbs.AsNoTracking().ToListAsync();
                  
        }


        public async Task GetTwitterData()
        {
            //Delete Entries
            var itemsToDelete = _context.Set<MdBModel>();
            _context.MdBModel.RemoveRange(itemsToDelete);
            _context.SaveChanges();
            

            var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);
              
            var AllMembers = list.GetMembers(600);

            
            foreach (var x in AllMembers)
            {
                
                var entry = new MdBModel();
                entry.TwitterID = x.Id;
                entry.IdString = x.IdStr;
                entry.TwitterName = x.Name;
                entry.TwitterScreenName = x.ScreenName;
                entry.TwitterProfileUrl = "https://twitter.com/" + x.ScreenName;
                entry.Verified = x.Verified;
                entry.TwitterDesc = x.Description;
                entry.Location = x.Location;
                entry.LinkedUrl = x.Url;
                entry.StatusesCount = x.StatusesCount;
                entry.FollowersCount = x.FollowersCount;
                entry.FriendsCount = x.FriendsCount;
                entry.FavCounts = x.FavouritesCount;
                entry.ProfileImageUrlHttps = x.ProfileImageUrlHttps;
                entry.CreatedAt = x.CreatedAt;    
                if (x.Status == null)
                {
                    entry.LastStatus = "N/A";
                }
                else
                {
                    entry.LastStatus = x.Status.Text;
                    
                }

                if (x.Status == null)
                {
                    entry.LastStatusCreated = new DateTime(666, 1, 1);
                   
                }
                else
                {
                    entry.LastStatusCreated = x.Status.CreatedAt;
                }

                
                _context.MdBModel.Add(entry);

                
            }

            await _context.SaveChangesAsync();

        }

        public async Task GetStatusData()
        {
            var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);

            var AllMembers = list.GetMembers(600);


            foreach (var x in AllMembers)
            {

                var entry = new MdBModel();
               
                entry.StatusesCount = x.StatusesCount;
                entry.FollowersCount = x.FollowersCount;
                entry.FriendsCount = x.FriendsCount;
                entry.FavCounts = x.FavouritesCount;
                entry.CreatedAt = x.CreatedAt;
                if (x.Status == null)
                {
                    entry.LastStatus = "N/A";
                }
                else
                {
                    entry.LastStatus = x.Status.Text;

                }

                if (x.Status == null)
                {
                    entry.LastStatusCreated = new DateTime(666, 1, 1);

                }
                else
                {
                    entry.LastStatusCreated = x.Status.CreatedAt;
                }


                _context.MdBModel.Update(entry);


            }

            await _context.SaveChangesAsync();



        }
    }
}
