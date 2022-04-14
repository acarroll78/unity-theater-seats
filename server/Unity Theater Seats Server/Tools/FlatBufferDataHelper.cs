using System;
using System.Collections.Generic;
using System.IO;
using FlatBuffers;

namespace Unity_Theater_Seats_Server
{
	static class FlatBufferDataHelper
	{
        public static void CreateUsers(string destinationPath = @"data\users.bin")
		{
            UInt32 userId = 0;
            string[] names = new string[]
            {
                "Bert",
                "Ernie",
                "Thelma",
                "Louise",
                "Fred",
                "Ginger", 
                "Shaq",
                "Kobe", 
                "Velma",
                "Daphne", 
                "Neo",
                "Trinity",
                "Morpheus"
            };

            var builder = new FlatBufferBuilder(2048);
            Offset<User>[] userOffsets = new Offset<User>[names.Length];
            for (int i = 0; i < names.Length; ++i)
            {
                var userNameOffset = builder.CreateString(names[i]);
                var userOffset = User.CreateUser(builder, userNameOffset, ++userId);
                userOffsets[i] = userOffset;
            }

            FlatBuffers.VectorOffset usersVectorOffset = UserList.CreateUsersVector(builder, userOffsets);
            UserList.StartUserList(builder);
            UserList.AddUsers(builder, usersVectorOffset);
            var userList = UserList.EndUserList(builder);
            builder.Finish(userList.Value);
            byte[] buffer = builder.SizedByteArray();

            File.WriteAllBytes(destinationPath, buffer);
        }

        public static void CreateFilms(string destinationPath = @"data\films.bin")
        {
            UInt32 filmId = 0;
            string[] names = new string[]
            {
                "Thunderball",
                "Angus and the Lizard King",
                "Stab 5",
                "Smoochie",
                "Well Said",
                "High Crimes",
                "Goo-Face"
            };

            var builder = new FlatBufferBuilder(2048);
            Offset<Film>[] filmOffsets = new Offset<Film>[names.Length];
            for (int i = 0; i < names.Length; ++i)
            {
                var filmNameOffset = builder.CreateString(names[i]);
                var filmOffset = Film.CreateFilm(builder, filmNameOffset, ++filmId);
                filmOffsets[i] = filmOffset;
            }

            FlatBuffers.VectorOffset filmsVectorOffset = FilmList.CreateFilmsVector(builder, filmOffsets);
            FilmList.StartFilmList(builder);
            FilmList.AddFilms(builder, filmsVectorOffset);
            var filmList = FilmList.EndFilmList(builder);
            builder.Finish(filmList.Value);
            byte[] buffer = builder.SizedByteArray();

            File.WriteAllBytes(destinationPath, buffer);
        }

        public static void CreateShows(string destinationPath = @"data\shows.bin")
		{
            int numShowsToCreate = 55;

            UInt32 showId = 1;
            int Month = 5;
            int[] Days = { 5, 6, 7, 8, 9 };
            int Year = 2025;

            Random rand = new Random();

            var builder = new FlatBufferBuilder(65536);
            Offset<Show>[] showOffsets = new Offset<Show>[numShowsToCreate];
            for (int i = 0; i < numShowsToCreate; ++i)
            {
                DateTime randDateTime = new DateTime(Year, Month, Days[rand.Next(0, Days.Length)], rand.Next(11, 24), rand.Next(0, 12) * 5, 0);
                var showOffset = Show.CreateShow(builder,++showId, (uint)rand.Next(1,8), randDateTime.Ticks);
                showOffsets[i] = showOffset;
            }

            FlatBuffers.VectorOffset showsVectorOffset = ShowList.CreateShowsVector(builder, showOffsets);
            ShowList.StartShowList(builder);
            ShowList.AddShows(builder, showsVectorOffset);
            var showList = ShowList.EndShowList(builder);
            builder.Finish(showList.Value);
            byte[] buffer = builder.SizedByteArray();

            File.WriteAllBytes(destinationPath, buffer);
        }
        public static void CreateReservations(string destinationPath = @"data\reservations.bin")
		{
            Random rand = new Random();

            int numUsers = 13;
            int numShows = 55;
            int numSeats = 60;

            int numReservations = 0;
            int requiredUniqueReservations = 255;

            List<Tuple<UInt32, UInt32, UInt32>> reservations = new List<Tuple<UInt32, UInt32, UInt32>>();
            while (numReservations < requiredUniqueReservations)
            {
                Tuple<UInt32, UInt32, UInt32> res = new Tuple<UInt32, UInt32, UInt32>((uint)rand.Next(0, numUsers) + 1, (uint)rand.Next(0, numShows) + 1, (uint)rand.Next(0, numSeats) + 1);
                if (!reservations.Contains(res))
                {
                    reservations.Add(res);
                    ++numReservations;
                }
            }

            var builder = new FlatBufferBuilder(65536);
            Offset<Reservation>[] reservationOffsets = new Offset<Reservation>[requiredUniqueReservations];
            int resIndex = 0;
            foreach (Tuple<UInt32, UInt32, UInt32> reservation in reservations)
            {
                var reservationOffset = Reservation.CreateReservation(builder, reservation.Item1, reservation.Item2, reservation.Item3);
                reservationOffsets[resIndex++] = reservationOffset;
            }

            FlatBuffers.VectorOffset reservationVectorOffset = ReservationList.CreateReservationsVector(builder, reservationOffsets);
            ReservationList.StartReservationList(builder);
            ReservationList.AddReservations(builder, reservationVectorOffset);
            var reservationList = ReservationList.EndReservationList(builder);
            builder.Finish(reservationList.Value);
            byte[] buffer = builder.SizedByteArray();

            File.WriteAllBytes(destinationPath, buffer);
        }

