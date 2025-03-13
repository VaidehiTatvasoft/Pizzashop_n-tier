namespace Entity.ViewModel;

public class ModifierGroupViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int MinSelectionRequired { get; set; }
        public int MaxSelectionAllowed { get; set; }
        public List<ModifierViewModel>? Modifiers { get; set; }
    }
