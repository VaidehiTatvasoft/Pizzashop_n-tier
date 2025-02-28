using Entity.Data;

namespace Entity.ViewModel;

public class UserListInfo
{
    public List<User> Users { get; set; }
        public int TotalUsers { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
}
