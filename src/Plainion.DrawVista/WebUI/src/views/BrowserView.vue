<template>
  <div class="box">
    <div style="text-align: left; margin-left: 50">
      <b>Navigation: </b>
      <span v-for="(step, idx) in history" :key="step">
        <span v-if="idx > 0">/</span>
        <span
          style="color: blue; text-decoration: underline; cursor: pointer"
          @click="goTo(step)"
          >{{ step }}</span
        >
      </span>
    </div>

    <br />

    <div class="svg-container box content" ref="svgContainer">
      <transition name="scale" mode="out-in">
        <SvgPanZoom
          :key="svg"
          style="width: 100%; height: 100%"
          :zoomEnabled="true"
          :controlIconsEnabled="false"
          :fit="true"
          :center="true"
          :minZoom="0.25"
        >
          <div v-html="svg"></div>
        </SvgPanZoom>
      </transition>
    </div>
  </div>
</template>

<script>
import { SvgPanZoom } from 'vue-svg-pan-zoom'
import API from '@/api'

export default {
  name: 'BrowserView',
  components: { SvgPanZoom },
  data() {
    return {
      current: null,
      history: [],
      pages: null,
      svg: null
    }
  },
  methods: {
    goTo(step) {
      while ((this.current = this.history.pop()) !== step);
      this.updateSvg()
    },
    updateSvg() {
      if (!this.pages) {
        return
      }
      const page = this.pages.find((x) => x.id === this.current)

      const parser = new DOMParser()
      const svgDoc = parser.parseFromString(page.content, 'image/svg+xml')
      const svgElement = svgDoc.documentElement
      svgElement.setAttribute(
        'height',
        this.$refs.svgContainer.offsetHeight - 30
      )

      this.svg = svgElement.outerHTML
    },
    navigate(id) {
      this.history.push(this.current)
      this.current = id.toLowerCase()
      this.updateSvg()
    }
  },
  mounted() {
    window.hook = this
    this.updateSvg()
  },
  async created() {
    const response = await API.get('/allFiles')
    this.pages = response.data
    this.current = 'index'
    this.updateSvg()
  }
}
</script>

<style>
.svg-container {
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
