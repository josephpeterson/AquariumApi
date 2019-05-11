using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IAquariumService
    {
        List<Aquarium> GetAllAquariums();
    }
    public class AquariumService : IAquariumService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<AquariumService> _logger;
        public AquariumService(IAquariumDao aquariumDao, ILogger<AquariumService> logger)
        {
            _aquariumDao = aquariumDao;
            _logger = logger;
        }

        public List<Aquarium> GetAllAquariums()
        {
            return _aquariumDao.GetTanks();
        }
    }
}
