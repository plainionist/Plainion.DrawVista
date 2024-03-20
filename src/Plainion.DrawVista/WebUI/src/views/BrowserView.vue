<template>
  <div class="box">
    <div style="text-align: center">
      <span>
        <label>Pages</label>
        <select
          @change="onPageSelected"
          v-model="selectedPage"
          class="page-selection"
        >
          <option v-for="page in pageNames" :key="page" :value="page">
            {{ page }}
          </option>
        </select>
      </span>

      <span style="margin-left: 30px">
        <label>Search</label>
        <input
          type="text"
          v-model="searchText"
          v-on:keyup.enter="onSearchEnter"
          class="search-input"
        />
      </span>
    </div>

    <div v-if="searchResults.length > 0" class="search-results-container">
      <span
        v-for="item in searchResults"
        v-bind:key="item.pageName"
        class="search-results-item"
        ><a href="#" @click="onSearchResultSelected(item.pageName)">{{ item.pageName }}</a></span
      >
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
      svg: null,
      searchText: null,
      searchResults: []
    }
  },
  methods: {
    onPageSelected() {
      this.navigate(this.selectedPage)
    },
    async onSearchEnter() {
      const response = await API.get(`/search?text=${this.searchText}`)
      this.searchResults = response.data
    },
    onSearchResultSelected(page) {
      this.navigate(page)
      this.searchText = null
      this.searchResults = []
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

.search-input {
  font-size: medium;
  min-width: 300px;
}

label {
  font-weight: bold;
  padding-right: 5px;
}

.search-results-container {
  margin-top: 10px;
  padding: 10px;
  border: 1px solid black;
}

.search-results-item {
  margin: 10px;
}
</style>
