using System;
using System.Collections.Generic;
using FlatBuffers;

public class FilmDatabase
{
	private Dictionary<UInt32, Film> Films = new Dictionary<UInt32, Film>(); //< Keyed by FilmId.

	public FilmDatabase()
	{

	}

	public FilmDatabase(Film[] ActiveFilms)
	{
		foreach (Film activeFilm in ActiveFilms)
		{
			AddFilm(activeFilm);
		}
	}

	public Film[] GetAllFilms()
	{
		List<Film> filmList = new List<Film>();
		foreach (Film existingFilm in Films.Values)
		{
			filmList.Add(existingFilm);
		}
		return filmList.ToArray();
	}

	// Returns default initialized Film if not found.
	public Film GetFilmById(UInt32 Id)
	{
		return Films.ContainsKey(Id) ? Films[Id] : new Film();
	}

	public void AddFilm(Film NewFilm)
	{
		if(!Films.ContainsKey(NewFilm.Id))
		{
			Films.Add(NewFilm.Id, NewFilm);
		}
	}
}
