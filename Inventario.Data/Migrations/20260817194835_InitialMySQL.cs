using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventario.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMySQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "__EFMigrationsHistory",
                columns: table => new
                {
                    MigrationId = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductVersion = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.MigrationId);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_archivos",
                columns: table => new
                {
                    id_archivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    archivo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre_archivo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_subida = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_archivo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_estatus",
                columns: table => new
                {
                    id_estatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion_estatus = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_estatus);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_grupos",
                columns: table => new
                {
                    id_grupo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion_grupo = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_grupo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_marcas",
                columns: table => new
                {
                    id_marca = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre_marca = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_marca);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_proveedores",
                columns: table => new
                {
                    id_proveedor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre_proveedor = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    numero_contacto = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    correo_electronico = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_proveedor);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_rol_pya",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion_rol = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_rol);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_tipos_combustible",
                columns: table => new
                {
                    id_combustible = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion_combustible = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_combustible);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_tipos_equipos",
                columns: table => new
                {
                    id_tipo_equipo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion_tipo_equipo = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_tipo_equipo);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_ubicaciones_proyectos",
                columns: table => new
                {
                    id_ubicacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre_proyecto = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ubicacion = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    siglas = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_ubicacion);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "requisiciones",
                columns: table => new
                {
                    id_requisicion = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_ubicacion = table.Column<int>(type: "int", nullable: true),
                    consecutivo = table.Column<int>(type: "int", nullable: true),
                    razon_social = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_requisicion = table.Column<DateOnly>(type: "date", nullable: true),
                    tipo_requisicion = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_requisicion);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "rol_empleado",
                columns: table => new
                {
                    id_rol_empleado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion_rol = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_rol_empleado);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "usuarios_roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion_rol = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_rol);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_pya",
                columns: table => new
                {
                    id_pya = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_rol_pya = table.Column<int>(type: "int", nullable: true),
                    nombre = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_pya);
                    table.ForeignKey(
                        name: "fk_rol_pya",
                        column: x => x.id_rol_pya,
                        principalTable: "catalogo_rol_pya",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_tramos",
                columns: table => new
                {
                    id_tramo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_ubicacion = table.Column<int>(type: "int", nullable: true),
                    nombre_tramo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_tramo);
                    table.ForeignKey(
                        name: "fk_ubicacion_tramo",
                        column: x => x.id_ubicacion,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "empleados",
                columns: table => new
                {
                    no_empleado = table.Column<int>(type: "int", nullable: false),
                    nombre_empleado = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_rol_empleado = table.Column<int>(type: "int", nullable: true),
                    ds3 = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.no_empleado);
                    table.ForeignKey(
                        name: "fk_rolempleado_empleado",
                        column: x => x.id_rol_empleado,
                        principalTable: "rol_empleado",
                        principalColumn: "id_rol_empleado",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    nombre_usuario = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_rol = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_usuario);
                    table.ForeignKey(
                        name: "fk_usuarios_roles",
                        column: x => x.id_rol,
                        principalTable: "usuarios_roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_frentes",
                columns: table => new
                {
                    id_frente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_tramo = table.Column<int>(type: "int", nullable: true),
                    nombre_frente = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_frente);
                    table.ForeignKey(
                        name: "fk_frente_tramo",
                        column: x => x.id_tramo,
                        principalTable: "catalogo_tramos",
                        principalColumn: "id_tramo",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_economicos",
                columns: table => new
                {
                    id_economico = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    consecutivo = table.Column<int>(type: "int", nullable: true),
                    id_tipo_equipo = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_grupo = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_combustible = table.Column<int>(type: "int", nullable: true),
                    id_propietario = table.Column<int>(type: "int", nullable: true),
                    id_administrador = table.Column<int>(type: "int", nullable: true),
                    id_estatus = table.Column<int>(type: "int", nullable: true),
                    id_ubicacion = table.Column<int>(type: "int", nullable: true),
                    descripcion = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modelo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    serie = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    periodo_fabricacion = table.Column<int>(type: "int", nullable: true),
                    motor = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modelo_motor = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    serie_motor = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    familia_motor = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    horometro = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    dimensiones = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    thk = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    placas = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    grado_propiedad = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    observaciones_asignaciones = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    estatus_seguro = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    id_marca = table.Column<int>(type: "int", nullable: true),
                    marca_motor = table.Column<int>(type: "int", nullable: true),
                    valor_adquisicion = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    tipo_seguro = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_operador = table.Column<int>(type: "int", nullable: true),
                    id_responsable = table.Column<int>(type: "int", nullable: true),
                    verificado = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_economico);
                    table.ForeignKey(
                        name: "fk_administrador_economico",
                        column: x => x.id_administrador,
                        principalTable: "catalogo_pya",
                        principalColumn: "id_pya",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eco_combustible",
                        column: x => x.id_combustible,
                        principalTable: "catalogo_tipos_combustible",
                        principalColumn: "id_combustible",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eco_estatus",
                        column: x => x.id_estatus,
                        principalTable: "catalogo_estatus",
                        principalColumn: "id_estatus",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eco_grupo",
                        column: x => x.id_grupo,
                        principalTable: "catalogo_grupos",
                        principalColumn: "id_grupo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eco_marca",
                        column: x => x.id_marca,
                        principalTable: "catalogo_marcas",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eco_marca_motor",
                        column: x => x.marca_motor,
                        principalTable: "catalogo_marcas",
                        principalColumn: "id_marca",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eco_tipo_equipo",
                        column: x => x.id_tipo_equipo,
                        principalTable: "catalogo_tipos_equipos",
                        principalColumn: "id_tipo_equipo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_eco_ubicacion",
                        column: x => x.id_ubicacion,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_operador_economico",
                        column: x => x.id_operador,
                        principalTable: "empleados",
                        principalColumn: "no_empleado",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_propietario_economico",
                        column: x => x.id_propietario,
                        principalTable: "catalogo_pya",
                        principalColumn: "id_pya",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_responsabgle_economico",
                        column: x => x.id_responsable,
                        principalTable: "empleados",
                        principalColumn: "no_empleado",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "historial_logs",
                columns: table => new
                {
                    id_log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    descripcion_log = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tipo_log = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    fecha_log = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ip_address = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_agent = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_log);
                    table.ForeignKey(
                        name: "fk_log_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "catalogo_movimientos_economicos",
                columns: table => new
                {
                    id_movimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_economico = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_ubicacion_llegada = table.Column<int>(type: "int", nullable: true),
                    id_ubicacion_salida = table.Column<int>(type: "int", nullable: true),
                    ubicacion_personalizada = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_movimiento = table.Column<DateTime>(type: "datetime", nullable: true),
                    nombre_archivo_2 = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    archivo_2 = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    nombre_archivo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    archivo = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_movimiento);
                    table.ForeignKey(
                        name: "fk_economico_movimiento",
                        column: x => x.id_economico,
                        principalTable: "catalogo_economicos",
                        principalColumn: "id_economico",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ubicacion_llegada_movimiento",
                        column: x => x.id_ubicacion_llegada,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_ubicacion_salida_movimiento",
                        column: x => x.id_ubicacion_salida,
                        principalTable: "catalogo_ubicaciones_proyectos",
                        principalColumn: "id_ubicacion",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_usuario_movimiento",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "economicos_archivos",
                columns: table => new
                {
                    id_economico_archivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_archivo = table.Column<int>(type: "int", nullable: true),
                    id_economico = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_economico_archivo);
                    table.ForeignKey(
                        name: "fk_archivo_archivoeconomico",
                        column: x => x.id_archivo,
                        principalTable: "catalogo_archivos",
                        principalColumn: "id_archivo",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_economico_archivoeconomico",
                        column: x => x.id_economico,
                        principalTable: "catalogo_economicos",
                        principalColumn: "id_economico",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "historial_servicio",
                columns: table => new
                {
                    id_servicio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    no_economico = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fecha_mantenimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    tipo_mantenimiento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    anotaciones = table.Column<string>(type: "text", nullable: true, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id_servicio);
                    table.ForeignKey(
                        name: "fk_economico_servicio",
                        column: x => x.no_economico,
                        principalTable: "catalogo_economicos",
                        principalColumn: "id_economico",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "fk_administrador_economico",
                table: "catalogo_economicos",
                column: "id_administrador");

            migrationBuilder.CreateIndex(
                name: "fk_eco_combustible",
                table: "catalogo_economicos",
                column: "id_combustible");

            migrationBuilder.CreateIndex(
                name: "fk_eco_estatus",
                table: "catalogo_economicos",
                column: "id_estatus");

            migrationBuilder.CreateIndex(
                name: "fk_eco_grupo",
                table: "catalogo_economicos",
                column: "id_grupo");

            migrationBuilder.CreateIndex(
                name: "fk_eco_marca",
                table: "catalogo_economicos",
                column: "id_marca");

            migrationBuilder.CreateIndex(
                name: "fk_eco_marca_motor",
                table: "catalogo_economicos",
                column: "marca_motor");

            migrationBuilder.CreateIndex(
                name: "fk_eco_tipo_equipo",
                table: "catalogo_economicos",
                column: "id_tipo_equipo");

            migrationBuilder.CreateIndex(
                name: "fk_eco_ubicacion",
                table: "catalogo_economicos",
                column: "id_ubicacion");

            migrationBuilder.CreateIndex(
                name: "fk_operador_economico",
                table: "catalogo_economicos",
                column: "id_operador");

            migrationBuilder.CreateIndex(
                name: "fk_propietario_economico",
                table: "catalogo_economicos",
                column: "id_propietario");

            migrationBuilder.CreateIndex(
                name: "fk_responsabgle_economico",
                table: "catalogo_economicos",
                column: "id_responsable");

            migrationBuilder.CreateIndex(
                name: "fk_frente_tramo",
                table: "catalogo_frentes",
                column: "id_tramo");

            migrationBuilder.CreateIndex(
                name: "fk_economico_movimiento",
                table: "catalogo_movimientos_economicos",
                column: "id_economico");

            migrationBuilder.CreateIndex(
                name: "fk_ubicacion_llegada_movimiento",
                table: "catalogo_movimientos_economicos",
                column: "id_ubicacion_llegada");

            migrationBuilder.CreateIndex(
                name: "fk_ubicacion_salida_movimiento",
                table: "catalogo_movimientos_economicos",
                column: "id_ubicacion_salida");

            migrationBuilder.CreateIndex(
                name: "fk_usuario_movimiento",
                table: "catalogo_movimientos_economicos",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "fk_rol_pya",
                table: "catalogo_pya",
                column: "id_rol_pya");

            migrationBuilder.CreateIndex(
                name: "fk_ubicacion_tramo",
                table: "catalogo_tramos",
                column: "id_ubicacion");

            migrationBuilder.CreateIndex(
                name: "fk_archivo_archivoeconomico",
                table: "economicos_archivos",
                column: "id_archivo");

            migrationBuilder.CreateIndex(
                name: "fk_economico_archivoeconomico",
                table: "economicos_archivos",
                column: "id_economico");

            migrationBuilder.CreateIndex(
                name: "fk_rolempleado_empleado",
                table: "empleados",
                column: "id_rol_empleado");

            migrationBuilder.CreateIndex(
                name: "fk_log_usuario",
                table: "historial_logs",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "fk_economico_servicio",
                table: "historial_servicio",
                column: "no_economico");

            migrationBuilder.CreateIndex(
                name: "fk_usuarios_roles",
                table: "usuarios",
                column: "id_rol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__EFMigrationsHistory");

            migrationBuilder.DropTable(
                name: "catalogo_frentes");

            migrationBuilder.DropTable(
                name: "catalogo_movimientos_economicos");

            migrationBuilder.DropTable(
                name: "catalogo_proveedores");

            migrationBuilder.DropTable(
                name: "economicos_archivos");

            migrationBuilder.DropTable(
                name: "historial_logs");

            migrationBuilder.DropTable(
                name: "historial_servicio");

            migrationBuilder.DropTable(
                name: "requisiciones");

            migrationBuilder.DropTable(
                name: "catalogo_tramos");

            migrationBuilder.DropTable(
                name: "catalogo_archivos");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "catalogo_economicos");

            migrationBuilder.DropTable(
                name: "usuarios_roles");

            migrationBuilder.DropTable(
                name: "catalogo_pya");

            migrationBuilder.DropTable(
                name: "catalogo_tipos_combustible");

            migrationBuilder.DropTable(
                name: "catalogo_estatus");

            migrationBuilder.DropTable(
                name: "catalogo_grupos");

            migrationBuilder.DropTable(
                name: "catalogo_marcas");

            migrationBuilder.DropTable(
                name: "catalogo_tipos_equipos");

            migrationBuilder.DropTable(
                name: "catalogo_ubicaciones_proyectos");

            migrationBuilder.DropTable(
                name: "empleados");

            migrationBuilder.DropTable(
                name: "catalogo_rol_pya");

            migrationBuilder.DropTable(
                name: "rol_empleado");
        }
    }
}
