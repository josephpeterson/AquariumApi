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
using System.Collections.Generic;
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
    [Fact]
    public void GivenAquariumId_ReturnAquariumProfile_WithCompletedCollections()
    {
        var db = new DbAquariumContext(_dbAquariumContextOptions);



        var account = new AquariumUser()
        {
            Id = 0,
            Username = "test username"
        };
        var aquarium = new Aquarium()
        {
            Id = 2,
            OwnerId = 0,
            Name = "test aquarium"
        };
        var fish = new Fish()
        {
            Id = 10,
            Name = "test fish",
            AquariumId = 2
        };
        var expected = new AquariumProfile()
        {
            Id = 0,
            Biography = "test biography"
        };
        expected.Account = account;
        expected.Aquariums = new List<Aquarium>() { aquarium };
        expected.Fish = new List<Fish>() { fish };

        db.Database.EnsureDeleted();
        db.TblAquariumProfiles.Add(expected);
        db.TblAccounts.Add(account);
        db.TblAquarium.Add(aquarium);
        db.TblFish.Add(fish);
        db.SaveChanges();

        var aquariumService = new AquariumService(_config,
            null,
            new AquariumDao(_mapper, _config, db, new NullLogger<AquariumDao>()),
            null,
            new NullLogger<AquariumService>(),
            null);

        var actual = aquariumService.GetProfileById(account.Id);

        Assert.Equal(expected.Account.Username, actual.Account.Username);
        Assert.Equal(expected.Biography, actual.Biography);
        Assert.Equal(expected.Aquariums.Count(), actual.Aquariums.Count());
        Assert.Equal(expected.Fish.Count(), actual.Fish.Count());
    }
}