using System;
using System.Collections.Generic;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Unity_Theater_Seats_Server
{
	class Program
	{
		static void Main(string[] args)
		{
			UserDatabase userDatabase;
			FilmDatabase filmDatabase;
			ShowDatabase showDatabase;
			ReservationDatabase reservationDatabase;

			// Load all data from disk and populate relevant members.
			// 1. Users
			User[] existingUsers;
			JsonDataHelper.ReadJsonData<User[]>(@"data\users.json", out existingUsers);
			userDatabase = new UserDatabase(existingUsers);
			existingUsers = null;

			// 2. Films
			Film[] activeFilms;
			JsonDataHelper.ReadJsonData<Film[]>(@"data\films.json", out activeFilms);
			filmDatabase = new FilmDatabase(activeFilms);
			activeFilms = null;

			// 3. Shows
			Show[] activeShows;
			JsonDataHelper.ReadJsonData<Show[]>(@"data\shows.json", out activeShows);
			showDatabase = new ShowDatabase(activeShows);
			activeShows = null;

			// 4. Reservations
			Reservation[] activeReservations;
			JsonDataHelper.ReadJsonData<Reservation[]>(@"data\reservations.json", out activeReservations);
			reservationDatabase = new ReservationDatabase(activeReservations);
			activeReservations = null;

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
			webSocketServer.AddWebSocketService<RequestFilmsBehavior>("/RequestFilms");

			// RequestShows Route
			webSocketServer.AddWebSocketService<RequestShowsBehavior>("/RequestShows");

			// RequestReservations Route
			System.Func<RequestReservationsBehavior> SetupRequestReservation = () =>
			{
				RequestReservationsBehavior requestReservationBehavior = new RequestReservationsBehavior();
				requestReservationBehavior.Setup(reservationDatabase);
				return requestReservationBehavior;
			};

			webSocketServer.AddWebSocketService("/RequestReservations", SetupRequestReservation);

			// Officially start the WebSocket server
			webSocketServer.Start();
			Console.WriteLine("Server started on ws://localhost:3278 with routes: /Login, /ReserveSeat /RequestFilms /RequestShows /RequestReservations");

			Console.ReadKey();
			webSocketServer.Stop();

			// Write User and Reservation Databases to JSON before exiting.
			JsonDataHelper.WriteUsersToFile(userDatabase.GetAllUsers());
			JsonDataHelper.WriteReservationsToFile(reservationDatabase.GetAllReservations());
			Console.WriteLine("Server shutting down.");
		}
	}
}
