namespace LegacyApp;

public class UserRepository : IUserRepository
{
    public void AddUser(User user)
    {
        UserDataAccess.AddUser(user);
    }
}