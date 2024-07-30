<template>
  <q-page class="window-height window-width justify-center content-start row q-pa-md">
    <div class="row full-width">
      <q-select filled class="col q-ma-md" v-model="selectedPage" :options="pages" :label="$t('SELECT_PAGE')" />
      <q-input filled class="col q-ma-md" v-model="searchFor" :label="$t('SEARCH')" />
    </div>
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

watch(selectedPage, (newSelection) => {
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
