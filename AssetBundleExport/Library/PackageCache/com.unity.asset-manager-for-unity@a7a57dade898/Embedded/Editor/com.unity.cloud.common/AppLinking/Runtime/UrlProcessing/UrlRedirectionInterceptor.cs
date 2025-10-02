using AOT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cloud.CommonEmbedded;
using Unity.Cloud.CommonEmbedded.Runtime;
using UnityEngine;

namespace Unity.Cloud.AppLinkingEmbedded.Runtime
{
    /// <summary>
    /// This <see cref="IUrlRedirectionInterceptor"/> implementation handles url
    /// redirection interception and validation of awaited query arguments.
    /// </summary>
    class UrlRedirectionInterceptor : IUrlRedirectionInterceptor
    {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern uint ShellExecute(IntPtr hwnd, string strOperation, string strFile, string strParameters, string strDirectory, Int32 nShowCmd);

        [DllImport("User32.dll")]
        static extern bool SetForegroundWindow(IntPtr handle);

        [DllImport("User32.dll")]
        static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        static extern IntPtr SetFocus(HandleRef hWnd);

        [DllImport("User32.dll")]
        static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string className, string windowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        static readonly string k_MainWindowTitle = Application.productName;

        const uint NAME_CHANGE_EVENT = 0x800C;
        const int SW_RESTORE = 9;

        static IntPtr m_MessageHook = IntPtr.Zero;
        static IntPtr m_MainWindowPtr;

        static WinEventDelegate winEventDelegate = new WinEventDelegate(WinEventProc);
#endif

        static readonly UCLogger s_Logger = LoggerProvider.GetLogger<UrlRedirectionInterceptor>();

        static IUrlRedirectionInterceptor s_UrlRedirectionInterceptorInstance;
        bool m_DisposedValue;
        readonly IUrlRedirectAwaiter m_Awaiter;
        readonly string m_HostDomain;

        /// <inheritdoc />
        public event Action<Uri> DeepLinkForwarded;

        /// <inheritdoc />
        public List<string> AwaitedQueryArguments { get; internal set; }

        /// <summary>
        /// Creates a <see cref="UrlRedirectionInterceptor"/> that handles url redirection interception and validation of awaited query arguments.
        /// </summary>
        /// <param name="hostDomain">Optional string value of current host domain.</param>
        /// <param name="refreshDelay">The interval in ms to refresh while awaiting for a redirection</param>
        /// <param name="timeoutDelay">The timeout in ms before timing out while awaiting for a redirection</param>
        internal UrlRedirectionInterceptor(string hostDomain = null, int refreshDelay = 500, int timeoutDelay = 600000)
            : this(new AsyncUrlRedirectAwaiter(refreshDelay, timeoutDelay), hostDomain)
        { }

        internal UrlRedirectionInterceptor(IUrlRedirectAwaiter awaiter, string hostDomain = null)
        {
            m_Awaiter = awaiter;
            m_HostDomain = hostDomain;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            AddRegistryKeys();
            m_MainWindowPtr = GetWindowHandle();
            // Set Hook once to listen to window title name changes
            if (m_MessageHook == IntPtr.Zero)
            {
                m_MessageHook = SetWinEventHook(NAME_CHANGE_EVENT, NAME_CHANGE_EVENT, IntPtr.Zero, winEventDelegate, 0, 0, 0);
            }
#else
            Application.deepLinkActivated += OnDeepLinkActivated;
#endif
        }

        void OnDeepLinkActivated(string url)
        {
            InterceptAwaitedUrl(url);
        }

        /// <inheritdoc />
        /// <remarks>Only used for windows applications.</remarks>
        public ProcessId GetRedirectProcessId()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return new ProcessId($"{m_MainWindowPtr}");
#else
            return ProcessId.None;
#endif
        }

