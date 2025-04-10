using Entity.ViewModel;
using Repository.Interface;
using Repository.Interfaces;
using Service.Interface;

namespace Service.Implementation;

public class MenuModifierService : IMenuModifierService
{
    private readonly IMenuModifierGroupRepository _menuModifierGroupRepository;
    private readonly IMenuModifierRepository _menuModifierRepository;
    private readonly IUnitRepository _unitRepository;

    public MenuModifierService(IMenuModifierGroupRepository menuModifierGroupRepository, IMenuModifierRepository menuModifierRepository, IUnitRepository unitRepository)
    {
        _menuModifierGroupRepository = menuModifierGroupRepository ?? throw new ArgumentNullException(nameof(menuModifierGroupRepository));
        _menuModifierRepository = menuModifierRepository ?? throw new ArgumentNullException(nameof(menuModifierRepository));
        _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
    }

    public async Task<List<MenuModifierGroupViewModel>> GetAllMenuModifierGroupAsync()
    {
        var modifierGroups = _menuModifierGroupRepository.GetAllMenuModifierGroupsAsync();
        return modifierGroups;
    }

    public List<MenuModifierViewModel> GetModifiersByGroupId(int id)
    {
        var modifiers = _menuModifierRepository.GetModifiersByGroupId(id);
        var modifierList = modifiers.Select(i => new MenuModifierViewModel
        {
            Name = i.Name,
            Rate = i.Rate,
        }).ToList();
        return modifierList;
    }

    public async Task<List<MenuModifierViewModel>> GetModifiersByModifierGroup(int id, int pageSize, int pageIndex, string? searchString)
    {
        var modifierList = await _menuModifierRepository.GetModifiersByModifierGroupAsync(id, pageSize, pageIndex, searchString);
        var filteredModifiers = modifierList
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList().ToList();
        // var filteredModifiers = modifierList.Select(c => new MenuModifierViewModel
        // {
        //     Id = c.Id,
        //     UnitName = c.Unit.ShortName,
        //     ModifierGroupId = c.ModifierGroupId,
        //     Name = c.Name,
        //     Description = c.Description,
        //     Rate = c.Rate,
        //     Quantity = c.Quantity,
        // }).ToList();
        return filteredModifiers;
    }

    public async Task<ModifierTabViewModel> GetModifierTabDetails(int ModifierGroupId, int pageSize, int pageIndex, string? searchString)
    {
        var modifierGroups = _menuModifierGroupRepository.GetAllMenuModifierGroupsAsync();

        var modifierList = await _menuModifierRepository.GetModifiersByModifierGroupAsync(ModifierGroupId, pageSize, pageIndex, searchString);
        var filteredModifiers = modifierList
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToList().ToList();
        // var filteredModifiers = modifierList.Select(c => new MenuModifierViewModel
        // {
        //     Id = c.Id,
        //     UnitName = c.Unit.ShortName,
        //     ModifierGroupId = c.ModifierGroupId,
        //     Name = c.Name,
        //     Description = c.Description,
        //     Rate = c.Rate,
        //     Quantity = c.Quantity,
        // }).ToList();
        var totalModifiers = _menuModifierRepository.GetModifierCountByMId(ModifierGroupId, searchString!);

        var modifierTabViewModel = new ModifierTabViewModel
        {
            modifierGroup = modifierGroups,
            modifier = filteredModifiers,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPage = (int)Math.Ceiling(totalModifiers / (double)pageSize),
            SearchString = searchString,
            TotalItems = totalModifiers
        };
        return modifierTabViewModel;
    }

    public int GetModifiersCountByCId(int mId, string? searchString)
    {
        return _menuModifierRepository.GetModifierCountByMId(mId, searchString!);
    }

    public bool AddModifier(AddEditModifierViewModel model, int userId)
    {
        int isExist = _menuModifierRepository.isModifierExist(model.Name, model.Modifiergroupid);
        if (isExist > 0)
            return false;
        _menuModifierRepository.AddModifier(model, userId);
        return true;
    }

    public bool EditModifier(AddEditModifierViewModel model, int userId)
    {
        bool isExist = _menuModifierRepository.isEditModifierExist(model.Name, model.Id, model.Modifiergroupid);
        if (isExist)
            return false;
        _menuModifierRepository.EditModifier(model, userId);
        return true;
    }

    public void DeleteModifier(int id)
    {
        _menuModifierRepository.DeleteModifier(id);
    }

    public void DeleteMultipleModifiers(int[] modifierIds)
{
    _menuModifierRepository.DeleteMultipleModifiers(modifierIds); 
}
    public AddEditModifierViewModel GetModifierByid(int id)
    {
        var modifier = _menuModifierRepository.GetModifierById(id);
        var modifierGroups = _menuModifierGroupRepository.GetAllMenuModifierGroupsAsync();
        var units = _unitRepository.GetAllUnits();

        var editModifier = new AddEditModifierViewModel
        {
            Id = modifier!.Id,
            Name = modifier.Name,
            Rate = modifier.Rate,
            Quantity = modifier.Quantity,
            Description = modifier.Description,
            Modifiergroupid = (int)modifier.ModifierGroupId,
            UnitId = modifier.UnitId,
            ModifierGroups = modifierGroups,
            Units = units
        };
        return editModifier;
    }

    public async Task<AddEditExistingModifiersViewModel> GetAllModifiers(int pageSize = 5, int pageIndex = 1, string? searchString = "")
    {
        var modifiers = await _menuModifierRepository.GetAllModifiers(searchString);
        // var filteredModifier = modifiers.Skip((pageIndex - 1) * pageSize)
        //                                 .Take(pageSize)
        //                                 .ToList();

        var addEditModifierViewModel = new AddEditExistingModifiersViewModel
        {
            modifier = modifiers,
            PageSize = pageSize,
            PageIndex = pageIndex,
            TotalPage = (int)Math.Ceiling(modifiers.Count() / (double)pageSize),
            SearchString = searchString,
            TotalItems = modifiers.Count()
        };
        return addEditModifierViewModel;
    }
    public async Task<int> AddModifierGroupAsync(MenuModifierGroupViewModel model, int userId)
    {
        return await _menuModifierGroupRepository.AddModifierGroupAsync(model, userId);
    }

    public async Task AddModifiersToGroupAsync(int modifierGroupId, List<int> modifierIds)
    {
        await _menuModifierRepository.AddModifiersToGroupAsync(modifierGroupId, modifierIds);
    }
    public async Task EditModifierGroupAsync(MenuModifierGroupViewModel model, int userId)
    {
        await _menuModifierGroupRepository.EditModifierGroupAsync(model, userId);
    }
    public async Task<MenuModifierGroupViewModel> GetModifierGroupByIdAsync(int id)
    {
        return await _menuModifierGroupRepository.GetModifierGroupByIdAsync(id);
    }
    public async Task UpdateModifiersInGroupAsync(int modifierGroupId, List<int> modifierIds)
    {
        await _menuModifierRepository.UpdateModifiersInGroupAsync(modifierGroupId, modifierIds);
    }
    public async Task DeleteModifierGroupAsync(int modifierGroupId)
    {
        await _menuModifierGroupRepository.DeleteModifierGroupAsync(modifierGroupId);
    }
}
