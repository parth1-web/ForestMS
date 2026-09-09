using System.ComponentModel.DataAnnotations;

namespace LE.Common.Enums
{
	public enum RelationType
	{
		[Display(Name = "आमा")]
		Mother = 1,
		[Display(Name = "बाबु")]
		Father = 2,
		[Display(Name = "श्रीमती")]
		Wife = 3,
		[Display(Name = "श्रीमान")]
		Husband = 4,
		[Display(Name = "छोरी")]
		Daughter = 5,
		[Display(Name = "छोरा")]
		Son = 6,
		[Display(Name = "दिदी")]
		ElderSister = 7,
		[Display(Name = "बहिनी")]
		Sister = 8,
		[Display(Name = "दाजु")]
		ElderBrother = 9,
		[Display(Name = "भाई")]
		Brother = 10,
		[Display(Name = "हजुरआमा")]
		GrandMother = 11,
		[Display(Name = "हजुरबुबा")]
		GrandFather = 12,
		[Display(Name = "सासू")]
		MotherInLaw = 13,
		[Display(Name = "ससुरा")]
		FatherInLaw = 14,
		[Display(Name = "भाउजु")]
		SisterInLaw = 15,
		[Display(Name = "देवर")]
		BrotherInLaw = 16,
		[Display(Name = "नाति")]
		GrandSon = 17,
		[Display(Name = "नातिनी")]
		GrandDaughter = 18,
		[Display(Name = "बुहारी")]
		DaughterInLaw = 19,
		[Display(Name = "ज्वाइँ")]
		SonInLaw = 20,
        [Display(Name = "घरमु्ली")]
        Headofthehousehold = 21,
    }
}
