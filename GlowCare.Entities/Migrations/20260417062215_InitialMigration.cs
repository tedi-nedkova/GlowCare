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
                    IsSpecialist = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
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
                name: "SpecialistApplication",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExperienceYears = table.Column<int>(type: "int", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialistApplication", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialistApplication_AspNetUsers_UserId",
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
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    EndTime = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schedules_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
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
                    { new Guid("8c384109-e13f-4556-a810-ab9ba28161a2"), "a1111111-1111-1111-1111-111111111111", "Admin", "ADMIN" },
                    { new Guid("a3f06ed0-4bca-4e98-8bef-4cd91e64e2a1"), "c3333333-3333-3333-3333-333333333333", "Specialist", "SPECIALIST" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDeleted", "IsSpecialist", "LastName", "LockoutEnabled", "LockoutEnd", "MembershipId", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { new Guid("29965aaa-46cf-4829-93b8-e38401be7547"), 0, 38, "f7960d3d-e123-4a8d-9f3a-b0164d010020", "maria.petrova@gmail.com", true, "Мария", 1, false, true, "Петрова", false, null, null, "MARIA.PETROVA@EXAMPLE.COM", "MARIA.PETROVA", "AQAAAAIAAYagAAAAEHVvexRV9zEeXZn2/NUG5aHdeCbQodn1Glckn/4DVKVmtMqPMITRhI9dqDX0SbzQow==", "0899123456", true, "68e7add5-6f2f-4792-a2e6-01f596c74a52", false, "maria.petrova" },
                    { new Guid("a7d3c5e2-9b41-4f12-8f34-123456789abc"), 0, 32, "6e5fc66b-0347-4688-a1c3-f78e9f6a861f", "elena.dimitrova@gmail.com", true, "Елена", 1, false, true, "Димитрова", false, null, null, "ELENA.DIMITROVA@EXAMPLE.COM", "ELENA.DIMITROVA", "AQAAAAIAAYagAAAAEP/a9M/E4mKQ3tDcG6+Z2G3GxmDbYT5Lj/s4uOl0P1MhCSFuI8UprqCwyyygAWJuGw==", "0888123456", true, "bfb94cc6-d1c4-4315-8f5e-f1d3535f4c01", false, "elena.dimitrova" },
                    { new Guid("ac31b0bb-d05a-438d-be06-9bfe3323cf08"), 0, 30, "70898beb-868d-4d0f-9840-2702692ec96c", "johndoe@gmail.com", true, "Джон", 0, false, true, "Доу", false, null, null, "JOHNDOE@EXAMPLE.COM", "JOHN.DOE", "AQAAAAIAAYagAAAAEPgIn8rXgN3w5ACcEfBm4Wr8KsrOBatgOjtcN9K9VbYybDrRT2ss/OzzweeNxTsd7w==", "0875757574", true, "a6241046-575b-4a3f-a668-c6aa755a232f", false, "john.doe" },
                    { new Guid("c9f4e7b1-2d33-4a11-8f56-abcdef123456"), 0, 30, "dbd427f3-623d-4eba-93e1-1c08a9d113f0", "ivana.koleva@gmail.com", true, "Ивана", 1, false, true, "Колева", false, null, null, "IVANA.KOLEVA@EXAMPLE.COM", "IVANA.KOLEVA", "AQAAAAIAAYagAAAAEAZ+tF0KvqfLsMAaTDlU3urqTvqow6XGf//OaVbGTZnZphpdgG0VSY4NK7Los6ph9g==", "0888234567", true, "e0ff1671-f59e-407c-a1bc-b6034b44ac41", false, "ivana.koleva" },
                    { new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760"), 0, 18, "d475a375-50f0-4edd-a06d-d30e3e7f0015", "teodora_nedkova@abv.bg", true, "Теодора", 1, false, false, "Недкова", false, null, null, "TEODORA_NEDKOVA@ABV.BG", "TEODORA.NEDKOVA", "AQAAAAIAAYagAAAAECDIc3bfAJ4/uXNepLvaG82SzfuknmR4kdOhTV4XouT4vaZzLCQM8ZgdMGHHzyfDuA==", "0878654562", true, "258513a82f1147ba92a0e43f1602b7c3", false, "teodora.nedkova" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Грижа за кожата" },
                    { 2, "Лазерни процедури" },
                    { 3, "Масаж" },
                    { 4, "Естетична медицина" },
                    { 5, "Процедури за лице" }
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
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Age", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "IsDeleted", "IsSpecialist", "LastName", "LockoutEnabled", "LockoutEnd", "MembershipId", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("e5c2a9b3-4a67-4f89-8d23-556677889900"), 0, 30, "1aec1e1c-d752-4ed9-97a3-2337c19d3d18", "nikol.georgieva@gmail.com", true, "Никол", 1, false, false, "Георгиева", false, null, 1, "NIKOL.GEORGIEVA@EXAMPLE.COM", "NIKOL.GEORGIEVA", "AQAAAAIAAYagAAAAECdjymZGHltvc+ioP7ndf8UHQ10jDxFojmmMTVi4HrhXVxBeirHWi1CUMAwyasNMQQ==", "0855123456", true, "e9c2550f-30cc-4116-a85c-20265258d5a5", false, "nikol.georgieva" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Biography", "ExperienceYears", "IsDeleted", "Occupation", "UserId" },
                values: new object[,]
                {
                    { new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), "Аз съм лицензиран специалист по лазерни процедури с 7 години опит в лазерната епилация, премахването на татуировки и процедурите за обновяване на кожата. Фокусът ми е върху това да предоставям безопасни, ефективни и индивидуално съобразени решения за всеки клиент с цел постигане на гладка и здрава кожа.", 6, false, "Лазерен специалист", new Guid("29965aaa-46cf-4829-93b8-e38401be7547") },
                    { new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), "Аз съм лицензиран дерматолог с 5 години опит в подпомагането на пациентите да постигнат по-здрава кожа. Моята експертиза включва лечение на състояния като акне, екзема и пигментни нарушения, както и извършване на съвременни лазерни и козметични процедури. Стремя се да информирам пациентите си и да създавам персонализирани терапевтични планове, които съчетават доказани медицински методи с модерни иновации в грижата за кожата.", 5, false, "Дерматолог", new Guid("a7d3c5e2-9b41-4f12-8f34-123456789abc") },
                    { new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), "Аз съм сертифициран масажист с 6 години опит в шведския, дълбокотъканния и лечебния масаж. Моята цел е да помогна на клиентите да се отпуснат, да облекчат стреса и да подобрят общото си благосъстояние чрез персонализирани процедури.", 6, false, "Масажист", new Guid("c9f4e7b1-2d33-4a11-8f56-abcdef123456") },
                    { new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), "Аз съм сертифициран естетик с 10 години опит в предоставянето на процедури за грижа за кожата, терапии за лице и анти-ейдж терапии. Моята цел е да помогна на клиентите да постигнат сияйна и здрава кожа чрез съчетание на съвременни техники и натурални методи.", 10, false, "Естетик", new Guid("ac31b0bb-d05a-438d-be06-9bfe3323cf08") }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CategoryId", "Description", "DurationInMinutes", "IsDeleted", "Name", "Points", "Price" },
                values: new object[,]
                {
                    { 1, 1, "Релаксираща терапия за лице, която почиства, ексфолира и подхранва кожата, като подобрява нейния вид и хидратация.", 60, false, "Терапия за лице", 50, 50.0m },
                    { 2, 2, "Ефективна лазерна епилация за цели крака, която осигурява гладка кожа без окосмяване с минимален дискомфорт.", 90, false, "Лазерна епилация - цели крака", 120, 120.0m },
                    { 3, 3, "Шведски масаж на цяло тяло за отпускане на мускулите, подобряване на кръвообращението и намаляване на стреса.", 60, false, "Шведски масаж", 70, 70.0m },
                    { 4, 1, "Процедура за обновяване на кожата, която подобрява текстурата и тена чрез премахване на увредените външни слоеве.", 45, false, "Химичен пилинг", 80, 80.0m },
                    { 5, 1, "Минимално инвазивна ексфолираща процедура, която освежава тена и намалява фините линии и белезите.", 40, false, "Микродермабразио", 70, 70m },
                    { 6, 4, "Инжекции с ботулинов токсин за намаляване на фините линии и бръчките чрез временно отпускане на лицевите мускули.", 30, false, "Ботокс инжекции", 250, 250.0m },
                    { 7, 4, "Инжекционни филъри за възстановяване на обема, изглаждане на линиите и подчертаване на контурите на лицето.", 45, false, "Дермални филъри", 300, 300.0m },
                    { 8, 2, "Бърза и ефективна лазерна процедура за дълготрайно намаляване на окосмяването в областта на подмишниците.", 20, false, "Лазерна епилация - подмишници", 60, 60.0m },
                    { 9, 2, "Безопасна и прецизна лазерна епилация за бикини зоната, осигуряваща гладки и комфортни резултати.", 30, false, "Лазерна епилация - бикини зона", 80, 80.0m },
                    { 10, 2, "Лазерна епилация за двете ръце, осигуряваща гладка кожа без окосмяване с минимален дискомфорт.", 60, false, "Лазерна епилация - цели ръце", 110, 110.0m },
                    { 11, 2, "Целенасочена лазерна процедура за лицево окосмяване, включително горна устна, брадичка и бузи.", 30, false, "Лазерна епилация - лице", 70, 70.0m },
                    { 12, 2, "Комплексна лазерна епилация за цялата зона на гърба.", 90, false, "Лазерна епилация - гръб", 150, 150.0m },
                    { 13, 2, "Пълен пакет за лазерна епилация на цяло тяло за гладка кожа без окосмяване.", 180, false, "Лазерна епилация - цяло тяло", 450, 450.0m },
                    { 14, 5, "Почистваща терапия за лице, създадена за дълбоко почистване, ексфолиране и премахване на замърсяванията от кожата.", 60, false, "Дълбоко почистваща терапия за лице", 65, 65.0m },
                    { 15, 5, "Подмладяваща процедура, насочена към фините линии и бръчките, която подпомага по-стегнат и младежки вид на кожата.", 75, false, "Анти-ейдж терапия за лице", 120, 120.0m },
                    { 16, 5, "Специализирана терапия за лице за намаляване на акнето, почистване на запушените пори и ограничаване на възпалението с професионални продукти.", 70, false, "Терапия за лице против акне", 90, 90.0m },
                    { 17, 5, "Хидратираща терапия за лице, създадена да възстанови влагата, плътността и сиянието на суха или повяхнала кожа.", 50, false, "Хидратираща терапия за лице", 75, 75.0m },
                    { 18, 5, "Изсветляваща процедура, която намалява пигментацията, изравнява тена и възстановява естествения блясък на кожата.", 60, false, "Изсветляваща терапия за лице", 95, 95.0m },
                    { 19, 5, "Процедура, която стимулира производството на колаген за подобряване на еластичността и гладкостта на кожата.", 70, false, "Колагенова терапия за лице", 140, 140.0m },
                    { 20, 3, "Релаксиращ масаж с етерични масла за намаляване на стреса, подобряване на настроението и насърчаване на общото благосъстояние.", 60, false, "Ароматерапевтичен масаж", 85, 85.0m }
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
                    { 1, new DateTime(2025, 9, 28, 10, 30, 0, 0, DateTimeKind.Unspecified), new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), false, "Първоначална консултация, заявен е задълбочен анализ на кожата.", 5, 1, new Guid("e5c2a9b3-4a67-4f89-8d23-556677889900") },
                    { 2, new DateTime(2025, 9, 29, 14, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), false, "Последваща сесия за масажна терапия.", 6, 2, new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760") },
                    { 3, new DateTime(2025, 10, 1, 9, 0, 0, 0, DateTimeKind.Unspecified), new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), false, "Планирана е процедура за подмладяване на лицето.", 8, 0, new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760") }
                });

            migrationBuilder.InsertData(
                table: "Schedules",
                columns: new[] { "Id", "DayOfWeek", "EmployeeId", "EndTime", "StartTime" },
                values: new object[,]
                {
                    { 1, 1, new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), "16:00", "10:00" },
                    { 2, 2, new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), "16:00", "09:00" },
                    { 3, 3, new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), "16:00", "10:00" },
                    { 4, 4, new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), "16:00", "09:00" },
                    { 5, 5, new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), "16:00", "10:00" },
                    { 6, 1, new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), "18:00", "09:00" },
                    { 7, 2, new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), "16:00", "09:00" },
                    { 8, 3, new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), "18:00", "09:00" },
                    { 9, 4, new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), "18:00", "09:00" },
                    { 10, 5, new Guid("b75e8e37-95e4-44ef-a32c-10aebaff55b3"), "16:00", "09:00" },
                    { 11, 1, new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), "17:00", "11:00" },
                    { 12, 2, new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), "17:00", "11:00" },
                    { 13, 3, new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), "17:00", "11:00" },
                    { 14, 4, new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), "17:00", "11:00" },
                    { 15, 5, new Guid("1dccdb69-fbd1-43c4-8c17-95796b5aa95e"), "17:00", "11:00" },
                    { 16, 1, new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), "18:00", "12:00" },
                    { 17, 3, new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), "18:00", "12:00" },
                    { 18, 5, new Guid("a0617bdf-80af-4316-a9c9-c2fd77170f7f"), "18:00", "12:00" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "EmployeeId", "ProcedureId", "Rating", "UserId" },
                values: new object[] { 1, "Отлично обслужване! Процедурата беше извършена професионално и се чувствах много комфортно.", new DateTime(2026, 4, 17, 6, 22, 15, 255, DateTimeKind.Utc).AddTicks(1042), new Guid("66e6b9a6-4c5a-4344-bdae-6edbacc4b608"), 1, 5, new Guid("fc95b3fa-f342-4172-ac8b-5b35951ad760") });

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
                name: "IX_Schedules_EmployeeId",
                table: "Schedules",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CategoryId",
                table: "Services",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialistApplication_UserId",
                table: "SpecialistApplication",
                column: "UserId");
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
                name: "EmployeesServices");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "SpecialistApplication");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

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
