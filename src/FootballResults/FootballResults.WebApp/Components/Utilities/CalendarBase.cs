using FootballResults.WebApp.Services.Time;
using Microsoft.AspNetCore.Components;

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
            int year = SelectedDate.Year;
            int month = SelectedDate.Month;
            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            List<DateTime[]> weeks = new List<DateTime[]>();
            DateTime[] week = new DateTime[7];

            for (int i = 0; i < 7; i++)
            {
                week[i] = startDate.AddDays(i);
            }

            // while first or last day of week is in the current month
            while (week[0].Month == month || week[week.Length - 1].Month == month)
            {
                weeks.Add(week);

                // add 7 days (1 week) to each day
                week = week.Select(day => day.AddDays(7)).ToArray();
            }

            Weeks = weeks;
        }

        protected bool IsToday(DateTime date)
        {
            return date.Date == DateTime.UtcNow.Add(ClientUtcDiff).Date;
        }

        protected bool IsSelected(DateTime date)
        {
            return date.Date == SelectedDate.Date;
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
