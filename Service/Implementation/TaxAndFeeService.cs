using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TaxAndFeeService :ITaxAndFeeService
{
    private readonly ITaxAndFeeRepository _taxAndFeeRepository;

    public TaxAndFeeService(ITaxAndFeeRepository taxAndFeeRepository)
    {
        _taxAndFeeRepository = taxAndFeeRepository;
    }

    public async Task<bool> AddTax(TaxandFeeViewModel model)
    {
        var tax = await _taxAndFeeRepository.GetTaxByName(model.Name);
        if (tax != null)
        {
            return false;
        }

        var newTax = new TaxesAndFee
        {
            Name = model.Name,
            Type = model.Type,
            TaxValue = model.TaxValue,
            IsDefault = model.IsDefault,
            IsActive = model.IsActive,
            CreatedBy = 1,
            CreatedAt = DateTime.Now
        };

        return await _taxAndFeeRepository.AddTax(newTax);
    }

    public async Task<List<TaxesAndFee>> GetAllTaxes()
    {
        return await _taxAndFeeRepository.GetAllTaxes();
    }

    public async Task<TaxandFeeViewModel> GetTaxById(int id)
    {
        var tax = await _taxAndFeeRepository.GetTaxById(id);

        if (tax == null)
        {
            return null;
        }

        var taxViewModel = new TaxandFeeViewModel
        {
            Id = tax.Id,
            Name = tax.Name,
            IsActive = tax.IsActive ?? true,
            IsDefault = tax.IsDefault ?? true,
            Type = tax.Type,
            TaxValue = tax.TaxValue
        };

        return taxViewModel;
    }

    public async Task<bool> UpdateTax(TaxandFeeViewModel model)
    {
        var tax = await _taxAndFeeRepository.GetTaxById(model.Id);

        if (tax != null)
        {
            var taxName = await _taxAndFeeRepository.GetTaxByNameExId(model.Name, model.Id);

            if (taxName == null)
            {
                tax.Name = model.Name;
                tax.IsDefault = model.IsDefault;
                tax.IsActive = model.IsActive;
                tax.Type = model.Type;
                tax.TaxValue = model.TaxValue;

                await _taxAndFeeRepository.UpdateTax(tax);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> DeleteTax(int id)
    {
        var tax = await _taxAndFeeRepository.GetTaxById(id);

        if (tax != null)
        {
            tax.IsDeleted = true;
            await _taxAndFeeRepository.UpdateTax(tax);
            return true;
        }

        return false;
    }
}
