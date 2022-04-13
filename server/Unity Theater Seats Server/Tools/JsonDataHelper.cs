using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Unity_Theater_Seats_Server
{
    // Helper Functions to create the JSON files used by the app (creation/file writing functions are not used at runtime).
    // Creates needed JSON and leaves it in the destination directory.
    // At the moment, all of these JSON files are pretty-printed for ease of editing.
    //
    // There is also a Templated ReadJsonData() function for loading the Json into static data containers.
    // 
    // Note: Remember to move the JSON files to the data directory in the project root as there is no automatic copy ATM.
    static class JsonDataHelper
    {
        public static void CreateUsers(string destinationPath = @"data\users.json")
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true,
            };

            User[] Users = new User[13]
            {
                new User("Bert", 1),
                new User("Ernie", 2),
                new User("Thelma", 3),
                new User("Louise", 4),
                new User("Fred", 5),
                new User("Ginger", 6),
                new User("Shaq", 7),
                new User("Kobe", 8),
                new User("Velma", 9),
                new User("Daphne", 10),
                new User("Neo", 11),
                new User("Trinity", 12),
                new User("Morpheus", 13)
            };

            string jsonString = JsonSerializer.Serialize(Users, options);
            File.WriteAllText(destinationPath, jsonString);
        }

        public static void CreateFilms(string destinationPath = @"data\films.json")
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true,
            };

            Film[] Films = new Film[7]
            {
                new Film(@"Thunderball", 1),
                new Film(@"Angus and the Monkey King", 2),
                new Film(@"Stab 5", 3),
                new Film(@"Smoochie", 4),
                new Film(@"Well Said", 5),
                new Film(@"Treason and High Crimes", 6),
                new Film(@"Goo-Face", 7),
            };

            string jsonString = JsonSerializer.Serialize(Films, options);
            File.WriteAllText(destinationPath, jsonString);
        }

        public static void CreateShows(string destinationPath = @"data\shows.json")
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true,
            };

            UInt32 showId = 1;
            UInt32[] Films = { 1, 2, 3, 4, 5, 6, 7 };
            int Month = 5;
            int[] Days = { 5, 6, 7, 8, 9 };
            int Year = 2025;

            List<Show> Shows = new List<Show>();
            Random rand = new Random();

            for(int i = 0; i < 55; ++i)
			{
                DateTime randDateTime = new DateTime(Year, Month, Days[rand.Next(0, Days.Length)], rand.Next(11, 23), rand.Next(0, 12) * 5, 0);
                Shows.Add(new Show(showId++, Films[rand.Next(0, Films.Length)], randDateTime));
			}

            string jsonString = JsonSerializer.Serialize(Shows.ToArray(), options);
            File.WriteAllText(destinationPath, jsonString);
        }

        public static void CreateReservations(string destinationPath = @"data\reservations.json")
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true,
            };

            List<Reservation> Reservations = new List<Reservation>();
            Random rand = new Random();

            int numUsers = 13;
            int numShows = 55;
            int numSeats = 60;
            
            int numReservations = 0;
            int requiredUniqueReservations = 255;
            while (numReservations < requiredUniqueReservations)
			{
                Reservation res = new Reservation((uint)rand.Next(0, numUsers) + 1, (uint)rand.Next(0, numShows) + 1, (uint)rand.Next(0, numSeats) + 1);
                if(!Reservations.Contains(res))
				{
                    Reservations.Add(res);
                    ++numReservations;
				}
			}

            string jsonString = JsonSerializer.Serialize(Reservations.ToArray(), options);
            File.WriteAllText(destinationPath, jsonString);
        }

        public static bool ReadJsonData<T>(string jsonFilePath, out T data)
        {
            if (File.Exists(jsonFilePath))
            {
                var options = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };
                string json = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF8);
                data = JsonSerializer.Deserialize<T>(json, options);

                if (data == null)
                {
                    Console.WriteLine(string.Format("Failed to read json file at {0}!", jsonFilePath));
                }

                return data != null;
            }
            else
            {
                Console.WriteLine(string.Format("Failed to load json file at {0}!", jsonFilePath));
                data = default(T);
                return false;
            }
        }

        public static void WriteUsersToFile(User[] Users, string destinationPath = @"data\users.json")
		{
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(Users, options);
            File.WriteAllText(destinationPath, jsonString);
        }

        public static void WriteReservationsToFile(Reservation[] Reservations, string destinationPath = @"data\reservations.json")
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(Reservations, options);
            File.WriteAllText(destinationPath, jsonString);
        }
    }
}
