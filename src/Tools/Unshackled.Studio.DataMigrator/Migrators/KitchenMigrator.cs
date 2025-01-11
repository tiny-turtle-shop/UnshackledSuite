﻿using Microsoft.EntityFrameworkCore;
using Unshackled.Kitchen.Core.Data;
using Unshackled.Studio.DataMigrator.Configuration;
using Unshackled.Studio.DataMigrator.Enums;

namespace Unshackled.Studio.DataMigrator.Migrators;
internal class KitchenMigrator : BaseMigrator<KitchenDbContext>
{
	public KitchenMigrator(MigrationConfiguration mConfig, MigrationTypes migrationType)
	{
		isIdentityInsertRequired = false;
		switch (migrationType)
		{
			case MigrationTypes.MsSqlToMySql:
				dbLegacy = MsSqlServerDbContext.Create(mConfig.MsSqlDatabase, mConfig.SourceTablePrefix);
				dbNew = MySqlServerDbContext.Create(mConfig.MySqlDatabase, mConfig.DestinationTablePrefix);
				break;
			case MigrationTypes.MsSqlToPostgreSql:
				dbLegacy = MsSqlServerDbContext.Create(mConfig.MsSqlDatabase, mConfig.SourceTablePrefix);
				dbNew = PostgreSqlServerDbContext.Create(mConfig.PostgreSqlDatabase, mConfig.DestinationTablePrefix);
				break;
			case MigrationTypes.MsSqlToSqlite:
				dbLegacy = MsSqlServerDbContext.Create(mConfig.MsSqlDatabase, mConfig.SourceTablePrefix);
				dbNew = SqliteDbContext.Create(mConfig.SqliteDatabase, mConfig.DestinationTablePrefix);
				PrepareSqlitePath(mConfig.SqliteDatabase);
				break;
			case MigrationTypes.MySqlToMsSql:
				dbLegacy = MySqlServerDbContext.Create(mConfig.MySqlDatabase, mConfig.SourceTablePrefix);
				dbNew = MsSqlServerDbContext.Create(mConfig.MsSqlDatabase, mConfig.DestinationTablePrefix);
				isIdentityInsertRequired = true;
				break;
			case MigrationTypes.MySqlToPostgreSql:
				dbLegacy = MySqlServerDbContext.Create(mConfig.MySqlDatabase, mConfig.SourceTablePrefix);
				dbNew = PostgreSqlServerDbContext.Create(mConfig.PostgreSqlDatabase, mConfig.DestinationTablePrefix);
				break;
			case MigrationTypes.MySqlToSqlite:
				dbLegacy = MySqlServerDbContext.Create(mConfig.MySqlDatabase, mConfig.SourceTablePrefix);
				dbNew = SqliteDbContext.Create(mConfig.SqliteDatabase, mConfig.DestinationTablePrefix);
				PrepareSqlitePath(mConfig.SqliteDatabase);
				break;
			case MigrationTypes.PostgreSqlToMsSql:
				dbLegacy = PostgreSqlServerDbContext.Create(mConfig.PostgreSqlDatabase, mConfig.SourceTablePrefix);
				dbNew = MsSqlServerDbContext.Create(mConfig.MsSqlDatabase, mConfig.DestinationTablePrefix);
				isIdentityInsertRequired = true;
				break;
			case MigrationTypes.PostgreSqlToMySql:
				dbLegacy = PostgreSqlServerDbContext.Create(mConfig.PostgreSqlDatabase, mConfig.SourceTablePrefix);
				dbNew = MySqlServerDbContext.Create(mConfig.MySqlDatabase, mConfig.DestinationTablePrefix);
				break;
			case MigrationTypes.PostgreSqlToSqlite:
				dbLegacy = PostgreSqlServerDbContext.Create(mConfig.PostgreSqlDatabase, mConfig.SourceTablePrefix);
				dbNew = SqliteDbContext.Create(mConfig.SqliteDatabase, mConfig.DestinationTablePrefix);
				PrepareSqlitePath(mConfig.SqliteDatabase);
				break;
			case MigrationTypes.SqliteToMsSql:
				dbLegacy = SqliteDbContext.Create(mConfig.SqliteDatabase, mConfig.SourceTablePrefix);
				dbNew = MsSqlServerDbContext.Create(mConfig.MsSqlDatabase, mConfig.DestinationTablePrefix);
				isIdentityInsertRequired = true;
				break;
			case MigrationTypes.SqliteToMySql:
				dbLegacy = SqliteDbContext.Create(mConfig.SqliteDatabase, mConfig.SourceTablePrefix);
				dbNew = MySqlServerDbContext.Create(mConfig.MySqlDatabase, mConfig.DestinationTablePrefix);
				break;
			case MigrationTypes.SqliteToPostgreSql:
				dbLegacy = SqliteDbContext.Create(mConfig.SqliteDatabase, mConfig.SourceTablePrefix);
				dbNew = PostgreSqlServerDbContext.Create(mConfig.PostgreSqlDatabase, mConfig.DestinationTablePrefix);
				break;
			default:
				dbLegacy = default!;
				dbNew = default!;
				break;
		}
	}

