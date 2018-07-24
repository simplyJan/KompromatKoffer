using System;
using KompromatKoffer.Areas.Identity.Data;
using KompromatKoffer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;

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
                    .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();

                services.AddAuthentication().AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = Config.Credentials.CONSUMER_KEY;
                    twitterOptions.ConsumerSecret = Config.Credentials.CONSUMER_SECRET;
                    twitterOptions.SaveTokens = true;
                });

                Auth.SetUserCredentials(Config.Credentials.CONSUMER_KEY, Config.Credentials.CONSUMER_SECRET, Config.Credentials.ACCESS_TOKEN, Config.Credentials.ACCESS_TOKEN_SECRET);

            });
        }
    }
}