const CACHE_NAME = 'tank-assault-v9';
const APP_SHELL = [
  './',
  './index.html',
  './manifest.json',
  './models/t90a.glb',
  './models/t34.glb',
  './models/apache.glb',
  './models/canyon.glb',
  './icons/icon-192.png',
  './icons/icon-512.png',
  './sfx/shell_explosion.mp3',
  './sfx/tank_engine.mp3',
  './sfx/player_fire.mp3',
  './sfx/enemy_fire.mp3',
  './sfx/reload.mp3',
  './sfx/heli_engine.mp3'
];

self.addEventListener('install', function(event){
  event.waitUntil(
    caches.open(CACHE_NAME).then(function(cache){ return cache.addAll(APP_SHELL); })
  );
  self.skipWaiting();
});

self.addEventListener('activate', function(event){
  event.waitUntil(
    caches.keys().then(function(keys){
      return Promise.all(keys.filter(function(k){ return k !== CACHE_NAME; }).map(function(k){ return caches.delete(k); }));
    })
  );
  self.clients.claim();
});

// شبكة أولاً ثم ذاكرة تخزين مؤقت كاحتياط — يضمن ظهور آخر تحديث فوراً عند توفر الاتصال،
// مع الحفاظ على إمكانية اللعب دون اتصال باستخدام آخر نسخة محفوظة
self.addEventListener('fetch', function(event){
  if(event.request.method !== 'GET') return;
  event.respondWith(
    fetch(event.request).then(function(response){
      var copy = response.clone();
      caches.open(CACHE_NAME).then(function(cache){ cache.put(event.request, copy); });
      return response;
    }).catch(function(){ return caches.match(event.request); })
  );
});
