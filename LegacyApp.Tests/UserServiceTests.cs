using NUnit.Framework;
using Moq;
using System;
using LegacyApp;
using LegacyApp.Tests;

namespace LegacyApp.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IClientRepository> _mockClientRepository;
        private Mock<IUserValidator> _mockUserValidator;
        private Mock<ICreditProcessor> _mockCreditProcessor;
        private Mock<IUserRepository> _mockUserRepository;

        private UserService _userService; 

        [SetUp]
        public void Setup()
        {
            _mockClientRepository = new Mock<IClientRepository>();
            _mockUserValidator = new Mock<IUserValidator>();
            _mockCreditProcessor = new Mock<ICreditProcessor>();
            _mockUserRepository = new Mock<IUserRepository>();

            _userService = new UserService(
                _mockCreditProcessor.Object,
                _mockClientRepository.Object,
                _mockUserValidator.Object,
                _mockUserRepository.Object
            );
        }

        private const string ValidFirstName = "John";
        private const string ValidLastName = "Doe";
        private const string ValidEmail = "john.doe@example.com";
        private readonly DateTime ValidDob = new DateTime(1990, 1, 1);
        private const int ValidClientId = 1;

        [Test]
        public void AddUser_ShouldReturnFalse_WhenNameIsInvalid()
        {
            _mockUserValidator.Setup(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
            _mockUserValidator.Setup(v => v.IsValidEmail(It.IsAny<string>())).Returns(true);
            _mockUserValidator.Setup(v => v.IsValidAge(It.IsAny<DateTime>())).Returns(true);


            bool result = _userService.AddUser("", ValidLastName, ValidEmail, ValidDob, ValidClientId);
            
            Assert.That(result, Is.False);
            
            _mockUserValidator.Verify(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidEmail(It.IsAny<string>()), Times.Never);
            _mockClientRepository.Verify(cr => cr.GetById(It.IsAny<int>()), Times.Never);
            _mockCreditProcessor.Verify(cp => cp.ProcessCredit(It.IsAny<User>()), Times.Never);
            _mockUserRepository.Verify(ur => ur.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void AddUser_ShouldReturnFalse_WhenEmailIsInvalid()
        {
            _mockUserValidator.Setup(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true); 
            _mockUserValidator.Setup(v => v.IsValidEmail(It.IsAny<string>())).Returns(false); 
            _mockUserValidator.Setup(v => v.IsValidAge(It.IsAny<DateTime>())).Returns(true);


            bool result = _userService.AddUser(ValidFirstName, ValidLastName, "invalid-email", ValidDob, ValidClientId);

            Assert.That(result, Is.False);
            _mockUserValidator.Verify(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidEmail(It.IsAny<string>()), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidAge(It.IsAny<DateTime>()), Times.Never);
            _mockClientRepository.Verify(cr => cr.GetById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void AddUser_ShouldReturnFalse_WhenAgeIsInvalid()
        {
            _mockUserValidator.Setup(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _mockUserValidator.Setup(v => v.IsValidEmail(It.IsAny<string>())).Returns(true);
            _mockUserValidator.Setup(v => v.IsValidAge(It.IsAny<DateTime>())).Returns(false); 

            bool result = _userService.AddUser(ValidFirstName, ValidLastName, ValidEmail, DateTime.Now.AddYears(-10),
                ValidClientId);

            Assert.That(result, Is.False);
            _mockUserValidator.Verify(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidEmail(It.IsAny<string>()), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidAge(It.IsAny<DateTime>()), Times.Once);
            _mockClientRepository.Verify(cr => cr.GetById(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void AddUser_ShouldReturnFalse_WhenCreditProcessingThrowsException()
        {
            var testClient = TestDataFactory.CreateClient(ValidClientId);
            _mockUserValidator.Setup(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _mockUserValidator.Setup(v => v.IsValidEmail(It.IsAny<string>())).Returns(true);
            _mockUserValidator.Setup(v => v.IsValidAge(It.IsAny<DateTime>())).Returns(true);
            _mockClientRepository.Setup(cr => cr.GetById(ValidClientId)).Returns(testClient);
            _mockCreditProcessor.Setup(cp => cp.ProcessCredit(It.IsAny<User>()))
                .Throws<InsufficientCreditException>(); 

            bool result = _userService.AddUser(ValidFirstName, ValidLastName, ValidEmail, ValidDob, ValidClientId);

            Assert.That(result, Is.False); 
            _mockUserValidator.Verify(v => v.IsValidName(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidEmail(It.IsAny<string>()), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidAge(It.IsAny<DateTime>()), Times.Once);
            _mockClientRepository.Verify(cr => cr.GetById(ValidClientId), Times.Once);
            _mockCreditProcessor.Verify(cp => cp.ProcessCredit(It.Is<User>(u => u.FirstName == ValidFirstName)),
                Times.Once);
            _mockUserRepository.Verify(ur => ur.AddUser(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public void AddUser_ShouldReturnTrue_And_CallRepositoryAddUser_WhenAllChecksPass()
        {
            var testClient = TestDataFactory.CreateClient(ValidClientId, "ImportantClient");
            // Setup all validators to pass
            _mockUserValidator.Setup(v => v.IsValidName(ValidFirstName, ValidLastName)).Returns(true);
            _mockUserValidator.Setup(v => v.IsValidEmail(ValidEmail)).Returns(true);
            _mockUserValidator.Setup(v => v.IsValidAge(ValidDob)).Returns(true);
            _mockClientRepository.Setup(cr => cr.GetById(ValidClientId)).Returns(testClient);
            _mockCreditProcessor.Setup(cp => cp.ProcessCredit(It.IsAny<User>()));
            _mockUserRepository.Setup(ur => ur.AddUser(It.IsAny<User>()));

            bool result = _userService.AddUser(ValidFirstName, ValidLastName, ValidEmail, ValidDob, ValidClientId);

            Assert.That(result, Is.True); 

            _mockUserValidator.Verify(v => v.IsValidName(ValidFirstName, ValidLastName), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidEmail(ValidEmail), Times.Once);
            _mockUserValidator.Verify(v => v.IsValidAge(ValidDob), Times.Once);
            _mockClientRepository.Verify(cr => cr.GetById(ValidClientId), Times.Once);
            _mockCreditProcessor.Verify(cp => cp.ProcessCredit(It.Is<User>(u => 
                u.FirstName == ValidFirstName &&
                u.LastName == ValidLastName &&
                u.EmailAddress == ValidEmail &&
                u.DateOfBirth == ValidDob &&
                u.Client == testClient
            )), Times.Once);
            _mockUserRepository.Verify(ur => ur.AddUser(It.Is<User>(u => 
                u.FirstName == ValidFirstName &&
                u.Client.ClientId == ValidClientId
            )), Times.Once);
        }
    }
}