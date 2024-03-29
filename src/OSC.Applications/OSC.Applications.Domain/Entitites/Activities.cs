using System.ComponentModel.DataAnnotations;
using EnumFastToStringGenerated;

namespace OSC.Applications.Domain.Entitites;

/// <summary>
/// Доступные типы конференций
/// </summary>
[EnumGenerator]
public enum Activity
{
    [Display(Description = "Доклад, 35-45 минут")] Report = 1,

    [Display(Description = "Мастеркласс, 1-2 часа")] Masterclass = 2,

    [Display(Description = "Дискуссия / круглый стол, 40-50 минут")]
    Discussion = 3,
}