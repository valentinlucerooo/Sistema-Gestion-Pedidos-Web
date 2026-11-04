document.addEventListener("DOMContentLoaded", () => {
  const productos = [
    {
      id: 4,
      nombre: "Pava Eléctrica",
      categoria: "ELECTRODOMÉSTICO",
      precio: 15000,
      icono: "🫖",
      color: "sage"
    },
    {
      id: 5,
      nombre: "Motorola G9 plus",
      categoria: "CELULAR",
      precio: 200000,
      icono: "📱",
      color: "peach"
    },
    {
      id: 6,
      nombre: "Notebook Lenovo IdeaPad",
      categoria: "ELECTRÓNICA",
      precio: 1034808,
      icono: "💻",
      color: "lavender"
    },
    {
      id: 7,
      nombre: "Mouse Logitech M170",
      categoria: "ACCESORIO",
      precio: 206960,
      icono: "🖱️",
      color: "blue"
    },
    {
      id: 11,
      nombre: "Pava electrica Philips HD4695/90",
      categoria: "Electrodomestico",
      precio: 55000,
      icono: "🫖",
      color: "sage"
    },
    {
      id: 12,
      nombre: "freidora de aire Philips NA231/00",
      categoria: "Electrodomestico",
      precio: 200000,
      icono: "🍟",
      color: "peach"
    },
    {
      id: 13,
      nombre: "MicroondasBGH Digital B120DS20",
      categoria: "Electrodomestico",
      precio: 215000,
      icono: "🍽️",
      color: "lavender"
    },
    {
      id: 14,
      nombre: "licuadora lliana chopblend AL453",
      categoria: "Electrodomestico",
      precio: 68000,
      icono: "🥤",
      color: "blue"
    },
    {
      id: 24,
      nombre: "televisor samsung crystal UHD 43\" 4K",
      categoria: "Tecnologia",
      precio: 250000,
      icono: "📺",
      color: "lavender"
    },
    {
      id: 25,
      nombre: "JBL Flip 6",
      categoria: "Tecnologia",
      precio: 230000,
      icono: "🔊",
      color: "sage"
    },
    {
      id: 26,
      nombre: "Xiaomi smart Band mi Band 8",
      categoria: "Tecnologia",
      precio: 180000,
      icono: "⌚",
      color: "peach"
    },
    {
      id: 27,
      nombre: "plancha de pelo Gama Bella toumaline",
      categoria: "Tecnologia",
      precio: 110000,
      icono: "💇",
      color: "blue"
    }
  ];
  const destinos = [
    { id: 1016, nombre: "CABA", costo: 10000 },
    { id: 1017, nombre: "CAPITAL FEDERAL", costo: 8000 },
    { id: 1018, nombre: "INTERIOR", costo: 15000 },
    { id: 1019, nombre: "NOA", costo: 20000 },
    { id: 1020, nombre: "NEA", costo: 18000 },
    { id: 1021, nombre: "SUR ARGENTINO", costo: 30000 },
    { id: 1022, nombre: "INTERNACIONAL", costo: 50000 }
  ];
  const carrito = new Map();
  const pedidosRealizados = [];
  let isSubmitting = false;
  const formatoPrecio = new Intl.NumberFormat("es-AR", {
    style: "currency",
    currency: "ARS",
    maximumFractionDigits: 0
  });

  const grillaProductos = document.querySelector("#productGrid");
  const listaCarrito = document.querySelector("#cartItems");
  const cantidadCarrito = document.querySelector("#cartCount");
  const subtotalProductos = document.querySelector("#productsSubtotal");
  const costoEnvio = document.querySelector("#shippingCost");
  const totalCarrito = document.querySelector("#cartTotal");
  const formulario = document.querySelector("#formPedido");
  const seleccionDestino = document.querySelector("#destino");
  const botonEnviar = document.querySelector("#submitOrder");
  const mensajePantalla = document.querySelector("#mensajeRespuesta");
  const listaPedidos = document.querySelector("#orderHistory");
  const estadoHistorial = document.querySelector("#historyStatus");
  const encabezado = document.querySelector(".site-header");
  const enlaceCuenta = document.querySelector("#accountLink");
  const botonCerrarSesion = document.querySelector("#logoutButton");
  const formularioAuth = document.querySelector("#authForm");
  const botonEnviarAuth = document.querySelector("#authSubmit");
  const mensajeAuth = document.querySelector("#authResponse");
  const pestanasAuth = Array.from(document.querySelectorAll("[data-auth-mode]"));
  const camposRegistro = Array.from(document.querySelectorAll("[data-register-only]"));
  const inputNombre = document.querySelector("#authNombre");
  const inputApellido = document.querySelector("#authApellido");
  const inputEmail = document.querySelector("#authEmail");
  const inputContrasena = document.querySelector("#authPassword");
  let modoAuth = "login";
  let isSubmittingAuth = false;
  let cargaHistorialActual = 0;

  function obtenerUsuarioGuardado() {
    try {
      const usuarioJson = localStorage.getItem("usuarioLogueado");
      return usuarioJson ? JSON.parse(usuarioJson) : null;
    } catch (error) {
      console.error("No se pudo leer el usuario guardado:", error);
      return null;
    }
  }

  function actualizarNavegacionUsuario() {
    const usuario = obtenerUsuarioGuardado();
    const idUsuario = Number(usuario?.idUsuario);
    const haySesion = Number.isInteger(idUsuario) && idUsuario > 0;

    encabezado.classList.toggle("has-session", haySesion);
    enlaceCuenta.textContent = haySesion ? "Mi cuenta" : "Ingresar";
    botonCerrarSesion.hidden = !haySesion;
  }

  botonCerrarSesion.addEventListener("click", () => {
    try {
      localStorage.removeItem("usuarioLogueado");
    } catch (error) {
      console.error("No se pudo cerrar la sesión guardada:", error);
      mensajeAuth.textContent = "No se pudo cerrar la sesión. Intentá nuevamente.";
      mensajeAuth.classList.add("is-error");
      return;
    }

    cargaHistorialActual += 1;
    pedidosRealizados.splice(0);
    renderizarHistorial();
    estadoHistorial.textContent = "";
    estadoHistorial.classList.remove("is-error");
    actualizarNavegacionUsuario();
    mensajeAuth.textContent = "La sesión se cerró correctamente.";
    mensajeAuth.classList.remove("is-error");
    inputContrasena.value = "";
    document.querySelector("#cuenta").scrollIntoView({ behavior: "smooth" });
  });

  function cambiarModoAuth(modo) {
    if (isSubmittingAuth) return;

    modoAuth = modo;
    const esRegistro = modoAuth === "register";
    camposRegistro.forEach((campo) => {
      campo.hidden = !esRegistro;
      campo.querySelector("input").required = esRegistro;
    });
    pestanasAuth.forEach((pestana) => {
      const seleccionada = pestana.dataset.authMode === modoAuth;
      pestana.classList.toggle("is-active", seleccionada);
      pestana.setAttribute("aria-selected", String(seleccionada));
      pestana.tabIndex = seleccionada ? 0 : -1;
    });
    formularioAuth.setAttribute(
      "aria-labelledby",
      esRegistro ? "registerTab" : "loginTab"
    );
    inputContrasena.autocomplete = esRegistro ? "new-password" : "current-password";
    botonEnviarAuth.textContent = esRegistro
      ? "Crear cuenta →"
      : "Iniciar sesión →";
    mensajeAuth.textContent = "";
    mensajeAuth.classList.remove("is-error");
  }

  function obtenerMensajeAuth(datos, mensajeAlternativo) {
    if (typeof datos === "string" && datos) return datos;
    if (!datos || typeof datos !== "object") return mensajeAlternativo;

    const mensaje = datos.mensaje || datos.message || datos.title;
    if (typeof mensaje === "string" && mensaje) return mensaje;

    if (datos.errors && typeof datos.errors === "object") {
      const errores = Object.values(datos.errors)
        .flat()
        .filter((error) => typeof error === "string");
      if (errores.length > 0) return errores.join(" ");
    }

    return mensajeAlternativo;
  }

  cambiarModoAuth("login");

  pestanasAuth.forEach((pestana) => {
    pestana.addEventListener("click", () => {
      cambiarModoAuth(pestana.dataset.authMode);
    });
  });

  document.querySelector(".auth-tabs").addEventListener("keydown", (evento) => {
    if (evento.key !== "ArrowLeft" && evento.key !== "ArrowRight") return;
    evento.preventDefault();
    const siguienteModo = modoAuth === "login" ? "register" : "login";
    const siguientePestana = pestanasAuth.find(
      (pestana) => pestana.dataset.authMode === siguienteModo
    );
    cambiarModoAuth(siguienteModo);
    siguientePestana.focus();
  });

  formularioAuth.addEventListener("submit", async (evento) => {
    evento.preventDefault();
    mensajeAuth.textContent = "";
    mensajeAuth.classList.remove("is-error");

    if (!formularioAuth.checkValidity()) {
      formularioAuth.reportValidity();
      return;
    }

    const esRegistro = modoAuth === "register";
    const usuario = esRegistro
      ? {
          nombre: inputNombre.value.trim(),
          apellido: inputApellido.value.trim(),
          email: inputEmail.value.trim(),
          "contraseña": inputContrasena.value
        }
      : {
          email: inputEmail.value.trim(),
          "contraseña": inputContrasena.value
        };
    const ruta = esRegistro
      ? "https://localhost:7195/api/usuariocontrol/Registrar"
      : "https://localhost:7195/api/usuariocontrol/Validar";

    isSubmittingAuth = true;
    botonEnviarAuth.disabled = true;
    pestanasAuth.forEach((pestana) => {
      pestana.disabled = true;
    });
    botonEnviarAuth.textContent = "Enviando...";

    try {
      const respuesta = await fetch(ruta, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(usuario)
      });
      const textoRespuesta = await respuesta.text();
      let datosRecibidos = textoRespuesta;

      if (textoRespuesta) {
        try {
          datosRecibidos = JSON.parse(textoRespuesta);
        } catch {
          datosRecibidos = textoRespuesta;
        }
      }

      if (!respuesta.ok) {
        throw new Error(
          obtenerMensajeAuth(
            datosRecibidos,
            `El servidor respondió con el estado ${respuesta.status}.`
          )
        );
      }

      if (!esRegistro) {
        const usuarioRespuesta = datosRecibidos?.usuario || datosRecibidos?.data?.usuario || datosRecibidos;
        const idUsuario = Number(usuarioRespuesta?.idUsuario);

        if (Number.isInteger(idUsuario) && idUsuario > 0) {
          pedidosRealizados.splice(0);
          renderizarHistorial();
          localStorage.setItem(
            "usuarioLogueado",
            JSON.stringify({
              idUsuario,
              nombre: usuarioRespuesta.nombre,
              apellido: usuarioRespuesta.apellido,
              email: usuarioRespuesta.email || usuario.email
            })
          );
          actualizarNavegacionUsuario();
          await cargarHistorialUsuario();
        } else {
          localStorage.removeItem("usuarioLogueado");
          mensajeAuth.textContent =
            "Inicio validado, pero el servidor no devolvió idUsuario. No se puede cargar el historial.";
          return;
        }
      }

      mensajeAuth.textContent = obtenerMensajeAuth(
        datosRecibidos,
        esRegistro
          ? "Tu cuenta se registró correctamente."
          : "Inicio de sesión validado correctamente."
      );
      inputContrasena.value = "";
    } catch (error) {
      console.error("Error en la autenticación:", error);
      mensajeAuth.textContent =
        error instanceof TypeError
          ? "No se pudo conectar con el servidor. Revisá que esté disponible e intentá nuevamente."
          : error.message;
      mensajeAuth.classList.add("is-error");
    } finally {
      isSubmittingAuth = false;
      botonEnviarAuth.disabled = false;
      pestanasAuth.forEach((pestana) => {
        pestana.disabled = false;
      });
      botonEnviarAuth.textContent =
        modoAuth === "register" ? "Crear cuenta →" : "Iniciar sesión →";
    }
  });

  grillaProductos.innerHTML = productos
    .map(
      (producto) => `
        <article class="product-card">
          <div class="product-card__visual product-card__visual--${producto.color}">
            <span class="product-card__emoji" aria-hidden="true">${producto.icono}</span>
          </div>
          <div class="product-card__details">
            <p class="product-card__category">${producto.categoria}</p>
            <h3>${producto.nombre}</h3>
            <div class="product-card__bottom">
              <span class="product-card__price">${formatoPrecio.format(producto.precio)}</span>
              <button
                class="add-button"
                type="button"
                data-add-product="${producto.id}"
                aria-label="Agregar ${producto.nombre} al pedido"
              >
                <span aria-hidden="true">+</span> Agregar
              </button>
            </div>
          </div>
        </article>
      `
    )
    .join("");

  function obtenerDestinoSeleccionado() {
    return destinos.find((destino) => destino.id === Number(seleccionDestino.value));
  }

  function renderizarHistorial() {
    listaPedidos.replaceChildren();

    if (pedidosRealizados.length === 0) {
      const estadoVacio = document.createElement("p");
      estadoVacio.className = "history-empty";
      estadoVacio.textContent = "Todavía no tenés pedidos asociados a tu cuenta.";
      listaPedidos.append(estadoVacio);
      return;
    }

    pedidosRealizados.forEach((pedido, index) => {
      const tarjeta = document.createElement("article");
      tarjeta.className = "history-card";

      const encabezado = document.createElement("div");
      encabezado.className = "history-card__header";

      const titulo = document.createElement("h3");
      titulo.textContent = `Pedido #${pedido.id || index + 1}`;

      const fecha = document.createElement("time");
      const fechaPedido = pedido.fecha instanceof Date
        ? pedido.fecha
        : new Date(pedido.fecha);
      if (Number.isNaN(fechaPedido.getTime())) {
        fecha.textContent = "Fecha no disponible";
      } else {
        fecha.dateTime = fechaPedido.toISOString();
        fecha.textContent = new Intl.DateTimeFormat("es-AR", {
          dateStyle: "short",
          timeStyle: "short"
        }).format(fechaPedido);
      }

      encabezado.append(titulo, fecha);

      const productos = document.createElement("ul");
      productos.className = "history-card__products";
      pedido.productos.forEach((producto) => {
        const fila = document.createElement("li");
        const nombre = document.createElement("span");
        nombre.textContent = `${producto.nombre} × ${producto.cantidad}${
          producto.tipo ? ` · ${producto.tipo}` : ""
        }`;
        fila.append(nombre);
        if (Number.isFinite(producto.precio)) {
          const subtotal = document.createElement("strong");
          subtotal.textContent = formatoPrecio.format(
            producto.precio * producto.cantidad
          );
          fila.append(subtotal);
        }
        productos.append(fila);
      });

      const destino = document.createElement("p");
      destino.className = "history-card__detail";
      destino.textContent = `Destino: ${pedido.destino}`;

      const prioridad = document.createElement("p");
      prioridad.className = "history-card__priority";
      prioridad.textContent = `Prioridad: ${pedido.prioridad}`;

      const total = document.createElement("div");
      total.className = "history-card__total";
      const etiquetaTotal = document.createElement("span");
      etiquetaTotal.textContent = "Total del pedido";
      const montoTotal = document.createElement("strong");
      montoTotal.textContent = Number.isFinite(pedido.total)
        ? formatoPrecio.format(pedido.total)
        : "No disponible";
      total.append(etiquetaTotal, montoTotal);

      tarjeta.append(encabezado, productos, destino, prioridad, total);
      listaPedidos.append(tarjeta);
    });
  }

  async function cargarHistorialUsuario() {
    let usuarioGuardado;
    const cargaActual = ++cargaHistorialActual;

    try {
      const usuarioJson = localStorage.getItem("usuarioLogueado");
      usuarioGuardado = usuarioJson ? JSON.parse(usuarioJson) : null;
    } catch (error) {
      console.error("No se pudo leer el usuario guardado:", error);
      estadoHistorial.textContent = "No se pudo leer la sesión guardada.";
      estadoHistorial.classList.add("is-error");
      return;
    }

    const idUsuario = Number(usuarioGuardado?.idUsuario);
    if (!Number.isInteger(idUsuario) || idUsuario <= 0) {
      estadoHistorial.textContent = "";
      estadoHistorial.classList.remove("is-error");
      pedidosRealizados.splice(0);
      renderizarHistorial();
      return;
    }

    estadoHistorial.textContent = "Cargando tus pedidos...";
    estadoHistorial.classList.remove("is-error");

    try {
      const respuesta = await fetch(
        `https://localhost:7195/api/pedido/pedidocontrol/usuario/${idUsuario}`
      );
      if (cargaActual !== cargaHistorialActual) return;

      const textoRespuesta = await respuesta.text();
      let datosRecibidos = textoRespuesta;

      if (textoRespuesta) {
        try {
          datosRecibidos = JSON.parse(textoRespuesta);
        } catch {
          datosRecibidos = textoRespuesta;
        }
      }

      if (!respuesta.ok) {
        throw new Error(
          obtenerMensajeAuth(
            datosRecibidos,
            `El servidor respondió con el estado ${respuesta.status}.`
          )
        );
      }

      if (cargaActual !== cargaHistorialActual) return;

      const pedidos = Array.isArray(datosRecibidos)
        ? datosRecibidos
        : datosRecibidos?.pedidos || datosRecibidos?.data;
      if (
        !Array.isArray(pedidos) ||
        pedidos.some((pedido) => !pedido || typeof pedido !== "object")
      ) {
        throw new Error("El servidor devolvió un formato de historial no válido.");
      }

      pedidosRealizados.splice(
        0,
        pedidosRealizados.length,
        ...pedidos.map((pedido) => ({
          id: pedido.idpedido ?? pedido.idPedido,
          fecha: pedido.fechaRegistro,
          destino: pedido.envio?.zona || "No especificado",
          prioridad: pedido.prioridad || "No especificada",
          productos: [
            {
              nombre: pedido.producto?.descripcion || "Producto",
              tipo: pedido.producto?.tipoProducto || pedido.producto?.productoTipo || "",
              cantidad: Number(pedido.cantidad) || 0,
              precio: null
            }
          ],
          total: Number(pedido.montoTotal)
        }))
      );
      estadoHistorial.textContent = "";
      renderizarHistorial();
    } catch (error) {
      if (cargaActual !== cargaHistorialActual) return;
      console.error("Error al cargar el historial de pedidos:", error);
      estadoHistorial.textContent =
        error instanceof TypeError
          ? "No se pudo conectar con el servidor. Revisá que esté disponible e intentá nuevamente."
          : error.message;
      estadoHistorial.classList.add("is-error");
    }
  }

  function renderizarCarrito() {
    const productosEnCarrito = Array.from(carrito.entries());
    const cantidadTotal = productosEnCarrito.reduce(
      (total, [, cantidad]) => total + cantidad,
      0
    );
    const precioProductos = productosEnCarrito.reduce((total, [id, cantidad]) => {
      const producto = productos.find((item) => item.id === id);
      return total + producto.precio * cantidad;
    }, 0);
    const destino = obtenerDestinoSeleccionado();
    const precioEnvio = destino ? destino.costo : 0;

    cantidadCarrito.textContent = String(cantidadTotal);
    subtotalProductos.textContent = formatoPrecio.format(precioProductos);
    costoEnvio.textContent = destino ? formatoPrecio.format(precioEnvio) : "Seleccioná un destino";
    totalCarrito.textContent = formatoPrecio.format(
      precioProductos + (cantidadTotal > 0 ? precioEnvio : 0)
    );
    botonEnviar.disabled = productosEnCarrito.length === 0 || isSubmitting;
    grillaProductos.querySelectorAll("[data-add-product]").forEach((boton) => {
      boton.disabled = isSubmitting;
    });

    if (productosEnCarrito.length === 0) {
      listaCarrito.innerHTML = '<p class="cart-empty">Todavía no agregaste productos.</p>';
      return;
    }

    listaCarrito.innerHTML = productosEnCarrito
      .map(([id, cantidad]) => {
        const producto = productos.find((item) => item.id === id);
        return `
          <article class="cart-row">
            <span class="cart-row__icon" aria-hidden="true">${producto.icono}</span>
            <div>
              <p class="cart-row__name">${producto.nombre}</p>
              <p class="cart-row__unit-price">${formatoPrecio.format(producto.precio)} c/u</p>
            </div>
            <div class="cart-row__actions" aria-label="Cantidad de ${producto.nombre}">
              <button
                class="quantity-button"
                type="button"
                data-change-quantity="-1"
                data-product-id="${id}"
                aria-label="Quitar una unidad de ${producto.nombre}"
                ${isSubmitting ? "disabled" : ""}
              >−</button>
              <span class="cart-row__quantity">${cantidad}</span>
              <button
                class="quantity-button"
                type="button"
                data-change-quantity="1"
                data-product-id="${id}"
                aria-label="Agregar una unidad de ${producto.nombre}"
                ${isSubmitting ? "disabled" : ""}
              >+</button>
            </div>
            <p class="cart-row__subtotal">Subtotal: ${formatoPrecio.format(producto.precio * cantidad)}</p>
          </article>
        `;
      })
      .join("");
  }

  grillaProductos.addEventListener("click", (evento) => {
    const boton = evento.target.closest("[data-add-product]");
    if (!boton || isSubmitting) return;

    const id = Number(boton.dataset.addProduct);
    carrito.set(id, (carrito.get(id) || 0) + 1);
    mensajePantalla.textContent = "";
    mensajePantalla.classList.remove("is-error");
    renderizarCarrito();
  });

  listaCarrito.addEventListener("click", (evento) => {
    const boton = evento.target.closest("[data-change-quantity]");
    if (!boton || isSubmitting) return;

    const id = Number(boton.dataset.productId);
    const nuevaCantidad = (carrito.get(id) || 0) + Number(boton.dataset.changeQuantity);
    if (nuevaCantidad <= 0) {
      carrito.delete(id);
    } else {
      carrito.set(id, nuevaCantidad);
    }
    renderizarCarrito();
  });

  seleccionDestino.addEventListener("change", renderizarCarrito);

  formulario.addEventListener("submit", async (evento) => {
    evento.preventDefault();
    mensajePantalla.textContent = "";
    mensajePantalla.classList.remove("is-error");

    if (!formulario.checkValidity()) {
      formulario.reportValidity();
      return;
    }

    if (carrito.size === 0) {
      mensajePantalla.textContent = "Agregá al menos un producto antes de confirmar el pedido.";
      mensajePantalla.classList.add("is-error");
      return;
    }

    const destino = obtenerDestinoSeleccionado();
    if (!destino) {
      mensajePantalla.textContent = "Seleccioná un lugar de destino para continuar.";
      mensajePantalla.classList.add("is-error");
      return;
    }

    let usuarioGuardado;
    try {
      usuarioGuardado = JSON.parse(localStorage.getItem("usuarioLogueado"));
    } catch (error) {
      console.error("No se pudo leer el usuario guardado:", error);
    }

    const idUsuario = Number(usuarioGuardado?.idUsuario);
    if (!Number.isInteger(idUsuario) || idUsuario <= 0) {
      mensajePantalla.textContent = "Tenés que iniciar sesión antes de confirmar el pedido.";
      mensajePantalla.classList.add("is-error");
      return;
    }

    const productosPedido = Array.from(carrito.entries()).map(([id, cantidad]) => ({
      producto: { idProducto: id },
      cantidad
    }));
    const total = productosPedido.reduce((subtotal, item) => {
      const producto = productos.find((productoDisponible) =>
        productoDisponible.id === item.producto.idProducto
      );
      return subtotal + producto.precio * item.cantidad;
    }, destino.costo);
   // Tomamos el primer producto seleccionado del carrito
    const primerItem = productosPedido[0];

    // Buscamos el producto completo en tu lista local para sacar su nombre y categoría
    const productoOriginal = productos.find(p => p.id === Number(primerItem.producto.idProducto));

    const pedido = {
      usuario: {
        idUsuario
      },
      producto: { 
        idProducto: Number(primerItem.producto.idProducto),
        descripcion: productoOriginal ? productoOriginal.nombre : "Producto",
        productoTipo: productoOriginal ? productoOriginal.categoria : "GENERAL"
      },
      cantidad: Number(primerItem.cantidad),
      envio: { 
        idEnvio: Number(destino.id),
        zona: destino.nombre 
      },
      montoTotal: Number(total),
      prioridad: String(formulario.elements.prioridad.value.trim()),
      fechaRegistro: new Date().toISOString()
    };
    const pedidoParaHistorial = {
      fecha: new Date(),
      destino: destino.nombre,
      prioridad: pedido.prioridad,
      productos: Array.from(carrito.entries()).map(([id, cantidad]) => {
        const producto = productos.find((item) => item.id === id);
        return {
          nombre: producto.nombre,
          cantidad,
          precio: producto.precio
        };
      }),
      total
    };

    isSubmitting = true;
    renderizarCarrito();
    botonEnviar.textContent = "Enviando pedido...";

    try {
      const respuesta = await fetch("https://localhost:7195/api/pedido/PedidoControl/individual", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(pedido)
      });
      const textoRespuesta = await respuesta.text();
      let datosRecibidos = {};

      if (textoRespuesta) {
        try {
          const respuestaJson = JSON.parse(textoRespuesta);
          if (respuestaJson && typeof respuestaJson === "object") {
            datosRecibidos = respuestaJson;
          } else {
            datosRecibidos = { mensaje: String(respuestaJson) };
          }
        } catch {
          datosRecibidos = { mensaje: textoRespuesta };
        }
      }

      if (!respuesta.ok) {
        throw new Error(
          datosRecibidos.mensaje || `El servidor respondió con el estado ${respuesta.status}.`
        );
      }

      mensajePantalla.textContent =
        datosRecibidos.mensaje || "¡Pedido recibido y guardado correctamente!";
      pedidosRealizados.unshift(pedidoParaHistorial);
      renderizarHistorial();
      formulario.reset();
      carrito.clear();
      renderizarCarrito();
      await cargarHistorialUsuario();
    } catch (error) {
      console.error("Error al enviar el pedido:", error);
      mensajePantalla.textContent =
        error instanceof TypeError
          ? "No se pudo conectar con el servidor. Revisá que esté disponible e intentá nuevamente."
          : error.message;
      mensajePantalla.classList.add("is-error");
    } finally {
      isSubmitting = false;
      botonEnviar.textContent = "Confirmar pedido →";
      renderizarCarrito();
    }
  });

  renderizarCarrito();
  renderizarHistorial();
  actualizarNavegacionUsuario();
  cargarHistorialUsuario();
});
