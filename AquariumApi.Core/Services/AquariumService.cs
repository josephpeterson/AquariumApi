using AquariumApi.DataAccess;
using AquariumApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.Core
{
    public interface IAquariumService
    {
        List<Aquarium> GetAllAquariums();
        Aquarium AddAquarium(Aquarium aquarium);
        Aquarium GetAquarium(int id);
        List<AquariumSnapshot> GetSnapshots(int aquariumId);
        AquariumSnapshot TakeSnapshot(int aquariumId);
    }
    public class AquariumService : IAquariumService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<AquariumService> _logger;
        private readonly IConfiguration _config;

        //Drivers
        private readonly IPhotoManager _photoManager;
        public AquariumService(IConfiguration config,IAquariumDao aquariumDao,IPhotoManager photoManager, ILogger<AquariumService> logger)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;
            _photoManager = photoManager;
            //_photoManager = new PhotoManager(config);
        }

        public List<Aquarium> GetAllAquariums()
        {
            return _aquariumDao.GetAquariums();
        }
        public Aquarium AddAquarium(Aquarium aquarium)
        {
            return _aquariumDao.AddAquarium(aquarium);
        }
        public List<AquariumSnapshot> GetSnapshots(int aquariumId)
        {
            return _aquariumDao.GetSnapshots().Where(s => s.AquariumId == aquariumId).ToList();
        }

        public Aquarium GetAquarium(int id)
        {
            var all = _aquariumDao.GetAquariums();
            return all.Where(aq => aq.Id == id).FirstOrDefault();
        }

        public AquariumSnapshot TakeSnapshot(int aquariumId)
        {
            var snapshotId = _aquariumDao.GetSnapshots().Where(s => s.AquariumId == aquariumId).Count();
            var snapshot = new AquariumSnapshot()
            {
                AquariumId = aquariumId,
                Date = DateTime.Now,
                Temperature = 0,
                Nitrate = 0.0M,
                Nitrite = 0.0M,
                Ph = 5.5M
            };
            AquariumSnapshot newSnapshot = _aquariumDao.AddSnapshot(snapshot);
            return newSnapshot;
        }
    }
}
