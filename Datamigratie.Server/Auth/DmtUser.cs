namespace Datamigratie.Server.Auth
{
    public class DmtUser
    {
        public bool IsLoggedIn { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string[] Roles { get; set; } = [];
        public bool HasFunctioneelBeheerderAccess { get; set; }
    }
}
