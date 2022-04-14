using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using FlatBuffers;

namespace Unity_Theater_Seats_Server
{
	// Handles login messages sent from client.
	// Expects: string user name
	// Returns: int userId
	public class LoginBehavior : WebSocketBehavior
	{
		private UserDatabase UserDB;
		private User currentUser;

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
			Console.WriteLine("/Login Connection closed");
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			Console.WriteLine("/Login Received message from client.");

			// Look up the user's name and return their (new or established) id
			currentUser = UserDB.GetOrAddUserByName(e.Data);

			FlatBufferBuilder builder = new FlatBufferBuilder(256);
			StringOffset nameOffset = builder.CreateString(currentUser.Name);
			Offset<User> newUserOffset = User.CreateUser(builder, nameOffset, currentUser.Id);
			builder.Finish(newUserOffset.Value);
			byte[] bytes = builder.SizedByteArray();

			Send(bytes);
		}
	}
}
