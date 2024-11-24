using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Food.My.Client.Features.Recipes.Models;
using Unshackled.Studio.Core.Client.Extensions;
using Unshackled.Studio.Core.Client.Services;

namespace Unshackled.Food.My.Client.Features.Recipes;

public partial class DialogMakeRecipe
{
	[CascadingParameter] MudDialogInstance MudDialog { get; set; } = null!;
	[Inject] IScreenWakeLockService ScreenLockService { get; set; } = null!;

	[Parameter] public List<RecipeIngredientModel> Ingredients { get; set; } = [];
	[Parameter] public List<RecipeStepModel> Steps { get; set; } = [];
	[Parameter] public decimal Scale { get; set; }

	protected bool DisableBack => currentStepIndex <= 0 || Steps.Count == 0;
	protected bool DisableForward => currentStepIndex >= Steps.Count;
	protected bool CanScreenLock { get; set; }
	protected bool IsScreenLocked { get; set; }

	private int currentStepIndex = 0;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		CanScreenLock = await ScreenLockService.IsWakeLockSupported();
	}

	protected MarkupString GetIngredientListTitle(RecipeStepIngredientModel model)
	{
		var scaledValue = (model.Amount * Scale).ToHtmlFraction();
		var titleAndPrep = string.IsNullOrEmpty(model.PrepNote) ? model.Title : $"{model.Title}, {model.PrepNote}";
		return (MarkupString)$"{scaledValue} {model.AmountLabel} {titleAndPrep}";
	}

	public async Task HandleScreenLocked(bool value)
	{
		if (!CanScreenLock)
			return;

		if (value)
		{
			if (!ScreenLockService.HasWakeLock())
				await ScreenLockService.RequestWakeLock();
			IsScreenLocked = true;
		}
		else
		{
			await ScreenLockService.ReleaseWakeLock();
			IsScreenLocked = false;
		}
	}

	public void HandleSwipeEnd(SwipeEventArgs e)
	{
		// Swipe back
		if (e.SwipeDirection == SwipeDirection.LeftToRight)
		{
			if (!DisableBack)
			{
				currentStepIndex--;
				StateHasChanged();
			}
		}
		// Swipe forward
		else if (e.SwipeDirection == SwipeDirection.RightToLeft)
		{
			if (!DisableForward)
			{
				currentStepIndex++;
				StateHasChanged();
			}
		}
	}
}