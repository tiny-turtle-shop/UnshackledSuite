﻿namespace Unshackled.Fitness.Core.Extensions;

public static class IntExtensions
{
	public static string CountLabel(this int value, string singular, string plural)
	{
		if (value == 1)
			return $"{value} {singular}";
		else
			return $"{value} {plural}";
	}

	public static string SecondsAsTimeSpan(this int value, int? intensity = null)
	{
		var ts = TimeSpan.FromSeconds(value);

		string format = @"mm\:ss";
		if (ts.Hours > 0)
		{
			format = @"hh\:mm\:ss";
		}

		if (intensity.HasValue && intensity.Value > 0)
			return $"{ts.ToString(format)} @ {intensity.Value}";
		else
			return ts.ToString(format);
	}

	public static string SecondsAsTimeSpan(this int value, string format, int? intensity = null)
	{
		var ts = TimeSpan.FromSeconds(value);

		if (intensity.HasValue && intensity.Value > 0)
			return $"{ts.ToString(format)} @ {intensity.Value}";
		else
			return ts.ToString(format);
	}

	public static double SecondsToHours(this int value)
	{
		var ts = TimeSpan.FromSeconds(value);
		return ts.TotalHours;
	}
}
