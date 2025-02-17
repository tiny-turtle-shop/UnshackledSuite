﻿using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class DuplicateSet
{
	public class Command : IRequest<CommandResult<FormWorkoutSetModel>>
	{
		public FormWorkoutSetModel Set { get; private set; }

		public Command(FormWorkoutSetModel set)
		{
			Set = set;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult<FormWorkoutSetModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<FormWorkoutSetModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormWorkoutSetModel, FormWorkoutSetModel>($"{baseUrl}duplicate-set", request.Set)
				?? new CommandResult<FormWorkoutSetModel>(false, Globals.UnexpectedError);
		}
	}
}
