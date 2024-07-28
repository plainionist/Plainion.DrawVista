<template>
  <q-page class="window-width justify-center content-start row q-pa-xl">
    <q-file
      v-model="filesToUpload"
      :label="$t('CLICK_OR_DROP_TO_UPLOAD')"
      filled
      multiple
      class="full-width full-height row filedrop"
    >
    </q-file>
    <q-btn @click="submit" :disable="requestInProgress" class="q-mt-md full-width">{{$t('UPLOAD_BTN')}}</q-btn>
  </q-page>
</template>

<script setup lang="ts">
import { Ref, ref } from 'vue';
import { useQuasar } from 'quasar';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';

const $q = useQuasar();
const { t } = useI18n();
const router = useRouter();

const requestInProgress: Ref<boolean> = ref(false);
const filesToUpload: Ref<File[]> = ref([]);

function submit() {
  requestInProgress.value = true;

  const formData = new FormData();
  filesToUpload.value.forEach((item) => formData.append(item.name, item));

  const headers = { 'Content-Type': 'multipart/form-data' }
  
  api.post('/upload', formData, { headers })
    .then(() => {
      requestInProgress.value = false;
      $q.notify({
        color: 'positive',
        position: 'top',
        message: t('UPLOAD_SUCCESS'),
        icon: 'check'
      });
      router.push('/');
    })
    .catch(() => {
      requestInProgress.value = false;
      $q.notify({
        color: 'negative',
        position: 'top',
        message: t('UPLOAD_FAILED'),
        icon: 'report_problem'
      });
     })
}
</script>

<style>
.filedrop .q-field__inner {
  height: 500px; 
}
.filedrop .q-field__control {
  height: 100% !important; 
}
</style>