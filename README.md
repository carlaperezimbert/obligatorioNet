# Obligatorio .NET 2026 - Sistema de Gestión

Proyecto académico desarrollado en .NET bajo una arquitectura en capas.

## 📁 Estructura de la Solución
* **CapaDominio:** Entidades del negocio (`Cliente`, `Proyecto`, `FacturaVenta`, `FacturaCompra`, `LiquidacionImpuestos`) y enumerados (`TipoProyecto`, `EstadoProyecto`).
* **CapaDatos:** Acceso a datos con ADO.NET y SQL Server (Repositorios).
* **CapaLogica:** Reglas de negocio, validaciones y cálculos impositivos (IVA / IRAE).
* **AppConsola / AppEscritorio:** Interfaces de usuario.

## 🚀 Cómo levantar el proyecto por primera vez
1. **Clonar el repositorio:** Abrir Visual Studio -> *Clonar repositorio* -> Pegar la URL de este repositorio.
2. **Base de Datos:** Ejecutar el script SQL de creación de la base de datos (`EmpresaNetDB`) en SQL Server.
3. **Cadena de conexión:** Actualizar el nombre del servidor local (`Server=TU_SERVIDOR`) en los repositorios de la `CapaDatos`.
4. **Compilar y ejecutar:** Abrir `Obligatorio.sln`, asegurarse de que las dependencias entre capas estén activas y ejecutar.

## 🔄 Reglas rápidas de Git para el equipo
* **Antes de empezar a programar:** Ir a *Git > Extraer (Fetch)* o *Sincronizar (Sync)* para bajar los últimos cambios.
* **Al terminar una tarea:** Ir a *Git > Cambios de Git*, escribir un mensaje claro de lo que se hizo y darle a *Confirmar todo y enviar (Commit All and Push)*.
* **Coordinación:** Avisar por el grupo antes de modificar una clase existente para evitar conflictos de código.
