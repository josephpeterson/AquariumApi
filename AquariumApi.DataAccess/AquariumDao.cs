using AquariumApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace AquariumApi.DataAccess
{
    public interface IAquariumDao
    {
        List<Aquarium> GetAquariums();
        List<AquariumSnapshot> GetSnapshots();
        Aquarium AddAquarium(Aquarium aquarium);
        AquariumSnapshot AddSnapshot(AquariumSnapshot snapshot);
        Aquarium UpdateAquarium(Aquarium aquarium);
        void DeleteAquarium(int aquariumId);
        AquariumSnapshot DeleteSnapshot(int snapshotId);
    }

    public class AquariumDao : IAquariumDao
    {
        private readonly DbAquariumContext _dbAquariumContext;
        private readonly ILogger<AquariumDao> _logger;
        private readonly IMapper _mapper;


        public AquariumDao(DbAquariumContext dbAquariumContext, IMapper mapper, ILogger<AquariumDao> logger)
        {
            _mapper = mapper;
            _dbAquariumContext = dbAquariumContext;
            _logger = logger;
        }

        public List<Aquarium> GetAquariums()
        {
            return _dbAquariumContext.TblAquarium.AsNoTracking().ToList();
        }
        public List<AquariumSnapshot> GetSnapshots()
        {
            return _dbAquariumContext.TblSnapshot.AsNoTracking().Include(s => s.Aquarium).ToList();
        }

        public Aquarium AddAquarium(Aquarium model)
        {
            _dbAquariumContext.TblAquarium.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }
        public void DeleteAquarium(int aquariumId)
        {
            var aquarium = GetAquariums().Where(aq => aq.Id == aquariumId).First();
            _dbAquariumContext.TblAquarium.Remove(aquarium);
            _dbAquariumContext.SaveChanges();
        }

        public AquariumSnapshot AddSnapshot(AquariumSnapshot model)
        {
            _dbAquariumContext.TblSnapshot.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }

        public Aquarium UpdateAquarium(Aquarium aquarium)
        {
            var aqToUpdate = _dbAquariumContext.TblAquarium.SingleOrDefault(aq => aq.Id == aquarium.Id);
            if (aqToUpdate == null)
                throw new KeyNotFoundException();
            aqToUpdate.Gallons = aquarium.Gallons;
            aqToUpdate.Type = aquarium.Type;
            aqToUpdate.StartDate = aquarium.StartDate;
            aqToUpdate.Name = aquarium.Name;
            _dbAquariumContext.SaveChanges();
            return aqToUpdate;
        }

        public AquariumSnapshot DeleteSnapshot(int snapshotId)
        {
            var snapshot = GetSnapshots().Where(snap => snap.Id == snapshotId).First();
            _dbAquariumContext.TblSnapshot.Remove(snapshot);
            _dbAquariumContext.SaveChanges();
            return snapshot;
        }
    }
}
