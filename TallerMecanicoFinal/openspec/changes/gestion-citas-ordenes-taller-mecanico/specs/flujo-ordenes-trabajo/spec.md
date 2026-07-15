## ADDED Requirements

### Requirement: Autos del Dia para mecanicos
El sistema SHALL mostrar en la pantalla Autos del Dia solo las citas u ordenes agendadas para la fecha actual en estado Registrado.

#### Scenario: Filtrado por fecha y estado
- **WHEN** un mecanico abre la pantalla Autos del Dia
- **THEN** el sistema MUST listar unicamente registros con fecha igual a la fecha actual
- **THEN** el sistema MUST excluir registros en cualquier estado distinto de Registrado

### Requirement: Asignacion de orden al mecanico
El sistema SHALL permitir que un mecanico seleccione un auto para trabajar y, al hacerlo, SHALL asociar al mecanico a la orden y cambiar el estado a En Proceso.

#### Scenario: Toma de trabajo
- **WHEN** el mecanico selecciona un auto disponible para trabajar
- **THEN** el sistema MUST asociar esa orden al mecanico autenticado
- **THEN** el sistema MUST cambiar el estado de la orden a En Proceso

### Requirement: Cambio de estado de la orden activa
El sistema SHALL permitir que el mecanico cambie el estado de su orden activa a Completado o Entregado.

#### Scenario: Cierre de una orden activa
- **WHEN** el mecanico tiene una orden activa
- **THEN** el sistema MUST permitir moverla a Completado o Entregado
- **THEN** el sistema MUST impedir cambios de estado a ordenes no asignadas al mecanico

### Requirement: Una sola orden En Proceso por mecanico
El sistema SHALL permitir que cada mecanico tenga solo una orden en estado En Proceso a la vez y SHALL bloquear la toma de un nuevo auto mientras exista una orden activa sin completar.

#### Scenario: Intento de tomar un segundo auto
- **WHEN** un mecanico ya tiene una orden en estado En Proceso
- **THEN** el sistema MUST bloquear la seleccion de otro auto
- **THEN** el sistema MUST informar que debe finalizar la orden actual antes de tomar otra

### Requirement: Registro de diagnostico y solucion
El sistema SHALL requerir texto libre de diagnostico y solucion aplicada cuando una orden se marque como Completado.

#### Scenario: Finalizacion del trabajo
- **WHEN** el mecanico marca una orden como Completado
- **THEN** el sistema MUST solicitar y guardar diagnostico y solucion en texto libre
- **THEN** el registro MUST quedar disponible para consulta historica posterior

### Requirement: Ventana de correccion posterior al completado
El sistema SHALL permitir que el mecanico edite el registro de una orden completada durante un plazo maximo de un mes despues del cambio a Completado y SHALL bloquear la edicion del mecanico una vez vencido ese plazo.

#### Scenario: Edicion dentro del plazo permitido
- **WHEN** una orden fue completada hace menos de un mes
- **THEN** el sistema MUST permitir al mecanico modificar su registro

#### Scenario: Edicion fuera del plazo permitido
- **WHEN** una orden fue completada hace mas de un mes
- **THEN** el sistema MUST bloquear la edicion para el mecanico

### Requirement: Eliminacion administrativa de cualquier registro
El sistema SHALL permitir al Administrador eliminar cualquier registro operativo en cualquier momento.

#### Scenario: Eliminacion por el Administrador
- **WHEN** el Administrador solicita eliminar una orden o cita
- **THEN** el sistema MUST permitir la eliminacion sin restriccion temporal
- **THEN** el registro MUST dejar de estar disponible para consultas posteriores
