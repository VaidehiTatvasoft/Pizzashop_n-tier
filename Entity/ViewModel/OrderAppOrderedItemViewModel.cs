namespace Entity.ViewModel;

public class OrderAppOrderedItemViewModel
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public string Instruction { get; set; }
    public List<string> Modifiers { get; set; }
}
