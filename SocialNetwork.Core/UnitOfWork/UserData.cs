using SocialNetwork.Core.Repository;

namespace SocialNetwork.Core.UnitOfWork
{
    public static class UserData
    {
        static UserData()
        {
            db = new SocialNetworkUW();
        }

        public static SocialNetworkUW db;
    }
}
