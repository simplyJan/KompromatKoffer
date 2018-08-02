
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

        public Tweetinvi.Models.IUser CurrentUser { get; set; }

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

                StartTasksAsync();

            }
        }

        public async void StartTasksAsync()
        {
            await SaveTwitterUserAsync();

        }
    
        //Make Service for Updating Users - - Move to Update Service
        public async Task SaveTwitterUserAsync()
        {
            var dbLastUpdated = Config.Parameter.DbLastUpdated;

            //Updatedelay reached => New Update
            if(dbLastUpdated.AddMinutes(Config.Parameter.UpdateDelay) < DateTime.Now)
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
                    var AllMembers = list.GetMembers(5);
                    //var AllMembers = list.GetMembers(list.MemberCount);

                    //foreach user get json from Twitter and save to disk
                    foreach (var x in AllMembers)
                    {
                        //Put CurrentUser in context
                        CurrentUser = x;

                        //Get timeline for screenname from twitter using Tweetinvi
                        var user = Tweetinvi.User.GetUserFromScreenName(x.ScreenName);
                        var userJson = Tweetinvi.JsonSerializer.ToJson(user);

                        string jsonFileName = x.ScreenName + ".json";
                        string fullPathToFile = path + dataDirectory + jsonFileName;


                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            DateTime lastModifiedLinux = System.IO.File.GetLastWriteTime(path
                                + dataDirectoryLinux + "/"
                                + jsonFileName);

                            _logger.LogInformation("...lastmodified: " + lastModifiedLinux);

                            //If File modified time + 5 min. < DateTime.Now
                            if (lastModifiedLinux.AddMinutes(5) < DateTime.Now)
                            {
                                if (Config.Parameter.SaveToDisk == true)
                                {
                                    System.IO.File.WriteAllText(
                                    path
                                    + dataDirectoryLinux + "/"
                                    + jsonFileName,
                                    userJson
                                    );
                                    _logger.LogInformation("...saved user..." + path + dataDirectoryLinux + "/" + jsonFileName);
                                }
                                if (Config.Parameter.SaveToDatabase == true)
                                {
                                    SaveToDatabase();
                                }
                            }
                        }
                        else
                        {

                            DateTime lastModified = System.IO.File.GetLastWriteTime(path
                                + dataDirectory
                                + jsonFileName);

                            _logger.LogInformation("...lastmodified: " + lastModified);

                            _logger.LogInformation("...new Update on: " + lastModified.AddMinutes(5));

                            //If File modified time + 5 min. < DateTime.Now
                            if (lastModified.AddMinutes(5) < DateTime.Now)
                            {
                                if (Config.Parameter.SaveToDisk == true)
                                {
                                    System.IO.File.WriteAllText(
                                        path
                                        + dataDirectory
                                        + jsonFileName,
                                        userJson
                                        );
                                    _logger.LogInformation("...saved user..." + path + dataDirectory + jsonFileName);
                                }

                                if (Config.Parameter.SaveToDatabase == true)
                                {
                                    SaveToDatabase();
                                }

                            }
                        }

                    }

                    Config.Parameter.DbLastUpdated = DateTime.Now;
                    await Task.Delay(1);
                }
            }
        }

        //Move to Update Service
        public void SaveToDatabase()
        {
            using (var db = new LiteDatabase(path + dataDirectory + @"\TwitterData.db"))
            {
                x = CurrentUser;

                //How to put the whole fucking json to bson...?!
                //var doc = JsonSerializer.Deserialize(userJson);
                //db.GetCollection("TwitterUsers").Insert(doc.AsDocument);
                var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                //Search for the Name
                var name = col.FindOne(a => a.Screen_name == x.ScreenName);

                //Create UserModel for User
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
                    Listed_count = x.ListedCount,
                    UserUpdated = DateTime.Now
                };

                //If Name in Database do update or already updated
                if (name != null)
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
       
    }
}