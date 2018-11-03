using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Debug;
using static UnityEngine.Application;

namespace TimeSlip.Await
{
    /// <summary>
    /// 将多线程上的方法在下一帧放在主线程上
    /// </summary>
    public class WaitForUpdate : CustomYieldInstruction
    {
        /// <summary>
        /// 将多线程上的方法在下一帧放在主线程上
        /// </summary>
        public WaitForUpdate() : base() { }

        public override bool keepWaiting
        {
            get { return false; }
        }
    }

    /// <summary>
    /// 将主线程上的方法在下一帧放在多线程上执行
    /// </summary>
    public class WaitForBackgroundThread
    {
        /// <summary>
        /// 将主线程上的方法在下一帧放在多线程上执行
        /// </summary>
        public WaitForBackgroundThread() { }

        public ConfiguredTaskAwaitable.ConfiguredTaskAwaiter GetAwaiter()
        {
            return Task.Run(() => { }).ConfigureAwait(false).GetAwaiter();
        }
    }

    /// <summary>
    /// 暂停多少秒，不受Time.timeScale影响
    /// </summary>
    public class WaitForRealSeconds
    {
        private int _milliseconds;

        /// <summary>
        /// 暂停多少秒，不受Time.timeScale影响
        /// </summary>
        public WaitForRealSeconds(float second)
        {
            if (second < 0)
            {
                LogError("暂停秒数必须大于0");
                second = 0f;
            }
            _milliseconds = Math.Min(int.MaxValue, (int)(second * 1000));
        }

        public TaskAwaiter GetAwaiter()
        {
            return Task.Delay(_milliseconds).GetAwaiter();
        }
    }

    /// <summary>
    /// 把当前方法放在主线程上执行
    /// </summary>
    public class WaitForAction
    {
        private Action _action;

        public Action GetAction() { return _action; }

        public WaitForAction(Action action)
        {
            _action = action;
        }
    }

    /// <summary>
    /// 把当前方法放在主线程上执行
    /// 带一个参数的
    /// </summary>
    public class WaitForAction<T>
    {
        private Action<T> _action;

        public Action<T> GetAction() { return _action; }
        private T _data;

        public T GetData() { return _data; }
        public WaitForAction(Action<T> action, T data)
        {
            _action = action;
            _data = data;
        }
    }

    /// <summary>
    /// 把当前方法放在主线程上执行
    /// 带一个返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WaitForFunc<T>
    {
        private Func<T> _func;

        public Func<T> GetFunc() { return _func; }

        public WaitForFunc(Func<T> func)
        {
            _func = func;
        }
    }

    /// <summary>
    /// 异步 扩展方法
    /// </summary>
    public static class AwaitExtensions
    {

        #region UnityEditor
#if UNITY_EDITOR

