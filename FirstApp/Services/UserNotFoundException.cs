using System;

namespace FirstApp.Service;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string userName) : base($"No user found with username: {userName}")
    {
    }
}
