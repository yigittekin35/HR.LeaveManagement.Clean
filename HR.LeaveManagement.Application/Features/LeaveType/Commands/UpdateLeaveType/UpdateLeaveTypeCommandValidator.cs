using FluentValidation;
using HR.LeaveManagement.Application.Contracts.Persistence;

namespace HR.LeaveManagement.Application.Features.LeaveType.Commands.UpdateLeaveType;

public class UpdateLeaveTypeCommandValidator : AbstractValidator<UpdateLeaveTypeCommand>
{
    ILeaveTypeRepository _leaveTypeRepository;

    public UpdateLeaveTypeCommandValidator(ILeaveTypeRepository leaveTypeRepository)
    {
        _leaveTypeRepository = leaveTypeRepository;

        RuleFor(p => p)
            .NotNull()
            .MustAsync(LeaveTypeMustExist);

        RuleFor(p => p.Name)
               .NotEmpty().WithMessage("{PropertyName} is required")
               .NotNull()
               .MaximumLength(70).WithMessage("{PropertyName} must be fewer than 70 characters");

        RuleFor(p => p.DefaultDays)
              .GreaterThan(1).WithMessage("{PropertyName} must be greater than 1")
              .LessThan(100).WithMessage("{PropertyName} must be less than 100");

        RuleFor(p => p)
            .MustAsync(LeaveTypeNameUnique).WithMessage("Leave type already exists");
    }

    private async Task<bool> LeaveTypeNameUnique(UpdateLeaveTypeCommand command, CancellationToken token)
    {
        return await _leaveTypeRepository.IsLeaveTypeUniqueAsync(command.Name);
    }

    private async Task<bool> LeaveTypeMustExist(UpdateLeaveTypeCommand command, CancellationToken token)
    {
        var leaveType = await _leaveTypeRepository.GetByIdAsync(command.Id);
        return leaveType is not null;
    }
}
