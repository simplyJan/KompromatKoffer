using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace KompromatKoffer.Model
{
    public class MdBModel
    {

        [Key]
        public int ID { get; set; }

        public long TwitterID { get; set; }
        public string IdString { get; set; }
        public string TwitterName { get; set; }
        public string TwitterScreenName { get; set; }
        public string TwitterProfileUrl { get; set; }
        public bool Verified { get; set; }
        public string TwitterDesc { get; set; }
        public string Location { get; set; }
        public string LinkedUrl { get; set; }
        public int StatusesCount { get; set; }
        public int FollowersCount { get; set; }
        public int FriendsCount { get; set; }
        public int FavCounts { get; set; }
        public string ProfileImageUrlHttps { get; set; }
        public DateTime CreatedAt { get; set; }

        //Last Status and TimeLine
        public string LastStatus { get; set; }
        public DateTime LastStatusCreated { get; set; }

        public string Party { get; set; }

        public string Abgeordnetenwatch { get; set; }

        

    }
}
