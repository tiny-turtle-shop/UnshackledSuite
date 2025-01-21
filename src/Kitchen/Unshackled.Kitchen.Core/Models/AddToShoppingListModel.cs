﻿using System.Text.Json.Serialization;

namespace Unshackled.Kitchen.Core.Models;

public class AddToShoppingListModel
{
	public string RecipeIngredientSid { get; set; } = string.Empty;
	public string ProductSid { get; set; } = string.Empty;
	public string ProductTitle { get; set; } = string.Empty;
	public string IngredientKey { get; set; } = string.Empty;
	public string IngredientTitle { get; set; } = string.Empty;
	public int QuantityToAdd { get; set; } = 1;
	public int QuantityInList { get; set; }
	public decimal TotalPortionUsed { get; set; }
	public bool IsUnitMismatch { get; set; } = false;
	public bool IsSkipped { get; set; }

	public List<RecipeAmountListModel> RecipeAmounts { get; set; } = [];

	[JsonIgnore]
	public bool IsAutoGenerated => string.IsNullOrEmpty(ProductSid);

	[JsonIgnore]
	public int TotalQuantity => QuantityToAdd + QuantityInList;

}
