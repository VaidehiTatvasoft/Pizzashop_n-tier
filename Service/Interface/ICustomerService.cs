using Entity.ViewModel;

namespace Service.Interface;

public interface ICustomerService
{
        CustomerDetailViewModal? GetCustomerViewModelById(int id);
        IEnumerable<CustomerViewModel> GetFilteredCustomerViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, DateTime? startDate, DateTime? endDate, out int totalItems);
}
