using Entity.Data;
using Entity.ViewModel;

namespace Service.Interface;

public interface ITaxAndFeeService
{
    Task<List<TaxesAndFee>> GetAllTaxes();

    Task<bool> AddTax(TaxandFeeViewModel model);

    Task<TaxandFeeViewModel> GetTaxById(int id);

    Task<bool> UpdateTax(TaxandFeeViewModel model);

    Task<bool> DeleteTax(int id);
}
