using AquariumApi.Models;
using Bifrost.IO.Ports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace AquariumApi.DeviceApi
{
    public interface ISerialService
    {
        int GetTemperatureC();

        bool CanRetrieveTemperature();
        bool CanRetrievePh();
        bool CanRetrieveNitrite();
        bool CanRetrieveNitrate();
        decimal GetNitrate();
        decimal GetNitrite();
        decimal GetPh();
    }
    public class SerialService : ISerialService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<SerialService> _logger;

        public static int? TemperatureC;

        public SerialService(IConfiguration config, ILogger<SerialService> logger)
        {
            _config = config;
            _logger = logger;

            try
            {
                //Test ports
                var ports = SerialPort.GetPortNames();
                foreach (var port in ports)
                {
                    Console.WriteLine($"Serial port name: {port}");
                }

                var serialPort = new SerialPort()
                {
                    PortName = "/dev/ttyACM0",
                    BaudRate = 9600
                };

                // Subscribe to the DataReceived event.
                // Now open the port.
                serialPort.Open();
                serialPort.DataReceived += SerialPort_DataReceived;
            }
            catch(Exception ex)
            {
                _logger.LogWarning("No serial ports detected");
            }
        }
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;

            // Read the data that's in the serial buffer.
            var serialdata = serialPort.ReadExisting();

            // Write to debug output.
            //Console.Write(serialdata);

            TemperatureC = Convert.ToInt16(serialdata);
        }

   

        public bool CanRetrieveTemperature()
        {
            return TemperatureC.HasValue;
        }

        public bool CanRetrievePh()
        {
            return false;
        }

        public bool CanRetrieveNitrite()
        {
            return false;
        }

        public bool CanRetrieveNitrate()
        {
            return false;
        }


        public int GetTemperatureC()
        {
            //Error?
            if(TemperatureC.HasValue)
                return TemperatureC.Value;
            return 0;
        }
        public decimal GetNitrate()
        {
            throw new NotImplementedException();
        }

        public decimal GetNitrite()
        {
            throw new NotImplementedException();
        }

        public decimal GetPh()
        {
            throw new NotImplementedException();
        }
    }
}
