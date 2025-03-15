using Entity.Data;

namespace Repository.Interfaces;

public interface IUnitRepository
{
        Task<List<Unit>> GetAllUnits();
}
