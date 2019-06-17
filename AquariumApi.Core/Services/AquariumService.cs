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
        Aquarium GetAquariumById(int id);
        List<AquariumSnapshot> GetSnapshots(int aquariumId);
        AquariumSnapshot TakeSnapshot(int aquariumId,bool forcePhoto);
        Aquarium UpdateAquarium(Aquarium aquarium);
        void DeleteAquarium (int aquariumId);
        void DeleteSnapshot(int removeSnapshotId);
        AquariumSnapshot GetSnapshotById(int snapshotId);
        void DeleteSpecies(int speciesId);
        Species AddSpecies(Species species);
        Species UpdateSpecies(Species species);
        List<Species> GetAllSpecies();
        void DeleteFish(int fishId);
        Fish UpdateFish(Fish fish);
        Fish AddFish(Fish fish);
        Fish GetFishById(int fishId);
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
        public Aquarium UpdateAquarium(Aquarium aquarium)
        {
            return _aquariumDao.UpdateAquarium(aquarium);
        }
        public Aquarium GetAquariumById(int id)
        {
            return _aquariumDao.GetAquariumById(id);
        }
        public void DeleteAquarium(int aquariumId)
        {
            _aquariumDao.DeleteAquarium(aquariumId);
        }

        /* Snapshots */
        public List<AquariumSnapshot> GetSnapshots(int aquariumId)
        {
            return _aquariumDao.GetSnapshots().Where(s => s.AquariumId == aquariumId).ToList();
        }
        public AquariumSnapshot GetSnapshotById(int snapshotId)
        {
            return _aquariumDao.GetSnapshotById(snapshotId);
        }
        public void DeleteSnapshot(int snapshotId)
        {
            var snapshot = _aquariumDao.DeleteSnapshot(snapshotId);
        }

        public AquariumSnapshot TakeSnapshot(int aquariumId,bool forcePhoto)
        {
            int snapshotId = _aquariumDao.GetSnapshots().ToList().Count();
            var path = String.Format(_config["PhotoFilePath"], aquariumId, snapshotId);
            var config = _aquariumDao.GetAquariumById(aquariumId).CameraConfiguration ?? new CameraConfiguration();
            config.Output = path;



            //Take photo
            try
            {
                _photoManager.TakePhoto(config);
            }
            catch (Exception e)
            {
                if (forcePhoto)
                    throw e;
                _logger.LogError(e.ToString());
                path = null;
            }


            var snapshot = new AquariumSnapshot()
            {
                AquariumId = aquariumId,
                Date = DateTime.Now,
                Temperature = 0,
                Nitrate = 0.0M,
                Nitrite = 0.0M,
                Ph = 5.5M,
                PhotoPath = path
            };
            AquariumSnapshot newSnapshot = _aquariumDao.AddSnapshot(snapshot);
            return newSnapshot;
        }
        
        public List<Species> GetAllSpecies()
        {
            return _aquariumDao.GetAllSpecies();
        }
        public Species AddSpecies(Species species)
        {
            return _aquariumDao.AddSpecies(species);
        }
        public Species UpdateSpecies(Species species)
        {
            return _aquariumDao.UpdateSpecies(species);
        }
        public void DeleteSpecies(int speciesId)
        {
            _aquariumDao.DeleteSpecies(speciesId);
        }
        public Fish GetFishById(int fishId)
        {
            return _aquariumDao.GetFishById(fishId);
        }
        public Fish AddFish(Fish fish)
        {
            return _aquariumDao.AddFish(fish);
        }
        public Fish UpdateFish(Fish fish)
        {
            return _aquariumDao.UpdateFish(fish);
        }
        public void DeleteFish(int fishId)
        {
            _aquariumDao.DeleteFish(fishId);
        }
    }
}
