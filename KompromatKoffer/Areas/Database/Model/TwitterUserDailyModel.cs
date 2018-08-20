using LiteDB;
using System;

namespace KompromatKoffer.Areas.Database.Model
{
    public class TwitterUserDailyModel
    {
        [BsonId]
        public long Id { get; set; }
        public string Screen_name { get; set; }
        public int Statuses_count { get; set; }
        public int Followers_count { get; set; }
        public int Friends_count { get; set; }
        public int Favourites_count { get; set; }
        public int Listed_count { get; set; }

        public DateTime DateToday { get; set; }

        public long TwitterId { get; set; }
        public string TwitterName { get; set; }


    }
}