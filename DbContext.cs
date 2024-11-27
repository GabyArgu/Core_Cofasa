using CoreContable.Entities;
using CoreContable.Entities.FunctionResult;
using CoreContable.Entities.FuntionResult;
using CoreContable.Entities.Views;
using CoreContable.Models.Report;
using CoreContable.Models.ResultSet;
using CoreContable.Utils;
using Microsoft.EntityFrameworkCore;

namespace CoreContable {
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext {
        public DbContext(DbContextOptions<DbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // Schema por defecto.
            modelBuilder.HasDefaultSchema(CC.SCHEMA);

            // Configuración de modelos sin llave primaria.
            modelBuilder.Entity<UserMenuPermissionFromFunctionResult>(e => e.HasNoKey());
            modelBuilder.Entity<ValidateUserOnLoginFromFunctionResult>(e => e.HasNoKey());
            modelBuilder.Entity<ObtenerDatosRepositorioResult>(e => e.HasNoKey());
            modelBuilder.Entity<ReportePolizaDiarioFromFunc>(e => e.HasNoKey());
            modelBuilder.Entity<ReportePolizaMayorFromFunc>(e => e.HasNoKey());
            modelBuilder.Entity<ReporteHistoricoCuentaFromFunc>(e => e.HasNoKey());
            modelBuilder.Entity<ReporteBalanceComprobacionFromFunc>(e => e.HasNoKey());
            modelBuilder.Entity<ConsultarCentroCuentaFromFunc>(e => e.HasNoKey());

            // Registro del nuevo modelo sin llave primaria
            modelBuilder.Entity<ReporteBalanceGralFromFunc>(e => e.HasNoKey());
            modelBuilder.Entity<ReporteDiarioMayorFromFunc>(e => e.HasNoKey());
            modelBuilder.Entity<ReporteEstadoResultadosDetalle>(e => e.HasNoKey());

            // Varias llaves primarias
            modelBuilder.Entity<Repositorio>().HasKey(x => new { x.COD_CIA, x.PERIODO, x.TIPO_DOCTO, x.NUM_POLIZA, x.NUM_REFERENCIA });
            modelBuilder.Entity<DmgNumera>().HasKey(x => new { x.COD_CIA, x.TIPO_DOCTO, x.ANIO, x.MES });
            modelBuilder.Entity<DmgCieCierre>().HasKey(x => new { x.CIE_CODCIA, x.CIE_CODIGO });
            modelBuilder.Entity<DetRepositorio>().HasKey(x => new { x.COD_CIA, x.PERIODO, x.TIPO_DOCTO, x.NUM_POLIZA, x.CORRELAT });
            modelBuilder.Entity<DmgPoliza>().HasKey(x => new { x.COD_CIA, x.PERIODO, x.TIPO_DOCTO, x.NUM_POLIZA });
            modelBuilder.Entity<CentroCuenta>().HasKey(x => new { x.COD_CIA, x.CENTRO_COSTO, x.CTA_1, x.CTA_2, x.CTA_3, x.CTA_4, x.CTA_5, x.CTA_6 });

            // Configuración de vistas existentes
            modelBuilder.Entity<CentroCuentaView>().ToView("vw_centro_cuenta").HasKey(x => new { x.COD_CIA, x.CENTRO_COSTO, x.CTA_1, x.CTA_2, x.CTA_3, x.CTA_4, x.CTA_5, x.CTA_6 });
            modelBuilder.Entity<DmgPolizaView>().ToView("vw_dmgpoliza").HasKey(x => new { x.COD_CIA, x.PERIODO, x.TIPO_DOCTO, x.NUM_POLIZA });
            modelBuilder.Entity<RepositorioView>().ToView("vw_repositorio").HasKey(x => new { x.COD_CIA, x.PERIODO, x.TIPO_DOCTO, x.NUM_POLIZA });
            modelBuilder.Entity<CuentasContablesView>().ToView("V_CuentasContables").HasKey(x => new { x.COD_CIA, x.CuentasConcatenadas, x.CuentaContable });

            // Configuración de la nueva vista CentroCuentaFormatoView
            modelBuilder.Entity<CentroCuentaFormatoView>()
                .ToView("vw_centro_cuenta_formato", "CONTABLE")
                .HasKey(x => new { x.COD_CIA, x.CENTRO_COSTO, x.CTA_1, x.CTA_2, x.CTA_3, x.CTA_4, x.CTA_5, x.CTA_6 });

            // Configuración de triggers
            modelBuilder.Entity<DetRepositorio>(entry => { entry.ToTable("det_repositorio", tb => tb.HasTrigger("Det_Repositorio_Insert")); });
            modelBuilder.Entity<DetRepositorio>(entry => { entry.ToTable("det_repositorio", tb => tb.HasTrigger("Det_repositorio_Update")); });
        }

        // DbSets de entidades
        public DbSet<Permission> Permission { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RolePermission> RolePermission { get; set; }
        public DbSet<UserApp> UserApp { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<Cias> Cias { get; set; }
        public DbSet<UserCia> UserCia { get; set; }
        public DbSet<AcMonMoneda> AcMonMoneda { get; set; }
        public DbSet<DmgParam> DmgParam { get; set; }
        public DbSet<DmgPeriod> DmgPeriod { get; set; }
        public DbSet<DmgCieCierre> DmgCieCierre { get; set; }
        public DbSet<DmgDoctos> DmgDoctos { get; set; }
        public DbSet<DmgCuentas> DmgCuentas { get; set; }
        public DbSet<CentroCosto> CentroCosto { get; set; }
        public DbSet<CentroCuenta> CentroCuenta { get; set; }

        public DbSet<Repositorio> Repositorio { get; set; }
        public DbSet<DetRepositorio> DetRepositorio { get; set; }
        public DbSet<DmgPoliza> DmgPoliza { get; set; }
        public DbSet<DmgDetalle> DmgDetalle { get; set; }
        public DbSet<DmgNumera> DmgNumera { get; set; }

        public DbSet<RepositoryImportLog> RepositoryImportLog { get; set; }

        // DbSets de vistas
        public DbSet<CentroCuentaView> CentroCuentaView { get; set; }
        public DbSet<DmgPolizaView> DmgPolizaView { get; set; }
        public DbSet<RepositorioView> RepositorioView { get; set; }
        public DbSet<CuentasContablesView> CuentasContablesView { get; set; }

        // Nuevo DbSet para la vista CentroCuentaFormatoView
        public DbSet<CentroCuentaFormatoView> CentroCuentaFormatoView { get; set; }

        // DbSets de resultados de funciones
        public DbSet<UserMenuPermissionFromFunctionResult> UserMenuPermissionFromFunctionResult { get; set; }
        public DbSet<ValidateUserOnLoginFromFunctionResult> ValidateUserOnLoginFromFunctionResult { get; set; }
        public DbSet<DetRepositorioFromFuncForDt> DetRepositorioFromFuncForDt { get; set; }
        public DbSet<ObtenerDatosRepositorioResult> ObtenerDatosRepositorioResult { get; set; }
        public DbSet<ReportePolizaDiarioFromFunc> ReportePolizaDiarioFromFunc { get; set; }
        public DbSet<ReportePolizaMayorFromFunc> ReportePolizaMayorFromFunc { get; set; }
        public DbSet<ReporteHistoricoCuentaFromFunc> ReporteHistoricoCuentaFromFunc { get; set; }
        public DbSet<ReporteBalanceComprobacionFromFunc> ReporteBalanceComprobacionFromFunc { get; set; }
        public DbSet<ObtenerDatosDmgPolizaFromFunc> ObtenerDatosDmgPolizaFromFunc { get; set; }
        public DbSet<ConsultarCentroCuentaFromFunc> ConsultarCentroCuentaFromFunc { get; set; }

        // Nuevo DbSet para el reporte de Balance General
        public DbSet<ReporteBalanceGralFromFunc> ReporteBalanceGralFromFunc { get; set; }

        public DbSet<ReporteDiarioMayorFromFunc> ReporteDiarioMayorFromFunc { get; set; }

        public DbSet<ReporteEstadoResultadosDetalle> ReporteEstadoResultadosFromFunc { get; set; }
    }
}
