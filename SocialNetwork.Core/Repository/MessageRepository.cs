using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.Core.Repository
{
    public class MessageRepository
    {
        protected readonly SocialNetworkContext _context;

        public MessageRepository(SocialNetworkContext context)
        {
            _context = context;
        }
    }
}
