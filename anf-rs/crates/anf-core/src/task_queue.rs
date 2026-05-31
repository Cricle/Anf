use std::future::Future;
use std::pin::Pin;

type BoxFuture<T> = Pin<Box<dyn Future<Output = T> + Send>>;

/// Run multiple async tasks with a concurrency limit.
/// Matches C# TaskQuene.RunAsync
pub async fn run_concurrent<T: Send + 'static>(
    tasks: Vec<BoxFuture<T>>,
    concurrent: usize,
) -> Vec<T> {
    use futures::stream::{self, StreamExt};

    stream::iter(tasks)
        .buffer_unordered(concurrent)
        .collect()
        .await
}

/// Run multiple async tasks concurrently, ignoring results.
pub async fn run_concurrent_void(tasks: Vec<BoxFuture<()>>, concurrent: usize) {
    run_concurrent(tasks, concurrent).await;
}
