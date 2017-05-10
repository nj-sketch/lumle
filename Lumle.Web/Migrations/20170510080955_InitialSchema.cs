using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

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
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    LastUpdatedBy = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_AppSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_BaseRoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClaimType = table.Column<string>(nullable: false),
                    ClaimValue = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_BaseRoleClaim", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localization_Culture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsEnable = table.Column<bool>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_Culture", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localization_ResourceCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localization_ResourceCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authorization_UserProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AboutMe = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DeletedDate = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    Website = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorization_UserProfile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_Role",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsBlocked = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    AccountStatus = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Culture = table.Column<string>(maxLength: 20, nullable: false, defaultValue: "en-US"),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TimeZone = table.Column<string>(maxLength: 100, nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_CredentialCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_CredentialCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_EmailTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Body = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DefaultBody = table.Column<string>(nullable: false),
                    DefaultSlugDisplayName = table.Column<string>(maxLength: 200, nullable: false),
                    DefaultSubject = table.Column<string>(maxLength: 500, nullable: false),
                    LastBody = table.Column<string>(nullable: false),
                    LastSlugDisplayName = table.Column<string>(maxLength: 200, nullable: false),
                    LastSubject = table.Column<string>(maxLength: 500, nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(maxLength: 100, nullable: false),
                    SlugDisplayName = table.Column<string>(maxLength: 200, nullable: false),
                    Subject = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_EmailTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminConfig_SystemHealth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminConfig_SystemHealth", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audit_AuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AuditSummary = table.Column<string>(nullable: true),
                    AuditType = table.Column<string>(nullable: false),
                    Changes = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IpAddress = table.Column<string>(nullable: true),
                    KeyField = table.Column<string>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    MachineName = table.Column<string>(nullable: true),
                    ModuleInfo = table.Column<string>(nullable: true),
                    NewValue = table.Column<string>(nullable: false),
                    OldValue = table.Column<string>(nullable: false),
                    TableName = table.Column<string>(nullable: false),
                    UserAgent = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audit_CustomLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CallSite = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    Exception = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "now()"),
                    Level = table.Column<string>(nullable: true),
                    LoggedDate = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Port = table.Column<string>(nullable: true),
                    RemoteAddress = table.Column<string>(nullable: true),
                    RequestMethod = table.Column<string>(nullable: true),
                    ServerAddress = table.Column<string>(nullable: true),
                    ServerName = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit_CustomLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Authorization_Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DisplayName = table.Column<string>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Menu = table.Column<string>(nullable: false),
                    Slug = table.Column<string>(nullable: false),
                    SubMenu = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorization_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blog_Article",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Author = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FeaturedImageUrl = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog_Article", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Calendar_Event",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(maxLength: 250, nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Calendar_Holiday",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(maxLength: 250, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Holiday", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PublicUser_CustomUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    IsBlocked = table.Column<bool>(nullable: false),
                    IsEmailVerified = table.Column<bool>(nullable: false),
                    IsStaff = table.Column<bool>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    PasswordSalt = table.Column<string>(nullable: false),
                    PhoneNo = table.Column<string>(nullable: true),
                    ProfileUrl = table.Column<string>(nullable: true),
                    Provider = table.Column<string>(nullable: false),
                    SubjectId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicUser_CustomUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Core_UserToken",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Core_UserToken", x => x.UserId);
                    table.UniqueConstraint("AK_Core_UserToken_UserId_LoginProvider_Name", x => new { x.UserId, x.LoginProvider, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Localization_Resource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    ResourceCategoryId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Core_RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
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
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: false),
                    TokenType = table.Column<string>(nullable: false),
                    UsedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
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
                name: "Core_UserRole",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
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
                name: "Core_UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
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
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
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
                name: "AdminConfig_Credential",
                columns: table => new
                {
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CredentialCategoryId = table.Column<int>(nullable: false),
                    DisplayName = table.Column<string>(maxLength: 250, nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Slug = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 1000, nullable: true)
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
                    Id = table.Column<int>(type: "bigserial ", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    ServiceName = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    SystemHealthId = table.Column<int>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Core_ApplicationToken_UserId",
                table: "Core_ApplicationToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "SlugIndex",
                table: "AdminConfig_AppSystem",
                column: "Slug",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Core_Role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Core_User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Core_User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Core_User_Email_NormalizedEmail",
                table: "Core_User",
                columns: new[] { "Email", "NormalizedEmail" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserRole_RoleId",
                table: "Core_UserRole",
                column: "RoleId");

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
                name: "IX_Core_RoleClaim_RoleId",
                table: "Core_RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserClaim_UserId",
                table: "Core_UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Core_UserLogin_UserId",
                table: "Core_UserLogin",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Core_ApplicationToken");

            migrationBuilder.DropTable(
                name: "AdminConfig_AppSystem");

            migrationBuilder.DropTable(
                name: "Core_BaseRoleClaim");

            migrationBuilder.DropTable(
                name: "Localization_Resource");

            migrationBuilder.DropTable(
                name: "Authorization_UserProfile");

            migrationBuilder.DropTable(
                name: "Core_UserRole");

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
                name: "Blog_Article");

            migrationBuilder.DropTable(
                name: "Calendar_Event");

            migrationBuilder.DropTable(
                name: "Calendar_Holiday");

            migrationBuilder.DropTable(
                name: "PublicUser_CustomUser");

            migrationBuilder.DropTable(
                name: "Core_RoleClaim");

            migrationBuilder.DropTable(
                name: "Core_UserClaim");

            migrationBuilder.DropTable(
                name: "Core_UserLogin");

            migrationBuilder.DropTable(
                name: "Core_UserToken");

            migrationBuilder.DropTable(
                name: "Localization_Culture");

            migrationBuilder.DropTable(
                name: "Localization_ResourceCategory");

            migrationBuilder.DropTable(
                name: "AdminConfig_CredentialCategory");

            migrationBuilder.DropTable(
                name: "AdminConfig_SystemHealth");

            migrationBuilder.DropTable(
                name: "Core_Role");

            migrationBuilder.DropTable(
                name: "Core_User");
        }
    }
}
