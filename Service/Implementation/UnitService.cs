using Entity.Data;
using Repository.Interfaces;
using Service.Interface;

namespace Service.Implementation;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;

    public UnitService(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    public async Task<List<Unit>> GetAllUnits()
    {
        return await _unitRepository.GetAllUnits();
    }

}
