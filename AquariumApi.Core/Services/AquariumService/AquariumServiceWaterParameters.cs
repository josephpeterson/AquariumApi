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
    public partial interface IAquariumService
    {
        ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId);
        WaterChange AddWaterChange(WaterChange waterChange);
        WaterChange UpdateWaterChange(WaterChange waterChange);
        void DeleteWaterChanges(List<int> waterChangeIds);
        ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId);
        WaterDosing AddWaterDosing(WaterDosing waterDosing);
        WaterDosing UpdateWaterDosing(WaterDosing waterDosing);
        void DeleteWaterDosings(List<int> waterDosingIds);
        ICollection<AquariumSnapshot> GetWaterParametersByAquarium(int aquariumId, PaginationSliver pagination);
    }
    public partial class AquariumService : IAquariumService
    {
        /* Water Changes */
        public ICollection<AquariumSnapshot> GetWaterParametersByAquarium(int aquariumId,PaginationSliver pagination)
        {
            return _aquariumDao.GetWaterParametersByAquarium(aquariumId,pagination);
        }
        public ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId)
        {
            return _aquariumDao.GetWaterChangesByAquarium(aquariumId);
        }
        public WaterChange AddWaterChange(WaterChange waterChange)
        {
            return _aquariumDao.AddWaterChange(waterChange);
        }
        public WaterChange UpdateWaterChange(WaterChange waterChange)
        {
            return _aquariumDao.UpdateWaterChange(waterChange);
        }
        public void DeleteWaterChanges(List<int> waterChangeIds)
        {
            _aquariumDao.DeleteWaterChanges(waterChangeIds);
        }
        public ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId)
        {
            return _aquariumDao.GetWaterDosingsByAquarium(aquariumId);
        }
        public WaterDosing AddWaterDosing(WaterDosing waterDosing)
        {
            return _aquariumDao.AddWaterDosing(waterDosing);
        }
        public WaterDosing UpdateWaterDosing(WaterDosing waterDosing)
        {
            return _aquariumDao.UpdateWaterDosing(waterDosing);
        }
        public void DeleteWaterDosings(List<int> waterDosingIds)
        {
            _aquariumDao.DeleteWaterDosings(waterDosingIds);
        }
    }
}