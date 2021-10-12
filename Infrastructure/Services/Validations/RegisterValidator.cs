using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Application.DTOs;

namespace Infrastructure.Services.Validations
{
    public class RegisterValidator : AbstractValidator<RegisterDTO> 
    {
        public RegisterValidator()
        {
            RuleFor(user => user.FirstName).NotEmpty().Length(3, 10);
            RuleFor(user => user.LastName).NotEmpty().Length(3, 26);
            RuleFor(user => user.Password).NotEmpty().Length(8, 30);
            RuleFor(user => user.ConfirmedPassword).NotEmpty().Length(8, 30)
            .Equal(user => user.Password).WithMessage("Hasła muszą być takie same");
            RuleFor(user => user.Email).NotEmpty().EmailAddress();
            RuleFor(user => user.PhoneNumber).NotEmpty().Length(7, 13); 
            RuleFor(user => user.Description).MaximumLength(200);
            RuleFor(user => user.DateOfBirth).NotEmpty();
        }
    }
}
