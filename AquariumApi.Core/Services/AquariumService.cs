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
        AquariumSnapshot TakeSnapshot(int aquariumId,bool forcePhoto);
        Aquarium UpdateAquarium(Aquarium aquarium);
        void DeleteAquarium (int aquariumId);
        void DeleteSnapshot(int removeSnapshotId);
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
        public List<AquariumSnapshot> GetSnapshots(int aquariumId)
        {
            return _aquariumDao.GetSnapshots().Where(s => s.AquariumId == aquariumId).ToList();
        }

        public Aquarium GetAquarium(int id)
        {
            var all = _aquariumDao.GetAquariums();
            return all.Where(aq => aq.Id == id).FirstOrDefault();
        }
        public void DeleteAquarium(int aquariumId)
        {
           _aquariumDao.DeleteAquarium(aquariumId);
        }
        public void DeleteSnapshot(int snapshotId)
        {
            var snapshot = _aquariumDao.DeleteSnapshot(snapshotId);
            if(snapshot.PhotoId != null)
            {
                var destination = _config["PhotoSubPath"] + $"{snapshot.AquariumId}/{snapshot.PhotoId}.jpg";
                if(File.Exists(destination))
                    System.IO.File.Delete(destination);
            }
        }

        public AquariumSnapshot TakeSnapshot(int aquariumId,bool forcePhoto)
        {
            int? photoId = _aquariumDao.GetSnapshots().Where(s => s.AquariumId == aquariumId).Count();

            //Take photo
            try
            {
                var folder = _config["PhotoSubPath"] + $"{aquariumId}";
                var destination = $"{folder}/{photoId}.jpg";
                Directory.CreateDirectory(folder);
                _logger.LogInformation($"Taking photo snapshot with photoId: {photoId}...");
                var photo = _photoManager.TakePhoto().Result;
                _logger.LogInformation($"Moving photo {photo}...");
                System.IO.File.Move(photo, destination);
            }
            catch (Exception e)
            {
                if (forcePhoto)
                    throw e;
                _logger.LogError(e.ToString());
                photoId = null;
            }


            var snapshot = new AquariumSnapshot()
            {
                AquariumId = aquariumId,
                Date = DateTime.Now,
                Temperature = 0,
                Nitrate = 0.0M,
                Nitrite = 0.0M,
                Ph = 5.5M,
                PhotoId = photoId
            };
            AquariumSnapshot newSnapshot = _aquariumDao.AddSnapshot(snapshot);
            return newSnapshot;
        }
    }
}
