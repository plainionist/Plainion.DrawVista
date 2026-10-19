<template>
  <div class="svg-container full-width" ref="svgContainer">
    <transition name="scale" mode="out-in">
      <SvgPanZoom
        :key="svg"
        style="width: 100%; height: 100%"
        :zoomEnabled="true"
        :controlIconsEnabled="false"
        :fit="true"
        :center="true"
        :minZoom="0.25"
        @created="panZoom = $event"
      >
        <div v-html="svg"></div>
      </SvgPanZoom>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { SvgPanZoom } from 'vue-svg-pan-zoom';
import { onMounted, onUnmounted, Ref, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';
import { AxiosResponse } from 'axios';

const $q = useQuasar();
const { t } = useI18n();
const router = useRouter();

const svg: Ref<string> = ref('');
const svgContainer = ref<HTMLElement>();
let panZoom: { resize(): void; fit(): void; center(): void } | undefined;
let resizeObserver: ResizeObserver | undefined;
    
const props =  defineProps({
  page: String,
});

watch(() => props.page, (newPage, oldPage) => {
  if (newPage == oldPage) {
      return;
  }
  updateSvg(newPage);
});

onMounted(() => {
  const container = svgContainer.value;
  if (!container) {
    throw new Error('Diagram canvas is not mounted');
  }
  resizeObserver = new ResizeObserver(() => {
    const svgElement = svgContainer.value?.querySelector('svg');
    if (svgElement) {
      sizeSvg(svgElement);
      panZoom?.resize();
      panZoom?.fit();
      panZoom?.center();
    }
  });
  resizeObserver.observe(container);
  updateSvg(undefined);
});

onUnmounted(() => resizeObserver?.disconnect());

async function updateSvg(page: string | undefined) {
  if (!page)
  {
    try {
      const response: AxiosResponse = await api.get('/startPage');
      setSvg(response.data);
    }
    catch {
      $q.notify({
        color: 'negative',
        position: 'top',
        message: t('LOADING_PAGE_FAILED'),
        icon: 'report_problem'
      });
    }
    return;
  }

  try {
    const response: AxiosResponse = await api.get(`/svg?pageName=${page}`);
    setSvg(response.data);
  }
  catch {
    $q.notify({
      color: 'negative',
      position: 'top',
      message: t('LOADING_PAGE_FAILED'),
      icon: 'report_problem'
    });
  }
}

function setSvg(pageContent: string): void {
  const parser = new DOMParser();
  const svgDoc = parser.parseFromString(pageContent, 'image/svg+xml');
  const svgElement = svgDoc.documentElement;
  sizeSvg(svgElement);

  svg.value = TransformToLinks(svgElement.outerHTML);
}

function sizeSvg(svgElement: Element): void {
  const container = svgContainer.value;
  if (!container) {
    throw new Error('Diagram canvas is not mounted');
  }
  svgElement.setAttribute('height', `${container.clientHeight}`);
  svgElement.setAttribute('width', `${container.clientWidth}`);
}

function TransformToLinks(svgWithLegacyUiLinks: string): string
{
  const replacement = `<a href="${router.resolve('/').href}?page=$3"><$1 $2 $4>$5$6</a>`;
  const pattern = /<(.+) (.*) onclick="window\.hook\.navigate\('(.*)'\)" ?(.*)>(.*)(<\/\1>)/g;
  var result: string = svgWithLegacyUiLinks.replace(pattern, replacement);
  return result;
}
</script>

<style>
.svg-container {
  flex: 1;
  min-height: 0;

  border: 1px solid black;
  background-color: white;
}

.svg-container svg {
  display: block;
}
</style>