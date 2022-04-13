using System;
using System.Text.Json.Serialization;

public class Reservation
{
	public UInt32 UserId { get; }
	public UInt32 ShowId { get; }
	public UInt32 SeatId { get; }
		
	[JsonConstructor]
	public Reservation(UInt32 UserId, UInt32 ShowId, UInt32 SeatId)
	{
		this.UserId = UserId;
		this.ShowId = ShowId;
		this.SeatId = SeatId;
	}
}
