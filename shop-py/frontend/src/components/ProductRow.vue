<script setup>
import { useCart } from '../composables/useCart'
import { vnd } from '../format'

defineProps({ product: { type: Object, required: true } })
const { add } = useCart()
</script>

<template>
  <div class="row">
    <div>
      <div class="name">{{ product.name }}</div>
      <div class="muted">{{ product.stock > 0 ? `Còn ${product.stock} sản phẩm` : 'Hết hàng' }}</div>
    </div>
    <span class="price">{{ vnd(product.price) }}</span>
    <button :disabled="product.stock === 0" @click="add(product.id)">Thêm vào giỏ</button>
  </div>
</template>
