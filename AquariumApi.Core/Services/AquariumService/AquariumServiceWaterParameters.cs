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
        ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId,PaginationSliver pagination = null);
        WaterChange UpsertWaterChange(WaterChange waterChange);
        void DeleteWaterChanges(List<int> waterChangeIds);
        ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId,PaginationSliver pagination = null);
        WaterDosing AddWaterDosing(WaterDosing waterDosing);
        WaterDosing UpdateWaterDosing(WaterDosing waterDosing);
        void DeleteWaterDosings(List<int> waterDosingIds);
        ICollection<AquariumSnapshot> GetWaterParametersByAquarium(int aquariumId, PaginationSliver pagination);
        AquariumSnapshot AddWaterParametersByAquarium(int aquariumId, AquariumSnapshot snapshot);
        ICollection<ATOStatus> GetWaterATOStatusesByAquarium(int aquariumId, PaginationSliver pagination);
        ATOStatus UpsertWaterATO(ATOStatus waterATO);

    }
    public partial class AquariumService : IAquariumService
    {
        /* Water Changes */
        #region Water Parameters
        public ICollection<AquariumSnapshot> GetWaterParametersByAquarium(int aquariumId,PaginationSliver pagination)
        {
            return _aquariumDao.GetWaterParametersByAquarium(aquariumId,pagination);
        }
        public AquariumSnapshot AddWaterParametersByAquarium(int aquariumId, AquariumSnapshot snapshot)
        {
            snapshot.AquariumId = aquariumId;
            return _aquariumDao.AddSnapshot(snapshot);
        }
        #endregion
        #region Water Changes
        public ICollection<WaterChange> GetWaterChangesByAquarium(int aquariumId,PaginationSliver pagination)
        {
            return _aquariumDao.GetWaterChangesByAquarium(aquariumId,pagination);
        }
        public WaterChange UpsertWaterChange(WaterChange waterChange)
        {
            return _aquariumDao.UpsertWaterChange(waterChange);
        }
        public void DeleteWaterChanges(List<int> waterChangeIds)
        {
            _aquariumDao.DeleteWaterChanges(waterChangeIds);
        }
        #endregion
        #region Water Dosing
        public ICollection<WaterDosing> GetWaterDosingsByAquarium(int aquariumId, PaginationSliver pagination)
        {
            return _aquariumDao.GetWaterDosingsByAquarium(aquariumId,pagination);
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
        #endregion
        #region Water Auto Top Off
        public ICollection<ATOStatus> GetWaterATOStatusesByAquarium(int aquariumId,PaginationSliver pagination)
        {
            return _aquariumDao.GetWaterATOs(aquariumId,pagination);
        }
        public ATOStatus UpsertWaterATO(ATOStatus waterATO)
        {
            return _aquariumDao.UpsertWaterATO(waterATO);
        }
        public void DeleteWaterATOs(List<int> waterATOIds)
        {
            _aquariumDao.DeleteWaterATOs(waterATOIds);
        }
        #endregion
    }
}