using System;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Server;
using FlatBuffers;

namespace Unity_Theater_Seats_Server
{
	class RequestShowsBehavior: WebSocketBehavior
	{
		private ShowDatabase ShowDB;

		public void Setup(ShowDatabase ShowDB)
		{
			this.ShowDB = ShowDB;
		}
		protected override void OnOpen()
		{
			Console.WriteLine("/RequestShows Connection established.");

			var builder = new FlatBufferBuilder(4096);

			Show[] existingShows = ShowDB.GetAllShows();
			if (existingShows.Length > 0)
			{
				Offset<Show>[] showOffsets = new Offset<Show>[existingShows.Length];
				for (int i = 0; i < existingShows.Length; ++i)
				{
					var newShowOffset = Show.CreateShow(builder, existingShows[i].Id, existingShows[i].FilmId, existingShows[i].ShowTime);
					showOffsets[i] = newShowOffset;
				}

				FlatBuffers.VectorOffset showsVectorOffset = ShowList.CreateShowsVector(builder, showOffsets);
				ShowList.StartShowList(builder);
				ShowList.AddShows(builder, showsVectorOffset);
				var showList = ShowList.EndShowList(builder);
				builder.Finish(showList.Value);
				byte[] buffer = builder.SizedByteArray();

				// Console.WriteLine("/RequestShows Returning flat buffer containing a ShowList with {0} Shows.", existingShows.Length);
				Send(buffer);
			}
		}

		protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine("/RequestShows Connection closed.");
		}
	}
}
