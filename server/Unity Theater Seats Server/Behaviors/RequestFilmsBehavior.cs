using System;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Unity_Theater_Seats_Server
{
	public class RequestFilmsBehavior : WebSocketBehavior
	{
		protected override void OnOpen()
		{
			Console.WriteLine("/RequestFilms Connection established. Returning JSON list of films.");

			// The films are already json-ified in the data files and aren't likely to have changed, so send them directly.
            string jsonFilePath = @"data\films.json";
            if (File.Exists(jsonFilePath))
            {
                string json = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF8);
				Send(json);
            }
        }

        protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine("/RequestFilms Connection closed.");
		}
	}
}
