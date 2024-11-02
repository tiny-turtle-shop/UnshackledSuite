﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Metrics.Actions;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Metrics;

public class DefinitionsBase : BaseComponent
{
	protected enum Views
	{
		List = 0,
		Adding = 1,
		Editing = 2,
		Deleting = 3
	}

	[Inject] protected IDialogService DialogService { get; set; } = default!;
	protected string AddingGroupSid { get; set; } = string.Empty;
	protected string? DeletingConfirmation { get; set; }
	protected FormMetricDefinitionModel? DeletingDefinition { get; set; }
	protected FormMetricDefinitionModel EditingDefinition { get; set; } = new();
	protected MudColor? EditingColor { get; set; }
	protected bool IsLoading { get; set; } = false;
	protected bool IsWorking { get; set; } = false;
	protected MetricListModel ListModel { get; set; } = new();
	protected List<FormMetricDefinitionGroupModel> DeletedGroups { get; set; } = new();
	protected Views ShowView { get; set; } = Views.List;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Metric Definitions", null, true));

		await ListMetricDefinitions();
	}

	protected void HandleAddToGroupClicked(string groupSid)
	{
		AddingGroupSid = groupSid;
		ShowView = Views.Adding;
	}

	protected void HandleAddMetricClicked(MetricTypes metricType)
	{
		EditingDefinition = new()
		{
			MetricType = metricType,
			ListGroupSid = AddingGroupSid,
			Title = "My Metric",
			MaxValue = metricType == MetricTypes.Range ? 5 : 0
		};
		EditingColor = null;
		ShowView = Views.Editing;
	}

	protected async Task HandleConfirmDelete()
	{
		if (DeletingDefinition != null && DeletingDefinition.Title == DeletingConfirmation)
		{
			IsWorking = true;
			var result = await Mediator.Send(new DeleteDefinition.Command(DeletingDefinition.Sid));
			ShowNotification(result);
			if (result.Success)
			{
				// Remove the definition from the list
				ListModel.Metrics.Remove(DeletingDefinition);

				// If no metrics left in the group
				if (!ListModel.Metrics.Where(x => x.ListGroupSid == DeletingDefinition.ListGroupSid).Any())
				{
					// Remove the group
					var group = ListModel.Groups.Where(x => x.Sid == DeletingDefinition.ListGroupSid).SingleOrDefault();
					if (group != null)
						ListModel.Groups.Remove(group);
				}
			}
			DeletingDefinition = null;
			ShowView = Views.List;
			IsWorking = false;
		}
	}

	protected void HandleDeleteClicked(FormMetricDefinitionModel model)
	{
		DeletingConfirmation = null;
		DeletingDefinition = model;
		ShowView = Views.Deleting;
	}

	public void HandleEditClicked(FormMetricDefinitionModel model)
	{
		EditingDefinition = new()
		{
			DateCreatedUtc = model.DateCreatedUtc,
			DateLastModifiedUtc = model.DateLastModifiedUtc,
			ListGroupSid = model.ListGroupSid,
			HighlightColor = model.HighlightColor,
			IsArchived = model.IsArchived,
			IsOnDashboard = model.IsOnDashboard,
			MaxValue = model.MaxValue,
			MemberSid = model.MemberSid,
			MetricType = model.MetricType,
			Sid = model.Sid,
			SortOrder = model.SortOrder,
			SubTitle = model.SubTitle,
			Title = model.Title
		};
		ShowView = Views.Editing;
	}

	public void HandleEditingColorChanged(MudColor color)
	{
		EditingColor = color;
		EditingDefinition.HighlightColor = color.Value;
	}

	public async Task HandleFormSubmit()
	{
		IsWorking = true;
		var result = await Mediator.Send(new SaveDefinition.Command(EditingDefinition));
		ShowNotification(result);
		if (result.Success)
		{
			ShowView = Views.List;
			await ListMetricDefinitions();
		}
		IsWorking = false;
	}

	protected async Task HandleSortChanged(SortableGroupResult<FormMetricDefinitionGroupModel, FormMetricDefinitionModel> sortResult)
	{
		IsWorking = true;

		UpdateSortModel model = new()
		{
			DeletedGroups = sortResult.DeletedGroups,
			Groups = sortResult.Groups,
			Metrics = sortResult.Items
		};
		var result = await Mediator.Send(new UpdateSort.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			ListModel.Groups = sortResult.Groups;
			ListModel.Metrics = sortResult.Items;
		}
		IsWorking = false;
		StateHasChanged();
	}

	protected async Task HandleToggleArchived(FormMetricDefinitionModel model, bool toggled)
	{
		IsWorking = true;
		var result = await Mediator.Send(new ToggleArchived.Command(model.Sid, toggled));
		ShowNotification(result);
		if (result.Success)
		{
			model.IsArchived = toggled;
		}
		IsWorking = false;
	}

	public string HighlightStyle(FormMetricDefinitionModel model)
	{
		if (!string.IsNullOrEmpty(model.HighlightColor))
		{
			return $"min-width:36px;min-height:36px;background-color:{model.HighlightColor};border-radius:.1em;margin-right:.5em;";
		}
		return string.Empty;
	}

	private async Task ListMetricDefinitions()
	{
		IsLoading = true;
		ListModel = await Mediator.Send(new ListDefinitions.Query());
		IsLoading = false;
	}
}