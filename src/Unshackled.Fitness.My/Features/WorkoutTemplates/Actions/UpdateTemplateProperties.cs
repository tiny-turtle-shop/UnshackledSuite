﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Studio.Core.Client.Models;
using Unshackled.Studio.Core.Data;
using Unshackled.Studio.Core.Server.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class UpdateTemplateProperties
{
	public class Command : IRequest<CommandResult<TemplateModel>>
	{
		public long MemberId { get; private set; }
		public FormTemplateModel Model { get; private set; }

		public Command(long memberId, FormTemplateModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<TemplateModel>>
	{
		public Handler(FitnessDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<TemplateModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			long templateId = request.Model.Sid.DecodeLong();

			var template = await db.WorkoutTemplates
				.Where(x => x.Id == templateId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (template == null)
				return new CommandResult<TemplateModel>(false, "Invalid template.");

			// Update template
			template.Description = request.Model.Description?.Trim();
			template.Title = request.Model.Title.Trim();

			// Mark modified to avoid missing string case changes.
			db.Entry(template).Property(x => x.Title).IsModified = true;
			db.Entry(template).Property(x => x.Description).IsModified = true;

			int changes = await db.SaveChangesAsync(cancellationToken);

			return new CommandResult<TemplateModel>(true, "Template updated.", mapper.Map<TemplateModel>(template));
		}
	}
}