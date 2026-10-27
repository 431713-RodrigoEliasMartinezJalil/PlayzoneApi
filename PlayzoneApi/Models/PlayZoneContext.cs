using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PlayzoneApi.Models;

public partial class PlayZoneContext : DbContext
{
    public PlayZoneContext()
    {
    }

    public PlayZoneContext(DbContextOptions<PlayZoneContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BibliotecaUsuario> BibliotecaUsuarios { get; set; }

    public virtual DbSet<DetalleOrden> DetalleOrdens { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<EstadoCuentum> EstadoCuenta { get; set; }

    public virtual DbSet<EstadosOrden> EstadosOrdens { get; set; }

    public virtual DbSet<EstadosPublicacion> EstadosPublicacions { get; set; }

    public virtual DbSet<EstadosReembolso> EstadosReembolsos { get; set; }

    public virtual DbSet<Etiqueta> Etiquetas { get; set; }

    public virtual DbSet<Genero> Generos { get; set; }

    public virtual DbSet<ListaDeseado> ListaDeseados { get; set; }

    public virtual DbSet<MetodosPago> MetodosPagos { get; set; }

    public virtual DbSet<Moneda> Monedas { get; set; }

    public virtual DbSet<NivelesUsuario> NivelesUsuarios { get; set; }

    public virtual DbSet<OrdenesCompra> OrdenesCompras { get; set; }

    public virtual DbSet<Pai> Pais { get; set; }

    public virtual DbSet<PreciosProducto> PreciosProductos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ProductosEmpresa> ProductosEmpresas { get; set; }

    public virtual DbSet<ProductosEtiqueta> ProductosEtiquetas { get; set; }

    public virtual DbSet<ProductosGenero> ProductosGeneros { get; set; }

    public virtual DbSet<Promocione> Promociones { get; set; }

    public virtual DbSet<PromocionesProducto> PromocionesProductos { get; set; }

    public virtual DbSet<Reembolso> Reembolsos { get; set; }

    public virtual DbSet<Reseña> Reseñas { get; set; }

    public virtual DbSet<TiposProducto> TiposProductos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VistaResumenCompra> VistaResumenCompras { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PlayZone;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BibliotecaUsuario>(entity =>
        {
            entity.HasKey(e => e.IdBiblioteca).HasName("PK__Bibliote__1EEBBDFEFB10A8F8");

            entity.ToTable("Biblioteca_Usuarios");

            entity.HasIndex(e => new { e.IdUsuario, e.IdProducto }, "UQ_Biblioteca_Usuario_Producto").IsUnique();

            entity.Property(e => e.IdBiblioteca).HasColumnName("id_biblioteca");
            entity.Property(e => e.FechaAdquisicion)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_adquisicion");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.OrigenAdquisicion)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("Compra_Directa")
                .HasColumnName("origen_adquisicion");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.BibliotecaUsuarios)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Biblioteca_Productos");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.BibliotecaUsuarios)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Biblioteca_Usuarios");
        });

        modelBuilder.Entity<DetalleOrden>(entity =>
        {
            entity.HasKey(e => e.IdDetalleOrden).HasName("PK__Detalle___D2FC3FD76F0DC400");

            entity.ToTable("Detalle_Orden");

            entity.Property(e => e.IdDetalleOrden).HasColumnName("id_detalle_orden");
            entity.Property(e => e.EsRegalo).HasColumnName("es_regalo");
            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.MontoDescuento)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_descuento");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Detalle_Ordenes");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Detalle_Productos");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PK__Empresas__4A0B7E2C7B1C3655");

            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Nombre)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.SitioWeb)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("sitio_web");

            entity.HasOne(d => d.IdPaisNavigation).WithMany(p => p.Empresas)
                .HasForeignKey(d => d.IdPais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Empresas_Pais");
        });

        modelBuilder.Entity<EstadoCuentum>(entity =>
        {
            entity.HasKey(e => e.IdEstadoCuenta).HasName("PK__Estado_C__5242412A2F4DF01C");

            entity.ToTable("Estado_Cuenta");

            entity.HasIndex(e => e.Estado, "UQ__Estado_C__40237F16ECCA175B").IsUnique();

            entity.Property(e => e.IdEstadoCuenta).HasColumnName("id_estado_cuenta");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
        });

        modelBuilder.Entity<EstadosOrden>(entity =>
        {
            entity.HasKey(e => e.IdEstadoOrden).HasName("PK__Estados___603FD4BC243076CE");

            entity.ToTable("Estados_Orden");

            entity.HasIndex(e => e.Nombre, "UQ__Estados___72AFBCC6F1FE89FF").IsUnique();

            entity.Property(e => e.IdEstadoOrden).HasColumnName("id_estado_orden");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadosPublicacion>(entity =>
        {
            entity.HasKey(e => e.IdEstadoPublicacion).HasName("PK__Estados___51A93F3FF0C74C60");

            entity.ToTable("Estados_Publicacion");

            entity.HasIndex(e => e.Nombre, "UQ__Estados___72AFBCC642B717EA").IsUnique();

            entity.Property(e => e.IdEstadoPublicacion).HasColumnName("id_estado_publicacion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<EstadosReembolso>(entity =>
        {
            entity.HasKey(e => e.IdEstadoReembolso).HasName("PK__Estados___62655AF0B312720C");

            entity.ToTable("Estados_Reembolso");

            entity.HasIndex(e => e.Nombre, "UQ__Estados___72AFBCC6CAE32A61").IsUnique();

            entity.Property(e => e.IdEstadoReembolso).HasColumnName("id_estado_reembolso");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.HasKey(e => e.IdEtiqueta).HasName("PK__Etiqueta__FA0DD2AD5EDE19D0");

            entity.HasIndex(e => e.Nombre, "UQ__Etiqueta__72AFBCC6A6701D2B").IsUnique();

            entity.Property(e => e.IdEtiqueta).HasColumnName("id_etiqueta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Genero>(entity =>
        {
            entity.HasKey(e => e.IdGenero).HasName("PK__Generos__99A8E4F91EF1DFF0");

            entity.HasIndex(e => e.Nombre, "UQ__Generos__72AFBCC66D029A59").IsUnique();

            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<ListaDeseado>(entity =>
        {
            entity.HasKey(e => e.IdListaDeseados).HasName("PK__Lista_De__941E625593C14490");

            entity.ToTable("Lista_Deseados");

            entity.HasIndex(e => new { e.IdUsuario, e.IdProducto }, "UQ_Lista_Usuario_Producto").IsUnique();

            entity.Property(e => e.IdListaDeseados).HasColumnName("id_lista_deseados");
            entity.Property(e => e.FechaAgregado)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_agregado");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ListaDeseados)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lista_Producto");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.ListaDeseados)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lista_Usuarios");
        });

        modelBuilder.Entity<MetodosPago>(entity =>
        {
            entity.HasKey(e => e.IdMetodoPago).HasName("PK__Metodos___85BE0EBC7444BF7C");

            entity.ToTable("Metodos_Pago");

            entity.HasIndex(e => e.Nombre, "UQ__Metodos___72AFBCC61C5CAD27").IsUnique();

            entity.Property(e => e.IdMetodoPago).HasColumnName("id_metodo_pago");
            entity.Property(e => e.Nombre)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Moneda>(entity =>
        {
            entity.HasKey(e => e.IdMoneda).HasName("PK__Monedas__807063E37C7DE7FC");

            entity.HasIndex(e => e.Codigo, "UQ__Monedas__40F9A206964C390F").IsUnique();

            entity.HasIndex(e => e.Nombre, "UQ__Monedas__72AFBCC6596BA4CF").IsUnique();

            entity.Property(e => e.IdMoneda).HasColumnName("id_moneda");
            entity.Property(e => e.Codigo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<NivelesUsuario>(entity =>
        {
            entity.HasKey(e => e.IdNivelUsuario).HasName("PK__Niveles___797F31A5AF3E82A2");

            entity.ToTable("Niveles_Usuarios");

            entity.HasIndex(e => e.IdUsuario, "UQ__Niveles___4E3E04AC9A14ABCC").IsUnique();

            entity.Property(e => e.IdNivelUsuario).HasColumnName("id_nivel_usuario");
            entity.Property(e => e.Experiencia).HasColumnName("experiencia");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Nivel)
                .HasDefaultValue(1)
                .HasColumnName("nivel");

            entity.HasOne(d => d.IdUsuarioNavigation).WithOne(p => p.NivelesUsuario)
                .HasForeignKey<NivelesUsuario>(d => d.IdUsuario)
                .HasConstraintName("FK_Niveles_Usuarios_Usuario");
        });

        modelBuilder.Entity<OrdenesCompra>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__Ordenes___DD5B8F33DC74F67C");

            entity.ToTable("Ordenes_Compra");

            entity.Property(e => e.IdOrden).HasColumnName("id_orden");
            entity.Property(e => e.FechaOrden)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_orden");
            entity.Property(e => e.IdEstadoOrden).HasColumnName("id_estado_orden");
            entity.Property(e => e.IdMetodoPago).HasColumnName("id_metodo_pago");
            entity.Property(e => e.IdMoneda).HasColumnName("id_moneda");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdEstadoOrdenNavigation).WithMany(p => p.OrdenesCompras)
                .HasForeignKey(d => d.IdEstadoOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ordenes_Estados");

            entity.HasOne(d => d.IdMetodoPagoNavigation).WithMany(p => p.OrdenesCompras)
                .HasForeignKey(d => d.IdMetodoPago)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ordenes_Metodos_Pago");

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.OrdenesCompras)
                .HasForeignKey(d => d.IdMoneda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ordenes_Monedas");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.OrdenesCompras)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ordenes_Usuarios");
        });

        modelBuilder.Entity<Pai>(entity =>
        {
            entity.HasKey(e => e.IdPais).HasName("PK__Pais__0941A3A7F0506700");

            entity.HasIndex(e => e.Nombre, "UQ__Pais__72AFBCC6A5A9C254").IsUnique();

            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<PreciosProducto>(entity =>
        {
            entity.HasKey(e => e.IdPrecioProducto).HasName("PK__Precios___C6D95EF0F7A7DCE8");

            entity.ToTable("Precios_Producto");

            entity.Property(e => e.IdPrecioProducto).HasColumnName("id_precio_producto");
            entity.Property(e => e.FechaFinVigencia).HasColumnName("fecha_fin_vigencia");
            entity.Property(e => e.FechaInicioVigencia)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_inicio_vigencia");
            entity.Property(e => e.IdMoneda).HasColumnName("id_moneda");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.PrecioMonto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_monto");

            entity.HasOne(d => d.IdMonedaNavigation).WithMany(p => p.PreciosProductos)
                .HasForeignKey(d => d.IdMoneda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Precios_Moneda");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.PreciosProductos)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Precios_Producto");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__FF341C0D3E64CA30");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.ClasificacionEdad)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("clasificacion_edad");
            entity.Property(e => e.Descripcion)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaLanzamiento).HasColumnName("fecha_lanzamiento");
            entity.Property(e => e.IdEstadoPublicacion).HasColumnName("id_estado_publicacion");
            entity.Property(e => e.IdProductoPadre).HasColumnName("id_producto_padre");
            entity.Property(e => e.IdTipoProducto).HasColumnName("id_tipo_producto");
            entity.Property(e => e.Titulo)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdEstadoPublicacionNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdEstadoPublicacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Estado");

            entity.HasOne(d => d.IdProductoPadreNavigation).WithMany(p => p.InverseIdProductoPadreNavigation)
                .HasForeignKey(d => d.IdProductoPadre)
                .HasConstraintName("FK_Productos_Padre");

            entity.HasOne(d => d.IdTipoProductoNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdTipoProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Tipo");
        });

        modelBuilder.Entity<ProductosEmpresa>(entity =>
        {
            entity.HasKey(e => e.IdProductoEmpresa).HasName("PK__Producto__40F6BB5530D2E3E3");

            entity.ToTable("Productos_Empresas");

            entity.HasIndex(e => new { e.IdProducto, e.IdEmpresa, e.Rol }, "UQ_Producto_Empresa_Rol").IsUnique();

            entity.Property(e => e.IdProductoEmpresa).HasColumnName("id_producto_empresa");
            entity.Property(e => e.IdEmpresa).HasColumnName("id_empresa");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("rol");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.ProductosEmpresas)
                .HasForeignKey(d => d.IdEmpresa)
                .HasConstraintName("FK_Productos_Empresas_Empresa");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductosEmpresas)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Productos_Empresas_Producto");
        });

        modelBuilder.Entity<ProductosEtiqueta>(entity =>
        {
            entity.HasKey(e => e.IdProductoEtiqueta).HasName("PK__Producto__617DF62D8D1A9B54");

            entity.ToTable("Productos_Etiquetas");

            entity.HasIndex(e => new { e.IdProducto, e.IdEtiqueta }, "UQ_Producto_Etiqueta").IsUnique();

            entity.Property(e => e.IdProductoEtiqueta).HasColumnName("id_producto_etiqueta");
            entity.Property(e => e.IdEtiqueta).HasColumnName("id_etiqueta");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");

            entity.HasOne(d => d.IdEtiquetaNavigation).WithMany(p => p.ProductosEtiqueta)
                .HasForeignKey(d => d.IdEtiqueta)
                .HasConstraintName("FK_Productos_Etiquetas_Etiqueta");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductosEtiqueta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Productos_Etiquetas_Producto");
        });

        modelBuilder.Entity<ProductosGenero>(entity =>
        {
            entity.HasKey(e => e.IdProductoGenero).HasName("PK__Producto__F016D71D50122E11");

            entity.ToTable("Productos_Generos");

            entity.HasIndex(e => new { e.IdProducto, e.IdGenero }, "UQ_Producto_Genero").IsUnique();

            entity.Property(e => e.IdProductoGenero).HasColumnName("id_producto_genero");
            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");

            entity.HasOne(d => d.IdGeneroNavigation).WithMany(p => p.ProductosGeneros)
                .HasForeignKey(d => d.IdGenero)
                .HasConstraintName("FK_Productos_Generos_Genero");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ProductosGeneros)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Productos_Generos_Producto");
        });

        modelBuilder.Entity<Promocione>(entity =>
        {
            entity.HasKey(e => e.IdPromocion).HasName("PK__Promocio__F89308E0B6C051FB");

            entity.Property(e => e.IdPromocion).HasColumnName("id_promocion");
            entity.Property(e => e.Activa)
                .HasDefaultValue(true)
                .HasColumnName("activa");
            entity.Property(e => e.FechaFin).HasColumnName("fecha_fin");
            entity.Property(e => e.FechaInicio).HasColumnName("fecha_inicio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.PorcentajeDescuento)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("porcentaje_descuento");
        });

        modelBuilder.Entity<PromocionesProducto>(entity =>
        {
            entity.HasKey(e => e.IdPromocionProducto).HasName("PK__Promocio__0481E9B86DDD79A6");

            entity.ToTable("Promociones_Productos");

            entity.HasIndex(e => new { e.IdPromocion, e.IdProducto }, "UQ_Promocion_Producto").IsUnique();

            entity.Property(e => e.IdPromocionProducto).HasColumnName("id_promocion_producto");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdPromocion).HasColumnName("id_promocion");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.PromocionesProductos)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_Promociones_Productos_Producto");

            entity.HasOne(d => d.IdPromocionNavigation).WithMany(p => p.PromocionesProductos)
                .HasForeignKey(d => d.IdPromocion)
                .HasConstraintName("FK_Promociones_Productos_Promocion");
        });

        modelBuilder.Entity<Reembolso>(entity =>
        {
            entity.HasKey(e => e.IdReembolso).HasName("PK__Reembols__51DA51368C3F6AF5");

            entity.HasIndex(e => e.IdDetalleOrden, "UQ__Reembols__D2FC3FD69FAFE1B7").IsUnique();

            entity.Property(e => e.IdReembolso).HasColumnName("id_reembolso");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_solicitud");
            entity.Property(e => e.IdDetalleOrden).HasColumnName("id_detalle_orden");
            entity.Property(e => e.IdEstadoReembolso).HasColumnName("id_estado_reembolso");
            entity.Property(e => e.MontoReembolsado)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_reembolsado");
            entity.Property(e => e.Motivo)
                .IsUnicode(false)
                .HasColumnName("motivo");

            entity.HasOne(d => d.IdDetalleOrdenNavigation).WithOne(p => p.Reembolso)
                .HasForeignKey<Reembolso>(d => d.IdDetalleOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reembolsos_Detalle");

            entity.HasOne(d => d.IdEstadoReembolsoNavigation).WithMany(p => p.Reembolsos)
                .HasForeignKey(d => d.IdEstadoReembolso)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reembolsos_Estado");
        });

        modelBuilder.Entity<Reseña>(entity =>
        {
            entity.HasKey(e => e.IdReseña).HasName("PK__Reseña__06C98AE0D7A463D8");

            entity.ToTable("Reseña");

            entity.Property(e => e.IdReseña).HasColumnName("id_reseña");
            entity.Property(e => e.Comentario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("comentario");
            entity.Property(e => e.EsRecomendado).HasColumnName("es_recomendado");
            entity.Property(e => e.FechaPublicacion)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_publicacion");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reseña_Producto");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reseñas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reseña_Usuarios");
        });

        modelBuilder.Entity<TiposProducto>(entity =>
        {
            entity.HasKey(e => e.IdTipoProducto).HasName("PK__Tipos_Pr__F5E0BFB854D1FADE");

            entity.ToTable("Tipos_Producto");

            entity.HasIndex(e => e.Nombre, "UQ__Tipos_Pr__72AFBCC603F7C12D").IsUnique();

            entity.Property(e => e.IdTipoProducto).HasColumnName("id_tipo_producto");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__4E3E04ADEE2071E7");

            entity.HasIndex(e => e.Email, "UQ__Usuarios__AB6E616474D94010").IsUnique();

            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuarios__D4D22D747B3810BA").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("apellido");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Biografia)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("biografia");
            entity.Property(e => e.Email)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdEstadoCuenta).HasColumnName("id_estado_cuenta");
            entity.Property(e => e.IdPais).HasColumnName("id_pais");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre_usuario");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");

            entity.HasOne(d => d.IdEstadoCuentaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdEstadoCuenta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Estado_Cuenta");

            entity.HasOne(d => d.IdPaisNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPais)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Pais");
        });

        modelBuilder.Entity<VistaResumenCompra>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vista_resumen_compras");

            entity.Property(e => e.FechaCompra).HasColumnName("Fecha_Compra");
            entity.Property(e => e.ImporteCobrado)
                .HasColumnType("decimal(11, 2)")
                .HasColumnName("Importe_Cobrado");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("Metodo_Pago");
            entity.Property(e => e.NroOrden).HasColumnName("Nro_Orden");
            entity.Property(e => e.Usuario)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Videojuego)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
