using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialNetwork.DataAccess.Repository
{
    public interface IRepository<T> where T : class 
    {
        Task<IEnumerable<T>> GetAllItems();
        Task<T> GetItemById(int id);
        Task CreateNewItem(T newItem);
        Task UpdateItem(T newItem);
        Task DeleteItemById(int id);
        Task DeleteAllItems();
    }
}