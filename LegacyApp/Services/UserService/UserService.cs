using System;

namespace LegacyApp
{
    public class UserService : IUserService
    {
        private readonly IClientRepository _clientRepository;
        private readonly ICreditProcessor _creditProcessor;
        private readonly IUserValidator _userValidator;
        private readonly IUserRepository _userRepository;

        public UserService() : this
        (
            new CreditProcessor(),
            new ClientRepository(),
            new UserValidator(),
            new UserRepository()
        )
        {
        }

        public UserService(ICreditProcessor creditProcessor,
            IClientRepository clientRepository, IUserValidator userValidator, IUserRepository userRepository)
        {
            _creditProcessor = creditProcessor;
            _clientRepository = clientRepository;
            _userValidator = userValidator;
            _userRepository = userRepository;
        }

        private User CreateUserByClientId(string firstName, string lastName,
            string email, DateTime dateOfBirth, int clientId)
        {
            var client = _clientRepository.GetById(clientId);

            return new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName
            };
        }

        public bool AddUser(string firstName, string lastName,
            string email, DateTime dateOfBirth, int clientId)
        {
            if (!_userValidator.IsValidName(firstName, lastName)) return false;
            if (!_userValidator.IsValidEmail(email)) return false;
            if (!_userValidator.IsValidAge(dateOfBirth)) return false;

            User user = CreateUserByClientId(firstName,
                lastName,
                email,
                dateOfBirth,
                clientId);

            try
            {
                _creditProcessor.ProcessCredit(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            _userRepository.AddUser(user);
            return true;
        }
    }
}