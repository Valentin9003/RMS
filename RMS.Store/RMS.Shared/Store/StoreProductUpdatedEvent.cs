namespace RMS.ServiceBusContracts.Store
{
    public record StoreProductUpdatedEvent(
        Guid ProductId,
        string Store,
        string Name,
        string Description,
        decimal Price,
        decimal MinPrice,
        DateTime ModifiedOn);
}
