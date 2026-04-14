using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.Diagnostics
{
    [RegisterTypeInIl2Cpp]
    public class DiagnosticsDumper : MonoBehaviour
    {
        public DiagnosticsDumper(IntPtr ptr)
            : base(ptr) { }

        const int RingFrames = 120;
        const float HitchThresholdMs = 100f;
        const float HitchLogCooldownSec = 5f;
        const float TickIntervalSec = 5f;
        const float SlowMeanMs = 0.5f;
        const float SlowP99Ms = 2f;
        const KeyCode SnapshotHotkey = KeyCode.F10;

        static readonly Dictionary<Type, RingBuffer> modUpdateTicks =
            new Dictionary<Type, RingBuffer>();
        static readonly Dictionary<Type, long> modAllocBytes = new Dictionary<Type, long>();
        static readonly Dictionary<string, int> assetBundleCalls = new Dictionary<string, int>();
        static readonly long[] frameHistogram = new long[8];
        static readonly double[] histogramBoundsMs =
        {
            4,
            8,
            16,
            33,
            66,
            100,
            200,
            double.PositiveInfinity,
        };

        static int gen0Prev,
            gen1Prev,
            gen2Prev;
        static long heapPrev;
        static float lastHitchLogTime = -10f;
        static float lastTickTime;
        static float sceneLoadStartTime;
        static string sceneLoadName;

        ProfilerRecorder mainThreadRec;
        ProfilerRecorder behaviourUpdateRec;
        ProfilerRecorder gcAllocRec;
        ProfilerRecorder gcReservedRec;
        ProfilerRecorder sysUsedRec;
        ProfilerRecorder drawCallsRec;
        ProfilerRecorder setPassRec;
        ProfilerRecorder trianglesRec;
        ProfilerRecorder batchesRec;
        ProfilerRecorder physicsBodiesRec;

        HarmonyLib.Harmony harmony;

        public static void AttachIfEnabled()
        {
            if (!ModUI.ModSettings.Debug.Profiling.Value)
            {
                return;
            }
            var go = new GameObject { name = "Mod_DiagnosticsDumper" };
            UnityEngine.Object.DontDestroyOnLoad(go);
            go.AddComponent<DiagnosticsDumper>();
        }

        void Awake()
        {
            Main.logger_instance?.Msg("[Profiling] DiagnosticsDumper attached (F10 = snapshot)");
            try
            {
                StartRecorders();
                InstallProbes();
                LogHarmonyInventory();
                sceneLoadName = SceneManager.GetActiveScene().name;
                sceneLoadStartTime = Time.realtimeSinceStartup;
                gen0Prev = GC.CollectionCount(0);
                gen1Prev = GC.CollectionCount(1);
                gen2Prev = GC.CollectionCount(2);
                heapPrev = GC.GetTotalMemory(false);
                lastTickTime = Time.realtimeSinceStartup;
            }
            catch (Exception ex)
            {
                Main.logger_instance?.Error("[Profiling] init failed: " + ex);
            }
        }

        void OnDestroy()
        {
            try
            {
                harmony?.UnpatchAll(harmony.Id);
            }
            catch { }
            DisposeRecorder(ref mainThreadRec);
            DisposeRecorder(ref behaviourUpdateRec);
            DisposeRecorder(ref gcAllocRec);
            DisposeRecorder(ref gcReservedRec);
            DisposeRecorder(ref sysUsedRec);
            DisposeRecorder(ref drawCallsRec);
            DisposeRecorder(ref setPassRec);
            DisposeRecorder(ref trianglesRec);
            DisposeRecorder(ref batchesRec);
            DisposeRecorder(ref physicsBodiesRec);
        }

        void Update()
        {
            float dtMs = Time.unscaledDeltaTime * 1000f;
            RecordFrame(dtMs);
            CheckHitch(dtMs);
            CheckSceneChange();

            if (Input.GetKeyDown(SnapshotHotkey))
            {
                DumpSnapshot();
            }

            if (Time.realtimeSinceStartup - lastTickTime >= TickIntervalSec)
            {
                lastTickTime = Time.realtimeSinceStartup;
                LogTick();
            }
        }

        void StartRecorders()
        {
            mainThreadRec = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 60);
            behaviourUpdateRec = ProfilerRecorder.StartNew(
                ProfilerCategory.Scripts,
                "BehaviourUpdate",
                60
            );
            gcAllocRec = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC.Alloc", 60);
            gcReservedRec = ProfilerRecorder.StartNew(
                ProfilerCategory.Memory,
                "GC Reserved Memory"
            );
            sysUsedRec = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
            drawCallsRec = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            setPassRec = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            trianglesRec = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            batchesRec = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            physicsBodiesRec = ProfilerRecorder.StartNew(
                ProfilerCategory.Physics,
                "Active Dynamic Bodies"
            );
            LogRecorderValidity();
        }

        void LogRecorderValidity()
        {
            string Tag(string n, ProfilerRecorder r) => r.Valid ? n : n + "[X]";
            Main.logger_instance?.Msg(
                "[Profiling] recorders: "
                    + Tag("MainThread", mainThreadRec)
                    + " "
                    + Tag("BehaviourUpdate", behaviourUpdateRec)
                    + " "
                    + Tag("GC.Alloc", gcAllocRec)
                    + " "
                    + Tag("GCReserved", gcReservedRec)
                    + " "
                    + Tag("SysUsed", sysUsedRec)
                    + " "
                    + Tag("DrawCalls", drawCallsRec)
                    + " "
                    + Tag("SetPass", setPassRec)
                    + " "
                    + Tag("Triangles", trianglesRec)
                    + " "
                    + Tag("Batches", batchesRec)
                    + " "
                    + Tag("PhysicsBodies", physicsBodiesRec)
                    + "  ([X] = stripped from release Player)"
            );
        }

        void InstallProbes()
        {
            harmony = new HarmonyLib.Harmony("LastEpoch_Hud.Diagnostics");

            var prefix = AccessTools.Method(typeof(ModUpdateProbe), nameof(ModUpdateProbe.Prefix));
            var postfix = AccessTools.Method(
                typeof(ModUpdateProbe),
                nameof(ModUpdateProbe.Postfix)
            );
            int patched = 0;
            var patchedTypes = new HashSet<Type>();
            foreach (var t in EnumerateModBehaviours())
            {
                foreach (var name in new[] { "Update", "FixedUpdate", "LateUpdate" })
                {
                    var m = t.GetMethod(
                        name,
                        BindingFlags.Public
                            | BindingFlags.NonPublic
                            | BindingFlags.Instance
                            | BindingFlags.DeclaredOnly
                    );
                    if (m == null)
                    {
                        continue;
                    }
                    try
                    {
                        harmony.Patch(
                            m,
                            prefix: new HarmonyMethod(prefix),
                            postfix: new HarmonyMethod(postfix)
                        );
                        patched++;
                        patchedTypes.Add(t);
                    }
                    catch (Exception ex)
                    {
                        Main.logger_instance?.Warning(
                            $"[Profiling] probe attach failed for {t.Name}.{name}: {ex.Message}"
                        );
                    }
                }
            }
            Main.logger_instance?.Msg(
                $"[Profiling] mod-update probes attached: {patched} methods on {patchedTypes.Count} types"
            );

            if (ModUI.ModSettings.Debug.ProfileGameTypes.Value)
            {
                AttachGameTypeProbes(prefix, postfix);
            }

            MelonCoroutines.Start(AttachAssetBundleProbeDelayed());
        }

        void AttachGameTypeProbes(MethodInfo prefix, MethodInfo postfix)
        {
            var gameSuspects = new[]
            {
                typeof(Il2Cpp.UIBase),
                typeof(Il2Cpp.EpochInputManager),
                typeof(Il2Cpp.PlayerSpawnManager),
                typeof(Il2Cpp.PlayerHealth),
                typeof(Il2Cpp.HealthPotion),
                typeof(Il2Cpp.GroundItemManager),
                typeof(Il2Cpp.ItemContainersManager),
                typeof(Il2Cpp.GoldTracker),
                typeof(Il2Cpp.ExperienceTracker),
                typeof(Il2Cpp.AbilityManager),
                typeof(Il2Cpp.SummonTracker),
                typeof(Il2Cpp.CameraManager),
                typeof(Il2Cpp.MapPanel),
                typeof(Il2Cpp.InventoryPanelUI),
                typeof(Il2Cpp.StashPanelUI),
                typeof(Il2Cpp.MovingPlayer),
            };
            int patched = 0;
            foreach (var t in gameSuspects)
            {
                foreach (var name in new[] { "Update", "FixedUpdate", "LateUpdate" })
                {
                    var m = AccessTools.DeclaredMethod(t, name);
                    if (m == null)
                    {
                        continue;
                    }
                    try
                    {
                        harmony.Patch(
                            m,
                            prefix: new HarmonyMethod(prefix),
                            postfix: new HarmonyMethod(postfix)
                        );
                        patched++;
                    }
                    catch (Exception ex)
                    {
                        Main.logger_instance?.Warning(
                            $"[Profiling] game-type probe attach failed for {t.Name}.{name}: {ex.Message}"
                        );
                    }
                }
            }
            Main.logger_instance?.Msg(
                $"[Profiling] game-type probes attached: {patched} methods on {gameSuspects.Length} suspect types"
            );
        }

        System.Collections.IEnumerator AttachAssetBundleProbeDelayed()
        {
            // Wait for UniverseLib (UnityExplorer dependency) to finish its own AssetBundle
            yield return new WaitForSecondsRealtime(15f);
            try
            {
                var getAllNames = AccessTools.Method(typeof(AssetBundle), "GetAllAssetNames");
                if (getAllNames != null)
                {
                    harmony.Patch(
                        getAllNames,
                        postfix: new HarmonyMethod(
                            typeof(AssetBundleProbe),
                            nameof(AssetBundleProbe.GetAllAssetNamesPostfix)
                        )
                    );
                    Main.logger_instance?.Msg(
                        "[Profiling] AssetBundle.GetAllAssetNames probe attached (deferred)"
                    );
                }
            }
            catch (Exception ex)
            {
                Main.logger_instance?.Warning(
                    "[Profiling] deferred AssetBundle probe attach failed: " + ex.Message
                );
            }
        }

        static IEnumerable<Type> EnumerateModBehaviours()
        {
            Type[] types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray();
            }

            foreach (var t in types)
            {
                if (t == typeof(DiagnosticsDumper))
                {
                    continue;
                }
                if (!typeof(MonoBehaviour).IsAssignableFrom(t))
                {
                    continue;
                }
                if (t.Assembly != Assembly.GetExecutingAssembly())
                {
                    continue;
                }
                yield return t;
            }
        }

        void LogHarmonyInventory()
        {
            try
            {
                var all = HarmonyLib.Harmony.GetAllPatchedMethods().ToList();
                var byOwner = new Dictionary<string, int>();
                foreach (var m in all)
                {
                    var info = HarmonyLib.Harmony.GetPatchInfo(m);
                    foreach (
                        var p in info
                            .Prefixes.Concat(info.Postfixes)
                            .Concat(info.Transpilers)
                            .Concat(info.Finalizers)
                    )
                    {
                        if (!byOwner.ContainsKey(p.owner))
                        {
                            byOwner[p.owner] = 0;
                        }
                        byOwner[p.owner]++;
                    }
                }
                Main.logger_instance?.Msg(
                    $"[Profiling] Harmony inventory: {all.Count} methods patched across {byOwner.Count} owners"
                );
                foreach (var kv in byOwner.OrderByDescending(k => k.Value).Take(10))
                {
                    Main.logger_instance?.Msg($"[Profiling]   {kv.Value, 4}× by {kv.Key}");
                }
            }
            catch (Exception ex)
            {
                Main.logger_instance?.Warning(
                    "[Profiling] Harmony inventory failed: " + ex.Message
                );
            }
        }

        void RecordFrame(float dtMs)
        {
            for (int i = 0; i < histogramBoundsMs.Length; i++)
            {
                if (dtMs <= histogramBoundsMs[i])
                {
                    frameHistogram[i]++;
                    return;
                }
            }
        }

        void CheckHitch(float dtMs)
        {
            if (dtMs < HitchThresholdMs)
            {
                return;
            }
            if (Time.realtimeSinceStartup - lastHitchLogTime < HitchLogCooldownSec)
            {
                return;
            }
            lastHitchLogTime = Time.realtimeSinceStartup;
            Main.logger_instance?.Warning(
                $"[Profiling] HITCH frame={Time.frameCount} dt={dtMs:F0}ms scene={SceneManager.GetActiveScene().name}"
            );
        }

        void LogTick()
        {
            int gen0 = GC.CollectionCount(0);
            int gen1 = GC.CollectionCount(1);
            int gen2 = GC.CollectionCount(2);
            long heap = GC.GetTotalMemory(false);
            long heapDelta = heap - heapPrev;
            int g0d = gen0 - gen0Prev;
            int g1d = gen1 - gen1Prev;
            int g2d = gen2 - gen2Prev;
            gen0Prev = gen0;
            gen1Prev = gen1;
            gen2Prev = gen2;
            heapPrev = heap;

            double mtMs = NsToMs(MeanRecorder(mainThreadRec));
            double buMs = NsToMs(MeanRecorder(behaviourUpdateRec));
            long allocPerFrame = MeanRecorder(gcAllocRec);
            long sysUsedMb = ReadRecorder(sysUsedRec) / (1024 * 1024);
            long gcResMb = ReadRecorder(gcReservedRec) / (1024 * 1024);
            long dc = ReadRecorder(drawCallsRec);
            long sp = ReadRecorder(setPassRec);
            long tri = ReadRecorder(trianglesRec);
            long bat = ReadRecorder(batchesRec);
            long phys = ReadRecorder(physicsBodiesRec);

            double modSumMs = 0;
            foreach (var rb in modUpdateTicks.Values)
            {
                modSumMs += TicksToMs(rb.Mean());
            }
            string scriptsBlock = behaviourUpdateRec.Valid
                ? $"Scripts={buMs:F2}ms (mods={modSumMs:F2}ms game={Math.Max(0, buMs - modSumMs):F2}ms)"
                : $"mods={modSumMs:F2}ms";
            string allocBlock = gcAllocRec.Valid ? $"alloc={allocPerFrame}B" : "";

            Main.logger_instance?.Msg(
                $"[Profiling] /frame: MT={mtMs:F2}ms {scriptsBlock} {allocBlock} "
                    + $"| 5s: gen0+={g0d} gen1+={g1d} gen2+={g2d} heap={heap / (1024 * 1024)}MB({heapDelta / 1024:+#;-#;0}KB) sys={sysUsedMb}MB gcRes={gcResMb}MB "
                    + $"| frame: DC={dc} SetPass={sp} Tri={tri} Bat={bat}"
            );

            LogSlowMods();
            LogAssetBundleSpam();
        }

        void LogSlowMods()
        {
            var slow = new List<(Type t, double mean, double p99, long alloc)>();
            foreach (var kv in modUpdateTicks)
            {
                double mean = TicksToMs(kv.Value.Mean());
                double p99 = TicksToMs(kv.Value.P99());
                if (mean < SlowMeanMs && p99 < SlowP99Ms)
                {
                    continue;
                }
                modAllocBytes.TryGetValue(kv.Key, out long alloc);
                slow.Add((kv.Key, mean, p99, alloc / Math.Max(1, kv.Value.Count)));
            }
            if (slow.Count == 0)
            {
                return;
            }
            slow.Sort((a, b) => b.p99.CompareTo(a.p99));
            foreach (var s in slow.Take(10))
            {
                Main.logger_instance?.Msg(
                    $"[Profiling]   slow {s.t.Name}: mean={s.mean:F2}ms p99={s.p99:F2}ms alloc={s.alloc}B/f"
                );
            }
        }

        void LogAssetBundleSpam()
        {
            if (assetBundleCalls.Count == 0)
            {
                return;
            }
            foreach (var kv in assetBundleCalls.OrderByDescending(k => k.Value).Take(5))
            {
                if (kv.Value < 10)
                {
                    break;
                }
                Main.logger_instance?.Msg(
                    $"[Profiling]   AssetBundle.{kv.Key}: {kv.Value} calls / {TickIntervalSec}s"
                );
            }
            assetBundleCalls.Clear();
        }

        void CheckSceneChange()
        {
            string current = SceneManager.GetActiveScene().name;
            if (current == sceneLoadName)
            {
                return;
            }
            float now = Time.realtimeSinceStartup;
            float ms = (now - sceneLoadStartTime) * 1000f;
            Main.logger_instance?.Msg(
                $"[Profiling] scene changed: '{sceneLoadName}' -> '{current}' after {ms:F0}ms"
            );
            sceneLoadName = current;
            sceneLoadStartTime = now;
        }

        void DumpSnapshot()
        {
            try
            {
                string ts = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string path = Path.Combine(
                    Application.dataPath,
                    "..",
                    "Mods",
                    Main.mod_name,
                    $"Diagnostics_{ts}.json"
                );
                var snap = BuildSnapshot();
                File.WriteAllText(path, JsonConvert.SerializeObject(snap, Formatting.Indented));
                Main.logger_instance?.Msg($"[Profiling] snapshot dumped: {path}");
                LogTopSlowMods(10);
            }
            catch (Exception ex)
            {
                Main.logger_instance?.Error("[Profiling] snapshot failed: " + ex);
            }
        }

        const int RepresentativesPerType = 10;
        const int TopTypesForRepresentatives = 30;
        const int DDOLSceneHandle = 0;

        static Dictionary<string, int> previousComponentCounts;
        static int missingScriptCount;

        Dictionary<string, object> BuildSnapshot()
        {
            var componentCounts = new Dictionary<string, int>();
            var componentActive = new Dictionary<string, int>();
            var componentInactive = new Dictionary<string, int>();
            var ddolCounts = new Dictionary<string, int>();
            var rootCounts = new Dictionary<string, int>();
            var representatives = new Dictionary<string, List<string>>();
            missingScriptCount = 0;
            try
            {
                var all = Resources.FindObjectsOfTypeAll<Component>();
                foreach (var obj in all)
                {
                    if (obj == null)
                    {
                        missingScriptCount++;
                        continue;
                    }
                    string n = obj.GetIl2CppType().FullName;
                    if (string.IsNullOrEmpty(n))
                    {
                        n = "?";
                    }
                    Inc(componentCounts, n);

                    bool ddol = false;
                    bool isActive = false;
                    string root = "?";
                    try
                    {
                        var go = obj.gameObject;
                        if (!go.IsNullOrDestroyed())
                        {
                            isActive = go.activeInHierarchy;
                            ddol =
                                go.scene.handle == DDOLSceneHandle
                                && go.scene.name == "DontDestroyOnLoad";
                            if (ddol)
                            {
                                Inc(ddolCounts, n);
                            }

                            var t = go.transform;
                            int depth = 0;
                            while (!t.parent.IsNullOrDestroyed() && depth < 32)
                            {
                                t = t.parent;
                                depth++;
                            }
                            root = (ddol ? "[DDOL]/" : (go.scene.name + "/")) + t.name;
                            Inc(rootCounts, root);
                        }
                    }
                    catch { }

                    if (isActive)
                    {
                        Inc(componentActive, n);
                    }
                    else
                    {
                        Inc(componentInactive, n);
                    }

                    if (!representatives.TryGetValue(n, out var reps))
                    {
                        reps = new List<string>(RepresentativesPerType);
                        representatives[n] = reps;
                    }
                    if (reps.Count < RepresentativesPerType)
                    {
                        reps.Add((ddol ? "[DDOL] " : "") + GetGameObjectPath(obj));
                    }
                }
            }
            catch (Exception ex)
            {
                componentCounts["__error__"] = 0;
                Main.logger_instance?.Warning("[Profiling] component scan failed: " + ex.Message);
            }

            var topTypes = componentCounts
                .OrderByDescending(k => k.Value)
                .Take(TopTypesForRepresentatives)
                .Select(k => k.Key)
                .ToHashSet();
            var topRepresentatives = new Dictionary<string, List<string>>();
            foreach (var t in topTypes)
            {
                if (representatives.TryGetValue(t, out var reps))
                {
                    topRepresentatives[t] = reps;
                }
            }

            var topSplit = new Dictionary<string, object>();
            foreach (var t in topTypes)
            {
                componentActive.TryGetValue(t, out int a);
                componentInactive.TryGetValue(t, out int i);
                topSplit[t] = new
                {
                    total = a + i,
                    active = a,
                    inactive = i,
                };
            }

            var deltas = new Dictionary<string, int>();
            if (previousComponentCounts != null)
            {
                var keys = new HashSet<string>(componentCounts.Keys);
                foreach (var k in previousComponentCounts.Keys)
                {
                    keys.Add(k);
                }
                foreach (var k in keys)
                {
                    componentCounts.TryGetValue(k, out int now);
                    previousComponentCounts.TryGetValue(k, out int prev);
                    int d = now - prev;
                    if (d != 0)
                    {
                        deltas[k] = d;
                    }
                }
            }
            previousComponentCounts = new Dictionary<string, int>(componentCounts);
            var topGrowing = deltas.OrderByDescending(k => Math.Abs(k.Value)).Take(20).ToList();
            var topDeltas = new Dictionary<string, int>();
            foreach (var kv in topGrowing)
            {
                topDeltas[kv.Key] = kv.Value;
            }

            var modTimings = new Dictionary<string, object>();
            foreach (var kv in modUpdateTicks)
            {
                modAllocBytes.TryGetValue(kv.Key, out long alloc);
                modTimings[kv.Key.FullName ?? kv.Key.Name] = new
                {
                    mean_ms = TicksToMs(kv.Value.Mean()),
                    p99_ms = TicksToMs(kv.Value.P99()),
                    samples = kv.Value.Count,
                    alloc_total_bytes = alloc,
                };
            }

            var histogram = new Dictionary<string, long>();
            for (int i = 0; i < frameHistogram.Length; i++)
            {
                string label =
                    i == frameHistogram.Length - 1
                        ? $">{histogramBoundsMs[i - 1]}ms"
                        : $"<={histogramBoundsMs[i]}ms";
                histogram[label] = frameHistogram[i];
            }

            return new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.Now.ToString("o"),
                ["frame"] = Time.frameCount,
                ["scene"] = SceneManager.GetActiveScene().name,
                ["frame_histogram"] = histogram,
                ["mod_timings"] = modTimings,
                ["component_counts_top30"] = TopByValue(componentCounts, 30),
                ["component_active_inactive_top30"] = topSplit,
                ["component_counts_ddol_top30"] = TopByValue(ddolCounts, 30),
                ["component_representatives_top30"] = topRepresentatives,
                ["component_count_delta_top20"] = topDeltas,
                ["owner_root_counts_top30"] = TopByValue(rootCounts, 30),
                ["missing_script_count"] = missingScriptCount,
                ["asset_bundle_loaded_count"] = AssetBundleNameCount(),
            };
        }

        static void Inc(Dictionary<string, int> d, string k)
        {
            if (!d.ContainsKey(k))
            {
                d[k] = 0;
            }
            d[k]++;
        }

        static string GetGameObjectPath(Component c)
        {
            try
            {
                var t = c.transform;
                if (t.IsNullOrDestroyed())
                {
                    return "?";
                }
                var sb = new System.Text.StringBuilder(t.name);
                var p = t.parent;
                int depth = 0;
                while (!p.IsNullOrDestroyed() && depth < 8)
                {
                    sb.Insert(0, p.name + "/");
                    p = p.parent;
                    depth++;
                }
                return sb.ToString();
            }
            catch
            {
                return "?";
            }
        }

        void LogTopSlowMods(int n)
        {
            if (modUpdateTicks.Count == 0)
            {
                Main.logger_instance?.Msg(
                    "[Profiling] no per-mod timings collected (probes failed?)"
                );
                return;
            }
            Main.logger_instance?.Msg($"[Profiling] top {n} mods by p99 update time:");
            var ordered = modUpdateTicks
                .Select(kv =>
                    (kv.Key, mean: TicksToMs(kv.Value.Mean()), p99: TicksToMs(kv.Value.P99()))
                )
                .OrderByDescending(x => x.p99)
                .Take(n);
            foreach (var x in ordered)
            {
                Main.logger_instance?.Msg(
                    $"[Profiling]   {x.Key.Name, -32} mean={x.mean, 6:F2}ms p99={x.p99, 6:F2}ms"
                );
            }
        }

        public static void RecordUpdate(Type t, long elapsedTicks, long allocBytes)
        {
            if (!modUpdateTicks.TryGetValue(t, out var rb))
            {
                rb = new RingBuffer(RingFrames);
                modUpdateTicks[t] = rb;
                modAllocBytes[t] = 0;
            }
            rb.Add(elapsedTicks);
            modAllocBytes[t] += allocBytes;
        }

        static Dictionary<string, int> TopByValue(Dictionary<string, int> src, int n)
        {
            var result = new Dictionary<string, int>();
            foreach (var kv in src.OrderByDescending(k => k.Value).Take(n))
            {
                result[kv.Key] = kv.Value;
            }
            return result;
        }

        static int AssetBundleNameCount()
        {
            if (Hud_Manager.asset_bundle.IsNullOrDestroyed())
            {
                return 0;
            }
            var names = Hud_Manager.asset_bundle.GetAllAssetNames();
            return names == null ? 0 : names.Count;
        }

        public static void RecordAssetBundleCall(string method)
        {
            if (!assetBundleCalls.ContainsKey(method))
            {
                assetBundleCalls[method] = 0;
            }
            assetBundleCalls[method]++;
        }

        static double TicksToMs(long ticks) => ticks * 1000.0 / Stopwatch.Frequency;

        static double NsToMs(long ns) => ns / 1_000_000.0;

        static long ReadRecorder(ProfilerRecorder r) => r.Valid ? r.LastValue : 0;

        static long MeanRecorder(ProfilerRecorder r)
        {
            if (!r.Valid || r.Capacity == 0)
            {
                return 0;
            }
            int n = r.Count;
            if (n == 0)
            {
                return 0;
            }
            long sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += r.GetSample(i).Value;
            }
            return sum / n;
        }

        static void DisposeRecorder(ref ProfilerRecorder r)
        {
            if (r.Valid)
            {
                r.Dispose();
            }
            r = default;
        }

        public class RingBuffer
        {
            readonly long[] data;
            int idx;
            public int Count { get; private set; }

            public RingBuffer(int capacity)
            {
                data = new long[capacity];
            }

            public void Add(long v)
            {
                data[idx] = v;
                idx = (idx + 1) % data.Length;
                if (Count < data.Length)
                {
                    Count++;
                }
            }

            public long Mean()
            {
                if (Count == 0)
                {
                    return 0;
                }
                long sum = 0;
                for (int i = 0; i < Count; i++)
                {
                    sum += data[i];
                }
                return sum / Count;
            }

            public long P99()
            {
                if (Count == 0)
                {
                    return 0;
                }
                var sorted = new long[Count];
                Array.Copy(data, sorted, Count);
                Array.Sort(sorted);
                int p = (int)(Count * 0.99);
                if (p >= Count)
                {
                    p = Count - 1;
                }
                return sorted[p];
            }
        }

        public struct ProbeState
        {
            public long ts;
            public long alloc;
        }

        public static class ModUpdateProbe
        {
            public static void Prefix(out ProbeState __state)
            {
                __state.ts = Stopwatch.GetTimestamp();
                __state.alloc = GC.GetAllocatedBytesForCurrentThread();
            }

            public static void Postfix(MonoBehaviour __instance, ProbeState __state)
            {
                if (__instance == null)
                {
                    return;
                }
                long elapsed = Stopwatch.GetTimestamp() - __state.ts;
                long alloc = GC.GetAllocatedBytesForCurrentThread() - __state.alloc;
                RecordUpdate(__instance.GetType(), elapsed, alloc);
            }
        }

        public static class AssetBundleProbe
        {
            public static void LoadAssetPostfix() => RecordAssetBundleCall("LoadAsset");

            public static void GetAllAssetNamesPostfix() =>
                RecordAssetBundleCall("GetAllAssetNames");
        }
    }
}
