using SocialNetwork.Models.Enums;

namespace SocialNetwork.Models.Models
{
    public class UsersViewModel : MainPageViewModel
    {
        public int Id { get; set; }
        public FriendStatusEnum Status { get; set; }
    }
}