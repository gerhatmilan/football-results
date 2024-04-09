using Microsoft.AspNetCore.Components;

namespace FootballResults.WebApp.Components.Forms
{
    public abstract class FormBase : ComponentBase
    {
        protected bool ButtonDisabled { get; set; } = false;
        protected virtual void ResetErrorMessages() { }

        protected virtual void DisableForm()
        {
            ButtonDisabled = true;
            StateHasChanged();
        }

        protected virtual async Task EnableForm()
        {
            await Task.Delay(1000);
            ButtonDisabled = false;
            StateHasChanged();
        }
    }
}
