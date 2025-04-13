using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Const
{
    public enum ApplicationRoles
    {
        [Display(Name = "موظف مجلس الكليه")]
        CollegeCouncilEmployee,

        [Display(Name = "موظف لجنه الدرسات العليا")]
        GraduateStudiesCommitteeEmployee,

        [Display(Name = "موظف حسابات علميه")]
        ScientificAccountsEmployee,

        [Display(Name = "موظف ذكاء اصطناعي")]
        ArtificialIntelligenceEmployee,

        [Display(Name = "موظف علوم حاسب")]
        ComputerScienceEmployee,

        [Display(Name = "موظف نظم المعلومات")]
        InformationSystemsEmployee,

        [Display(Name = "موظف إدارة الدرسات العليا")]
        GraduateStudiesManagementEmployee,

        [Display(Name = "طالب")]
        Student
    }
}
