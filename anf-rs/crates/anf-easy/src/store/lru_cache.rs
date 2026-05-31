use lru::LruCache;
use std::hash::Hash;
use std::num::NonZeroUsize;
use std::sync::Mutex;

/// Thread-safe LRU cache wrapper
/// Matches C# LruCacher<TKey, TValue>
pub struct LruCacher<K: Hash + Eq + Clone, V: Clone> {
    inner: Mutex<LruCache<K, V>>,
}

impl<K: Hash + Eq + Clone, V: Clone> LruCacher<K, V> {
    pub fn new(max: usize) -> Self {
        Self {
            inner: Mutex::new(LruCache::new(NonZeroUsize::new(max).unwrap())),
        }
    }

    pub fn get(&self, key: &K) -> Option<V> {
        let mut cache = self.inner.lock().unwrap();
        cache.get(key).cloned()
    }

    pub fn put(&self, key: K, value: V) {
        let mut cache = self.inner.lock().unwrap();
        cache.put(key, value);
    }

    pub fn get_or_insert_with<F: FnOnce() -> V>(&self, key: K, f: F) -> V {
        let mut cache = self.inner.lock().unwrap();
        if let Some(v) = cache.get(&key) {
            return v.clone();
        }
        let v = f();
        cache.put(key, v.clone());
        v
    }

    pub fn contains(&self, key: &K) -> bool {
        let cache = self.inner.lock().unwrap();
        cache.contains(key)
    }

    pub fn remove(&self, key: &K) -> Option<V> {
        let mut cache = self.inner.lock().unwrap();
        cache.pop(key)
    }

    pub fn clear(&self) {
        let mut cache = self.inner.lock().unwrap();
        cache.clear();
    }

    pub fn len(&self) -> usize {
        let cache = self.inner.lock().unwrap();
        cache.len()
    }

    pub fn is_empty(&self) -> bool {
        let cache = self.inner.lock().unwrap();
        cache.is_empty()
    }
}
