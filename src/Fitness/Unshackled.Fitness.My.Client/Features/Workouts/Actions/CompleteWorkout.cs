﻿using MediatR;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Studio.Core.Client;
using Unshackled.Studio.Core.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class CompleteWorkout
{
	public class Command : IRequest<CommandResult>
	{
		public CompleteWorkoutModel Model { get; private set; }

		public Command(CompleteWorkoutModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}finish", request.Model)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
