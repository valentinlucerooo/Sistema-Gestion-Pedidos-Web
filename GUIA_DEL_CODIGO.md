# Guía del código de Tienda Norte

Esta guía describe cómo se conectan las dos interfaces —la tienda del cliente y el panel de administración— y dónde modificar su apariencia.

## Archivos del proyecto

| Archivo | Para qué sirve |
| --- | --- |
| [index.html](./index.html) | Estructura de la tienda: cuenta, catálogo, carrito y pedidos anteriores. |
| [app.js](./app.js) | Lógica de la tienda: acceso, carrito, envío del pedido e historial. |
| [styles.css](./styles.css) | Colores, tamaños, distribución y adaptación a pantallas de la tienda. |
| [admin.html](./admin.html) | Estructura del panel: productos, envíos y pedidos. |
| [admin.js](./admin.js) | Lógica del panel: tablas, formularios, validación y llamadas al backend. |
| [admin.css](./admin.css) | Apariencia del panel de administración. |

El HTML define qué elementos existen; JavaScript los busca por `id`, clase o atributo `data-*` para darles comportamiento. CSS usa las clases para darles apariencia. Por eso, un `id` o una clase compartidos entre esos archivos no se deben renombrar de forma aislada.

## Tienda del cliente

### Estructura: `index.html`

La página se divide en estas secciones:

1. **Encabezado:** enlaces al catálogo, al historial, a la cuenta y al carrito.
2. **Presentación:** título principal y acceso rápido al catálogo.
3. **Cuenta:** pestañas para iniciar sesión o registrarse, y sus campos.
4. **Catálogo:** espacio donde `app.js` crea una tarjeta para cada producto.
5. **Pedido:** lista del carrito, subtotales, costo de envío, total y formulario de prioridad y destino.
6. **Historial:** espacio donde se muestran las tarjetas de pedidos del usuario.
7. **Pie de página:** marca y texto de cierre.

Los `id` permiten que JavaScript encuentre un elemento concreto; por ejemplo, el formulario `formPedido` y el contenedor `productGrid`. Los nombres de clase, como `product-card`, son ganchos visuales que CSS utiliza para aplicar estilos.

### Comportamiento: `app.js`

El código se ejecuta cuando el navegador termina de cargar el documento (`DOMContentLoaded`).

1. **Datos de la tienda:** los arreglos `productos` y `destinos` contienen el catálogo y las zonas de envío. Cada producto tiene identificador, nombre, categoría, precio, ícono y color; cada destino tiene identificador, nombre y costo.
2. **Estado en memoria:** `carrito` es un `Map` que relaciona el identificador de un producto con su cantidad. `pedidosRealizados` guarda los pedidos que se mostrarán en el historial. `isSubmitting` evita modificar el carrito durante el envío.
3. **Elementos de página:** las llamadas `querySelector` guardan referencias a los elementos de `index.html`, para actualizarlos sin volver a buscarlos.
4. **Cuenta y sesión:** las pestañas cambian entre acceso y registro. El formulario envía los datos al backend. Una sesión válida se guarda en `localStorage` con la clave `usuarioLogueado`; el correo de administración lleva al panel.
5. **Catálogo y carrito:** el catálogo se crea a partir del arreglo de productos. Al agregar o cambiar unidades, se actualiza el `Map` y se vuelve a dibujar el carrito, su cantidad, el subtotal, el costo de envío y el total.
6. **Confirmación del pedido:** antes de enviar, se comprueba que el formulario sea válido, haya productos, exista destino y haya una sesión. Luego se arma el JSON y se envía al endpoint de creación. Si la respuesta es exitosa, se limpia el formulario y el carrito, y se vuelve a consultar el historial.
7. **Historial:** `cargarHistorialUsuario` solicita los pedidos del usuario. Normaliza los datos recibidos y `renderizarHistorial` crea una tarjeta con número, fecha, productos, destino, prioridad, estado y total. El identificador de carga evita que una respuesta antigua reemplace datos de otra sesión.

**Nota sobre el pedido:** el JSON de creación representa un solo producto y una cantidad, aunque el total local suma los productos del carrito. Si se permite comprar varios productos juntos, el contrato del backend debería aceptar una colección de artículos y el frontend debería enviar esa colección.

## Panel de administración

### Estructura: `admin.html`

El panel tiene tres tablas:

- **Productos:** descripción, categoría, precio y acciones para modificar o eliminar.
- **Envíos:** zona, precio y acciones para modificar o eliminar.
- **Pedidos:** número, cliente, destino, fecha, total, selector de estado y acción para archivar.

