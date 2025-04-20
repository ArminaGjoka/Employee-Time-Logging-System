using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using Timbratura_Testo.Models;
using Timbratura_Testo.Services;

namespace Timbratura_Testo.Components.Pages
{
    public partial class Test
    {
        [Inject] private TimeRounding TimeRounding { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private List<Timbratura> _timbratureList = new List<Timbratura>();
        private ApplicationUser _user { get; set; }= default!;
        private bool _IsLoading { get; set; }
        private bool _isIngressoClicked = false;
        private bool _isUscitaClicked = false;

        protected override async Task OnInitializedAsync()
        {
          
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var _userPrincipal = authState.User;

            if (_userPrincipal.Identity.IsAuthenticated)
            {
                var userId = UserManager.GetUserId(_userPrincipal);
                _user = await UserManager.FindByIdAsync(userId);

                await LoadTimbratureAsync();
            }
        }

        private async Task LoadTimbratureAsync()
        {
            _timbratureList = await DbContext.Timbrature
                .Where(t => t.UserId == _user.Id && t.EntryDate.Date == DateTime.Today)
                .ToListAsync();
        }


        private async Task TimbrareIngresso()
        {
            _IsLoading = true;

            try
            {
                var oraIngresso = DateTime.Now;
                var roundedEntryTime = TimeRounding.RoundTime(oraIngresso);

                var timbratura = new Timbratura
                {
                    UserId = _user.Id,
                    EntryDate = oraIngresso.Date,
                    EntryTime = oraIngresso,
                    RoundedEntryTime = roundedEntryTime,
                    ExitTime = null,
                    Duration = null
                };

                DbContext.Timbrature.Add(timbratura);
                await DbContext.SaveChangesAsync();

                _timbratureList.Add(timbratura);


                _isIngressoClicked = true;
                _isUscitaClicked = false;

                StateHasChanged();
            }
            finally
            {
                _IsLoading = false;
            }
        }

        private async Task TimbrareUscita()
        {
            _IsLoading = true;

            try
            {
                var oraUscita = DateTime.Now;
                var roundedExitTime = TimeRounding.RoundTime(oraUscita);

                var ultimaTimbraturaIngresso = _timbratureList.LastOrDefault(t => t.ExitTime == null && t.UserId == _user.Id);

                if (ultimaTimbraturaIngresso != null)
                {
                    ultimaTimbraturaIngresso.ExitTime = oraUscita;
                    ultimaTimbraturaIngresso.RoundedExitTime = roundedExitTime;
                    ultimaTimbraturaIngresso.Duration = (int)(oraUscita - ultimaTimbraturaIngresso.EntryTime).TotalMinutes;

                    DbContext.Timbrature.Update(ultimaTimbraturaIngresso);
                    await DbContext.SaveChangesAsync();


                    _isUscitaClicked = true;
                    _isIngressoClicked = false;

                    StateHasChanged();
                }
            }
            finally
            {
                _IsLoading = false;
            }
        }

        private void NavigateToTimbratureCalendar()  //metodo per passare i dati sul'altra pagina razor
        {

            NavigationManager.NavigateTo("/timbrature-calendar");
        }
    }
}
