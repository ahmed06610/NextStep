using System.ComponentModel.DataAnnotations;

namespace NextStep.Core.Const
{
    public enum DepartmentEnum
    {
        [Display(Name = "مجلس الكليه")]
        CollegeCouncil,

        [Display(Name = "لجنه الدرسات العليا")]
        GraduateStudiesCommittee,

        [Display(Name = "حسابات علميه")]
        ScientificComputing,

        [Display(Name = "ذكاء اصطناعي")]
        ArtificialIntelligence,

        [Display(Name = "علوم حاسب")]
        ComputerScience,

        [Display(Name = "نظم المعلومات")]
        InformationSystems,

        [Display(Name = "القسم العلمي")]
        ScientificDepartment,

        [Display(Name = "إدارة الدرسات العليا")]
        GraduateStudiesAdministration
    }
    
}
