using AquariumApi.Core;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AquariumApi.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class AquariumServiceTests
{
    IMapper _mapper;
    private DbContextOptions<DbAquariumContext> _dbAquariumContextOptions;
    DbAquariumContext _dbAquariumContext;
    AquariumService _aquariumService;
    AquariumDao _aquariumDao;
    private IConfigurationRoot _config;

    public AquariumServiceTests()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        // Set up configuration sources.
        var builder = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile($"./config.json", optional: false)
            .AddEnvironmentVariables();
        _config = builder.Build();

        var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        _mapper = config.CreateMapper();
        //Set up db
        _dbAquariumContextOptions = new DbContextOptionsBuilder<DbAquariumContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString() + "dbAquarium")
            .Options;
        //_dbAquariumContext = new DbAquariumContext();
        //_aquariumDao = , new NullLogger<AquariumDao>());
        //_aquariumService = new AquariumService(_config,new AquariumDao(dbAquarium, _mapper, new NullLogger<AquariumService>());
    }
    /*
    [Fact]
    public void GivenAquarium_AquariumIsAdded()
    {
        Aquarium expected = new Aquarium
        {
            Id = 1,
            Gallons = 30,
        };
        var dbAquarium = new DbAquariumContext(_dbAquariumContextOptions);
        var aquariumService = new AquariumService(_config,
            new AquariumDao(dbAquarium, _mapper, new NullLogger<AquariumDao>()),
            new NullLogger<AquariumService>());

        aquariumService.AddAquarium(expected);
        var test = aquariumService.GetAquariumById(expected.Id);
        Assert.Equal(expected.Gallons, test.Gallons);
    }
    */
    [Fact]
    public void Stuff()
    {
        var aquariumId = 1;
        Aquarium aquarium = new Aquarium
        {
            Id = aquariumId,
            Gallons = 30,
        };
        AquariumSnapshot snapshot = new AquariumSnapshot
        {
            Id = 1,
            AquariumId = aquariumId,
            Ammonia = 0,
            Nitrate = 0,
            Nitrite = 0,
            Temperature = 70,
            Ph = 7.0M,
            Date = DateTime.Now
        };

        /*
        //Setup mock photo manager
        var photoManager = new Mock<IPhotoManager>();
        photoManager.Setup(p => p.TakePhoto()).Returns(Task.FromResult("temp.jpg"));
        //Setup mock database...
        var dbAquarium = new DbAquariumContext(_dbAquariumContextOptions);
        var aquariumService = new AquariumService(_config,
            new AquariumDao(dbAquarium, _mapper, new NullLogger<AquariumDao>()),
            new PhotoManager(_config),
            new NullLogger<AquariumService>());
        //dbAquarium.Database.EnsureDeleted();

        aquariumService.AddAquarium(aquarium);
        aquariumService.TakeSnapshot(aquariumId);

        
        
        var test = aquariumService.GetSnapshots(aquariumId).First();
        Assert.NotNull(test);
        */
    }
}