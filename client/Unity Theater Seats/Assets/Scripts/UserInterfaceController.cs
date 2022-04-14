using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Controls all of the logic around the User Interface in the app
public class UserInterfaceController : MonoBehaviour
{
    private enum ScreenState
	{
        Login,
        ReserveSeat,
        None
	};

    [SerializeField]
    private VisualTreeAsset SeatTemplate;

    public TheaterSeatWebClient webClient;
    private ScreenState CurrentScreenState = ScreenState.None;

    public UIDocument UserInterfaceDoc;
    private VisualElement Root;

	#region Header Elements
	private VisualElement HeaderRoot;
    private Label UserNameLabel;
    #endregion

    #region Login Elements
    private VisualElement LoginRoot;
    private TextField NameEntryTextField;
    private Button LoginButton;
	#endregion

	#region ReserveSeat Elements
	private VisualElement ReserveSeatRoot;
    private VisualElement SelectDateRoot;
    private RadioButtonGroup DateButtons;
    private VisualElement SelectFilmRoot;
    private RadioButtonGroup FilmButtons;
    private VisualElement SelectShowRoot;
    private RadioButtonGroup ShowButtons;
    private VisualElement SelectSeatRoot;
    private VisualElement SeatingChartRoot;
    private VisualElement SeatContainer;
    #endregion

    private List<TemplateContainer> Seats = new List<TemplateContainer>();

    // Current selections
    private DateTime selectedDate;
    private UInt32 selectedFilm;
    private UInt32 selectedShow;
    private UInt32 numSeats = 60; //< Hard-coded for now.

    // Start is called before the first frame update
    void Start()
    {
        // Header
        Root = UserInterfaceDoc.rootVisualElement;
        HeaderRoot = Root.Query<VisualElement>("Header");
        UserNameLabel = HeaderRoot.Query<Label>("UserNameLabel");

        // Login
        LoginRoot = Root.Query<VisualElement>("Login");
        NameEntryTextField = LoginRoot.Query<TextField>("NameEntryTextField");
        LoginButton = LoginRoot.Query<Button>("LoginButton");
        LoginButton.clicked += HandleLoginButtonClicked;
        webClient.OnLoginSuccess += HandleLoginSuccess;

        // Seat Reservation
        ReserveSeatRoot = Root.Query<VisualElement>("ReserveSeat");
        SelectDateRoot = Root.Query<VisualElement>("SelectDate");
        DateButtons = SelectDateRoot.Query<RadioButtonGroup>("DateButtons");
        SelectFilmRoot = Root.Query<VisualElement>("SelectFilm");
        FilmButtons = SelectFilmRoot.Query<RadioButtonGroup>("FilmButtons");
        SelectShowRoot = Root.Query<VisualElement>("SelectShow");
        ShowButtons = SelectShowRoot.Query<RadioButtonGroup>("ShowButtons");
        SelectSeatRoot = Root.Query<VisualElement>("SelectSeat");
        SeatingChartRoot = SelectSeatRoot.Query<VisualElement>("SeatingChartRoot");
        SeatContainer = SeatingChartRoot.Query<VisualElement>("SeatContainer");

        // Hardcoded 60 seats per theater, would ideally make the theater layout data driven 
        for (UInt32 i = 1; i <= numSeats; ++i)
        {
            TemplateContainer newSeat = SeatTemplate.Instantiate();
            Button seatSelectButton = newSeat.Query<Button>("SelectButton");
            SeatContainer.Add(newSeat);
            Seats.Add(newSeat);

            // Workaround for sharing the same ID.
            UInt32 x = i;
            seatSelectButton.clicked += () =>
            {
                HandleSeatReserveClick(x);
            };
        }

        webClient.OnSeatReserved += HandleSeatReserved;

        LoginRoot.style.display = DisplayStyle.None;
        ReserveSeatRoot.style.display = DisplayStyle.None;

        SetScreenState(ScreenState.Login);
    }

    void Update()
	{
        LoginButton.SetEnabled(NameEntryTextField.value.Length > 0 && webClient != null && webClient.IsLoginReady);
	}

    private void SetScreenState(ScreenState NewState)
	{
        if(CurrentScreenState != NewState)
		{
            if (CurrentScreenState == ScreenState.Login)
            {
                // Disable the old
                LoginRoot.style.display = DisplayStyle.None;
            }
            else if(CurrentScreenState == ScreenState.ReserveSeat)
			{
                ReserveSeatRoot.style.display = DisplayStyle.None;
			}

            switch(NewState)
			{
                case ScreenState.Login:
				{
                    LoginRoot.style.display = DisplayStyle.Flex;
                    break;
				}
                case ScreenState.ReserveSeat:
				{
                    ReserveSeatRoot.style.display = DisplayStyle.Flex;
                    PopulateDateButtons();

                    break;
				}
			}
            CurrentScreenState = NewState;
        }
    }

    private void HandleLoginButtonClicked()
	{
        webClient.Login(NameEntryTextField.value);
	}

    private void HandleReserveButtonClicked()
	{
        // UInt32 ShowId, UInt32 SeatId
        webClient.ReserveSeat(1, 34);
	}

    private void HandleLoginSuccess()
	{
        UserNameLabel.text = webClient.UserName;
        SetScreenState(ScreenState.ReserveSeat);
	}

