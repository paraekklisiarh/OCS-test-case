using System.ComponentModel.DataAnnotations;
using OCS.Applications.Domain.Entitites;

namespace OCS.Applications.Contracts.Requests;

public class UpdateApplicationDto(Activity? activity, string name, string description, string outline)
{
    /// <summary>
    /// Тип мероприятия
    /// </summary>
    public Activity? Activity { get; set; } = activity;

    /// <summary>
    /// Название мепроприятия
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; } = name;

    /// <summary>
    /// Краткое описание заявки
    /// </summary>
    [MaxLength(300)]
    public string Description { get; set; } = description;

    /// <summary>
    /// Содержимое заявки
    /// </summary>
    [MaxLength(1000)]
    public string Outline { get; set; } = outline;
}