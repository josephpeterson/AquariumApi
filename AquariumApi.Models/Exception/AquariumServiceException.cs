using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public class AquariumServiceException: BaseException
    {
        
        public AquariumServiceException(string message = null): base(message)
        {
            Type = ExceptionTypes.AquariumService;
        }
    }
}