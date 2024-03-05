<template>
  <div class="box">
    <div style="text-align: center">
      <select
        @change="onPageSelected"
        v-model="selectedPage"
        class="page-selection"
      >
        <option v-for="page in pageNames" :key="page" :value="page">
          {{ page }}
        </option>
      </select>
    </div>

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
      selectedPage: null,
      pageNames: null,
      svg: null
    }
  },
  methods: {
    onPageSelected() {
      this.navigate(this.selectedPage)
    },
    async updateSvg() {
      const current = this.getUrlQueryParams().find(
        (obj) => obj.name === 'page'
      )?.value
      if (!current) {
        return
      }

      const response = await API.get(`/svg?pageName=${current}`)
      const pageContent = response.data

      const parser = new DOMParser()
      const svgDoc = parser.parseFromString(pageContent, 'image/svg+xml')
      const svgElement = svgDoc.documentElement
      svgElement.setAttribute(
        'height',
        this.$refs.svgContainer.offsetHeight - 30
      )

      this.svg = svgElement.outerHTML
    },
    navigate(id) {
      this.selectedPage = this.pageNames.find(
        (x) => x.toLowerCase() === id.toLowerCase()
      )

      this.updateBrowserHistory(this.selectedPage)

      this.updateSvg()
    },
    updateBrowserHistory(pageName) {
      history.pushState(
        {
          page: pageName
        },
        null,
        `${window.location.pathname}?page=${pageName}`
      )
    },
    getUrlQueryParams() {
      return window.location.search
        .replace('?', '')
        .split('&')
        .filter((v) => v)
        .map((s) => {
          s = s.replace('+', '%20')
          s = s.split('=').map((s) => decodeURIComponent(s))
          return {
            name: s[0],
            value: s[1]
          }
        })
    }
  },
  mounted() {
    window.hook = this
    window.onpopstate = (e) => {
      this.updateSvg()
    }
    this.updateSvg()
  },
  async created() {
    const response = await API.get('/pageNames')
    this.pageNames = response.data

    this.navigate('index')
  }
}
</script>

<style>
.svg-container {
  margin-top: 10px;
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
.page-selection {
  font-size: medium;
  min-width: 300px;
}
</style>
