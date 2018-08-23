using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Tweetinvi.Models.Entities;

namespace KompromatKoffer.Areas.Database.Model
{
    public class TwitterStreamModel
    {
        [BsonId]
        public long TweetID { get; set; }
        public string TweetUser { get; set; }
        public long TweetUserID { get; set; }
        public string TweetUserPicture { get; set; }
        public string TweetUserDesc { get; set; }
        public string TweetText { get; set; }
        public List<Tweetinvi.Models.Entities.IHashtagEntity> TweetHashtags { get; set; }
        public int TweetReTweetCount { get; set; }
        public int TweetFavoriteCount { get; set; }
        public DateTime TweetCreatedAt { get; set; }
        public string TweetUrl { get; set; }



    }
}