	public async Task Migrate()
	{
		await dbNew.Database.MigrateAsync();

		await MigrateUsers();
		await MigrateUserClaims();
		await MigrateUserLogins();
		await MigrateUserTokens();
		await MigrateRoles();
		await MigrateRoleClaims();
		await MigrateUserRoles();
		await MigrateDbSet(dbLegacy.Members, dbNew.Members, "Members");
		await MigrateMemberMeta();
		await MigrateDbSet(dbLegacy.Cookbooks, dbNew.Cookbooks, "Cookbooks");
		await MigrateCookbookMembers("Cookbook Members");
		await MigrateDbSet(dbLegacy.CookbookInvites, dbNew.CookbookInvites, "Cookbook Invites");
		await MigrateDbSet(dbLegacy.Households, dbNew.Households, "Households");
		await MigrateHouseholdMembers("Household Members");
		await MigrateDbSet(dbLegacy.HouseholdInvites, dbNew.HouseholdInvites, "Household Invites");
		await MigrateDbSet(dbLegacy.ProductCategories, dbNew.ProductCategories, "Product Categories");
		await MigrateDbSet(dbLegacy.Products, dbNew.Products, "Products");
		await MigrateDbSet(dbLegacy.ProductBundles, dbNew.ProductBundles, "Product Bundles");
		await MigrateProductBundleItems("Product Bundle Items");
		await MigrateDbSet(dbLegacy.Tags, dbNew.Tags, "Tags");
		await MigrateDbSet(dbLegacy.Recipes, dbNew.Recipes, "Recipes");
		await MigrateDbSet(dbLegacy.RecipeIngredientGroups, dbNew.RecipeIngredientGroups, "Recipe Ingredient Groups");
		await MigrateDbSet(dbLegacy.RecipeIngredients, dbNew.RecipeIngredients, "Recipe Ingredients");
		await MigrateDbSet(dbLegacy.RecipeSteps, dbNew.RecipeSteps, "Recipe Steps");
		await MigrateDbSet(dbLegacy.RecipeNotes, dbNew.RecipeNotes, "Recipe Notes");
		await MigrateDbSet(dbLegacy.RecipeImages, dbNew.RecipeImages, "Recipe Images");
		await MigrateRecipeTags("Recipe Tags");
		await MigrateProductSubstitutions("Product Substitutions");
		await MigrateDbSet(dbLegacy.Stores, dbNew.Stores, "Stores");
		await MigrateDbSet(dbLegacy.StoreLocations, dbNew.StoreLocations, "Store Locations");
		await MigrateStoreProductLocations("Store Product Locations");
		await MigrateDbSet(dbLegacy.ShoppingLists, dbNew.ShoppingLists, "Shopping Lists");
		await MigrateShoppingListItems("Shopping List Items");
		await MigrateShoppingListRecipeItems("Shopping List Recipe Items");
		await MigrateCookbookRecipes("Cookbook Recipes");
	}

	protected async Task MigrateCookbookMembers(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.CookbookMembers.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.CookbookMembers
					.AsNoTracking()
					.OrderBy(x => x.CookbookId)
						.ThenBy(x => x.MemberId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.CookbookMembers.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateCookbookRecipes(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.CookbookRecipes.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.CookbookRecipes
					.AsNoTracking()
					.OrderBy(x => x.CookbookId)
						.ThenBy(x => x.RecipeId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.CookbookRecipes.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateHouseholdMembers(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.HouseholdMembers.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.HouseholdMembers
					.AsNoTracking()
					.OrderBy(x => x.HouseholdId)
						.ThenBy(x => x.MemberId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.HouseholdMembers.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateProductBundleItems(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.ProductBundleItems.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.ProductBundleItems
					.AsNoTracking()
					.OrderBy(x => x.ProductBundleId)
						.ThenBy(x => x.ProductId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.ProductBundleItems.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateProductSubstitutions(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.ProductSubstitutions.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.ProductSubstitutions
					.AsNoTracking()
					.OrderBy(x => x.IngredientKey)
						.ThenBy(x => x.ProductId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.ProductSubstitutions.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateRecipeTags(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.RecipeTags.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.RecipeTags
					.AsNoTracking()
					.OrderBy(x => x.RecipeId)
						.ThenBy(x => x.TagId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.RecipeTags.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateStoreProductLocations(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.StoreProductLocations.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.StoreProductLocations
					.AsNoTracking()
					.OrderBy(x => x.StoreId)
						.ThenBy(x => x.ProductId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.StoreProductLocations.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateShoppingListItems(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.ShoppingListItems.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.ShoppingListItems
					.AsNoTracking()
					.OrderBy(x => x.ShoppingListId)
						.ThenBy(x => x.ProductId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.ShoppingListItems.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}

	protected async Task MigrateShoppingListRecipeItems(string title)
	{
		Msg.WriteHeader(title);
		Msg.WriteOngoing($"Migrating {title}");
		int count = await dbLegacy.ShoppingListRecipeItems.CountAsync();
		if (count > 0)
		{
			Msg.WriteOngoing($"found {count}");

			int pages = (int)Math.Ceiling((decimal)count / pageSize);
			for (int i = 0; i < pages; i++)
			{
				var list = await dbLegacy.ShoppingListRecipeItems
					.AsNoTracking()
					.OrderBy(x => x.ShoppingListId)
						.ThenBy(x => x.ProductId)
						.ThenBy(x => x.RecipeId)
					.Skip(i * pageSize)
					.Take(pageSize)
					.ToListAsync();

				if (list.Count > 0)
				{
					dbNew.ShoppingListRecipeItems.AddRange(list);
					await dbNew.SaveChangesAsync();
					Msg.WriteDot();
				}
			}
			Msg.WriteComplete();
		}
		else
		{
			Msg.WriteComplete($"found {count}");
		}
	}
}
