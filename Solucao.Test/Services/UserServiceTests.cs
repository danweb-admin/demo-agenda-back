using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Solucao.Application.AutoMapper;
using Solucao.Application.Contracts;
using Solucao.Application.Data;
using Solucao.Application.Data.Entities;
using Solucao.Application.Data.Repositories;
using Solucao.Application.Service.Implementations;
using Solucao.Application.Service.Interfaces;
using Xunit;

namespace Solucao.Tests
{
    public class UserServiceTests
    {

        private Mock<SolucaoContext> contextMock;
        private Mock<UserRepository> repositoryMock;
        private Mock<HistoryRepository> historyMock;
        private Mock<IHttpContextAccessor> httpContextMock;
        private Mock<ILogger<UserRepository>> loggerMock;
        private Mock<UserRepository> userRepoMock;

        private readonly IMapper _mapper;

        public UserServiceTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new EntityToViewModelMappingProfile());
                cfg.AddProfile(new ViewModelToEntityMappingProfile());
            }).CreateMapper();
            loggerMock = new Mock<ILogger<UserRepository>>();
            httpContextMock = new Mock<IHttpContextAccessor>();
            contextMock = new Mock<SolucaoContext>();
            userRepoMock = new Mock<UserRepository>(contextMock.Object, loggerMock.Object);
            historyMock = new Mock<HistoryRepository>(contextMock.Object, httpContextMock.Object, userRepoMock.Object);
            repositoryMock = new Mock<UserRepository>(contextMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsUsers()
        {
            // Arrange
            var mD5ServiceMock = new Mock<IMD5Service>();
            var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object, historyMock.Object);

            repositoryMock.Setup(repo => repo.GetAll(true))
                .ReturnsAsync(new List<User>());

            // Act
            var result = await userService.GetAll(true);

            // Assert
            Assert.NotNull(result);
        }

        //[Fact]
        //public async Task GetById_ValidId_ReturnsUser()
        //{
        //    // Arrange
        //    var mD5ServiceMock = new Mock<IMD5Service>();
        //    var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object);

        //    var userId = Guid.NewGuid(); 

        //    repositoryMock.Setup(repo => repo.GetById(userId))
        //        .ReturnsAsync(new User());

        //    // Act
        //    var result = await userService.GetById(userId);

        //    // Assert
        //    Assert.NotNull(result);
        //}

        [Fact]
        public async Task Add_ValidUser_ReturnsSuccess()
        {
            // Arrange
            var mD5ServiceMock = new Mock<IMD5Service>();
            var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object, historyMock.Object);

            var user = new User
            {
                // Provide valid data for testing
            };

            var userViewModel = new UserViewModel
            {
                Id = Guid.NewGuid()
            };

            repositoryMock.Setup(repo => repo.Add(user))
                .ReturnsAsync(new User());

            

            // Act
            var result = await userService.Add(user, userViewModel.Id);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public async Task Update_ValidUser_ReturnsSuccess()
        {
            // Arrange
            var mD5ServiceMock = new Mock<IMD5Service>();
            var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object, historyMock.Object);

            var userId = Guid.NewGuid(); 
            var userViewModel = new User
            {
                Id = userId
            };

            repositoryMock.Setup(repo => repo.GetById(userId))
                .ReturnsAsync(new User { Id = userId });

            repositoryMock.Setup(repo => repo.Update(It.IsAny<User>()))
                .ReturnsAsync(new User());

            // Act
            var result = await userService.Update(userViewModel, userId, userViewModel.Id);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public async Task Authenticate_ValidCredentials_ReturnsUserViewModel()
        {
            // Arrange
            var mD5ServiceMock = new Mock<IMD5Service>();
            var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object, historyMock.Object);

            var email = "test@example.com";
            var password = "password"; 

            repositoryMock.Setup(repo => repo.GetByEmail(email))
                .ReturnsAsync(new User
                {
                    Email = email,
                    Password = mD5ServiceMock.Object.ReturnMD5(password) 
                });

            mD5ServiceMock.Setup(service => service.CompareMD5(password, It.IsAny<string>()))
                .Returns(true);

            // Act
            var result = await userService.Authenticate(email, password);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetByName_ValidName_ReturnsUserViewModel()
        {
            // Arrange
            var mD5ServiceMock = new Mock<IMD5Service>();
            var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object, historyMock.Object);

            var userName = "John Doe"; 

            repositoryMock.Setup(repo => repo.GetByName(userName))
                .ReturnsAsync(new User { Name = userName });

            // Act
            var result = await userService.GetByName(userName);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task ChangeUserPassword_ValidUser_ReturnsSuccess()
        {
            // Arrange
            var mD5ServiceMock = new Mock<IMD5Service>();
            var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object, historyMock.Object);

            var userViewModel = new UserViewModel
            {
                Id = Guid.NewGuid()
            };

            repositoryMock.Setup(repo => repo.Update(It.IsAny<User>()))
                .ReturnsAsync(new User());

            // Act
            var result = await userService.ChangeUserPassword(userViewModel, "newPassword", userViewModel.Id);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public async Task GetByEmail_ValidEmail_ReturnsUserViewModel()
        {
            // Arrange
            var mD5ServiceMock = new Mock<IMD5Service>();
            var userService = new UserService(repositoryMock.Object, _mapper, mD5ServiceMock.Object, historyMock.Object);

            var userEmail = "test@example.com"; // Provide a valid userEmail for testing

            repositoryMock.Setup(repo => repo.GetByEmail(userEmail))
                .ReturnsAsync(new User { /* Provide valid data for testing */ });

            // Act
            var result = await userService.GetByEmail(userEmail);

            // Assert
            Assert.NotNull(result);
        }
    }
}
