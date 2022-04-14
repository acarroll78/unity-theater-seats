using System;
using System.Collections.Generic;
using FlatBuffers;

public class UserDatabase
{
	private Dictionary<UInt32, User> Users = new Dictionary<UInt32, User>(); //< Keyed by UserId.
	private UInt32 HighestId = 0;

	public UserDatabase()
	{

	}

	public UserDatabase(User[] NewUsers)
	{
		foreach (User newUser in NewUsers)
		{
			if (!Users.ContainsKey(newUser.Id))
			{
				AddUser(newUser.Name, newUser.Id);
			}
		}
	}

	public User[] GetAllUsers()
	{
		List<User> userList = new List<User>();
		foreach(User existingUser in Users.Values)
		{
			userList.Add(existingUser);
		}
		return userList.ToArray();
	}

	// Returns default initialized user if not found
	public User GetUserById(UInt32 Id)
	{
		return Users.ContainsKey(Id) ? Users[Id] : new User();
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

		User newUser = AddUser(Name, ++HighestId);
		return newUser;
	}

	public void AddUser(User NewUser)
	{
		if (!Users.ContainsKey(NewUser.Id))
		{
			Users.Add(NewUser.Id, NewUser);
			if(NewUser.Id > HighestId)
			{
				HighestId = NewUser.Id;
			}
		}
	}

	private User AddUser(string Name, UInt32 Id)
	{
		FlatBufferBuilder builder = new FlatBufferBuilder(256);
		StringOffset nameOffset = builder.CreateString(Name);
		Offset<User> newUserOffset = User.CreateUser(builder, nameOffset, Id);
		builder.Finish(newUserOffset.Value);
		byte[] bytes = builder.SizedByteArray();
		ByteBuffer buffer = new ByteBuffer(bytes);
		User newUser = User.GetRootAsUser(buffer);
		Users.Add(Id, newUser);

		return newUser;
	}
}
