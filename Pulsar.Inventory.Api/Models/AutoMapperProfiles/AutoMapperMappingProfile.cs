using AutoMapper;
using Pulsar.Inventory.Api.Models.InventoryItems;

namespace Pulsar.Inventory.Api.Models.AutoMapperProfiles
{
    /// <summary>
    ///     Add Automapper Mappings that will auto load during startup
    /// </summary>
    public class AutoMapperMappingProfile : Profile
    {
        public AutoMapperMappingProfile()
        {
            CreateMap<InventoryItem, InventoryItemViewModel>().ReverseMap();

        }
    }
}