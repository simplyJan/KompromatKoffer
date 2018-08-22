using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KompromatKoffer.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Tweetinvi;
using Tweetinvi.Streaming;

namespace KompromatKoffer.Pages.Stream
{

    [Authorize]
    public class UserStreamModel : PageModel
    {

        private readonly IHubContext<UserStreamHub> _hubContext;

        public UserStreamModel(IHubContext<UserStreamHub> hubContext)
        {
            _hubContext = hubContext;
        }


        public async Task OnGetAsync()
        {

            await Task.Delay(1);
           //await _hubContext.Clients.All.SendAsync("Notify", $"Home page loaded at: {DateTime.Now}");


            
            

        } 

    }

}