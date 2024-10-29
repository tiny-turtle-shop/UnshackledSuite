﻿using System.Reflection;
using MudBlazor;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Utils;

public static class Calculator
{

	/*
	 *  Adapted from code found at
	 *   - https://github.com/iamartyom/ColorHelper
	 *   - https://24ways.org/2010/calculating-color-contrast/
	 */
	public static string ContrastHexColor(string hexCode)
	{
		if (hexCode.StartsWith("#") && hexCode.Length > 1) 
			hexCode = hexCode.Substring(1);

		if (hexCode.Length < 6)
			return string.Empty;

		if (hexCode.Length > 6)
			hexCode = hexCode.Substring(0, 6);

		int value = Convert.ToInt32(hexCode, 16);
		byte r = (byte)((value >> 16) & 255);
		byte g = (byte)((value >> 8) & 255);
		byte b = (byte)(value & 255);

		double[] modifiedRGB = { r / 255.0, g / 255.0, b / 255.0 };

		double y = (modifiedRGB[0] * 0.299) + (modifiedRGB[1] * 0.587) + (modifiedRGB[2] * 0.114);

		if (y >= .5)
			return "#000000";
		else
			return "#ffffff";
	}

	public static DateOnlyRange DateRange(DateTime? endDate, int previousMonths, DateTime defaultDate)
	{
		int toYear = endDate.HasValue ? endDate.Value.Year : defaultDate.Year;
		int toMonth = endDate.HasValue ? endDate.Value.Month : defaultDate.Month;
		int fromYear = toYear;
		int fromMonth = toMonth - previousMonths;

		if (fromMonth <= 0)
		{
			fromMonth = fromMonth + 12;
			fromYear--;
		}

		return new DateOnlyRange(
			new DateOnly(fromYear, fromMonth, 1), 
			new DateOnly(toYear, toMonth + 1, 1).AddDays(-1)
		);
	}

	public static double DistanceBetween(double lat1, double lon1, double lat2, double lon2)
	{
		double d1 = lat1 * (Math.PI / 180.0);
		double num1 = lon1 * (Math.PI / 180.0);
		double d2 = lat2 * (Math.PI / 180.0);
		double num2 = lon2 * (Math.PI / 180.0) - num1;
		var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
				 Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
		return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
	}

	public static double? DistanceBetween(double? lat1, double? lon1, double? lat2, double? lon2)
	{
		if (lat1.HasValue && lon1.HasValue && lat2.HasValue && lon2.HasValue)
			return DistanceBetween(lat1.Value, lon1.Value, lat2.Value, lon2.Value);
		else
			return null;
	}

	public static int Pages(int pageSize, int total)
	{
		return (int)Math.Ceiling((decimal)total / pageSize);
	}

	public static int PageStartItem(int currentPage, int pageSize)
	{
		return ((currentPage - 1) * pageSize) + 1;
	}

	public static int PageEndItem(int currentPage, int pageSize, int totalItems)
	{
		return Math.Min(currentPage * pageSize, totalItems);
	}

	public static void WeekAndDayInCycle(DateTime startDate, DateTime currentDate, int weeksInCycle, out int week, out int day)
	{
		TimeSpan dateDiff = currentDate - startDate;
		double daysFromStart = Math.Floor(dateDiff.TotalDays);

		// if future or current date, daysFromStart is negative or zero
		if (daysFromStart <= 0)
		{
			week = 0;
			day = 0;
		}
		else
		{
			// number of completed weeks from start date
			double weeksFromStart = Math.Floor(daysFromStart / 7);
			// number of complete cycles from start date
			double cycles = Math.Floor(weeksFromStart / weeksInCycle);
			// current week of program
			week = (int)Math.Floor(weeksFromStart - (cycles * weeksInCycle));
			// current day of week
			day = (int)Math.Floor(daysFromStart - (weeksFromStart * 7));
		}
	}

	public static decimal Volume(decimal? weight, int? reps)
	{
		decimal wgt = weight.HasValue ? weight.Value : 0;
		int r = reps.HasValue ? reps.Value : 0;
		return wgt * r;
	}

	public static decimal Volume(decimal? weight, int? repsLeft, int? repsRight)
	{
		decimal wgt = weight.HasValue ? weight.Value : 0;
		int rl = repsLeft.HasValue ? repsLeft.Value : 0;
		int rr = repsRight.HasValue ? repsRight.Value : 0;

		return wgt * (rl + rr);
	}

	public static TimeSpan TotalTime(DateTime? start, DateTime? end)
	{
		return end.HasValue && start.HasValue
			? new TimeSpan(end.Value.Ticks - start.Value.Ticks) 
			: TimeSpan.Zero;
	}
}
