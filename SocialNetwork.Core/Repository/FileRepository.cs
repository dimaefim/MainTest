using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialNetwork.Core.Interfaces;
using SocialNetwork.DataAccess.DbEntity;
using SocialNetwork.DataAccess.Implementation;

namespace SocialNetwork.Core.Repository
{
    public class FileRepository : IRepository<FileEntity>
    {
        private readonly SocialNetworkContext _context;

        public FileRepository(SocialNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FileEntity>> GetAllItemsAsync()
        {
            return await _context.Files.ToListAsync();
        }

        public async Task<FileEntity> GetItemByIdAsync(int id = 0)
        {
            return await _context.Files.FindAsync(id);
        }

        public async Task CreateNewItemAsync(FileEntity newFile)
        {
            if (newFile != null)
            {
                _context.Files.Add(newFile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateItemAsync(FileEntity newFile)
        {
            var updateFile = await _context.Files.FindAsync(newFile.Id);

            if (updateFile != null)
            {
                _context.Entry(newFile).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteItemByIdAsync(int id = 0)
        {
            var deleteFile = await _context.Files.FindAsync(id);

            if (deleteFile != null)
            {
                _context.Files.Remove(deleteFile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAllItemsAsync()
        {
            _context.Files.RemoveRange(_context.Files);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<FileEntity> GetAllItems()
        {
            return _context.Files.ToList();
        }

        public FileEntity GetItemById(int id = 0)
        {
            return _context.Files.Find(id);
        }

        public void CreateNewItem(FileEntity newFile)
        {
            if (newFile != null)
            {
                _context.Files.Add(newFile);
                _context.SaveChanges();
            }
        }

        public void UpdateItem(FileEntity newFile)
        {
            var updateFile = _context.Users.Find(newFile.Id);

            if (updateFile != null)
            {
                _context.Entry(newFile).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public void DeleteItemById(int id = 0)
        {
            var deleteFile = _context.Files.Find(id);

            if (deleteFile != null)
            {
                _context.Files.Remove(deleteFile);
                _context.SaveChanges();
            }
        }

        public void DeleteAllItems()
        {
            _context.Files.RemoveRange(_context.Files);
            _context.SaveChanges();
        }
    }
}
