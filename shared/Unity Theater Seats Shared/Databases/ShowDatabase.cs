using System;
using System.Collections.Generic;
using System.Linq;

public class ShowDatabase
{
	private Dictionary<UInt32, Show> Shows; //< Keyed by ShowId.

	public ShowDatabase(Show[] ActiveShows)
	{
		Shows = new Dictionary<UInt32, Show>();

		foreach (Show activeShow in ActiveShows)
		{
			Shows.Add(activeShow.Id, activeShow);
		}
	}

	// Returns null if not found.
	public Show GetShowById(UInt32 Id)
	{
		return Shows.ContainsKey(Id) ? Shows[Id] : null;
	}

	public DateTime[] GetAllShowDates()
	{
		SortedSet<DateTime> showDates = new SortedSet<DateTime>();
		foreach(Show show in Shows.Values)
		{
			DateTime date = new DateTime(show.ShowTime.Year, show.ShowTime.Month, show.ShowTime.Day);

			if (!showDates.Contains(date))
			{
				showDates.Add(date);
			}
		}

		return showDates.ToArray();
	}

	public UInt32[] GetFilmIdsForDate(DateTime Date)
	{
		SortedSet<UInt32> films = new SortedSet<UInt32>();
		foreach (Show show in Shows.Values)
		{
			if( show.ShowTime.Year == Date.Year &&
				show.ShowTime.Month == Date.Month &&
				show.ShowTime.Day == Date.Day)
			{
				films.Add(show.FilmId);
			}
		}

		return films.ToArray();
	}

	public Show[] GetShowsForFilmAndDate(UInt32 FilmId, DateTime Date)
	{
		SortedSet<Show> shows = new SortedSet<Show>();
		foreach (Show show in Shows.Values)
		{
			if (show.ShowTime.Year == Date.Year &&
				show.ShowTime.Month == Date.Month &&
				show.ShowTime.Day == Date.Day && 
				show.FilmId == FilmId)
			{
				shows.Add(show);
			}
		}

		return shows.ToArray();
	}
}
