<template>
  <div>
    <div style="text-align: left; margin-left: 50">
      <b>Navigation: </b>
      <span v-for="(step, idx) in history" :key="step">
        <span
          style="color: blue; text-decoration: underline; cursor: pointer"
          @click="back"
          >{{ step }}</span
        >
        <span v-if="idx > 0">/</span>
      </span>
    </div>

    <br />

    <div class="svg-container">
      <transition name="scale" mode="out-in">
        <SvgPanZoom
          :key="svg"
          style="width: 100%; height: 100%"
          :zoomEnabled="true"
          :controlIconsEnabled="false"
          :fit="true"
          :center="true"
        >
          <div v-html="svg"></div>
        </SvgPanZoom>
      </transition>
    </div>
  </div>
</template>

<script>
import { SvgPanZoom } from 'vue-svg-pan-zoom'
export default {
  name: 'BrowserView',
  components: { SvgPanZoom },
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

<style>
.svg-container {
  margin: auto;
  width: 95%;
  height: 100%;
  padding: 10px;
  border: 1px solid black;
}
.scale-enter-active,
.scale-leave-active {
  transition: all 0.25s ease;
}

.scale-enter-from,
.scale-leave-to {
  opacity: 0;
  transform: scale(0.9);
}
</style>
