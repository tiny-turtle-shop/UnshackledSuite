﻿using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Members;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<MemberEntity, Member>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
	}
}
