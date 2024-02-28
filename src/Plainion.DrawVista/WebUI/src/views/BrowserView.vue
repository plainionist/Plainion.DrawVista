<template>
  <div class="box">
    <div style="text-align: left; margin-left: 50">
      <span style="font-weight: bold">Pages: </span>
      <span style="text-align: right">
        <select @change="onPageSelected" v-model="current">
          <option v-for="page in pageNames" :key="page" :value="page">
            {{ page }}
          </option>
        </select>
      </span>
      <span style="margin-left: 50px; font-weight: bold">Track: </span>
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
          :minZoom="0.25">
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
      pageNames: null,
      svg: null
    }
  },
  methods: {
    onPageSelected() {
      this.history = []
      this.updateSvg()
    },
    goTo(step) {
      while ((this.current = this.history.pop()) !== step);
      this.updateSvg()
    },
    async fetchContent(pageName) {
      const response = await API.get(`/svg?pageName=${pageName}`)
      return response.data
    },
    async updateSvg() {
      if (!this.pageNames || this.pageNames.length === 0 || !this.current) {
        return
      }

      const page = this.pageNames.find((x) => x === this.current.toLowerCase())

      const pageContent = await this.fetchContent(page)

      const parser = new DOMParser()
      const svgDoc = parser.parseFromString(pageContent, 'image/svg+xml')
      const svgElement = svgDoc.documentElement
      svgElement.setAttribute('height', this.$refs.svgContainer.offsetHeight - 30)

      this.svg = svgElement.outerHTML
    },
    navigate(id) {
      if (this.current) {
        this.history.push(this.current)
      }

      this.current = this.pageNames.find((x) => x === id.toLowerCase())
      this.updateSvg()
    }
  },
  mounted() {
    window.hook = this
    this.updateSvg()
  },
  async created() {
    const response = await API.get('/pageNames')
    this.pageNames = response.data.map(x => x.toLowerCase())

    this.navigate('index')
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
