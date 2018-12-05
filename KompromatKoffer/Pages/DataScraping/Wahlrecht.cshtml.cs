using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tweetinvi;
using OpenScraping;
using OpenScraping.Config;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Net;
using System.IO;

namespace KompromatKoffer.Pages.DataScraping
{
    public class WahlrechtModel : PageModel
    {
        private readonly ILogger<WahlrechtModel> _logger;

        public WahlrechtModel(ILogger<WahlrechtModel> logger)
        {
            _logger = logger;
        }

        public string OutputBTW { get; set; }


        //Scrap Data from given websites and build with it
        public void OnGet()
        {

            WebRequest request = WebRequest.Create("http://www.wahlrecht.de/umfragen/index.htm");
            WebResponse response = request.GetResponse();
            System.IO.Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            // www.bbc.com.json contains the JSON configuration file pasted above
            var configJson = @"
            {
                'title': '//h1',
                'body': '//table[contains(@class, \'wilko\')]'
            }
            ";

            var config = StructuredDataConfig.ParseJsonString(configJson);

            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(responseFromServer);

            //Log the Data Scraping
            _logger.LogInformation(JsonConvert.SerializeObject(scrapingResults, Formatting.Indented));

            var output = JsonConvert.SerializeObject(scrapingResults);
            OutputBTW = output;




        }




    }
}