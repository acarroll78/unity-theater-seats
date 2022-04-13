using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Unity_Theater_Seats_Server
{
	// Handles login messages sent from client.
	// Expects: string user name
	// Returns: int userId
	public class LoginBehavior : WebSocketBehavior
	{
		private UserDatabase UserDB;
		private User currentUser = null;

		public void Setup(UserDatabase UserDB)
		{
			this.UserDB = UserDB;
		}

		protected override void OnOpen()
		{
			Console.WriteLine("/Login Connection established.");
		}

		protected override void OnClose(CloseEventArgs e)
		{
			if(currentUser != null)
			{
				Console.WriteLine("/Login Connection closed for user {0}.", currentUser.Name);
			}
			else
			{
				Console.WriteLine("/Login Connection closed for unknown user.");
			}
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			Console.WriteLine("Received message from client: " + e.Data);

			// Look up the user's name and return their (new or established) id
			currentUser = UserDB.GetOrAddUserByName(e.Data);
			Send(BitConverter.GetBytes(currentUser.Id));
		}
	}
}
