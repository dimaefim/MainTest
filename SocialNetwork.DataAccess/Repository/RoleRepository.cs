using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.DataAccess.Repository
{
    public class RoleRepository : IRepository<RoleEntity>
    {

        private readonly SocialNetworkContext _context;

        public RoleRepository(SocialNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleEntity>> GetAllItems()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<RoleEntity> GetItemById(int id = 0)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task CreateNewItem(RoleEntity newRole)
        {
            if (newRole != null)
            {
                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItem(RoleEntity newRole)
        {
            var updateRole = await _context.Roles.FindAsync(newRole.Id);

            if (updateRole != null)
            {
                _context.Entry(newRole).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemById(int id = 0)
        {
            var deleteRole = await _context.Roles.FindAsync(id);

            if (deleteRole != null)
            {
                _context.Roles.Remove(deleteRole);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllItems()
        {
            _context.Roles.RemoveRange(_context.Roles);
            await _context.SaveChangesAsync();
        }
    }
}
