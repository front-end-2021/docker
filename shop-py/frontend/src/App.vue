<script setup>
import { onMounted, ref } from 'vue'
import { api } from './api'
import { useCart } from './composables/useCart'
import ProductRow from './components/ProductRow.vue'
import CartPanel from './components/CartPanel.vue'

const products = ref([])
const { refresh } = useCart()

const loadProducts = async () => { products.value = await api.products() }
onMounted(() => { loadProducts(); refresh() })
</script>

<template>
  <div class="page">
    <h1>Mini Shop</h1>
    <div class="layout">
      <section>
        <ProductRow v-for="p in products" :key="p.id" :product="p" />
      </section>
      <CartPanel @ordered="loadProducts" />
    </div>
  </div>
</template>
