using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace KompromatKoffer.Pages
{
    public class IndexModel : PageModel
    {

        public void OnGet()
        {

            Auth.SetUserCredentials(Config.Credentials.CONSUMER_KEY,Config.Credentials.CONSUMER_SECRET, Config.Credentials.ACCESS_TOKEN,Config.Credentials.ACCESS_TOKEN_SECRET);



            

        }




    }
}
