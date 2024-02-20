<template>
  <div>
    <br />
    <br />
    <button @click="submit" :disabled="uploading">Delete all uploaded files</button>
    <br />
    <br />
  </div>
</template>

<script>
import API from '@/api'
export default {
  name: 'ClearView',
  methods: {
    async submit() {
      this.uploading = true

      await API.post('/clear')

      this.uploading = false

      this.$router.push('/')
    }
  }
}
</script>
