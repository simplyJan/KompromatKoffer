using System;
using System.Collections.Generic;
using System.Linq;
using KompromatKoffer.Areas.Database.Model;
using LiteDB;

namespace DatabaseMaintenance
{
    class Program
    {
        static void Main(string[] args)
        {


            //PoliticalPartyUpdate();
            //FixDBwithLast();

            InsertNewUserDaily();

            //UpdateTwitterUserDaily();

        }


        private static void PoliticalPartyUpdate()
        {
            using (var db = new LiteDatabase("TwitterData.db"))
            {
                try
                {
                    Console.WriteLine(">>> Getting Collection...");
                    var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                    var partyList = col.FindAll().Where(s => s.PoliticalParty == null);

                    Console.WriteLine(">>> Political Partymembers:");

                    List<string> abfall = new List<string>();
                    List<string> green = new List<string>();
                    List<string> left = new List<string>();
                    List<string> afd = new List<string>();
                    List<string> cducsu = new List<string>();
                    List<string> spd = new List<string>();
                    List<string> fdp = new List<string>();
                    List<string> blau = new List<string>();
                    List<string> nix = new List<string>();

                    bool debugLine = false;

                    foreach (var entry in partyList)
                    {
                        if (debugLine == true)
                            Console.WriteLine(">>> Found: {0} ", entry.Name.ToString());

                        //Change with desciption

                        if (entry.Description == null)
                        {
                            if (debugLine == true)
                                Console.WriteLine("Is Abfall ");
                            abfall.Add(entry.Name);
                        }
                        else
                        {


                            if (entry.Description.Contains("BÜNDNIS 90/DIE GRÜNEN")
                                || entry.Description.Contains("Grüne")
                                || entry.Description.Contains("grüne")
                                || entry.Description.Contains("Gruene")
                                || entry.Description.Contains("gruene")
                                )
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is GRÜNE");
                                green.Add(entry.Name);
                                var userModel = new TwitterUserModel
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Screen_name = entry.Screen_name,
                                    Description = entry.Description,
                                    Created_at = entry.Created_at,
                                    Location = entry.Location,
                                    Geo_enabled = entry.Geo_enabled,
                                    Url = entry.Url,
                                    Statuses_count = entry.Statuses_count,
                                    Followers_count = entry.Followers_count,
                                    Friends_count = entry.Friends_count,
                                    Verified = entry.Verified,
                                    Profile_image_url_https = entry.Profile_image_url_https,
                                    Favourites_count = entry.Favourites_count,
                                    Listed_count = entry.Listed_count,
                                    UserUpdated = entry.UserUpdated,
                                    PoliticalParty = "BÜNDNIS 90/DIE GRÜNEN",
                                };


                                col.Update(userModel);

                            }
                            else if (entry.Description.Contains("DIE LINKE")
                                || entry.Description.Contains("LINKE")
                                || entry.Description.Contains("linke")
                                || entry.Description.Contains("linksfraktion")
                                || entry.Description.Contains("links")
                                || entry.Description.Contains("DieLinke")
                                || entry.Description.Contains("Linksfraktion")
                                )
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is DIE LINKE");
                                left.Add(entry.Name);

                                var userModel = new TwitterUserModel
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Screen_name = entry.Screen_name,
                                    Description = entry.Description,
                                    Created_at = entry.Created_at,
                                    Location = entry.Location,
                                    Geo_enabled = entry.Geo_enabled,
                                    Url = entry.Url,
                                    Statuses_count = entry.Statuses_count,
                                    Followers_count = entry.Followers_count,
                                    Friends_count = entry.Friends_count,
                                    Verified = entry.Verified,
                                    Profile_image_url_https = entry.Profile_image_url_https,
                                    Favourites_count = entry.Favourites_count,
                                    Listed_count = entry.Listed_count,
                                    UserUpdated = entry.UserUpdated,
                                    PoliticalParty = "Die Linke",
                                };


                                col.Update(userModel);



                            }
                            else if (entry.Description.Contains("AFD")
                                || entry.Description.Contains("AfD")
                                || entry.Description.Contains("afd")
                                || entry.Screen_name.Contains("afd")
                                || entry.Screen_name.Contains("AFD")
                                || entry.Name.Contains("AFD")
                                || entry.Name.Contains("AfD")
                                )
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is AFD");
                                afd.Add(entry.Name);
                                var userModel = new TwitterUserModel
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Screen_name = entry.Screen_name,
                                    Description = entry.Description,
                                    Created_at = entry.Created_at,
                                    Location = entry.Location,
                                    Geo_enabled = entry.Geo_enabled,
                                    Url = entry.Url,
                                    Statuses_count = entry.Statuses_count,
                                    Followers_count = entry.Followers_count,
                                    Friends_count = entry.Friends_count,
                                    Verified = entry.Verified,
                                    Profile_image_url_https = entry.Profile_image_url_https,
                                    Favourites_count = entry.Favourites_count,
                                    Listed_count = entry.Listed_count,
                                    UserUpdated = entry.UserUpdated,
                                    PoliticalParty = "AFD",
                                };


