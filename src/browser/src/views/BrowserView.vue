<template>
  <div>
    <div>
      <span
        style="color: blue; text-decoration: underline; cursor: pointer"
        @click="back"
        v-if="current !== 'index'"
        >back</span
      >
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
      pages: null
    }
  },
  computed: {
    svg() {
      const page = this.pages.find((x) => x.id === this.current)
      return page.content
    }
  },
  methods: {
    back() {
      this.navigate('index')
    },
    navigate(id) {
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
    this.navigate('index')
  }
}
</script>
