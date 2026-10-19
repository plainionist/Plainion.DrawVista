<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="bg-primary text-white" height-hint="38">
      <q-toolbar class="tools-toolbar">
        <q-tabs dense shrink align="left" class="tools-navigation">
          <q-route-tab v-for="route in routes" :key="route.name" :to="route.path" :label="route.name?.toString() ?? 'undefined'" />
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
import { routes } from 'src/router/routes';
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
