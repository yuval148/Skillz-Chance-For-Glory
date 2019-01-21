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
