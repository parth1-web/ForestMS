using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

public static class EnumExtensions
{
	public static List<SelectListItem> GetEnumDisplayName<TEnum>() where TEnum : Enum
	{
		return Enum.GetValues(typeof(TEnum))
				   .Cast<TEnum>()
				   .Select(e => new SelectListItem
				   {
					   Value = e.ToString(),
					   Text = e.GetDisplayName()
				   })
				   .ToList();
	}

	public static string GetDisplayName(this Enum enumValue)
	{
		var displayAttribute = enumValue.GetType()
										.GetField(enumValue.ToString())
										?.GetCustomAttributes(typeof(DisplayAttribute), false)
										.FirstOrDefault() as DisplayAttribute;

		return displayAttribute?.Name ?? enumValue.ToString();
	}
}
