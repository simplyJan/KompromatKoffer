using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KompromatKoffer.Areas.Database.Model;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace KompromatKoffer.Pages.Administration
{
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
            public static string LINKE = "Die Linke";
            public static string UNION = "CDU/CSU";
            public static string FDP = "FDP";
            public static string SPD = "SPD";
            public static string OTHER = "Fraktionslos";
        }

        public IList<PoliticalPartyModel> PoliticalParty { get; set; }

        

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




    }
}