using System;
using KompromatKoffer.Areas.Identity.Data;
using KompromatKoffer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(KompromatKoffer.Areas.Identity.IdentityHostingStartup))]
namespace KompromatKoffer.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ApplicationContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ApplicationContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<ApplicationContext>();



            });
        }
    }
}