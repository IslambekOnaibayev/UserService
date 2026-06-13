namespace Infrastructure.Data.Config
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(u => u.Id)
                .ValueGeneratedNever()
                .HasVogenConversion()
                .IsRequired();

            builder.Property(u => u.Name)
                .HasVogenConversion()
                .HasMaxLength(UserName.MaxLength)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasVogenConversion()
                .HasMaxLength(UserEmail.MaxLength)
                .IsRequired();

            builder.Property(u => u.NormalizedEmail)
                .HasVogenConversion()
                .HasMaxLength(UserEmail.MaxLength)
                .IsRequired();

            builder.HasIndex(u => u.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("IX_Users_NormalizedEmail");

            builder.Property(u => u.PasswordHash)
                .HasVogenConversion()
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(u => u.Status)
                .HasConversion(
                    s => s.Name,
                    v => UserStatus.FromName(v))
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.EmailConfirmationToken)
                .HasConversion(
                    t => t.HasValue ? t.Value.Value : null,
                    v => v == null ? (EmailConfirmationToken?)null : EmailConfirmationToken.From(v))
                .HasMaxLength(EmailConfirmationToken.MaxLength)
                .IsRequired(false);

            builder.Property(u => u.RegistrationTime).IsRequired();
            builder.Property(u => u.LastLoginTime).IsRequired(false);
            builder.Property(u => u.LastActivityTime).IsRequired(false);
        }
    }
}
