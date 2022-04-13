using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnityEngine;

using NativeWebSocket;

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

    public string UserName { get; private set; }
    public uint UserId { get; private set; }
    public bool IsLoginReady { get; private set; }
    public bool HasLoggedIn { get; private set; }

	#region Message Responses
	public Action OnLoginSuccess { get; set; }
    public Action<UInt32, UInt32, UInt32> OnSeatReserved { get; set; } //< UserId, ShowId, Seat #
    #endregion

    // Start is called before the first frame update
    void Start ()
    {
        // This DB needs to be setup immediately (with an empty List)
        ReservationDB = new ReservationDatabase();

        // Initialize all the sockets and routes.
        loginWebSocket = new WebSocket(webAddress + loginRoute);
        requestFilmsSocket = new WebSocket(webAddress + requestFilmsRoute);
        requestShowsSocket = new WebSocket(webAddress + requestShowsRoute);
        requestReservationsSocket = new WebSocket(webAddress + requestReservationsRoute);
        reserveSeatSocket = new WebSocket(webAddress + reserveSeatRoute);

        // Request all of the basic data from the server before enabling login.
        requestReservationsSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Request Shows response received from the server. Data: " + bytes);
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };

            // This DB is added early and reservations are added sequentially unlike the other DBs as the server can send reservation data AT ANY TIME.
            Reservation[] Reservations = JsonSerializer.Deserialize<Reservation[]>(bytes, options);
            foreach (Reservation newRes in Reservations)
            {
                ReservationDB.AddReservation(newRes);
            }
            requestReservationsSocket.Close();
        };

        requestReservationsSocket.Connect();

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
            Debug.Log("Reserve seat response received from the server. Data: " + bytes);
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            Reservation newRes = JsonSerializer.Deserialize<Reservation>(bytes, options);
            ReservationDB.AddReservation(newRes);
            OnSeatReserved.Invoke(newRes.UserId, newRes.ShowId, newRes.SeatId);
        };

        reserveSeatSocket.Connect();

        // Request for information on all films
        requestFilmsSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Request Films response received from the server. Data: " + bytes);
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            Film[] Films = JsonSerializer.Deserialize<Film[]>(bytes, options);
            FilmDB = new FilmDatabase(Films);
            requestFilmsSocket.Close();
        };

        requestFilmsSocket.Connect();

        // Request for information on all shows
        requestShowsSocket.OnMessage += (bytes) =>
        {
            Debug.Log("Request Shows response received from the server. Data: " + bytes);
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            Show[] Shows = JsonSerializer.Deserialize<Show[]>(bytes, options);
            ShowDB = new ShowDatabase(Shows);
            requestShowsSocket.Close();
        };

        requestShowsSocket.Connect();

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
            UserId = BitConverter.ToUInt32(bytes, 0);
            Debug.Log("Login response received from the server. User ID: " + UserId);
            HasLoggedIn = true;
            loginWebSocket.Close();
            OnLoginSuccess.Invoke();
        };

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
            UserName = name;
            await loginWebSocket.SendText(name);
        }
    }

    public async void ReserveSeat(UInt32 ShowId, UInt32 SeatId)
	{
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = false,
        };

        Reservation res = new Reservation(UserId, ShowId, SeatId);
        string jsonString = JsonSerializer.Serialize(res, options);
        await reserveSeatSocket.SendText(jsonString);
	}
}
