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
        List<Aquarium> GetTanks();
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

        public List<Aquarium> GetTanks()
        {
            return _dbAquariumContext.TblTank.AsNoTracking().Select(t => _mapper.Map<Aquarium>(t)).ToList();
        }
    }
}
