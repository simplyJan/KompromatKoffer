
using KompromatKoffer.Areas.Database.Model;
using KompromatKoffer.Pages;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace KompromatKoffer.Areas.Database.Pages
{
    [Authorize]
    public class PeopleModel : PageModel
    {
        private readonly ILogger<PeopleModel> _logger;

        public PeopleModel(ILogger<PeopleModel> logger)
        {
            _logger = logger;
        }

        public static string path = System.IO.Directory.GetCurrentDirectory();
        public static string dataDirectory = @"\Database\";
        public static string dataDirectoryLinux = @"/Database";

        private IUser x;

        public IEnumerable<TwitterUserModel> CompleteDB { get; set; }

        public PaginatedList<TwitterUserModel> TwitterUserModel { get; set; }

        public LiteCollection<TwitterUserModel> TwitterUserCol { get; set; }

        public Tweetinvi.Models.IUser CurrentUser { get; set; }

        public string MemberCount { get; set; }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string StatusCountSort { get; set; }
        public string FollowersCountSort { get; set; }
        public string FriendsCountSort { get; set; }
        public string FavCountSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public string CreatedAtSort { get; set; }

        public async Task OnGet(string searchString, int? pageIndex, string currentFilter, string sortOrder)
        {
            
            using (var db = new LiteDatabase("TwitterData.db"))
            {

                CurrentSort = sortOrder;

                var col = db.GetCollection<TwitterUserModel>("TwitterUser");
                var completeDB = col.FindAll();
                CompleteDB = completeDB;

                TwitterUserCol = col;

                MemberCount = Convert.ToString(TwitterUserCol.Count());

                _logger.LogInformation(Convert.ToString(TwitterUserCol.Count())+"MdB in der Datenbank");

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
                        s => s.Name.ToLower().Contains(searchString.ToLower())
                        );

                }

                //Sorting
                NameSort = sortOrder == "TwitterName" ? "TwitterName_Desc" : "TwitterName";
                StatusCountSort = sortOrder == "StatusesCount_Desc" ? "StatusesCount" : "StatusesCount_Desc";
                FollowersCountSort = sortOrder == "FollowersCount_Desc" ? "FollowersCount" : "FollowersCount_Desc";
                FriendsCountSort = sortOrder == "FriendsCount_Desc" ? "FriendsCount" : "FriendsCount_Desc";
                FavCountSort = sortOrder == "FavCount_Desc" ? "FavCount" : "FavCount_Desc";
                DateSort = sortOrder == "LastCreatedDate" ? "LastCreatedDate_Desc" : "LastCreatedDate";
                CreatedAtSort = sortOrder == "CreatedAtDate" ? "CreatedAtDate_Desc" : "CreatedAtDate";

                switch (sortOrder)
                {
                    case "TwitterName":
                        CompleteDB = CompleteDB.OrderBy(s => s.Name);
                        break;
                    case "TwitterName_Desc":
                        CompleteDB = CompleteDB.OrderByDescending(s => s.Name);
                        break;
                    case "StatusesCount":
                        CompleteDB = CompleteDB.OrderBy(s => s.Statuses_count);
                        break;
                    case "StatusesCount_Desc":
                        CompleteDB = CompleteDB.OrderByDescending(s => s.Statuses_count);
                        break;
                    case "FollowersCount":
                        CompleteDB = CompleteDB.OrderBy(s => s.Followers_count);
                        break;
                    case "FollowersCount_Desc":
                        CompleteDB = CompleteDB.OrderByDescending(s => s.Followers_count);
                        break;
                    case "FriendsCount":
                        CompleteDB = CompleteDB.OrderBy(s => s.Friends_count);
                        break;
                    case "FriendsCount_Desc":
                        CompleteDB = CompleteDB.OrderByDescending(s => s.Friends_count);
                        break;
                    case "FavCount":
                        CompleteDB = CompleteDB.OrderBy(s => s.Favourites_count);
                        break;
                    case "FavCount_Desc":
                        CompleteDB = CompleteDB.OrderByDescending(s => s.Favourites_count);
                        break;
                    case "CreatedAtDate":
                        CompleteDB = CompleteDB.OrderBy(s => s.Created_at);
                        break;
                    case "CreatedAtDate_Desc":
                        CompleteDB = CompleteDB.OrderByDescending(s => s.Created_at);
                        break;
                    default:
                        CompleteDB = CompleteDB.OrderByDescending(s => s.Followers_count);
                        break;
                }

                int pageSize = 100;
                TwitterUserModel = await PaginatedList<TwitterUserModel>.CreateAsync(
                CompleteDB, pageIndex ?? 1, pageSize);

                //StartTasksAsync();

            }
        }

        public async void StartTasksAsync()
        {
            await GetTwitterUserListAsync();

        }
    
        //Make Service for Updating Users - - Move to Update Service
        public async Task GetTwitterUserListAsync()
        {
            var dbLastUpdated = Config.Parameter.DbLastUpdated;

            //Updatedelay reached => New Update => wait for new Update
            if(dbLastUpdated.AddMinutes(Config.Parameter.UpdateDelay) < DateTime.Now)
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
                    
                        //Put CurrentUser in context
                        CurrentUser = x;

                        //Get timeline for screenname from twitter using Tweetinvi
                        var user = Tweetinvi.User.GetUserFromScreenName(x.ScreenName);
                        var userJson = Tweetinvi.JsonSerializer.ToJson(user);

                        await SaveToDatabase();

                
                    
                }
            }
        }

        //Move to Update Service
        public async Task SaveToDatabase()
        {
            using (var db = new LiteDatabase("TwitterData.db"))
            {
                x = CurrentUser;

                var mapper = BsonMapper.Global;

                mapper.Entity<TwitterUserModel>()
                    .Id(x => x.Id); // set your document ID

                var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                //Search for the Name
                var name = col.FindOne(a => a.Screen_name == x.ScreenName);

                if (name == null)
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
                        UserUpdated = DateTime.Now
                    };

                    //Create new database entry for given user
                    col.Insert(twitterUser);
                    _logger.LogInformation("...created new dbentry => " + x.ScreenName);

                }
                else
                {
                    if (name.UserUpdated.AddMinutes(Config.Parameter.SaveInterval) < DateTime.Now)
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
                            UserUpdated = DateTime.Now
                        };

                        //Update User if name is not null and if the saveinterval is reached^^
                        col.Update(twitterUser);
                        _logger.LogInformation("...updated dbentry => " + x.ScreenName);

                    }
                    else
                    {
                        if (name != null)
                        {
                            _logger.LogInformation("...already updated => " + x.ScreenName);
                        }
                        else
                        {
                            _logger.LogInformation("...not found => " + x.ScreenName);
                        }
                    }
                }
                await Task.Delay(200);
            }
        }     

    }
}