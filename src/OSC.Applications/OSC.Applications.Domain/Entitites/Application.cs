using System.ComponentModel.DataAnnotations;

namespace OSC.Applications.Domain.Entitites;

/// <summary>
/// Заявка на участие
/// </summary>
public sealed class Application
{
    /// <summary>
    /// Инициализирует новый черновик заявки
    /// </summary>
    /// <param name="authorId">Идентификатор автора</param>
    /// <param name="activity">Тип мероприятия</param>
    /// <param name="name">Название мероприятия</param>
    /// <param name="description">Краткое описание</param>
    /// <param name="outline">Содержимое заявки</param>
    /// <param name="createdAt">Дата создания заявки</param>
    /// <remarks>Тип заявки - <see cref="ApplicationStatus.Draft"/></remarks>
    public Application(Guid authorId, Activity? activity, string name, string description, string outline, DateTimeOffset createdAt)
    {
        AuthorId = authorId;
        Activity = activity;
        Name = name;
        Description = description;
        Outline = outline;
        CreatedAt = createdAt;
        Status = ApplicationStatus.Draft;
    }

    public Application()
    {
        
    }
    
    /// <summary>
    /// Идентификатор заявки
    /// </summary>
    [Key]
    public Guid Id { get; init; }
    
    /// <summary>
    /// Идентификатор автора заявки
    /// </summary>
    public Guid AuthorId { get; init; }
    
    /// <summary>
    /// Тип мероприятия
    /// </summary>
    public Activity? Activity { get; set; }

    /// <summary>
    /// Название мепроприятия
    /// </summary>
    [MaxLength(100)]
    public string? Name { get; set; }
    
    /// <summary>
    /// Краткое описание заявки
    /// </summary>
    [MaxLength(300)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Содержимое заявки
    /// </summary>
    [MaxLength(1000)]
    public string? Outline { get; set; }
    
    /// <summary>
    /// Статус заявки
    /// </summary>
    public ApplicationStatus Status { get; set; }
    
    /// <summary>
    /// Дата создания заявки
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// Дата подачи заявки. Null, если заявка не подана.
    /// </summary>
    public DateTimeOffset? SubmittedAt { get; set; }
}