<template>
    <div v-if="searchResults.length > 0" class="search-results-container full-width">
      <div
        v-for="item in searchResults"
        v-bind:key="item.pageName"
        class="search-results-item"
      >
        <router-link :to="{ path: '/', query: { page: item.pageName }}" @click="clearSearch">{{ item.pageName }}</router-link>
        &#8680;
        <span v-for="caption in item.captions" v-bind:key="caption">
          "{{ caption }}"
        </span>
      </div>
    </div>
</template>

<script setup lang="ts">
import { onMounted, Ref, ref } from 'vue';
import { api } from 'src/boot/axios';
import { onUnmounted } from 'vue';
import { AxiosResponse } from 'axios';
import { useQuasar } from 'quasar';
import { useI18n } from 'vue-i18n';

const $q = useQuasar();
const { t } = useI18n();

let interval: NodeJS.Timeout;
let lastSearchString: string | undefined;

const searchResults: Ref<{
            pageName: string,
            captions: string[]
        }[]> = ref([]);
    
const props = defineProps({
    searchString: String
});

onMounted(() => {
  interval = setInterval(() => {
    if (lastSearchString == props.searchString)
    {
      return;
    }
    lastSearchString = props.searchString;
    if (!props.searchString) {
      searchResults.value = [];
      return;
    }
    retrieveSearchResults();
  }, 250);
});

onUnmounted(() => clearInterval(interval));

async function retrieveSearchResults(): Promise<void> {
  try {
    const response: AxiosResponse = await api.get(`/search?text=${props.searchString}`);
    searchResults.value = response.data;
  }
  catch {
    $q.notify({
      color: 'negative',
      position: 'top',
      message: t('LOADING_EARCH_RESULTS_FAILED'),
      icon: 'report_problem'
    });
  }
}

function clearSearch(): void {
  lastSearchString = '';
  searchResults.value = [];
}
</script>

<style>
.search-results-container {
  margin-top: 10px;
  padding: 10px;

  border: 1px solid black;
  background-color: white;
}
</style>