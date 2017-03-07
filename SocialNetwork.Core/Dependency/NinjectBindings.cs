using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Ninject;
using Ninject.Syntax;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Repository;
using System.Web.Http.Dependencies;

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
            NinjectKernel.Bind<IRepository<UserEntity>>().To<UserRepository>();
            NinjectKernel.Bind<IRepository<RoleEntity>>().To<RoleRepository>();
        }

        public static IKernel Instance => NinjectKernel;
    }

    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            _kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(_kernel.BeginBlock());
        }
    }

    public class NinjectDependencyScope : IDependencyScope
    {
        private IResolutionRoot _resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            Contract.Assert(resolver != null);
            _resolver = resolver;
        }

        public void Dispose()
        {
            var disposable = _resolver as IDisposable;
            disposable?.Dispose();
            _resolver = null;
        }

        public object GetService(Type serviceType)
        {
            if (_resolver == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            return _resolver.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (_resolver == null)
                throw new ObjectDisposedException("this", "This scope has already been disposed");
            return _resolver.GetAll(serviceType);
        }
    }
}
