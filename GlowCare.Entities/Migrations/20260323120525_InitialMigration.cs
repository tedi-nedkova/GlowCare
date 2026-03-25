using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GlowCare.Entities.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Memberships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<int>(type: "int", nullable: false),
                    DiscountPercentage = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Memberships", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    MembershipId = table.Column<int>(type: "int", nullable: true),
                    IsSpecialist = table.Column<bool>(type: "bit", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EmployeesSchedules",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesSchedules", x => new { x.EmployeeId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_EmployeesSchedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesSchedules_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesServices",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesServices", x => new { x.EmployeeId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_EmployeesServices_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeesServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedures_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedures_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Procedures_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcedureId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("5dcbdb75-d5c9-4109-ab52-fd869be79532"), "b2222222-2222-2222-2222-222222222222", "User", "USER" },
                    { new Guid("8c384109-e13f-4556-a810-ab9ba28161a2"), "a1111111-1111-1111-1111-111111111111", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "IsSpecialist", "LastName", "LockoutEnabled", "LockoutEnd", "MembershipId", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("29965aaa-46cf-4829-93b8-e38401be7547"), 0, 38, "84f927bd-8a67-4313-85ae-be02c2df51ee", "maria.petrova@gmail.com", true, "Maria", 1, true, "Petrova", false, null, null, "MARIA.PETROVA@EXAMPLE.COM", "MARIA.PETROVA", "AQAAAAIAAYagAAAAEONuv2lXu87BbI152yd1xIBcPLAkMVSyN12t5hekEYDzGaqKia8KFAhSMOCZG7sO4Q==", "0899123456", true, "68e7add5-6f2f-4792-a2e6-01f596c74a52", false, "maria.petrova" },
                    { new Guid("a7d3c5e2-9b41-4f12-8f34-123456789abc"), 0, 32, "d0452ff0-72fc-44ab-85a3-8e9cbe6c31f5", "elena.dimitrova@gmail.com", true, "Elena", 1, true, "Dimitrova", false, null, null, "ELENA.DIMITROVA@EXAMPLE.COM", "ELENA.DIMITROVA", "AQAAAAIAAYagAAAAED5jYSrue7t/zZ0fFuEVbYokPSh2hNfFhxsZEPciAN7srbm+E0xYl2t7If6dSLMi8g==", "0888123456", true, "bfb94cc6-d1c4-4315-8f5e-f1d3535f4c01", false, "elena.dimitrova" },
                    { new Guid("ac31b0bb-d05a-438d-be06-9bfe3323cf08"), 0, 30, "884e0506-74d5-4a84-b754-7d3de0bb758c", "johndoe@gmail.com", true, "John", 0, true, "Doe", false, null, null, "JOHNDOE@EXAMPLE.COM", "JOHN.DOE", "AQAAAAIAAYagAAAAENAvbtaFkKAgjo/f0Bxhjwp91cCD1KpLDJDwLA0BKo5/Aj+yJpoHbqK7pp3AWBrRvg==", "0875757574", true, "a6241046-575b-4a3f-a668-c6aa755a232f", false, "john.doe" },
                    { new Guid("c9f4e7b1-2d33-4a11-8f56-abcdef123456"), 0, 30, "34632a02-9836-48a1-a2e2-9c803e9fb0db", "ivana.koleva@gmail.com", true, "Ivana", 1, true, "Koleva", false, null, null, "IVANA.KOLEVA@EXAMPLE.COM", "IVANA.KOLEVA", "AQAAAAIAAYagAAAAENsZTDnfeUSzE64mqsRZrZA5BVGZyyRTNilF/ggaHYYr+F3WGlU3kPxGWQUUXe3iow==", "0888234567", true, "e0ff1671-f59e-407c-a1bc-b6034b44ac41", false, "ivana.koleva" },
                    { new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760"), 0, 18, "b43d6bfd-e599-47d2-9e6e-e71107a12343", "teodora_nedkova@abv.bg", true, "Teodora", 1, null, "Nedkova", false, null, null, "TEODORA_NEDKOVA@ABV.BG", "TEODORA.NEDKOVA", "AQAAAAIAAYagAAAAEFAf+YMQjWdtdEmjisw0oHpiFdD4Y7n9mulSTAqDYxPR/gmywqYNwUguWLc2N1clJw==", "0878654562", true, "258513a82f1147ba92a0e43f1602b7c3", false, "teodora.nedkova" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Skin Care" },
                    { 2, "Laser Treatments" },
                    { 3, "Massage" },
                    { 4, "Aesthetic Medicine" },
                    { 5, "Facial Treatments" }
                });

            migrationBuilder.InsertData(
                table: "Memberships",
                columns: new[] { "Id", "DiscountPercentage", "Points", "Title" },
                values: new object[,]
                {
                    { 1, 20, 0, 0 },
                    { 2, 10, 500, 1 },
                    { 3, 15, 1000, 2 },
                    { 4, 20, 2000, 3 }
                });

            migrationBuilder.InsertData(
                table: "Schedules",
                columns: new[] { "Id", "DaysOfWeek", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { 1, "Monday,Wednesday,Thursday", "18:00", "09:00" },
                    { 2, "Monday,Wednesday,Friday", "16:00", "10:00" },
                    { 3, "Tuesday,Thursday", "16:00", "9:00" },
                    { 4, "Monday,Tuesday,Wednesday,Thursday,Friday", "15:00", "11:00" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "IsSpecialist", "LastName", "LockoutEnabled", "LockoutEnd", "MembershipId", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("e5c2a9b3-4a67-4f89-8d23-556677889900"), 0, 30, "584b571e-cde7-46b1-a956-7dcaf39cd2ac", "nikol.georgieva@gmail.com", true, "Nikol", 1, null, "Georgieva", false, null, 1, "NIKOL.GEORGIEVA@EXAMPLE.COM", "NIKOL.GEORGIEVA", "AQAAAAIAAYagAAAAECPj5DFtBYXQg72sTTiOAmTktFZ+GKV+9u5FaOnwlw0piLY+HkXfHtXAZHi1UHh7qw==", "0855123456", true, "e9c2550f-30cc-4116-a85c-20265258d5a5", false, "nikol.georgieva" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Biography", "ExperienceYears", "IsDeleted", "Occupation", "UserId" },
                values: new object[,]
                {
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), "I am a licensed laser removal specialist with 7 years of experience in hair removal, tattoo removal, and skin resurfacing treatments. My focus is on providing safe, effective, and customized solutions for each client to achieve smooth and healthy skin.", 6, false, "Laser Technician", new Guid("29965aaa-46cf-4829-93b8-e38401be7547") },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), "I am a licensed dermatologist with 5 years of experience helping patients achieve healthier skin. My expertise includes treating conditions such as acne, eczema, and pigmentation disorders, as well as performing advanced laser and cosmetic procedures. I am passionate about educating my patients and creating personalized treatment plans that combine proven medical methods with modern skin care innovations.", 5, false, "Dermatologist", new Guid("a7d3c5e2-9b41-4f12-8f34-123456789abc") },
                    { new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), "I am a certified massage therapist with 6 years of experience in Swedish, deep tissue, and therapeutic massage. My goal is to help clients relax, relieve stress, and improve overall well-being through personalized treatments.", 6, false, "Massage Therapist", new Guid("c9f4e7b1-2d33-4a11-8f56-abcdef123456") },
                    { new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), "I am a certified esthetician with 10 years of experience providing skin care treatments, facials, and anti-aging therapies. My goal is to help clients achieve glowing, healthy skin using both modern techniques and natural methods.", 10, false, "Esthetician", new Guid("ac31b0bb-d05a-438d-be06-9bfe3323cf08") }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CategoryId", "Description", "DurationInMinutes", "IsDeleted", "Name", "Points", "Price" },
                values: new object[,]
                {
                    { 1, 1, "A relaxing facial treatment to cleanse, exfoliate, and nourish the skin, improving overall complexion and hydration.", 60, false, "Facial Treatment", 50, 50.00m },
                    { 2, 2, "Effective laser hair removal treatment for full legs, providing smooth, hair-free skin with minimal discomfort.", 90, false, "Laser Hair Removal - Full Legs", 120, 120.00m },
                    { 3, 3, "A full-body Swedish massage to relax muscles, improve circulation, and reduce stress.", 60, false, "Swedish Massage", 70, 70.00m },
                    { 4, 1, "A skin-resurfacing procedure that improves texture and tone by removing damaged outer layers of skin.", 45, false, "Chemical Peel", 80, 80.00m },
                    { 5, 1, "A minimally invasive exfoliation treatment that refreshes skin tone and reduces fine lines and scars.", 40, false, "Microdermabrasion", 70, 70m },
                    { 6, 4, "Botulinum toxin injections to reduce the appearance of fine lines and wrinkles by temporarily relaxing facial muscles.", 30, false, "Botox Injections", 250, 250.00m },
                    { 7, 4, "Injectable fillers to restore volume, smooth lines, and enhance facial contours.", 45, false, "Dermal Fillers", 300, 300.00m },
                    { 8, 2, "Quick and effective laser treatment for long-lasting hair reduction in the underarm area.", 20, false, "Laser Hair Removal - Underarms", 60, 60.00m },
                    { 9, 2, "Safe and precise laser hair removal for the bikini area, ensuring smooth and comfortable results.", 30, false, "Laser Hair Removal - Bikini Line", 80, 80.00m },
                    { 10, 2, "Laser hair removal treatment for both arms, providing smooth, hair-free skin with minimal discomfort.", 60, false, "Laser Hair Removal - Full Arms", 110, 110.00m },
                    { 11, 2, "Targeted laser treatment for facial hair, including upper lip, chin, and cheeks.", 30, false, "Laser Hair Removal - Face", 70, 70.00m },
                    { 12, 2, "Comprehensive laser hair removal treatment for the full back area.", 90, false, "Laser Hair Removal - Back", 150, 150.00m },
                    { 13, 2, "Complete full-body laser hair removal package for smooth, hair-free skin.", 180, false, "Laser Hair Removal - Full Body", 450, 450.00m },
                    { 14, 5, "A purifying facial designed to deeply cleanse, exfoliate, and remove impurities from the skin.", 60, false, "Deep Cleansing Facial", 65, 65.00m },
                    { 15, 5, "A rejuvenating treatment that targets fine lines and wrinkles, promoting firmer and youthful-looking skin.", 75, false, "Anti-Aging Facial", 120, 120.00m },
                    { 16, 5, "A specialized facial to reduce acne, clear clogged pores, and minimize inflammation with professional-grade products.", 70, false, "Acne Treatment Facial", 90, 90.00m },
                    { 17, 5, "A moisture-boosting facial designed to restore hydration, plumpness, and radiance to dry or dull skin.", 50, false, "Hydrating Facial", 75, 75.00m },
                    { 18, 5, "A skin-brightening treatment that reduces pigmentation, evens skin tone, and restores natural glow.", 60, false, "Brightening Facial", 95, 95.00m },
                    { 19, 5, "A treatment that boosts collagen production to improve elasticity and smoothness of the skin.", 70, false, "Collagen Facial", 140, 140.00m },
                    { 20, 3, "A relaxing massage using essential oils to reduce stress, improve mood, and promote overall well-being.", 60, false, "Aromatherapy Massage", 85, 85.00m }
                });

            migrationBuilder.InsertData(
                table: "EmployeesSchedules",
                columns: new[] { "EmployeeId", "ScheduleId" },
                values: new object[,]
                {
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), 4 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 2 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 3 },
                    { new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), 3 },
                    { new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), 1 },
                    { new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), 3 }
                });

            migrationBuilder.InsertData(
                table: "EmployeesServices",
                columns: new[] { "EmployeeId", "ServiceId" },
                values: new object[,]
                {
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), 2 },
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), 8 },
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), 9 },
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), 10 },
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), 11 },
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), 12 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 1 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 4 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 5 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 6 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 14 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 15 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 16 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 17 },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 18 },
                    { new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), 3 },
                    { new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), 20 },
                    { new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), 6 },
                    { new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), 7 },
                    { new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), 13 }
                });

            migrationBuilder.InsertData(
                table: "Procedures",
                columns: new[] { "Id", "AppointmentDate", "EmployeeId", "IsDeleted", "Notes", "ServiceId", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 28, 10, 30, 0, 0, DateTimeKind.Unspecified), new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), false, "First-time consultation, requested deep skin analysis.", 5, 1, new Guid("e5c2a9b3-4a67-4f89-8d23-556677889900") },
                    { 2, new DateTime(2025, 9, 29, 14, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), false, "Follow-up massage therapy session.", 6, 2, new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760") },
                    { 3, new DateTime(2025, 10, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), false, "Facial rejuvenation treatment scheduled.", 8, 0, new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760") }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "EmployeeId", "ProcedureId", "Rating", "UserId" },
                values: new object[] { 1, "Excellent service! The procedure was professional and I felt very comfortable.", new DateTime(2026, 3, 23, 12, 5, 24, 309, DateTimeKind.Utc).AddTicks(3543), new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 1, 5, new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760") });

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
                name: "IX_AspNetUsers_MembershipId",
                table: "AspNetUsers",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_EmployeeId",
                table: "Certificates",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UserId",
                table: "Employees",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesSchedules_ScheduleId",
                table: "EmployeesSchedules",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeesServices_ServiceId",
                table: "EmployeesServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_EmployeeId",
                table: "Procedures",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_ServiceId",
                table: "Procedures",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_UserId",
                table: "Procedures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EmployeeId",
                table: "Reviews",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProcedureId",
                table: "Reviews",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CategoryId",
                table: "Services",
                column: "CategoryId");
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
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "EmployeesSchedules");

            migrationBuilder.DropTable(
                name: "EmployeesServices");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Memberships");
        }
    }
}
