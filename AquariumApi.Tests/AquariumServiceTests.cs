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
    private ActivityService _activityService;
    private AccountService _accountService;
    private AquariumService _aquariumService;
    private AquariumDao _aquariumDao;
    private IConfigurationRoot _config;
    private DbAquariumContext _dbAquariumContext;

    public AquariumServiceTests()
    {
        // Set up configuration sources.
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"./config.json", optional: false)
            .AddEnvironmentVariables();
        _config = builder.Build();

        //Set up AutoMapper
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        _mapper = config.CreateMapper();

        //Set up db
        _dbAquariumContext = new DbAquariumContext(new DbContextOptionsBuilder<DbAquariumContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString() + "dbAquarium")
            .Options);
        _aquariumDao = new AquariumDao(_mapper, _config, _dbAquariumContext, new NullLogger<AquariumDao>());
        
        

        //Set up service
        _activityService = new ActivityService(_config, _aquariumDao, _mapper);
        _accountService = new AccountService(null, _config, _aquariumDao, null, null);
        _aquariumService = new AquariumService(_config,
            new NullLogger<AquariumService>(),
            _aquariumDao,
            _accountService,
            _activityService,
            null,
            null);
    }
    private void CreateDatabaseWithAccount()
    {
        _dbAquariumContext.TblAccounts.Add(new AquariumUser
        {
           Username = "Test",
           Email = "test@test.com",
           Role = "User"
        });
        _dbAquariumContext.SaveChanges();
    }


    [Fact]
    public void GivenAccount_CreateAquarium_AquariumCreates()
    {
        _dbAquariumContext.Database.EnsureDeleted();
        CreateDatabaseWithAccount();
        var acc = _dbAquariumContext.TblAccounts.First();

        var expected = new Aquarium()
        {
            Name = "Test Aquarium",
            Gallons = 10,
            OwnerId = acc.Id, //todo - move this into an aquariumservice parameter, have the controller determine the id and validate.
        };
        var aq = _aquariumService.AddAquarium(expected);
        var actual = _aquariumService.GetAquariumById(aq.Id);
        Assert.NotNull(actual);
    }
    [Fact]
    public void GivenAccount_DeleteAquarium_AquariumDeletes()
    {
        _dbAquariumContext.Database.EnsureDeleted();
        CreateDatabaseWithAccount();
        var acc = _dbAquariumContext.TblAccounts.First();

        var aquarium = new Aquarium()
        {
            Name = "Test Aquarium",
            Gallons = 10,
            OwnerId = acc.Id, //todo - move this into an aquariumservice parameter, have the controller determine the id and validate.
        };
        aquarium = _aquariumService.AddAquarium(aquarium);
        _aquariumService.DeleteAquarium(aquarium.Id);
        Assert.Null(_aquariumService.GetAquariumById(aquarium.Id));
    }
    [Fact]
    public void GivenAccountHasAquarium_AddFish_FishIsAdded()
    {
        _dbAquariumContext.Database.EnsureDeleted();
        CreateDatabaseWithAccount();
        var acc = _dbAquariumContext.TblAccounts.First();

        var aquarium = new Aquarium()
        {
            Name = "Test Aquarium",
            Gallons = 10,
            OwnerId = acc.Id, //todo - move this into an aquariumservice parameter, have the controller determine the id and validate.
        };
        aquarium = _aquariumService.AddAquarium(aquarium);

        var fish = new Fish()
        {
            Name = "Basic fish",
            AquariumId = aquarium.Id
        };

        var newfish = _aquariumService.AddFish(fish);


        var expected = fish.Name;
        var actual = _aquariumService.GetAquariumById(aquarium.Id).Fish.First().Name;
        Assert.Equal(expected,actual);
    }
}