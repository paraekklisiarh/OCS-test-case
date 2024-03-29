using System.Net;
using System.Net.Http.Json;
using OCS.Applications.Contracts.Requests;
using OCS.Applications.Domain.Entitites;
using OCS.Applications.Tests.Fixtures;

namespace OCS.Applications.Tests;

[Collection("Applications API collection")]
public class ApplicationsApiConstraintsTests(ApplicationsApiFactory applicationsApiFactory)
    : ControllerTestsBase(applicationsApiFactory)
{
    private const string ValidName = "Valid Name";
    private const string ValidOutline = "Valid Outline";
    private const string ValidDescription = "Good Description";

    [Fact(DisplayName = "у пользователя может только одна не отправленная заявка (черновик заявки)")]
    public async Task CanNotAddSecondApplication()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        var newApplication = new CreateApplicationDto
            { AuthorId = application.AuthorId, Description = ValidDescription };

        // Act
        var response = await Client.PostAsync("applications", JsonContent.Create(newApplication));

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact(DisplayName = "Можно создать черновик заявки, если существует отправленная заявка")]
    public async Task CanCreateDraftApplicationIfSubmittedApplicationExists()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Submitted };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        var newApplication = new CreateApplicationDto
        {
            AuthorId = application.AuthorId, Description = ValidDescription, Activity = Activity.Report,
            Outline = ValidOutline, Name = ValidName
        };

        // Act
        var response = await Client.PostAsync("applications", JsonContent.Create(newApplication));

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // ToDo: to Theory
    [Fact(DisplayName = "нельзя создать заявку не указав идентификатор пользователя")]
    public async Task CanNotCreateApplicationWithoutAuthorId()
    {
        // Arrange
        var newApplication = new CreateApplicationDto
        {
            AuthorId = Guid.Empty, Description = ValidDescription, Activity = Activity.Report, Outline = ValidOutline,
            Name = ValidName
        };

        // Act
        var response = await Client.PostAsync("applications", JsonContent.Create(newApplication));

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "нельзя создать заявку не указав хотя бы еще одно поле помимо идентификатора пользователя")]
    public async Task CanNotCreateApplicationWithoutSetFields()
    {
        // Arrange
        var newApplication = new CreateApplicationDto
            { AuthorId = Guid.NewGuid(), Activity = null, Description = null, Name = null, Outline = null };

        // Act
        var response = await Client.PostAsync("applications", JsonContent.Create(newApplication));

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName =
        "нельзя отредактировать заявку так, чтобы  в ней не остались заполненными идентификатор пользователя + еще одно поле")]
    public async Task CanNotEditApplicationWithoutSetFields()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        var newApplication = new UpdateApplicationDto(null, null, null, null);

        // Act
        var response = await Client.PutAsync($"applications/{application.Id}", JsonContent.Create(newApplication));

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "нельзя редактировать отправленные на рассмотрение заявки")]
    public async Task CanNotEditSubmittedApplication()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Submitted };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        var correctUpdate = new UpdateApplicationDto(Activity.Report, ValidName, ValidDescription, ValidOutline);

        // Act
        var response = await Client.PutAsync($"applications/{application.Id}", JsonContent.Create(correctUpdate));

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "нельзя отменить / удалить заявки отправленные на рассмотрение")]
    public async Task CanNotDeleteSubmittedApplication()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Submitted };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.DeleteAsync($"applications/{application.Id}");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "нельзя удалить не существующую заявку")]
    public async Task CanNotDeleteNonExistingApplication()
    {
        // Arrange
        var incorrectGuid = Guid.NewGuid();

        // Act
        var response = await Client.DeleteAsync($"applications/{incorrectGuid}");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "нельзя редактировать не существующую заявку")]
    public async Task CanNotEditNonExistingApplication()
    {
        // Arrange
        var incorrectGuid = Guid.NewGuid();
        var correctUpdate = new UpdateApplicationDto(Activity.Report, ValidName, ValidDescription, ValidOutline);

        // Act
        var response = await Client.PutAsync($"applications/{incorrectGuid}", JsonContent.Create(correctUpdate));

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName = "можно отправить на рассмотрение только заявки у которых заполнены все обязательные поля")]
    public async Task CanNotSubmitApplicationWithoutSetFields()
    {
        // Arrange
        var application = new Application { AuthorId = Guid.NewGuid(), Status = ApplicationStatus.Draft };
        await Context.Applications.AddAsync(application);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.PostAsync($"applications/{application.Id}/submit", null);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "нельзя отправить на рассмотрение не существующую заявку")]
    public async Task CanNotSubmitNonExistingApplication()
    {
        // Arrange
        var incorrectGuid = Guid.NewGuid();

        // Act
        var response = await Client.PostAsync($"applications/{incorrectGuid}/submit", null);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact(DisplayName =
        "запрос на получение поданных и не поданных заявок одновременно должен считаться не корректным")]
    public async Task CanNotGetSubmittedAndNotSubmittedApplications()
    {
        // Arrange
        var submittedAfter = DateTime.Now;
        var notSubmittedBefore = DateTime.Now.AddDays(1);

        // Act
        var response =
            await Client.GetAsync(
                $"applications?submittedAfter={submittedAfter}&unsubmittedOlder={notSubmittedBefore}");

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}