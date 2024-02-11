const path = require('path')

const { defineConfig } = require('@vue/cli-service')

module.exports = defineConfig({
  outputDir: '../wwwroot/',
  transpileDependencies: true,
  configureWebpack: {
    module: {
      rules: [
        {
          test: /\.svg$/,
          include: [path.resolve(__dirname, 'src/assets')],
          loader: 'raw-loader',
          type: 'javascript/auto'
        }
      ]
    }
  }
})
