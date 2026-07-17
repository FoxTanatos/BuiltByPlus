using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace BuiltBy
{
    public class BuiltByCommands : CommandModule
    {
        [Command("builtby", "Передає авторство всіх блоків структури вказаному гравцю.")]
        [Permission(MyPromoteLevel.Admin)]
        public void TransferBuiltBy(string gridName, string playerName)
        {
            Plugin plugin = Context.Plugin as Plugin;
            if (plugin == null)
            {
                Context.Respond("Плагін BuiltBy недоступний.");
                return;
            }

            string result;
            plugin.TransferStructureAuthorship(gridName, playerName, out result);
            Context.Respond(result);
        }
    }
}
