using MudBlazor;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Components;
using Unshackled.Fitness.My.Client.Features.Programs.Actions;
using Unshackled.Fitness.My.Client.Features.Programs.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs;

public class IndexBase : BaseSearchComponent<SearchProgramModel, ProgramListModel>
{
	protected FormAddProgramModel FormAddModel { get; set; } = new();
	protected FormAddProgramModel.Validator FormValidator { get; set; } = new();
	protected string DrawerIcon => Icons.Material.Filled.AddCircle;
	protected bool DrawerOpen { get; set; } = false;
	protected string DrawerTitle => "Add New Program";

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		SearchKey = "SearchPrograms";

		Breadcrumbs.Add(new BreadcrumbItem("Programs", null, true));

		SearchModel = await GetLocalSetting(SearchKey) ?? new();

		await DoSearch(SearchModel.Page);
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		await SaveLocalSetting(SearchKey, SearchModel);
		SearchResults = await Mediator.Send(new SearchPrograms.Query(SearchModel));
		IsLoading = false;
	}

	protected void HandleAddClicked()
	{
		FormAddModel = new()
		{
			ProgramType = ProgramTypes.FixedRepeating
		};
		DrawerOpen = true;
	}

	protected void HandleCancelAddClicked()
	{
		DrawerOpen = false;
	}

	protected async Task HandleFormSubmitted()
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddProgram.Command(FormAddModel));
		ShowNotification(result);
		if (result.Success)
		{
			NavManager.NavigateTo($"/programs/{result.Payload}");
		}
		DrawerOpen = false;
		IsWorking = false;
	}
}