        [UnityEditor.Callbacks.DidReloadScripts]
        static void RunTimeCheck()
        {
            UnityEditor.ScriptingRuntimeVersion scriptingRuntimeVersion = UnityEditor.PlayerSettings.scriptingRuntimeVersion;
            if (scriptingRuntimeVersion != UnityEditor.ScriptingRuntimeVersion.Latest)
            {
                throw new Exception("当前运行时不知道Async和Await,请更换运行时！");
            }
            UnityEditor.ApiCompatibilityLevel apiCompatibilityLevel = UnityEditor.PlayerSettings.GetApiCompatibilityLevel(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
            if (apiCompatibilityLevel != UnityEditor.ApiCompatibilityLevel.NET_4_6)
            {
                Debug.LogWarning("Async和Await推荐环境为Net4.X");
            }
        }

#endif
        #endregion

        public static UnityAwaiter GetAwaiter(this YieldInstruction instruction)
        {
            return GetAwaiterReturnVoid(instruction);
        }

        #region 所有协程Wait整合成一个方法
        //public static UnityAwaiter GetAwaiter(this WaitForSeconds instruction)
        //{
        //    return GetAwaiterReturnVoid(instruction);
        //}

        //public static UnityAwaiter GetAwaiter(this WaitForUpdate instruction)
        //{
        //    return GetAwaiterReturnVoid(instruction);
        //}

        //public static UnityAwaiter GetAwaiter(this WaitForEndOfFrame instruction)
        //{
        //    return GetAwaiterReturnVoid(instruction);
        //}

        //public static UnityAwaiter GetAwaiter(this WaitForFixedUpdate instruction)
        //{
        //    return GetAwaiterReturnVoid(instruction);
        //}

        //public static UnityAwaiter GetAwaiter(this WaitForSecondsRealtime instruction)
        //{
        //    return GetAwaiterReturnVoid(instruction);
        //}

        //public static UnityAwaiter GetAwaiter(this WaitUntil instruction)
        //{
        //    return GetAwaiterReturnVoid(instruction);
        //}

        //public static UnityAwaiter GetAwaiter(this WaitWhile instruction)
        //{
        //    return GetAwaiterReturnVoid(instruction);
        //} 
        #endregion

#if UNITY_4 || UNITY_5
    // Return itself so you can do things like (await new WWW(url)).bytes
    public static UnityAwaiter<WWW> GetAwaiter(this WWW instruction)
    {
        return GetAwaiterReturnSelf(instruction);
    }
#endif

        public static UnityAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation instruction)
        {
            return GetAwaiterReturnSelf(instruction);
        }

