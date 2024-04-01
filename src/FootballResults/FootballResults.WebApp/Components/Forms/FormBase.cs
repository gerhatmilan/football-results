using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Forms
{
    public abstract class FormBase : ComponentBase
    {
        protected bool ButtonDisabled { get; set; } = false;
        protected abstract void ResetErrorMessages();

        protected void DisableForm()
        {
            ButtonDisabled = true;
            StateHasChanged();
        }

        protected async Task EnableForm()
        {
            await Task.Delay(1000).ContinueWith(_ => ButtonDisabled = false);
            ButtonDisabled = false;
            StateHasChanged();
        }
    }
}
