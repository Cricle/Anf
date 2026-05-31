use std::sync::atomic::{AtomicBool, Ordering};
use tokio_util::sync::CancellationToken;

/// DownloadManager: trait for managing download task execution
pub trait DownloadManager: Send + Sync {
    fn start(&self);
    fn stop(&self);
    fn is_running(&self) -> bool;
}

/// QueueDownloadManager: simple queue-based download manager
pub struct QueueDownloadManager {
    running: AtomicBool,
    token: CancellationToken,
}

impl QueueDownloadManager {
    pub fn new() -> Self {
        Self {
            running: AtomicBool::new(false),
            token: CancellationToken::new(),
        }
    }
}

impl Default for QueueDownloadManager {
    fn default() -> Self {
        Self::new()
    }
}

impl DownloadManager for QueueDownloadManager {
    fn start(&self) {
        self.running.store(true, Ordering::Relaxed);
    }
    fn stop(&self) {
        self.running.store(false, Ordering::Relaxed);
        self.token.cancel();
    }
    fn is_running(&self) -> bool {
        self.running.load(Ordering::Relaxed)
    }
}
