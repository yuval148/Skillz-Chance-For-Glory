using ElfKingdom;

namespace MyBot
{
    interface IStrategicCalculations
    {
        int ManaWasted { get; set; }

        void CalculateAll(Game game);
        void DoTurn(Game game);
        bool Equals(object obj);
        int GetHashCode();
        string ToString();
    }
}