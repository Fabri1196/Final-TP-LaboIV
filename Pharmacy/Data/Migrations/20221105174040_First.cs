using Microsoft.EntityFrameworkCore.Migrations;

namespace Pharmacy.Data.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "healthcareSystem",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_healthcareSystem", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lab",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    city = table.Column<string>(nullable: true),
                    country = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lab", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customer",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    surname = table.Column<string>(nullable: true),
                    name = table.Column<string>(nullable: true),
                    photo = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    HealthcareSystemid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_healthcareSystem_HealthcareSystemid",
                        column: x => x.HealthcareSystemid,
                        principalTable: "healthcareSystem",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "medication",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(nullable: true),
                    price = table.Column<int>(nullable: false),
                    Categoryid = table.Column<int>(nullable: false),
                    Labid = table.Column<int>(nullable: false),
                    photo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_medication", x => x.id);
                    table.ForeignKey(
                        name: "FK_medication_category_Categoryid",
                        column: x => x.Categoryid,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_medication_lab_Labid",
                        column: x => x.Labid,
                        principalTable: "lab",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "customerMedication",
                columns: table => new
                {
                    Customerid = table.Column<int>(nullable: false),
                    Medicationid = table.Column<int>(nullable: false),
                    id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customerMedication", x => new { x.Customerid, x.Medicationid });
                    table.ForeignKey(
                        name: "FK_customerMedication_customer_Customerid",
                        column: x => x.Customerid,
                        principalTable: "customer",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customerMedication_medication_Medicationid",
                        column: x => x.Medicationid,
                        principalTable: "medication",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_HealthcareSystemid",
                table: "customer",
                column: "HealthcareSystemid");

            migrationBuilder.CreateIndex(
                name: "IX_customerMedication_Medicationid",
                table: "customerMedication",
                column: "Medicationid");

            migrationBuilder.CreateIndex(
                name: "IX_medication_Categoryid",
                table: "medication",
                column: "Categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_medication_Labid",
                table: "medication",
                column: "Labid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "customerMedication");

            migrationBuilder.DropTable(
                name: "customer");

            migrationBuilder.DropTable(
                name: "medication");

            migrationBuilder.DropTable(
                name: "healthcareSystem");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "lab");
        }
    }
}
