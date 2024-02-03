<template>
  <div>
    <div v-html="svg"></div>
    <div v-html="require('!html-loader!../assets/orchestrator.svg')"></div>
  </div>
</template>

<script>
export default {
  name: 'BrowserView',
  data() {
    return {
      svg: null
    }
  },
  created() {
    const files = require.context('@/assets/', false, /\.svg$/)
    const db = files.keys().map((f) => {
      return {
        id: f,
        content: files(f)
      }
    })
    console.log(db)

    const name = 'overview'
    this.svg = require(`@/assets/${name}.svg`)
  }
}
</script>
