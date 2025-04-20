using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Timbratura_Testo.Models;
using Timbratura_Testo.Services;

namespace Timbratura_Testo.Components.Pages
{
    public partial class TimbratureCalendar
    {

        private ApplicationUser _user { get; set; } = default!;

        [Parameter]
        public List<Timbratura> TimbratureList { get; set; } = new List<Timbratura>(); // questa lista la prende dal'altra razor

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var userPrincipal = authState.User;
            var user = await UserManager.GetUserAsync(userPrincipal);
            if (user is null)
            {
                return;
            }

            _user = user;
            await LoadTimbratureAsync();
            StateHasChanged();
        }

        private async Task LoadTimbratureAsync()
        {
            TimbratureList = await DbContext.Timbrature
                .Where(t => t.UserId == _user.Id)
                //.Take(10)
                //.OrderByDescending(x => x.EntryTime)
                .ToListAsync();
                 StateHasChanged();
        }
    }
}
