using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeServices.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Slug = table.Column<string>(type: "varchar(80)", unicode: false, maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IconClass = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    AccentColor = table.Column<string>(type: "varchar(7)", unicode: false, maxLength: 7, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentCategoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCategories_ServiceCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProviderProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Headline = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    VerificationStatus = table.Column<int>(type: "int", nullable: false),
                    VerificationNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: false),
                    ReviewsCount = table.Column<int>(type: "int", nullable: false),
                    CompletedOrdersCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderProfiles_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_ServiceCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Building = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderPortfolioImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderPortfolioImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderPortfolioImages_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderServiceAreas",
                columns: table => new
                {
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServiceAreas", x => new { x.ProviderProfileId, x.AreaId });
                    table.ForeignKey(
                        name: "FK_ProviderServiceAreas_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProviderServiceAreas_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PriceUnit = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderServices_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProviderServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    AddressDetails = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ScheduledDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TimeSlot = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AgreedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CancellationReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    FromStatus = table.Column<int>(type: "int", nullable: true),
                    ToStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistory_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderStatusHistory_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceRequestId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderProfileId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<byte>(type: "tinyint", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.CheckConstraint("CK_Review_Rating", "[Rating] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_ProviderProfiles_ProviderProfileId",
                        column: x => x.ProviderProfileId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_ServiceRequests_ServiceRequestId",
                        column: x => x.ServiceRequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "DisplayOrder", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, true, "دمشق" },
                    { 2, 2, true, "ريف دمشق" },
                    { 3, 3, true, "حلب" },
                    { 4, 4, true, "حمص" },
                    { 5, 5, true, "حماة" },
                    { 6, 6, true, "اللاذقية" },
                    { 7, 7, true, "طرطوس" },
                    { 8, 8, true, "إدلب" },
                    { 9, 9, true, "درعا" },
                    { 10, 10, true, "السويداء" },
                    { 11, 11, true, "القنيطرة" },
                    { 12, 12, true, "دير الزور" },
                    { 13, 13, true, "الرقة" },
                    { 14, 14, true, "الحسكة" }
                });

            migrationBuilder.InsertData(
                table: "ServiceCategories",
                columns: new[] { "Id", "AccentColor", "CreatedAt", "Description", "DisplayOrder", "IconClass", "ImagePath", "IsActive", "Name", "ParentCategoryId", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "#E8A317", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "تمديدات كهربائية، إصلاح الأعطال، الإنارة وأنظمة الطاقة الشمسية.", 1, "fa-solid fa-bolt", null, true, "الكهرباء", null, "electricity", null },
                    { 2, "#2D7FF9", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "كشف التسريبات، التمديدات الصحية وتركيب الأدوات الصحية.", 2, "fa-solid fa-faucet-drip", null, true, "السباكة", null, "plumbing", null },
                    { 3, "#E0567A", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "دهان داخلي وخارجي، ديكورات جبسية وورق جدران.", 3, "fa-solid fa-paint-roller", null, true, "الدهان والديكور", null, "painting", null },
                    { 4, "#17A2B8", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "تركيب وصيانة المكيفات وإصلاح البرادات والمجمدات.", 4, "fa-solid fa-snowflake", null, true, "التكييف والتبريد", null, "ac-refrigeration", null },
                    { 5, "#7B61FF", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "نقل الأثاث وفكه وتركيبه وتغليفه بأمان.", 5, "fa-solid fa-truck-moving", null, true, "نقل العفش", null, "furniture-moving", null },
                    { 6, "#F2703D", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "طباخون وطباخات للطبخ اليومي والولائم وتحضير المونة.", 6, "fa-solid fa-utensils", null, true, "الطبخ", null, "cooking", null },
                    { 7, "#20A779", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "تنظيف شامل ودوري، تنظيف السجاد والكنب وما بعد الإكساء.", 7, "fa-solid fa-broom", null, true, "تنظيف المنازل", null, "home-cleaning", null },
                    { 8, "#5B6B7F", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "نجارة، ألمنيوم وزجاج، تركيب ستائر وأعمال صيانة متنوعة.", 8, "fa-solid fa-screwdriver-wrench", null, true, "الصيانة العامة", null, "general-maintenance", null }
                });

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id", "CityId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, true, "المزة" },
                    { 2, 1, true, "المالكي" },
                    { 3, 1, true, "أبو رمانة" },
                    { 4, 1, true, "كفرسوسة" },
                    { 5, 1, true, "المهاجرين" },
                    { 6, 1, true, "الشعلان" },
                    { 7, 1, true, "باب توما" },
                    { 8, 1, true, "القصاع" },
                    { 9, 1, true, "ركن الدين" },
                    { 10, 1, true, "الميدان" },
                    { 11, 1, true, "المزرعة" },
                    { 12, 1, true, "برزة" },
                    { 13, 1, true, "مشروع دمر" },
                    { 14, 1, true, "الشاغور" },
                    { 15, 1, true, "الصالحية" },
                    { 16, 1, true, "القصور" },
                    { 17, 1, true, "التجارة" },
                    { 18, 2, true, "جرمانا" },
                    { 19, 2, true, "صحنايا" },
                    { 20, 2, true, "أشرفية صحنايا" },
                    { 21, 2, true, "قدسيا" },
                    { 22, 2, true, "التل" },
                    { 23, 2, true, "دوما" },
                    { 24, 2, true, "يعفور" },
                    { 25, 2, true, "الكسوة" },
                    { 26, 2, true, "جديدة عرطوز" },
                    { 27, 2, true, "الزبداني" },
                    { 28, 2, true, "داريا" },
                    { 29, 2, true, "معضمية الشام" },
                    { 30, 3, true, "الفرقان" },
                    { 31, 3, true, "الشهباء" },
                    { 32, 3, true, "الموكامبو" },
                    { 33, 3, true, "السبيل" },
                    { 34, 3, true, "الحمدانية" },
                    { 35, 3, true, "الجميلية" },
                    { 36, 3, true, "العزيزية" },
                    { 37, 3, true, "حلب الجديدة" },
                    { 38, 3, true, "السليمانية" },
                    { 39, 3, true, "المحافظة" },
                    { 40, 3, true, "السريان" },
                    { 41, 3, true, "الأشرفية" },
                    { 42, 4, true, "الإنشاءات" },
                    { 43, 4, true, "الوعر" },
                    { 44, 4, true, "الغوطة" },
                    { 45, 4, true, "عكرمة" },
                    { 46, 4, true, "الحمراء" },
                    { 47, 4, true, "الزهراء" },
                    { 48, 4, true, "الخالدية" },
                    { 49, 4, true, "باب السباع" },
                    { 50, 5, true, "مركز المدينة" },
                    { 51, 5, true, "الحاضر" },
                    { 52, 5, true, "جنوب الملعب" },
                    { 53, 5, true, "الشريعة" },
                    { 54, 5, true, "الأربعين" },
                    { 55, 5, true, "طريق حلب" },
                    { 56, 5, true, "القصور" },
                    { 57, 6, true, "الزراعة" },
                    { 58, 6, true, "الصليبة" },
                    { 59, 6, true, "الأمريكان" },
                    { 60, 6, true, "الرمل الشمالي" },
                    { 61, 6, true, "الرمل الجنوبي" },
                    { 62, 6, true, "الشيخ ضاهر" },
                    { 63, 6, true, "الطابيات" },
                    { 64, 6, true, "الشاطئ الأزرق" },
                    { 65, 7, true, "مركز المدينة" },
                    { 66, 7, true, "الكورنيش البحري" },
                    { 67, 7, true, "بانياس" },
                    { 68, 7, true, "صافيتا" },
                    { 69, 7, true, "الدريكيش" },
                    { 70, 8, true, "مركز المدينة" },
                    { 71, 8, true, "أريحا" },
                    { 72, 8, true, "جسر الشغور" },
                    { 73, 8, true, "معرة النعمان" },
                    { 74, 9, true, "درعا المحطة" },
                    { 75, 9, true, "درعا البلد" },
                    { 76, 9, true, "إزرع" },
                    { 77, 10, true, "مركز المدينة" },
                    { 78, 10, true, "شهبا" },
                    { 79, 10, true, "صلخد" },
                    { 80, 11, true, "خان أرنبة" },
                    { 81, 12, true, "مركز المدينة" },
                    { 82, 12, true, "الميادين" },
                    { 83, 12, true, "البوكمال" },
                    { 84, 13, true, "مركز المدينة" },
                    { 85, 13, true, "الطبقة" },
                    { 86, 14, true, "مركز المدينة" },
                    { 87, 14, true, "القامشلي" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "DisplayOrder", "IsActive", "Name", "Slug", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "تمديدات كهربائية جديدة", "electrical-wiring", null },
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "إصلاح الأعطال الكهربائية", "electrical-repair", null },
                    { 3, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "تركيب الإنارة والثريات", "lighting-installation", null },
                    { 4, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "أنظمة الطاقة الشمسية والبطاريات", "solar-systems", null },
                    { 5, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "كشف وإصلاح التسريبات", "leak-repair", null },
                    { 6, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "تمديدات صحية", "plumbing-installation", null },
                    { 7, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "تركيب الأدوات الصحية", "sanitary-fixtures", null },
                    { 8, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "تسليك المجاري", "drain-cleaning", null },
                    { 9, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "دهان داخلي", "interior-painting", null },
                    { 10, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "دهان خارجي", "exterior-painting", null },
                    { 11, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "ديكورات جبسية", "gypsum-decor", null },
                    { 12, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "ورق جدران", "wallpaper", null },
                    { 13, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "تركيب المكيفات", "ac-installation", null },
                    { 14, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "صيانة وتعبئة غاز المكيفات", "ac-maintenance", null },
                    { 15, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "إصلاح البرادات والمجمدات", "fridge-repair", null },
                    { 16, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "نقل عفش المنازل", "home-moving", null },
                    { 17, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "فك وتركيب الأثاث", "furniture-assembly", null },
                    { 18, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "تغليف الأثاث", "furniture-packing", null },
                    { 19, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "نقل المكاتب", "office-moving", null },
                    { 20, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "طبخ يومي منزلي", "daily-cooking", null },
                    { 21, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "طبخ الولائم والمناسبات", "event-cooking", null },
                    { 22, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "تحضير المونة", "pantry-preparation", null },
                    { 23, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "حلويات منزلية", "homemade-sweets", null },
                    { 24, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "تنظيف شامل للمنزل", "deep-cleaning", null },
                    { 25, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "تنظيف دوري", "regular-cleaning", null },
                    { 26, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "تنظيف السجاد والكنب", "carpet-sofa-cleaning", null },
                    { 27, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "تنظيف بعد الإكساء", "post-construction-cleaning", null },
                    { 28, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "نجارة وإصلاح الأبواب", "carpentry", null },
                    { 29, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "ألمنيوم وزجاج", "aluminum-glass", null },
                    { 30, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 3, true, "تركيب الستائر والإكسسوارات", "curtains-installation", null },
                    { 31, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, true, "صيانة عامة للمنزل", "handyman", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_CityId_Name",
                table: "Areas",
                columns: new[] { "CityId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_AreaId",
                table: "CustomerAddresses",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_UserId",
                table: "CustomerAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistory_ChangedByUserId",
                table: "OrderStatusHistory",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistory_ServiceRequestId",
                table: "OrderStatusHistory",
                column: "ServiceRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderPortfolioImages_ProviderProfileId",
                table: "ProviderPortfolioImages",
                column: "ProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfiles_CityId",
                table: "ProviderProfiles",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfiles_UserId",
                table: "ProviderProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderProfiles_VerificationStatus_AverageRating",
                table: "ProviderProfiles",
                columns: new[] { "VerificationStatus", "AverageRating" });

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServiceAreas_AreaId",
                table: "ProviderServiceAreas",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServices_Price",
                table: "ProviderServices",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServices_ProviderProfileId_ServiceId",
                table: "ProviderServices",
                columns: new[] { "ProviderProfileId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServices_ServiceId",
                table: "ProviderServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_CustomerId",
                table: "Reviews",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProviderProfileId_IsHidden",
                table: "Reviews",
                columns: new[] { "ProviderProfileId", "IsHidden" });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ServiceRequestId",
                table: "Reviews",
                column: "ServiceRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategories_ParentCategoryId",
                table: "ServiceCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCategories_Slug",
                table: "ServiceCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_AreaId",
                table: "ServiceRequests",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_CustomerId_Status",
                table: "ServiceRequests",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_OrderNumber",
                table: "ServiceRequests",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ProviderProfileId_Status",
                table: "ServiceRequests",
                columns: new[] { "ProviderProfileId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ServiceId",
                table: "ServiceRequests",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CategoryId",
                table: "Services",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Slug",
                table: "Services",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrderStatusHistory");

            migrationBuilder.DropTable(
                name: "ProviderPortfolioImages");

            migrationBuilder.DropTable(
                name: "ProviderServiceAreas");

            migrationBuilder.DropTable(
                name: "ProviderServices");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "ProviderProfiles");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "ServiceCategories");
        }
    }
}
