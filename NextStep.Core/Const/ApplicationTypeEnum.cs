using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextStep.Core.Const
{
    public enum ApplicationTypeEnum
    {
        // Enrollment Requests
        [Display(Name = "طلب الالتحاق الخاص بقسم علوم حاسب", Description = "طلب التحاق بقسم علوم الحاسب")]
        ComputerScienceEnrollment,
        [Display(Name = "طلب الالتحاق الخاص بقسم نظم المعلومات", Description = "طلب التحاق بقسم نظم المعلومات")]
        InformationSystemsEnrollment,
        [Display(Name = "طلب الالتحاق الخاص بقسم حسابات علميه", Description = "طلب التحاق بقسم الحسابات العلمية")]
        ScientificComputingEnrollment,
        [Display(Name = "طلب الالتحاق الخاص بقسم ذكاء اصطناعي", Description = "طلب التحاق بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceEnrollment,

        // Extension Requests
        [Display(Name = "طلب مد الخاص بقسم علوم حاسب", Description = "طلب تمديد بقسم علوم الحاسب")]
        ComputerScienceExtension,
        [Display(Name = "طلب مد الخاص بقسم نظم المعلومات", Description = "طلب تمديد بقسم نظم المعلومات")]
        InformationSystemsExtension,
        [Display(Name = "طلب مد الخاص بقسم حسابات علميه", Description = "طلب تمديد بقسم الحسابات العلمية")]
        ScientificComputingExtension,
        [Display(Name = "طلب مد الخاص بقسم ذكاء اصطناعي", Description = "طلب تمديد بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceExtension,

        // Registration Hold
        [Display(Name = "ايقاف قيد الخاص بقسم علوم حاسب", Description = "ايقاف قيد بقسم علوم الحاسب")]
        ComputerScienceHold,
        [Display(Name = "ايقاف قيد الخاص بقسم نظم المعلومات", Description = "ايقاف قيد بقسم نظم المعلومات")]
        InformationSystemsHold,
        [Display(Name = "ايقاف قيد الخاص بقسم حسابات علميه", Description = "ايقاف قيد بقسم الحسابات العلمية")]
        ScientificComputingHold,
        [Display(Name = "ايقاف قيد الخاص بقسم ذكاء اصطناعي", Description = "ايقاف قيد بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceHold,

        // Registration Cancellation
        [Display(Name = "الغاء تسجيل الخاص بقسم علوم حاسب", Description = "الغاء تسجيل بقسم علوم الحاسب")]
        ComputerScienceCancellation,
        [Display(Name = "الغاء تسجيل الخاص بقسم نظم المعلومات", Description = "الغاء تسجيل بقسم نظم المعلومات")]
        InformationSystemsCancellation,
        [Display(Name = "الغاء تسجيل الخاص بقسم حسابات علميه", Description = "الغاء تسجيل بقسم الحسابات العلمية")]
        ScientificComputingCancellation,
        [Display(Name = "الغاء تسجيل الخاص بقسم ذكاء اصطناعي", Description = "الغاء تسجيل بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceCancellation,

        // Supervision Committee
        [Display(Name = "تعيين لجنة الاشراف الخاص بقسم علوم حاسب", Description = "تعيين لجنة إشراف بقسم علوم الحاسب")]
        ComputerScienceSupervision,
        [Display(Name = "تعيين لجنة الاشراف الخاص بقسم نظم المعلومات", Description = "تعيين لجنة إشراف بقسم نظم المعلومات")]
        InformationSystemsSupervision,
        [Display(Name = "تعيين لجنة الاشراف الخاص بقسم حسابات علميه", Description = "تعيين لجنة إشراف بقسم الحسابات العلمية")]
        ScientificComputingSupervision,
        [Display(Name = "تعيين لجنة الاشراف الخاص بقسم ذكاء اصطناعي", Description = "تعيين لجنة إشراف بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceSupervision,

        // Qualification Seminar
        [Display(Name = "سيمنار صلاحية الخاص بقسم علوم حاسب", Description = "سيمنار صلاحية بقسم علوم الحاسب")]
        ComputerScienceQualification,
        [Display(Name = "سيمنار صلاحية الخاص بقسم نظم المعلومات", Description = "سيمنار صلاحية بقسم نظم المعلومات")]
        InformationSystemsQualification,
        [Display(Name = "سيمنار صلاحية الخاص بقسم حسابات علميه", Description = "سيمنار صلاحية بقسم الحسابات العلمية")]
        ScientificComputingQualification,
        [Display(Name = "سيمنار صلاحية الخاص بقسم ذكاء اصطناعي", Description = "سيمنار صلاحية بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceQualification,

        // Examination Committee
        [Display(Name = "تشكيل لجنة حكم الخاص بقسم علوم حاسب", Description = "تشكيل لجنة حكم بقسم علوم الحاسب")]
        ComputerScienceExamination,
        [Display(Name = "تشكيل لجنة حكم الخاص بقسم نظم المعلومات", Description = "تشكيل لجنة حكم بقسم نظم المعلومات")]
        InformationSystemsExamination,
        [Display(Name = "تشكيل لجنة حكم الخاص بقسم حسابات علميه", Description = "تشكيل لجنة حكم بقسم الحسابات العلمية")]
        ScientificComputingExamination,
        [Display(Name = "تشكيل لجنة حكم الخاص بقسم ذكاء اصطناعي", Description = "تشكيل لجنة حكم بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceExamination,

        // Defense Seminar
        [Display(Name = "سيمنار مناقشة الخاص بقسم علوم حاسب", Description = "سيمنار مناقشة بقسم علوم الحاسب")]
        ComputerScienceDefense,
        [Display(Name = "سيمنار مناقشة الخاص بقسم نظم المعلومات", Description = "سيمنار مناقشة بقسم نظم المعلومات")]
        InformationSystemsDefense,
        [Display(Name = "سيمنار مناقشة الخاص بقسم حسابات علميه", Description = "سيمنار مناقشة بقسم الحسابات العلمية")]
        ScientificComputingDefense,
        [Display(Name = "سيمنار مناقشة الخاص بقسم ذكاء اصطناعي", Description = "سيمنار مناقشة بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceDefense,

        // Grant Requests
        [Display(Name = "منح الخاص بقسم علوم حاسب", Description = "طلب منح بقسم علوم الحاسب")]
        ComputerScienceGrant,
        [Display(Name = "منح الخاص بقسم نظم المعلومات", Description = "طلب منح بقسم نظم المعلومات")]
        InformationSystemsGrant,
        [Display(Name = "منح الخاص بقسم حسابات علميه", Description = "طلب منح بقسم الحسابات العلمية")]
        ScientificComputingGrant,
        [Display(Name = "منح الخاص بقسم ذكاء اصطناعي", Description = "طلب منح بقسم الذكاء الاصطناعي")]
        ArtificialIntelligenceGrant
    }
}
