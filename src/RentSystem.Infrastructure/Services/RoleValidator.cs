using Microsoft.Extensions.Options;
using RentSystem.Application.Interfaces;
using RentSystem.Infrastructure.Configuration;

namespace RentSystem.Infrastructure.Services
{
    public class RoleValidator : IRoleValidator
    {
        private readonly RoleSettings _roleSettings;

        public RoleValidator(IOptions<RoleSettings> roleSettings)
        {
            _roleSettings = roleSettings.Value;
        }

        public bool IsValidRole(string role)
        {
            return _roleSettings.AllowedRolesList.Contains(role.ToLower());
        }

        public List<string> GetAllowedRoles()
        {
            return _roleSettings.AllowedRolesList;
        }
    }
}
