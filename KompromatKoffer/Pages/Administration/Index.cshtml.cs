using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KompromatKoffer.Areas.Administration.Model;
using KompromatKoffer.Areas.Identity.Data;
using KompromatKoffer.Models;
using KompromatKoffer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace KompromatKoffer.Pages.Administration
{
    [Authorize(Roles = "Administrator")]
    public class AdministrationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        private ApplicationContext _context;

        public IEnumerable<AdminModel> UserWithRoles;

        public AdministrationModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AdministrationModel> logger,
            ApplicationContext context
)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public Settings ListSettings { get; set; }

        [BindProperty]
        public MailSettings SendMailSettings { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class Settings
        {

            [Display(Name = "ListName")]
            public string ListName { get; set; } //= Config.Parameter.ListName;


            [Display(Name = "ListOwner")]
            public string ListOwner { get; set; } //= Config.Parameter.ScreenName;


            [Display(Name = "TweetsRetrieved")]
            public int TweetsRetrieved { get; set; } //= Config.Parameter.TweetsRetrieved;


            [Display(Name = "UpdateDelay")]
            public int UpdateDelay { get; set; } //= Config.Parameter.UpdateDelay;


            [Display(Name = "TaskDelay")]
            public int TaskDelay { get; set; } //= Config.Parameter.TaskDelay;


            [Display(Name = "TwitterUserUpdateInterval")]
            public int TwitterUserUpdateInterval { get; set; } //= Config.Parameter.TwitterUserUpdateInterval;


            [Display(Name = "TwitterUserDailyUpdateInterval")]
            public int TwitterUserDailyUpdateInterval { get; set; } //= Config.Parameter.TwitterUserDailyUpdateInterval;

 
            [Display(Name = "TwitterUserDailyTaskDelay")]
            public int TwitterUserDailyTaskDelay { get; set; } //= Config.Parameter.TwitterUserDailyTaskDelay;

   
            [Display(Name = "TwitterUserDailyUpdateDelay")]
            public int TwitterUserDailyUpdateDelay { get; set; } //= Config.Parameter.TwitterUserDailyUpdateDelay;

            [Display(Name = "TwitterStreamCountUpdateDelay")]
            public int TwitterStreamCountUpdateDelay { get; set; }

            [Display(Name = "TwitterStreamCountWriteDelay")]
            public int TwitterStreamCountWriteDelay { get; set; }

            [Display(Name = "TwitterStreamCountTaskDelay")]
            public int TwitterStreamCountTaskDelay { get; set; }

            [Display(Name = "TwitterStreamCountUpdateLastHours")]
            public int TwitterStreamCountUpdateLastHours { get; set; }

            [Display(Name = "TwitterStreamDayRange")]
            public int TwitterStreamDayRange { get; set; }

            [Display(Name = "IndexTweetLimit")]
            public int IndexTweetLimit { get; set; }

            [Display(Name = "ShowEntries")]
            public int ShowEntries { get; set; }

            [Display(Name = "DBBackupSpawn")]
            public int DBBackupSpawn { get; set; }

            [DataType(DataType.MultilineText)]
            public string IndexWarningMessage { get; set; }

        }

        public class MailSettings
        {
     
            [Display(Name = "Mail Address")]
            public string MailAddress { get; set; } //= Config.Parameter.Mail_From_Email_Address;

            [Display(Name = "Mail Displayname")]
            public string MailDisplayname { get; set; } //= Config.Parameter.Mail_From_Email_DisplayName;

            [Display(Name = "Mail Host")]
            public string MailHost { get; set; } //= Config.Parameter.Mail_Host;

            [Display(Name = "Mail Port")]
            public int MailPort { get; set; } //= Config.Parameter.Mail_Port;

            [Display(Name = "Login")]
            public string Login { get; set; } //= Config.Parameter.Mail_Email_Login;

            [Display(Name = "Password")]
            public string Password { get; set; } //= Config.Parameter.Mail_Email_Passwort;
        }

        //PageView
        public IActionResult OnGet()
        {
            //List Settings
            ListSettings = new Settings()
            {
                ListName = Config.Parameter.ListName,
                ListOwner = Config.Parameter.ScreenName,
                UpdateDelay = Config.Parameter.UpdateDelay,
                TwitterUserUpdateInterval = Config.Parameter.TwitterUserUpdateInterval,
                TwitterUserDailyUpdateInterval = Config.Parameter.TwitterUserDailyUpdateInterval,
                TwitterStreamCountWriteDelay = Config.Parameter.TwitterStreamCountWriteDelay,
                TwitterStreamCountTaskDelay = Config.Parameter.TwitterStreamCountTaskDelay,
                TwitterStreamCountUpdateLastHours = Config.Parameter.TwitterStreamCountUpdateLastHours,
                TwitterStreamDayRange = Config.Parameter.TwitterStreamDayRange,
                ShowEntries = Config.Parameter.ShowEntries,
                IndexTweetLimit = Config.Parameter.IndexTweetLimit,
                DBBackupSpawn = Config.Parameter.DBBackupSpawn,
                IndexWarningMessage = Config.Parameter.WarningMessage
            };

            //Mail Settings
            SendMailSettings = new MailSettings()
            {
                MailAddress = Config.Parameter.Mail_From_Email_Address,
                MailDisplayname = Config.Parameter.Mail_From_Email_DisplayName,
            };


            var usersWithRoles = (from user in _context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email
                                  }).ToList().Select(p => new AdminModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                  });

            UserWithRoles = usersWithRoles;

            return Page();

        }

        //Update Settings
        public IActionResult OnPostSettings()
        {



                Config.Parameter.ListName = ListSettings.ListName;
                Config.Parameter.ScreenName = ListSettings.ListOwner;
                Config.Parameter.UpdateDelay = ListSettings.UpdateDelay;
                Config.Parameter.TwitterUserUpdateInterval = ListSettings.TwitterUserUpdateInterval;
                Config.Parameter.TwitterUserDailyUpdateInterval = ListSettings.TwitterUserDailyUpdateInterval;
                Config.Parameter.TwitterStreamCountWriteDelay = ListSettings.TwitterStreamCountWriteDelay;
                Config.Parameter.TwitterStreamCountTaskDelay = ListSettings.TwitterStreamCountTaskDelay;
                Config.Parameter.TwitterStreamCountUpdateLastHours = ListSettings.TwitterStreamCountUpdateLastHours;
                Config.Parameter.TwitterStreamDayRange = ListSettings.TwitterStreamDayRange;
                Config.Parameter.IndexTweetLimit = ListSettings.IndexTweetLimit;
                Config.Parameter.ShowEntries = ListSettings.ShowEntries;
                Config.Parameter.DBBackupSpawn = ListSettings.DBBackupSpawn;
                Config.Parameter.WarningMessage = ListSettings.IndexWarningMessage;
           


            //List Settings
            ListSettings = new Settings()
            {
                ListName = Config.Parameter.ListName,
                ListOwner = Config.Parameter.ScreenName,
                UpdateDelay = Config.Parameter.UpdateDelay,
                TwitterUserUpdateInterval = Config.Parameter.TwitterUserUpdateInterval,
                TwitterUserDailyUpdateInterval = Config.Parameter.TwitterUserDailyUpdateInterval,
                TwitterStreamCountWriteDelay = Config.Parameter.TwitterStreamCountWriteDelay,
                TwitterStreamCountTaskDelay = Config.Parameter.TwitterStreamCountTaskDelay,
                TwitterStreamCountUpdateLastHours = Config.Parameter.TwitterStreamCountUpdateLastHours,
                TwitterStreamDayRange = Config.Parameter.TwitterStreamDayRange,
                IndexTweetLimit = Config.Parameter.IndexTweetLimit,
                ShowEntries = Config.Parameter.ShowEntries,
                DBBackupSpawn = Config.Parameter.DBBackupSpawn,
                IndexWarningMessage = Config.Parameter.WarningMessage
            };

            //Mail Settings
            SendMailSettings = new MailSettings()
            {
                MailAddress = Config.Parameter.Mail_From_Email_Address,
                MailDisplayname = Config.Parameter.Mail_From_Email_DisplayName,
            };


            var usersWithRoles = (from user in _context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email
                                  }).ToList().Select(p => new AdminModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                  });

            UserWithRoles = usersWithRoles;


            return Page();

        }

        //Update eMail Settings
        public IActionResult OnPostMailSettings()
        {
            Config.Parameter.Mail_From_Email_Address = SendMailSettings.MailAddress;
            Config.Parameter.Mail_From_Email_DisplayName = SendMailSettings.MailDisplayname;


            //Mail Settings
            SendMailSettings = new MailSettings()
            {
                MailAddress = Config.Parameter.Mail_From_Email_Address,
                MailDisplayname = Config.Parameter.Mail_From_Email_DisplayName,
            };

            //List Settings
            ListSettings = new Settings()
            {
                ListName = Config.Parameter.ListName,
                ListOwner = Config.Parameter.ScreenName,
                UpdateDelay = Config.Parameter.UpdateDelay,
                TwitterUserUpdateInterval = Config.Parameter.TwitterUserUpdateInterval,
                TwitterUserDailyUpdateInterval = Config.Parameter.TwitterUserDailyUpdateInterval,
                TwitterStreamCountWriteDelay = Config.Parameter.TwitterStreamCountWriteDelay,
                TwitterStreamCountTaskDelay = Config.Parameter.TwitterStreamCountTaskDelay,
                TwitterStreamCountUpdateLastHours = Config.Parameter.TwitterStreamCountUpdateLastHours,
                TwitterStreamDayRange = Config.Parameter.TwitterStreamDayRange,
                IndexTweetLimit = Config.Parameter.IndexTweetLimit,
                ShowEntries = Config.Parameter.ShowEntries,
                DBBackupSpawn = Config.Parameter.DBBackupSpawn,
                IndexWarningMessage = Config.Parameter.WarningMessage
            };


            var usersWithRoles = (from user in _context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email
                                  }).ToList().Select(p => new AdminModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                  });

            UserWithRoles = usersWithRoles;


            return Page();
        }

        public IActionResult OnPostStartStopStream()
        {


            //Show the Page with Data


            //List Settings
            ListSettings = new Settings()
            {
                ListName = Config.Parameter.ListName,
                ListOwner = Config.Parameter.ScreenName,
                UpdateDelay = Config.Parameter.UpdateDelay,
                TwitterUserUpdateInterval = Config.Parameter.TwitterUserUpdateInterval,
                TwitterUserDailyUpdateInterval = Config.Parameter.TwitterUserDailyUpdateInterval,
                TwitterStreamCountWriteDelay = Config.Parameter.TwitterStreamCountWriteDelay,
                TwitterStreamCountTaskDelay = Config.Parameter.TwitterStreamCountTaskDelay,
                TwitterStreamCountUpdateLastHours = Config.Parameter.TwitterStreamCountUpdateLastHours,
                TwitterStreamDayRange = Config.Parameter.TwitterStreamDayRange,
                ShowEntries = Config.Parameter.ShowEntries,
                IndexTweetLimit = Config.Parameter.IndexTweetLimit,
                DBBackupSpawn = Config.Parameter.DBBackupSpawn,
                IndexWarningMessage = Config.Parameter.WarningMessage
            };

            //Mail Settings
            SendMailSettings = new MailSettings()
            {
                MailAddress = Config.Parameter.Mail_From_Email_Address,
                MailDisplayname = Config.Parameter.Mail_From_Email_DisplayName,
            };


            var usersWithRoles = (from user in _context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email
                                  }).ToList().Select(p => new AdminModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                  });

            UserWithRoles = usersWithRoles;

            return Page();

        }
    }
}