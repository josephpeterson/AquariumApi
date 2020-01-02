using AquariumApi.Core;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AquariumApi.DeviceApi;
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

public class AquariumDeviceServiceTests
{
    IMapper _mapper;
    private ActivityService _activityService;
    private AccountService _accountService;
    private AquariumService _aquariumService;
    private AquariumDao _aquariumDao;
    private IConfigurationRoot _config;
    private DbAquariumContext _dbAquariumContext;
    private ScheduleService _scheduleService;

    public AquariumDeviceServiceTests()
    {
        // Set up configuration sources.
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"./config.json", optional: false)
            .AddEnvironmentVariables();
        _config = builder.Build();

        //Set up service
        _scheduleService = new ScheduleService(_config, null, null, null);


    }
    [Fact]
    public void GivenSchedule_ExpandSchedule_CreatesExpandedTasks()
    {
        DeviceSchedule schedule = new DeviceSchedule()
        {
            Tasks = new List<DeviceScheduleTask>()
            {
                new DeviceScheduleTask()
                {
                    TaskId = ScheduleTaskTypes.Snapshot,
                    Interval = 20,
                    StartTime = DateTime.Now
                }
            }
        };

        var scheduledTasks = _scheduleService.ExpandSchedule(schedule);
        Assert.Equal(24*3, scheduledTasks.Count);
    }
    [Fact]
    public void GivenExpandedSchedule_GetFutureTask_ReturnsFutureTask()
    {
        var scheduleStart = Convert.ToDateTime("05/05/2005 06:00:00");
        var date = Convert.ToDateTime("05/05/2005 07:24:00");
        DeviceSchedule schedule = new DeviceSchedule()
        {
            Tasks = new List<DeviceScheduleTask>()
            {
                new DeviceScheduleTask()
                {
                    TaskId = ScheduleTaskTypes.Snapshot,
                    Interval = 20,
                    StartTime = scheduleStart
                }
            }
        };
        var scheduledTasks = _scheduleService.ExpandSchedule(schedule);

        var futureTask = _scheduleService.GetNextTask(scheduledTasks, date);

        var expected = 5;
        var expectedEta = 16;
        Assert.Equal(expected, scheduledTasks.IndexOf(futureTask.task));
        Assert.Equal(expectedEta, futureTask.eta.TotalMinutes);
    }
    [Fact]
    public void GivenExpandedScheduleAtEndOfDay_GetFutureTask_ReturnsFutureTask()
    {
        var scheduleStart = Convert.ToDateTime("05/05/2005 06:00:00");
        var date = Convert.ToDateTime("05/05/2005 00:00:00");
        DeviceSchedule schedule = new DeviceSchedule()
        {
            Tasks = new List<DeviceScheduleTask>()
            {
                new DeviceScheduleTask()
                {
                    TaskId = ScheduleTaskTypes.Snapshot,
                    Interval = 20,
                    StartTime = scheduleStart
                }
            }
        };
        var scheduledTasks = _scheduleService.ExpandSchedule(schedule);

        var futureTask = _scheduleService.GetNextTask(scheduledTasks, date);

        var expected = 0;
        var expectedEta = 6;
        Assert.Equal(expected, scheduledTasks.IndexOf(futureTask.task));
        Assert.Equal(expectedEta, futureTask.eta.TotalHours);
    }
}