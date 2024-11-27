﻿using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Unshackled.Food.Core.Data;
using Unshackled.Food.My.Client.Features.Members.Models;
using Unshackled.Studio.Core.Data.Entities;

namespace Unshackled.Food.My.Features.Members.Actions;

public class GetCurrentAccountStatus
{
	public class Query : IRequest<CurrentAccountStatusModel>
	{
		public ClaimsPrincipal User { get; private set; }

		public Query(ClaimsPrincipal user)
		{
			User = user;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, CurrentAccountStatusModel>
	{
		private readonly UserManager<UserEntity> userManager;
		private readonly SignInManager<UserEntity> signInManager;

		public Handler(FoodDbContext db, IMapper mapper, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : base(db, mapper) 
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<CurrentAccountStatusModel> Handle(Query request, CancellationToken cancellationToken)
		{
			CurrentAccountStatusModel accountStatus = new();
			var user = await userManager.GetUserAsync(request.User);

			if (user != null)
			{
				accountStatus.HasPassword = await userManager.HasPasswordAsync(user);

				var currentLogins = await userManager.GetLoginsAsync(user);
				var otherLogins = (await signInManager.GetExternalAuthenticationSchemesAsync())
					.Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
					.ToList();

				accountStatus.HasExternalLogin = currentLogins.Count > 0;
				accountStatus.HasExternalLoginsAvailable = currentLogins.Count > 0 || otherLogins.Count > 0;
			}

			return accountStatus;
		}
	}
}
