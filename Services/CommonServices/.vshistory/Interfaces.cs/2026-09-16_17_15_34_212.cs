namespace Lumen.Services.CommonServices
{
    public interface IHasCreatedAt
    {
        DateTime CreatedAt { get; set; }
    }

    public interface IHasCreatedBy
    {
        long? CreatedByUserId { get; set; }
    }
}
