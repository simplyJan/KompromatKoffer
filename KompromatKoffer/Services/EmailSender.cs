using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net; 
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;

namespace KompromatKoffer.Services
{
    public class EmailSender : IEmailSender
    {

        private readonly ILogger _logger;



        private string _sendGridKey;


        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
            {
                _sendGridKey = optionsAccessor.Value.SendGridKey;
            _logger = logger;
        }

            public AuthMessageSenderOptions Options { get; } //set only via Secret Manager

            public Task SendEmailAsync(string email, string subject, string message)
            {
                _logger.LogWarning(">>>> Trying send mail with: " + _sendGridKey);

                return Execute(_sendGridKey, subject, message, email);
            }

            public Task Execute(string apiKey, string subject, string message, string email)
            {
                var client = new SendGridClient(apiKey);
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress(Config.Parameter.Mail_From_Email_Address, Config.Parameter.Mail_From_Email_DisplayName),
                    Subject = subject,
                    PlainTextContent = message,
                    HtmlContent = message
                };
                msg.AddTo(new EmailAddress(email));

                // Disable click tracking.
                // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
                msg.SetClickTracking(false, false);

                return client.SendEmailAsync(msg);
            }
        }
    }
