using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tweetinvi;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IO;
using System.Collections;
using HtmlAgilityPack;

namespace KompromatKoffer.Pages.DataScraping
{
    public class WahlrechtModel : PageModel
    {
        private readonly ILogger<WahlrechtModel> _logger;

        public WahlrechtModel(ILogger<WahlrechtModel> logger)
        {
            _logger = logger;
        }

        public HtmlAgilityPack.HtmlNode INSATable { get; set; }

        //Scrap Data from given websites and build with it
        public void OnGet()
        {

            WebClient webClient = new WebClient();
            string page = webClient.DownloadString("http://www.wahlrecht.de/umfragen/insa.htm");

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(page);

            var table = doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'wilko')]");

            _logger.LogInformation(Convert.ToString(table.SelectNodes("//tr").Count));

            INSATable = table;


        }

    }
}