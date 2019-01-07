using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KompromatKoffer.Areas.Database.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tweetinvi;

namespace KompromatKoffer.Pages.Force
{
    public class PoliceModel : PageModel
    {
        public IEnumerable<Tweetinvi.Models.ITweet> TimeLine;

        public IEnumerable<Tweetinvi.Models.ITweet> TimeLine2;

        public IEnumerable<Tweetinvi.Models.ITweet> TimeLine3;

        public IEnumerable<Tweetinvi.Models.ITweet> TimeLine4;

        public IEnumerable<Tweetinvi.Models.ITweet> TimeLine5;

        public void OnGet()
        {

            //Make Array now!


            var user = Tweetinvi.User.GetUserFromScreenName("gstaberlin");
            var user2 = Tweetinvi.User.GetUserFromScreenName("polizeiberlin");
            var user3 = Tweetinvi.User.GetUserFromScreenName("bka");
            var user4 = Tweetinvi.User.GetUserFromScreenName("berliner_fw");
            var user5 = Tweetinvi.User.GetUserFromScreenName("bsi_presse");
            var _timeline = Timeline.GetUserTimeline(user.Id,1);
            var _timeline2 = Timeline.GetUserTimeline(user2.Id, 1);
            var _timeline3 = Timeline.GetUserTimeline(user3.Id, 1);
            var _timeline4 = Timeline.GetUserTimeline(user4.Id, 1);
            var _timeline5 = Timeline.GetUserTimeline(user5.Id, 1);





            TimeLine = _timeline;
            TimeLine2 = _timeline2;
            TimeLine3 = _timeline3;
            TimeLine4 = _timeline4;
            TimeLine5 = _timeline5;



        }




    }
}