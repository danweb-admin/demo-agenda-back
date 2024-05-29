using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Migrations;
using Solucao.Application.Contracts;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Interfaces;
using Solucao.Application.Utils.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using HistoryRepository = Solucao.Application.Data.Repositories.HistoryRepository;

namespace Solucao.Application.Service.Implementations
{
    public class UserService : IUserService
    {
        private UserRepository userRepository;
        private readonly IMapper mapper;
        private readonly IMD5Service mD5Service;
        private readonly HistoryRepository history;

        public UserService(UserRepository _userRepository, IMapper _mapper, IMD5Service _mD5Service, HistoryRepository _history)
        {
            userRepository = _userRepository;
            mapper = _mapper;
            mD5Service = _mD5Service;
            history = _history;
        }

        public async Task<IEnumerable<UserViewModel>> GetAll(bool isDriver)
        {
            return mapper.Map<IEnumerable<UserViewModel>>(await userRepository.GetAll(isDriver));
        }

        public Task<UserViewModel> GetById(Guid Id)
        {
            return mapper.Map<Task<UserViewModel>>(userRepository.GetById(Id));
        }

        public async Task<ValidationResult> Add(User user, Guid loggedUserId)
        {
            user.CreatedAt = DateTime.Now;
            user.Password = mD5Service.ReturnMD5(user.Password);

            var result = await userRepository.Add(user);

            if (result == null)
                return new ValidationResult("Houve um problema para criar o Usuário");

            // adiciona a tabela de histórico de alteracao
            await history.Add(TableEnum.User, result.Id, OperationEnum.Criacao, loggedUserId);

            return ValidationResult.Success;
        }

        public async Task<ValidationResult> Update(User user, Guid id, Guid loggedUserId)
        {
            var user_ = await userRepository.GetById(id);

            user.UpdatedAt = DateTime.Now;
            user.Password = user_.Password;

            var result = await userRepository.Update(user);

            if (result == null)
                return new ValidationResult("Houve um problema para editar o Usuário");

            // adiciona a tabela de histórico de alteracao
            await history.Add(TableEnum.User, result.Id, OperationEnum.Alteracao, loggedUserId);

            return ValidationResult.Success;
        }

        public async Task<UserViewModel> Authenticate(string email, string password)
        {
            var user = await userRepository.GetByEmail(email);

            if (user != null)
            {
                if (mD5Service.CompareMD5(password, user.Password))
                {
                    await history.Add(TableEnum.User, user.Id, OperationEnum.Logou,user.Id);

                    return mapper.Map<UserViewModel>(user);
                }
            }

            return null;

        }

        public async Task<UserViewModel> GetByName(string Name)
        {
            return mapper.Map<UserViewModel>(await userRepository.GetByName(Name));
        }

        public async Task<ValidationResult> ChangeUserPassword(UserViewModel user, string newPassword, Guid loggedUserId)
        {
            var _user = mapper.Map<User>(user);
            _user.UpdatedAt = DateTime.Now;
            _user.Password = mD5Service.ReturnMD5(newPassword);

            var result = await userRepository.Update(_user);

            if (result == null)
                return new ValidationResult("Houve um problema para alteraa a senha do Usuário");

            // adiciona a tabela de histórico de alteracao
            await history.Add(TableEnum.User, result.Id, OperationEnum.AlteracaoSenha, loggedUserId);

            return ValidationResult.Success;

        }

        public async Task<UserViewModel> GetByEmail(string email)
        {
            return mapper.Map<UserViewModel>(await userRepository.GetByEmail(email));
        }
    }
}
