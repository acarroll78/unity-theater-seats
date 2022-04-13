using System;
using System.Text.Json.Serialization;

public class User
{
	public string Name { get; }
	public UInt32 Id { get; }

	[JsonConstructor]
	public User(string Name, UInt32 Id)
	{
		this.Name = Name;
		this.Id = Id;
	}
}
