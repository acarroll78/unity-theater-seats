using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using FlatBuffers;

public class TheaterSeatWebClient : MonoBehaviour
{
    public FilmDatabase FilmDB { get; private set; }
    public ShowDatabase ShowDB { get; private set; }
    public ReservationDatabase ReservationDB { get; private set; }
    private string webAddress = "ws://localhost:3278";
    private string loginRoute = "/Login";
    private string requestFilmsRoute = "/RequestFilms";
    private string requestShowsRoute = "/RequestShows";
    private string requestReservationsRoute = "/RequestReservations";
    private string reserveSeatRoute = "/ReserveSeat";
    private NativeWebSocket.WebSocket loginWebSocket = null;
    private NativeWebSocket.WebSocket requestFilmsSocket = null;
    private NativeWebSocket.WebSocket requestShowsSocket = null;
    private NativeWebSocket.WebSocket requestReservationsSocket = null;
    private NativeWebSocket.WebSocket reserveSeatSocket = null;

    private User currentUser;
    public string UserName { get { return currentUser.Name; } }
    public uint UserId { get { return currentUser.Id; } }
    public bool IsLoginReady { get; private set; }
    public bool HasLoggedIn { get; private set; }

	#region Message Responses
	public Action OnLoginSuccess { get; set; }
    public Action<UInt32, UInt32, UInt32> OnSeatReserved { get; set; } //< UserId, ShowId, Seat #
    #endregion

    // Start is called before the first frame update
    void Start ()
    {
        // DBs need to be setup immediately (with an empty List)
        ShowDB = new ShowDatabase();
        ReservationDB = new ReservationDatabase();
        FilmDB = new FilmDatabase();
        bool printDetails = false;

        // Initialize all the sockets and routes.
        loginWebSocket = new WebSocket(webAddress + loginRoute);
        requestFilmsSocket = new WebSocket(webAddress + requestFilmsRoute);
        requestShowsSocket = new WebSocket(webAddress + requestShowsRoute);
        requestReservationsSocket = new WebSocket(webAddress + requestReservationsRoute);
        reserveSeatSocket = new WebSocket(webAddress + reserveSeatRoute);

        // Request all of the basic data from the server before enabling login.
        requestReservationsSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Request Reservations response received from the server.");
            ByteBuffer buffer = new ByteBuffer(bytes);
            ReservationList reservationList = ReservationList.GetRootAsReservationList(buffer);
            int numReservations = reservationList.ReservationsLength;

            for (int i = 0; i < numReservations; ++i)
            {
                Reservation newReservation = reservationList.Reservations(i).Value;
                if (printDetails)
                {
                    Console.WriteLine("\tAdded Show: User Id ({0}), Show Id ({1}, Seat Id ({2})).", newReservation.UserId, newReservation.ShowId, newReservation.SeatId);
                }
                ReservationDB.AddReservation(newReservation);
            }

            requestReservationsSocket.Close();
        };

        // Reserve Seat logic, this is also where we will hear about new reservations from other users.
        reserveSeatSocket.OnOpen += () =>
        {
            Debug.Log("Connection open on " + webAddress + reserveSeatRoute);
        };
        reserveSeatSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed on " + webAddress + reserveSeatRoute);
        };
        reserveSeatSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Reserve Seat response received from the server.");
            ByteBuffer buffer = new ByteBuffer(bytes);
            Reservation newReservation = Reservation.GetRootAsReservation(buffer);

            if(printDetails)
			{
                Console.WriteLine("Added Reservation: User Id ({0}), Show Id ({1}), Seat Id ({2})", newReservation.UserId, newReservation.ShowId, newReservation.SeatId);
            }

            ReservationDB.AddReservation(newReservation);
            OnSeatReserved.Invoke(newReservation.UserId, newReservation.ShowId, newReservation.SeatId);
        };

        // Request for information on all films
        requestFilmsSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Request Films response received from the server.");
            ByteBuffer buffer = new ByteBuffer(bytes);
            FilmList filmList = FilmList.GetRootAsFilmList(buffer);
            int numFilms = filmList.FilmsLength;

            for(int i = 0; i < numFilms; ++i)
			{
                if(printDetails)
				{
                    Debug.Log("\tAdded Film: " + filmList.Films(i).Value.Name);
                }
                FilmDB.AddFilm(filmList.Films(i).Value);
			}

           requestFilmsSocket.Close();
        };

        // Request for information on all shows
        requestShowsSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Request Shows response received from the server.");
            ByteBuffer buffer = new ByteBuffer(bytes);
            ShowList showList = ShowList.GetRootAsShowList(buffer);
            int numShows = showList.ShowsLength;

            for (int i = 0; i < numShows; ++i)
            {
                Show newShow = showList.Shows(i).Value;
                if (printDetails)
                {
                    Console.WriteLine("\tAdded Show: Film Id ({0}), at ShowTime ({1}).", newShow.FilmId, newShow.ShowTime);
                }
                ShowDB.AddShow(newShow);
            }

            requestShowsSocket.Close();
        };

        // Login logic (this gets the id of the current user)
        loginWebSocket.OnOpen += () =>
        {
            Debug.Log("Connection open on " + webAddress + loginRoute);
            IsLoginReady = true;
        };
        loginWebSocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed on " + webAddress + loginRoute);
            IsLoginReady = false;
        };
        loginWebSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Reserve Seat response received from the server.");

            ByteBuffer buffer = new ByteBuffer(bytes);
            currentUser = User.GetRootAsUser(buffer);

            if(printDetails)
			{
                Debug.Log("\tUser ID: " + currentUser.Id + " with name: " + currentUser.Name);
            }
            HasLoggedIn = true;
            loginWebSocket.Close();
            OnLoginSuccess.Invoke();
        };

        reserveSeatSocket.Connect();
        requestReservationsSocket.Connect();
        requestFilmsSocket.Connect();
        requestShowsSocket.Connect();
        loginWebSocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        requestReservationsSocket?.DispatchMessageQueue();
        reserveSeatSocket?.DispatchMessageQueue();        
        requestFilmsSocket?.DispatchMessageQueue();
        requestShowsSocket?.DispatchMessageQueue();
        loginWebSocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
	{
        await loginWebSocket.Close();
        await requestFilmsSocket.Close();
        await requestShowsSocket.Close();
        await requestReservationsSocket.Close();
        await reserveSeatSocket.Close();
	}

    public async void Login(string name)
	{
        if(IsLoginReady && !HasLoggedIn)
		{
            await loginWebSocket.SendText(name);
        }
    }

    public async void ReserveSeat(UInt32 ShowId, UInt32 SeatId)
	{
        FlatBufferBuilder builder = new FlatBufferBuilder(256);
        Offset<Reservation> reservationOffset = Reservation.CreateReservation(builder, UserId, ShowId, SeatId);
        builder.Finish(reservationOffset.Value);
        byte[] bytes = builder.SizedByteArray();

        await reserveSeatSocket.Send(bytes);

    }
}
