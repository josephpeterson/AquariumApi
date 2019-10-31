using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IEmailerService
    {
        Task SendAsync(string recieveAddress, string subject, string content);
    }
    public class EmailerService : IEmailerService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public EmailerService(IConfiguration configuration,IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }
        public async Task SendAsync(string recieveAddress,string subject,string content)
        {
            var apiKey = _configuration["Emailer:ApiKey"];
            var fromAddress = _configuration["Emailer:Address1"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromAddress, "Example User");
            var to = new EmailAddress(recieveAddress, "Example User");
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            await client.SendEmailAsync(msg);
        }
    }
}
