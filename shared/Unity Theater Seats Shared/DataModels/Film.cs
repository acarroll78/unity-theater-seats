using System;
using System.Text.Json.Serialization;

public class Film
{
	public string Name { get; }
	public UInt32 Id { get; }

	[JsonConstructor]
	public Film(string Name, UInt32 Id)
	{
		this.Name = Name;
		this.Id = Id;
	}
}
