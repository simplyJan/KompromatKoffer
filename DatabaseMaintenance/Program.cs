using System;
using KompromatKoffer.Areas.Database.Model;
using LiteDB;

namespace DatabaseMaintenance
{
    class Program
    {
        static void Main(string[] args)
        {

            PoliticalPartyUpdate();
            

        }


        private static void PoliticalPartyUpdate()
        {
            using (var db = new LiteDatabase("TwitterData.db"))
            {
                try
                {
                    Console.WriteLine("Getting Collection");
                    var col = db.GetCollection<TwitterUserModel>("TwitterUser");


                    col.Find


                }
                catch(LiteException ex)
                {
                    Console.WriteLine("LiteDB Exception: " + ex);
                }


            }



        }

    }
}
