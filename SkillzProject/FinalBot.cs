using ElfKingdom;

namespace MyBot
{
    class FinalBot : ISkillzBot
    {
        int manaWasted = 0;
        ElfCommands elfCommands = new ElfCommands();
        PortalCommands portalCommands = new PortalCommands();
        public void DoTurn(Game game)
        {
            int startingMana = game.GetMyMana();
            Command(elfCommands, game);
            Command(portalCommands, game);
            manaWasted += startingMana - game.GetMyMana();
        }
        void Command(StrategicCalculations toCommand, Game game)
        {
            toCommand.manaWasted = manaWasted;
            toCommand.CalculateAll(game);
            toCommand.DoTurn(game);
        }
    }
}
