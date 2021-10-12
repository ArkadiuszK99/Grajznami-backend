using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Application.DTO;

namespace Infrastructure.Services.Validation
{
    public class AddEventValidator : AbstractValidator<AddEventDTO>
    {
        public AddEventValidator()
        {
            RuleFor(@event => @event.Name).Length(5, 30);
            RuleFor(@event => @event.Description).MaximumLength(200);
            RuleFor(@event => @event.Date).NotEmpty();
            RuleFor(@event => @event.UsersLimit).NotEmpty().NotNull();
            RuleFor(@event => @event.IsFree).NotNull();
            RuleFor(@event => @event.IsPublic).NotNull();
        }
    }
}
