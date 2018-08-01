
using KompromatKoffer.Areas.Database.Model;
using KompromatKoffer.Pages;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace KompromatKoffer.Areas.Database.Pages
{

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

        public IEnumerable<TwitterUserModel> CompleteDB { get; set; }

        public PaginatedList<TwitterUserModel> TwitterUserModel { get; set; }

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
            
            using (var db = new LiteDatabase(path + dataDirectory + @"\TwitterData.db"))
            {

                CurrentSort = sortOrder;

                var col = db.GetCollection<TwitterUserModel>("TwitterUser");
                var completeDB = col.FindAll();
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
                        s => s.Name.ToLower().Contains(searchString.ToLower())
                        );

                }

                //Sorting
                NameSort = sortOrder == "TwitterName" ? "TwitterName_Desc" : "TwitterName";
                StatusCountSort = sortOrder == "StatusesCount" ? "StatusesCount" : "StatusesCount_Desc";
                FollowersCountSort = sortOrder == "FollowersCount" ? "FollowersCount" : "FollowersCount_Desc";
                FriendsCountSort = sortOrder == "FriendsCount" ? "FriendsCount" : "FriendsCount_Desc";
                FavCountSort = sortOrder == "FavCount" ? "FavCount" : "FavCount_Desc";
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

            }
        }

        public async void StartTasksAsync()
        {
            await SaveTwitterUserAsync();

        }
    
        //Make Services for Updating Users - - Move to Services
        public async Task SaveTwitterUserAsync()
        {
            //Check if directory exists - move to startup!
            #region Check if DirectoryExists
            try
            {
                // Determine whether the directory exists.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (System.IO.Directory.Exists(dataDirectoryLinux))
                    {
                        _logger.LogInformation("...the data path exists already...");
                        return;
                    }
                }
                else
                {
                    if (System.IO.Directory.Exists(path + dataDirectory))
                    {
                        _logger.LogInformation("...the data path exists already...");
                        return;
                    }
                }

                // Try to create the directory.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path + dataDirectoryLinux);
                    _logger.LogInformation("...the data directory was created successfully at {0}...");
                }
                else
                {
                    System.IO.DirectoryInfo di = System.IO.Directory.CreateDirectory(path + dataDirectory);
                    _logger.LogInformation("...the data directory was created successfully at {0}...");

                }
            }
            catch (Exception e)
            {
                _logger.LogInformation("...directory creation failed: {0}...", e.ToString());
            }
            #endregion

            //FInally do the Database connection, save json to disk and put data in the db.
            finally
            {
                //Get all members from TwitterList - Tweetinvi
                var list = Tweetinvi.TwitterList.GetExistingList(Config.Parameter.ListName, Config.Parameter.ScreenName);
                //list.MemberCount
                //var AllMembers = list.GetMembers(list.MemberCount);
                var AllMembers = list.GetMembers(list.MemberCount);

                //foreach user get json from Twitter and save to disk
                foreach (var x in AllMembers)
                {
                    //Get timeline for screenname from twitter using Tweetinvi
                    var user = Tweetinvi.User.GetUserFromScreenName(x.ScreenName);
                    var userJson = Tweetinvi.JsonSerializer.ToJson(user);

                    string jsonFileName = x.ScreenName + ".json";
                    string fullPathToFile = path + dataDirectory + jsonFileName;
                    _logger.LogInformation("...found user..." + jsonFileName);

                    if (Config.Parameter.SaveToDisk == true)
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            System.IO.File.WriteAllText(
                                path
                                + dataDirectoryLinux + "/"
                                + jsonFileName,
                                userJson
                                );
                        }
                        else
                        {

                            System.IO.File.WriteAllText(
                                path
                                + dataDirectory
                                + jsonFileName,
                                userJson
                                );
                        }

                        _logger.LogInformation("...saved..." + x.ScreenName + "...to disk...");
                        _logger.LogInformation("=> " + fullPathToFile);
                    }

                    if(Config.Parameter.SaveToDatabase == true)
                    {
                        using (var db = new LiteDatabase(path + dataDirectory + @"\TwitterData.db"))
                        {
                            //How to put the whole fucking json to bson...?!
                            //var doc = JsonSerializer.Deserialize(userJson);
                            //db.GetCollection("TwitterUsers").Insert(doc.AsDocument);
                            var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                            var twitterUser = new TwitterUserModel
                            {  
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
                                Listed_count = x.ListedCount
                            };

                            var result = col.FindOne(z => z.Name == x.Name);

                            if (result != null)
                            {
                                col.Update(twitterUser);
                                _logger.LogInformation("...updated dbentry for => " + x.ScreenName);
                            }
                            else
                            {
                                col.Insert(twitterUser);
                                _logger.LogInformation("...created new dbentry for => " + x.ScreenName);
                            }               
                        }                
                    }
                    await Task.Delay(45);    
                }
            }
        }


       
    }
}