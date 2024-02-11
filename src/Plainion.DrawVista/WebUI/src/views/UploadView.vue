<template>
  <div>
    <input type="file" @change="uploadFile" ref="file" />
    <button @click="submitFile">Upload</button>
  </div>
</template>

<script>
import API from '@/api'

export default {
  name: 'UploadView',
  methods: {
    uploadFile() {
      this.Images = this.$refs.file.files[0]
    },
    submitFile() {
      const formData = new FormData()
      formData.append('file', this.Images)
      const headers = { 'Content-Type': 'multipart/form-data' }
      API.post('/upload', formData, { headers })
    }
  }
}
</script>
