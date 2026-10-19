<template>
  <q-page class="browser-page" :style-fn="pageStyle">
    <Teleport to="#browser-tools">
      <q-select
        filled
        dense
        options-dense
        hide-bottom-space
        class="bg-white rounded-borders"
        v-model="selectedPage"
        :options="pages"
        :display-value="selectedPage || $t('SELECT_PAGE')"
        :aria-label="$t('SELECT_PAGE')"
      />
      <q-input
        filled
        dense
        hide-bottom-space
        class="bg-white rounded-borders"
        v-model="searchFor"
        :placeholder="$t('SEARCH')"
        :aria-label="$t('SEARCH')"
      />
    </Teleport>
    <SearchResults :search-string="searchFor" />
    <SvgContainer :page="selectedPage" />
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, Ref, ref, watch } from 'vue';
import { useQuasar } from 'quasar';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';
import SearchResults from 'src/components/SearchResults.vue';
import SvgContainer from 'src/components/SvgContainer.vue';

const $q = useQuasar();
const { t } = useI18n();
const router = useRouter();
const route = useRoute();

const pages: Ref<string[]> = ref([]);
const selectedPage: Ref<string | undefined> = ref('');
const searchFor: Ref<string> = ref('');

function pageStyle(offset: number, height: number) {
  return { height: `${height - offset}px` };
}

watch(selectedPage, (newSelection) => {
  searchFor.value = '';
  if (!newSelection) {
    return;
  }
  router.push({
    path: '/',
    query: { page: newSelection}
  })
})

watch(route, (newRoute) => {
  selectedPage.value = newRoute.query.page?.toString();
})

onMounted(() => {
  api.get('/pageNames')
    .then((response) => {
      pages.value = response.data;
    })
    .catch(() => {
      $q.notify({
        color: 'negative',
        position: 'top',
        message: t('LOADING_PAGES_FAILED'),
        icon: 'report_problem'
      });
     })
  selectedPage.value = route.query.page?.toString();
});
</script>

<style>
.browser-page {
  display: flex;
  flex-direction: column;
  gap: 3px;
  padding: 3px;
}

.browser-page > .search-results-container {
  flex-shrink: 0;
  max-height: 30%;
  overflow: auto;
  margin: 0;
  padding: 3px;
}

.q-item--active {
  color: black;
}
</style>