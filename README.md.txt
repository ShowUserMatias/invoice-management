Ejercicio técnico – Desarrollador Full-Stack

Objetivo
Implementar un sistema web para gestión de facturas, integrando datos desde un archivo JSON, almacenándolos en una base de datos y permitiendo su administración mediante una interfaz intuitiva.

Requisitos Claves

Integración desde JSON
* Carga facturas desde archivo JSON (bd_exam.json) en tiempo de ejecución
* Validar que ‘invoice_number’ sea único
* Validar la coherencia entre la suma de subtotales de los productos y el ‘total_amount’
* Calcular automáticamente el estado de las facturas:
o ‘Issued’: Sin notas de crédito
o ‘Cancelled’; Suma de montos NC igual al monto total de la factura
o ‘Partial’: Suma de montos NC menor al monto total de la factura
* Calcular automáticamente el estado de pago:
o ‘Pending’: Pago pendiente dentro del plazo
o ‘Overdue’: Si la fecha actual supera el ‘payment_due_date’
o ‘Paid’: Pago registrado

Funcionalidades Principales
* Búsqueda de facturas
o Permite búsqueda por número de factura y estados (estado de factura y estado de pago)
* Gestión Notas de Crédito (NC)
o Agregar NC a una factura con fecha de creación automática
o Validar que el monto de la NC no supere el saldo pendiente de la factura




Tecnologías Utilizadas

Backend 
* .NET 8 (ASP.NET Core)
* API RESTful
* Swagger para documentación y prueba de los endpoints
* Códigos HTTP adecuados para respuestas
* CORS habilitado para permitir el acceso desde el frontend

Patrón Aplicado
Patrón de Capas. Patrón de Inyección de Dependencias, separación por capas: Controllers (exposición de la API), Services (lógica de negocio), y Data Access (contexto de base de datos con Entity Framework Core)

Base de Datos
* SQLite como motor de base de datos
* Entity Framework Core como ORM
* Script de creación de la base generado mediante ‘dotnet ef migrations’ y ‘dotnet ef database update’. No es necesario script manual debido a la migración automática.

Frontend
* React 19
* React Router DOM 7
* Axios para consumo de la API
* Validaciones de formularios con control de campos y mensaje al usuario






Instalación y Ejecución

Clonar Repositorio
https://github.com/ShowUserMatias/invoice-management.git

Backend (.NET 8)
1. Ir a carpeta ‘backend/InvoiceApi’
2. Verificar que el archivo appsettings.json tenga la siguiente configuración
a. "ConnectionStrings": {  "DefaultConnection": "Data Source=invoice.db"}
3. Ejecutar 
a. dotnet restore
b. dotnet ef database update
c. dotnet run
4. Acceder a Swagger con una dirección como esta
a. https://localhost:{puerto}/swagger/index.html (Reemplazar el puerto por el indicado)

Frontend
1. Ir a carpeta frontend/invoice-frontend
2. Instalar dependencias con:
a.  npm install
3. Iniciar aplicación con
a.  npm start
4. Acceder a localhost

Estructura del Repositorio
Invoice-management/
backend/
	invoiceApi/ (código fuente .NET + SQLite)
frontend/
	invoice-frontend/ (React + Axios + Router)
README.md
