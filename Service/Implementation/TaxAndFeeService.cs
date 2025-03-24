using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation;

public class TaxAndFeeService : ITaxAndFeeService
{
    private readonly ITaxAndFeeRepository _taxAndFeeRepository;

    public TaxAndFeeService(ITaxAndFeeRepository taxAndFeeRepository)
    {
        _taxAndFeeRepository = taxAndFeeRepository;
    }

   public async Task<bool> AddTax(TaxandFeeViewModel model, ClaimsPrincipal userClaims)
{
    var tax = await _taxAndFeeRepository.GetTaxByName(model.Name);
    if (tax != null)
    {
        return false;
    }
    var userIdClaim = userClaims.FindFirst("UserId");
    if (userIdClaim == null)
    {
        return false;
    }
    var userId = int.Parse(userIdClaim.Value);
    var newTax = new TaxesAndFee
    {
        Name = model.Name,
        Type = model.Type,
        TaxValue = model.TaxValue,
        IsDefault = model.IsDefault,
        IsActive = model.IsActive,
        CreatedBy = userId,
        CreatedAt = DateTime.UtcNow
    };

    bool result = await _taxAndFeeRepository.AddTax(newTax);

    if (result)
    {
        model.Id = newTax.Id; 
    }

    return result;
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
            IsActive = tax.IsActive,
            IsDefault = tax.IsDefault,
            Type = tax.Type,
            TaxValue = tax.TaxValue
        };

        return taxViewModel;
    }

    public async Task<bool> UpdateTax(TaxandFeeViewModel model, ClaimsPrincipal userClaims)
    {
        var tax = await _taxAndFeeRepository.GetTaxById(model.Id);

        if (tax != null)
        {
            var taxName = await _taxAndFeeRepository.GetTaxByNameExId(model.Name, model.Id);
            var userIdClaim = userClaims.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return false;
            }
            var userId = int.Parse(userIdClaim.Value);
            if (taxName == null)
            {
                tax.Name = model.Name;
                tax.IsDefault = model.IsDefault;
                tax.IsActive = model.IsActive;
                tax.Type = model.Type;
                tax.TaxValue = model.TaxValue;
                tax.ModifiedBy = userId;
                tax.ModifiedAt = DateTime.UtcNow;

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

    public async Task<bool> DeleteTax(int id, ClaimsPrincipal userClaims)
    {
        var tax = await _taxAndFeeRepository.GetTaxById(id);
        var userIdClaim = userClaims.FindFirst("UserId");
        if (userIdClaim == null)
        {
            return false;
        }
        var userId = int.Parse(userIdClaim.Value);
        if (tax != null)
        {
            tax.IsDeleted = true;
            tax.ModifiedBy = userId;
            tax.ModifiedAt = DateTime.UtcNow;
            await _taxAndFeeRepository.UpdateTax(tax);
            return true;
        }

        return false;
    }
    public async Task<bool> UpdateTaxStatus(int id, bool isActive, bool isDefault, ClaimsPrincipal userClaims)
{
    var tax = await _taxAndFeeRepository.GetTaxById(id);
    if (tax == null)
    {
        return false;
    }

    var userIdClaim = userClaims.FindFirst("UserId");
    if (userIdClaim == null)
    {
        return false;
    }

    var userId = int.Parse(userIdClaim.Value);
    tax.IsActive = isActive;
    tax.IsDefault = isDefault;
    tax.ModifiedBy = userId;
    tax.ModifiedAt = DateTime.UtcNow;

    await _taxAndFeeRepository.UpdateTax(tax);
    return true;
}
}
