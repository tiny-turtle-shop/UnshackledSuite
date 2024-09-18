﻿using System.Globalization;
using TimeZoneNames;

namespace Unshackled.Fitness.Core.Extensions;

public static class DateTimeExtensions
{
	public static DateTime CombineDateAndTime(this DateTime datePortion, TimeSpan timePortion)
	{
		return datePortion.Date.Add(timePortion);
	}

	public static DateTime CombineDateAndTime(this DateTime datePortion, TimeSpan? timePortion)
	{
		return datePortion.CombineDateAndTime(timePortion ?? TimeSpan.Zero);
	}

	public static DateTime? CombineDateAndTime(this DateTime? datePortion, TimeSpan? timePortion)
	{
		if (datePortion == null)
			return null;

		return datePortion.Value.CombineDateAndTime(timePortion ?? TimeSpan.Zero);
	}

	public static DateTime CombineDateAndTime(this TimeSpan timePortion, DateTime datePortion)
	{
		return datePortion.CombineDateAndTime(timePortion);
	}

	public static DateTime? CombineDateAndTime(this TimeSpan timePortion, DateTime? datePortion)
	{
		return datePortion.CombineDateAndTime(timePortion);
	}

	public static DateTime CombineDateAndTime(this TimeSpan? timePortion, DateTime datePortion)
	{
		return datePortion.CombineDateAndTime(timePortion);
	}

	public static DateTime? CombineDateAndTime(this TimeSpan? timePortion, DateTime? datePortion)
	{
		return datePortion.CombineDateAndTime(timePortion);
	}

	public static string DisplayAsLocalTimeZone(this DateTime? date)
	{
		if (date.HasValue)
		{
			return date.Value.DisplayAsLocalTimeZone();
		}
		return string.Empty;
	}

	public static string DisplayAsLocalTimeZone(this DateTime date)
	{
		var localDate = date.ToLocalTime();
		string tzid = TimeZoneInfo.Local.Id;                // example: "Eastern Standard time"
		string lang = CultureInfo.CurrentCulture.Name;      // example: "en-US"
		var abbreviations = TZNames.GetAbbreviationsForTimeZone(tzid, lang);

		if (TimeZoneInfo.Local.IsDaylightSavingTime(localDate))
			return string.Format("{0} {1}", localDate.ToString("f"), abbreviations.Daylight);
		else
			return string.Format("{0} {1}", localDate.ToString("f"), abbreviations.Standard);
	}
}
