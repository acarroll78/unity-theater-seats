using System;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Unity_Theater_Seats_Server
{
	class RequestShowsBehavior: WebSocketBehavior
	{
		protected override void OnOpen()
		{
			Console.WriteLine("/RequestShows Connection established. Returning JSON list of shows.");

			// The shows are already json-ified in the data files and aren't likely to have changed, so send them directly.
			string jsonFilePath = @"data\shows.json";
			if (File.Exists(jsonFilePath))
			{
				string json = File.ReadAllText(jsonFilePath, System.Text.Encoding.UTF8);
				Send(json);
			}
		}

		protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine("/RequestShows Connection closed.");
		}
	}
}
