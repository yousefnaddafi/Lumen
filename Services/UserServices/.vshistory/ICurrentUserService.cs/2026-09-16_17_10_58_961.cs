namespace Lumen.Services.UserServices
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        IEnumerable<long> BranchIds { get; }
    }
}
