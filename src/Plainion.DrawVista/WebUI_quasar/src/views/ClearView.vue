<template>
  <q-page class="window-width justify-center row q-pa-xl">
    <div>
      <q-btn @click="clear()" :disable="requestInProgress" class="q-pa-xl">{{$t('CLEAR_BTN')}}</q-btn>
    </div>
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

let requestInProgress: Ref<boolean> = ref(false);

function clear() {
  requestInProgress.value = true;

  api.post('/clear')
    .then(() => {
      requestInProgress.value = false;
      router.push('/');
    })
    .catch(() => {
      $q.notify({
        color: 'negative',
        position: 'top',
        message: t('CLEAR_FAILED'),
        icon: 'report_problem'
      });
      requestInProgress.value = false;
     })
}
</script>
