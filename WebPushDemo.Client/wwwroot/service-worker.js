self.addEventListener('install', async event => {
    console.log('Instalando el service worker...');
    self.skipWaiting();
});

self.addEventListener('fetch', event => {
    return null;

});