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

namespace KompromatKoffer.Pages.Database
{
    [Authorize]
    public class GlobalModel : PageModel
    {

        private readonly ILogger<GlobalModel> _logger;

        public GlobalModel(ILogger<GlobalModel> logger)
        {
            _logger = logger;
        }

        public LiteCollection<TwitterUserDailyModel> CompleteDB;

        [BindProperty]
        public string CurrentUserScreenname { get; set; }

        public string LittleTuple;

        public int SinceDays { get; set; } = -7;

        public int DaysRange { get; set; }
        public int CurrentRange { get; set; }

        public async Task OnGet(int sinceDays)
        {

            #region Database Connection - get TwitterUserDailyCollection

            try
            {
                using (var db = new LiteDatabase("TwitterData.db"))
                {
                    var col = db.GetCollection<TwitterUserDailyModel>("TwitterUserDaily");
                    CompleteDB = col;

                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception " + ex);
            }

            #endregion

            SinceDays = sinceDays;


            await Task.Delay(1);


           

           

           


        }







    }
}