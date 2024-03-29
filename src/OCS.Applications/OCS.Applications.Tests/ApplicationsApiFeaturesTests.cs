using System.Net.Http.Json;
using System.Text.Json;
using EnumFastToStringGenerated;
using Microsoft.EntityFrameworkCore;
using OCS.Applications.Contracts;
using OCS.Applications.Contracts.Requests;
using OCS.Applications.Contracts.Responses;
using OCS.Applications.Domain.Entitites;
using OCS.Applications.Tests.Fixtures;
using OCS.Applications.Tests.Helpers;

namespace OCS.Applications.Tests;

[Collection("Applications API collection")]
public class ApplicationsApiFeaturesTests(ApplicationsApiFactory applicationsApiFactory)
    : ControllerTestsBase(applicationsApiFactory)
{
    private const string ValidName = "Valid Name";
    private const string ValidOutline = "Valid Outline";
    private const string ValidDescription = "Good Description";

    [Fact(DisplayName = "создание черновика заявки успешно")]
    public async Task CanCreateDraftApplication()
    {
        // Arrange
        var newApplication = new CreateApplicationDto
        {
            AuthorId = Guid.NewGuid(), Description = ValidDescription, Activity = Activity.Report, Name = ValidName,
            Outline = ValidOutline
        };
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonOptions.Converters.Add(new ActivityConverter());

        // Act
        var response = await Client.PostAsync("applications", JsonContent.Create(newApplication));
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApplicationDto>(json, jsonOptions);
        
        var entity = await Context.Applications.FirstOrDefaultAsync(x =>
            x.AuthorId == newApplication.AuthorId && x.Status == ApplicationStatus.Draft);

        // Assert
        Assert.NotNull(entity);
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal(newApplication.AuthorId, result.AuthorId);
    }

    [Fact(DisplayName = "редактирование черновика заявки успешно")]
    public async Task CanEditDraftApplication()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var correctUpdate = new UpdateApplicationDto(Activity.Report, ValidName, ValidDescription, ValidOutline);

        // Act
        var response = await Client.PutAsync($"applications/{application.Id}", JsonContent.Create(correctUpdate));
        var entity = await Context.Applications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == application.Id);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(entity);
        Assert.Equal(correctUpdate.Activity, entity.Activity);
        Assert.Equal(correctUpdate.Name, entity.Name);
        Assert.Equal(correctUpdate.Outline, entity.Outline);
        Assert.Equal(correctUpdate.Description, entity.Description);
    }

    [Fact(DisplayName = "удаление черновика заявки успешно")]
    public async Task CanDeleteDraftApplication()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        // Act
        var response = Client.DeleteAsync($"applications/{application.Id}");

        // Assert
        Assert.True(response.Result.IsSuccessStatusCode);
        Assert.False(Context.Applications.Any(x => x.Id == application.Id));
    }

    [Fact(DisplayName = "отправка корректной заявки на рассмотрение программным комитетом успешна")]
    public async Task CanSubmitCorrectApplication()
    {
        // Arrange
        var application = new Application
        {
            AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft, Activity = Activity.Report,
            Description = ValidDescription, Name = ValidName, Outline = ValidOutline, CreatedAt = DateTimeOffset.UtcNow
        };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        // Act
        var response = await Client.PostAsync($"applications/{application.Id}/submit", null);
        var entity = await Context.Applications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == application.Id);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(ApplicationStatus.Submitted, entity!.Status);
    }

    [Fact(DisplayName = "получение заявок поданных после указанной даты успешно")]
    public async Task CanGetSubmittedApplicationsAfterDate()
    {
        // Arrange 
        var targetDate = DateTimeOffset.UtcNow.AddDays(-10);
        var formattedTime = targetDate.ToString("yyyy-MM-dd HH:mm:ss.ff");
        var applications = new List<Application>
        {
            new()
            {
                AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Submitted,
                SubmittedAt = targetDate.AddDays(1)
            },
            new()
            {
                AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Submitted,
                SubmittedAt = targetDate.AddDays(2)
            }
        };
        await Context.Applications.AddRangeAsync(applications);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"applications?submittedAfter={formattedTime}");
        var result = await response.Content.ReadFromJsonAsync<List<ApplicationDto>>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(applications.Count, result.Count);
    }

    [Fact(DisplayName = "получение заявок не поданных и старше определенной даты успешно")]
    public async Task CanGetNotSubmittedApplicationsOlderThanDate()
    {
        // Arrange 
        var targetDate = DateTimeOffset.UtcNow.AddDays(-10);
        var formattedTime = targetDate.ToString("yyyy-MM-dd HH:mm:ss.ff");
        var applications = new List<Application>
        {
            new()
            {
                AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft,
                SubmittedAt = targetDate.AddDays(-1)
            },
            new()
            {
                AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft,
                SubmittedAt = targetDate.AddDays(-2)
            }
        };
        await Context.Applications.AddRangeAsync(applications);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"applications?unsubmittedOlder={formattedTime}");
        var result = await response.Content.ReadFromJsonAsync<List<ApplicationDto>>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(applications.Count, result.Count);
    }

    [Fact(DisplayName = "получение текущей не поданной заявки для указанного пользователя успешно")]
    public async Task CanGetCurrentNotSubmittedApplication()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        // Act
        var response = Client.GetAsync($"users/{application.AuthorId}/currentapplication");
        var result = await response.Result.Content.ReadFromJsonAsync<ApplicationDto>();

        // Assert
        Assert.True(response.Result.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(application.Id, result.Id);
    }

    [Fact(DisplayName = "Можно получить заявку по идентификатору")]
    public async Task CanGetApplicationById()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"applications/{application.Id}");
        var result = await response.Content.ReadFromJsonAsync<ApplicationDto>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(application.Id, result.Id);
    }

    [Fact(DisplayName = "Можно получить список возможных активностей")]
    public async Task CanGetAvailableActivities()
    {
        // Act
        var response = await Client.GetAsync("activities");
        var result = await response.Content.ReadFromJsonAsync<List<ActivitiesResponse>>();
        
        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.Equal(ActivityEnumExtensions.GetLengthFast(), result.Count );
    }
}