use crate::downloading::download_task::DownloadTask;
use tokio_util::sync::CancellationToken;

pub struct DownloadBox {
    pub task: DownloadTask,
    pub address: String,
    token_source: CancellationToken,
}

impl DownloadBox {
    pub fn new(task: DownloadTask, address: String) -> Self {
        let token_source = task.token.clone();
        Self {
            task,
            address,
            token_source,
        }
    }
    pub fn cancel(&self) {
        self.token_source.cancel();
    }
    pub fn is_cancelled(&self) -> bool {
        self.token_source.is_cancelled()
    }
}
