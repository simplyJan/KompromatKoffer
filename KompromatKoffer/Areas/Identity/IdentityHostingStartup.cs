using KompromatKoffer.Areas.Identity.Data;
using KompromatKoffer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
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

                /*services.AddDefaultIdentity<ApplicationUser>()
                    .AddRoles<IdentityRole>()
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultTokenProviders();
                */
                services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationContext>()
                .AddDefaultUI()
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

        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


            IdentityResult roleResult;
            //Adding Addmin Role  
            var roleCheck = await RoleManager.RoleExistsAsync("Administrator");
            if (!roleCheck)
            {
                //create the roles and seed them to the database  
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Administrator"));
            }
            //Assign Admin role to the main User here we have given our newly loregistered login id for Admin management  
            ApplicationUser user = await UserManager.FindByEmailAsync("billing@scobiform.com");
            var User = new ApplicationUser();
            await UserManager.AddToRoleAsync(user, "Administrator");

        }

    }
}