async function request(path, options = {}) {
  const res = await fetch('/api' + path, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  })
  if (!res.ok) {
    const err = await res.json().catch(() => ({}))
    throw new Error(err.detail || `Lỗi ${res.status}`)
  }
  return res.json()
}

export const api = {
  products: () => request('/products'),
  cart: (id) => request(`/carts/${id}`),
  addItem: (id, product_id, quantity = 1) =>
    request(`/carts/${id}/items`, { method: 'POST', body: JSON.stringify({ product_id, quantity }) }),
  removeItem: (id, product_id) => request(`/carts/${id}/items/${product_id}`, { method: 'DELETE' }),
  checkout: (id) => request(`/orders/checkout/${id}`, { method: 'POST' }),
}
