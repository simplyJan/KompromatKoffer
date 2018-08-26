using KompromatKoffer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;


namespace KompromatKoffer
{
    public class Startup
    {

        private readonly ILogger _logger;

        public Startup(ILogger<Startup> logger)
        {
            _logger = logger;
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            //Set DB to Update after StartUp
            Config.Parameter.DbLastUpdated = DateTime.Now;
            Config.Parameter.UserDailyDataLastUpdated = DateTime.Now.AddMinutes(5);

            // Add application services.
            services.AddSingleton<IEmailSender, EmailSender>();

            //Background Service for daily saving TwitterUser data to database
            if (Config.Parameter.SaveToDatabase == true)
            {
                services.AddHostedService<TwitterUserData>();
            }

            //Get the TwitterUser on a daily basis
            if (Config.Parameter.TwitterUserDaily == true)
            {
                services.AddHostedService<TwitterUserDailyData>();
            }

            //Currently no TimeLineDatabase
            //services.AddHostedService<TwitterUserTimelineData>();

            //Update the TwitterCounts for the TwitterStream
            if (Config.Parameter.UpdateTwittterCounts == true)
            {
                services.AddHostedService<TwitterStreamCountUpdate>();
            }
         
            if(Config.Parameter.DoBackup == true)
            {
                services.AddHostedService<BackupService>();
            }

            
            //Authorize for Admins
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
            });


            //TwitterStream
            if (Config.Parameter.TwitterStream == true)
            {
                services.AddHostedService<ConsumeScopedServiceHostedService>();
                services.AddScoped<TwitterStreamService, ScopedProcessingService>();
            }

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IServiceProvider services, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseAuthentication();

    
            app.UseMvc();

        }


        
    }

    


}
