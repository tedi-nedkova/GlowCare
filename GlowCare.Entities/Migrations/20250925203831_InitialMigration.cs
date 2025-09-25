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
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
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
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DaysOfWeek",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaysOfWeek", x => x.Id);
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
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false)
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
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MembershipId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clients_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchedulesDaysOfWeek",
                columns: table => new
                {
                    DayOfWeekId = table.Column<int>(type: "int", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchedulesDaysOfWeek", x => new { x.DayOfWeekId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_SchedulesDaysOfWeek_DaysOfWeek_DayOfWeekId",
                        column: x => x.DayOfWeekId,
                        principalTable: "DaysOfWeek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchedulesDaysOfWeek_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
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
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
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
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedures_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
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
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcedureId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
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
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "Specialist", "SPECIALIST" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "29965aaa-46cf-4829-93b8-e38401be7547", 0, 38, "ac713d17-709a-4d57-8df5-8d72138ad308", "maria.petrova@gmail.com", true, "Maria", 1, "Petrova", false, null, "MARIA.PETROVA@EXAMPLE.COM", "MARIA.PETROVA", null, "0899123456", true, "a64bf4db-bc02-4b56-a1d0-2f0c269aab7f", true, "maria.petrova" },
                    { "a7d3c5e2-9b41-4f12-8f34-123456789abc", 0, 32, "93b6d7a5-fb45-423e-a1f7-a127b43b7c6d", "elena.dimitrova@gmail.com", true, "Elena", 1, "Dimitrova", false, null, "ELENA.DIMITROVA@EXAMPLE.COM", "ELENA.DIMITROVA", null, "0888123456", true, "27158a89-93b0-4091-9814-ab893f0076f6", true, "elena.dimitrova" },
                    { "ac31b0bb-d05a-438d-be06-9bfe3323cf08", 0, 30, "c4afcafb-5e2b-4050-bfc6-cdcb5107d7ef", "johndoe@gmail.com", true, "John", 0, "Doe", false, null, "JOHNDOE@EXAMPLE.COM", "JOHN.DOE", null, "0875757574", true, "cbcc94c9-0b1d-41b5-b834-52aacd77d44f", true, "john.doe" },
                    { "c9f4e7b1-2d33-4a11-8f56-abcdef123456", 0, 30, "493657fd-770a-446a-93cf-48f1bd90fb2b", "ivana.koleva@gmail.com", true, "Ivana", 1, "Koleva", false, null, "IVANA.KOLEVA@EXAMPLE.COM", "IVANA.KOLEVA", null, "0888234567", true, "8240f8eb-6267-4060-b626-00fb9041f997", true, "ivana.koleva" },
                    { "e5c2g9b3-4a67-4f89-8d23-556677889900", 0, 30, "1a281c2a-9d1a-4f22-9964-ac330405b2a2", "nikol.georgieva@gmail.com", true, "Nikol", 1, "Georgieva", false, null, "NIKOL.GEORGIEVA@EXAMPLE.COM", "NIKOL.GEORGIEVA", null, "0855123456", true, "2db7e367-bbca-411b-a785-86147247a468", true, "nikol.georgieva" }
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
                table: "DaysOfWeek",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Monday" },
                    { 2, "Tuesday" },
                    { 3, "Wednesday" },
                    { 4, "Thursday" },
                    { 5, "Friday" },
                    { 6, "Saturday" },
                    { 7, "Sunday" }
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
                columns: new[] { "Id", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { 1, "18:00", "09:00" },
                    { 2, "16:00", "10:00" },
                    { 3, "16:00", "9:00" },
                    { 4, "15:00", "11:00" }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "MembershipId", "UserId" },
                values: new object[] { "1f4c1474-cf86-4199-9f6c-1f417b7ac9e4", 1, "e5c2g9b3-4a67-4f89-8d23-556677889900" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Biography", "ExperienceYears", "Occupation", "UserId" },
                values: new object[,]
                {
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", "I am a licensed laser removal specialist with 7 years of experience in hair removal, tattoo removal, and skin resurfacing treatments. My focus is on providing safe, effective, and customized solutions for each client to achieve smooth and healthy skin.", 6, "Laser Technician", "29965aaa-46cf-4829-93b8-e38401be7547" },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", "I am a licensed dermatologist with 5 years of experience helping patients achieve healthier skin. My expertise includes treating conditions such as acne, eczema, and pigmentation disorders, as well as performing advanced laser and cosmetic procedures. I am passionate about educating my patients and creating personalized treatment plans that combine proven medical methods with modern skin care innovations.", 5, "Dermatologist", "a7d3c5e2-9b41-4f12-8f34-123456789abc" },
                    { "a0617bdf-80af-4316-a9c9-c2fd77170f7f", "I am a certified massage therapist with 6 years of experience in Swedish, deep tissue, and therapeutic massage. My goal is to help clients relax, relieve stress, and improve overall well-being through personalized treatments.", 6, "Massage Therapist", "c9f4e7b1-2d33-4a11-8f56-abcdef123456" },
                    { "b75e8e37-95e4-44ef-a32c-10aebaff55b3", "I am a certified esthetician with 10 years of experience providing skin care treatments, facials, and anti-aging therapies. My goal is to help clients achieve glowing, healthy skin using both modern techniques and natural methods.", 10, "Esthetician", "ac31b0bb-d05a-438d-be06-9bfe3323cf08" }
                });

            migrationBuilder.InsertData(
                table: "SchedulesDaysOfWeek",
                columns: new[] { "DayOfWeekId", "ScheduleId" },
                values: new object[,]
                {
                    { 2, 3 },
                    { 4, 3 },
                    { 5, 2 },
                    { 5, 4 },
                    { 6, 4 }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CategoryId", "Description", "DurationInMinutes", "Name", "Points", "Price" },
                values: new object[,]
                {
                    { 1, 1, "A relaxing facial treatment to cleanse, exfoliate, and nourish the skin, improving overall complexion and hydration.", 60, "Facial Treatment", 50, 50.00m },
                    { 2, 2, "Effective laser hair removal treatment for full legs, providing smooth, hair-free skin with minimal discomfort.", 90, "Laser Hair Removal - Full Legs", 120, 120.00m },
                    { 3, 3, "A full-body Swedish massage to relax muscles, improve circulation, and reduce stress.", 60, "Swedish Massage", 70, 70.00m },
                    { 4, 1, "A skin-resurfacing procedure that improves texture and tone by removing damaged outer layers of skin.", 45, "Chemical Peel", 80, 80.00m },
                    { 5, 1, "A minimally invasive exfoliation treatment that refreshes skin tone and reduces fine lines and scars.", 40, "Microdermabrasion", 70, 70m },
                    { 6, 4, "Botulinum toxin injections to reduce the appearance of fine lines and wrinkles by temporarily relaxing facial muscles.", 30, "Botox Injections", 250, 250.00m },
                    { 7, 4, "Injectable fillers to restore volume, smooth lines, and enhance facial contours.", 45, "Dermal Fillers", 300, 300.00m },
                    { 8, 2, "Quick and effective laser treatment for long-lasting hair reduction in the underarm area.", 20, "Laser Hair Removal - Underarms", 60, 60.00m },
                    { 9, 2, "Safe and precise laser hair removal for the bikini area, ensuring smooth and comfortable results.", 30, "Laser Hair Removal - Bikini Line", 80, 80.00m },
                    { 10, 2, "Laser hair removal treatment for both arms, providing smooth, hair-free skin with minimal discomfort.", 60, "Laser Hair Removal - Full Arms", 110, 110.00m },
                    { 11, 2, "Targeted laser treatment for facial hair, including upper lip, chin, and cheeks.", 30, "Laser Hair Removal - Face", 70, 70.00m },
                    { 12, 2, "Comprehensive laser hair removal treatment for the full back area.", 90, "Laser Hair Removal - Back", 150, 150.00m },
                    { 13, 2, "Complete full-body laser hair removal package for smooth, hair-free skin.", 180, "Laser Hair Removal - Full Body", 450, 450.00m },
                    { 14, 5, "A purifying facial designed to deeply cleanse, exfoliate, and remove impurities from the skin.", 60, "Deep Cleansing Facial", 65, 65.00m },
                    { 15, 5, "A rejuvenating treatment that targets fine lines and wrinkles, promoting firmer and youthful-looking skin.", 75, "Anti-Aging Facial", 120, 120.00m },
                    { 16, 5, "A specialized facial to reduce acne, clear clogged pores, and minimize inflammation with professional-grade products.", 70, "Acne Treatment Facial", 90, 90.00m },
                    { 17, 5, "A moisture-boosting facial designed to restore hydration, plumpness, and radiance to dry or dull skin.", 50, "Hydrating Facial", 75, 75.00m },
                    { 18, 5, "A skin-brightening treatment that reduces pigmentation, evens skin tone, and restores natural glow.", 60, "Brightening Facial", 95, 95.00m },
                    { 19, 5, "A treatment that boosts collagen production to improve elasticity and smoothness of the skin.", 70, "Collagen Facial", 140, 140.00m },
                    { 20, 3, "A relaxing massage using essential oils to reduce stress, improve mood, and promote overall well-being.", 60, "Aromatherapy Massage", 85, 85.00m }
                });

            migrationBuilder.InsertData(
                table: "EmployeesSchedules",
                columns: new[] { "EmployeeId", "ScheduleId" },
                values: new object[,]
                {
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", 4 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 2 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 3 },
                    { "a0617bdf-80af-4316-a9c9-c2fd77170f7f", 3 },
                    { "b75e8e37-95e4-44ef-a32c-10aebaff55b3", 1 },
                    { "b75e8e37-95e4-44ef-a32c-10aebaff55b3", 3 }
                });

            migrationBuilder.InsertData(
                table: "EmployeesServices",
                columns: new[] { "EmployeeId", "ServiceId" },
                values: new object[,]
                {
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", 2 },
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", 8 },
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", 9 },
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", 10 },
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", 11 },
                    { "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", 12 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 1 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 4 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 5 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 6 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 14 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 15 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 16 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 17 },
                    { "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 18 },
                    { "a0617bdf-80af-4316-a9c9-c2fd77170f7f", 3 },
                    { "a0617bdf-80af-4316-a9c9-c2fd77170f7f", 20 },
                    { "b75e8e37-95e4-44ef-a32c-10aebaff55b3", 6 },
                    { "b75e8e37-95e4-44ef-a32c-10aebaff55b3", 7 },
                    { "b75e8e37-95e4-44ef-a32c-10aebaff55b3", 13 }
                });

            migrationBuilder.InsertData(
                table: "Procedures",
                columns: new[] { "Id", "AppointmentDate", "ClientId", "EmployeeId", "Notes", "ServiceId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 28, 10, 30, 0, 0, DateTimeKind.Unspecified), "1f4c1474-cf86-4199-9f6c-1f417b7ac9e4", "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", "First-time consultation, requested deep skin analysis.", 5, 1 },
                    { 2, new DateTime(2025, 9, 29, 14, 0, 0, 0, DateTimeKind.Unspecified), "1f4c1474-cf86-4199-9f6c-1f417b7ac9e4", "b75e8e37-95e4-44ef-a32c-10aebaff55b3", "Follow-up massage therapy session.", 6, 2 },
                    { 3, new DateTime(2025, 10, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), "1f4c1474-cf86-4199-9f6c-1f417b7ac9e4", "1dccdb69-fbd1-43c4-8c17-95796b5aa95e", "Facial rejuvenation treatment scheduled.", 8, 0 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "ClientId", "Comment", "CreatedAt", "EmployeeId", "ProcedureId", "Rating" },
                values: new object[] { 1, "1f4c1474-cf86-4199-9f6c-1f417b7ac9e4", "Excellent service! The procedure was professional and I felt very comfortable.", new DateTime(2025, 9, 25, 20, 38, 30, 726, DateTimeKind.Utc).AddTicks(893), "66e6b9a6-4c5a-4344-bdae-6edbacc4b608", 1, 5 });

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
                name: "IX_Certificates_EmployeeId",
                table: "Certificates",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_MembershipId",
                table: "Clients",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId");

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
                name: "IX_Procedures_ClientId",
                table: "Procedures",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_EmployeeId",
                table: "Procedures",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_ServiceId",
                table: "Procedures",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ClientId",
                table: "Reviews",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_EmployeeId",
                table: "Reviews",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProcedureId",
                table: "Reviews",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_SchedulesDaysOfWeek_ScheduleId",
                table: "SchedulesDaysOfWeek",
                column: "ScheduleId");

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
                name: "SchedulesDaysOfWeek");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "DaysOfWeek");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Memberships");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
