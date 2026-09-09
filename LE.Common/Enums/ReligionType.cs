using System.ComponentModel.DataAnnotations;
namespace LE.Common.Enums
{
	public enum ReligionType
	{
		[Display(Name = "हिन्दू")]
		Hindu = 1,

		[Display(Name = "किरात")]
		Kirat = 2,

		[Display(Name = "बुद्ध")]
		Buddhist = 3,

		[Display(Name = "इस्लाम")]
		Islam = 4,

		[Display(Name = "अन्य")]
		Others = 5,
	}
}
