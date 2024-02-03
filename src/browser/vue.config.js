const path = require('path')

const { defineConfig } = require('@vue/cli-service')

module.exports = defineConfig({
  transpileDependencies: true,
  configureWebpack: {
    module: {
      rules: [
        {
          test: /\.svg$/,
          include: [
            path.resolve(__dirname, "src/assets")
          ],
          loader: 'html-loader'
        }
      ]
    }
  }
})