Los formularios de producto y envío están en elementos `<dialog>`. Se usan tanto para crear como para modificar: al editar, JavaScript carga los valores actuales antes de abrirlos.

### Comportamiento: `admin.js`

1. **Control de acceso:** lee `usuarioLogueado` y permite mostrar el panel solo a la cuenta administradora. Si no corresponde, vuelve a la página de acceso.
2. **Rutas y solicitudes:** `LIST_API` agrupa las rutas de productos, envíos y pedidos. `request` prepara encabezados, agrega el token si existe, interpreta la respuesta y muestra los errores del servidor.
3. **Datos y presentación:** `collections` conserva las listas cargadas. Las funciones `renderProducts`, `renderShipping` y `renderOrders` construyen las filas de cada tabla.
4. **Formularios:** `openProductDialog` y `openShippingDialog` limpian los formularios o los precargan con los datos del registro. Al guardar, se validan los campos y se selecciona `POST` para crear o `PUT` para modificar.
5. **Acciones de tablas:** los eventos de clic eliminan registros con `DELETE` o abren el formulario. El selector de pedidos usa `PATCH` para cambiar el estado; archivar también actualiza ese estado.
6. **Bloqueo temporal:** `withBusy` impide enviar varias acciones a la vez y desactiva el botón mientras se procesa la solicitud.
7. **Carga inicial:** al final del archivo se consultan productos, envíos y pedidos; cada respuesta llena su colección y vuelve a dibujar la tabla.

Las rutas principales que usa el panel son:

| Acción | Ruta |
| --- | --- |
| Productos | `GET/POST api/producto`, `PUT/DELETE api/producto/{id}` |
| Envíos | `GET/POST api/enviocontrol`, `PUT/DELETE api/enviocontrol/{id}` |
| Listar pedidos | `GET api/pedido/pedidocontrol` |
| Cambiar estado | `PATCH api/pedido/actualizarEstado/{id}` |

El inicio y registro de cuenta se realizan desde `app.js`; el historial se consulta con el identificador de usuario. Los nombres de las propiedades del JSON deben seguir coincidiendo con el backend.

## Cómo cambiar la estética

1. **Colores globales:** en la parte superior de `styles.css` y `admin.css`, modifica variables como `--page` (fondo), `--surface` (tarjetas), `--ink` (texto), `--green` (color principal) y `--line` (bordes).
2. **Tienda:** busca en `styles.css` el selector de la parte que quieras cambiar. Ejemplos:
   - `.hero`: presentación principal.
   - `.auth-section`: acceso y registro.
   - `.product-card`: tarjeta de producto.
   - `.order-card`: panel del carrito.
   - `.history-card`: tarjeta del historial.
3. **Administración:** en `admin.css`, por ejemplo, `.panel` corresponde a cada sección, `.table-wrap` a la tabla y `.edit-dialog` al formulario emergente.
4. **Textos:** los textos fijos están en los HTML; los botones y tarjetas que se generan al vuelo están en `app.js` o `admin.js`.
5. **Diseño adaptable:** las reglas `@media` de cada CSS ajustan la interfaz para pantallas más angostas. Si cambias una regla visual, busca también otras apariciones del mismo selector: las reglas posteriores pueden reemplazar las anteriores.

`styles.css` tiene más de un bloque `:root`; los valores repetidos en un bloque posterior prevalecen sobre los anteriores. Para cambiar un color, comprueba el valor efectivo en el último bloque que lo define.

## Personalización visual con nombres en español

En `index.html` y `admin.html` agregué clases descriptivas en español para que ubiques las partes de la pantalla. Las clases originales siguen en el mismo elemento como compatibilidad; no se modificó ningún archivo JavaScript.

### Tienda: `index.html` y `styles.css`

| Clase en español | Parte visual |
| --- | --- |
| `.encabezado-tienda` | Barra superior de la tienda. |
| `.portada-principal` | Bloque grande de presentación inicial. |
| `.seccion-acceso` / `.tarjeta-acceso` | Acceso y registro de cuenta. |
| `.seccion-catalogo` / `.grilla-catalogo` | Área y grilla de productos. |
| `.seccion-compra` / `.panel-carrito` | Sección del pedido y panel del carrito. |
| `.lista-articulos-carrito` / `.resumen-importes` | Artículos y resumen de importes. |
| `.seccion-historial` / `.lista-historial-pedidos` | Historial del cliente y su lista de tarjetas. |
| `.pie-tienda` | Pie de página. |

