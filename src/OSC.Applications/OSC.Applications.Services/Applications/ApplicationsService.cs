using FluentValidation;
using Mapster;
using Microsoft.Extensions.Logging;
using OSC.Applications.Contracts;
using OSC.Applications.Contracts.Requests;
using OSC.Applications.Domain.Entitites;
using OSC.Applications.Domain.Repository;

namespace OSC.Applications.Services.Applications;

public sealed class ApplicationsService(
    IRepositoryManager repositoryManager,
    IValidator<Application> submitValidator,
    ILogger<ApplicationsService> logger
)
    : IApplicationsService
{
    public async Task<OperationResult<ApplicationDto>> CreateDraftAsync(CreateApplicationDto dto,
        CancellationToken cancellationToken)
    {
        if (await repositoryManager.ApplicationsRepository.AnyUnsubmittedByAuthorIdAsync(dto.AuthorId,
                cancellationToken))
        {
            logger.LogInformation("Draft application with author {dto.AuthorId} already exists", dto.AuthorId);
            return new OperationResult<ApplicationDto>(false, OperationResultType.Conflict)
                { ErrorMessage = $"Draft application with author id {dto.AuthorId} already exists" };
        }

        // Todo: Выглядит как костыль
        var entity = dto.Adapt<Application>();
        entity.CreatedAt = DateTimeOffset.UtcNow;
        entity.Status = ApplicationStatus.Draft;
        
        var result = await repositoryManager.ApplicationsRepository
            .AddAsync(entity, cancellationToken);
        await repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created draft application: {ApplicationId}", result.Id);
        return new OperationResult<ApplicationDto>(true, OperationResultType.Success,
            result.Adapt<ApplicationDto>());
    }

    public async Task<OperationResult<ApplicationDto>> UpdateAsync(
        Guid applicationId,
        UpdateApplicationDto dto,
        CancellationToken cancellationToken)
    {
        // проверяем существует ли заявка
        var application = await repositoryManager.ApplicationsRepository.GetByIdAsync(applicationId, cancellationToken);
        if (application is null)
        {
            logger.LogInformation("Application with id {applicationId} not found", applicationId);
            return new OperationResult<ApplicationDto>(false, OperationResultType.NotFound);
        }
        
        // Нельзя редактировать отправленные на рассмотрение заявки
        if (application.Status != ApplicationStatus.Draft)
        {
            logger.LogInformation("Application with id {applicationId} is not in draft status", applicationId);
            return new OperationResult<ApplicationDto>(false, OperationResultType.Forbidden)
                { ErrorMessage = $"Application with id {applicationId} is not in draft status" };
        }

        application.Name = dto.Name;
        application.Activity = dto.Activity;
        application.Description = dto.Description;
        application.Outline = dto.Outline;

        repositoryManager.ApplicationsRepository.Update(application);
        await repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Updated dto {ApplicationId}", applicationId);
        return new OperationResult<ApplicationDto>(true, OperationResultType.Success,
            application.Adapt<ApplicationDto>());
    }

    public async Task<OperationResult<ApplicationDto>> DeleteAsync(Guid applicationId,
        CancellationToken cancellationToken)
    {
        var application = await repositoryManager.ApplicationsRepository.GetByIdAsync(applicationId, cancellationToken);
        // нельзя удалить или редактировать не существующую заявку
        if (application is null)
        {
            logger.LogInformation("Application with id {applicationId} not found", applicationId);
            return new OperationResult<ApplicationDto>(false, OperationResultType.NotFound);
        }

        // нельзя отменить / удалить заявки отправленные на рассмотрение
        if (application.Status != ApplicationStatus.Draft)
        {
            logger.LogInformation("Application with id {applicationId} cannot be deleted", applicationId);
            return new OperationResult<ApplicationDto>(false, OperationResultType.Forbidden)
                { ErrorMessage = "Application cannot be deleted" };
        }

        await repositoryManager.ApplicationsRepository.DeleteAsync(application, cancellationToken);
        await repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted dto {ApplicationId}", applicationId);
        return new OperationResult<ApplicationDto>(true, OperationResultType.Success);
    }

    public async Task<OperationResult<ApplicationDto>> GetAsync(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await repositoryManager.ApplicationsRepository.GetByIdAsync(applicationId, cancellationToken);

        if (result is not null)
            return new OperationResult<ApplicationDto>(true, OperationResultType.Success,
                result.Adapt<ApplicationDto>());

        logger.LogInformation("Application with id {applicationId} not found", applicationId);
        return new OperationResult<ApplicationDto>(false, OperationResultType.NotFound);
    }

    public async Task<OperationResult<ApplicationDto>> GetSubmittedAfterAsync(DateTimeOffset submittedAfter,
        CancellationToken cancellationToken)
    {
        var results =
            await repositoryManager.ApplicationsRepository.GetSubmittedAfterAsync(submittedAfter, cancellationToken);

        return new OperationResult<ApplicationDto>(true, OperationResultType.Success,
            results.Select(a => a.Adapt<ApplicationDto>()).ToList());
    }

    public async Task<OperationResult<ApplicationDto>> GetUnsubmittedOlderAsync(DateTimeOffset unsubmittedOlder,
        CancellationToken cancellationToken)
    {
        var results =
            await repositoryManager.ApplicationsRepository.GetUnsubmittedOlderAsync(unsubmittedOlder,
                cancellationToken);

        return new OperationResult<ApplicationDto>(true, OperationResultType.Success,
            results.Select(a => a.Adapt<ApplicationDto>()).ToList());
    }

    public async Task<OperationResult<ApplicationDto>> SubmitAsync(Guid applicationId,
        CancellationToken cancellationToken)
    {
        // проверка наличия заявки
        var application = await repositoryManager.ApplicationsRepository.GetByIdAsync(applicationId, cancellationToken);
        if (application is null)
        {
            logger.LogInformation("Application with id {applicationId} not found", applicationId.ToString());
            return new OperationResult<ApplicationDto>(false, OperationResultType.NotFound);
        }

        // валидация заявки
        var validationResult = await submitValidator.ValidateAsync(application, cancellationToken);
        if (validationResult.IsValid is false)
        {
            logger.LogInformation("Application with id {applicationId} cannot be submitted", applicationId.ToString());
            return new OperationResult<ApplicationDto>(false, OperationResultType.ValidationError)
                { ErrorMessage = validationResult.ToString() };
        }

        // только черновик может быть подан на рассмотрение
        if (application.Status is not ApplicationStatus.Draft)
        {
            logger.LogInformation("Application with id {applicationId} already submitted", applicationId.ToString());
            return new OperationResult<ApplicationDto>(false, OperationResultType.Conflict);
        }

        application.Status = ApplicationStatus.Submitted;
        application.SubmittedAt = DateTimeOffset.UtcNow;

        repositoryManager.ApplicationsRepository.Update(application);
        await repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Submitted dto {ApplicationId}", applicationId);
        return new OperationResult<ApplicationDto>(true, OperationResultType.Success);
    }

    public async Task<OperationResult<ApplicationDto>> GetUnsubmittedByAuthorAsync(Guid authorId,
        CancellationToken cancellationToken)
    {
        var result = await repositoryManager.ApplicationsRepository.GetUnsubmittedByAuthorAsync(authorId, cancellationToken);

        return new OperationResult<ApplicationDto>(true, OperationResultType.Success, result.Adapt<ApplicationDto>());
    }
}