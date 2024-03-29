using System.ComponentModel.DataAnnotations;
using OCS.Applications.Domain.Entitites;

namespace OCS.Applications.Contracts;

public class ApplicationDto
{
    public ApplicationDto(Guid id, Guid authorId, Activity? activity, string name, string description, string outline)
    {
        Id = id;
        AuthorId = authorId;
        Activity = activity;
        Name = name;
        Description = description;
        Outline = outline;
    }

    /// <summary>
    /// Идентификатор заявки
    /// </summary>
    public Guid? Id { get; init; }
    
    /// <summary>
    /// Идентификатор автора заявки
    /// </summary>
    [Required]
    public Guid AuthorId { get; init; }
    
    /// <summary>
    /// Тип мероприятия
    /// </summary>
    public Activity? Activity { get; set; }
    
    /// <summary>
    /// Название мепроприятия
    /// </summary>
    [MaxLength(100)]
    public string Name { get; set; }
    
    /// <summary>
    /// Краткое описание заявки
    /// </summary>
    [MaxLength(300)]
    public string Description { get; set; }
    
    /// <summary>
    /// Содержимое заявки
    /// </summary>
    [MaxLength(1000)]
    public string Outline { get; set; }
}