using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KompromatKoffer.Model;

namespace KompromatKoffer.Models
{
    public class MdBContext : DbContext
    {
        public MdBContext (DbContextOptions<MdBContext> options)
            : base(options)
        {
        }

        public DbSet<KompromatKoffer.Model.MdBModel> MdBModel { get; set; }
    }
}
