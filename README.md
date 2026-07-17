# 🚗 Sistema de Gestión de Citas y Mantenimiento Vehicular

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_MVC-0072C6?style=for-the-badge&logo=microsoft&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Azure](https://img.shields.io/badge/Azure_PaaS-0089D6?style=for-the-badge&logo=microsoft-azure&logoColor=white)

## 📌 Descripción del Proyecto

Este proyecto es un sistema web transaccional desarrollado como parte de un proyecto de tesis universitaria en Ingeniería de Sistemas. Su objetivo principal es digitalizar y optimizar el flujo de trabajo de un taller mecánico, gestionando desde el registro de clientes y vehículos, hasta la asignación de citas, control de mantenimientos y reportería.

El desarrollo está rigurosamente fundamentado en la metodología **Spec-Driven Development (SDD)** dentro del ciclo de vida del **Proceso Unificado (UP)**, garantizando el cumplimiento de los estándares internacionales de calidad y pruebas **ISO/IEC 25010** e **ISO/IEC/IEEE 29119**.

## 🏗️ Arquitectura y Patrones de Diseño

El sistema está construido bajo una arquitectura monolítica en capas, implementando principios de diseño robustos para garantizar la mantenibilidad y atomicidad de las operaciones:

*   **Patrón Arquitectónico:** Modelo-Vista-Controlador (MVC).
*   **Acceso a Datos:** Patrón *Repository* y *Unit of Work* para garantizar transacciones atómicas.
*   **Mapeo Objeto-Relacional (ORM):** Entity Framework Core.
*   **Inyección de Dependencias (DI):** Nativa de ASP.NET Core para el acoplamiento débil entre la BLL (Capa de Negocio) y el DAL (Capa de Datos).

## 🚀 Módulos y Funcionalidades Principales

1.  **Seguridad y Accesos (Identity):**
    *   Autenticación de usuarios (Administradores y Mecánicos).
    *   Autorización basada en roles.
2.  **Gestión de Operaciones (Administrativo):**
    *   Registro transaccional de Clientes y Vehículos (Validación de placas únicas).
    *   Programación de citas con prevención de cruce de horarios (Reglas SDD).
3.  **Área Técnica (Mecánicos):**
    *   Bandeja de citas asignadas por mecánico.
    *   Registro detallado de diagnósticos técnicos, servicios aplicados y repuestos.
4.  **Reportería:**
    *   Consulta cronológica del historial de mantenimientos por vehículo.
    *   Exportación de reportes de atención.

## 🛠️ Requisitos Previos (Instalación Local)

Para ejecutar este proyecto en un entorno local de desarrollo, necesitas:

*   [.NET 9 SDK](https://dotnet.microsoft.com/download) instalado.
*   [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express o Developer Edition) o SQL Server Management Studio (SSMS).
*   Visual Studio 2022 o Visual Studio Code.

## ⚙️ Configuración y Despliegue

1.  **Clonar el repositorio:**
    ```bash
    git clone (https://github.com/Mapache-coder/TallerMecanicoFinal)
    ```
2.  **Configurar la Base de Datos:**
    Abre el archivo `appsettings.json` en la capa de Presentación y modifica la cadena de conexión (`DefaultConnection`) para que apunte a tu instancia local de SQL Server. *Nota: Asegúrate de que la ruta del servidor base esté correctamente configurada según tu entorno.*
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=TU_SERVIDOR;Database=TallerDB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"
    }
    ```
3.  **Aplicar Migraciones (Entity Framework Core):**
    Abre la Consola del Administrador de Paquetes (PMC) en Visual Studio o usa el CLI de .NET en la terminal para crear la base de datos:
    ```bash
    dotnet ef database update
    ```
4.  **Ejecutar el proyecto:**
    Presiona `F5` en Visual Studio o ejecuta:
    ```bash
    dotnet run
    ```

## ☁️ Entorno de Producción

El sistema está diseñado para ser desplegado en el ecosistema **Microsoft Azure (PaaS)**, utilizando servicios como *Azure App Service* para el alojamiento web y *Azure SQL Database* para la persistencia de datos relacionales.

## 📄 Licencia y Contexto Académico

Este repositorio contiene código generado para fines académicos y de investigación. Se permite su revisión como referencia arquitectónica de implementaciones en .NET 9 con metodología SDD.