        public static void InitializeUserDatabase(UserDatabase userDB, string dataPath = @"data\users.bin", bool printDetails = false)
		{
            byte[] bytes = File.ReadAllBytes(dataPath);
            ByteBuffer buffer = new ByteBuffer(bytes);
            UserList userList = UserList.GetRootAsUserList(buffer);
            int numUsers = userList.UsersLength;

            for (int i = 0; i < numUsers; ++i)
            {
                User newUser = userList.Users(i).Value;
                if(printDetails)
				{
                    Console.WriteLine("\tAdded User: Name ({0}), Id ({1}).", newUser.Name, newUser.Id);
                }
                userDB.AddUser(newUser);
            }
        }

        public static void InitializeFilmDatabase(FilmDatabase filmDB, string dataPath = @"data\films.bin", bool printDetails = false)
		{
            byte[] bytes = File.ReadAllBytes(dataPath);
            ByteBuffer buffer = new ByteBuffer(bytes);
            FilmList filmList = FilmList.GetRootAsFilmList(buffer);
            int numFilms = filmList.FilmsLength;

            for (int i = 0; i < numFilms; ++i)
            {
                Film newFilm = filmList.Films(i).Value;
                if (printDetails)
                {
                    Console.WriteLine("\tAdded Film: Name ({0}), Id ({1}).", newFilm.Name, newFilm.Id);
                }
                filmDB.AddFilm(newFilm);
            }
        }

        public static void InitializeShowDatabase(ShowDatabase showDB, string dataPath = @"data\shows.bin", bool printDetails = false)
        {
            byte[] bytes = File.ReadAllBytes(dataPath);
            ByteBuffer buffer = new ByteBuffer(bytes);
            ShowList showList = ShowList.GetRootAsShowList(buffer);
            int numShows = showList.ShowsLength;

            for (int i = 0; i < numShows; ++i)
            {
                Show newShow = showList.Shows(i).Value;
                if (printDetails)
                {
                    Console.WriteLine("\tAdded Show: Film Id ({0}), at ShowTime ({1}).", newShow.FilmId, newShow.ShowTime);
                }
                showDB.AddShow(newShow);
            }
        }

        public static void InitializeReservationDatabase(ReservationDatabase reservationDB, string dataPath = @"data\reservations.bin", bool printDetails = false)
		{
            byte[] bytes = File.ReadAllBytes(dataPath);
            ByteBuffer buffer = new ByteBuffer(bytes);
            ReservationList reservationList = ReservationList.GetRootAsReservationList(buffer);
            int numReservations = reservationList.ReservationsLength;

            for (int i = 0; i < numReservations; ++i)
            {
                Reservation newReservation = reservationList.Reservations(i).Value;
                if (printDetails)
                {
                    Console.WriteLine("\tAdded Show: User Id ({0}), Show Id ({1}, Seat Id ({2})).", newReservation.UserId, newReservation.ShowId, newReservation.SeatId);
                }
                reservationDB.AddReservation(newReservation);
            }
        }
    }
}
