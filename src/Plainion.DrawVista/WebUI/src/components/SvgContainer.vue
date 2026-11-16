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
        @created="
          panZoom = $event;
          fitImage($event);
        "
      >
        <div v-html="svg"></div>
      </SvgPanZoom>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { SvgPanZoom } from 'vue-svg-pan-zoom';
import { onMounted, onUnmounted, type Ref, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';
import type { AxiosResponse } from 'axios';
import {
  applyLayerVisibility,
  readSvgLayers,
  restoreLayerPreferences,
  saveLayerPreferences,
  type DiagramLayer,
} from './svgLayers';

const $q = useQuasar();
const { t } = useI18n();
const router = useRouter();

const svg: Ref<string> = ref('');
const svgContainer = ref<HTMLElement>();
const emit = defineEmits<{ layersChanged: [layers: DiagramLayer[]] }>();
let activeLayers: DiagramLayer[] = [];
let activePage: string | undefined;
let loadSequence = 0;
type PanZoomInstance = {
  resize(): void;
  center(): void;
  zoomBy(scale: number): void;
  getSizes(): {
    width: number;
    height: number;
    realZoom: number;
    viewBox: { width: number; height: number };
  };
};
let panZoom: PanZoomInstance | undefined;
let resizeObserver: ResizeObserver | undefined;

const props = defineProps({
  page: String,
});

watch(
  () => props.page,
  (newPage, oldPage) => {
    if (newPage == oldPage) {
      return;
    }
    void updateSvg(newPage);
  },
);

onMounted(() => {
  const container = svgContainer.value;
  if (!container) {
    throw new Error('Diagram canvas is not mounted');
  }
  resizeObserver = new ResizeObserver(() => {
    const svgElement = svgContainer.value?.querySelector('svg');
    if (svgElement) {
      sizeSvg(svgElement);
      if (panZoom) {
        panZoom.resize();
        fitImage(panZoom);
      }
    }
  });
  resizeObserver.observe(container);
  void updateSvg(props.page);
});

onUnmounted(() => {
  resizeObserver?.disconnect();
  loadSequence++;
});

async function updateSvg(page: string | undefined) {
  const sequence = ++loadSequence;
  emit('layersChanged', []);
  try {
    const response: AxiosResponse<string> = page
      ? await api.get('/svg', { params: { pageName: page } })
      : await api.get('/startPage');
    if (sequence === loadSequence) {
      setSvg(response.data, page);
    }
  } catch {
    if (sequence !== loadSequence) {
      return;
    }
    $q.notify({
      color: 'negative',
      position: 'top',
      message: t('LOADING_PAGE_FAILED'),
      icon: 'report_problem',
    });
  }
}

function setSvg(pageContent: string, page: string | undefined): void {
  const parser = new DOMParser();
  const svgDoc = parser.parseFromString(pageContent, 'image/svg+xml');
  const svgElement = svgDoc.documentElement;
  if (svgElement.localName !== 'svg' || svgDoc.querySelector('parsererror')) {
    throw new Error('Invalid diagram SVG');
  }
  activeLayers = readSvgLayers(svgElement);
  activePage = page;
  if (page && activeLayers.length > 0) {
    try {
      activeLayers = restoreLayerPreferences(
        page,
        activeLayers,
        window.sessionStorage,
      );
    } catch (error) {
      notifyPreferenceFailure(error);
    }
  }
  applyLayerVisibility(svgElement, activeLayers);
  sizeSvg(svgElement);
  svgElement.querySelectorAll('a').forEach((link) => {
    link.setAttribute('target', '_blank');
  });

  svgElement.querySelectorAll('[onclick]').forEach((element) => {
    const match = /^window\.hook\.navigate\('(.*)'\)$/.exec(
      element.getAttribute('onclick') ?? '',
    );
    if (!match?.[1]) {
      return;
    }
    const link = svgDoc.createElementNS(element.namespaceURI, 'a');
    link.setAttribute(
      'href',
      router.resolve({ path: '/', query: { page: match[1] } }).href,
    );
    element.removeAttribute('onclick');
    element.replaceWith(link);
    link.appendChild(element);
  });
  svg.value = svgElement.outerHTML;
  emit('layersChanged', activeLayers);
}

function setLayerVisibility(id: string, visible: boolean): void {
  const element = svgContainer.value?.querySelector('svg');
  if (!element || !activeLayers.some((layer) => layer.id === id)) {
    throw new Error(`Diagram layer is not mounted: ${id}`);
  }
  activeLayers = activeLayers.map((layer) =>
    layer.id === id ? { ...layer, visible } : layer,
  );
  applyLayerVisibility(element, activeLayers);
  emit('layersChanged', activeLayers);
  if (activePage) {
    try {
      saveLayerPreferences(activePage, activeLayers, window.sessionStorage);
    } catch (error) {
      notifyPreferenceFailure(error);
    }
  }
}

function notifyPreferenceFailure(error: unknown): void {
  console.error('Layer preferences could not be restored or saved', error);
  $q.notify({
    color: 'negative',
    position: 'top',
    message: t('LAYER_PREFERENCES_FAILED'),
    icon: 'report_problem',
  });
}

defineExpose({ setLayerVisibility });

function sizeSvg(svgElement: Element): void {
  const container = svgContainer.value;
  if (!container) {
    throw new Error('Diagram canvas is not mounted');
  }
  svgElement.setAttribute('height', `${container.clientHeight}`);
  svgElement.setAttribute('width', `${container.clientWidth}`);
}

function fitImage(instance: PanZoomInstance): void {
  const margin = 20;
  const { width, height, viewBox, realZoom } = instance.getSizes();
  const scale = Math.min(
    Math.max(1, width - 2 * margin) / viewBox.width,
    Math.max(1, height - 2 * margin) / viewBox.height,
  );
  instance.zoomBy(scale / realZoom);
  instance.center();
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
