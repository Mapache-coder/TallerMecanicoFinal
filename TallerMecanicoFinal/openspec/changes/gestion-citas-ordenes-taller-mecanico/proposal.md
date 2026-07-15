## Why

El taller necesita un sistema operativo único para centralizar citas y órdenes de trabajo, evitar conflictos de agenda y dar trazabilidad al flujo diario de los mecánicos. Ahora mismo, el proceso se beneficia de una interfaz moderna y rápida, pero sin mezclar costos, facturación ni inventario.

## What Changes

- Se introduce una interfaz moderna y 100% responsiva con Bootstrap 5, paleta morada clara y neutra, dashboards basados en Cards y formularios en modales AJAX.
- El Administrador podrá crear, modificar, eliminar y consultar citas con validaciones de negocio estrictas.
- El Mecánico tendrá una vista de "Autos del Día" para tomar trabajo, cambiar estados y registrar diagnóstico y solución aplicada.
- El Administrador podrá consultar el historial completo de un auto por placa y ver reportes simples de desempeño por mecánico.
- **BREAKING**: El sistema se limita a la operación de citas y órdenes de trabajo; no incluirá costos, caja, facturación ni inventarios.

## Capabilities

### New Capabilities
- `experiencia-ui`: navegación responsiva, cards para tableros, modales AJAX y lenguaje visual profesional para todas las pantallas.
- `agenda-citas`: alta, edición, eliminación y consulta de citas con reglas de agenda y validación de datos obligatorios.
- `flujo-ordenes-trabajo`: vista diaria de autos, asignación de mecánico, cambio de estados y registro de diagnóstico/solución.
- `perfiles-reportes`: historial por placa, perfil del mecánico y reporte mensual de órdenes completadas.

### Modified Capabilities
- Ninguna.

## Impact

- Vistas MVC, parciales y scripts AJAX para modales y actualizaciones sin recarga completa.
- Modelo de datos en SQL Server para citas, órdenes, mecánicos, vehículos y trazabilidad histórica.
- Servicios de dominio y persistencia con Repository + Unit of Work sobre EF Core.
- Autorización por rol para Administrador y Mecánico.
