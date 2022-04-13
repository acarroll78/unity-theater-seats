using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSocketSharp;
using WebSocketSharp.Server;

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
			Console.WriteLine("/RequestReservations Connection established. Returning JSON list of reservations.");

			// Convert all existing reservations into json and send.
			var options = new JsonSerializerOptions
			{
				Converters = { new JsonStringEnumConverter() },
				WriteIndented = false,
			};

			Reservation[] res = ReservationDB.GetAllReservations();
			string jsonString = JsonSerializer.Serialize(res, options);
			Send(jsonString);
		}

		protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine("/RequestReservations Connection closed.");
		}
	}
}
