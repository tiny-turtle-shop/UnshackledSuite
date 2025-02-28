﻿using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Products.Models;
using Unshackled.Fitness.My.Client.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Products;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<ProductCategoryEntity, CategoryModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.HouseholdSid, m => m.MapFrom(s => s.HouseholdId.Encode()));
		CreateMap<ProductEntity, MergeProductModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.Category, m => m.MapFrom(s => s.Category != null ? s.Category.Title : "Uncategorized"));
		CreateMap<ProductEntity, ProductListModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.Category, m => m.MapFrom(s => s.Category != null ? s.Category.Title : null))
			.ForMember(d => d.HouseholdSid, m => m.MapFrom(s => s.HouseholdId.Encode()));
		CreateMap<ProductEntity, ProductModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.CategorySid, m => m.MapFrom(s => s.ProductCategoryId.HasValue ? s.ProductCategoryId.Value.Encode() : string.Empty))
			.ForMember(d => d.Category, m => m.MapFrom(s => s.Category != null ? s.Category.Title : null))
			.ForMember(d => d.HouseholdSid, m => m.MapFrom(s => s.HouseholdId.Encode()));
		CreateMap<ProductImageEntity, ImageModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
		CreateMap<ShoppingListEntity, ShoppingListModel>()
			.ForMember(d => d.HouseholdSid, m => m.MapFrom(s => s.HouseholdId.Encode()))
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
	}
}
