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
      <q-btn-dropdown
        dense
        no-caps
        class="layers-button"
        :label="$t('LAYERS')"
        :aria-label="$t('LAYERS')"
        :disable="layers.length <= 1"
      >
        <q-list dense>
          <q-item v-for="layer in layers" :key="layer.id">
            <q-item-section>
              <q-checkbox
                :model-value="layer.visible"
                :label="layer.name"
                @update:model-value="
                  svgView?.setLayerVisibility(layer.id, $event)
                "
              />
            </q-item-section>
          </q-item>
        </q-list>
      </q-btn-dropdown>
    </Teleport>
    <SearchResults :search-string="searchFor" />
    <SvgContainer
      ref="svgView"
      :page="selectedPage"
      @layers-changed="layers = $event"
    />
  </q-page>
</template>

<script setup lang="ts">
import { onMounted, type Ref, ref, watch } from 'vue';
import { useQuasar } from 'quasar';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';
import SearchResults from 'src/components/SearchResults.vue';
import SvgContainer from 'src/components/SvgContainer.vue';
import type { DiagramLayer } from 'src/components/svgLayers';

const $q = useQuasar();
const { t } = useI18n();
const router = useRouter();
const route = useRoute();

const pages: Ref<string[]> = ref([]);
const selectedPage: Ref<string | undefined> = ref('');
const searchFor: Ref<string> = ref('');
const layers = ref<DiagramLayer[]>([]);
const svgView = ref<InstanceType<typeof SvgContainer>>();

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
    query: { page: newSelection },
  });
});

watch(route, (newRoute) => {
  selectedPage.value = newRoute.query.page?.toString();
  if (!selectedPage.value) {
    loadPages();
  }
});

function loadPages() {
  api
    .get<string[]>('/pageNames')
    .then((response) => {
      pages.value = response.data;
    })
    .catch(() => {
      $q.notify({
        color: 'negative',
        position: 'top',
        message: t('LOADING_PAGES_FAILED'),
        icon: 'report_problem',
      });
    });
}

onMounted(() => {
  loadPages();
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

.layers-button {
  flex-shrink: 0;
  min-height: 32px;
  font-size: 12px;
}
</style>
