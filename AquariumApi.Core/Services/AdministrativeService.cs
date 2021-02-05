using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IAdministrativeService
    {
        List<AquariumUser> GetAquariumUsers();
        List<BugReport> GetBugReports();
    }
    public class AdministrativeService : IAdministrativeService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<AdministrativeService> _logger;
        private readonly IDeviceClient _deviceService;
        private readonly IPhotoManager _photoManager;
        private readonly IConfiguration _config;

        public AdministrativeService(IConfiguration config, IAquariumDao aquariumDao, IDeviceClient deviceService, ILogger<AdministrativeService> logger, IPhotoManager photoManager)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;
            _deviceService = deviceService;
            _photoManager = photoManager;
        }

        public List<AquariumUser> GetAquariumUsers()
        {
            return _aquariumDao.GetAllAccounts();
        }

        public List<BugReport> GetBugReports()
        {
            return _aquariumDao.GetAllBugs();
        }
    }
}
