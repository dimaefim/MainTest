using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.Core.Interfaces
{
    public interface IRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetAllItemsAsync();
        Task<T> GetItemByIdAsync(int id);
        Task CreateNewItemAsync(T newItem);
        Task UpdateItemAsync(T newItem);
        Task DeleteItemByIdAsync(int id);
        Task DeleteAllItemsAsync();

        IEnumerable<T> GetAllItems();
        T GetItemById(int id);
        void CreateNewItem(T newItem);
        void UpdateItem(T newItem);
        void DeleteItemById(int id);
        void DeleteAllItems();
    }
}