## ADDED Requirements

### Requirement: Administrador gestiona citas
El sistema SHALL permitir al Administrador crear, ver, modificar y eliminar citas.

#### Scenario: CRUD de una cita
- **WHEN** el Administrador accede al modulo de citas
- **THEN** el sistema MUST permitir crear, editar, consultar y eliminar registros de cita

### Requirement: Datos obligatorios de la cita
El sistema SHALL exigir como obligatorios placa, marca, modelo, DNI del propietario, nombre del propietario, kilometraje, telefono del propietario, dia y hora.

#### Scenario: Validacion de campos obligatorios
- **WHEN** el usuario intenta guardar una cita con uno o mas campos vacios
- **THEN** el sistema MUST rechazar el guardado
- **THEN** el sistema MUST mostrar errores de validacion por cada dato faltante

### Requirement: Programacion futura y minuto fijo
El sistema SHALL permitir agendar citas solo para fechas iguales o posteriores a la fecha actual y SHALL almacenar la hora siempre con minutos en :00.

#### Scenario: Agendar para hoy o futuro
- **WHEN** el usuario selecciona una fecha anterior a la fecha actual
- **THEN** el sistema MUST rechazar la cita
- **WHEN** el usuario selecciona una hora con minutos distintos de :00
- **THEN** el sistema MUST normalizar o rechazar la hora para que quede almacenada con minutos :00

### Requirement: Unicidad de cita por fecha y hora
El sistema SHALL impedir dos citas en el mismo dia a la misma hora.

#### Scenario: Conflicto de agenda
- **WHEN** existe una cita ya registrada para una fecha y hora determinadas
- **THEN** el sistema MUST impedir registrar otra cita con la misma combinacion de fecha y hora
- **THEN** el sistema MUST informar al usuario del conflicto de agenda

### Requirement: Eliminacion fisica de citas canceladas
El sistema SHALL permitir al Administrador eliminar una cita en cualquier momento y SHALL eliminar fisicamente las citas canceladas de la base de datos en lugar de conservarlas como historial.

#### Scenario: Eliminacion de una cita cancelada
- **WHEN** el Administrador elimina una cita cancelada
- **THEN** el sistema MUST remover el registro de la base de datos
- **THEN** el sistema MUST evitar que esa cita aparezca en consultas historicas posteriores
