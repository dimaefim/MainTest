using System;
using System.Threading.Tasks;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.DataAccess.Repository
{
    public class UnitOfWork : IDisposable
    {
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private readonly SocialNetworkContext _context = new SocialNetworkContext();
        private IRepository<UserEntity> _userRepository;
        private IRepository<RoleEntity> _roleRepository;
        private bool _disposed;

        public IRepository<UserEntity> Users => _userRepository ?? (_userRepository = new UserRepository(_context));

        public IRepository<RoleEntity> Roles => _roleRepository ?? (_roleRepository = new RoleRepository(_context));
    }
}
