using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class DeviceException: BaseException
    {
        
        public DeviceException(string message = null): base(message)
        {
            Type = ExceptionTypes.Device;
        }
    }
}