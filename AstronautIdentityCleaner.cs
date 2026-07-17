using NLog;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Torch.API;
using VRage.Game.ModAPI;

namespace BuiltBy
{
    public sealed class AstronautIdentityCleaner
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private static readonly HashSet<string> AllowedModels = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "NPC_Astronaut",
            "NPC_Astronaut_Female"
        };

        private readonly ITorchBase _torch;
        private readonly List<IMyIdentity> _identities = new List<IMyIdentity>();
        private AstronautCleanupConfig _config;
        private DateTime _startTimeUtc;
        private DateTime _lastAutoCleanupAttemptUtc = DateTime.MinValue;
        private bool _autoCleanupDone;
        private bool _deleteInProgress;

        public AstronautIdentityCleaner(ITorchBase torch, AstronautCleanupConfig config)
        {
            _torch = torch;
            _config = AstronautCleanupConfig.Normalize(config);
            _startTimeUtc = DateTime.UtcNow;
        }

        public void UpdateConfig(AstronautCleanupConfig config)
        {
            _config = AstronautCleanupConfig.Normalize(config);
            _autoCleanupDone = false;
            _startTimeUtc = DateTime.UtcNow;
        }

        public void Update()
        {
            if (_autoCleanupDone || _deleteInProgress || !_config.AutoCleanupEnabled)
                return;

            if ((DateTime.UtcNow - _startTimeUtc).TotalMinutes < _config.AutoCleanupDelayMinutes)
                return;

            if ((DateTime.UtcNow - _lastAutoCleanupAttemptUtc).TotalSeconds < _config.AutoCleanupIntervalSeconds)
                return;

            _lastAutoCleanupAttemptUtc = DateTime.UtcNow;
            AstronautCleanupResult result = DeleteMatches(BuiltByText.Ukrainian);
            if (result.Success)
            {
                Log.Info("Astronaut Identity auto cleanup completed: found {0}, deleted {1}.", result.Found, result.Deleted);
                _autoCleanupDone = result.Found <= result.Deleted || result.Found == 0 || result.Deleted == 0;
            }
            else
            {
                Log.Warn("Astronaut Identity auto cleanup failed: {0}", result.Message);
                _autoCleanupDone = true;
            }
        }

        public AstronautCleanupResult Scan(string language)
        {
            List<IMyIdentity> matches = FindMatches();
            return AstronautCleanupResult.Ok(matches.Count, 0,
                string.Format(BuiltByText.Get(language, "ScanResult"), matches.Count));
        }

        public AstronautCleanupResult DeleteMatches(string language)
        {
            if (_deleteInProgress)
                return AstronautCleanupResult.Failed(BuiltByText.Get(language, "DeleteBusy"));

            _deleteInProgress = true;
            try
            {
                List<IMyIdentity> matches = FindMatches();
                int deleted = 0;
                foreach (IMyIdentity identity in matches)
                {
                    if (deleted >= _config.MaxDeletePerRun)
                        break;

                    if (RemoveIdentity(identity))
                        deleted++;
                }

                string message = matches.Count > deleted
                    ? string.Format(BuiltByText.Get(language, "DeleteResultLimited"), matches.Count, deleted)
                    : string.Format(BuiltByText.Get(language, "DeleteResult"), matches.Count, deleted);

                return AstronautCleanupResult.Ok(matches.Count, deleted, message);
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Astronaut Identity cleanup failed.");
                return AstronautCleanupResult.Failed(
                    string.Format(BuiltByText.Get(language, "DeleteError"), exception.Message));
            }
            finally
            {
                _deleteInProgress = false;
            }
        }

        private List<IMyIdentity> FindMatches()
        {
            _identities.Clear();
            if (MyAPIGateway.Session == null || MyAPIGateway.Players == null || _config.DisplayNames.Count == 0)
                return new List<IMyIdentity>();

            MyAPIGateway.Players.GetAllIdentites(_identities, IsTargetIdentity);
            return new List<IMyIdentity>(_identities);
        }

        private bool IsTargetIdentity(IMyIdentity identity)
        {
            if (identity == null)
                return false;

            if (!_config.SafeModeEnabled)
                return _config.DisplayNames.Contains(identity.DisplayName);

            if (!AllowedModels.Contains(identity.Model))
                return false;

            if (MyAPIGateway.Players.TryGetSteamId(identity.IdentityId) != 0)
                return false;

            return true;
        }

        private bool RemoveIdentity(IMyIdentity identity)
        {
            object players = MySession.Static == null ? null : MySession.Static.Players;
            if (players == null)
                return false;

            MethodInfo method = FindRemoveIdentityMethod(players.GetType());
            if (method == null)
            {
                Log.Warn("Cannot remove Astronaut Identity '{0}' ({1}): remove method was not found.",
                    identity.DisplayName, identity.IdentityId);
                return false;
            }

            object[] args = BuildRemoveIdentityArgs(method, identity.IdentityId);
            method.Invoke(players, args);
            Log.Info("Removed Astronaut Identity '{0}' ({1}), model {2}.",
                identity.DisplayName, identity.IdentityId, identity.Model);
            return true;
        }

        private static MethodInfo FindRemoveIdentityMethod(Type playersType)
        {
            MethodInfo[] methods = playersType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                if (!method.Name.Equals("RemoveIdentity", StringComparison.OrdinalIgnoreCase))
                    continue;

                ParameterInfo[] parameters = method.GetParameters();
                if (parameters.Length == 0)
                    continue;

                if (parameters[0].ParameterType == typeof(long))
                    return method;
            }

            return null;
        }

        private static object[] BuildRemoveIdentityArgs(MethodInfo method, long identityId)
        {
            ParameterInfo[] parameters = method.GetParameters();
            object[] args = new object[parameters.Length];
            args[0] = identityId;

            for (int i = 1; i < parameters.Length; i++)
            {
                Type type = parameters[i].ParameterType;
                if (type == typeof(bool))
                    args[i] = true;
                else if (type.IsValueType)
                    args[i] = Activator.CreateInstance(type);
                else
                    args[i] = null;
            }

            return args;
        }
    }

    [Serializable]
    public sealed class AstronautCleanupConfig
    {
        public List<string> DisplayNames { get; set; }
        public bool AutoCleanupEnabled { get; set; }
        public bool SafeModeEnabled { get; set; }
        public int AutoCleanupDelayMinutes { get; set; }
        public int AutoCleanupIntervalSeconds { get; set; }
        public int MaxDeletePerRun { get; set; }

        public AstronautCleanupConfig()
        {
            DisplayNames = new List<string>();
            SafeModeEnabled = true;
            AutoCleanupDelayMinutes = 5;
            AutoCleanupIntervalSeconds = 30;
            MaxDeletePerRun = 500;
        }

        public static AstronautCleanupConfig CreateDefault()
        {
            AstronautCleanupConfig config = new AstronautCleanupConfig();
            config.DisplayNames.Add("Astronaut");
            config.DisplayNames.Add("Female Astronaut");
            config.AutoCleanupEnabled = false;
            config.SafeModeEnabled = true;
            config.AutoCleanupDelayMinutes = 5;
            config.AutoCleanupIntervalSeconds = 30;
            config.MaxDeletePerRun = 500;
            return config;
        }

        public AstronautCleanupConfig Clone()
        {
            return new AstronautCleanupConfig
            {
                DisplayNames = new List<string>(DisplayNames ?? new List<string>()),
                AutoCleanupEnabled = AutoCleanupEnabled,
                SafeModeEnabled = SafeModeEnabled,
                AutoCleanupDelayMinutes = AutoCleanupDelayMinutes,
                AutoCleanupIntervalSeconds = AutoCleanupIntervalSeconds,
                MaxDeletePerRun = MaxDeletePerRun
            };
        }

        public static AstronautCleanupConfig Normalize(AstronautCleanupConfig config)
        {
            if (config == null)
                return CreateDefault();

            config.DisplayNames = (config.DisplayNames ?? new List<string>())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name)
                .ToList();

            if (config.DisplayNames.Count == 0)
            {
                config.DisplayNames.Add("Astronaut");
                config.DisplayNames.Add("Female Astronaut");
            }

            if (config.AutoCleanupDelayMinutes < 1)
                config.AutoCleanupDelayMinutes = 5;

            if (config.AutoCleanupIntervalSeconds < 5)
                config.AutoCleanupIntervalSeconds = 30;

            if (config.MaxDeletePerRun < 1)
                config.MaxDeletePerRun = 500;

            return config;
        }
    }

    public sealed class AstronautCleanupResult
    {
        public bool Success { get; private set; }
        public int Found { get; private set; }
        public int Deleted { get; private set; }
        public string Message { get; private set; }

        public static AstronautCleanupResult Ok(int found, int deleted, string message)
        {
            return new AstronautCleanupResult
            {
                Success = true,
                Found = found,
                Deleted = deleted,
                Message = message
            };
        }

        public static AstronautCleanupResult Failed(string message)
        {
            return new AstronautCleanupResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
