using Entity.ViewModel;

namespace Service.Interface;

public interface ICustomerService
{
        IEnumerable<CustomerViewModel> GetFilteredCustomerViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, DateTime? startDate, DateTime? endDate, out int totalItems);
}
