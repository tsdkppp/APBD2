using System;

namespace LegacyApp
{
    public class CreditProcessor : ICreditProcessor
    {
        private readonly IUserCreditService _userCreditService;

        public CreditProcessor() : this(new UserCreditService())
        {
        }

        public CreditProcessor(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService ?? throw new ArgumentNullException(nameof(userCreditService));
        }

        public void ProcessCredit(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.Client == null)
                throw new ArgumentNullException(nameof(user.Client), "User must have an associated Client.");

            string clientType = user.Client.Type;

            if (clientType == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else
            {
                user.HasCreditLimit = true;

                int creditLimit = _userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);

                if (clientType == "ImportantClient")
                {
                    user.CreditLimit = creditLimit * 2;
                }
                else
                {
                    user.CreditLimit = creditLimit;
                }
            }

            if (user.HasCreditLimit && user.CreditLimit < 500)
            {
                throw new InsufficientCreditException(
                    $"User {user.FirstName} {user.LastName} " +
                    $"does not meet the minimum credit limit of 500.");
            }
        }
    }
}