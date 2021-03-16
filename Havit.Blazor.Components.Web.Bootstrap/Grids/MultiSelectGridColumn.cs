﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Havit.Blazor.Components.Web.Bootstrap
{
	public class MultiSelectGridColumn<TItem> : HxGridColumnBase<TItem>
	{
		[Parameter] public HashSet<TItem> SelectedDataItems { get; set; }
		[Parameter] public bool AllDataItemsSelected { get; set; }
		[Parameter] public EventCallback OnSelectAllClicked { get; set; }
		[Parameter] public EventCallback OnSelectNoneClicked { get; set; }
		[Parameter] public EventCallback<TItem> OnSelectDataItemClicked { get; set; }
		[Parameter] public EventCallback<TItem> OnUnselectDataItemClicked { get; set; }

		protected override CellTemplate GetHeaderCellTemplate()
		{
			return new CellTemplate
			{
				CssClass = "text-center",
				Template = (RenderTreeBuilder builder) =>
				{
					builder.OpenElement(100, "div");
					builder.AddAttribute(101, "class", "form-check form-check-inline");

					builder.OpenElement(200, "input");
					builder.AddAttribute(201, "type", "checkbox");
					builder.AddAttribute(202, "class", "form-check-input");

					builder.AddAttribute(203, "checked", AllDataItemsSelected);
					builder.AddAttribute(204, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleSelectAllOrNoneClick));
					builder.AddEventStopPropagationAttribute(205, "onclick", true);

					builder.CloseElement(); // input

					builder.CloseElement(); // div
				}
			};
		}

		protected override CellTemplate GetItemCellTemplate(TItem item)
		{
			return new CellTemplate
			{
				CssClass = "text-center",
				Template = (RenderTreeBuilder builder) =>
				{
					builder.OpenElement(100, "div");
					builder.AddAttribute(101, "class", "form-check form-check-inline");

					builder.OpenElement(200, "input");
					builder.AddAttribute(201, "type", "checkbox");
					builder.AddAttribute(202, "class", "form-check-input");

					bool selected = SelectedDataItems?.Contains(item) ?? false;
					builder.AddAttribute(203, "checked", selected);
					builder.AddAttribute(204, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, HandleSelectDataItemClick(item, selected)));
					builder.AddEventStopPropagationAttribute(205, "onclick", true);

					builder.CloseElement(); // input

					builder.CloseElement(); // div
				}
			};
		}

		protected override CellTemplate GetFooterCellTemplate()
		{
			return new CellTemplate(RenderFragmentBuilder.Empty());
		}

		protected override IEnumerable<SortingItem<TItem>> GetSorting()
		{
			return Enumerable.Empty<SortingItem<TItem>>();
		}

		private Func<ChangeEventArgs, Task> HandleSelectDataItemClick(TItem item, bool wasSelected)
		{
			return async (ChangeEventArgs changeEventArgs) =>
			{
				await (wasSelected ? OnUnselectDataItemClicked : OnSelectDataItemClicked).InvokeAsync(item);
			};
		}

		private async Task HandleSelectAllOrNoneClick(ChangeEventArgs args)
		{
			if (AllDataItemsSelected)
			{
				await OnSelectNoneClicked.InvokeAsync();
			}
			else
			{
				await OnSelectAllClicked.InvokeAsync();
			}
		}
	}
}
