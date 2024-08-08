# Pico y Placa Quito

Este repositorio contiene una API desarrollada en .NET para gestionar consultas sobre la restricción de circulación de vehículos según la normativa de "Pico y Placa". La API almacena las consultas realizadas en una base de datos SQL.

## Estructura del Proyecto

- **Base de Datos**: SQL Server
- **Backend**: .NET 6.0 (o superior)

## Requisitos

- **SQL Server**
- **.NET 6.0 (o superior)**

## Configuración de la Base de Datos

Para configurar la base de datos, sigue estos pasos:

### 1. Crear la Base de Datos

Ejecuta el siguiente script en SQL Server Management Studio (SSMS) para crear la base de datos y las tablas necesarias:

```sql
CREATE DATABASE PicoYPlacaDB;

USE PicoYPlacaDB;

-- Creación de las tablas
CREATE TABLE Vehiculo (
    VehiculoID INT IDENTITY PRIMARY KEY,
    Placa VARCHAR(7) UNIQUE NOT NULL
);

CREATE TABLE RegistroConsultas (
    ConsultaID INT IDENTITY PRIMARY KEY,
    VehiculoID INT NOT NULL,
    Fecha DATE NOT NULL,
    Hora TIME(0) NOT NULL,
    PuedeCircular BIT NOT NULL,
    FOREIGN KEY (VehiculoID) REFERENCES Vehiculo(VehiculoID)
);
```

### 2. Crear el Procedimiento Almacenado

Ejecuta el siguiente script en SQL Server Management Studio (SSMS) para crear el procedimiento almacenado que se utiliza para registrar consultas:

```sql
CREATE PROCEDURE RegistrarConsulta
    @Placa VARCHAR(10),
    @Fecha DATE,
    @Hora TIME(0),
    @PuedeCircular BIT
AS
BEGIN
    -- Comenzar una transacción
    BEGIN TRANSACTION;

    -- Declarar una variable para el ID del vehículo
    DECLARE @VehiculoID INT;

    -- Verificar si el vehículo ya existe
    SELECT @VehiculoID = VehiculoID
    FROM Vehiculo
    WHERE Placa = @Placa;

    -- Si el vehículo no existe, insertarlo
    IF @VehiculoID IS NULL
    BEGIN
        INSERT INTO Vehiculo (Placa)
        VALUES (@Placa);

        -- Obtener el ID del vehículo recién insertado
        SET @VehiculoID = SCOPE_IDENTITY();
    END

    -- Insertar el registro de la consulta en la tabla correcta
    INSERT INTO RegistroConsultas (VehiculoID, Fecha, Hora, PuedeCircular)
    VALUES (@VehiculoID, @Fecha, @Hora, @PuedeCircular);

    -- Confirmar la transacción
    COMMIT TRANSACTION;
END;

```
### 3. Configuración de la Cadena de Conexión en .NET

1. Abre el archivo `appsettings.json`.

2. Agrega la cadena de conexión en la sección `ConnectionStrings` como se muestra a continuación:

```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=nombre_servidor\\nombre_instancia;Database=nombre_base_datos;Trusted_Connection=True;TrustServerCertificate=True"
  }
```
3. Reemplaza nombre_servidor\\nombre_instancia con el nombre de tu servidor SQL y nombre_base_datos con el nombre de tu base de datos.

### 4. Uso del API
Para utilizar la API de registro de consultas, realiza una solicitud `POST` al siguiente endpoint:
[http://localhost:5013/api/PicoPlaca](http://localhost:5013/api/PicoPlaca)
La solicitud debe incluir un cuerpo JSON con los siguientes datos:

- **placa**: La placa del vehículo (Ejemplo: "AND4455").
- **fecha**: La fecha en formato `yyyy-MM-dd` (Ejemplo: "2024-08-08").
- **hora**: La hora en formato `HH:mm:ss` (Ejemplo: "08:30:00").

#### Ejemplo de Solicitud

Cuerpo de la solicitud:

```json
{
  "placa": "AND4455",
  "fecha": "2024-08-08",
  "hora": "08:30:00"
}
```
#### Respuesta Esperada

La respuesta de la API indicará si el vehículo puede circular o no en el momento especificado. La respuesta incluirá los siguientes campos:

- **puedeCircular**: Un valor booleano (`true` o `false`) que indica si el vehículo puede circular.
- **mensaje**: Un mensaje que proporciona información adicional sobre el resultado.

##### Ejemplo de Respuesta

```json
{
  "puedeCircular": true,
  "mensaje": "El vehículo puede circular."
}
