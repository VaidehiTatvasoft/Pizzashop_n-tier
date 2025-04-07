using System;
using System.Collections.Generic;
using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;

        }

        public IEnumerable<CustomerViewModel> GetFilteredCustomerViewModels(string searchTerm, string sortOrder, int pageIndex, int pageSize, DateTime? startDate, DateTime? endDate, out int totalItems)
        {
            return _customerRepository.GetFilteredCustomers(searchTerm, sortOrder, pageIndex, pageSize, startDate, endDate, out totalItems);
        }
        public CustomerDetailViewModal? GetCustomerViewModelById(int id)
        {
            return _customerRepository.GetCustomerViewModelById(id);
        }
    }
}