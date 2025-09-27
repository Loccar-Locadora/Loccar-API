using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoccarInfra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOCATARIO",
                columns: table => new
                {
                    idlocatario = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('locatario_idlocatario_seq'::regclass)"),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    telefone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    locador = table.Column<bool>(type: "boolean", nullable: true),
                    login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    senha = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("locatario_pkey", x => x.idlocatario);
                });

            migrationBuilder.CreateTable(
                name: "VEICULO",
                columns: table => new
                {
                    idveiculo = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('veiculo_idveiculo_seq'::regclass)"),
                    marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    modelo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    anofabricacao = table.Column<int>(type: "integer", nullable: true),
                    anomodelo = table.Column<int>(type: "integer", nullable: true),
                    renavam = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: true),
                    capacidadetanque = table.Column<int>(type: "integer", nullable: true),
                    valordiaria = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    valordiariareduzida = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    valordiariamensal = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    valordiariaempresa = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("veiculo_pkey", x => x.idveiculo);
                });

            migrationBuilder.CreateTable(
                name: "PESSOA_FISICA",
                columns: table => new
                {
                    idlocatario = table.Column<int>(type: "integer", nullable: false),
                    cpf = table.Column<string>(type: "character(11)", fixedLength: true, maxLength: 11, nullable: false),
                    estadoCivil = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contratado = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pessoa_fisica_pkey", x => x.idlocatario);
                    table.ForeignKey(
                        name: "pessoa_fisica_idlocatario_fkey",
                        column: x => x.idlocatario,
                        principalTable: "LOCATARIO",
                        principalColumn: "idlocatario");
                });

            migrationBuilder.CreateTable(
                name: "PESSOA_JURIDICA",
                columns: table => new
                {
                    idlocatario = table.Column<int>(type: "integer", nullable: false),
                    cnpj = table.Column<string>(type: "character(14)", fixedLength: true, maxLength: 14, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pessoa_juridica_pkey", x => x.idlocatario);
                    table.ForeignKey(
                        name: "pessoa_juridica_idlocatario_fkey",
                        column: x => x.idlocatario,
                        principalTable: "LOCATARIO",
                        principalColumn: "idlocatario");
                });

            migrationBuilder.CreateTable(
                name: "MOTOCICLETA",
                columns: table => new
                {
                    idveiculo = table.Column<int>(type: "integer", nullable: false),
                    controletracao = table.Column<bool>(type: "boolean", nullable: true),
                    freioabs = table.Column<bool>(type: "boolean", nullable: true),
                    pilotoautomatico = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("motocicleta_pkey", x => x.idveiculo);
                    table.ForeignKey(
                        name: "motocicleta_idveiculo_fkey",
                        column: x => x.idveiculo,
                        principalTable: "VEICULO",
                        principalColumn: "idveiculo");
                });

            migrationBuilder.CreateTable(
                name: "RESERVA",
                columns: table => new
                {
                    numeroreserva = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('reserva_numeroreserva_seq'::regclass)"),
                    idlocatario = table.Column<int>(type: "integer", nullable: false),
                    idveiculo = table.Column<int>(type: "integer", nullable: false),
                    datalocacao = table.Column<DateOnly>(type: "date", nullable: false),
                    horalocacao = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    dataentrega = table.Column<DateOnly>(type: "date", nullable: true),
                    horaentrega = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    diarias = table.Column<int>(type: "integer", nullable: true),
                    valordiaria = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    tipodiaria = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    valorsegurocarro = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    valorseguroterceiro = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    valorimposto = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reserva_pkey", x => x.numeroreserva);
                    table.ForeignKey(
                        name: "reserva_idlocatario_fkey",
                        column: x => x.idlocatario,
                        principalTable: "LOCATARIO",
                        principalColumn: "idlocatario");
                    table.ForeignKey(
                        name: "reserva_idveiculo_fkey",
                        column: x => x.idveiculo,
                        principalTable: "VEICULO",
                        principalColumn: "idveiculo");
                });

            migrationBuilder.CreateTable(
                name: "VEICULO_CARGA",
                columns: table => new
                {
                    idveiculo = table.Column<int>(type: "integer", nullable: false),
                    capacidadecarga = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    tipocarga = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tara = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    tamanhocompartimentocarga = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("veiculo_carga_pkey", x => x.idveiculo);
                    table.ForeignKey(
                        name: "veiculo_carga_idveiculo_fkey",
                        column: x => x.idveiculo,
                        principalTable: "VEICULO",
                        principalColumn: "idveiculo");
                });

            migrationBuilder.CreateTable(
                name: "VEICULO_PASSAGEIRO",
                columns: table => new
                {
                    idveiculo = table.Column<int>(type: "integer", nullable: false),
                    capacidadepassageiro = table.Column<int>(type: "integer", nullable: true),
                    televisao = table.Column<bool>(type: "boolean", nullable: true),
                    arcondicionado = table.Column<bool>(type: "boolean", nullable: true),
                    direcaohidraulica = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("veiculo_passageiro_pkey", x => x.idveiculo);
                    table.ForeignKey(
                        name: "veiculo_passageiro_idveiculo_fkey",
                        column: x => x.idveiculo,
                        principalTable: "VEICULO",
                        principalColumn: "idveiculo");
                });

            migrationBuilder.CreateTable(
                name: "VEICULO_PASSEIO",
                columns: table => new
                {
                    idveiculo = table.Column<int>(type: "integer", nullable: false),
                    automatico = table.Column<bool>(type: "boolean", nullable: true),
                    direcaohidraulica = table.Column<bool>(type: "boolean", nullable: true),
                    arcondicionado = table.Column<bool>(type: "boolean", nullable: true),
                    categoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("veiculo_passeio_pkey", x => x.idveiculo);
                    table.ForeignKey(
                        name: "veiculo_passeio_idveiculo_fkey",
                        column: x => x.idveiculo,
                        principalTable: "VEICULO",
                        principalColumn: "idveiculo");
                });

            migrationBuilder.CreateIndex(
                name: "locatario_login_key",
                table: "LOCATARIO",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "pessoa_fisica_cpf_key",
                table: "PESSOA_FISICA",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "pessoa_juridica_cnpj_key",
                table: "PESSOA_JURIDICA",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RESERVA_idlocatario",
                table: "RESERVA",
                column: "idlocatario");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVA_idveiculo",
                table: "RESERVA",
                column: "idveiculo");

            migrationBuilder.CreateIndex(
                name: "veiculo_renavam_key",
                table: "VEICULO",
                column: "renavam",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MOTOCICLETA");

            migrationBuilder.DropTable(
                name: "PESSOA_FISICA");

            migrationBuilder.DropTable(
                name: "PESSOA_JURIDICA");

            migrationBuilder.DropTable(
                name: "RESERVA");

            migrationBuilder.DropTable(
                name: "VEICULO_CARGA");

            migrationBuilder.DropTable(
                name: "VEICULO_PASSAGEIRO");

            migrationBuilder.DropTable(
                name: "VEICULO_PASSEIO");

            migrationBuilder.DropTable(
                name: "LOCATARIO");

            migrationBuilder.DropTable(
                name: "VEICULO");
        }
    }
}
