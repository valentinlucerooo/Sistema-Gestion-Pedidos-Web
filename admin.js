document.addEventListener("DOMContentLoaded", () => {
  const ADMIN_EMAIL = "admin@ecommerce.com";
  const LOGIN_URL = "index.html#cuenta";
  const API_BASE_URL = "https://localhost:7195/api";

  // Ajustá estas rutas si los controladores del backend usan otro prefijo.
  const LIST_API = {
    products: "https://localhost:7195/api/producto",
    shipping: "https://localhost:7195/api/enviocontrol",
    orders: `${API_BASE_URL}/pedido/pedidocontrol`
  };
  const ORDER_API = `${API_BASE_URL}/pedido`;
  const ORDER_STATUSES = ["Pendiente", "En preparacion", "En viaje", "Entregado"];
  const currency = new Intl.NumberFormat("es-AR", {
    style: "currency",
    currency: "ARS",
    maximumFractionDigits: 2
  });

  const app = document.querySelector("#adminApp");
  const productTable = document.querySelector("#productsTable");
  const shippingTable = document.querySelector("#shippingTable");
  const ordersTable = document.querySelector("#ordersTable");
  const productDialog = document.querySelector("#productDialog");
  const shippingDialog = document.querySelector("#shippingDialog");
  const productForm = document.querySelector("#productForm");
  const shippingForm = document.querySelector("#shippingForm");
  const collections = { products: [], shipping: [], orders: [] };
  let busy = false;

  function getSavedUser() {
    try {
      const savedUser = localStorage.getItem("usuarioLogueado");
      return savedUser ? JSON.parse(savedUser) : null;
    } catch (error) {
      console.error("No se pudo leer la sesión guardada:", error);
      return null;
    }
  }

  const user = getSavedUser();
  const isAdmin = typeof user?.email === "string"
    && user.email.trim().toLowerCase() === ADMIN_EMAIL;
  if (!isAdmin) {
    window.location.replace(LOGIN_URL);
    return;
  }

  app.hidden = false;
  document.querySelector("#adminEmail").textContent = user.email;

  function firstValue(object, ...keys) {
    for (const key of keys) {
      if (object && object[key] !== undefined && object[key] !== null) {
        return object[key];
      }
    }
    return "";
  }

  function getId(item, type) {
    const keys = type === "products"
      ? ["idProducto", "idproducto", "IDProducto", "id"]
      : type === "shipping"
        ? ["idEnvio", "idenvio", "IDEnvio", "id"]
        : ["idPedido", "idpedido", "IDPedido", "IDpedido", "ID_PEDIDO", "id_pedido", "id"];
    return firstValue(item, ...keys);
  }

  function getCollection(payload, key) {
    if (Array.isArray(payload)) return payload;
    const candidates = [key, key.toLowerCase(), "data"];
    for (const candidate of candidates) {
      if (Array.isArray(payload?.[candidate])) return payload[candidate];
    }
    throw new Error(`El servidor devolvió un listado de ${key} con formato no válido.`);
  }

  function getErrorMessage(payload, fallback) {
    if (typeof payload === "string" && payload.trim()) return payload;
    const message = payload?.mensaje || payload?.message || payload?.title;
    if (typeof message === "string" && message.trim()) return message;
    if (payload?.errors && typeof payload.errors === "object") {
      const errors = Object.values(payload.errors).flat()
        .filter((entry) => typeof entry === "string");
      if (errors.length) return errors.join(" ");
    }
    return fallback;
  }

  async function request(url, options = {}) {
    const headers = new Headers(options.headers || {});
    if (options.body && !headers.has("Content-Type")) {
      headers.set("Content-Type", "application/json");
    }

    const savedToken = user.token || user.accessToken;
    if (savedToken && !headers.has("Authorization")) {
      headers.set("Authorization", `Bearer ${savedToken}`);
    }

    const response = await fetch(url, { ...options, headers });
    const responseText = await response.text();
    let payload = null;
    if (responseText) {
      try {
        payload = JSON.parse(responseText);
      } catch {
        payload = responseText;
      }
    }

    if (!response.ok) {
      throw new Error(getErrorMessage(
        payload,
        `El servidor respondió con el estado ${response.status}.`
      ));
    }
    return payload;
  }

  function setMessage(elementId, message = "", isError = false) {
    const element = document.getElementById(elementId);
    element.textContent = message;
    element.classList.toggle("is-error", isError);
  }

  function setTableMessage(table, columns, message) {
    const row = document.createElement("tr");
    const cell = document.createElement("td");
    cell.className = "table-state";
    cell.colSpan = columns;
    cell.textContent = message;
    row.append(cell);
    table.replaceChildren(row);
  }

  function textCell(text, className = "") {
    const cell = document.createElement("td");
    cell.textContent = text;
    if (className) cell.className = className;
    return cell;
  }

  function makeButton(label, action, className = "action-button") {
    const button = document.createElement("button");
    button.type = "button";
    button.className = className;
    button.textContent = label;
    button.dataset.action = action;
    return button;
  }

  function renderProducts() {
    if (!collections.products.length) {
      setTableMessage(productTable, 4, "Todavía no hay productos cargados.");
      return;
    }

    const rows = collections.products.map((product) => {
      const row = document.createElement("tr");
      const title = document.createElement("span");
      title.className = "cell-title";
      title.textContent = firstValue(product, "descripcion", "nombre", "Descripcion") || "Sin nombre";
      const id = getId(product, "products");
      const idNote = document.createElement("span");
      idNote.className = "cell-subtitle";
      idNote.textContent = `ID ${id}`;
      const productCell = document.createElement("td");
      productCell.append(title, idNote);
      const category = firstValue(product, "productoTipo", "tipoProducto", "categoria", "tipo") || "Sin categoría";
      const price = Number(firstValue(product, "precio", "costo", "precioProducto"));
      row.append(
        productCell,
        textCell(String(category)),
        textCell(Number.isFinite(price) ? currency.format(price) : "Sin precio")
      );

      const actions = document.createElement("td");
      const actionGroup = document.createElement("div");
      actionGroup.className = "row-actions";
      const edit = makeButton("Modificar producto", "edit-product");
      edit.dataset.id = String(id);
      const remove = makeButton("Eliminar producto", "delete-product", "action-button action-button--danger");
      remove.dataset.id = String(id);
      actionGroup.append(edit, remove);
      actions.append(actionGroup);
      row.append(actions);
      return row;
    });
    productTable.replaceChildren(...rows);
  }

  function renderShipping() {
    if (!collections.shipping.length) {
      setTableMessage(shippingTable, 3, "Todavía no hay zonas de envío cargadas.");
      return;
    }

    const rows = collections.shipping.map((shipping) => {
      const row = document.createElement("tr");
      const zone = firstValue(shipping, "zona", "nombre", "Zona") || "Sin nombre";
      const id = getId(shipping, "shipping");
      const zoneCell = document.createElement("td");
      const title = document.createElement("span");
      title.className = "cell-title";
      title.textContent = String(zone);
      const idNote = document.createElement("span");
      idNote.className = "cell-subtitle";
      idNote.textContent = `ID ${id}`;
      zoneCell.append(title, idNote);
      const price = Number(firstValue(shipping, "precio", "Precio", "costo", "precioEnvio", "monto"));
      row.append(
        zoneCell,
        textCell(Number.isFinite(price) ? currency.format(price) : "Sin precio")
      );

      const actions = document.createElement("td");
      const actionGroup = document.createElement("div");
      actionGroup.className = "row-actions";
      const edit = makeButton("Modificar zona de envío", "edit-shipping");
      edit.dataset.id = String(id);
      const remove = makeButton("Eliminar zona de envío", "delete-shipping", "action-button action-button--danger");
      remove.dataset.id = String(id);
      actionGroup.append(edit, remove);
      actions.append(actionGroup);
      row.append(actions);
      return row;
    });
    shippingTable.replaceChildren(...rows);
  }

  function getOrderStatus(order) {
    return String(firstValue(order, "estado", "Estado") || "Pendiente");
  }

  function isDelivered(status) {
    return status.trim().toLocaleLowerCase("es") === "entregado";
  }

  function renderOrders() {
    const query = document.querySelector("#orderSearch").value.trim().toLocaleLowerCase("es");
    const orders = collections.orders.filter((order) => {
      const status = getOrderStatus(order);
      if (status.trim().toLocaleLowerCase("es") === "archivado") return false;
      const id = String(getId(order, "orders"));
      const customer = firstValue(order.usuario, "email", "nombre", "apellido");
      return `${id} ${customer}`.toLocaleLowerCase("es").includes(query);
    });

    if (!orders.length) {
      setTableMessage(
        ordersTable,
        7,
        collections.orders.length ? "No hay pedidos activos que coincidan con la búsqueda." : "Todavía no hay pedidos activos."
      );
      return;
    }

    const rows = orders.map((order) => {
      const row = document.createElement("tr");
      const id = getId(order, "orders");
      const idCell = document.createElement("td");
      const orderTitle = document.createElement("span");
      orderTitle.className = "cell-title";
      orderTitle.textContent = `#${id}`;
      idCell.append(orderTitle);

      const customer = order.usuario || {};
      const customerName = [
        firstValue(customer, "nombre"),
        firstValue(customer, "apellido")
      ].filter(Boolean).join(" ");
      const customerEmail = firstValue(customer, "email", "correo");
      const customerCell = document.createElement("td");
      const customerTitle = document.createElement("span");
      customerTitle.className = "cell-title";
      customerTitle.textContent = customerName || customerEmail || "No informado";
      customerCell.append(customerTitle);
      if (customerName && customerEmail) {
        const email = document.createElement("span");
        email.className = "cell-subtitle";
        email.textContent = String(customerEmail);
        customerCell.append(email);
      }

      const zone = firstValue(order.envio, "zona", "nombre") || "No especificado";
      const rawDate = firstValue(order, "fechaRegistro", "fecha", "createdAt");
      const parsedDate = rawDate ? new Date(rawDate) : null;
      const date = parsedDate && !Number.isNaN(parsedDate.getTime())
        ? new Intl.DateTimeFormat("es-AR", { dateStyle: "short", timeStyle: "short" }).format(parsedDate)
        : "No disponible";
      const total = Number(firstValue(order, "montoTotal", "total"));
      const status = getOrderStatus(order);
      row.append(
        idCell,
        customerCell,
        textCell(String(zone)),
        textCell(date),
        textCell(Number.isFinite(total) ? currency.format(total) : "No disponible")
      );

      const statusCell = document.createElement("td");
      const statusControl = document.createElement("select");
      statusControl.className = "status-select";
      statusControl.setAttribute("aria-label", `Estado del pedido ${id}`);
      statusControl.dataset.action = "update-order";
      statusControl.dataset.id = String(id);
      const statuses = [...ORDER_STATUSES];
      if (status && !statuses.some((item) => item.toLocaleLowerCase("es") === status.toLocaleLowerCase("es"))) {
        statuses.push(status);
      }
      statuses.forEach((value) => {
        const option = document.createElement("option");
        option.value = value;
        option.textContent = value;
        option.selected = value.toLocaleLowerCase("es") === status.toLocaleLowerCase("es");
        statusControl.append(option);
      });
      statusCell.append(statusControl);
      row.append(statusCell);

      const actionCell = document.createElement("td");
      if (isDelivered(status)) {
        const archive = makeButton("Archivar pedido", "archive-order", "action-button");
        archive.dataset.id = String(id);
        archive.setAttribute("aria-label", `Archivar pedido ${id}`);
        actionCell.append(archive);
      } else {
        actionCell.textContent = "—";
      }
      row.append(actionCell);
      return row;
    });
    ordersTable.replaceChildren(...rows);
  }

  async function loadCollection(type, endpoint, key, table, columnCount, render) {
    setTableMessage(table, columnCount, "Cargando…");
    try {
      const payload = await request(endpoint);
      collections[type] = getCollection(payload, key);
      render();
    } catch (error) {
      if (type === "products") {
        console.error("Error al cargar productos:", error.message);
      } else {
        console.error(`No se pudo cargar ${key}:`, error);
      }
      setTableMessage(table, columnCount, error.message);
    }
  }

  function findById(collection, id, type) {
    return collection.find((item) => String(getId(item, type)) === String(id));
  }

  function openProductDialog(product = null) {
    productForm.reset();
    productForm.elements.id.value = product ? String(getId(product, "products")) : "";
    productForm.elements.descripcion.value = product
      ? firstValue(product, "descripcion", "nombre", "Descripcion")
      : "";
    productForm.elements.productoTipo.value = product
      ? firstValue(product, "productoTipo", "tipoProducto", "categoria", "tipo")
      : "";
    productForm.elements.precio.value = product
      ? firstValue(product, "precio", "costo", "precioProducto")
      : "";
    productForm.querySelector(".dialog-error").textContent = "";
    document.querySelector("#productDialogTitle").textContent = product
      ? "Modificar producto"
      : "Agregar producto";
    productForm.querySelector('[type="submit"]').textContent = product
      ? "Guardar cambios"
      : "Guardar producto";
    productDialog.showModal();
  }

  function openShippingDialog(shipping = null) {
    shippingForm.reset();
    shippingForm.elements.id.value = shipping ? String(getId(shipping, "shipping")) : "";
    shippingForm.elements.zona.value = shipping
      ? firstValue(shipping, "zona", "nombre", "Zona")
      : "";
    shippingForm.elements.precio.value = shipping
      ? firstValue(shipping, "precio", "Precio", "costo", "precioEnvio", "monto")
      : "";
    shippingForm.querySelector(".dialog-error").textContent = "";
    document.querySelector("#shippingDialogTitle").textContent = shipping
      ? "Modificar zona"
      : "Agregar zona";
    shippingForm.querySelector('[type="submit"]').textContent = shipping
      ? "Guardar cambios"
      : "Guardar zona";
    shippingDialog.showModal();
  }

  async function withBusy(button, action) {
    if (busy) return;
    busy = true;
    if (button) button.disabled = true;
    try {
      await action();
    } finally {
      busy = false;
      if (button?.isConnected) button.disabled = false;
    }
  }

  document.querySelector("#addProductButton").addEventListener("click", () => openProductDialog());
  document.querySelector("#addShippingButton").addEventListener("click", () => openShippingDialog());
  document.querySelector("#orderSearch").addEventListener("input", renderOrders);

  document.querySelector("#logoutButton").addEventListener("click", () => {
    try {
      localStorage.removeItem("usuarioLogueado");
    } catch (error) {
      console.error("No se pudo cerrar la sesión:", error);
      window.alert("No se pudo cerrar la sesión. Intentá nuevamente.");
      return;
    }
    window.location.replace(LOGIN_URL);
  });

  document.querySelectorAll("[data-close-dialog]").forEach((button) => {
    button.addEventListener("click", () => button.closest("dialog").close());
  });

  productTable.addEventListener("click", async (event) => {
    const button = event.target.closest("[data-action]");
    if (!button || busy) return;
    const product = findById(collections.products, button.dataset.id, "products");
    if (button.dataset.action === "edit-product" && product) {
      openProductDialog(product);
      return;
    }
    if (button.dataset.action === "delete-product" && product) {
      const name = firstValue(product, "descripcion", "nombre") || `#${button.dataset.id}`;
      if (!window.confirm(`¿Eliminar el producto “${name}”?`)) return;
      setMessage("productsMessage");
      await withBusy(button, async () => {
        try {
          await request(`${LIST_API.products}/${encodeURIComponent(button.dataset.id)}`, { method: "DELETE" });
          collections.products = collections.products.filter(
            (item) => String(getId(item, "products")) !== button.dataset.id
          );
          renderProducts();
          setMessage("productsMessage", "Producto eliminado.");
        } catch (error) {
          console.error("No se pudo eliminar el producto:", error);
          setMessage("productsMessage", error.message, true);
        }
      });
    }
  });

  shippingTable.addEventListener("click", async (event) => {
    const button = event.target.closest("[data-action]");
    if (!button || busy) return;
    const shipping = findById(collections.shipping, button.dataset.id, "shipping");
    if (button.dataset.action === "edit-shipping" && shipping) {
      openShippingDialog(shipping);
      return;
    }
    if (button.dataset.action === "delete-shipping" && shipping) {
      const zone = firstValue(shipping, "zona", "nombre") || `#${button.dataset.id}`;
      if (!window.confirm(`¿Eliminar la zona de envío “${zone}”?`)) return;
      setMessage("shippingMessage");
      await withBusy(button, async () => {
        try {
          await request(`${LIST_API.shipping}/${encodeURIComponent(button.dataset.id)}`, { method: "DELETE" });
          collections.shipping = collections.shipping.filter(
            (item) => String(getId(item, "shipping")) !== button.dataset.id
          );
          renderShipping();
          setMessage("shippingMessage", "Zona de envío eliminada.");
        } catch (error) {
          console.error("No se pudo eliminar la zona de envío:", error);
          setMessage("shippingMessage", error.message, true);
        }
      });
    }
  });

  productForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!productForm.reportValidity()) return;
    const id = productForm.elements.id.value;
    const payload = {
      descripcion: productForm.elements.descripcion.value.trim(),
      productoTipo: productForm.elements.productoTipo.value.trim(),
      precio: Number(productForm.elements.precio.value)
    };
    if (!payload.descripcion || !payload.productoTipo || !Number.isFinite(payload.precio) || payload.precio < 0) {
      productForm.querySelector(".dialog-error").textContent = "Revisá los datos ingresados.";
      return;
    }

    const submit = productForm.querySelector('[type="submit"]');
    await withBusy(submit, async () => {
      try {
        await request(
          id ? `${LIST_API.products}/${encodeURIComponent(id)}` : LIST_API.products,
          { method: id ? "PUT" : "POST", body: JSON.stringify(payload) }
        );
        productDialog.close();
        await loadCollection("products", LIST_API.products, "productos", productTable, 4, renderProducts);
        setMessage("productsMessage", id ? "Producto actualizado." : "Producto agregado.");
      } catch (error) {
        console.error("No se pudo guardar el producto:", error);
        productForm.querySelector(".dialog-error").textContent = error.message;
      }
    });
  });

  shippingForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    if (!shippingForm.reportValidity()) return;
    const id = shippingForm.elements.id.value;
    const payload = {
      zona: shippingForm.elements.zona.value.trim(),
      precio: shippingForm.elements.precio.valueAsNumber
    };
    if (!payload.zona || !Number.isFinite(payload.precio) || payload.precio <= 0) {
      shippingForm.querySelector(".dialog-error").textContent = "Revisá los datos ingresados.";
      return;
    }

    const submit = shippingForm.querySelector('[type="submit"]');
    await withBusy(submit, async () => {
      try {
        await request(
          id ? `${LIST_API.shipping}/${encodeURIComponent(id)}` : LIST_API.shipping,
          { method: id ? "PUT" : "POST", body: JSON.stringify(payload) }
        );
        shippingDialog.close();
        await loadCollection("shipping", LIST_API.shipping, "envios", shippingTable, 3, renderShipping);
        setMessage("shippingMessage", id ? "Zona actualizada." : "Zona agregada.");
      } catch (error) {
        console.error("No se pudo guardar la zona de envío:", error);
        shippingForm.querySelector(".dialog-error").textContent = error.message;
      }
    });
  });

  ordersTable.addEventListener("change", async (event) => {
    const select = event.target.closest('[data-action="update-order"]');
    if (!select || busy) return;
    const order = findById(collections.orders, select.dataset.id, "orders");
    if (!order) return;
    const idPedido = getId(order, "orders");
    if (idPedido === "") {
      setMessage("ordersMessage", "No se pudo identificar el pedido.", true);
      return;
    }
    const previousStatus = getOrderStatus(order);
    const nextStatus = select.value;
    if (nextStatus === previousStatus) return;

    select.disabled = true;
    setMessage("ordersMessage");
    await withBusy(select, async () => {
      try {
        await request(`${ORDER_API}/actualizarEstado/${encodeURIComponent(idPedido)}`, {
          method: "PATCH",
          body: JSON.stringify({ estado: nextStatus })
        });
        if ("estado" in order) order.estado = nextStatus;
        else order.Estado = nextStatus;
        renderOrders();
        setMessage(
          "ordersMessage",
          nextStatus.toLocaleLowerCase("es") === "archivado"
            ? "Pedido archivado."
            : "Estado del pedido actualizado."
        );
      } catch (error) {
        console.error("No se pudo actualizar el estado del pedido:", error);
        select.disabled = false;
        select.value = previousStatus;
        setMessage("ordersMessage", error.message, true);
      }
    });
  });

  ordersTable.addEventListener("click", async (event) => {
    const button = event.target.closest('[data-action="archive-order"]');
    if (!button || busy) return;
    if (!window.confirm(`¿Archivar el pedido #${button.dataset.id}? El registro no se eliminará.`)) return;
    setMessage("ordersMessage");
    await withBusy(button, async () => {
      try {
        await request(`${ORDER_API}/actualizarEstado/${encodeURIComponent(button.dataset.id)}`, {
          method: "PATCH",
          body: JSON.stringify({ estado: "Archivado" })
        });
        const order = findById(collections.orders, button.dataset.id, "orders");
        if (order) {
          if ("estado" in order) order.estado = "Archivado";
          else order.Estado = "Archivado";
        }
        renderOrders();
        setMessage("ordersMessage", "Pedido archivado; el registro permanece en el sistema.");
      } catch (error) {
        console.error("No se pudo archivar el pedido:", error);
        setMessage("ordersMessage", error.message, true);
      }
    });
  });

  loadCollection("products", LIST_API.products, "productos", productTable, 4, renderProducts);
  loadCollection("shipping", LIST_API.shipping, "envios", shippingTable, 3, renderShipping);
  loadCollection("orders", LIST_API.orders, "pedidos", ordersTable, 7, renderOrders);
});
