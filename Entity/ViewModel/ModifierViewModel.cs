namespace Entity.ViewModel;

public class ModifierViewModel
{
    public int ModifierGroupId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Rate { get; set; }
    public int? Quantity { get; set; }
    public int UnitId { get; set; }
    public string? UnitName { get; set; }
    public string? Description { get; set; }
}
