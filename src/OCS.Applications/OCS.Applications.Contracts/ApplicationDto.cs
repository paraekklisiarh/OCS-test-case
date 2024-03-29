using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OCS.Applications.Domain.Entitites;

namespace OCS.Applications.Contracts;

public class ApplicationDto
{
    public ApplicationDto()
    {
        
    }

    /// <summary>
    /// Идентификатор заявки
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Идентификатор автора заявки
    /// </summary>
    [JsonPropertyName("author")]
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