Las tarjetas de producto y de historial se crean dinámicamente desde JavaScript. Como pediste no tocarlo, sus clases internas existentes se conservan; puedes dar estilo a las tarjetas desde el contenedor en español. Por ejemplo:

```css
.grilla-catalogo > article {
  border-radius: 12px;
}

.lista-historial-pedidos > article {
  border: 2px solid #d5ded2;
}
```

En la parte final de `styles.css` también puedes ajustar las variables `--color-fondo-pagina`, `--color-fondo-tarjetas`, `--color-texto-principal`, `--color-principal`, `--espacio-entre-tarjetas` y `--radio-tarjeta-producto`.

### Administración: `admin.html` y `admin.css`

| Clase en español | Parte visual |
| --- | --- |
| `.encabezado-administracion` | Barra superior del panel. |
| `.contenido-administracion` | Área principal del administrador. |
| `.panel-productos`, `.panel-envios`, `.panel-pedidos` | Paneles separados para cada tipo de información. |
| `.contenedor-tabla-productos`, `.contenedor-tabla-envios`, `.contenedor-tabla-pedidos` | Contenedores desplazables de las tablas. |
| `.campo-busqueda-pedidos` | Campo para filtrar pedidos. |
| `.dialogo-producto`, `.dialogo-envio` | Ventanas emergentes de edición y creación. |
| `.campo-descripcion-producto`, `.campo-categoria-producto`, `.campo-precio-producto` | Campos del formulario de producto. |
| `.campo-zona-envio`, `.campo-precio-envio` | Campos del formulario de envío. |

En la parte superior de `admin.css` puedes modificar variables como `--color-fondo-pagina`, `--color-principal`, `--color-error`, `--radio-panel-administracion` y `--espacio-interior-panel`.

### Para qué sirven las etiquetas HTML

Las etiquetas no se traducen: son elementos estándar del navegador. Se usan por su significado:

- `<header>`: encabezado o barra superior.
- `<nav>`: grupo de enlaces de navegación.
- `<main>`: contenido principal de la página.
- `<section>`: sección temática, como catálogo, pedido o historial.
- `<form>`, `<label>` e `<input>`: formulario y campos accesibles.
- `<table>`, `<thead>`, `<tbody>`, `<tr>` y `<th>`: tabla de administración.
- `<dialog>`: ventana emergente para editar o agregar un registro.
- `<footer>`: pie de página.

Para cambiar su apariencia, no hace falta reemplazar esas etiquetas: usa las clases visuales en CSS. Conservé los `id` y atributos `data-*` porque los necesita el JavaScript para encontrar formularios, botones y tablas.

### Traducción de los selectores más usados

Estos nombres no aparecen en pantalla; son referencias para conectar el HTML con el CSS y JavaScript:

| Selector | Significado |
| --- | --- |
| `.site-header` / `.site-footer` | Encabezado y pie de la tienda. |
| `.hero` | Portada principal de la tienda. |
| `.auth-section` / `.auth-card` | Sección y tarjeta de acceso o registro. |
| `.product-grid` / `.product-card` | Grilla y tarjeta de cada producto. |
| `.order-section` / `.order-card` | Sección del pedido y tarjeta del carrito. |
| `.cart-row` / `.cart-total` | Fila de producto y total del carrito. |
| `.history-section` / `.history-card` | Sección y tarjeta del historial. |
| `.admin-header` / `.admin-main` | Encabezado y contenido del panel. |
| `.panel` / `.table-wrap` | Sección y contenedor de tabla del panel. |
| `.edit-dialog` / `.form-field` | Ventana de edición y campo de formulario. |
| `.button--primary` / `.action-button` | Botón principal y botón de acción en una fila. |

## Nombres que conviene conservar

Aunque parte de los nombres técnicos están en inglés, muchos no se muestran en pantalla. Además, algunos forman parte de contratos con el navegador o el backend:

- No traduzcas las claves enviadas al servidor, por ejemplo `idUsuario`, `idProducto`, `idEnvio`, `precio`, `montoTotal` y `estado`.
- No cambies las rutas ni los métodos HTTP (`GET`, `POST`, `PUT`, `PATCH`, `DELETE`) sin cambiar también el controlador.
- No cambies solo un `id`, una clase o un atributo `data-*`: debe seguir coincidiendo entre HTML, JavaScript y CSS.
- Para modificar textos que ve el usuario, cambia el texto del HTML o la cadena que JavaScript asigna a `textContent` o a una plantilla.

Los nombres como `product-card` o `history-card` son identificadores internos de CSS; significan “tarjeta de producto” y “tarjeta del historial”. No son etiquetas visibles para el cliente.
