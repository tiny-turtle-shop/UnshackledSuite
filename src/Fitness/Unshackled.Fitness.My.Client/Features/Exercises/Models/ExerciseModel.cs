﻿using Unshackled.Fitness.Core.Enums;
using Unshackled.Studio.Core.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Models;

public class ExerciseModel : BaseMemberObject
{
	public Guid? MatchId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public string? Notes { get; set; }
	public bool IsTrackingSplit { get; set; }
	public bool IsArchived { get; set; }
	public SetMetricTypes DefaultSetMetricType { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
	public WorkoutSetTypes DefaultSetType { get; set; }
}
