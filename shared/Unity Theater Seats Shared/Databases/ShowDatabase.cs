using System;
using System.Collections.Generic;
using System.Linq;

public class ShowDatabase
{
	private Dictionary<UInt32, Show> Shows = new Dictionary<UInt32, Show>(); //< Keyed by ShowId.

	public ShowDatabase()
	{

	}

	public ShowDatabase(Show[] ActiveShows)
	{
		foreach (Show activeShow in ActiveShows)
		{
			Shows.Add(activeShow.Id, activeShow);
		}
	}

	// Returns null if not found.
	public Show GetShowById(UInt32 Id)
	{
		return Shows.ContainsKey(Id) ? Shows[Id] : new Show();
	}

	public DateTime[] GetAllShowDates()
	{
		List<DateTime> showDates = new List<DateTime>();
		foreach(Show show in Shows.Values)
		{
			DateTime tempDate = new DateTime(show.ShowTime);
			DateTime modifiedDate = new DateTime(tempDate.Year, tempDate.Month, tempDate.Day);
			if(!showDates.Contains(modifiedDate))
			{
				showDates.Add(modifiedDate);
			}
		}
		showDates.Sort();
		return showDates.ToArray();
	}

	public Show[] GetAllShows()
	{
		return Shows.Values.ToArray();
	}

	public UInt32[] GetFilmIdsForDate(DateTime Date)
	{
		SortedSet<UInt32> films = new SortedSet<UInt32>();
		foreach (Show show in Shows.Values)
		{
			DateTime showDate = new DateTime(show.ShowTime);
			if( showDate.Year == Date.Year &&
				showDate.Month == Date.Month &&
				showDate.Day == Date.Day)
			{
				films.Add(show.FilmId);
			}
		}

		return films.ToArray();
	}

	public Show[] GetShowsForFilmAndDate(UInt32 FilmId, DateTime Date)
	{
		List<Show> shows = new List<Show>();
		foreach (Show show in Shows.Values)
		{
			DateTime showDate = new DateTime(show.ShowTime);
			if (showDate.Year == Date.Year &&
				showDate.Month == Date.Month &&
				showDate.Day == Date.Day && 
				show.FilmId == FilmId)
			{
				shows.Add(show);
			}
		}

		shows.Sort(delegate (Show t1, Show t2) { return t1.ShowTime.CompareTo(t2.ShowTime); });

		return shows.ToArray();
	}

	public void AddShow(Show NewShow)
	{
		if (!Shows.ContainsKey(NewShow.Id))
		{
			Shows.Add(NewShow.Id, NewShow);
		}
	}

}
