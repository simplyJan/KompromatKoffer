using LiteDB;
using System;

namespace KompromatKoffer.Areas.Database.Model
{
    public class TwitterUserModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Screen_name { get; set; }
        public string Description { get; set; }
        public DateTime Created_at { get; set; }
        public string Location { get; set; }
        public bool Geo_enabled { get; set; }
        public object Url { get; set; }
        public int Statuses_count { get; set; }
        public int Followers_count { get; set; }
        public int Friends_count { get; set; }
        public bool Verified { get; set; }
        public string Profile_image_url_https { get; set; }
        public int Favourites_count { get; set; }
        public int Listed_count { get; set; }

        //Check when user was updated
        public DateTime UserUpdated { get; set; } = DateTime.Now;
    }
}
