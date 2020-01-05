using AquariumApi.Core;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AquariumApi.DeviceApi;
using AquariumApi.DeviceApi.Clients;
using AquariumApi.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
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
        _scheduleService = new ScheduleService(_config, null, null);


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

        var scheduledTasks = schedule.ExpandTasks();
        Assert.Equal(24*3, scheduledTasks.Count);
    }
    [Fact]
    public void GivenExpandedSchedule_GetFutureTask_ReturnsFutureTask()
    {
        var scheduleStart = Convert.ToDateTime("05/05/2005 06:00:00");
        var date = Convert.ToDateTime("05/05/2005 07:24:00");
        var schedules =
            new List<DeviceSchedule>()
            {
                new DeviceSchedule()
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
                }
            };
        var futureTask = _scheduleService.GetNextTask(schedules, date);
        var tasks = schedules.SelectMany(s => s.ExpandTasks()).ToList();


        var expected = 5;
        var expectedEta = 16;
        Assert.Equal(expected, futureTask.Index);
        Assert.Equal(expectedEta, futureTask.eta.TotalMinutes);
    }
    [Fact]
    public void GivenExpandedScheduleAtEndOfDay_GetFutureTask_ReturnsFutureTask()
    {
        var scheduleStart = Convert.ToDateTime("05/05/2005 06:00:00");
        var date = Convert.ToDateTime("05/05/2005 00:00:00");
        var schedules =
            new List<DeviceSchedule>()
            {
                new DeviceSchedule()
                {
                    Tasks = new List<DeviceScheduleTask>()
                    {
                        new DeviceScheduleTask()
                        {
                            Id = 0,
                            TaskId = ScheduleTaskTypes.Snapshot,
                            Interval = 20,
                            StartTime = scheduleStart
                        },
                        new DeviceScheduleTask()
                        {
                            Id = 1,
                            TaskId = ScheduleTaskTypes.Unknown,
                            Interval = 5,
                            StartTime = scheduleStart.AddHours(2)
                        }
                    }
                }
            };

        var futureTask = _scheduleService.GetNextTask(schedules, date);

        var expected = 0;
        var expectedEta = 6;

        var tasks = schedules.SelectMany(s => s.ExpandTasks());

        Assert.True(futureTask.task.Id == expected);
        Assert.Equal(expectedEta, futureTask.eta.TotalHours);
    }
    [Fact]
    public void GivenScheduleStarted_TaskRunsIntermittently()
    {
        var scheduleStart = Convert.ToDateTime("05/05/2005 06:00:00");
        var date = Convert.ToDateTime("05/05/2005 08:05:00");
        var schedules =
            new List<DeviceSchedule>()
            {
                new DeviceSchedule()
                {
                    Tasks = new List<DeviceScheduleTask>()
                    {
                        
                        new DeviceScheduleTask()
                        {
                            Id = 0,
                            TaskId = ScheduleTaskTypes.Unknown,
                            Interval = 20,
                            StartTime = scheduleStart
                        },
                        new DeviceScheduleTask()
                        {
                            Id = 10,
                            TaskId = ScheduleTaskTypes.Snapshot,
                            Interval = 10,
                            StartTime = scheduleStart,
                            EndTime = scheduleStart.AddHours(1)
                        }
                    }
                }
            };

        var futureTask = _scheduleService.GetNextTask(schedules, date);

        var expectedId = 0;
        var expectedEta = 15;

        Assert.True(futureTask.task.Id == expectedId);
        Assert.Equal(expectedEta, futureTask.eta.TotalMinutes);
    }

    [Fact]
    public void GivenScheduleHasSimpleTasks_TasksApply()
    {
        var date = Convert.ToDateTime("05/05/2005 08:05:00");
        var scheduleStart = Convert.ToDateTime("05/05/2005 06:00:00");
        var expectedTaskCount = 24 + 1 + 6;

        var schedules =
            new List<DeviceSchedule>()
            {
                new DeviceSchedule()
                {
                    Tasks = new List<DeviceScheduleTask>()
                    {
                        new DeviceScheduleTask()
                        {
                            Id = 0,
                            TaskId = ScheduleTaskTypes.Unknown,
                            Interval = 60,
                        },
                        new DeviceScheduleTask()
                        {
                            Id = 1,
                            TaskId = ScheduleTaskTypes.Unknown,
                        },
                        new DeviceScheduleTask()
                        {
                            Id = 10,
                            TaskId = ScheduleTaskTypes.Snapshot,
                            Interval = 10,
                            StartTime = scheduleStart,
                            EndTime = scheduleStart.AddHours(1)
                        }
                    }
                }
            };

        var allScheduledTasks = schedules.SelectMany(s => s.ExpandTasks());

        Assert.Equal(expectedTaskCount, allScheduledTasks.Count());
    }
    [Fact]
    public void TestThis()
    {
        var str = "[{\"Id\":28,\"AuthorId\":1,\"Name\":\"test a\",\"Host\":\"https://aquarium-api-dev.azurewebsites.net/v1\",\"Author\":null,\"Tasks\":[{\"Id\":1,\"TaskId\":1,\"ScheduleId\":28,\"StartTime\":\"0001-01-01T00:00:00\",\"EndTime\":null,\"Interval\":60,\"Schedule\":null}],\"ScheduleAssignments\":[{\"Id\":8,\"DeviceId\":25,\"ScheduleId\":28,\"Deployed\":false,\"Schedule\":null,\"Device\":null}],\"Running\":false},{\"Id\":29,\"AuthorId\":1,\"Name\":\"test b\",\"Host\":\"https://aquarium-api-dev.azurewebsites.net/v1\",\"Author\":null,\"Tasks\":[{\"Id\":2,\"TaskId\":1,\"ScheduleId\":29,\"StartTime\":\"0001-01-01T00:00:00\",\"EndTime\":null,\"Interval\":3,\"Schedule\":null}],\"ScheduleAssignments\":[{\"Id\":7,\"DeviceId\":25,\"ScheduleId\":29,\"Deployed\":false,\"Schedule\":null,\"Device\":null}],\"Running\":false}]";
        var date = Convert.ToDateTime("01/03/2020 07:46:00");
        var schedules = JsonConvert.DeserializeObject<List<DeviceSchedule>>(str);

        var allScheduledTasks = schedules.SelectMany(s => s.ExpandTasks());

        var futureTask = _scheduleService.GetNextTask(schedules, date);

        Assert.Equal(24, allScheduledTasks.Count());
        
    }
}