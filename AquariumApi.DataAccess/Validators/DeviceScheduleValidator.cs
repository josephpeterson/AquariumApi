using AquariumApi.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace AquariumApi.DataAccess.Validators
{
    public class DeviceScheduleValidator : AbstractValidator<DeviceSchedule>
    {
        public DeviceScheduleValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("The schedule must contain a valid name");
            RuleFor(x => x.Tasks).NotEmpty().WithMessage("The schedule must have atleast one task assigned to it");
        }
    }
}
