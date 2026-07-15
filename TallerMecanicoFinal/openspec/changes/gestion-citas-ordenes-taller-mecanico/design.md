## Context

El proyecto ya es una aplicacion ASP.NET Core MVC sobre .NET 9 con SQL Server como destino de persistencia. El cambio introduce un flujo operativo completo para un taller mecanico: agenda de citas, toma de trabajo por mecanicos, historial por placa y reportes simples de desempeno. El alcance es estrictamente operativo y debe evitar cualquier funcionalidad de costos, caja, facturacion o inventarios.

## Goals / Non-Goals

**Goals:**
- Entregar una experiencia web moderna, limpia y 100% responsiva con Bootstrap 5.
- Mantener el flujo de trabajo en la misma pantalla mediante modales AJAX y actualizaciones parciales.
- Modelar citas y ordenes de trabajo con reglas de negocio estrictas y trazables.
- Permitir consultas rapidas para Administrador y Mecanico con vistas simples de lectura.
- Implementar la persistencia con Repository + Unit of Work sobre EF Core y SQL Server.

**Non-Goals:**
- No incluir costos, caja, facturacion ni inventarios.
- No construir una SPA completa ni una app movil nativa.
- No agregar procesos contables, pagos, compras o repuestos.
- No introducir analitica avanzada ni reportes exportables en esta fase.

## Decisions

### 1. MVC servidor-rendered con parciales AJAX en vez de SPA
Usaremos ASP.NET Core MVC con partial views y llamadas AJAX para formularios y cambios de estado.

Rationale:
- El proyecto ya sigue una arquitectura MVC.
- Bootstrap 5 y jQuery permiten modales AJAX sin agregar complejidad innecesaria.
- Las vistas parciales facilitan actualizar tarjetas, listas y estados sin recargar toda la pagina.

Alternatives considered:
- SPA con React o Blazor: mayor costo y no necesario para el alcance operativo.
- Recargas completas de pagina: mas simple, pero rompe la experiencia fluida pedida.

### 2. Una sola entidad operativa para cita y orden de trabajo
La cita agendada y su ejecucion se modelaran como un unico agregado operativo que evoluciona de Registrado a En Proceso, Completado o Entregado.

Rationale:
- Evita duplicar datos entre una cita y una orden separada.
- Simplifica el historial por placa y el reporte mensual.
- Permite validar una unica regla de unicidad por fecha y hora.

Alternatives considered:
- Tablas separadas de cita y orden: mas explicitas, pero agregan joins y sincronizacion extra.
- Convertir la cita en una orden nueva al iniciar el trabajo: mas flexible, pero complica trazabilidad y reportes.

### 3. Repository + Unit of Work como frontera de persistencia
Expondremos repositorios por agregado y una unidad de trabajo para coordinar cambios transaccionales.

Rationale:
- Es el patron solicitado.
- Permite aislar reglas de negocio y evitar acceso directo al DbContext desde controladores.
- Facilita aplicar validaciones de unicidad y cambios de estado en una sola transaccion.

Alternatives considered:
- DbContext directo en controladores o servicios: mas rapido, pero acopla la UI a la persistencia.
- CQRS completo: excesivo para este alcance.

### 4. Roles de seguridad: Administrador y Mecanico
Se implementara autorizacion basada en roles para separar la agenda, la operacion diaria y los reportes.

Rationale:
- Las reglas de negocio distinguen claramente lo que puede hacer cada perfil.
- Evita exponer acciones administrativas en pantallas de mecanico.
- Hace mas simples las validaciones de acceso en controladores y vistas.

Alternatives considered:
- Permitir acceso por navegacion solamente: insuficiente para las restricciones de negocio.
- Autorizacion personalizada por claims complejos: no aporta valor para este escenario.

### 5. Validacion doble para reglas criticas de agenda
Las reglas de fecha futura, hora en :00 y unicidad por dia/hora se validaran en la capa de aplicacion y tambien con restricciones de base de datos.

Rationale:
- La validacion en aplicacion ofrece mensajes claros al usuario.
- La restriccion en SQL Server protege contra condiciones de carrera.
- La normalizacion de minutos a :00 evita valores inconsistentes.

