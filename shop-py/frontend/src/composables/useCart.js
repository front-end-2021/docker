import { ref } from 'vue'
import { api } from '../api'

// State nằm ngoài hàm nên mọi component dùng chung một giỏ hàng (không cần Pinia).
const cartId = localStorage.getItem('cart_id') ?? crypto.randomUUID()
localStorage.setItem('cart_id', cartId)

const cart = ref({ items: [], total: 0 })
const order = ref(null)
const error = ref('')

async function run(fn) {
  error.value = ''
  try {
    return await fn()
  } catch (e) {
    error.value = e.message
  }
}

export function useCart() {
  return {
    cart,
    order,
    error,
    refresh: () => run(async () => { cart.value = await api.cart(cartId) }),
    add: (productId) => run(async () => {
      order.value = null;
      cart.value = await api.addItem(cartId, productId)
    }),
    remove: (productId) => run(async () => { cart.value = await api.removeItem(cartId, productId) }),
    checkout: () => run(async () => {
      order.value = await api.checkout(cartId)
      cart.value = await api.cart(cartId)
    }),
  }
}
