using AutoMapper;
using Restaurant.Infrastructure.Models;
using Restaurant.Infrastructure.ViewModels;
using RestaurantAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ViewCartItem, Item>()
                .ForMember(dest => dest.ItemId, source => source.MapFrom(source => source.Id));

        }
    }
}
