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
            CreateMap<AquariumParameters, tblWaterParameters>();
            CreateMap<Aquarium, Aquarium>();
            CreateMap<CameraConfiguration, CameraConfiguration>();
        }
    }
}
