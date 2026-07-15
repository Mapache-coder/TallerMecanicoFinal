## 1. Foundation and persistence

- [ ] 1.1 Define the core domain entities and enums for appointments, mechanic assignment, and work order status.
- [ ] 1.2 Add the SQL Server schema, EF Core mappings, and the unique index that prevents duplicate date/time appointments.
- [ ] 1.3 Implement Repository + Unit of Work abstractions and the application services needed by the UI.
- [ ] 1.4 Seed the Administrator and Mechanic roles, plus any baseline users or lookup data required to test the workflow.

## 2. Appointment management

- [ ] 2.1 Build the admin appointment controller, view models, and validation pipeline for required fields and future dates.
- [ ] 2.2 Create the card-based appointment management screen with Bootstrap 5 and AJAX modal forms for create, edit, and delete.
- [ ] 2.3 Enforce the minute :00 normalization and duplicate date/time guard in the service layer with user-friendly errors.
- [ ] 2.4 Implement physical deletion for canceled appointments so they do not remain available in historical queries.

## 3. Workshop workflow

- [ ] 3.1 Implement the "Autos del Dia" screen filtered to current-day records in Registrado status.
- [ ] 3.2 Add the mechanic action to take a car, assign the authenticated mechanic, transition the order to En Proceso, and block a second active order.
- [ ] 3.3 Add the modal flow for changing status to Completado or Entregado and capture diagnosis and solution when completed.
- [ ] 3.4 Enforce the one-month correction window after Completado for mechanic edits and allow Administrator deletion at any time.

## 4. History, profile, and reporting

- [ ] 4.1 Build the plate search flow that returns only Completadas orders and future Programadas appointments for the Administrator.
- [ ] 4.2 Implement the mechanic profile screen with personal data and the weekly completed-order counter.
- [ ] 4.3 Implement the monthly performance report that aggregates completed orders by mechanic.
- [ ] 4.4 Add focused automated tests or validation checks for the core business rules, including single active order, deletion rules, and report calculations.