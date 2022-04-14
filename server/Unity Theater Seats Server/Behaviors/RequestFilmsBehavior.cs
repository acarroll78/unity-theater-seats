using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using FlatBuffers;

namespace Unity_Theater_Seats_Server
{
	public class RequestFilmsBehavior : WebSocketBehavior
	{
		private FilmDatabase FilmDB;

		public void Setup(FilmDatabase FilmDB)
		{
			this.FilmDB = FilmDB;
		}

		protected override void OnOpen()
		{
			Console.WriteLine("/RequestFilms Connection established.");

			var builder = new FlatBufferBuilder(2048);

			Film[] existingFilms = FilmDB.GetAllFilms();
			if(existingFilms.Length > 0)
			{
				Offset<Film>[] filmOffsets = new Offset<Film>[existingFilms.Length];
				for (int i = 0; i < existingFilms.Length; ++i)
				{
					var newFilmNameOffset = builder.CreateString(existingFilms[i].Name);
					var newFilmOffset = Film.CreateFilm(builder, newFilmNameOffset, existingFilms[i].Id);
					filmOffsets[i] = newFilmOffset;
				}

				FlatBuffers.VectorOffset filmsVectorOffset = FilmList.CreateFilmsVector(builder, filmOffsets);
				FilmList.StartFilmList(builder);
				FilmList.AddFilms(builder, filmsVectorOffset);
				var filmList = FilmList.EndFilmList(builder);
				builder.Finish(filmList.Value);
				byte[] buffer = builder.SizedByteArray();

				// Console.WriteLine("/RequestFilms Returning flat buffer containing a FilmList with {0} Films.", existingFilms.Length);
				Send(buffer);
			}
		}

        protected override void OnClose(CloseEventArgs e)
		{
			Console.WriteLine("/RequestFilms Connection closed.");
		}
	}
}
