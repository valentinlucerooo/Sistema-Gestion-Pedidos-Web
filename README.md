# Sistema de Gestión de Pedidos - Frontend Web

Interfaz web para la gestión comercial de productos, tarifas de envíos y seguimiento de órdenes de compra en tiempo real. Desarrollada con tecnologías web estándar (HTML5, CSS3, JavaScript) consumiendo de forma asincrónica una API REST en .NET 8.

## Características Principales

### Panel de Administrador
* **Gestión de Productos (CRUD Completo):** Alta, edición, listado y eliminación de artículos del catálogo en tiempo real con validaciones de negocio.
* **Gestión de Envíos (CRUD Completo):** Alta, edición, listado y eliminación de zonas y tarifas de envío con validaciones de negocio.
* **Control de Pedidos:** Visualización global de todas las órdenes registradas y actualización del estado de entrega (`Pendiente`, `En camino`, `Entregado`, etc.).

### Panel de Cliente
* **Seguimiento de Historial de Compras:** Consultas personalizadas de los pedidos realizados por el usuario.
* **Seguimiento de Estado de Compra:** Visualización clara del estado actual de entrega (`Pendiente`, `En camino`, `Entregado`, etc.).

## Stack Tecnológico
* **Estructura y Estilo:** HTML5, CSS3
* **Lógica de Cliente:** JavaScript ES6+ (Vanilla JS).
* **Comunicación HTTP:** Consumo de endpoints RESTful mediante `Fetch API` (`async/await`) con manejo de respuestas y códigos de estado HTTP (`200`, `400`, etc.).

## Vistas de la Web

### Panel de Administrador


### Panel de Cliente


## Endpoints Consumidos
El frontend se comunica con la API mediante las siguientes rutas base:
* `GET / POST / PUT / DELETE` ➔ `/api/Producto` (Gestión de catálogo)
* `GET / POST / PUT / DELETE` ➔ `/api/Envio` (Zonas y tarifas)
* `GET / POST / PUT` ➔ `/api/Pedido` (Creación, listado y cambio de estado de órdenes)
