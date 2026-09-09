using System.ComponentModel.DataAnnotations;

namespace LE.Common.Enums
{
    public enum IllegalActivityType
    {
        [Display(Name = ("दुरुपयोग, वन पैदावार"))]
        Misuse_forest = 1,

        [Display(Name = ("चोरी"))]
        Steal = 2,

        [Display(Name = ("आर्थिक हिनामिना"))]
        Financial_fraud = 3,

        [Display(Name = ("अन्य"))]
        Others,
    }
}
