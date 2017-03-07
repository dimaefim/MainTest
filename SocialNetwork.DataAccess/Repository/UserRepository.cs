using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.DataAccess.Repository
{
    public class UserRepository : IRepository<UserEntity>
    {
        private readonly SocialNetworkContext _context;

        public UserRepository(SocialNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserEntity>> GetAllItems()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserEntity> GetItemById(int id = 0)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task CreateNewItem(UserEntity newUser)
        {
            if (newUser != null)
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItem(UserEntity newUser)
        {
            var updateUser = await _context.Users.FindAsync(newUser.Id);

            if (updateUser != null)
            {
                _context.Entry(newUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemById(int id = 0)
        {
            var deleteUser = await _context.Users.FindAsync(id);

            if (deleteUser != null)
            {
                _context.Users.Remove(deleteUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllItems()
        {
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();
        }
    }
}
