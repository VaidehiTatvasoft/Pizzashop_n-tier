using pizzashop.Services.Interfaces;
using System.Security.Claims;
using Entity.Data;
using Entity.ViewModel;
using Pizzashop.Repository.Interfaces;

namespace Pizzashop.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> AddUserAsync(UserViewModel model, ClaimsPrincipal userClaims)
        {
            var userIdClaim = userClaims.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return false;
            }

            var userId = int.Parse(userIdClaim.Value);
            var newUser = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Phone = model.Phone,
                CountryId = model.CountryId,
                StateId = model.StateId,
                CityId = model.CityId,
                Address = model.Address,
                Zipcode = model.Zipcode,
                RoleId = model.RoleId,
                ProfileImage = model.ProfileImage,
                Email = model.Email,
                PasswordHash = model.PasswordHash,
                CreatedBy = userId
            };

            return await _userRepository.AddUserAsync(newUser);
        }

        public async Task<UserViewModel> GetUserViewModelByIdAsync(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
            {
                return null;
            }

            return new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Phone = user.Phone,
                CountryId = user.CountryId,
                StateId = user.StateId,
                CityId = user.CityId,
                Address = user.Address,
                Zipcode = user.Zipcode,
                RoleId = user.RoleId,
                Status = user.IsDeleted.HasValue && user.IsDeleted.Value ? "Inactive" : "Active",
                Email = user.Email,
                ProfileImage = user.ProfileImage
            };
        }

        public async Task<bool> EditUserAsync(UserViewModel model)
        {
            var user = await _userRepository.GetUserById(model.Id);
            if (user == null)
            {
                return false;
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Username = model.Username;
            user.Phone = model.Phone;
            user.CountryId = model.CountryId;
            user.StateId = model.StateId;
            user.CityId = model.CityId;
            user.Address = model.Address;
            user.Zipcode = model.Zipcode;
            user.RoleId = model.RoleId;
            user.IsDeleted = model.Status == "Inactive";
            user.ProfileImage = model.ProfileImage;

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> UpdateProfileAsync(UserViewModel model, ClaimsPrincipal userClaims)
        {
            var userEmailClaim = userClaims.FindFirst(ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return false;
            }

            string userEmail = userEmailClaim.Value;
            var user = await _userRepository.GetUserByEmail(userEmail);
            if (user == null)
            {
                return false;
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Username = model.Username;
            user.Phone = model.Phone;
            user.CountryId = model.CountryId;
            user.StateId = model.StateId;
            user.CityId = model.CityId;
            user.Address = model.Address;
            user.Zipcode = model.Zipcode;
            user.ProfileImage = model.ProfileImage;

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

       public IEnumerable<User> GetUsersList(string searchString, string sortOrder, int pageIndex, int pageSize, out int count)
    {
        return _userRepository.GetUserList(searchString, sortOrder, pageIndex, pageSize, out count);
    }

        public async Task<UserViewModel> GetUserProfileAsync(ClaimsPrincipal userClaims)
    {
        var userEmailClaim = userClaims.FindFirst(ClaimTypes.Email);
        if (userEmailClaim == null)
        {
            return null;
        }

        string userEmail = userEmailClaim.Value;
        var user = await _userRepository.GetUserByEmail(userEmail);
        if (user == null)
        {
            return null;
        }
        var roleName = await _userRepository.GetRoleNameById(user.RoleId);

        return new UserViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Phone = user.Phone,
            RoleId = user.RoleId,
            RoleName = roleName, 
            Email = user.Email,
            CountryId = user.CountryId,
            StateId = user.StateId,
            CityId = user.CityId,
            Zipcode = user.Zipcode,
            Address = user.Address
        };
    }

        public async Task<bool> ChangePasswordAsync(ChangePasswordModel model, ClaimsPrincipal userClaims)
        {
            var userEmailClaim = userClaims.FindFirst(ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return false;
            }

            string email = userEmailClaim.Value;
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return false;
            }

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            return await _userRepository.UpdateUserAsync(user);
        }
    }
}