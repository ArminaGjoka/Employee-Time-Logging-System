using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using Timbratura_Testo.Models;
using MudBlazor;


namespace Timbratura_Testo.Components.Pages
{
    public partial class Administratore : ComponentBase
    {
       private readonly RoleManager<IdentityRole> _roleManager = default!;
        private List<ApplicationUser> userList = new List<ApplicationUser>();
        private List<Ruoli> listaRuoli = new List<Ruoli>();
        private List<SelectListItem> selectList = new List<SelectListItem>();
        private ApplicationUser? userInEdit;
        [Inject] private ISnackbar Snackbar { get; set; }
        Utenti nuovoUtente = new Utenti();

        protected override async Task OnInitializedAsync()
        {
            listaRuoli = new List<Ruoli>()
            {
                new Ruoli() { RoleName = "Admin", Id = "1" },
                new Ruoli() { RoleName = "Hr", Id = "2" },
                new Ruoli() { RoleName = "Manager", Id = "3" },
                new Ruoli() { RoleName = "User", Id = "4" },

            };


            foreach (var ruoli in listaRuoli)
            {
                selectList.Add(new SelectListItem()
                {
                    Text = ruoli.RoleName,
                    Value = ruoli.Id,
                    Selected = false,
                });
            }


            await LoadUserListAsync();
        }


        private async Task LoadUserListAsync()
        {
            userList = await DbContext.Users.ToListAsync();

            StateHasChanged();
        }


        public async Task AddUserWithRoleGiacomo()
        {
            var userModel = nuovoUtente;
            var user = await UserManager.FindByEmailAsync(userModel.LoginName);

            if (user is null)
            {
                user = await CreateUser(userModel);
            }

            if (user is null)
            {
                return;
            }

            var roleIds = userModel.UserRole.Split(',');

            foreach (var roleId in roleIds)
            {
                var dbRole = listaRuoli.FirstOrDefault(x => x.Id == roleId);
                if (dbRole is null)
                {
                    Snackbar.Add($"Ruolo con ID {roleId} non trovato.", Severity.Warning);
                    continue;
                }

                var userIsInRole = await UserManager.IsInRoleAsync(user, dbRole.RoleName);
                if (userIsInRole)
                {
                    Snackbar.Add($"L'utente {user.UserName} è già nel ruolo {dbRole.RoleName}.", Severity.Info);
                    continue;
                }

                var addRoleResult = await UserManager.AddToRoleAsync(user, dbRole.RoleName);
                if (!addRoleResult.Succeeded)
                {
                    Snackbar.Add($"Impossibile aggiungere l'utente {user.UserName} al ruolo {dbRole.RoleName}: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}", Severity.Error);
                    continue;
                }

                var addClaimResult = await UserManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, dbRole.RoleName));
                if (!addClaimResult.Succeeded)
                {
                    Snackbar.Add($"Impossibile aggiungere il claim per l'utente {user.UserName} al ruolo {dbRole.RoleName}: {string.Join(", ", addClaimResult.Errors.Select(e => e.Description))}", Severity.Error);
                    continue;
                }

                Snackbar.Add($"Utente {user.UserName} aggiunto al ruolo {dbRole.RoleName} con successo.", Severity.Success);
            }

            await LoadUserListAsync();
        }


        private async Task<ApplicationUser?> CreateUser(Utenti usermodel)
        {
            var user = new ApplicationUser
            {
                UserName = usermodel.LoginName,
                Email = usermodel.LoginName,
                EmailConfirmed = true
            };

            var createUserResult = await UserManager.CreateAsync(user, "123ABCd!");
            if (!createUserResult.Succeeded)
            {
                Snackbar.Add($"Impossibile creare l'utente {user.UserName}: {string.Join(", ", createUserResult.Errors.Select(e => e.Description))}", Severity.Error);
                return null;
            }

            Snackbar.Add($"Utente {user.UserName} creato con successo.", Severity.Success);
            return user;
        }


        //public async Task AddUserWithRole(EditContext context)
        //{
        //    var usermodel = context.Model as Utenti;
        //    var user = await UserManager.FindByEmailAsync(usermodel.LoginName);

        //    if (user == null)
        //    {

        //        user = new ApplicationUser
        //        {
        //            UserName = usermodel.LoginName,
        //            Email = usermodel.LoginName,
        //            EmailConfirmed = true
        //        };

        //        var createUserResult = await UserManager.CreateAsync(user, "123ABCd!");
        //        if (!createUserResult.Succeeded)
        //        {
        //            await _jsruntime.InvokeVoidAsync("alert", "Operazione fallita: Impossibile creare l'utente.");
        //            return;
        //        }
        //    }

        //    var messagePrompt = "Utente aggiornato";

        //    var roleIds = usermodel.UserRole.Split(',');
        //    foreach (var roleId in roleIds)
        //    {
        //        var gotRoleFromList = listaRuoli.FirstOrDefault(x => x.Id == roleId);
        //        if (gotRoleFromList != null)
        //        {
        //            var isInRole = await UserManager.IsInRoleAsync(user, gotRoleFromList.RoleName);
        //            if (!isInRole)
        //            {
        //                var roleAddedResult = await UserManager.AddToRoleAsync(user, gotRoleFromList.RoleName);
        //                if (roleAddedResult.Succeeded)
        //                {
        //                    messagePrompt += $" Ruolo {gotRoleFromList.RoleName} aggiunto";

        //                    var claimAddedResult = await UserManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, gotRoleFromList.RoleName));
        //                    if (claimAddedResult.Succeeded)
        //                    {
        //                        messagePrompt += $" Claim {gotRoleFromList.RoleName} aggiunto";
        //                    }
        //                }
        //                else
        //                {
        //                    messagePrompt += $" Impossibile aggiungere il ruolo {gotRoleFromList.RoleName}";
        //                }
        //            }
        //            else
        //            {
        //                messagePrompt += $" Utente già nel ruolo {gotRoleFromList.RoleName}";
        //            }
        //        }
        //    }

        //    await _jsruntime.InvokeVoidAsync("alert", messagePrompt);


        //    await LoadUserListAsync();
        //}


        private void StartEdit(ApplicationUser user)
        {
            userInEdit = user;
        }


        private async Task SaveEdit(ApplicationUser user)
        {
            var result = await UserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var messages = string.Join(Environment.NewLine, result.Errors.Select(x => x.Description));
                Snackbar.Add($"Errore in elaborazione:{Environment.NewLine} {messages}");
                await DisableEditAndReload();
                return;
            }

            await DbContext.SaveChangesAsync();
            Snackbar.Add("Utente aggiornato correttamente", Severity.Info);
            await DisableEditAndReload();
        }

        private async Task DisableEditAndReload()
        {
            userInEdit = null;
            await LoadUserListAsync();
        }
        
        private void CancelEdit()
        {
            userInEdit = null;
        }


        private async Task DeleteUser(ApplicationUser user)
        {
            var result = await UserManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                Snackbar.Add("Utente non eleminato", Severity.Success);
                return;  
            }
            Snackbar.Add("Utente eliminato con successo", Severity.Success);
            
            await LoadUserListAsync();
        }

        public void ViewRoles(ApplicationUser user)
        {
            NavigationManager.NavigateTo($"/roles/{user.Email}");
        }


    }
}
