public class MappedModifierViewModel
    {
        public int Id { get; set; }

    public string Name { get; set; } 

    public int? MinSelectionRequired { get; set; }

    public int? MaxSelectionAllowed { get; set; }

    public string? Description { get; set; }

    public bool? IsDeleted { get; set; }
    }