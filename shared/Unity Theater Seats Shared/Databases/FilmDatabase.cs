using System;
using System.Collections.Generic;

public class FilmDatabase
{
	private Dictionary<UInt32, Film> Films; //< Keyed by FilmId.

	public FilmDatabase(Film[] ActiveFilms)
	{
		Films = new Dictionary<UInt32, Film>();

		foreach (Film activeFilm in ActiveFilms)
		{
			Films.Add(activeFilm.Id, activeFilm);
		}
	}

	// Returns null if not found.
	public Film GetFilmById(UInt32 Id)
	{
		return Films.ContainsKey(Id) ? Films[Id] : null;
	}
}
