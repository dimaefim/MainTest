namespace SocialNetwork.DataAccess.DbEntity
{
    public class UsersInRolesEntity
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public virtual UserEntity User { get; set; }
        public virtual RoleEntity Role { get; set; }
    }
}
