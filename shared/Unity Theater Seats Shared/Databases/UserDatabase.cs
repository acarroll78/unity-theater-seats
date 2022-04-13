using System;
using System.Collections.Generic;

public class UserDatabase
{
	private Dictionary<UInt32, User> Users; //< Keyed by UserId.
	private UInt32 HighestId;

	public UserDatabase(User[] ExistingUsers)
	{
		Users = new Dictionary<UInt32, User>();
		HighestId = 1;

		foreach (User existingUser in ExistingUsers)
		{
			if (GetUserById(existingUser.Id) == null)
			{
				Users.Add(existingUser.Id, existingUser);

				if (existingUser.Id > HighestId)
				{
					HighestId = existingUser.Id;
				}
			}
		}
	}

	// Gets all users
	public User[] GetAllUsers()
	{
		List<User> userList = new List<User>();
		foreach(User existingUser in Users.Values)
		{
			userList.Add(existingUser);
		}
		return userList.ToArray();
	}

	// Returns null if not found.
	public User GetUserById(UInt32 Id)
	{
		return Users.ContainsKey(Id) ? Users[Id] : null;
	}

	// Attempts to add a new user and increments the internal id counter if successful.
	// Returns the newly created User OR null if it failed.
	public User GetOrAddUserByName(string Name)
	{
		foreach(User ExistingUser in Users.Values)
		{
			if(ExistingUser.Name == Name)
			{
				return ExistingUser;
			}
		}

		User newUser = new User(Name, ++HighestId);
		Users.Add(newUser.Id, newUser);

		return newUser;
	}
}
