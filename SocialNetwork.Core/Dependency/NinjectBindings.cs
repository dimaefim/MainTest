using System.Web;
using Ninject;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.Implementation;
using SocialNetwork.Core.Repository;

namespace SocialNetwork.Core.Dependency
{
    public static class NinjectBindings
    {
        private static readonly IKernel NinjectKernel;

        static NinjectBindings()
        {
            NinjectKernel = new StandardKernel();
            AddBindings();
        }

        private static void AddBindings()
        {
            NinjectKernel.Bind<SocialNetworkContext>().ToSelf().InScope(t => HttpContext.Current);
            NinjectKernel.Bind<IRepository<UserEntity>>().To<UserRepository>();
            NinjectKernel.Bind<IRepository<RoleEntity>>().To<RoleRepository>();
            NinjectKernel.Bind<IUsersRepository>().To<UsersRepository>();
        }

        public static IKernel Instance => NinjectKernel;
    }
}