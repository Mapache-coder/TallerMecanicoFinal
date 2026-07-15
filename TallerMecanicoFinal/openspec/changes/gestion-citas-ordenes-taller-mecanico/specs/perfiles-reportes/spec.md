## ADDED Requirements

### Requirement: Historial de autos por placa
El sistema SHALL permitir al Administrador buscar cualquier auto por su placa y ver un historial compuesto unicamente por ordenes Completadas y citas futuras Programadas.

#### Scenario: Consulta de historial
- **WHEN** el Administrador ingresa una placa existente
- **THEN** el sistema MUST mostrar solo las ordenes Completadas asociadas a esa placa
- **THEN** el sistema MUST mostrar solo las citas futuras Programadas asociadas a esa placa
- **THEN** el sistema MUST excluir citas canceladas o registros eliminados

### Requirement: Perfil del mecanico
El sistema SHALL mostrar los datos del mecanico y un contador simple de cuantas ordenes ha completado en la semana actual.

#### Scenario: Consulta de perfil
- **WHEN** el mecanico abre su perfil
- **THEN** el sistema MUST mostrar sus datos basicos
- **THEN** el sistema MUST mostrar un contador de ordenes completadas durante la semana actual

### Requirement: Reporte de desempeno mensual
El sistema SHALL mostrar al Administrador un reporte estadistico sencillo con el total de ordenes completadas por cada mecanico en el mes actual.

#### Scenario: Visualizacion del reporte mensual
- **WHEN** el Administrador abre el reporte de desempeno
- **THEN** el sistema MUST mostrar el total de ordenes completadas por mecanico dentro del mes actual
- **THEN** el reporte MUST ser suficiente para comparacion rapida entre mecanicos
