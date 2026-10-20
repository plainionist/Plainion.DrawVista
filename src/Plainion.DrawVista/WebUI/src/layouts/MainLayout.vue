<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="bg-primary text-white" height-hint="38">
      <q-toolbar class="tools-toolbar">
        <q-tabs dense shrink align="left" class="tools-navigation">
          <template v-for="navigationRoute in routes" :key="navigationRoute.name">
            <q-btn
              v-if="navigationRoute.name === 'clear'"
              flat
              dense
              class="delete-page-button"
              :label="$t('delete')"
              :disable="!currentPage || requestInProgress"
              :loading="requestInProgress"
              @click="confirmDelete"
            />
            <q-route-tab
              :to="navigationRoute.path"
              :label="navigationRoute.name?.toString() ?? 'undefined'"
            />
          </template>
        </q-tabs>
        <div id="browser-tools" class="browser-tools" />
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useQuasar } from 'quasar';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import { api } from 'src/boot/axios';
import { routes } from 'src/router/routes';

const $q = useQuasar();
const { t } = useI18n();
const route = useRoute();
const router = useRouter();
const requestInProgress = ref(false);
const currentPage = computed(() =>
  route.name === 'browse' && typeof route.query.page === 'string' ? route.query.page : undefined
);

function confirmDelete() {
  const pageName = currentPage.value;
  if (!pageName) {
    return;
  }

  $q.dialog({
    title: t('DELETE_TITLE'),
    message: t('DELETE_CONFIRMATION', { pageName }),
    color: 'dark',
    cancel: true,
    persistent: true
  }).onOk(() => {
    void deletePage(pageName);
  });
}

async function deletePage(pageName: string) {
  requestInProgress.value = true;
  try {
    await api.delete('/delete', { params: { pageName } });
    await router.replace({ name: 'browse' });
    $q.notify({
      color: 'positive',
      position: 'top',
      message: t('DELETE_SUCCESS', { pageName }),
      icon: 'check'
    });
  }
  catch {
    $q.notify({
      color: 'negative',
      position: 'top',
      message: t('DELETE_FAILED'),
      icon: 'report_problem'
    });
  }
  finally {
    requestInProgress.value = false;
  }
}
</script>

<style lang="scss">
.tools-toolbar {
  min-height: 38px;
  padding: 3px;
  gap: 6px;
  flex-wrap: nowrap;
}

.tools-navigation {
  flex-shrink: 0;

  .q-tab {
    min-height: 32px;
    padding: 0 10px;
  }

  .q-tab__label {
    font-size: 12px;
  }

  .delete-page-button {
    min-height: 32px;
    padding: 0 10px;
    font-size: 12px;
  }
}

.browser-tools {
  display: flex;
  flex: 1;
  min-width: 0;
  gap: 6px;

  .q-field.q-field--dense {
    flex: 1;
    min-width: 0;
    font-size: 12px;

    .q-field__control,
    .q-field__native,
    .q-field__marginal {
      height: 32px;
      min-height: 32px;
    }
  }
}
</style>
