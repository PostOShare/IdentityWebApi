using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EntityORM.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Login",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RegisteredDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityPM_Login", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "OTPValidate",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    OTP = table.Column<decimal>(type: "numeric(6,0)", nullable: true),
                    RequestedTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    RetryAttempt = table.Column<decimal>(type: "numeric(2,0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityPM_OTPValidate", x => x.Username);
                    table.ForeignKey(
                        name: "FK_OTPValidate_Login",
                        column: x => x.Username,
                        principalTable: "Login",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityPM_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Login_User",
                        column: x => x.Username,
                        principalTable: "Login",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAuth",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityPM_UserAuth", x => x.Username);
                    table.ForeignKey(
                        name: "FK_UserAuth_Login",
                        column: x => x.Username,
                        principalTable: "Login",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Login_Username",
                table: "Login",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_OTPValidate_Username",
                table: "OTPValidate",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuth_Username",
                table: "UserAuth",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OTPValidate");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "UserAuth");

            migrationBuilder.DropTable(
                name: "Login");
        }
    }
}
