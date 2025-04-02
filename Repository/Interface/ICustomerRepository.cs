using Entity.ViewModel;

namespace Repository.Interface;

public interface ICustomerRepository
{
        IEnumerable<CustomerViewModel> GetFilteredCustomers(string searchTerm, string sortOrder, int pageIndex, int pageSize, DateTime? startDate, DateTime? endDate, out int totalItems);

}
