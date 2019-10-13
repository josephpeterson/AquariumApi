using AquariumApi.DataAccess;
using AquariumApi.Models;
using AutoMapper;
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
        Activity GetActivity(int activityId);
        List<Activity> GetRecentActivity(int accountId);
        void RegisterActivity(Activity newActivity);
    }
    public class ActivityService : IActivityService
    {
        private IConfiguration _configuration;
        private IMapper _mapper;
        private IAquariumDao _aquariumDao;

        public ActivityService(IConfiguration configuration,IAquariumDao aquariumDao, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
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
        public Activity GetActivity(int activityId)
        {
            var activity = _aquariumDao.GetAccountActivity(activityId);
            var con = ExpandActivity(activity);
            return con;
        }
        private Activity ExpandActivity(Activity activity)
        {
            switch(activity.ActivityType)
            {
                case ActivityTypes.CreateAquariumTestResults:
                    var a = _mapper.Map<CreateAquariumTestResultsActivity>(activity);
                    a.Snapshot = _aquariumDao.GetSnapshotById(a.SnapshotId);
                    return a;
                default:
                    return activity;
            }
        }
    }
}