    //< UserId, ShowId, Seat #
    private void HandleSeatReserved(UInt32 UserId, UInt32 ShowId, UInt32 SeatId)
	{
        Debug.Log("UserInterface: handling new reservation for Show ID (" + ShowId + "), User ID (" + UserId + ") and Seat ID(" + SeatId + ")");
        ReserveSeatOnMap(ShowId, UserId, SeatId);
    }

    private void HandleSeatReserveClick(UInt32 SeatId)
	{
        Debug.Log("Seat clicked: ID(" + SeatId + ")");
        webClient.ReserveSeat(selectedShow, SeatId);
    }

    private void PopulateDateButtons()
	{
        DateButtons.Clear();

        RadioButton firstButton = null;
        foreach (DateTime showTime in webClient.ShowDB.GetAllShowDates())
		{
            RadioButton newButton = new RadioButton() { text = (showTime.Month.ToString() + "/" + showTime.Day.ToString() + "/" + showTime.Year.ToString()) };
            newButton.RegisterValueChangedCallback((evt) => { OnDateChanged(evt, showTime.Month, showTime.Day, showTime.Year); });
            DateButtons.Add(newButton);

            if(firstButton == null)
			{
                firstButton = newButton;
			}
        }

        if(firstButton != null)
		{
            firstButton.SetSelected(true);
        }
    }

    private void PopulateFilmButtons()
    {
        FilmButtons.Clear();

        RadioButton firstButton = null;

        foreach (UInt32 filmId in webClient.ShowDB.GetFilmIdsForDate(selectedDate))
        {
            Film film = webClient.FilmDB.GetFilmById(filmId);
            RadioButton newButton = new RadioButton() { text = film.Name };
            newButton.RegisterValueChangedCallback((evt) => { OnFilmChanged(evt, filmId); });
            FilmButtons.Add(newButton);

            if (firstButton == null)
            {
                firstButton = newButton;
            }
        }

        if (firstButton != null)
        {
            firstButton.SetSelected(true);
        }
    }

    private void PopulateShowButtons()
    {
        ShowButtons.Clear();

        RadioButton firstButton = null;

        foreach (Show show in webClient.ShowDB.GetShowsForFilmAndDate(selectedFilm, selectedDate))
        {
            DateTime showTime = new DateTime(show.ShowTime);
            RadioButton newButton = new RadioButton() { text = showTime.ToString("h:mm tt") };
            newButton.RegisterValueChangedCallback((evt) => { OnShowChanged(evt, show.Id); });
            ShowButtons.Add(newButton);

            if (firstButton == null)
            {
                firstButton = newButton;
            }
        }

        if (firstButton != null)
        {
            firstButton.SetSelected(true);
        }
    }

    private void PopulateSeatMap()
	{
        // This is a brute force method of drawing the seat map. Would likely change this to only update changes in many situations.
        List<Reservation> reservations = webClient.ReservationDB.GetReservationsByShowId(selectedShow);

        for (UInt32 i = 1; i <= numSeats; ++i)
		{
            bool isReserved = false;
            Reservation resForSeat = new Reservation();
            foreach (Reservation res in reservations)
			{
                if(res.SeatId == i)
				{
                    resForSeat = res;
                    isReserved = true;
				}
			}
            Button SeatButton = Seats[(int)i - 1].Query<Button>("SelectButton");
            VisualElement SeatIcon = Seats[(int)i - 1].Query<VisualElement>("Icon");

            if (isReserved)
			{
                // This seat is reserved.
                if(resForSeat.UserId == webClient.UserId)
				{
                    SeatIcon.style.unityBackgroundImageTintColor = Color.cyan;
				}
                else
				{
                    SeatIcon.style.unityBackgroundImageTintColor = Color.gray;
                }

                SeatButton.SetEnabled(false);
            }
            else
			{
                // Not reserved. Enable and return styling to normal.
                SeatIcon.style.unityBackgroundImageTintColor = Color.white;
                SeatButton.SetEnabled(true);
			}
		}
	}

    void ReserveSeatOnMap(UInt32 ShowId, UInt32 UserId, UInt32 SeatId)
    {
        if (selectedShow == ShowId)
        {
            Button SeatButton = Seats[(int)SeatId - 1].Query<Button>("SelectButton");
            VisualElement SeatIcon = Seats[(int)SeatId - 1].Query<VisualElement>("Icon");

            // This seat is reserved.
            if (UserId == webClient.UserId)
            {
                SeatIcon.style.unityBackgroundImageTintColor = Color.cyan;
            }
            else
            {
                SeatIcon.style.unityBackgroundImageTintColor = Color.gray;
            }

            SeatButton.SetEnabled(false);
        }
    }

    void OnDateChanged(ChangeEvent<bool> evt, int month, int day, int year)
    {
        if(evt.newValue == true)
		{
            Debug.Log("Date Selected: " + month + "/" + day + "/" + year);
            selectedDate = new DateTime(year, month, day);

            // Update available films for this date.
            PopulateFilmButtons();
        }
    }

    void OnFilmChanged(ChangeEvent<bool> evt, UInt32 FilmId)
    {
        if (evt.newValue == true)
        {
            Debug.Log("Film Selected: " + FilmId);
            selectedFilm = FilmId;

            // Update available showtimes for this date.
            PopulateShowButtons();
        }
    }

    void OnShowChanged(ChangeEvent<bool> evt, UInt32 ShowId)
    {
        if(evt.newValue == true)
		{
            Debug.Log("Show Selected:" + ShowId);
            selectedShow = ShowId;

            // Update the seat map.
            PopulateSeatMap();
        }
    }
}
