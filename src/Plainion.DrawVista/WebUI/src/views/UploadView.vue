<template>
  <div>
    <button @click="submitFile">Upload</button>
    <br />
    <br />
    <center>
      <div style="width: 50%">
        <drop-items @itemsSelected="onItemsSelected" />
      </div>
    </center>
  </div>
</template>

<script>
import API from '@/api'
import DropItems from './DropItems.vue'
export default {
  name: 'UploadView',
  components: { DropItems },
  methods: {
    onItemsSelected(items) {
      this.items = items
    },
    submitFile() {
      const formData = new FormData()
      this.items.forEach((file) => formData.append('file', file))
      const headers = { 'Content-Type': 'multipart/form-data' }
      API.post('/upload', formData, { headers })
    }
  }
}
</script>
