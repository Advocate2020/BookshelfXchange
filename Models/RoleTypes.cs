namespace BookShelfXChange.Models
{
    public class RoleTypes
    {
        public static Role ADMIN => new()
        {
            Id = 1,
            Name = "Admin",
            Description = "Administrator role"
        };

        public static Role USER => new()
        {
            Id = 2,
            Name = "User",
            Description = "Regular user role"
        };
    }
}
