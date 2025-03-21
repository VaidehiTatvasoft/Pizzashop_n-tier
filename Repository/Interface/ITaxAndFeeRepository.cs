using Entity.Data;

namespace Repository.Interface;

public interface ITaxAndFeeRepository
{
    Task<bool> AddTax(TaxesAndFee tax);
    Task<List<TaxesAndFee>> GetAllTaxes();
    Task<TaxesAndFee> GetTaxById(int id);
    Task<TaxesAndFee> GetTaxByName(string name);
    Task<TaxesAndFee> GetTaxByNameExId(string name, int id);
    Task<bool> UpdateTax(TaxesAndFee tax);

}
