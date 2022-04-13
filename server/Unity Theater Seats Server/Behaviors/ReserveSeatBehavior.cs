using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Unity_Theater_Seats_Server
{
	// Attempts to reserve a seat for a show for the current user, which is broadcast to all sessions if successful.
	// Expects:
	// int userId - requesting user's id
	// int showId - the specific show's id
	// int seatNumber - the number of the seat being requested
	class ReserveSeatBehavior : WebSocketBehavior
	{
		ReservationDatabase ReservationDB;

		public void Setup(ReservationDatabase ReservationDB)
		{
			this.ReservationDB = ReservationDB;
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			Console.WriteLine("/ReserveSeat Received message from client: " + e.Data);

			var options = new JsonSerializerOptions
			{
				Converters = { new JsonStringEnumConverter() },
				WriteIndented = false,
			};

			Reservation res = JsonSerializer.Deserialize<Reservation>(e.Data, options);
			if(ReservationDB.AddReservation(res))
			{
				// Let all users know someone reserved a seat (this is overkill but works)
				Sessions.Broadcast(e.Data);
			}
		}
	}
}
