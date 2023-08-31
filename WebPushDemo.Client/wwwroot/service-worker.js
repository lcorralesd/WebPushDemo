self.addEventListener('install', async event => {
    console.log('Instalando el service worker...');
    self.skipWaiting();
});

self.addEventListener('fetch', event => {
    return null;

});

self.addEventListener('push', event => {
    const payload = event.data.json();
    console.log('Notificación recibida', payload);
    const options = {
        body: payload.body,
        icon: 'img/icon.png',
        vibrate: [100, 50, 100],
        data: { primaryKey: 1 },
        actions: [
            { action: 'explore', title: 'Explorar este nuevo mundo',
                icon: 'img/checkmark.png' },
            { action: 'close', title: 'Cerrar',
                icon: 'img/xmark.png' },
        ]
    };
    event.waitUntil(
        self.registration.showNotification(payload.title, options)
    );

    //event.waitUntil(
    //    self.registration.showNotification("Mesnaje importante", {
    //        body: payload.message,
    //        icon: 'icon-192.png',
    //        vibrate: [100, 50, 100],
    //        data: { primaryKey: 1 }
    //    }
    //    )
    //);
});

self.addEventListener('notificationclick', event => {
    var navigateTo = event.notification.data.url;
    event.waitUntil(
        clients.openWindow(navigateTo)
    );
});