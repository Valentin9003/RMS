namespace RMS.ServiceBusContracts.CentrallApp;

public record CentralAppProductSyncedEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ProductId { get; init; }
    public CentrallAppProductSyncedEventPayload? Payload { get; set; }
    public ProductState State { get; init; }
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

public record CentrallAppProductSyncedEventPayload
{
    public string Store { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public decimal MinPrice { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime ModifiedOn { get; init; }

    public CentrallAppProductSyncedEventPayload(
        string store,
        string name,
        string description,
        decimal price,
        decimal minPrice,
        DateTime createdOn,
        DateTime modifiedOn)
    {
        Store = store;
        Name = name;
        Description = description;
        Price = price;
        MinPrice = minPrice;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public CentrallAppProductSyncedEventPayload()
    {
    }
}

public enum ProductState
{
    Created,
    Updated,
    Deleted
}

