﻿using Unshackled.Studio.Core.Client.Models;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Models;

public class FormSearchModel : SearchModel
{
	public DateTime? EndDate { get; set; }
	public int NumberOfMonths { get; set; }
}
