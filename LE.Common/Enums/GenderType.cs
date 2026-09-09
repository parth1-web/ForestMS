using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LE.Common.Enums
{
	public enum GenderType
	{
		[Display(Name = ("पुरुष"))]
		Male = 1,

		[Display(Name = ("महिला"))]
		Female = 2,

		[Display(Name = ("अन्य"))]
		Others = 3,
	}
}
