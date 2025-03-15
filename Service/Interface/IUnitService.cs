using Entity.Data;

namespace Service.Interface;

public interface IUnitService
{
    Task<List<Unit>> GetAllUnits();
}
