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
        [Display(Name = "طلب التحاق", Description = "طلب التحاق بالدراسات العليا")]
        EnrollmentRequest,

        [Display(Name = "طلب مد", Description = "طلب تمديد فترة الدراسة")]
        ExtensionRequest,

        [Display(Name = "ايقاف قيد", Description = "طلب إيقاف القيد الدراسي")]
        RegistrationHold,

        [Display(Name = "الغاء تسجيل", Description = "طلب إلغاء التسجيل")]
        RegistrationCancellation,

        [Display(Name = "تعيين لجنه الاشراف و سيمنار 1", Description = "تعيين لجنة الإشراف والسمينار الأول")]
        SupervisionCommitteeAssignment,

        [Display(Name = "سيمنار صلاحيه", Description = "سمينار صلاحية البحث")]
        QualificationSeminar,

        [Display(Name = "تشكيل لجنه حكم ومناقشه", Description = "تشكيل لجنة الحكم والمناقشة")]
        ExaminationCommitteeFormation,

        [Display(Name = "سيمنار مناقشه", Description = "سمينار مناقشة البحث")]
        DefenseSeminar,

        [Display(Name = "منح", Description = "طلب منح دراسية")]
        GrantRequest
    }
}
