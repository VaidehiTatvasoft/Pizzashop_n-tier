using Entity.Data;
using Entity.ViewModel;

namespace Repository.Interface;

public interface ICustomerRepository
{
       CustomerDetailViewModal? GetCustomerViewModelById(int id);
        IEnumerable<CustomerViewModel> GetFilteredCustomers(string searchTerm, string sortOrder, int pageIndex, int pageSize, DateTime? startDate, DateTime? endDate, out int totalItems);
}
