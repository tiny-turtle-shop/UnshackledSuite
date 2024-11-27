﻿using FluentValidation;

namespace Unshackled.Food.My.Client.Features.Members.Models;

public class FormRemovePasswordModel
{
	public string OldPassword { get; set; } = "";

	public class Validator : AbstractValidator<FormRemovePasswordModel>
	{
		public Validator()
		{
			RuleFor(x => x.OldPassword)
				.NotEmpty().WithMessage("Required");
		}
	}
}
