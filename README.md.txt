Ejercicio t�cnico � Desarrollador Full-Stack

Objetivo
Implementar un sistema web para gesti�n de facturas, integrando datos desde un archivo JSON, almacen�ndolos en una base de datos y permitiendo su administraci�n mediante una interfaz intuitiva.

Requisitos Claves

Integraci�n desde JSON
* Carga facturas desde archivo JSON (bd_exam.json) en tiempo de ejecuci�n
* Validar que �invoice_number� sea �nico
* Validar la coherencia entre la suma de subtotales de los productos y el �total_amount�
* Calcular autom�ticamente el estado de las facturas:
o �Issued�: Sin notas de cr�dito
o �Cancelled�; Suma de montos NC igual al monto total de la factura
o �Partial�: Suma de montos NC menor al monto total de la factura
* Calcular autom�ticamente el estado de pago:
o �Pending�: Pago pendiente dentro del plazo
o �Overdue�: Si la fecha actual supera el �payment_due_date�
o �Paid�: Pago registrado

Funcionalidades Principales
* B�squeda de facturas
o Permite b�squeda por n�mero de factura y estados (estado de factura y estado de pago)
* Gesti�n Notas de Cr�dito (NC)
o Agregar NC a una factura con fecha de creaci�n autom�tica
o Validar que el monto de la NC no supere el saldo pendiente de la factura




Tecnolog�as Utilizadas

Backend 
* .NET 8 (ASP.NET Core)
* API RESTful
* Swagger para documentaci�n y prueba de los endpoints
* C�digos HTTP adecuados para respuestas
* CORS habilitado para permitir el acceso desde el frontend

Patr�n Aplicado
Patr�n de Capas. Patr�n de Inyecci�n de Dependencias, separaci�n por capas: Controllers (exposici�n de la API), Services (l�gica de negocio), y Data Access (contexto de base de datos con Entity Framework Core)

Base de Datos
* SQLite como motor de base de datos
* Entity Framework Core como ORM
* Script de creaci�n de la base generado mediante �dotnet ef migrations� y �dotnet ef database update�. No es necesario script manual debido a la migraci�n autom�tica.

Frontend
* React 19
* React Router DOM 7
* Axios para consumo de la API
* Validaciones de formularios con control de campos y mensaje al usuario






Instalaci�n y Ejecuci�n

Clonar Repositorio
https://github.com/ShowUserMatias/invoice-management.git

Backend (.NET 8)
1. Ir a carpeta �backend/InvoiceApi�
2. Verificar que el archivo appsettings.json tenga la siguiente configuraci�n
a. "ConnectionStrings": {  "DefaultConnection": "Data Source=invoice.db"}
3. Ejecutar 
a. dotnet restore
b. dotnet ef database update
c. dotnet run
4. Acceder a Swagger con una direcci�n como esta
a. https://localhost:{puerto}/swagger/index.html (Reemplazar el puerto por el indicado)

Frontend
1. Ir a carpeta frontend/invoice-frontend
2. Instalar dependencias con:
a.  npm install
3. Iniciar aplicaci�n con
a.  npm start
4. Acceder a localhost

Estructura del Repositorio
Invoice-management/
backend/
	invoiceApi/ (c�digo fuente .NET + SQLite)
frontend/
	invoice-frontend/ (React + Axios + Router)
README.md
