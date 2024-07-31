<template>
  <div class="svg-container full-width full-height" ref="svgContainer">
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
</template>

<script setup lang="ts">
import { SvgPanZoom } from 'vue-svg-pan-zoom';
import { onMounted, Ref, ref, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useQuasar } from 'quasar';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';

const $q = useQuasar();
const { t } = useI18n();
const router = useRouter();

const svg: Ref<string> = ref('');
const svgContainer = ref();
    
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
  updateSvg(undefined);
});

function updateSvg(page: string | undefined) {
  if (!page)
  {
    api.get('/startPage')
    .then((response) => {
      setSvg(response.data, true);
    })
    .catch(() => {
      $q.notify({
        color: 'negative',
        position: 'top',
        message: t('LOADING_PAGE_FAILED'),
        icon: 'report_problem'
      });
      })
      return;
  }

  api.get(`/svg?pageName=${page}`)
  .then((response) => {
    setSvg(response.data, false);
  })
  .catch(() => {
    $q.notify({
      color: 'negative',
      position: 'top',
      message: t('LOADING_PAGE_FAILED'),
      icon: 'report_problem'
    });
    })
}

function setSvg(pageContent: string, startPage: boolean): void {
  const parser = new DOMParser();
  const svgDoc = parser.parseFromString(pageContent, 'image/svg+xml');
  const svgElement = svgDoc.documentElement;
  svgElement.setAttribute(
    'height',
    svgContainer.value.offsetHeight - 30 + ''
  );
  if(startPage) {
    svgElement.setAttribute(
      'width',
      svgContainer.value.offsetWidth - 30 + ''
    );
  }

  svg.value = TransformToLinks(svgElement.outerHTML);
}

function TransformToLinks(svgWithLegacyUiLinks: string): string
{
  const replacement = `<a href="${router.resolve('/').href}?page=$3"><$1 $2 $4>$5$6</a>`;
  const pattern = /<(.+) (.*) onclick="window\.hook\.navigate\('(.*)'\)" ?(.*)>(.*)(<\/\1>)/g;
  const result: string = svgWithLegacyUiLinks.replace(pattern, replacement);
  return result;
}
</script>

<style>
.svg-container {
  margin-top: 10px;
  padding: 10px;

  border: 1px solid black;
  background-color: white;
}
</style>