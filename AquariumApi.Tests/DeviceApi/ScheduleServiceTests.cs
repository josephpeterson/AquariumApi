using AquariumApi.Core;
using AquariumApi.DataAccess;
using AquariumApi.DataAccess.AutoMapper;
using AquariumApi.DeviceApi;
using AquariumApi.DeviceApi.Clients;
using AquariumApi.DeviceApi.Providers;
using AquariumApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AquariumDeviceApiTests
{
    public class AquariumDeviceScheduleServiceTests
    {
        IMapper _mapper;
        private Mock<IAquariumAuthService> _aquariumAuthService;
        private Mock<IAquariumClient> _aquariumClient;
        private Mock<DateTimeProvider> _dateTimeProvider;
        private ExceptionService _exceptionService;
        private GpioService _gpioService;
        private ScheduleService _scheduleService;
        private IConfigurationRoot _config;
        private Aquarium _aquarium;

        public AquariumDeviceScheduleServiceTests()
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

            //Set up Hosting Environment
            var mockEnvironment = new Mock<IHostingEnvironment>();
            //...Setup the mock as needed
            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns("Development");

            //Set up DateTimeProvider
            _dateTimeProvider = new Mock<DateTimeProvider>();
            _dateTimeProvider.Setup(m => m.Now)
                .Returns(DateTime.Parse("1/10/2021"));


            SetupAquariumDevice();
            SetupAquariumAuthService();
            SetupAquariumClient();

            _exceptionService = new ExceptionService(_config, Mock.Of<ILogger<ExceptionService>>(), _aquariumClient.Object);
            _gpioService = new GpioService(_config, Mock.Of<ILogger<GpioService>>(), null, mockEnvironment.Object);
            _scheduleService = new ScheduleService(_config, Mock.Of<ILogger<ScheduleService>>(), _dateTimeProvider.Object,_aquariumAuthService.Object, _aquariumClient.Object, _exceptionService, _gpioService);

        }
        private void SetupAquariumDevice()
        {
            _aquarium = new Aquarium()
            {
                Name = "Test Aquarium",
                StartDate = DateTime.Now.Subtract(TimeSpan.FromDays(200)),
                Device = new AquariumDevice()
                {
                    Sensors = new Collection<DeviceSensor>()
                {
                    new DeviceSensor()
                    {
                        Id = 10,
                        Name = "ATO Pump",
                        Pin = 10,
                        AlwaysOn = false,
                        Polarity = Polarity.Output,
                        Type = SensorTypes.ATOPumpRelay
                    },
                    new DeviceSensor()
                    {
                        Id = 13,
                        Name = "Sump Water Float Switch",
                        Pin = 13,
                        AlwaysOn = false,
                        Polarity = Polarity.Input,
                        Type = SensorTypes.FloatSwitch
                    },
                    new DeviceSensor()
                    {
                        Id = 14,
                        Name = "Water Change Drain",
                        Pin = 14,
                        AlwaysOn = false,
                        Polarity = Polarity.Output,
                        Type = SensorTypes.Solenoid
                    },
                    new DeviceSensor()
                    {
                        Id = 15,
                        Name = "Water Change Replentish Pump",
                        Pin = 15,
                        AlwaysOn = true,
                        Polarity = Polarity.Output,
                        Type = SensorTypes.ATOPumpRelay
                    },
                },
                    Tasks = new Collection<DeviceScheduleTask>()
                {
                    new DeviceScheduleTask()
                    {
                        Name = "Auto Top Off Task",
                        Id = 123,
                        MaximumRuntime = 30,
                        TargetSensorId = 10, //ATO Pump
                        TargetSensorValue = GpioPinValue.High,
                        TriggerSensorId = 13, //Float Switch
                        TriggerSensorValue = GpioPinValue.High,
                        DateConditions = null, //Run every day
                    },
                    new DeviceScheduleTask()
                    {
                        Name = "Water Change Drain",
                        Id = 234,
                        MaximumRuntime = 30,
                        TargetSensorId = 14, //Water change solenoid
                        TriggerSensorId = 13, //Float Switch
                        TriggerSensorValue = GpioPinValue.High,
                        DateConditions = "0100000", //Run every monday
                    },
                    new DeviceScheduleTask()
                    {
                        Name = "Water Change Replentish",
                        Id = 345,
                        MaximumRuntime = 30,
                        TargetSensorId = 15, //Water change replentish pump
                        TargetSensorValue = GpioPinValue.High,
                        TriggerSensorId = 13, //Float Switch
                        TriggerSensorValue = GpioPinValue.High,
                        DateConditions = "0100000", //Run every monday
                    }
                }
                }
            };
        }
        private void SetupAquariumDeviceSchedule()
        {
            _aquarium.Device.Schedules = new Collection<DeviceSchedule>()
        {
            new DeviceSchedule()
            {
                Name = "Test Schedule 1",
                TaskAssignments = new Collection<DeviceScheduleTaskAssignment>()
                {
                    new DeviceScheduleTaskAssignment()
                    {
                        Id = 0,
                        StartTime = DateTime.Parse("1/1/2000 8:30"),
                        TaskId = 123,
                        Task = _aquarium.Device.Tasks.First(t => t.Id == 123), //Auto Top Off Task
                        Repeat = false,
                    }
                }
            }
        };
        }
        private void SetupAquariumClient()
        {
            var mockEnvironment = new Mock<IAquariumClient>();
            //...Setup the mock as needed
            mockEnvironment
                .Setup(m => m.PingAquariumService())
                .ReturnsAsync(_aquarium.Device);
            mockEnvironment
                .Setup(m => m.DispatchScheduledJob(It.IsAny<ScheduledJob>()))
                .ReturnsAsync(new ScheduledJob());
            _aquariumClient = mockEnvironment;
        }
        private void SetupAquariumAuthService()
        {
            var mockEnvironment = new Mock<IAquariumAuthService>();
            //...Setup the mock as needed
            mockEnvironment
                .Setup(m => m.GetAquarium())
                .Returns(_aquarium);
            _aquariumAuthService = mockEnvironment;
        }




        [Fact]
        public void GivenScheduleAssignment_AllDailyTasksScheduled()
        {
            SetupAquariumDeviceSchedule();
            _scheduleService.Setup();

            var scheduledJobs = _scheduleService.GetAllScheduledTasks();

            Assert.Equal(7, scheduledJobs.Count());
        }
        [Fact]
        public void GivenScheduleAssignmentWithRepeatingTask_AllDailyTasksScheduled()
        {
            //setup
            SetupAquariumDeviceSchedule();
            var repeatingTask = _aquarium.Device.Schedules.First().TaskAssignments.First();
            repeatingTask.Repeat = true;
            repeatingTask.RepeatInterval = 10;
            repeatingTask.RepeatEndTime = repeatingTask.StartTime.AddHours(2).AddMinutes(34);

            //act
            _scheduleService.Setup();
            var scheduledJobs = _scheduleService.GetAllScheduledTasks();

            //assert
            Assert.Equal(8 * 14, scheduledJobs.Count());
        }
        [Fact]
        public void GivenTimePassed_TaskHasStarted()
        {
            //setup
            SetupAquariumDeviceSchedule();
            _dateTimeProvider
               .Setup(m => m.Now)
               .Returns(DateTime.Parse("1/1/2021 8:20"));
            _gpioService.Setup(_aquarium.Device.Sensors);
            _scheduleService.Setup();

            //act
            _dateTimeProvider
               .Setup(m => m.Now)
               .Returns(DateTime.Parse("1/1/2021 8:30"));
            Thread.Sleep(50);
            var nextTask = _scheduleService.GetNextTask();

            //assert
            var allTasks = _scheduleService.GetAllScheduledTasks();
            var tasks = allTasks.Where(sj => sj.Status == JobStatus.Running);
            Assert.Single(tasks);
        }
        [Fact]
        public void GivenTaskAssginmentRunsOnlyMondays_TaskScheduledOnlyMondays()
        {
            //setup
            SetupAquariumDeviceSchedule();
            var taskAssignment = _aquarium.Device.Schedules.First().TaskAssignments.First();
            var targetTask = _aquarium.Device.Tasks.First(t => t.Id == taskAssignment.TaskId);
            targetTask.DateConditions = "0100000";

            //act
            _scheduleService.Setup();
            var time = _dateTimeProvider.Object.Now;

            //assert
            var allTasks = _scheduleService.GetAllScheduledTasks();
            Assert.Single(allTasks);
            Assert.Equal(DayOfWeek.Monday, allTasks.First().StartTime.Date.DayOfWeek);
        }
        [Fact]
        public void GivenTaskAssginmentRunsOnly15th_TaskScheduledOnly15th()
        {
            //setup
            SetupAquariumDeviceSchedule();
            var taskAssignment = _aquarium.Device.Schedules.First().TaskAssignments.First();
            var targetTask = _aquarium.Device.Tasks.First(t => t.Id == taskAssignment.TaskId);
            targetTask.DateConditions = "15";
            _dateTimeProvider
               .Setup(m => m.Now)
               .Returns(DateTime.Parse("1/13/2020"));

            //act
            _scheduleService.Setup();

            //assert
            var allTasks = _scheduleService.GetAllScheduledTasks();
            Assert.Single(allTasks);
            Assert.Equal(15, allTasks.First().StartTime.Day);
        }
        [Fact]
        public void GivenTaskRuns_RunUntilFloatSwitchReadsHigh()
        {
            //setup
            SetupAquariumDeviceSchedule();
            _scheduleService.Setup();

            //act
            var task = _aquarium.Device.Tasks.First(t => t.Id == 123); //auto top off task
            var job = new ScheduledJob()
            {
                TaskId = task.Id.Value,
                Task = task,
            };
            _scheduleService.GenericPerformTask(job);

            //assert
            var floatSensor = _aquarium.Device.Sensors.First(s => s.Id == task.TriggerSensorId);
            var pumpSensor = _aquarium.Device.Sensors.First(s => s.Id == task.TargetSensorId);
            Assert.Equal(GpioPinValue.Low, floatSensor.Value);
            Assert.Equal(GpioPinValue.High, pumpSensor.Value);

            //act
            Thread.Sleep(100);
            _gpioService.SetPinValue(floatSensor, GpioPinValue.High);
            _gpioService.OnDeviceSensorTriggered(floatSensor, null, null);
            Thread.Sleep(50);
            var allTasks = _scheduleService.GetAllScheduledTasks();
            var tasks = allTasks.Where(sj => sj.Status == JobStatus.Running);
            Assert.Empty(tasks);
            Assert.Equal(GpioPinValue.High, floatSensor.Value);
            Assert.Equal(GpioPinValue.Low, pumpSensor.Value);
        }
        [Fact]
        public void GivenTaskRuns_StopsAfterMaxRuntime()
        {
            //setup
            SetupAquariumDeviceSchedule();
            _scheduleService.Setup();

            //act
            var task = _aquarium.Device.Tasks.First(t => t.Id == 123); //auto top off task
            var job = new ScheduledJob()
            {
                TaskId = task.Id.Value,
                Task = task,
            };
            var runningJob = _scheduleService.GenericPerformTask(job);

            var allTasks = _scheduleService.GetAllScheduledTasks();
            var tasks = allTasks.Where(sj => sj.Status == JobStatus.Running);
            Assert.Single(tasks);



            //act
            _dateTimeProvider
               .Setup(m => m.Now)
               .Returns(runningJob.ScheduledJob.MaximumEndTime.Value);
            Thread.Sleep(100);

            allTasks = _scheduleService.GetAllScheduledTasks();
            tasks = allTasks.Where(sj => sj.Status == JobStatus.Running);
            Assert.Empty(tasks);
        }
        [Fact]
        public void GivenFloatSwitchHigh_TaskWillNotStart()
        {
            //setup
            SetupAquariumDeviceSchedule();
            _scheduleService.Setup();
            Thread.Sleep(100);
            var task = _aquarium.Device.Tasks.First(t => t.Id == 123); //auto top off task
            var floatSensor = _aquarium.Device.Sensors.First(s => s.Id == task.TriggerSensorId);

            //act
            _gpioService.SetPinValue(floatSensor, GpioPinValue.High);
            _gpioService.OnDeviceSensorTriggered(floatSensor, null, null);
            var job = new ScheduledJob()
            {
                TaskId = task.Id.Value,
                Task = task,
            };
            Func<Task> ThrowExceptionFunc = () =>
            {
                var runningJob = _scheduleService.GenericPerformTask(job);
                return runningJob.RunningTask;
            };

            //assert
            Assert.ThrowsAsync<DeviceException>(ThrowExceptionFunc);
            var allTasks = _scheduleService.GetAllScheduledTasks();
            var tasks = allTasks.Where(sj => sj.Status == JobStatus.Running);
            Assert.Empty(tasks);
        }
    }
}
