using System;
using System.Text.Json.Serialization;

public class Show : IComparable<Show>
{
	public UInt32 Id { get; }
	public UInt32 FilmId { get; }
	public DateTime ShowTime { get; }

	[JsonConstructor]
	public Show(uint Id, uint FilmId, DateTime ShowTime)
	{
		this.Id = Id;
		this.FilmId = FilmId;
		this.ShowTime = ShowTime;
	}

	public int CompareTo(Show that)
	{
		return this.ShowTime.CompareTo(that.ShowTime);
	}
}
