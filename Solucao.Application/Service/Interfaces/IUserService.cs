using Solucao.Application.Contracts;
using Solucao.Application.Contracts.Requests;
using Solucao.Application.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solucao.Application.Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserViewModel>> GetAll(bool isDriver);

        Task<UserViewModel> GetById(Guid Id);

        Task<UserViewModel> GetByName(string Name);

        Task<UserViewModel> GetByToken(string Token);

        Task<ValidationResult> Add(User user, Guid loggedUserId);

        Task<ValidationResult> Update(User user, Guid id, Guid loggedUserId);

        Task<ValidationResult> ChangeUserPassword(UserViewModel user, string newPassword, Guid loggedUserId);

        Task<UserViewModel> Authenticate(string email, string password);

        Task<UserViewModel> GetByEmail(string email);
    }
}
