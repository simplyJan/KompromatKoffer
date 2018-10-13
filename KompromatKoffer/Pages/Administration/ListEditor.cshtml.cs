using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KompromatKoffer.Areas.Database.Model;
using LiteDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace KompromatKoffer.Pages.Administration
{
    [Authorize(Roles = "Administrator")]
    public class ListEditorModel : PageModel
    {
       
        private readonly ILogger _logger;

        public ListEditorModel(ILogger<ListEditorModel> logger)
        {
            _logger = logger;
        }

        public IEnumerable<TwitterUserModel> CompleteDB { get; set; }

        [BindProperty]
        public int PoliticalPartyMember { get; set; }
        public IList<SelectListItem> PoliticalParty { get; set; }

        public string FavCountSort { get; set; }

        public IActionResult OnGet(string sortOrder)
        {

            PoliticalParty = new List<SelectListItem> {
            new SelectListItem { Value = "1", Text = "AFD" },
            new SelectListItem { Value = "2", Text = "BÜNDNIS 90/DIE GRÜNEN" },
            new SelectListItem { Value = "3", Text = "CDU/CSU" },
            new SelectListItem { Value = "4", Text = "Die Linke" },
            new SelectListItem { Value = "5", Text = "FDP" },
            new SelectListItem { Value = "6", Text = "SPD" },
            new SelectListItem { Value = "7", Text = "Fraktionslos" },
            new SelectListItem { Value = "8", Text = null },
            };

            using (var db = new LiteDatabase("TwitterData.db"))
            {

                _logger.LogInformation("===========> Getting Collection...");

                var mapper = BsonMapper.Global;

                mapper.Entity<TwitterUserModel>()
                    .Id(y => y.Id);

                var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                var completeCollection = col.FindAll();

                CompleteDB = completeCollection;

                FavCountSort = sortOrder == "FavCount_Desc" ? "FavCount" : "FavCount_Desc";

                switch (sortOrder)
                {

                    default:
                        CompleteDB = CompleteDB.OrderBy(s => s.PoliticalParty);
                        break;
                }

                if (sortOrder != null)
                {
                    _logger.LogInformation("Sort order is... " + sortOrder);
                }

            }
            return Page();
        }

        public IActionResult OnPostSaveEntry(long id)
        {
            
            using (var db = new LiteDatabase("TwitterData.db"))
            {
                _logger.LogInformation(">>>>>>>>> Checking DB for {0} ...", id);

                var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                var changeEntry = col.Find(x => x.Id == id);

                if (changeEntry != null)

                {
                    var politicalPartyMembership = "";

                    if(PoliticalPartyMember == 1)
                    {
                        politicalPartyMembership = "AFD";
                    }

                    if (PoliticalPartyMember == 2)
                    {
                        politicalPartyMembership = "BÜNDNIS 90/DIE GRÜNEN";
                    }


                    if (PoliticalPartyMember == 3)
                    {
                        politicalPartyMembership = "CDU/CSU";
                    }


                    if (PoliticalPartyMember == 4)
                    {
                        politicalPartyMembership = "Die Linke";
                    }


                    if (PoliticalPartyMember == 5)
                    {
                        politicalPartyMembership = "FDP";
                    }


                    if (PoliticalPartyMember == 6)
                    {
                        politicalPartyMembership = "SPD";
                    }

                    if (PoliticalPartyMember == 7)
                    {
                        politicalPartyMembership = "Fraktionslos";
                    }

                    if (PoliticalPartyMember == 8)
                    {
                        politicalPartyMembership = null;
                    }

                    foreach (var item in changeEntry)
                    {
                        _logger.LogInformation(">>>>>>>>> Found DB Entry\n>>>>>>>>> {0} - {1} - {2}", item.Id, item.Name, item.PoliticalParty);

                        var twitterUser = new TwitterUserModel
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Screen_name = item.Screen_name,
                            Description = item.Description,
                            Created_at = item.Created_at,
                            Location = item.Location,
                            Geo_enabled = item.Geo_enabled,
                            Url = item.Url,
                            Statuses_count = item.Statuses_count,
                            Followers_count = item.Followers_count,
                            Friends_count = item.Friends_count,
                            Verified = item.Verified,
                            Profile_image_url_https = item.Profile_image_url_https,
                            Favourites_count = item.Favourites_count,
                            Listed_count = item.Listed_count,
                            UserUpdated = DateTime.Now,
                            PoliticalParty = politicalPartyMembership
                        };

                        //Update User if name is not null and if the saveinterval is reached^^
                        col.Update(twitterUser);
                        _logger.LogInformation(">>>>>>>>> Updated {0} with {1} ... ", politicalPartyMembership, item.Id);

                    }

                }

            }

            return RedirectToPage();

        }


    }
}