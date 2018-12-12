using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CodeHollow.FeedReader;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace KompromatKoffer.Pages.Database
{
    [Authorize]
    public class NewsDatabaseModel : PageModel
    {

        private readonly ILogger<NewsDatabaseModel> _logger;

        public NewsDatabaseModel(ILogger<NewsDatabaseModel> logger)
        {
            _logger = logger;
        }

        public CodeHollow.FeedReader.Feed Sputnik;
        public CodeHollow.FeedReader.Feed WeltDe;
        public CodeHollow.FeedReader.Feed SpiegelOnline;
        public CodeHollow.FeedReader.Feed Nzz;
        public CodeHollow.FeedReader.Feed StandardAt;
        public CodeHollow.FeedReader.Feed BildDe;
        public CodeHollow.FeedReader.Feed Ntv;
        public CodeHollow.FeedReader.Feed FocusOnline;
        public CodeHollow.FeedReader.Feed SueddeutscheDe;
        public CodeHollow.FeedReader.Feed ZeitDe;
        public CodeHollow.FeedReader.Feed FazNet;
        public CodeHollow.FeedReader.Feed SternDe;
        public CodeHollow.FeedReader.Feed Tagesschau;
        public CodeHollow.FeedReader.Feed Zdf;

        public string CurrentFilter { get; set; }



        public void OnGet(string searchString, string currentFilter)
        {
            //Set searchstring Dummy
            CurrentFilter = "Putin";


            try
            {

                //Make RSS Service from all News Sites
                //Make Method to get all news sites from arrayfield in backend

                //Sputnik Feed
                var sputnik = FeedReader.Read("https://de.sputniknews.com/export/rss2/archive/index.xml");
                Sputnik = sputnik;

                //Welt.de
                var weltde = FeedReader.Read("https://www.welt.de/feeds/latest.rss");
                WeltDe = weltde;

                //Spiegel.de
                var spiegelOnline = FeedReader.Read("http://www.spiegel.de/schlagzeilen/index.rss");
                SpiegelOnline = spiegelOnline;

                //Nzz
                var nzz = FeedReader.Read("http://www.nzz.ch/international.rss");
                Nzz = nzz;

                //Standard.at
                var standardAt = FeedReader.Read("https://derstandard.at/?page=rss");
                StandardAt = standardAt;

                //Bild.de
                var bildDe = FeedReader.Read("https://www.bild.de/rssfeeds/rss3-20745882,feed=alles.bild.html");
                BildDe = bildDe;

                //N-TV.de
                var ntv = FeedReader.Read("https://www.n-tv.de/rss");
                Ntv = ntv;

                //Focus Online
                var focusOnline = FeedReader.Read("http://rss.focus.de/fol/XML/rss_folnews.xml");
                FocusOnline = focusOnline;

                //Süddeutsche.de
                var sueddeutscheDe = FeedReader.Read("http://rss.sueddeutsche.de/app/service/rss/alles/index.rss?output=rss");
                SueddeutscheDe = sueddeutscheDe;

                //Zeit.de
                var zeitDe = FeedReader.Read("http://newsfeed.zeit.de/all");
                ZeitDe = zeitDe;

                //Faz.net
                var faznet = FeedReader.Read("https://www.faz.net/rss/aktuell/");
                FazNet = faznet;

                //Stern.de
                var sternDe = FeedReader.Read("https://www.stern.de/feed/standard/alle-nachrichten/");
                SternDe = sternDe;

                //YTagesschau.de
                var tagesschau = FeedReader.Read("http://www.tagesschau.de/xml/rss2");
                Tagesschau = tagesschau;

                //https://www.zdf.de/rss/podcast/video/zdf/nachrichten/heute-journal
                var zdf = FeedReader.Read("http://www.tagesschau.de/xml/rss2");
                Zdf = zdf;

                //Searchstring for RSS Feeds
                CurrentFilter = searchString;

                //Search in all RSS Feeds for Keyword


                //Search Filtering
                if (!String.IsNullOrEmpty(searchString))
                {


                }



                }
            catch(Exception ex)
            {
                _logger.LogWarning(" " + ex);
            }





        }




    }
}