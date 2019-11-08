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
    public interface IFishService
    {
        Fish CreateFish(Fish fish);
        Fish GetFishById(int fishId);
        List<Fish> GetAllFishByAccount(int accountId);
        void DeleteFishById(int fishId);
        Fish UpdateFish(Fish fish);
        Feeding FeedFish(Feeding fish);
        Fish TransferFish(int fishId,int aquariumId);
        Fish MarkDeseased(FishDeath death);
        FishBreeding Breed(FishBreeding breeding);
        FishDisease AddDiseaseDiagnosis(FishDisease diseaseDiagnois);

    }
    public class FishService : IFishService
    {
        private IConfiguration _configuration;
        private IAquariumDao _aquariumDao;

        public FishService(IConfiguration configuration,IAquariumDao aquariumDao)
        {
            _configuration = configuration;
            _aquariumDao = aquariumDao;
        }

        public FishDisease AddDiseaseDiagnosis(FishDisease diseaseDiagnois)
        {
            return _aquariumDao.AddDiseaseDiagnosis(diseaseDiagnois);
        }

        public FishBreeding Breed(FishBreeding breeding)
        {
            //todo check if fish can breed with eachother
            var mother = _aquariumDao.GetFishById(breeding.MotherId);
            var father = _aquariumDao.GetFishById(breeding.FatherId);
            if (mother.SpeciesId != father.SpeciesId)
                throw new Exception("These species cannot breed with eachother");
            if (breeding.Amount > 25)
                throw new Exception("This is too large of a breeding");

            var newBreeding = _aquariumDao.AddBreeding(breeding);
            return newBreeding;
        }

        public Fish CreateFish(Fish fish)
        {
            fish.Name = fish.Name.Trim();
            fish.Description = fish.Description?.Trim();

            if (string.IsNullOrEmpty(fish.Name))
                throw new Exception("Invalid fish name");


            return _aquariumDao.AddFish(fish);
        }

        public void DeleteFishById(int fishId)
        {
            _aquariumDao.DeleteFish(fishId);
        }

        public Feeding FeedFish(Feeding feed)
        {
            return _aquariumDao.AddFeeding(feed);
        }

        public List<Fish> GetAllFishByAccount(int accountId)
        {
            return _aquariumDao.GetAllFishByAccount(accountId);
        }

        public Fish GetFishById(int fishId)
        {
            return _aquariumDao.GetFishById(fishId);
        }

        public Fish MarkDeseased(FishDeath death)
        {
            return _aquariumDao.MarkDeseased(death);
        }

        public Fish TransferFish(int fishId, int aquariumId)
        {
            return _aquariumDao.TransferFish(fishId, aquariumId);
        }

        public Fish UpdateFish(Fish fish)
        {
            return _aquariumDao.UpdateFish(fish);
        }
        
    }
}
