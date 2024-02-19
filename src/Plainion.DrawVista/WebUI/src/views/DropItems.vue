<template>
  <div
    :class="dropZoneStyle"
    @dragover="onDragOver"
    @dragleave="onDragLeave"
    @drop="onDrop"
  >
    <input
      type="file"
      multiple
      name="file"
      id="input"
      class="hide-content"
      @change="onChange"
      ref="file"
    />

    <label for="input" class="label">
      <div>Drop or <u>Click</u> to Upload!</div>
    </label>

    <div v-if="items.length > 0" class="items-container">
      <div v-for="item in items" :key="item.name" class="item">
        <div>
          <a href="#" @click="remove(item)" class="remove-button">[X]</a>
          {{ item.name }}
        </div>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'DropItems',
  data() {
    return {
      isDragging: false,
      items: []
    }
  },
  watch: {
    items() {
      console.log(this.items)
      this.$emit('itemsSelected', this.items)
    }
  },
  computed: {
    dropZoneStyle() {
      return ['dropzone', this.isDragging ? 'dropzone-active' : '']
    }
  },
  methods: {
    onDragOver(e) {
      e.preventDefault()
      this.isDragging = true
    },
    onDragLeave() {
      this.isDragging = false
    },
    onDrop(e) {
      e.preventDefault()
      this.isDragging = false
      this.items = [...e.dataTransfer.files]
    },
    onChange() {
      this.items = [...this.$refs.file.files]
    },
    remove(item) {
      const index = this.items.indexOf(item)
      this.items.splice(index, 1)
    }
  }
}
</script>

<style scoped>
.dropzone {
  padding: 2rem;
  background: whitesmoke;
  border: 1px solid lightgray;
  cursor: pointer;
}
.dropzone-active {
  background-color: lightgreen;
}
.hide-content {
  opacity: 0;
  overflow: hidden;
  position: absolute;
  width: 1px;
  height: 1px;
}
.label {
  font-size: large;
}
.items-container {
  margin-top: 2rem;
  display: flex;
  flex-wrap: wrap;
  align-items: flex-start;
}
.item {
  display: flex;
  background-color: lightgray;
  margin: 5px;
  padding: 7px;
}
.remove-button {
  margin-right: 5px;
}
</style>
