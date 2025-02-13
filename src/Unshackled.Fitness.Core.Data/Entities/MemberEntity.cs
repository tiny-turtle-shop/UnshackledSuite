﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unshackled.Studio.Core.Client.Enums;

namespace Unshackled.Studio.Core.Data.Entities;

public class MemberEntity : BaseEntity
{
	public string Email { get; set; } = string.Empty;
	public bool IsActive { get; set; }
	public Themes AppTheme { get; set; }

	public class TypeConfiguration : BaseEntityTypeConfiguration<MemberEntity>, IEntityTypeConfiguration<MemberEntity>
	{
		public override void Configure(EntityTypeBuilder<MemberEntity> config)
		{
			base.Configure(config);

			config.ToTable("Members");

			config.Property(x => x.Email)
				 .HasMaxLength(256)
				 .IsRequired();

			config.HasIndex(x => x.Email).IsUnique();
		}
	}
}

