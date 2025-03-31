using Entity.Data;

namespace Entity.ViewModel;

public class ModifierItemListViewModel
{
     public IEnumerable<Modifier> ModifierItems { get; set; }
    public PaginationViewModel Pagination { get; set; }
}
