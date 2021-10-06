using AutoMapper;
using HotelListing.Data;
using HotelListing.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Config
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<Country, CreateCountryDTO>().ReverseMap();
            CreateMap<Hotel, HotelDTO>().ReverseMap();
            CreateMap<Hotel, CreateHotelDTO>().ReverseMap();
        }
    }
}
