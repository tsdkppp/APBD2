using System;
using LegacyApp;

namespace LegacyApp.Tests
{
    public static class TestDataFactory
    {
        public static Client CreateClient(int clientId)
        {
            return new Client
            {
                ClientId = clientId,
                Name = "DefaultClient",
                Email = "default@client.com",
                Address = "123 Main St",
                Type = "Default"
            };
        }

        public static Client CreateClient(int clientId, string name)
        {
            return new Client
            {
                ClientId = clientId,
                Name = name,
                Email = $"{name.ToLower()}@client.com",
                Address = "123 Main St",
                Type = "Default"
            };
        }

        public static Client CreateClient(int clientId, string name, string email, string address, string type)
        {
            return new Client
            {
                ClientId = clientId,
                Name = name,
                Email = email,
                Address = address,
                Type = type
            };
        }

        public static User CreateUser(Client client)
        {
            return new User
            {
                Client = client,
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john.doe@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                HasCreditLimit = false,
                CreditLimit = 0
            };
        }

        public static User CreateUser(Client client, string firstName, string lastName, string emailAddress,
            DateTime dob, bool hasCreditLimit, int creditLimit)
        {
            return new User
            {
                Client = client,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                DateOfBirth = dob,
                HasCreditLimit = hasCreditLimit,
                CreditLimit = creditLimit
            };
        }
    }
}