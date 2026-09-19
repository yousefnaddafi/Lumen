namespace Lumen.Services.CommonServices
{
    public interface IHasCreatedAt
    {
        DateTime CreatedAt { get; set; }
    }

    public interface IHasCreatedBy
    {
        string? CreatedByUserId { get; set; }
    }
}
