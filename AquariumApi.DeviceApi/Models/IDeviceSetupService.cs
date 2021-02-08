using AquariumApi.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AquariumApi.DeviceApi
{
    public interface IDeviceSetupService
    {
        void Setup();
        void CleanUp();
    }
}














