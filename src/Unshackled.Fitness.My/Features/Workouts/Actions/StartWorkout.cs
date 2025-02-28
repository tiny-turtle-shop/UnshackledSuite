﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class StartWorkout
{
	public class Command : IRequest<CommandResult<DateTime>>
	{
		public long MemberId { get; private set; }
		public StartWorkoutModel Model { get; private set; }

		public Command(long memberId, StartWorkoutModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<DateTime>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<DateTime>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.Model.WorkoutSid))
				return new CommandResult<DateTime>(false, "Invalid workout ID.");

			long workoutId = request.Model.WorkoutSid.DecodeLong();

			if (workoutId == 0)
				return new CommandResult<DateTime>(false, "Invalid workout ID.");

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (workout == null)
				return new CommandResult<DateTime>(false, "Workout not found.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Mark pre-workout tasks as complete
				await db.WorkoutTasks
					.Where(x => x.WorkoutId == workoutId && x.Type == WorkoutTaskTypes.PreWorkout)
					.UpdateFromQueryAsync(x => new WorkoutTaskEntity { Completed = true }, cancellationToken);

				workout.DateStarted = request.Model.DateStarted;
				workout.DateStartedUtc = request.Model.DateStartedUtc;
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult<DateTime>(true, "Workout started", workout.DateStartedUtc.Value);
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult<DateTime>(false, "Unexpected Error: Could not start the workout.");
			}
		}
	}
}