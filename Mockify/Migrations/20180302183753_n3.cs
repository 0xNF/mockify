﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Mockify.Migrations
{
    public partial class n3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreateManyUsersViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreateThisManyUsers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateManyUsersViewModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreateUserViewModel",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    BirthDate = table.Column<string>(maxLength: 10, nullable: true),
                    Country = table.Column<string>(maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 128, nullable: true),
                    Password = table.Column<string>(maxLength: 100, nullable: false),
                    Product = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreateUserViewModel", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "RateLimits",
                columns: table => new
                {
                    RateLimitsId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CallsPerWindow = table.Column<int>(nullable: false),
                    CurrentCalls = table.Column<int>(nullable: false),
                    RateWindow = table.Column<TimeSpan>(nullable: false),
                    WindowStartTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateLimits", x => x.RateLimitsId);
                });

            migrationBuilder.CreateTable(
                name: "ResponseModes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseModes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
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
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Birthdate = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 32, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    Followers = table.Column<int>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    OverallRateLimitRateLimitsId = table.Column<int>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    Product = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_RateLimits_OverallRateLimitRateLimitsId",
                        column: x => x.OverallRateLimitRateLimitsId,
                        principalTable: "RateLimits",
                        principalColumn: "RateLimitsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServerSettings",
                columns: table => new
                {
                    ServerSettingsId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DefaultMarket = table.Column<string>(nullable: true),
                    RateLimitsId = table.Column<int>(nullable: true),
                    ResponseModeid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSettings", x => x.ServerSettingsId);
                    table.ForeignKey(
                        name: "FK_ServerSettings_RateLimits_RateLimitsId",
                        column: x => x.RateLimitsId,
                        principalTable: "RateLimits",
                        principalColumn: "RateLimitsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServerSettings_ResponseModes_ResponseModeid",
                        column: x => x.ResponseModeid,
                        principalTable: "ResponseModes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
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
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
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
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
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
                name: "ExternalUrl",
                columns: table => new
                {
                    ExternalUrlId = table.Column<string>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalUrl", x => x.ExternalUrlId);
                    table.ForeignKey(
                        name: "FK_ExternalUrl_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Endpoint",
                columns: table => new
                {
                    EndpointId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(nullable: true),
                    RateLimitsId = table.Column<int>(nullable: true),
                    ResponseModeid = table.Column<int>(nullable: true),
                    ServerSettingsId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endpoint", x => x.EndpointId);
                    table.ForeignKey(
                        name: "FK_Endpoint_RateLimits_RateLimitsId",
                        column: x => x.RateLimitsId,
                        principalTable: "RateLimits",
                        principalColumn: "RateLimitsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Endpoint_ResponseModes_ResponseModeid",
                        column: x => x.ResponseModeid,
                        principalTable: "ResponseModes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Endpoint_ServerSettings_ServerSettingsId",
                        column: x => x.ServerSettingsId,
                        principalTable: "ServerSettings",
                        principalColumn: "ServerSettingsId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RedirectURI",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegisteredApplicationId = table.Column<string>(nullable: true),
                    URI = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedirectURI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserApplicationToken",
                columns: table => new
                {
                    TokenId = table.Column<string>(nullable: false),
                    AppUserRateLimitsRateLimitsId = table.Column<int>(nullable: true),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    ExpiresAt = table.Column<DateTime>(nullable: false),
                    TokenType = table.Column<string>(nullable: true),
                    TokenValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserApplicationToken", x => x.TokenId);
                    table.ForeignKey(
                        name: "FK_UserApplicationToken_RateLimits_AppUserRateLimitsRateLimitsId",
                        column: x => x.AppUserRateLimitsRateLimitsId,
                        principalTable: "RateLimits",
                        principalColumn: "RateLimitsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserApplicationToken_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ClientId = table.Column<string>(nullable: false),
                    ApplicationDescription = table.Column<string>(nullable: true),
                    ApplicationName = table.Column<string>(nullable: true),
                    ClientSecret = table.Column<string>(nullable: true),
                    OverallRateLimitRateLimitsId = table.Column<int>(nullable: true),
                    TokenId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Applications", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Applications_RateLimits_OverallRateLimitRateLimitsId",
                        column: x => x.OverallRateLimitRateLimitsId,
                        principalTable: "RateLimits",
                        principalColumn: "RateLimitsId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Applications_UserApplicationToken_TokenId",
                        column: x => x.TokenId,
                        principalTable: "UserApplicationToken",
                        principalColumn: "TokenId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_OverallRateLimitRateLimitsId",
                table: "Applications",
                column: "OverallRateLimitRateLimitsId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_TokenId",
                table: "Applications",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OverallRateLimitRateLimitsId",
                table: "AspNetUsers",
                column: "OverallRateLimitRateLimitsId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_RateLimitsId",
                table: "Endpoint",
                column: "RateLimitsId");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_ResponseModeid",
                table: "Endpoint",
                column: "ResponseModeid");

            migrationBuilder.CreateIndex(
                name: "IX_Endpoint_ServerSettingsId",
                table: "Endpoint",
                column: "ServerSettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalUrl_ApplicationUserId",
                table: "ExternalUrl",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RedirectURI_RegisteredApplicationId",
                table: "RedirectURI",
                column: "RegisteredApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerSettings_RateLimitsId",
                table: "ServerSettings",
                column: "RateLimitsId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerSettings_ResponseModeid",
                table: "ServerSettings",
                column: "ResponseModeid");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplicationToken_AppUserRateLimitsRateLimitsId",
                table: "UserApplicationToken",
                column: "AppUserRateLimitsRateLimitsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplicationToken_ApplicationUserId",
                table: "UserApplicationToken",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserApplicationToken_ClientId",
                table: "UserApplicationToken",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_RedirectURI_Applications_RegisteredApplicationId",
                table: "RedirectURI",
                column: "RegisteredApplicationId",
                principalTable: "Applications",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserApplicationToken_Applications_ClientId",
                table: "UserApplicationToken",
                column: "ClientId",
                principalTable: "Applications",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_RateLimits_OverallRateLimitRateLimitsId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_RateLimits_OverallRateLimitRateLimitsId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserApplicationToken_RateLimits_AppUserRateLimitsRateLimitsId",
                table: "UserApplicationToken");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_UserApplicationToken_TokenId",
                table: "Applications");

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
                name: "CreateManyUsersViewModel");

            migrationBuilder.DropTable(
                name: "CreateUserViewModel");

            migrationBuilder.DropTable(
                name: "Endpoint");

            migrationBuilder.DropTable(
                name: "ExternalUrl");

            migrationBuilder.DropTable(
                name: "RedirectURI");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "ServerSettings");

            migrationBuilder.DropTable(
                name: "ResponseModes");

            migrationBuilder.DropTable(
                name: "RateLimits");

            migrationBuilder.DropTable(
                name: "UserApplicationToken");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Applications");
        }
    }
}