Alternatives considered:
- Solo validacion en UI: facil de evadir.
- Solo constraint en base de datos: mensajes pobres y peor experiencia de usuario.

### 6. Reportes y perfiles como consultas de lectura simples
Los reportes de historial, perfil y desempeno se implementaran como consultas agregadas optimizadas, no como procesos en segundo plano.

Rationale:
- El volumen esperado es bajo a medio y no justifica infraestructura adicional.
- Los calculos de semana actual y mes actual son directos y predecibles.
- Mantiene el sistema facil de mantener y depurar.

Alternatives considered:
- Materializar vistas o jobs programados: innecesario por ahora.
- Cache agresiva: no prioritaria para este alcance.

### 7. Unicidad operativa por mecanico y ventana de correccion
Un mecanico solo podra tener una orden En Proceso a la vez. El sistema bloqueara la toma de un nuevo auto mientras exista una orden activa sin completar. Tras marcar una orden como Completado, el mecanico conservara una ventana de edicion de un mes para realizar correcciones; vencido ese plazo, la edicion quedara bloqueada para ese mecanico. El Administrador mantendra la capacidad de eliminar cualquier registro en cualquier momento.

Rationale:
- La restriccion de una sola orden activa evita dispersion de trabajo y simplifica el control diario del taller.
- La ventana de correccion acota cambios posteriores sin impedir ajustes reales del servicio terminado.
- La eliminacion administrativa ilimitada conserva la autoridad operativa para depurar registros cuando sea necesario.

Alternatives considered:
- Permitir multiples ordenes En Proceso por mecanico: mas flexible, pero contradice el flujo operativo definido.
- Bloquear toda edicion despues de Completado: mas estricto, pero no contempla correcciones legitimas posteriores.
- Limitar la eliminacion del Administrador por tiempo: innecesario para el alcance solicitado.

### 8. Historial por placa basado en ordenes completas y citas programadas
La busqueda por placa mostrara unicamente ordenes Completadas y citas futuras Programadas. Las citas canceladas no se conservaran como estado historico visible porque el Administrador las eliminara directamente de la base de datos.

Rationale:
- Evita ruido historico con registros cancelados que no aportan valor operativo.
- Mantiene la vista de historial enfocada en trabajo realizado y trabajo aun pendiente.
- Simplifica la lectura del historial por placa para consulta rapida.

Alternatives considered:
- Mostrar citas canceladas como historial: contradice la regla operativa definitiva.
- Incluir todos los estados: ensucia la consulta de historial y complica la lectura rapida.

## Risks / Trade-offs

- [Risk] La unicidad de cita por fecha y hora puede fallar bajo concurrencia. -> [Mitigation] Constraint unico en SQL Server y validacion previa en servicio.
- [Risk] La interpretacion de semana y mes puede variar segun zona horaria. -> [Mitigation] Usar la zona horaria del negocio y calcular rangos de forma consistente en el servidor.
- [Risk] Los modales AJAX pueden volverse fragiles si hay demasiada logica en cliente. -> [Mitigation] Mantener la logica de negocio en servidor y usar el cliente solo para presentacion y apertura/cierre de modales.
- [Risk] Un agregado operativo unico puede crecer demasiado si se agregan mas modulos despues. -> [Mitigation] Mantener repositorios y servicios separados por responsabilidad aunque compartan la misma entidad base.

## Migration Plan

1. Crear las entidades, relaciones e indices en SQL Server mediante migracion EF Core.
2. Incorporar roles base de Administrador y Mecanico, junto con datos iniciales si aplica.
3. Implementar los repositorios, la unidad de trabajo y los servicios de aplicacion para citas, ordenes y reportes.
4. Sustituir las pantallas de lista por dashboards con Cards y mover los formularios a modales AJAX.
5. Validar el flujo completo en ambiente de prueba con citas duplicadas, cambios de estado y consultas de historial.
6. Si hay un problema de despliegue, revertir al build anterior y mantener la base de datos en el ultimo esquema compatible.

