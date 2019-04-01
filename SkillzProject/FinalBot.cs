using ElfKingdom;

namespace MyBot
{
    class FinalBot : ISkillzBot
    {
        public int ManaWasted { get; set; } = 0;
        internal ElfCommands ElfCommands { get; set; } = new ElfCommands();
        internal PortalCommands PortalCommands { get; set; } = new PortalCommands();

        public void DoTurn(Game game)
        {
            game.Debug("ChanceForGlory\nBoros is dead; long live Orzhov!!\n\nBanana binana is a nice banana. Codey Codcodey is a secret society.");
            int startingMana = game.GetMyMana();
            Command(ElfCommands, game);
            Command(PortalCommands, game);
            ManaWasted += startingMana - game.GetMyMana();
        }
        void Command(StrategicCalculations toCommand, Game game)
        {
            toCommand.ManaWasted = ManaWasted;
            toCommand.CalculateAll(game);
            toCommand.DoTurn(game);
        }
    }
}
