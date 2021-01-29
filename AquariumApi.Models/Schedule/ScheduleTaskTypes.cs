using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquariumApi.Models
{
    public enum ScheduleTaskTypes
    {
        Unknown = 0,
        Snapshot = 1,
        StartATO = 2,
    }
}