<script setup>
import { useCart } from '../composables/useCart'
import { vnd } from '../format'

const emit = defineEmits(['ordered'])
const { cart, order, error, remove, checkout } = useCart()

async function placeOrder() {
  await checkout()
  emit('ordered') // báo App tải lại tồn kho
}
</script>

<template>
  <aside class="cart">
    <h2>Giỏ hàng</h2>
    <p v-if="!cart.items.length" class="muted">Chưa có sản phẩm nào. Chọn một sản phẩm để bắt đầu.</p>
    <ul v-else>
      <li v-for="i in cart.items" :key="i.product_id">
        <span>
          {{ i.name }} × {{ i.quantity }}
          <button class="link" @click="remove(i.product_id)">Xóa</button>
        </span>
        <span>{{ vnd(i.line_total) }}</span>
      </li>
    </ul>
    <p class="total"><span>Tổng</span><strong>{{ vnd(cart.total) }}</strong></p>
    <button :disabled="!cart.items.length" @click="placeOrder">Đặt hàng</button>
    <p v-if="error" class="error">{{ error }}</p>
    <p v-if="order" class="ok">Đơn #{{ order.id }} đã được tạo, trạng thái: {{ order.status }}.</p>
  </aside>
</template>
