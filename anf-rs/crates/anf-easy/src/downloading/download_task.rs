use std::sync::atomic::{AtomicBool, AtomicI32, Ordering};
use tokio::sync::Notify;
use tokio_util::sync::CancellationToken;

/// DownloadTask: tracks progress through a series of download items
pub struct DownloadTask {
    item_count: i32,
    position: AtomicI32,
    pub token: CancellationToken,
    done: AtomicBool,
    notify: Notify,
}

impl DownloadTask {
    pub fn new(item_count: usize, token: CancellationToken) -> Self {
        Self {
            item_count: item_count as i32,
            position: AtomicI32::new(-1),
            token,
            done: AtomicBool::new(false),
            notify: Notify::new(),
        }
    }

    pub fn position(&self) -> i32 {
        self.position.load(Ordering::Relaxed)
    }
    pub fn item_count(&self) -> i32 {
        self.item_count
    }
    pub fn is_done(&self) -> bool {
        self.done.load(Ordering::Relaxed)
    }

    pub fn advance(&self) -> i32 {
        let pos = self.position.fetch_add(1, Ordering::SeqCst) + 1;
        if pos >= self.item_count {
            self.done.store(true, Ordering::Relaxed);
            self.notify.notify_waiters();
        }
        pos
    }

    pub async fn wait_done(&self) {
        if self.is_done() {
            return;
        }
        self.notify.notified().await;
    }
}
