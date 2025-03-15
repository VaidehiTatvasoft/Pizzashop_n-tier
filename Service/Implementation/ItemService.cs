using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Service.Implementation
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<ItemViewModel>> GetAllItems()
        {
            var items = await _itemRepository.GetAllItems();
            return items.Select(item => new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                IsAvailable = item.IsAvailable
            });
        }

        public async Task<ItemViewModel> GetItemById(int id)
        {
            var item = await _itemRepository.GetItemById(id);
            if (item == null) return null;

            return new ItemViewModel
            {
                Id = item.Id,
                Name = item.Name,
                Type = item.Type,
                Rate = item.Rate,
                Quantity = item.Quantity,
                IsAvailable = item.IsAvailable
            };
        }

        public async Task AddItem(ItemViewModel model)
        {
            var item = new MenuItem
            {
                Name = model.Name,
                Type = model.Type,
                Rate = model.Rate,
                Quantity = model.Quantity,
                IsAvailable = model.IsAvailable
            };

            await _itemRepository.AddItem(item);
        }

        public async Task UpdateItem(ItemViewModel model)
        {
            var item = new MenuItem
            {
                Id = model.Id,
                Name = model.Name,
                Type = model.Type,
                Rate = model.Rate,
                Quantity = model.Quantity,
                IsAvailable = model.IsAvailable
            };

            await _itemRepository.UpdateItem(item);
        }

        public async Task DeleteItem(int id)
        {
            await _itemRepository.DeleteItem(id);
        }
    }
}
