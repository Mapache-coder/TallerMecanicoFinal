## ADDED Requirements

### Requirement: Interfaz responsiva y visual consistente
El sistema SHALL mostrar todas las pantallas con Bootstrap 5, ser 100% responsivo y usar una paleta de tonos morados claros y neutros con tarjetas y sombras suaves para los resumenes.

#### Scenario: Visualizacion en escritorio y movil
- **WHEN** un usuario abre cualquier pantalla de gestion en escritorio o movil
- **THEN** el layout MUST adaptarse sin desbordes horizontales
- **THEN** los resumenes y paneles principales MUST renderizarse como Cards, no como tablas de dashboard
- **THEN** la identidad visual MUST mantener tonos morados claros y neutros

### Requirement: Formularios en modales AJAX
El sistema SHALL abrir los formularios de alta, edicion y cambio de estado en modales AJAX y actualizar la vista afectada sin recarga completa de la pagina.

#### Scenario: Guardado de un formulario modal
- **WHEN** el usuario confirma un formulario dentro de un modal AJAX
- **THEN** el sistema MUST persistir los cambios
- **THEN** la vista de origen MUST refrescar el contenido afectado sin recargar toda la pagina
- **THEN** el modal MUST cerrarse o mostrar el resultado de validacion segun corresponda
