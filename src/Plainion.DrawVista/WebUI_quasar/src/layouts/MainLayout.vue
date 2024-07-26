<template>
  <q-layout view="hHh lpR fFf">
    <q-header elevated class="bg-primary text-white" height-hint="98">
      <q-toolbar>
        <q-toolbar-title>
          <q-avatar>
            <img src="https://cdn.quasar.dev/logo-v2/svg/logo-mono-white.svg">
          </q-avatar>
          Plainion.DrawVista
        </q-toolbar-title>
      </q-toolbar>

      <q-tabs align="left">
        <q-route-tab v-for="route in routes" :key="route.name" :to="route.path" :label="route.name?.toString() ?? 'undefined'" />
      </q-tabs>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>

<script setup lang="ts">
import { routes } from 'src/router/routes';

</script>
