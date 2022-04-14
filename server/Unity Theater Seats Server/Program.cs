using System;
using WebSocketSharp.Server;

namespace Unity_Theater_Seats_Server
{
	class Program
	{
		static void Main(string[] args)
		{
			UserDatabase userDatabase = new UserDatabase();
			FilmDatabase filmDatabase = new FilmDatabase();
			ShowDatabase showDatabase = new ShowDatabase();
			ReservationDatabase reservationDatabase = new ReservationDatabase();

			// Load all data from disk and populate relevant members.
			// 1. Users
			FlatBufferDataHelper.InitializeUserDatabase(userDatabase, @"data\users.bin", printDetails:false);

			// 2. Films
			FlatBufferDataHelper.InitializeFilmDatabase(filmDatabase, @"data\films.bin", printDetails:false);

			// 3. Shows
			FlatBufferDataHelper.InitializeShowDatabase(showDatabase, @"data\shows.bin", printDetails:false);

			// 4. Reservations
			FlatBufferDataHelper.InitializeReservationDatabase(reservationDatabase, @"data\reservations.bin", printDetails:false);

			// Setup WebSocket and routes
			WebSocketServer webSocketServer = new WebSocketServer("ws://localhost:3278");

			// Login Route
			System.Func<LoginBehavior> SetupLogin = () =>
			{
				LoginBehavior loginBehavior = new LoginBehavior();
				loginBehavior.Setup(userDatabase);
				return loginBehavior;
			};
			webSocketServer.AddWebSocketService("/Login", SetupLogin);

			// ReserveSeat Route
			System.Func<ReserveSeatBehavior> SetupReserveSeat = () =>
			{
				ReserveSeatBehavior reserveSeatBehavior = new ReserveSeatBehavior();
				reserveSeatBehavior.Setup(reservationDatabase);
				return reserveSeatBehavior;
			};
			webSocketServer.AddWebSocketService("/ReserveSeat", SetupReserveSeat);

			// RequestFilms Route
			System.Func<RequestFilmsBehavior> SetupRequestFilms = () =>
			{
				RequestFilmsBehavior requestFilmsBehavior = new RequestFilmsBehavior();
				requestFilmsBehavior.Setup(filmDatabase);
				return requestFilmsBehavior;
			};
			webSocketServer.AddWebSocketService("/RequestFilms", SetupRequestFilms);

			// RequestShows Route
			System.Func<RequestShowsBehavior> SetupRequestShows = () =>
			{
				RequestShowsBehavior requestShowsBehavior = new RequestShowsBehavior();
				requestShowsBehavior.Setup(showDatabase);
				return requestShowsBehavior;
			};
			webSocketServer.AddWebSocketService("/RequestShows", SetupRequestShows);

			// RequestReservations Route
			System.Func<RequestReservationsBehavior> SetupRequestReservations = () =>
			{
				RequestReservationsBehavior requestReservationBehavior = new RequestReservationsBehavior();
				requestReservationBehavior.Setup(reservationDatabase);
				return requestReservationBehavior;
			};

			webSocketServer.AddWebSocketService("/RequestReservations", SetupRequestReservations);

			// Officially start the WebSocket server
			webSocketServer.Start();
			Console.WriteLine("Server started on ws://localhost:3278 with routes: /Login, /ReserveSeat /RequestFilms /RequestShows /RequestReservations");

			Console.ReadKey();
			webSocketServer.Stop();

			// Write User and Reservation Databases to JSON before exiting.
			// JsonDataHelper.WriteUsersToFile(userDatabase.GetAllUsers());
			// JsonDataHelper.WriteReservationsToFile(reservationDatabase.GetAllReservations());
			Console.WriteLine("Server shutting down.");
		}
	}
}