        /// <summary>
        /// Gets or creates the static instance of <see cref="IUrlRedirectionInterceptor"/>.
        /// </summary>
        /// <returns>The static instance of <see cref="IUrlRedirectionInterceptor"/>.</returns>
        public static IUrlRedirectionInterceptor GetInstance()
        {
            if (s_UrlRedirectionInterceptorInstance == null)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                if (Uri.TryCreate(Application.absoluteURL, UriKind.Absolute, out Uri activationUri))
                {
                    s_UrlRedirectionInterceptorInstance = new UrlRedirectionInterceptor(activationUri.Host);
                }
                else
                {
                    s_UrlRedirectionInterceptorInstance = new UrlRedirectionInterceptor();
                }
#else
                s_UrlRedirectionInterceptorInstance = new UrlRedirectionInterceptor();
#endif
            }
            return s_UrlRedirectionInterceptorInstance;
        }


        /// <inheritdoc />
        public void InterceptAwaitedUrl(string url, List<string> awaitedQueryArguments = null)
        {
            UrlRedirectUtils.ValidateUrlArgument(url, out Uri uri);

            s_Logger.LogDebug($"InterceptAwaitedUrl: '{url}'");
            UnitySynchronizationContextGrabber.s_UnitySynchronizationContext.Post(_ =>
            {
                if (!TryInterceptRedirectionUrl(uri, awaitedQueryArguments))
                {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                    BringUnityAppProcessToFront();
#endif
                    DeepLinkForwarded?.Invoke(uri);
                }

            }, null);
        }

        /// <inheritdoc />
        public UrlRedirectResult? GetRedirectionResult()
        {
            return m_Awaiter.RedirectResult;
        }

        /// <inheritdoc />
        public async Task<UrlRedirectResult> AwaitRedirectAsync(List<string> awaitedQueryArguments = null)
        {
            AwaitedQueryArguments = awaitedQueryArguments;

            // Do a backflip now and await response from external process
            m_Awaiter.BeginWait();

            while (!m_Awaiter.RedirectResult.HasValue)
            {
                if (m_Awaiter.HasTimedOut)
                    throw new TimeoutException($"Redirect timed out after {m_Awaiter.TimeoutDelay / 1000f} seconds");

                await m_Awaiter.WaitForRefreshAsync();
            }

            return m_Awaiter.RedirectResult.Value;
        }

        bool TryInterceptRedirectionUrl(Uri uri, List<string> awaitedQueryArguments = null)
        {
            if (awaitedQueryArguments != null)
            {
                AwaitedQueryArguments = awaitedQueryArguments;
            }

            var queryArgs = QueryArgumentsParser.GetDictionaryFromArguments(uri);
            if (AwaitedQueryArguments != null && UrlRedirectUtils.UrlHasAwaitedQueryArguments(queryArgs, AwaitedQueryArguments))
            {
                var redirectResult = new UrlRedirectResult
                {
                    Status = UrlRedirectStatus.Success,
                    QueryArguments = queryArgs
                };

                // Only WebGL is hosted and could have callback login query arguments in its url.
                if (!string.IsNullOrEmpty(m_HostDomain))
                {
                    s_Logger.LogDebug($"Hosted app on '{m_HostDomain}' received callback url redirection.");
                    m_Awaiter.SetResult(redirectResult);
                }
                else
                {
                    PostResult(redirectResult);
                }

                return true;
            }

            return false;
        }

        void PostResult(UrlRedirectResult redirectResult)
        {
            // Post the result on the original thread
            UnitySynchronizationContextGrabber.s_UnitySynchronizationContext.Post(_ =>
            {
                m_Awaiter.SetResult(redirectResult);
            }, null);
        }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [MonoPInvokeCallback(typeof(UrlRedirectionInterceptor))]
        static void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            // Skip anything not related to hwnd window changes
            if (idObject != 0 || idChild != 0)
            {
                return;
            }
            // Only process changes in current main window
            if (m_MainWindowPtr == hwnd)
            {
                // Get the string value of the name change
                var length = GetWindowTextLength(m_MainWindowPtr);
                StringBuilder sb = new StringBuilder(length + 1);

                GetWindowText(m_MainWindowPtr, sb, sb.Capacity);
                var windowTitle = sb.ToString();
                // Only process change that differs from Interop expected Window title
                if (!windowTitle.Equals(k_MainWindowTitle))
                {
                    // Try process message as deeplink
                    s_UrlRedirectionInterceptorInstance.InterceptAwaitedUrl(windowTitle);

                    // Sets back the Window title to
                    SetWindowText(m_MainWindowPtr, k_MainWindowTitle);
                }
            }
        }

        static IntPtr GetWindowHandle()
        {
            // "UnityWndClass" is the string value returned when invoking user32.dll GetClassName function
            IntPtr hWnd = FindWindow("UnityWndClass", Application.productName);
            IntPtr ActivehWnd = GetActiveWindow();
            if (hWnd != IntPtr.Zero)
            {
                return hWnd;
            }
            SetWindowText(ActivehWnd, Application.productName);

            return ActivehWnd;
        }

        string GetResolverPath()
        {
            var appDomainLocation = Application.dataPath.Replace("/", "\\");
            var subPath = appDomainLocation.Substring(0, appDomainLocation.LastIndexOf("_Data"));
            var lastFolderIndex = subPath.LastIndexOf("\\");
            var exePathRoot = subPath.Substring(0, lastFolderIndex + 1);
            var exeAppName = subPath.Substring(lastFolderIndex + 1);
            return $"{exePathRoot}Unity_Cloud_Interop\\{exeAppName}.exe";
        }

        void AddRegistryKeys()
        {
            var resolverLocation = GetResolverPath();
            var uniqueCustomUriCommand = $"register {UnityCloudPlayerSettings.Instance.AppNamespace}";
#if ENABLE_MONO
            Process.Start(new ProcessStartInfo(resolverLocation, uniqueCustomUriCommand));
#else
            uint ret = ShellExecute((IntPtr)0, string.Empty, resolverLocation, uniqueCustomUriCommand, string.Empty, 0);
#endif
        }

        static void BringUnityAppProcessToFront()
        {
            var processHandle = GetWindowHandle();
            if (IsIconic(processHandle))
            {
                ShowWindow(processHandle, SW_RESTORE);
            }
            SetForegroundWindow(processHandle);
            SetFocus(new HandleRef(null, processHandle));
        }

#endif

        /// <summary>
        /// Performs tasks related to disposing of associated resources.
        /// </summary>
        /// <param name="disposing">Whether we want to .</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                    UnhookWinEvent(m_MessageHook);
#else
                    Application.deepLinkActivated -= OnDeepLinkActivated;
#endif
                }
                m_DisposedValue = true;
            }
        }

        /// <summary>
        /// Performs tasks related to disposing of associated resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
