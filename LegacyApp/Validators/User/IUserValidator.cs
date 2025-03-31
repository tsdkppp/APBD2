using System;

namespace LegacyApp;

public interface IUserValidator
{
    bool IsValidName(string firstName, string lastName);
    bool IsValidEmail(string email);
    bool IsValidAge(DateTime dateOfBirth);
}