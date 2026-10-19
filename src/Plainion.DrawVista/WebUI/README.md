# Plainion.DrawVista (webui)

Diagram Wiki

## Requirements

- Node.js 22.13 or newer in a supported even-numbered release (tested with 22.18.0)
- pnpm 11 or newer (tested with 11.25.0)

The project uses the installed Node.js and pnpm; it does not download or replace either tool.

## Install the dependencies

```sh
pnpm install --frozen-lockfile
```

Installation runs `quasar prepare` to generate the TypeScript configuration and declarations.

### Start the app in development mode (hot-code reloading, error reporting, etc.)
```sh
pnpm dev
```


### Lint the files
```sh
pnpm lint
```


### Format the files
```sh
pnpm format
```



### Build the app for production
```sh
pnpm build
```

The production files are written to `../wwwroot`. Release .NET builds generate and include these assets, and fail if the frontend build fails.

### Validate the app

```sh
pnpm check
```

This runs ESLint, Vue/TypeScript checking, and the dependency compatibility tests.

### Dependency compatibility

- `@quasar/app-vite` is pinned to 2.5.3: newer releases enforce Node.js 22.22 or newer at runtime, even where their package metadata allows earlier Node.js 22 versions.
- TypeScript stays on 6.0 because the current TypeScript ESLint packages do not support TypeScript 7.
- The `archiver-utils>glob` override replaces the remaining deprecated glob 10 dependency with maintained glob 13. The tests cover file expansion and ZIP creation.
- The pnpm patch for `vue-svg-pan-zoom` exposes its existing TypeScript declarations through its package exports, without changing its runtime code.
- Dependency build scripts are explicitly approved in `pnpm-workspace.yaml`; no global pnpm configuration is required.

### Customize the configuration
See [Configuring quasar.config](https://quasar.dev/quasar-cli-vite/quasar-config-file).
