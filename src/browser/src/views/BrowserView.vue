<template>
  <div>
    <div>
      <span v-for="(step,idx) in history" :key="step">
        <span
          style="color: blue; text-decoration: underline; cursor: pointer"
          @click="back"
          >{{ step }}</span
        >
        <span v-if="idx > 0">/</span>
      </span>
    </div>

    <br />

    <div v-html="svg"></div>
  </div>
</template>

<script>
export default {
  name: 'BrowserView',
  data() {
    return {
      current: null,
      history: [],
      pages: null
    }
  },
  computed: {
    svg() {
      const page = this.pages.find((x) => x.id === this.current)
      console.log(this.current)
      return page.content
    }
  },
  methods: {
    back() {
      this.current = this.history.pop()
    },
    navigate(id) {
      this.history.push(this.current)
      this.current = id.toLowerCase()
    }
  },
  mounted() {
    window.hook = this
  },
  created() {
    const files = require.context('@/assets/', false, /\.svg$/)
    this.pages = files.keys().map((f) => {
      return {
        id: f.replace(/^\.\//, '').replace(/.svg$/, '').toLowerCase(),
        content: files(f).default
      }
    })
    this.current = 'index'
  }
}
</script>
