﻿using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public class AddWorkout
{
	public class Command : IRequest<CommandResult<string>>
	{
		public string TemplateSid { get; private set; }

		public Command(string templateSid)
		{
			TemplateSid = templateSid;
		}
	}

	public class Handler : BaseDashboardHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<string, string>($"{baseUrl}add-workout", request.TemplateSid)
				?? new CommandResult<string>(false, Globals.UnexpectedError);
		}
	}
}
