using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.Core.Repository
{
    public class FriendRepository
    {
        protected readonly SocialNetworkContext _context;

        public FriendRepository(SocialNetworkContext context)
        {
            _context = context;
        }
    }
}