        public static UnityAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest instruction)
        {
            var awaiter = new UnityAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ResourceRequest(awaiter, instruction)));
            return awaiter;
        }

        public static UnityAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction)
        {
            var awaiter = new UnityAwaiter<AssetBundle>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.AssetBundleCreateRequest(awaiter, instruction)));
            return awaiter;
        }

        public static UnityAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest instruction)
        {
            var awaiter = new UnityAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.AssetBundleRequest(awaiter, instruction)));
            return awaiter;
        }

        public static UnityAwaiter<T> GetAwaiter<T>(this IEnumerator<T> coroutine)
        {
            var awaiter = new UnityAwaiter<T>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                new CoroutineWrapper<T>(coroutine, awaiter).Run()));
            return awaiter;
        }

        public static UnityAwaiter<object> GetAwaiter(this IEnumerator coroutine)
        {
            var awaiter = new UnityAwaiter<object>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                new CoroutineWrapper<object>(coroutine, awaiter).Run()));
            return awaiter;
        }

        public static UnityAwaiter GetAwaiter(this WaitForAction waitForAction)
        {
            var awaiter = new UnityAwaiter();
            if (waitForAction.GetAction() != null)
            {
                RunOnUnityScheduler(waitForAction.GetAction());
            }
            awaiter.Complete(null);
            return awaiter;
        }

        public static UnityAwaiter GetAwaiter<T>(this WaitForAction<T> waitForAction)
        {
            var awaiter = new UnityAwaiter();
            if (waitForAction.GetAction() != null)
            {
                RunOnUnityScheduler(waitForAction.GetAction(), waitForAction.GetData());
            }
            awaiter.Complete(null);
            return awaiter;
        }

        public static UnityAwaiter<T> GetAwaiter<T>(this WaitForFunc<T> waitForFunc)
        {
            var awaiter = new UnityAwaiter<T>();
            if (waitForFunc.GetFunc() != null)
            {
                RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(InstructionWrappers.FuncGetReturn<T>(awaiter, waitForFunc.GetFunc())));
            }
            return awaiter;
        }

        /// <summary>
        /// 将任务转成协程
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
        }

        /// <summary>
        /// 将任务转成协程
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IEnumerator<T> AsIEnumerator<T>(this Task<T> task) where T : class
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
            if (task.IsFaulted)
            {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
            yield return task.Result;
        }

        public static Task StartTask(this MonoBehaviour mono, Action action)
        {
            return Task.Factory.StartNew(action);
        }

        public static Task StartTask(this MonoBehaviour mono, Action<object> action, object data)
        {
            return Task.Factory.StartNew(action, data);
        }

        public static Task<T> StartTask<T>(this MonoBehaviour mono, Func<T> action)
        {
            return Task.Factory.StartNew<T>(action);
        }

        public static Task<T> StartTask<T>(this MonoBehaviour mono, Func<object, T> action, object data)
        {
            return Task.Factory.StartNew<T>(action, data);
        }

        /// <summary>
        /// 断言
        /// </summary>
        /// <param name="condition"></param>
        static void Assert(bool condition)
        {
            if (!condition)
            {
                throw new Exception("Assert hit in Await!");
            }
        }

        /// <summary>
        /// 在主线程上执行某个方法
        /// </summary>
        /// <param name="action"></param>
        static void RunOnUnityScheduler(Action action, object data = null)
        {
            if (SynchronizationContext.Current == SyncContextUtil.UnitySynchronizationContext)
            {
                action();
            }
            else
            {
                SyncContextUtil.UnitySynchronizationContext.Post(_ => action(), data);
            }
        }

        /// <summary>
        /// 在主线程上执行某个方法
        /// </summary>
        /// <param name="action"></param>
        static void RunOnUnityScheduler<T>(Action<T> action, T data)
        {
            if (SynchronizationContext.Current == SyncContextUtil.UnitySynchronizationContext)
            {
                action(data);
            }
            else
            {
                SyncContextUtil.UnitySynchronizationContext.Post(_ => action(data), null);
            }
        }

        static UnityAwaiter GetAwaiterReturnVoid(object instruction)
        {
            var awaiter = new UnityAwaiter();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ReturnVoid(awaiter, instruction)));
            return awaiter;
        }

        static UnityAwaiter<T> GetAwaiterReturnSelf<T>(T instruction)
        {
            var awaiter = new UnityAwaiter<T>();
            RunOnUnityScheduler(() => AsyncCoroutineRunner.Instance.StartCoroutine(
                InstructionWrappers.ReturnSelf(awaiter, instruction)));
            return awaiter;
        }

        /// <summary>
        /// 自定义异步类
        /// </summary>
        public class UnityAwaiter : INotifyCompletion
        {
            bool _isDone;
            Exception _exception;
            Action _continuation;

            public bool IsCompleted
            {
                get { return _isDone; }
            }

            public void GetResult()
            {
                Assert(_isDone);

                if (_exception != null)
                {
                    ExceptionDispatchInfo.Capture(_exception).Throw();
                }
            }

            public void Complete(Exception e)
            {
                Assert(!_isDone);

                _isDone = true;
                _exception = e;

                //在多线程执行完毕后，根据需求回调主线程
                if (_continuation != null)
                {
                    RunOnUnityScheduler(_continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                Assert(_continuation == null);
                Assert(!_isDone);

                _continuation = continuation;
            }
        }

        /// <summary>
        /// 自定义异步类(泛型)
        /// </summary>
        public class UnityAwaiter<T> : INotifyCompletion
        {
            bool _isDone;
            Exception _exception;
            Action _continuation;
            T _result;

            public bool IsCompleted
            {
                get { return _isDone; }
            }

            public T GetResult()
            {
                Assert(_isDone);

                if (_exception != null)
                {
                    ExceptionDispatchInfo.Capture(_exception).Throw();
                }

                return _result;
            }

            public void Complete(T result, Exception e)
            {
                Assert(!_isDone);

                _isDone = true;
                _exception = e;
                _result = result;

                //在多线程执行完毕后，根据需求回调主线程
                if (_continuation != null)
                {
                    RunOnUnityScheduler(_continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation)
            {
                Assert(_continuation == null);
                Assert(!_isDone);

                _continuation = continuation;
            }
        }

        /// <summary>
        /// 原有协程的包装
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class CoroutineWrapper<T>
        {
            readonly UnityAwaiter<T> _awaiter;
            readonly Stack<IEnumerator> _processStack;

            public CoroutineWrapper(
                IEnumerator coroutine, UnityAwaiter<T> awaiter)
            {
                _processStack = new Stack<IEnumerator>();
                _processStack.Push(coroutine);
                _awaiter = awaiter;
            }

            public IEnumerator Run()
            {
                while (true)
                {
                    var topWorker = _processStack.Peek();

                    bool isDone;

                    try
                    {
                        isDone = !topWorker.MoveNext();
                    }
                    catch (Exception e)
                    {
                        //尝试输入异常信息
                        var objectTrace = GenerateObjectTrace(_processStack);

                        if (objectTrace.Any())
                        {
                            _awaiter.Complete(
                                default(T), new Exception(
                                    GenerateObjectTraceMessage(objectTrace), e));
                        }
                        else
                        {
                            _awaiter.Complete(default(T), e);
                        }

                        yield break;
                    }

                    if (isDone)
                    {
                        _processStack.Pop();

                        if (_processStack.Count == 0)
                        {
                            _awaiter.Complete((T)topWorker.Current, null);
                            yield break;
                        }
                    }

                    if (topWorker.Current is IEnumerator)
                    {
                        //如果还是协程，继续运行
                        _processStack.Push((IEnumerator)topWorker.Current);
                    }
                    else
                    {
                        //返回给Unity做处理
                        yield return topWorker.Current;
                    }
                }
            }

            string GenerateObjectTraceMessage(List<Type> objTrace)
            {
                var result = new StringBuilder();

                foreach (var objType in objTrace)
                {
                    if (result.Length != 0)
                    {
                        result.Append(" -> ");
                    }

                    result.Append(objType.ToString());
                }

                result.AppendLine();
                return "Unity Coroutine Object Trace: " + result.ToString();
            }

            static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> enumerators)
            {
                var objTrace = new List<Type>();

                foreach (var enumerator in enumerators)
                {
                    var field = enumerator.GetType().GetField("$this", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (field == null)
                    {
                        continue;
                    }

                    var obj = field.GetValue(enumerator);

                    if (obj == null)
                    {
                        continue;
                    }

                    var objType = obj.GetType();

                    if (!objTrace.Any() || objType != objTrace.Last())
                    {
                        objTrace.Add(objType);
                    }
                }

                objTrace.Reverse();
                return objTrace;
            }
        }

        static class InstructionWrappers
        {
            public static IEnumerator ReturnVoid(
                UnityAwaiter awaiter, object instruction)
            {
                // 暂且认定不会抛异常
                yield return instruction;
                awaiter.Complete(null);
            }

            public static IEnumerator AssetBundleCreateRequest(
                UnityAwaiter<AssetBundle> awaiter, AssetBundleCreateRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.assetBundle, null);
            }

            public static IEnumerator ReturnSelf<T>(
                UnityAwaiter<T> awaiter, T instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction, null);
            }

            public static IEnumerator FuncGetReturn<T>(UnityAwaiter<T> awaiter, Func<T> func)
            {
                yield return null;
                awaiter.Complete(func(), null);
            }

            public static IEnumerator AssetBundleRequest(
                UnityAwaiter<UnityEngine.Object> awaiter, AssetBundleRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }

            public static IEnumerator ResourceRequest(
                UnityAwaiter<UnityEngine.Object> awaiter, ResourceRequest instruction)
            {
                yield return instruction;
                awaiter.Complete(instruction.asset, null);
            }
        }
    }


    public class AsyncCoroutineRunner : MonoBehaviour
    {
        static AsyncCoroutineRunner _instance;

        public static AsyncCoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("AsyncCoroutineRunner")
                        .AddComponent<AsyncCoroutineRunner>();
                }
                return _instance;
            }
        }

        void Awake()
        {
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            Task.Factory.CancellationToken.WaitHandle.Close();
        }
    }

    public static class SyncContextUtil
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            UnitySynchronizationContext = SynchronizationContext.Current;
            UnityThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public static int UnityThreadId
        {
            get; private set;
        }

        public static SynchronizationContext UnitySynchronizationContext
        {
            get; private set;
        }
    }
}
