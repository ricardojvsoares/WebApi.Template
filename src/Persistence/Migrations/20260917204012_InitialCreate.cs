using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "permissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email_normalized = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    display_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_users_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permissions",
                columns: table => new
                {
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "fk_role_permissions_permission",
                        column: x => x.permission_id,
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_permissions_role",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    price = table.Column<float>(type: "real", nullable: false),
                    image = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                    table.ForeignKey(
                        name: "fk_products_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_products_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    replaced_by_token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "todos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    due_date_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    owner_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_todos", x => x.id);
                    table.ForeignKey(
                        name: "fk_todos_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_todos_owner_user",
                        column: x => x.owner_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_todos_updated_by",
                        column: x => x.updated_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_role",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_user",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { new Guid("0cd3edf6-686d-d4ae-ff47-b9d3bbbfbe69"), "Read product", "read:product" },
                    { new Guid("34fa5c42-9981-b912-3d92-520d024a934a"), "Create todo", "create:todo" },
                    { new Guid("575735b2-7b8a-b45f-91f0-e4763fc17672"), "Delete product", "delete:product" },
                    { new Guid("59476916-57a5-3c4d-c5ff-d4b89ba136b9"), "Create product", "create:product" },
                    { new Guid("77aeadbe-6e8a-8ed2-82c1-0d78e92801c7"), "Update user", "update:user" },
                    { new Guid("78d293f3-e805-fc2c-bc91-53905131b862"), "Delete todo", "delete:todo" },
                    { new Guid("8243ef35-3764-a5ad-8d19-c092f7b5eda5"), "Read todo", "read:todo" },
                    { new Guid("8c4558ae-4d3e-9b2e-5d92-c66ae44405a7"), "Create user", "create:user" },
                    { new Guid("a27d8f7d-6d6b-ba97-631d-cf6271e6993b"), "Delete user", "delete:user" },
                    { new Guid("c2251b89-ad44-b870-3970-3b759236b276"), "Update todo", "update:todo" },
                    { new Guid("c9e8cc75-68f3-82db-6581-9d5a3c104560"), "Read user", "read:user" },
                    { new Guid("e1903468-a0e4-73f6-86d2-8187c669a819"), "Update product", "update:product" }
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Every permission the API defines.", "admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Baseline access: full control over the caller's own todos.", "user" }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id" },
                values: new object[,]
                {
                    { new Guid("0cd3edf6-686d-d4ae-ff47-b9d3bbbfbe69"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("34fa5c42-9981-b912-3d92-520d024a934a"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("575735b2-7b8a-b45f-91f0-e4763fc17672"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("59476916-57a5-3c4d-c5ff-d4b89ba136b9"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("77aeadbe-6e8a-8ed2-82c1-0d78e92801c7"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("78d293f3-e805-fc2c-bc91-53905131b862"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("8243ef35-3764-a5ad-8d19-c092f7b5eda5"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("8c4558ae-4d3e-9b2e-5d92-c66ae44405a7"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("a27d8f7d-6d6b-ba97-631d-cf6271e6993b"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("c2251b89-ad44-b870-3970-3b759236b276"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("c9e8cc75-68f3-82db-6581-9d5a3c104560"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("e1903468-a0e4-73f6-86d2-8187c669a819"), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("0cd3edf6-686d-d4ae-ff47-b9d3bbbfbe69"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("34fa5c42-9981-b912-3d92-520d024a934a"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("575735b2-7b8a-b45f-91f0-e4763fc17672"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("59476916-57a5-3c4d-c5ff-d4b89ba136b9"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("78d293f3-e805-fc2c-bc91-53905131b862"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("8243ef35-3764-a5ad-8d19-c092f7b5eda5"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("c2251b89-ad44-b870-3970-3b759236b276"), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("e1903468-a0e4-73f6-86d2-8187c669a819"), new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.CreateIndex(
                name: "uq_permissions_name",
                table: "permissions",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_products_created_at_utc",
                table: "products",
                column: "created_at_utc",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_products_created_by",
                table: "products",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_products_updated_by",
                table: "products",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uq_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "uq_roles_name",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_todos_created_by",
                table: "todos",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_todos_owner_user_id_is_completed",
                table: "todos",
                columns: new[] { "owner_user_id", "is_completed" });

            migrationBuilder.CreateIndex(
                name: "ix_todos_updated_by",
                table: "todos",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_created_by",
                table: "users",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_users_updated_by",
                table: "users",
                column: "updated_by");

            migrationBuilder.CreateIndex(
                name: "uq_users_email_normalized",
                table: "users",
                column: "email_normalized",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "role_permissions");

            migrationBuilder.DropTable(
                name: "todos");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "permissions");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
