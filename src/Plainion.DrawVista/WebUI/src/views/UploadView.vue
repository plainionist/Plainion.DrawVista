<template>
  <div>
    <button @click="submit" :disabled="uploading">Upload</button>
    <br />
    <br />
    <div class="center">
      <drop-items @itemsSelected="onItemsSelected" />
    </div>
  </div>
</template>

<script>
import API from '@/api'
import DropItems from './DropItems.vue'
export default {
  name: 'UploadView',
  components: { DropItems },
  data() {
    return {
      uploading: false
    }
  },
  methods: {
    onItemsSelected(items) {
      this.items = items
    },
    async submit() {
      this.uploading = true

      const formData = new FormData()
      this.items.forEach((item) => formData.append(item.name, item))
      const headers = { 'Content-Type': 'multipart/form-data' }

      await API.post('/upload', formData, { headers })

      this.uploading = false

      this.$router.push('/')
    }
  }
}
</script>

<style scoped>
.center {
  margin: auto;
  width: 50%;
}
</style>
