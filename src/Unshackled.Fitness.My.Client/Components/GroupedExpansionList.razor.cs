using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using Unshackled.Fitness.Core.Interfaces;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Components;

public partial class GroupedExpansionList<TGroup, TItem>
	where TGroup : ISortableGroup, new()
	where TItem : IGroupedSortable, new()
{
	[Parameter] public string? Class { get; set; }
	[Parameter] public RenderFragment? EmptyListRenderer { get; set; }
	[Parameter] public RenderFragment<TGroup>? GroupRenderer { get; set; }
	[Parameter] public List<TGroup> Groups { get; set; } = new();
	[Parameter] public bool IsLoading { get; set; } = false;
	[Parameter] public RenderFragment<RowContext<TItem>>? ItemRenderer { get; set; }
	[Parameter] public List<TItem> Items { get; set; } = new();
	[Parameter] public bool UseDividers { get; set; } = true;
	[Parameter] public bool MultiExpansion { get; set; } = false;
	[Parameter] public bool Outlined { get; set; } = true;
	[Parameter] public int Elevation { get; set; } = 1;
	[Parameter] public string PanelClass { get; set; } = string.Empty;

	protected string ListClass => new CssBuilder("list-view")
		.AddClass(Class)
		.Build();
}