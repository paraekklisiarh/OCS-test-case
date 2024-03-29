using System.ComponentModel;

namespace OSC.Applications.Domain.Entitites;

/// <summary>
/// Доступные типы конференций
/// </summary>
public enum Activity
{
    [Description("Доклад, 35-45 минут")] Report = 1,

    [Description("Мастеркласс, 1-2 часа")] Masterclass = 2,

    [Description("Дискуссия / круглый стол, 40-50 минут")]
    Discussion = 3,
}