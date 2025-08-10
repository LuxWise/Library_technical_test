namespace Library.Services.Auth.Users
{
    public interface ICurrentUserService
    {
        Guid? GetCurrentUserId();
        string? GetUserEmail();
        string? GetUserName();
    }    
}
