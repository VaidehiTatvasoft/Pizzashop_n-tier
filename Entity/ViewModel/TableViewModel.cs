namespace Entity.ViewModel;

public class TableViewModel
{

     public int Id { get; set; }
        public int SectionId { get; set; }
        public string Name { get; set; } = null!;
        public int? Capacity { get; set; }
        public bool IsAvailable { get; set; }
}

