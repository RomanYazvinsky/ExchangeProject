using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exchange.Core.ViewModels;
using Exchange.Data.Constants;

namespace Exchange.Core.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserVm>> GetAllUsersAsync();
        Task<bool> ModifyUserRoleAsync(Guid userId, Role role);
        Task<bool> RemoveUserAsync(Guid userId);
    }
}