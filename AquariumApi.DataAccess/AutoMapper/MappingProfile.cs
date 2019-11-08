using AquariumApi.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AquariumApi.DataAccess.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Aquarium, Aquarium>();
            CreateMap<Activity, Activity>();
            CreateMap<AquariumUser, AquariumUser>();
            CreateMap<AquariumProfile, AquariumProfile>()
                .ForMember(c => c.Account, opt => opt.Ignore());
            CreateMap<Fish, Fish>();
            CreateMap<CameraConfiguration, CameraConfiguration>();

            CreateMap<AquariumDevice, AquariumDevice>()
                .ForMember(c => c.CameraConfiguration, opt => opt.Ignore());

            CreateMap<Activity, CreateAquariumTestResultsActivity>();
        }
    }
}
