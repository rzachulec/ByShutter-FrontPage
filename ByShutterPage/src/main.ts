import { createApp } from 'vue'
import '@fontsource-variable/geist' // self-hosted (OFL); latin + latin-ext subsets are fetched on demand
import './style.css'
import App from './App.vue'
import router from './router'

createApp(App).use(router).mount('#app')
