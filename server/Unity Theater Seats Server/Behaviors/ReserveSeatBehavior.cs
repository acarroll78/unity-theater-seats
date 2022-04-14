using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using FlatBuffers;

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
			ByteBuffer buffer = new ByteBuffer(e.RawData);
			Reservation newReservation = Reservation.GetRootAsReservation(buffer);

			Console.WriteLine("/ReserveSeat Received message from client. User Id ({0}), Show Id ({1}), Seat Id ({2})", newReservation.UserId, newReservation.ShowId, newReservation.SeatId);

			if (ReservationDB.AddReservation(newReservation))
			{
				// Let all users know someone reserved a seat (this is overkill but works)
				Sessions.Broadcast(e.RawData);
			}
		}
	}
}
