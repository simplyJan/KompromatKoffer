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
using Microsoft.AspNetCore.Identity;
using KompromatKoffer.Areas.Identity.Data;
using static KompromatKoffer.Areas.Identity.Pages.Account.ExternalLoginModel;

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
        public string CreatedAtSort { get; set; }

        public PaginatedList<MdBModel> MdBModel { get; set; }

        public async Task OnGetAsync(string searchString, string sortOrder, string searchStringLastStatus, string currentFilter, int? pageIndex, string searchStringLocation, string searchStringDesc)
        {



            #region => Sorting - Filtering - Pagination
            CurrentSort = sortOrder;

            IQueryable<MdBModel> mdbs = from s in _context.MdBModel
                                        select s;
            //Indexing
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

            if (!String.IsNullOrEmpty(searchStringLocation))
            {
                mdbs = mdbs.Where(
                    s => s.Location.Contains(searchStringLocation)
                    );

            }

            if (!String.IsNullOrEmpty(searchStringDesc))
            {
                mdbs = mdbs.Where(
                    s => s.TwitterDesc.Contains(searchStringDesc)
                    );

            }

            //Sorting
            NameSort = sortOrder == "TwitterName" ? "TwitterName_Desc" : "TwitterName";
            StatusCountSort = sortOrder == "StatusesCount" ? "StatusesCount_Desc" : "StatusesCount";
            FollowersCountSort = sortOrder == "FollowersCount" ? "FollowersCount_Desc" : "FollowersCount";
            FriendsCountSort = sortOrder == "FriendsCount" ? "FriendsCount_Desc" : "FriendsCount";
            FavCountSort = sortOrder == "FavCount" ? "FavCount_Desc" : "FavCount";
            DateSort = sortOrder == "LastCreatedDate" ? "LastCreatedDate_Desc" : "LastCreatedDate";
            CreatedAtSort = sortOrder == "CreatedAtDate" ? "CreatedAtDate_Desc" : "CreatedAtDate";

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
                case "FollowersCount":
                    mdbs = mdbs.OrderBy(s => s.FollowersCount);
                    break;
                case "FollowersCount_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.FollowersCount);
                    break;
                case "FriendsCount":
                    mdbs = mdbs.OrderBy(s => s.FriendsCount);
                    break;
                case "FriendsCount_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.FriendsCount);
                    break;
                case "FavCount":
                    mdbs = mdbs.OrderBy(s => s.FavCounts);
                    break;
                case "FavCount_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.FavCounts);
                    break;
                case "LastCreatedDate":
                    mdbs = mdbs.OrderBy(s => s.LastStatusCreated);
                    break;
                case "LastCreatedDate_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.LastStatusCreated);
                    break;
                case "CreatedAtDate":
                    mdbs = mdbs.OrderBy(s => s.CreatedAt);
                    break;
                case "CreatedAtDate_Desc":
                    mdbs = mdbs.OrderByDescending(s => s.CreatedAt);
                    break;
                default:
                    mdbs = mdbs.OrderBy(s => s.TwitterName);
                    break;
            }


            int pageSize = 100;
            MdBModel = await PaginatedList<MdBModel>.CreateAsync(
                mdbs.AsNoTracking(), pageIndex ?? 1, pageSize);


            #endregion

            //Manual Update Database
            //await GetTwitterData();

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

        public async Task UpdateStatusData()
        {
            var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);

            var AllMembers = list.GetMembers(600);

            foreach (var x in AllMembers)
            {

                var entry = new MdBModel();

                _context.MdBModel.Attach(entry);

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


                _context.Entry(entry).Property("LastStatus").IsModified = true;

            }

            await _context.SaveChangesAsync();

        }

        
    }
}
