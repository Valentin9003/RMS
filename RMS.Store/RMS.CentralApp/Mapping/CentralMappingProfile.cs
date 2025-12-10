using AutoMapper;
using RMS.CentralApp.Models;
using RMS.CentralApp.Infrastructure.Persistence.Entities;
using RMS.ServiceBusContracts.CentrallApp;
using RMS.ServiceBusContracts.Store;

namespace RMS.CentralApp.Mapping
{
    public class CentralMappingProfile : Profile
    {
        public CentralMappingProfile()
        {
            CreateMap<StoreProductCreatedEvent, StoreProduct>();

            CreateMap<ProductRequest, StoreProduct>();

            CreateMap<ProductRequest, CentrallAppProductSyncedEventPayload>()
                .ForMember(p => p.Store, cfg => cfg.Ignore())
                .ForMember(p => p.CreatedOn, cfg => cfg.Ignore());

            CreateMap<ProductRequest, StoreProduct>();

            CreateMap<StoreProduct, CentrallAppProductSyncedEventPayload>()
                .ForMember(p => p.ModifiedOn, x => x.MapFrom(s => s.UpdatedOn));

            CreateMap<StoreProduct, ProductResponse>();

        }
    }
}
