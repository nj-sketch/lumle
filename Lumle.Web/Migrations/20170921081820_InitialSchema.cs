using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Lumle.Web.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminConfig_AppSystem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_AppSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_CredentialCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_CredentialCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultSlugDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DefaultSubject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LastBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastSlugDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LastSubject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SlugDisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_SystemHealth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_SystemHealth", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audit_AuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuditSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AuditType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KeyField = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audit_CustomLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CallSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoggedDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Port = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RemoteAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_CustomLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authorization_Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Menu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubMenu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorization_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authorization_UserProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorization_UserProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blog_Article",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FeaturedImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Calendar_Event",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Calendar_Holiday",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Holiday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_BaseRoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_BaseRoleClaim", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    AccountStatus = table.Column<int>(type: "int", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "en-US"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeZone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localization_Culture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_Culture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localization_ResourceCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_ResourceCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicUser_MobileUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsStaff = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubjectId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicUser_MobileUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_Credential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CredentialCategoryId = table.Column<int>(type: "int", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_Credential", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminConfig_Credential_AdminConfig_CredentialCategory_CredentialCategoryId",
                        column: x => x.CredentialCategoryId,
                        principalTable: "AdminConfig_CredentialCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_ServiceHealth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    SystemHealthId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_ServiceHealth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminConfig_ServiceHealth_AdminConfig_SystemHealth_SystemHealthId",
                        column: x => x.SystemHealthId,
                        principalTable: "AdminConfig_SystemHealth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_RoleClaim_Core_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Core_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_ApplicationToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_ApplicationToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_ApplicationToken_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Core_UserClaim_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_Core_UserLogin_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserRole",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Core_UserRole_Core_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Core_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Core_UserRole_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserToken",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserToken", x => x.UserId);
                    table.UniqueConstraint("AK_Core_UserToken_UserId_LoginProvider_Name", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_Core_UserToken_Core_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Core_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Localization_Resource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CultureId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceCategoryId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_Resource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Localization_Resource_Localization_Culture_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Localization_Culture",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Localization_Resource_Localization_ResourceCategory_ResourceCategoryId",
                        column: x => x.ResourceCategoryId,
                        principalTable: "Localization_ResourceCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "SlugIndex",
                table: "AdminConfig_AppSystem",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminConfig_Credential_CredentialCategoryId_Slug",
                table: "AdminConfig_Credential",
                columns: new[] { "CredentialCategoryId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminConfig_EmailTemplate_Slug",
                table: "AdminConfig_EmailTemplate",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminConfig_ServiceHealth_SystemHealthId",
                table: "AdminConfig_ServiceHealth",
                column: "SystemHealthId");

            migrationBuilder.CreateIndex(
                name: "IX_Authorization_Permission_Slug",
                table: "Authorization_Permission",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Core_ApplicationToken_UserId",
                table: "Core_ApplicationToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Core_Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Core_RoleClaim_RoleId",
                table: "Core_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Core_User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Core_User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_Email_NormalizedEmail",
                table: "Core_User",
                columns: new[] { "Email", "NormalizedEmail" },
                unique: true,
                filter: "[Email] IS NOT NULL AND [NormalizedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserClaim_UserId",
                table: "Core_UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserLogin_UserId",
                table: "Core_UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserRole_RoleId",
                table: "Core_UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Localization_Culture_Name",
                table: "Localization_Culture",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localization_Resource_ResourceCategoryId",
                table: "Localization_Resource",
                column: "ResourceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Localization_Resource_CultureId_Key",
                table: "Localization_Resource",
                columns: new[] { "CultureId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Localization_ResourceCategory_Name",
                table: "Localization_ResourceCategory",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminConfig_AppSystem");

            migrationBuilder.DropTable(
                name: "AdminConfig_Credential");

            migrationBuilder.DropTable(
                name: "AdminConfig_EmailTemplate");

            migrationBuilder.DropTable(
                name: "AdminConfig_ServiceHealth");

            migrationBuilder.DropTable(
                name: "Audit_AuditLog");

            migrationBuilder.DropTable(
                name: "Audit_CustomLog");

            migrationBuilder.DropTable(
                name: "Authorization_Permission");

            migrationBuilder.DropTable(
                name: "Authorization_UserProfile");

            migrationBuilder.DropTable(
                name: "Blog_Article");

            migrationBuilder.DropTable(
                name: "Calendar_Event");

            migrationBuilder.DropTable(
                name: "Calendar_Holiday");

            migrationBuilder.DropTable(
                name: "Core_ApplicationToken");

            migrationBuilder.DropTable(
                name: "Core_BaseRoleClaim");

            migrationBuilder.DropTable(
                name: "Core_RoleClaim");

            migrationBuilder.DropTable(
                name: "Core_UserClaim");

            migrationBuilder.DropTable(
                name: "Core_UserLogin");

            migrationBuilder.DropTable(
                name: "Core_UserRole");

            migrationBuilder.DropTable(
                name: "Core_UserToken");

            migrationBuilder.DropTable(
                name: "Localization_Resource");

            migrationBuilder.DropTable(
                name: "PublicUser_MobileUser");

            migrationBuilder.DropTable(
                name: "AdminConfig_CredentialCategory");

            migrationBuilder.DropTable(
                name: "AdminConfig_SystemHealth");

            migrationBuilder.DropTable(
                name: "Core_Role");

            migrationBuilder.DropTable(
                name: "Core_User");

            migrationBuilder.DropTable(
                name: "Localization_Culture");

            migrationBuilder.DropTable(
                name: "Localization_ResourceCategory");
        }
    }
}
