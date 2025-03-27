namespace Entity.ViewModel;

public class ItemViewModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; }
    public bool? Type { get; set; }
    public decimal Rate { get; set; }
    public int? Quantity { get; set; }
    public int UnitId { get; set; }
    public bool? IsAvailable { get; set; }
    public bool? IsDefaultTax { get; set; }
    public decimal? TaxPercentage { get; set; }
    public string? ShortCode { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public int? ModifierGroupId { get; set; }
    public int? MaxSelectionAllowed { get; set; }
    public int? MinSelectionAllowed { get; set; }
}
