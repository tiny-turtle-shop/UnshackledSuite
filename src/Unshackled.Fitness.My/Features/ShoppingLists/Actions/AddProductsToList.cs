﻿using AutoMapper;
using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.ShoppingLists.Models;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.ShoppingLists.Actions;

public class AddProductsToList
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public AddProductsModel Model { get; private set; }

		public Command(long memberId, AddProductsModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long shoppingListId = request.Model.ShoppingListSid.DecodeLong();

			if (shoppingListId == 0)
				return new CommandResult(false, "Invalid shopping list ID.");

			if (!await db.HasShoppingListPermission(shoppingListId, request.MemberId, PermissionLevels.Write))
				return new CommandResult(false, Globals.PermissionError);

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				List<ShoppingListItemEntity> newItems = [];
				foreach (string productSid in request.Model.Products.Keys)
				{
					long productId = productSid.DecodeLong();

					// invalid product ID, skip and continue
					if (productId == 0) continue;

					ShoppingListItemEntity item = new()
					{
						ProductId = productId,
						Quantity = request.Model.Products[productSid],
						ShoppingListId = shoppingListId
					};
					newItems.Add(item);
				}

				if (newItems.Count > 0)
				{
					db.ShoppingListItems.AddRange(newItems);
					await db.SaveChangesAsync(cancellationToken);
					await transaction.CommitAsync(cancellationToken);

					return new CommandResult(true, "Products successfully added to list.");
				}

				return new CommandResult(false, "Nothing to add.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}