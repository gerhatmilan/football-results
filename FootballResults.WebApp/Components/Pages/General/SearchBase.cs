﻿using FootballResults.DataAccess.Entities.Football;
using FootballResults.DataAccess.Entities.Users;
using FootballResults.WebApp.Services.Football;
using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Pages.General
{
    public class SearchBase : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ILeagueService LeagueService { get; set; } = default!;

        [Inject]
        protected ITeamService TeamService { get; set; } = default!;

        [SupplyParameterFromQuery(Name = "value")]
        protected string? SearchValue { get; set; } = default!;

        [CascadingParameter(Name = "User")]
        protected User User { get; set; } = default!;

        protected IEnumerable<League>? Leagues { get; set; }
        protected IEnumerable<Team>? Teams { get; set; }

        protected bool IsLoading { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (!(String.IsNullOrEmpty(SearchValue) || IsLoading))
            {
                IsLoading = true; // needed because onparameterssetasync is called twice causing multiple operations at the same time on the db...
                Leagues = await LeagueService.SearchAsync(SearchValue);
                Teams = await TeamService.SearchAsync(SearchValue);
                IsLoading = false;
            }
        }
    }
}