                                col.Update(userModel);
                            }
                            else if (entry.Description.Contains("CDU")
                                || entry.Description.Contains("CSU")
                                || entry.Description.Contains("CDU/CSU")
                                || entry.Description.Contains("cducsu")
                                || entry.Screen_name.Contains("cdu")
                                || entry.Screen_name.Contains("CDU")
                                || entry.Screen_name.Contains("csu")
                                || entry.Screen_name.Contains("CSU")
                                || entry.Name.Contains("CDU")
                                || entry.Name.Contains("CSU")
                                )
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is CDU/CSU");
                                cducsu.Add(entry.Name);
                                var userModel = new TwitterUserModel
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Screen_name = entry.Screen_name,
                                    Description = entry.Description,
                                    Created_at = entry.Created_at,
                                    Location = entry.Location,
                                    Geo_enabled = entry.Geo_enabled,
                                    Url = entry.Url,
                                    Statuses_count = entry.Statuses_count,
                                    Followers_count = entry.Followers_count,
                                    Friends_count = entry.Friends_count,
                                    Verified = entry.Verified,
                                    Profile_image_url_https = entry.Profile_image_url_https,
                                    Favourites_count = entry.Favourites_count,
                                    Listed_count = entry.Listed_count,
                                    UserUpdated = entry.UserUpdated,
                                    PoliticalParty = "CDU/CSU",
                                };


                                col.Update(userModel);
                            }
                            else if (entry.Description.Contains("FDP")
                                || entry.Description.Contains("fdp")
                                || entry.Description.Contains("liberal")
                                || entry.Description.Contains("Freien Demokraten")
                                || entry.Screen_name.Contains("fdp")
                                || entry.Screen_name.Contains("FDP")
                                || entry.Name.Contains("FDP")
                                )
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is FDP");
                                fdp.Add(entry.Name);
                                var userModel = new TwitterUserModel
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Screen_name = entry.Screen_name,
                                    Description = entry.Description,
                                    Created_at = entry.Created_at,
                                    Location = entry.Location,
                                    Geo_enabled = entry.Geo_enabled,
                                    Url = entry.Url,
                                    Statuses_count = entry.Statuses_count,
                                    Followers_count = entry.Followers_count,
                                    Friends_count = entry.Friends_count,
                                    Verified = entry.Verified,
                                    Profile_image_url_https = entry.Profile_image_url_https,
                                    Favourites_count = entry.Favourites_count,
                                    Listed_count = entry.Listed_count,
                                    UserUpdated = entry.UserUpdated,
                                    PoliticalParty = "FDP",
                                };


                                col.Update(userModel);
                            }
                            else if (entry.Description.Contains("SPD")
                                || entry.Description.Contains("spd")
                                || entry.Description.Contains("sozialdemokrat")
                                || entry.Screen_name.Contains("spd")
                                || entry.Screen_name.Contains("SPD")
                                || entry.Name.Contains("SPD")
                                )
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is SPD");
                                spd.Add(entry.Name);
                                var userModel = new TwitterUserModel
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Screen_name = entry.Screen_name,
                                    Description = entry.Description,
                                    Created_at = entry.Created_at,
                                    Location = entry.Location,
                                    Geo_enabled = entry.Geo_enabled,
                                    Url = entry.Url,
                                    Statuses_count = entry.Statuses_count,
                                    Followers_count = entry.Followers_count,
                                    Friends_count = entry.Friends_count,
                                    Verified = entry.Verified,
                                    Profile_image_url_https = entry.Profile_image_url_https,
                                    Favourites_count = entry.Favourites_count,
                                    Listed_count = entry.Listed_count,
                                    UserUpdated = entry.UserUpdated,
                                    PoliticalParty = "SPD",
                                };


                                col.Update(userModel);
                            }
                            else if (entry.Description.Contains("Blaue")
                                || entry.Description.Contains("Blauen")
                                || entry.Description.Contains("blaue")
                                || entry.Description.Contains("blau"))
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is Blau");
                                blau.Add(entry.Name);
                                var userModel = new TwitterUserModel
                                {
                                    Id = entry.Id,
                                    Name = entry.Name,
                                    Screen_name = entry.Screen_name,
                                    Description = entry.Description,
                                    Created_at = entry.Created_at,
                                    Location = entry.Location,
                                    Geo_enabled = entry.Geo_enabled,
                                    Url = entry.Url,
                                    Statuses_count = entry.Statuses_count,
                                    Followers_count = entry.Followers_count,
                                    Friends_count = entry.Friends_count,
                                    Verified = entry.Verified,
                                    Profile_image_url_https = entry.Profile_image_url_https,
                                    Favourites_count = entry.Favourites_count,
                                    Listed_count = entry.Listed_count,
                                    UserUpdated = entry.UserUpdated,
                                    PoliticalParty = "Die Blauen",
                                };


                                col.Update(userModel);
                            }
                            else
                            {
                                if (debugLine == true)
                                    Console.WriteLine("Is NIX");
                                nix.Add(entry.Name);
                            }

                        }

                        //Find equal name  - in list / textfile? - on wikiData? 


                        //change the political party entry


                    }


                    //Debug Listings

                    Console.WriteLine(">>> Die Linke {0} ", left.Count());
                    Console.WriteLine(">>> Grüne {0} ", green.Count());
                    Console.WriteLine(">>> AFD {0} ", afd.Count());
                    Console.WriteLine(">>> CDU/CSU {0} ", cducsu.Count());
                    Console.WriteLine(">>> SPD {0} ", spd.Count());
                    Console.WriteLine(">>> FDP {0} ", fdp.Count());
                    Console.WriteLine(">>> Blaue {0} ", blau.Count());


                    Console.WriteLine(">>> Nichts gefunden {0} ", nix.Count());
                    foreach (var jarnix in nix) { Console.WriteLine(jarnix.ToString()); }

                    Console.WriteLine(">>> Abfall {0} ", abfall.Count());
                    foreach (var muell in abfall) { Console.WriteLine(muell.ToString()); }




                }
                catch (LiteException ex)
                {
                    Console.WriteLine("LiteDB Exception: " + ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex);
                }


                Console.ReadKey();

            }



        }

        private static void FixDBwithLast()
        {
            #region Repair DB easy fix for now
            //Repair Routine for putting Collection back from Backup

            try
            {

                try
                {
                    using (var db1 = new LiteDatabase("TwitterData2.db"))
                    using (var db2 = new LiteDatabase("TwitterData.db"))
                    {
                        var from = db1.GetCollection("TwitterUserDaily");
                        var to = db2.GetCollection("TwitterUserDaily");

                        to.Insert(from.FindAll());
                        Console.WriteLine("=>>>> DB fixed....");
                    }
                }
                catch (LiteException ex)
                {
                    Console.WriteLine("=>>> fixing Lite DB error", ex);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("=>>> error", ex);
            }


            return;

            #endregion

        }

        private static void UpdateTwitterUserDaily()
        {

            using (var db = new LiteDatabase("TwitterData.db"))
            {
                try
                {


                    Console.WriteLine(">>> Getting Collection...");
                    var col = db.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");

                    var update = col.FindAll().Where(s => s.TwitterName == null);

                    Console.WriteLine("Entries that will be updated: " + update.Count());

                    var twitterUser = db.GetCollection<TwitterUserModel>("TwitterUser");
                    Console.WriteLine(">>> Getting TwitterUser from Main Database...");

                    Console.WriteLine(">>> Starting Update...");


                    //Update TwitterID and TwitterName
                    //use Tweetinvi to get the TwitterID
                    //or use existing TwitterUserDatabase
                    
                    foreach (var x in update)
                    {
                        try
                        {
                            var userDetail = twitterUser.FindAll().Where(s => s.Screen_name == x.Screen_name).First();
                        
                        if (userDetail != null)
                        {
                            Console.WriteLine(">>> Updating..." + x.Screen_name + " TwitterUser found: " + userDetail.Id);


                            var twitterUserDaily = new TwitterUserDailyModel
                            {
                                Id = x.Id,
                                Screen_name = x.Screen_name,
                                Statuses_count = x.Statuses_count,
                                Followers_count = x.Followers_count,
                                Friends_count = x.Friends_count,
                                Favourites_count = x.Favourites_count,
                                Listed_count = x.Listed_count,
                                DateToday = x.DateToday,
                                TwitterId = userDetail.Id,
                                TwitterName = userDetail.Name
                            };

                            col.Update(twitterUserDaily);

                            Console.WriteLine(">>> Updating..." + userDetail.Id + " successful...");
                        }
                        else
                        {
                            Console.WriteLine(">>> Skipped..." + x.Id);
                        }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception: " + ex);
                        }

                    }





                }
                catch (LiteException ex)
                {
                    Console.WriteLine("LiteDB Exception: " + ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex);
                }

                Console.WriteLine("Update successful");
                Console.ReadKey();

            }
        }

        private static void InsertNewUserDaily()
        {
            IEnumerable<TwitterUserDailyModel> update;
            IEnumerable<TwitterStreamModel> update2;

            try
            {
                Console.WriteLine("How many past days do you want to Input...?");
                var days = Convert.ToInt16(Console.ReadLine());

                Console.WriteLine("Which Data Collection do you want to use...?\n1 - TwitterUserDaily\n2 - TwitterStream ");
                var collection = Convert.ToInt16(Console.ReadLine());

                try
                {


                    using (var db2 = new LiteDatabase("TwitterData2.db"))
                    using (var db1 = new LiteDatabase("TwitterData.db"))
                    {
                        if (collection == 1)
                        {

                            var from = db2.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");
                            update = from.FindAll().Where(s => s.DateToday == DateTime.Today.AddDays(-days));
                            var updatecheck = from.FindAll();
                            Console.WriteLine("Do you want to update... " + "\n" + update.Count() + "\t" + DateTime.Today.AddDays(-1) + "\n" + updatecheck.Count() + "\t" + DateTime.Today + "\n Press enter if YES!");
                            Console.ReadKey();

                            var to = db1.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");

                            foreach (var x in update)
                            {
                                Console.WriteLine("Insert>" + x.Id);

                                var twitterUserDaily = new TwitterUserDailyModel
                                {
                                    Id = x.Id,
                                    Screen_name = x.Screen_name,
                                    Statuses_count = x.Statuses_count,
                                    Followers_count = x.Followers_count,
                                    Friends_count = x.Friends_count,
                                    Favourites_count = x.Favourites_count,
                                    Listed_count = x.Listed_count,
                                    DateToday = x.DateToday,
                                    TwitterId = x.Id,
                                    TwitterName = x.TwitterName
                                };

                                to.Upsert(twitterUserDaily);

                            }

                        }
                        if (collection == 2)
                        {
                            var from = db2.GetCollection<TwitterStreamModel>("TwitterStream");
                            update2 = from.FindAll().Where(s => s.TweetCreatedAt > DateTime.Now.AddDays(-days));
                            var updatecheck = from.FindAll();
                            Console.WriteLine("Do you want to update... " + "\n" + update2.Count() + "\t" + DateTime.Today.AddDays(-1) + "\n" + updatecheck.Count() + "\t" + DateTime.Today + "\n Press enter if YES!");
                            Console.ReadKey();

                            var to = db1.GetCollection<TwitterStreamModel>("TwitterStream");

                            foreach (var x in update2)
                            {
                                Console.WriteLine("Insert>" + x.TweetID);

                                var twitterStream = new TwitterStreamModel
                                {
                                    TweetID = x.TweetID,
                                    TweetUser = x.TweetUser,
                                    TweetUserName = x.TweetUserName,
                                    TweetUserID = x.TweetUserID,
                                    TweetUserPicture = x.TweetUserPicture,
                                    TweetUserDesc = x.TweetUserDesc,
                                    TweetText = x.TweetText,
                                    TweetHashtags = x.TweetHashtags,
                                    TweetReTweetCount = x.TweetReTweetCount,
                                    TweetFavoriteCount = x.TweetFavoriteCount,
                                    TweetCreatedAt = x.TweetCreatedAt,
                                    TweetUrl = x.TweetUrl
                                };

                                to.Upsert(twitterStream);

                            }

                        }

                        Console.WriteLine("=>>>> DB updated....");

                    }


                   

                }
                catch (LiteException ex)
                {
                    Console.WriteLine("=>>> fixing Lite DB error", ex);
     
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("=>>> error", ex);
            }

            Console.ReadKey();
            return;



        }
    }
}
