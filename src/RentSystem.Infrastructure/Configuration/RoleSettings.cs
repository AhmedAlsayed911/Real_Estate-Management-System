namespace RentSystem.Infrastructure.Configuration
{
    public class RoleSettings
    {
        public string AllowedRoles { get; set; } = string.Empty;
        public List<string> AllowedRolesList => 
            AllowedRoles.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim().ToLower())
                .ToList();
    }
}
