using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;

namespace Service.Interface;

public interface ITaxAndFeeService
{
    Task<List<TaxesAndFee>> GetAllTaxes();

    Task<bool> AddTax(TaxandFeeViewModel model, ClaimsPrincipal userClaims);

    Task<TaxandFeeViewModel> GetTaxById(int id);

    Task<bool> UpdateTax(TaxandFeeViewModel model, ClaimsPrincipal userClaims);

    Task<bool> DeleteTax(int id, ClaimsPrincipal userClaims);
}
