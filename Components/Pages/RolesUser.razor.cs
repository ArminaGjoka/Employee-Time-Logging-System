using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Timbratura_Testo.Components.Pages
{
    public partial class RolesUser
    {
 
    [Parameter]
        public string? userName { get; set; }

        private List<string>? userRoles;
        private string? roleToAdd;
        private string? roleToRemove;

        protected override async Task OnParametersSetAsync()
        {
            await LoadUserRolesAsync();
        }

        private async Task LoadUserRolesAsync()
        {
            var user = await UserManager.FindByEmailAsync(userName);
            if (user != null)
            {
                userRoles = (await UserManager.GetRolesAsync(user)).ToList();
            }
        }

        private async Task AddRole()
        {
            var user = await UserManager.FindByEmailAsync(userName);
            if (user != null && !string.IsNullOrWhiteSpace(roleToAdd))
            {
                var result = await UserManager.AddToRoleAsync(user, roleToAdd);
                if (result.Succeeded)
                {
                    await _jsruntime.InvokeVoidAsync("alert", $"Ruolo '{roleToAdd}' aggiunto con successo.");
                    await LoadUserRolesAsync();
                    roleToAdd = "";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        await _jsruntime.InvokeVoidAsync("alert", $"Errore durante l'aggiunta del ruolo: {error.Description}");
                    }
                }
            }
        }

        private async Task RemoveRole()
        {
            var user = await UserManager.FindByEmailAsync(userName);
            if (user != null && !string.IsNullOrWhiteSpace(roleToRemove))
            {
                var result = await UserManager.RemoveFromRoleAsync(user, roleToRemove);
                if (result.Succeeded)
                {
                    await _jsruntime.InvokeVoidAsync("alert", $"Ruolo '{roleToRemove}' rimosso con successo.");
                    await LoadUserRolesAsync();
                    roleToRemove = "";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        await _jsruntime.InvokeVoidAsync("alert", $"Errore durante la rimozione del ruolo: {error.Description}");
                    }
                }
            }
        }
    }

}

