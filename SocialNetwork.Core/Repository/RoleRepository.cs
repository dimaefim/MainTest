using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.Core.Repository
{
    public class RoleRepository : IRepository<RoleEntity>
    {
        private readonly SocialNetworkContext _context;

        public RoleRepository(SocialNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleEntity>> GetAllItemsAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<RoleEntity> GetItemByIdAsync(int id = 0)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task CreateNewItemAsync(RoleEntity newRole)
        {
            if (newRole != null)
            {
                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItemAsync(RoleEntity newRole)
        {
            var updateRole = await _context.Roles.FindAsync(newRole.Id);

            if (updateRole != null)
            {
                _context.Entry(newRole).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemByIdAsync(int id = 0)
        {
            var deleteRole = await _context.Roles.FindAsync(id);

            if (deleteRole != null)
            {
                _context.Roles.Remove(deleteRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllItemsAsync()
        {
            _context.Roles.RemoveRange(_context.Roles);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<RoleEntity> GetAllItems()
        {
            return _context.Roles.ToList();
        }

        public RoleEntity GetItemById(int id = 0)
        {
            return _context.Roles.Find(id);
        }

        public void CreateNewItem(RoleEntity newRole)
        {
            if (newRole != null)
            {
                _context.Roles.Add(newRole);
                _context.SaveChanges();
            }
        }

        public void UpdateItem(RoleEntity newRole)
        {
            var updateRole = _context.Roles.FindAsync(newRole.Id);

            if (updateRole != null)
            {
                _context.Entry(newRole).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void DeleteItemById(int id = 0)
        {
            var deleteRole = _context.Roles.Find(id);

            if (deleteRole != null)
            {
                _context.Roles.Remove(deleteRole);
                _context.SaveChanges();
            }
        }

        public void DeleteAllItems()
        {
            _context.Roles.RemoveRange(_context.Roles);
            _context.SaveChanges();
        }
    }
}