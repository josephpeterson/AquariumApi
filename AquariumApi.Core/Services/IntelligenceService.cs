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
    /* This service will provide all of the business logic pertaining to analysis of aquariums */
    public interface IIntelligenceService
    {
        SuggestionBias GetWaterChangeSuggestion();
        SuggestionBias GetParameterTestSuggestion();
        IntelligentSuggestion GetSuggestion();
    }
    public class IntelligenceService : IIntelligenceService
    {
        private readonly IAquariumDao _aquariumDao;
        private readonly ILogger<IntelligenceService> _logger;
        private readonly IDeviceClient _deviceService;
        private readonly IPhotoManager _photoManager;
        private readonly IConfiguration _config;

        public IntelligenceService(IConfiguration config, IAquariumDao aquariumDao, IDeviceClient deviceService, ILogger<IntelligenceService> logger, IPhotoManager photoManager)
        {
            _config = config;
            _aquariumDao = aquariumDao;
            _logger = logger;
            _deviceService = deviceService;
            _photoManager = photoManager;
        }

        public SuggestionBias GetParameterTestSuggestion(int aquariumId,PaginationSliver pagination)
        {
            var testEvery = 14; //days

            var parameters = _aquariumDao.GetWaterParametersByAquarium(aquariumId, pagination).OrderByDescending(s => s.StartTime);
            var lastTest = parameters.FirstOrDefault();
            if (lastTest != null && DateTime.Now - lastTest.StartTime >= TimeSpan.FromDays(testEvery))
                return new SuggestionBias()
                {
                    Level = SuggestionLevel.Due,
                };
            return new SuggestionBias()
            {
                Level = SuggestionLevel.OnTrack
            };
        }

        public SuggestionBias GetParameterTestSuggestion()
        {
            throw new NotImplementedException();
        }

        public IntelligentSuggestion GetSuggestion()
        {
            throw new NotImplementedException();
        }

        public SuggestionBias GetWaterChangeSuggestion()
        {
            throw new NotImplementedException();
        }
        public SuggestionBias GetWaterQuality(int aquariumId,PaginationSliver pagination)
        {

            throw new NotImplementedException();
        }
    }

    public class IntelligentSuggestion {
        public SuggestionBias WaterChangeSuggestion { get; set; }
        public SuggestionBias ParameterTestSuggestion { get; set; }
    }
    public class SuggestionBias
    {
        public SuggestionLevel Level { get; set; }
    }
    public enum SuggestionLevel
    {
        OnTrack,
        Due,
        Overdue,
        Underdue
    }
}
