namespace Blaczko.Core.Utils
{
	/// <summary>
	/// Use this class to run tasks with a delay between them (to implement rate limiting, for example)
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public class DelayedExecutor<TResult>
	{
		private TimeSpan executionDelay;

		private bool waitForFinish;

		private TaskCompletionSource<bool> canExecuteNext;

		private Queue<EnqueuedTask> taskQueue = new();

		private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

		/// <param name="executionDelay">How much time should pass between executions</param>
		/// <param name="waitForFinish">
		///		If set to false, the delay will apply between starting executions.
		///		If set to true, the delay will apply between the finish of the last, and start of the next.
		///	</param>
		public DelayedExecutor(TimeSpan executionDelay, bool waitForFinish = false)
		{
			this.executionDelay = executionDelay;
			this.waitForFinish = waitForFinish;

			canExecuteNext = new();
			canExecuteNext.SetResult(true);
		}

		/// <summary>
		/// It will enqueue the provided task, execute it when possible, and return the result
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		public async Task<TResult> Execute(Func<Task<TResult>> task)
		{
			var newElement = new EnqueuedTask
			{
				TaskToExecute = task,
				TaskCompletionSource = new TaskCompletionSource<TResult>()
			};

			taskQueue.Enqueue(newElement);
			RequestExecution();

			return await newElement.TaskCompletionSource.Task;
		}

		/// <summary>
		/// Waits for the last execution delay to finish, thens starts a new execution
		/// </summary>
		private void RequestExecution()
		{
			_ = Task.Run(async () =>
			{
				try
				{
					await semaphoreSlim.WaitAsync();

					await canExecuteNext.Task;
					canExecuteNext = new TaskCompletionSource<bool>();
					await ExecuteNext();
				}
				finally
				{
					semaphoreSlim.Release();
				}
			});
		}

		/// <summary>
		/// Dequeues an element, runs it, sets the result, starts the execution delay task
		/// </summary>
		/// <returns></returns>
		private async Task ExecuteNext()
		{
			if (taskQueue.Count == 0)
			{
				return;
			}

			var nextTask = taskQueue.Dequeue();

			if(!waitForFinish)
			{
				_ = Task.Run(async () => await RunDelay());
			}

			var result = await nextTask.TaskToExecute();

			if (waitForFinish)
			{
				_ = Task.Run(async () => await RunDelay());
			}

			nextTask.TaskCompletionSource.SetResult(result);
		}

		private async Task RunDelay()
		{
			await Task.Delay(executionDelay);
			canExecuteNext.SetResult(true);
		}

		internal class EnqueuedTask
		{
			internal TaskCompletionSource<TResult> TaskCompletionSource;
			internal Func<Task<TResult>> TaskToExecute;
		}
	}
}
