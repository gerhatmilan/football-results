using FootballResults.WebApp.Services.Time;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;

namespace FootballResults.WebApp.Components.Utilities
{
    public class CalendarBase : ComponentBase
    {
        private const int FIRST_YEAR = 2008;
        private DateTime _selectedDate;

        [Inject]
        protected IClientTimeService ClientTimeService { get; set; } = default!;

        [Parameter]
        public EventCallback<DateTime> SelectedDateValueChanged { get; set; }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    RefreshWeeks();
                    OnSelectedDateChanged(value);
                }
            }
        }

        protected TimeSpan ClientUtcDiff { get; set; }

        protected List<DateTime[]> Weeks { get; set; } = new List<DateTime[]>();

        protected override async Task OnInitializedAsync()
        {
            ClientUtcDiff = await ClientTimeService.GetClientUtcDiffAsync();
            // set selected date according to client's date
            SelectedDate = DateTime.UtcNow.Add(ClientUtcDiff);
        }

        private void OnSelectedDateChanged(DateTime newdate)
        {
            SelectedDateValueChanged.InvokeAsync(newdate);
        }

        protected void AddMonth(int months)
        {
            SelectedDate = SelectedDate.AddMonths(months);
            SelectedDate = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
        }

        protected void AddYear(int years)
        {
            SelectedDate = SelectedDate.AddYears(years);
            SelectedDate = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
        }

        protected void RefreshWeeks()
        {
            Weeks.Clear();

            int year = SelectedDate.Year;
            int month = SelectedDate.Month;
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            // go back until monday
            while (startDate.DayOfWeek != DayOfWeek.Monday)
            {
                startDate = startDate.AddDays(-1);
            }

            for (int weekCounter = 0; weekCounter < 6; weekCounter++)
            {
                DateTime[] week = new DateTime[7];

                for (int dayCounter = 0; dayCounter < 7; dayCounter++)
                {
                    week[dayCounter] = startDate.AddDays(weekCounter * 7 + dayCounter);
                }

                Weeks.Add(week);
            }
        }

        protected bool IsToday(DateTime date)
        {
            return date.Date == DateTime.UtcNow.Add(ClientUtcDiff).Date;
        }

        protected bool IsSelected(DateTime date)
        {
            return date.Date == SelectedDate.Date;
        }

        protected bool IsCurrentMonth(DateTime date)
        {
            return date.Month == SelectedDate.Month;
        }

        protected string GetClassForDay(DateTime date)
        {
            string returnValue = "";

            if (IsToday(date))
            {
                returnValue = "today";
            }
            else if (IsSelected(date))
            {
                returnValue = "selected";
            }
            else if (!IsCurrentMonth(date))
            {
                returnValue = "other-month";
            }

            return returnValue;
        }

        protected List<int> GetAvailableYears()
        {
            List<int> list = new List<int>();

            for (int i = FIRST_YEAR; i <= DateTime.Now.Year; i++)
            {
                list.Add(i);
            }

            return list;
        }
    }
}
