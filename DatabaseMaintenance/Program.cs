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


            PoliticalPartyUpdate();
            

        }

 
        private static void PoliticalPartyUpdate()
        {
            using (var db = new LiteDatabase("TwitterData.db"))
            {
                try
                {
                    Console.WriteLine(">>> Getting Collection...");
                    var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                    var partyList = col.FindAll().Where(s => s.PoliticalParty == "filloutbyhandfornow");

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
                                if(debugLine == true)
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
                    foreach(var jarnix in nix){Console.WriteLine(jarnix.ToString());}

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

    }
}
