using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KompromatKoffer.Areas.Administration.Model;
using KompromatKoffer.Areas.Identity.Data;
using KompromatKoffer.Models;
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
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;

        private ApplicationContext _context;

        public IEnumerable<AdminModel> UserWithRoles;

        public AdministrationModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AdministrationModel> logger,
            ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "ListName")]
            public string ListName { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "ListOwner")]
            public string ListOwner { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TweetsRetrieved")]
            public int TweetsRetrieved { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "UpdateDelay")]
            public int UpdateDelay { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TaskDelay")]
            public int TaskDelay { get; set; } 

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TwitterUserUpdateInterval")]
            public int TwitterUserUpdateInterval { get; set; } 

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TwitterUserDailyUpdateInterval")]
            public int TwitterUserDailyUpdateInterval { get; set; } 

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TwitterUserDailyTaskDelay")]
            public int TwitterUserDailyTaskDelay { get; set; } 

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "TwitterUserDailyUpdateDelay")]
            public int TwitterUserDailyUpdateDelay { get; set; } 
        }

        public class MailSettings
        {
            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Mail Address")]
            public string MailAddress { get; set; } 

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Mail Displayname")]
            public string MailDisplayname { get; set; } 

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Mail Host")]
            public string MailHost { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Mail Port")]
            public string MailPort { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Login")]
            public string Login { get; set; }

            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Password")]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            //List Settings
            /*
            ListSettings.ListName = Config.Parameter.ListName;
            ListSettings.ListOwner = Config.Parameter.ScreenName;
            ListSettings.TweetsRetrieved = Config.Parameter.TweetsRetrieved;
            ListSettings.UpdateDelay = Config.Parameter.UpdateDelay;
            ListSettings.TaskDelay = Config.Parameter.TaskDelay;
            ListSettings.TwitterUserUpdateInterval = Config.Parameter.TwitterUserUpdateInterval;
            ListSettings.TwitterUserDailyUpdateInterval = Config.Parameter.TwitterUserDailyUpdateInterval;
            ListSettings.TwitterUserDailyTaskDelay = Config.Parameter.TwitterUserTimelineTaskDelay;
            ListSettings.TwitterUserDailyUpdateDelay = Config.Parameter.TwitterUserDailyUpdateDelay;

            //Mail Settings
            SendMailSettings.MailAddress = Config.Parameter.Mail_From_Email_Address;
            SendMailSettings.MailDisplayname = Config.Parameter.Mail_From_Email_DisplayName;
            SendMailSettings.MailHost = Config.Parameter.Mail_Host;
            SendMailSettings.MailPort = Config.Parameter.Mail_Port;
            SendMailSettings.Login = Config.Parameter.Mail_Email_Login;
            SendMailSettings.Password = Config.Parameter.Mail_Email_Passwort;
            */


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


            await Task.Delay(1);

            return Page();

        }


        public async Task<IActionResult> OnPostAsync()
        {








            await Task.Delay(1);
            return Page();
        }
    }
}