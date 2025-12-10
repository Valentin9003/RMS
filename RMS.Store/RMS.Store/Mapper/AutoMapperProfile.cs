using AutoMapper;
using RMS.ServiceBusContracts.CentrallApp;
using RMS.Store.Models;
using RMS.Store.Persistence.Entities;

namespace RMS.Store.Mapping;

public class StoreMappingProfile : Profile
{
    public StoreMappingProfile()
    {
        CreateMap<StoreProductRequest, StoreProduct>();

        CreateMap<CentrallAppProductSyncedEventPayload, StoreProduct>();
    }
}
