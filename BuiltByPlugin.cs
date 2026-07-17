using NLog;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Plugins;
using Torch.Managers.PatchManager;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace BuiltBy
{
    public sealed class Plugin : TorchPluginBase, IWpfPlugin
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static HashSet<string> _spawnFactionTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<IMyEntity> _entities = new HashSet<IMyEntity>();
        private readonly List<IMyCubeGrid> _mechanicalGroup = new List<IMyCubeGrid>();
        private readonly List<IMySlimBlock> _blocks = new List<IMySlimBlock>();
        private readonly List<IMyIdentity> _identities = new List<IMyIdentity>();
        private PatchManager _patchManager;
        private PatchContext _patchContext;
        private PluginConfig _config;
        private AstronautIdentityCleaner _astronautCleaner;
        private string _configPath;

        public override void Init(ITorchBase torch)
        {
            base.Init(torch);
            Directory.CreateDirectory(StoragePath);
            _configPath = Path.Combine(StoragePath, "BuiltBy.cfg");
            _config = LoadConfig();
            _config.Language = BuiltByText.NormalizeLanguage(_config.Language);
            _spawnFactionTags = new HashSet<string>(_config.FactionTags, StringComparer.OrdinalIgnoreCase);
            _astronautCleaner = new AstronautIdentityCleaner(torch, _config.AstronautCleanup);
            InstallSpawnPatch(torch);
            Log.Info("BuiltBy initialized. SetAuthorship faction tags: {0}", string.Join(", ", _spawnFactionTags));
        }

        public override void Update()
        {
            base.Update();
            if (_astronautCleaner != null)
                _astronautCleaner.Update();
        }

        public UserControl GetControl()
        {
            return new BuiltByControl(this);
        }

        public List<string> GetConfiguredFactionTags()
        {
            return new List<string>(_config.FactionTags);
        }

        public string GetLanguage()
        {
            return BuiltByText.NormalizeLanguage(_config.Language);
        }

        public void SaveLanguage(string language)
        {
            _config.Language = BuiltByText.NormalizeLanguage(language);
            SaveConfig(_config);
        }

        public string T(string key)
        {
            return BuiltByText.Get(GetLanguage(), key);
        }

        public void SaveFactionTags(IEnumerable<string> tags)
        {
            _config.FactionTags = NormalizeFactionTags(tags);
            SaveConfig(_config);
            _spawnFactionTags = new HashSet<string>(_config.FactionTags, StringComparer.OrdinalIgnoreCase);
        }

        public AstronautCleanupConfig GetAstronautCleanupConfig()
        {
            return _config.AstronautCleanup.Clone();
        }

        public void SaveAstronautCleanupConfig(AstronautCleanupConfig config)
        {
            _config.AstronautCleanup = AstronautCleanupConfig.Normalize(config);
            SaveConfig(_config);

            if (_astronautCleaner != null)
                _astronautCleaner.UpdateConfig(_config.AstronautCleanup);
        }

        public AstronautCleanupResult FindAstronautIdentityMatches()
        {
            if (_astronautCleaner == null)
                return AstronautCleanupResult.Failed(T("NoCleanup"));

            return _astronautCleaner.Scan(GetLanguage());
        }

        public AstronautCleanupResult DeleteAstronautIdentityMatches()
        {
            if (_astronautCleaner == null)
                return AstronautCleanupResult.Failed(T("NoCleanup"));

            return _astronautCleaner.DeleteMatches(GetLanguage());
        }

        public bool TransferStructureAuthorship(string gridName, string playerName, out string result)
        {
            if (MyAPIGateway.Session == null)
            {
                result = T("WorldNotLoaded");
                return false;
            }

            List<IMyCubeGrid> matches = FindGridsByExactName(gridName);
            if (matches.Count == 0)
            {
                result = string.Format(T("GridNotFound"), gridName);
                return false;
            }

            if (matches.Count > 1)
            {
                result = string.Format(T("MultipleGrids"), gridName);
                return false;
            }

            List<IMyIdentity> identities = FindIdentitiesByExactName(playerName);
            if (identities.Count == 0)
            {
                result = string.Format(T("PlayerNotFound"), playerName);
                return false;
            }

            if (identities.Count > 1)
            {
                result = string.Format(T("MultiplePlayers"), playerName);
                return false;
            }

            IMyIdentity identity = identities[0];
            if (MySession.Static.Players.IdentityIsNpc(identity.IdentityId))
            {
                result = T("NpcNotPlayer");
                return false;
            }

            int transferred = TransferConstruct(matches[0], identity.IdentityId);
            result = string.Format(T("ManualTransferResult"), transferred, gridName, identity.DisplayName);
            Log.Info("Manual BuiltBy transfer: grid '{0}' ({1}), player '{2}' ({3}), blocks {4}.",
                matches[0].DisplayName, matches[0].EntityId, identity.DisplayName, identity.IdentityId, transferred);
            return true;
        }

        private void InstallSpawnPatch(ITorchBase torch)
        {
            Type[] parameters =
            {
                typeof(List<MyCubeGrid>), typeof(string), typeof(Vector3D), typeof(Vector3), typeof(Vector3),
                typeof(Vector3), typeof(Vector3), typeof(string), typeof(string), typeof(SpawningOptions),
                typeof(long), typeof(bool), typeof(Stack<Action>)
            };

            MethodInfo target = typeof(MyPrefabManager).GetMethod(
                "SpawnPrefab",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                parameters,
                null);
            MethodInfo prefix = typeof(Plugin).GetMethod(
                "SpawnPrefabPrefix",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (target == null || prefix == null)
                throw new InvalidOperationException("BuiltBy could not locate the Space Engineers SpawnPrefab method.");

            _patchManager = torch.GetManager<PatchManager>();
            if (_patchManager == null)
                throw new InvalidOperationException("BuiltBy could not access the Torch PatchManager.");

            _patchContext = _patchManager.AcquireContext();
            _patchContext.GetPattern(target).Prefixes.Add(prefix);
        }

        private static bool SpawnPrefabPrefix(
            List<MyCubeGrid> resultList,
            string prefabName,
            Vector3D position,
            Vector3 forward,
            Vector3 up,
            Vector3 initialLinearVelocity,
            Vector3 initialAngularVelocity,
            string beaconName,
            string entityName,
            ref SpawningOptions spawningOptions,
            long ownerId,
            bool updateSync,
            Stack<Action> callbacks)
        {
            if (ownerId == 0 || MyAPIGateway.Session == null || _spawnFactionTags.Count == 0)
                return true;

            IMyFaction faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(ownerId);
            if (faction == null || faction.FounderId != ownerId || !_spawnFactionTags.Contains(faction.Tag))
                return true;

            spawningOptions |= SpawningOptions.SetAuthorship;
            spawningOptions |= SpawningOptions.SetNpcSpawnedGrid;
            Log.Info("Enabled vanilla SetAuthorship for prefab '{0}', faction {1}, founder {2}.",
                prefabName, faction.Tag, ownerId);
            return true;
        }

        private int TransferConstruct(IMyCubeGrid mainGrid, long identityId)
        {
            GetMechanicalGroup(mainGrid);
            int transferred = 0;

            foreach (IMyCubeGrid grid in _mechanicalGroup)
            {
                if (grid == null || grid.MarkedForClose)
                    continue;

                _blocks.Clear();
                grid.GetBlocks(_blocks);
                foreach (IMySlimBlock block in _blocks)
                {
                    if (block.BuiltBy == identityId)
                        continue;

                    MySlimBlock concreteBlock = block as MySlimBlock;
                    if (concreteBlock == null)
                        continue;

                    concreteBlock.TransferAuthorship(identityId);
                    transferred++;
                }

                MyCubeGrid concreteGrid = grid as MyCubeGrid;
                if (concreteGrid != null)
                    concreteGrid.IsNpcSpawnedGrid = false;
            }

            return transferred;
        }

        private void GetMechanicalGroup(IMyCubeGrid mainGrid)
        {
            _mechanicalGroup.Clear();
            MyAPIGateway.GridGroups.GetGroup(mainGrid, GridLinkTypeEnum.Mechanical, _mechanicalGroup);
            if (_mechanicalGroup.Count == 0)
                _mechanicalGroup.Add(mainGrid);
        }

        private List<IMyCubeGrid> FindGridsByExactName(string name)
        {
            List<IMyCubeGrid> result = new List<IMyCubeGrid>();
            _entities.Clear();
            MyAPIGateway.Entities.GetEntities(_entities, entity => entity is IMyCubeGrid);

            foreach (IMyEntity entity in _entities)
            {
                IMyCubeGrid grid = entity as IMyCubeGrid;
                if (grid != null && !grid.MarkedForClose && string.Equals(grid.DisplayName, name, StringComparison.OrdinalIgnoreCase))
                    result.Add(grid);
            }

            return result;
        }

        private List<IMyIdentity> FindIdentitiesByExactName(string name)
        {
            _identities.Clear();
            MyAPIGateway.Players.GetAllIdentites(_identities, identity =>
                string.Equals(identity.DisplayName, name, StringComparison.OrdinalIgnoreCase));
            return new List<IMyIdentity>(_identities);
        }

        private PluginConfig LoadConfig()
        {
            if (!File.Exists(_configPath))
            {
                PluginConfig defaultConfig = CreateDefaultConfig();
                SaveConfig(defaultConfig);
                return defaultConfig;
            }

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig));
                using (FileStream stream = File.OpenRead(_configPath))
                {
                    PluginConfig loaded = (PluginConfig)serializer.Deserialize(stream);
                    loaded.Language = BuiltByText.NormalizeLanguage(loaded.Language);
                    loaded.FactionTags = NormalizeFactionTags(loaded.FactionTags);
                    loaded.AstronautCleanup = AstronautCleanupConfig.Normalize(loaded.AstronautCleanup);
                    return loaded;
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to load BuiltBy configuration. Using defaults.");
                return CreateDefaultConfig();
            }
        }

        private static PluginConfig CreateDefaultConfig()
        {
            PluginConfig config = new PluginConfig();
            config.Language = BuiltByText.Ukrainian;
            config.FactionTags.Add("RTEC");
            config.AstronautCleanup = AstronautCleanupConfig.CreateDefault();
            return config;
        }

        private static List<string> NormalizeFactionTags(IEnumerable<string> tags)
        {
            if (tags == null)
                return new List<string>();

            return tags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Select(tag => tag.Trim().ToUpperInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(tag => tag)
                .ToList();
        }

        private void SaveConfig(PluginConfig config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PluginConfig));
            using (FileStream stream = File.Create(_configPath))
                serializer.Serialize(stream, config);
        }

        public override void Dispose()
        {
            if (_patchManager != null && _patchContext != null)
            {
                _patchManager.FreeContext(_patchContext);
                _patchContext = null;
            }

            _spawnFactionTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _entities.Clear();
            _mechanicalGroup.Clear();
            _blocks.Clear();
            _identities.Clear();
            _astronautCleaner = null;
            base.Dispose();
        }
    }

    [Serializable]
    public class PluginConfig
    {
        public List<string> FactionTags { get; set; }
        public AstronautCleanupConfig AstronautCleanup { get; set; }
        public string Language { get; set; }

        public PluginConfig()
        {
            Language = BuiltByText.Ukrainian;
            FactionTags = new List<string>();
            AstronautCleanup = AstronautCleanupConfig.CreateDefault();
        }
    }
}
