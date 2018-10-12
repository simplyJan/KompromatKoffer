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

        public class PoliticalPartyModel
        {
            public static String AFD = "AFD";
            public static String GRUEN = "BÜNDNIS 90/DIE GRÜNEN";
            public static string UNION = "CDU/CSU";
            public static string LINKE = "Die Linke";
            public static string FDP = "FDP";
            public static string SPD = "SPD";
            public static string OTHER = "Fraktionslos";
        }

        public IList<PoliticalPartyModel> PoliticalParty { get; set; }

        public string NewPolPaParty { get; set; }

        

        public void OnGet()
        {
            using (var db = new LiteDatabase("TwitterData.db"))
            {

                _logger.LogInformation("===========> Getting Collection...");

                var mapper = BsonMapper.Global;

                mapper.Entity<TwitterUserModel>()
                    .Id(y => y.Id);

                var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                var completeCollection = col.FindAll();

                CompleteDB = completeCollection;
                
            }
        }


        public IActionResult OnPostSaveEntry(long id, string polPa)
        {
            using (var db = new LiteDatabase("TwitterData.db"))
            {
                _logger.LogInformation(">>>>>>>>> Checking DB for {0} ...", id);

                var col = db.GetCollection<TwitterUserModel>("TwitterUser");

                var changeEntry = col.Find(x => x.Id == id);

                if (changeEntry != null)

                {

                    foreach(var item in changeEntry)
                    {
                        _logger.LogInformation(">>>>>>>>> Updated DB Entry\n>>>>>>>>> {0} - {1} - {2}", item.Id, item.Name, item.PoliticalParty);
                    }




                    //do stuff

                    _logger.LogInformation(">>>>>>>>> Updated DB Entry for {0} ... ", polPa);

                }

            }


           

            return RedirectToPage();




        }






    }
}