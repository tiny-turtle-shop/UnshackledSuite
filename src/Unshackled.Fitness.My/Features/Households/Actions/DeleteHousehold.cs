﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Extensions;
using Unshackled.Fitness.My.Services;

namespace Unshackled.Fitness.My.Features.Households.Actions;

public class DeleteHousehold
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public string HouseholdSid { get; private set; }

		public Command(long memberId, string householdSid)
		{
			MemberId = memberId;
			HouseholdSid = householdSid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		private readonly StorageSettings storageSettings;
		private readonly IFileStorageService fileService;

		public Handler(BaseDbContext db, IMapper mapper, StorageSettings storageSettings, IFileStorageService fileService) : base(db, mapper)
		{
			this.storageSettings = storageSettings;
			this.fileService = fileService;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long householdId = request.HouseholdSid.DecodeLong();

			if (householdId == 0)
				return new CommandResult(false, "Invalid household ID.");

			if (!await db.Households.Where(x => x.Id == householdId && x.MemberId == request.MemberId).AnyAsync(cancellationToken))
				return new CommandResult(false, "Only the owner can delete a household.");

			if (await db.HouseholdMembers.Where(x => x.HouseholdId == householdId && x.MemberId != request.MemberId).AnyAsync(cancellationToken))
				return new CommandResult(false, "A household with members cannot be deleted.");

			var household = await db.Households
				.Where(x => x.Id == householdId)
				.SingleOrDefaultAsync(cancellationToken);

			if (household == null)
				return new CommandResult(false, "Invalid household.");

			string imageDir = string.Format(Globals.Paths.HouseholdImageDir, household.Id.Encode());
			bool deleted = await fileService.DeleteDirectory(storageSettings.Container, imageDir, cancellationToken);

			if (!deleted)
				return new CommandResult(false, "Unable to delete recipe images. Household cannot be deleted.");

			var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// if active household for any member, remove it
				await db.MemberMeta
					.Where(x => x.MetaKey == Globals.MetaKeys.ActiveHouseholdId && x.MetaValue == householdId.ToString())
					.DeleteFromQueryAsync(cancellationToken);

				await db.HouseholdInvites
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.HouseholdMembers
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.MealPrepPlanRecipes
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.MealPrepPlanSlots
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ShoppingListRecipeItems
					.Include(x => x.ShoppingList)
					.Where(x => x.ShoppingList.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ShoppingListItems
					.Include(x => x.ShoppingList)
					.Where(x => x.ShoppingList.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ShoppingLists
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.StoreProductLocations
					.Include(x => x.Store)
					.Where(x => x.Store.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.StoreLocations
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.Stores
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ProductBundleItems
					.Include(x => x.ProductBundle)
					.Where(x => x.ProductBundle.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ProductBundles
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ProductSubstitutions
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ProductImages
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.Products
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ProductCategories
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.RecipeTags
					.Include(x => x.Recipe)
					.Where(x => x.Recipe.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.Tags
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.CookbookRecipes
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.RecipeImages
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.RecipeNotes
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.RecipeSteps
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.RecipeIngredients
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.RecipeIngredientGroups
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				await db.Recipes
					.Where(x => x.HouseholdId == householdId)
					.DeleteFromQueryAsync(cancellationToken);

				db.Households.Remove(household);
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "The household has been deleted.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, "The household could not be deleted.");
			}
		}
	}
}