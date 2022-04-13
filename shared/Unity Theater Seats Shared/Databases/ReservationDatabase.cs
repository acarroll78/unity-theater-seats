using System;
using System.Collections.Generic;

public class ReservationDatabase
{
	private List<Reservation> Reservations = new List<Reservation>();

	public ReservationDatabase()
	{

	}

	public ReservationDatabase(Reservation[] ActiveReservations)
	{
		foreach (Reservation res in ActiveReservations)
		{
			AddReservation(res);
		}
	}

	// Returns empty list if none found.
	public List<Reservation> GetReservationsByShowId(UInt32 ShowId)
	{
		List<Reservation> result = new List<Reservation>();
		foreach (Reservation activeReservation in Reservations)
		{
			if (activeReservation.ShowId == ShowId)
			{
				result.Add(activeReservation);
			}
		}

		return result;
	}

	public Reservation[] GetAllReservations()
	{
		return Reservations.ToArray();
	}

	public bool AddReservation(Reservation NewReservation)
	{
		// Add these uniquely; this is super brute force at the moment.
		foreach (Reservation reservation in Reservations)
		{
			if (reservation == NewReservation)
			{
				return false;
			}
		}
		Reservations.Add(NewReservation);
		return true;
	}
}
