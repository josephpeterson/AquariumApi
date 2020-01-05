using AquariumApi.Core;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AquariumApi.DeviceApi.Clients;
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

public class DeviceIntegrationTests
{
    IMapper _mapper;
    private ActivityService _activityService;
    private AccountService _accountService;
    private AquariumService _aquariumService;
    private AquariumDao _aquariumDao;
    private IConfigurationRoot _config;
    private DbAquariumContext _dbAquariumContext;

    public DeviceIntegrationTests()
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
    [Fact]
    public void GivenServiceAvailable_TestDeviceUploadIntegration()
    {
        var logger = new NullLogger<AquariumClient>();
        var aquariumClient = new AquariumClient(logger, null);

        var host = "http://localhost:5000";
        var deviceId = 18;
        AquariumSnapshot snapshot = new AquariumSnapshot()
        {
            ManualEntry = true,
            Date = Convert.ToDateTime("05/11/1996")
        };

        var photo = File.ReadAllBytes("temp.jpg");

        var data = aquariumClient.SendAquariumSnapshotToHost(host, deviceId, snapshot, photo);

        Assert.NotNull(data);
    }
}