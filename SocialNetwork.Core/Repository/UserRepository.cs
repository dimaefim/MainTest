using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.Core.Repository
{
    public class UserRepository : IRepository<UserEntity>
    {
        private readonly SocialNetworkContext _context;

        public UserRepository(SocialNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserEntity>> GetAllItemsAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserEntity> GetItemByIdAsync(int id = 0)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task CreateNewItemAsync(UserEntity newUser)
        {
            if (newUser != null)
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItemAsync(UserEntity newUser)
        {
            var updateUser = await _context.Users.FindAsync(newUser.Id);

            if (updateUser != null)
            {
                _context.Entry(newUser).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemByIdAsync(int id = 0)
        {
            var deleteUser = await _context.Users.FindAsync(id);

            if (deleteUser != null)
            {
                _context.Users.Remove(deleteUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllItemsAsync()
        {
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<UserEntity> GetAllItems()
        {
            return _context.Users.ToList();
        }

        public UserEntity GetItemById(int id = 0)
        {
            return _context.Users.Find(id);
        }

        public void CreateNewItem(UserEntity newUser)
        {
            if (newUser != null)
            {
                _context.Users.Add(newUser);
                _context.SaveChanges();
            }
        }

        public void UpdateItem(UserEntity newUser)
        {
            var updateUser = _context.Users.Find(newUser.Id);

            if (updateUser != null)
            {
                _context.Entry(newUser).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void DeleteItemById(int id = 0)
        {
            var deleteUser = _context.Users.Find(id);

            if (deleteUser != null)
            {
                _context.Users.Remove(deleteUser);
                _context.SaveChanges();
            }
        }

        public void DeleteAllItems()
        {
            _context.Users.RemoveRange(_context.Users);
            _context.SaveChanges();
        }
    }
}
