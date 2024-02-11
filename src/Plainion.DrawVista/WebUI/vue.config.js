const path = require('path')

const { defineConfig } = require('@vue/cli-service')

switch(process.env.NODE_ENV){
  case 'production':
    process.env.VUE_APP_BASE_URL = ''
    break
  case 'development':
    process.env.VUE_APP_BASE_URL = 'http://localhost:5236'
    break
}

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
