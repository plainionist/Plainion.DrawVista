import axios from 'axios'

const hostURL = process.env.VUE_APP_BASE_URL ? process.env.VUE_APP_BASE_URL : window.location.origin

const API = axios.create({
  withCredentials: true,
  baseURL: hostURL
})

export default API
