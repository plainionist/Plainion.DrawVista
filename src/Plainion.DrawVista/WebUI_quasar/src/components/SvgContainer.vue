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
import { Ref, ref, watch } from 'vue';
import { useQuasar } from 'quasar';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';

const $q = useQuasar();
const { t } = useI18n();

const svg: Ref<string> = ref('');
const svgContainer = ref();
    
const props =  defineProps({
  page: String,
});

watch(() => props.page, (newPage, oldPage) => {
  if (newPage == oldPage) {
      return;
  }
  console.log(newPage);
  updateSvg(newPage);
})

function updateSvg(page: string | undefined) {
  if (!page)
  {
      //TODO: load startpage here later on   
      return;
  }

  api.get(`/svg?pageName=${page}`)
  .then((response) => {
    const pageContent = response.data;

    const parser = new DOMParser();
    const svgDoc = parser.parseFromString(pageContent, 'image/svg+xml');
    const svgElement = svgDoc.documentElement;
    svgElement.setAttribute(
      'height',
      svgContainer.value.offsetHeight - 30 + ''
    );

    svg.value = svgElement.outerHTML;
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

</script>