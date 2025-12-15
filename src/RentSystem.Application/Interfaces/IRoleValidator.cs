namespace RentSystem.Application.Interfaces
{
    public interface IRoleValidator
    {
        bool IsValidRole(string role);
        List<string> GetAllowedRoles();
    }
}
