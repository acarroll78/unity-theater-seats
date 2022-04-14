using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using FlatBuffers;

namespace Unity_Theater_Seats_Server
{
	class RequestReservationsBehavior : WebSocketBehavior
	{
		ReservationDatabase ReservationDB;

		public void Setup(ReservationDatabase ReservationDB)
		{
			this.ReservationDB = ReservationDB;
		}

		protected override void OnOpen()
		{
			Console.WriteLine("/RequestReservations Connection established.");

			Reservation[] existingReservations = ReservationDB.GetAllReservations();

			if(existingReservations.Length > 0)
			{
				var builder = new FlatBufferBuilder(65536);
				Offset<Reservation>[] reservationOffsets = new Offset<Reservation>[existingReservations.Length];
				int resIndex = 0;
				foreach (Reservation existingReservation in existingReservations)
				{
					var reservationOffset = Reservation.CreateReservation(builder, existingReservation.UserId, existingReservation.ShowId, existingReservation.SeatId);
					reservationOffsets[resIndex++] = reservationOffset;
				}

				// Console.WriteLine("/RequestReservations Returning flat buffer containing a ReservationList with {0} Reservations.", existingReservations.Length);

				FlatBuffers.VectorOffset reservationVectorOffset = ReservationList.CreateReservationsVector(builder, reservationOffsets);
				ReservationList.StartReservationList(builder);
				ReservationList.AddReservations(builder, reservationVectorOffset);
				var reservationList = ReservationList.EndReservationList(builder);
				builder.Finish(reservationList.Value);
				byte[] buffer = builder.SizedByteArray();
				Send(buffer);
			}
		}

		protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine("/RequestReservations Connection closed.");
		}
	}
}
