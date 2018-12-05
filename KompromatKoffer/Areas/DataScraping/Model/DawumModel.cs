using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json.Converters;

namespace KompromatKoffer.Areas.DataScraping.Model
{
    //Data from DAWUM -> https://dawum.de - (Lizenz: ODC-ODbL) -> https://opendatacommons.org/licenses/odbl/1.0/ 
    
    public class DawumModel
    {
        public partial class Welcome
        {
            public Database Database { get; set; }
            public Dictionary<string, Parliament> Parliaments { get; set; }
            public Dictionary<string, Institute> Institutes { get; set; }
            public Dictionary<string, Institute> Taskers { get; set; }
            public Dictionary<string, Party> Parties { get; set; }
            public Dictionary<string, Survey> Surveys { get; set; }
        }

        public partial class Database
        {
            public License License { get; set; }
            public string Publisher { get; set; }
            public string Author { get; set; }
            public DateTimeOffset LastUpdate { get; set; }
        }

        public partial class License
        {
            public string Name { get; set; }
            public string Shortcut { get; set; }
            public Uri Link { get; set; }
        }

        public partial class Institute
        {
            public string Name { get; set; }
        }

        public partial class Parliament
        {
            public string Shortcut { get; set; }
            public string Name { get; set; }
            public string Election { get; set; }
        }

        public partial class Party
        {
            public string Shortcut { get; set; }
            public string Name { get; set; }
        }

        public partial class Survey
        {
            public DateTimeOffset Date { get; set; }
            public SurveyPeriod SurveyPeriod { get; set; }
            public long SurveyedPersons { get; set; }
            public long ParliamentId { get; set; }
            public long InstituteId { get; set; }
            public long TaskerId { get; set; }
            public Dictionary<string, double> Results { get; set; }
        }

        public partial class SurveyPeriod
        {
            public DateTimeOffset DateStart { get; set; }
            public DateTimeOffset DateEnd { get; set; }
        }


    }
}
