using AquariumApi.Models;
using AutoMapper;
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

        public AquariumSnapshot AddSnapshot(AquariumSnapshot model)
        {
            _dbAquariumContext.TblSnapshot.Add(model);
            _dbAquariumContext.SaveChanges();
            return model;
        }
    }
}
