namespace RMS.ServiceBusContracts.Store
{
    public record StoreProductCreatedEvent(
        Guid ProductId,
        string Store,
        string Name,
        string Description,
        decimal Price,
        decimal MinPrice,
        DateTime ModifiedOn);
}
