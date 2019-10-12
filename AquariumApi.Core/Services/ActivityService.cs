using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
    public interface IActivityService
    {
        List<Activity> GetRecentActivity(int accountId);
        void RegisterActivity(Activity newActivity);
    }
    public class ActivityService : IActivityService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public ActivityService(IConfiguration configuration,IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }
        public void RegisterActivity(Activity newActivity)
        {
            newActivity.Timestamp = DateTime.Now;
            _aquariumDao.RegisterActivity(newActivity);
        }
        public List<Activity>GetRecentActivity(int accountId)
        {
            var recentActivity = _aquariumDao.GetRecentAccountActivity(accountId);

            return recentActivity;
        }
    }
}
